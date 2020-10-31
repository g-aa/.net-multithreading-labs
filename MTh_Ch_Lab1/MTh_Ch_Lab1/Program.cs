using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            // исходные данные для работы:
            int expCount = 10; // число проводимых экспериментов
            int[] vectorSizes = { 10, 100, 1000, 100000 }; // длины исследуемых массивов
            int[] thredsCounts = { 1, 2, 4, 8 }; // число потоков для расчета

            string[] funcType = { "простая", "параметрируемая (k={0})", "неравномерная" };
            Func<double[], CalcTask.CalcParams, double[]>[] funcs = { L_calcFunction, P_calcFunction, H_calcFunction};

            string expLineFormat = "Результат:\tэксперимент - {0},\tчисло потоков - {1},\tчисло элементов - {2},\tтип функции - {3},\tвремя выполнения - {4} ms";
            string expMeanFormat = "Среднее:\tчисло потоков - {0},\tчисло элементов - {1},\tтип функции - {2},\tвремя выполнения - {3} ms\n";
            string headFormat = "Число физических ядер процессора:\t{0}\n";
            StringBuilder textReprt = new StringBuilder(); // для записи в файлик
            
            Console.WriteLine(string.Format(headFormat, Environment.ProcessorCount));
            textReprt.AppendLine(string.Format(headFormat, Environment.ProcessorCount));

            // задания 1, 2, 3, 4, 5:
            for (int f = 0; f < funcType.Length; f++)
            {
                int k = (f != 1) ? 0 : new Random().Next(2, 25); // для параметрируемой функции масштабный коэффициент k
                
                // обход векторов:
                for (int vs = 0; vs < vectorSizes.Length; vs++)
                {
                    double[] array_1D = Program.GetRandomVector(vectorSizes[vs]);
                    // обход потоков:
                    for (int tc = 0; tc < thredsCounts.Length; tc++)
                    {
                        // число проводимых испытаний:
                        double meanTime = 0; // среднее время выполнения
                        for (int expIdx = 0; expIdx < expCount; expIdx++)
                        {
                            CalcTask[] tasks = new CalcTask[thredsCounts[tc]];
                            Thread[] threads = new Thread[thredsCounts[tc]];

                            int startIdx = 0; // стартовый индекс для обработки в массиве
                            int idxStep = vectorSizes[vs] / thredsCounts[tc]; // приращение шага при обработки массива

                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            for (int idx = 0; idx < thredsCounts[tc]; idx++)
                            {
                                int length = (idx < thredsCounts[tc] - 1) ? idxStep : (array_1D.Length - startIdx);
                                tasks[idx] = new CalcTask(array_1D, new CalcTask.CalcParams() { StartIndex = startIdx, Length = length, K = k }, funcs[f]);
                                threads[idx] = new Thread(new ThreadStart(tasks[idx].Calculate));
                                startIdx += idxStep;
                                threads[idx].Start();
                            }
                            Array.ForEach(threads, (Thread thred) => { thred.Join(); });
                            sw.Stop();

                            meanTime += expIdx > 0 ? sw.Elapsed.TotalMilliseconds : 0;
                            string textLine = string.Format(expLineFormat, expIdx + 1, thredsCounts[tc], vectorSizes[vs], funcType[f], sw.Elapsed.TotalMilliseconds);

                            if (f == 1)
                            {
                                textLine = string.Format(textLine, k);
                            }
                                
                            Console.WriteLine(textLine);
                            textReprt.AppendLine(textLine);
                        }
                        Console.WriteLine(string.Format(expMeanFormat, thredsCounts[tc], vectorSizes[vs], funcType[f], (meanTime / (expCount - 1))));
                        textReprt.AppendLine(string.Format(expMeanFormat, thredsCounts[tc], vectorSizes[vs], funcType[f], (meanTime / (expCount - 1))));
                    }
                    Console.WriteLine();
                    textReprt.AppendLine();
                }
            }

            // задание 6:
            Console.WriteLine("Задание №6 эффективность параллелизма при круговом разделении элементов вектора:");
            textReprt.AppendLine("Задание №6 эффективность параллелизма при круговом разделении элементов вектора:");
            // обход векторов:
            for (int vs = 0; vs < vectorSizes.Length; vs++)
            {
                double[] array_1D = Program.GetRandomVector(vectorSizes[vs]);
                // обход потоков:
                for (int tc = 0; tc < thredsCounts.Length; tc++)
                {
                    double meanTime = 0; // среднее время выполнения
                    // число проводимых испытаний:
                    for (int expIdx = 0; expIdx < expCount; expIdx++)
                    {
                        CalcTask[] tasks = new CalcTask[thredsCounts[tc]];
                        Thread[] threads = new Thread[thredsCounts[tc]];

                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        for (int idx = 0; idx < thredsCounts[tc]; idx++)
                        {
                            tasks[idx] = new CalcTask(array_1D, new CalcTask.CalcParams() { StartIndex = idx, Length = array_1D.Length, K = thredsCounts[tc] }, H_calc6Function);
                            threads[idx] = new Thread(new ThreadStart(tasks[idx].Calculate));
                            threads[idx].Start();
                        }
                        Array.ForEach(threads, (Thread thred) => { thred.Join(); });
                        sw.Stop();

                        meanTime += expIdx > 0 ? sw.Elapsed.TotalMilliseconds : 0;
                        string textLine = string.Format(expLineFormat, expIdx + 1, thredsCounts[tc], vectorSizes[vs], funcType[2], sw.Elapsed.TotalMilliseconds);

                        Console.WriteLine(textLine);
                        textReprt.AppendLine(textLine);
                    }
                    Console.WriteLine(string.Format(expMeanFormat, thredsCounts[tc], vectorSizes[vs], funcType[2], (meanTime / (expCount - 1))));
                    textReprt.AppendLine(string.Format(expMeanFormat, thredsCounts[tc], vectorSizes[vs], funcType[2], (meanTime / (expCount- 1))));
                }
                Console.WriteLine();
                textReprt.AppendLine();
            }

            using (StreamWriter output = new StreamWriter(Path.Combine(Path.GetFullPath(@"..\..\"), "CalcReport.txt")))
            {
                output.WriteLine(textReprt.ToString());
            }
            Console.Write("Press any key...");
            Console.ReadKey();
        }


        /// <summary>
        /// Генератор одномерного массива с случайными значениями
        /// </summary>
        /// <param name="size">Длина вектора</param>
        /// <returns></returns>
        public static double[] GetRandomVector(int size)
        {
            double[] array = new double[size];
            Random random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.NextDouble();
            }
            return array;
        }


        /// <summary>
        /// Простая вычислительная функция
        /// </summary>
        /// <param name="array_1D"></param>
        /// <param name="calcParams"></param>
        /// <returns></returns>
        public static double[] L_calcFunction(double[] array_1D, CalcTask.CalcParams calcParams)
        {
            double[] result = new double[calcParams.Length];
            for (int i = 0; i < calcParams.Length; i++)
            {
                result[i] = Math.Pow(Math.E, array_1D[calcParams.StartIndex + i]);
            }
            return result;
        }


        /// <summary>
        /// Параметрируемая вычислительная функция
        /// </summary>
        /// <param name="array_1D"></param>
        /// <param name="calcParams"></param>
        /// <returns></returns>
        public static double[] P_calcFunction(double[] array_1D, CalcTask.CalcParams calcParams)
        {
            double[] result = new double[calcParams.Length];
            for (int i = 0; i < calcParams.Length; i++)
            {
                for (int j = 0; j < calcParams.K + i; j++)
                {
                    result[i] += Math.Pow(Math.E, array_1D[calcParams.StartIndex + i]);
                }
            }
            return result;
        }


        /// <summary>
        /// Сложная вычислительная функция
        /// </summary>
        /// <param name="array_1D"></param>
        /// <param name="calcParams"></param>
        /// <returns></returns>
        public static double[] H_calcFunction(double[] array_1D, CalcTask.CalcParams calcParams)
        {
            double[] result = new double[calcParams.Length];
            for (int i = 0; i < calcParams.Length; i++)
            {
                for (int j = 0; j < calcParams.StartIndex + i; j++)
                {
                    result[i] += Math.Pow(Math.E, array_1D[calcParams.StartIndex + i]);
                }
            }
            return result;
        }

        public static double[] H_calc6Function(double[] array_1D, CalcTask.CalcParams calcParams)
        {
            double[] result = new double[calcParams.Length];
            for (int i = calcParams.StartIndex; i < calcParams.Length; i += calcParams.K)
            {
                for (int j = 0; j < i; j++)
                {
                    result[i] += Math.Pow(Math.E, array_1D[i]);
                }
            }
            return result;
        }
    }
}