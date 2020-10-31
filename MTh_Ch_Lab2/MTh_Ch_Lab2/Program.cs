using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            // исходные данные для расчета:
            bool printAll = false;
            uint testsCount = 10;
            uint[] limitsNum = { 10_000, 1_000_000, 10_000_000 };
            uint[] threadsNum = { 2, 4, 8 };
            StringBuilder txtReport = new StringBuilder();


            // Задание №1 - исследование однопоточного расчета:
            txtReport.AppendLine(SingleThrProcessing("Алгоритм Эратосфена:", MyMath.EratosthenesAlgorithm, limitsNum, testsCount, printAll));

            txtReport.AppendLine(SingleThrProcessing("Модифицированный алгоритм Эратосфена:", MyMath.ModEratosthenesAlgorithm, limitsNum, testsCount, printAll));


            // Задание №2 - декомпозиция по набору данных:
            txtReport.AppendLine(MultiThrProcessing("Алгоритм №1 - декомпозиция по данным:", new Task_1Handler(limitsNum[0], threadsNum[0]), limitsNum, threadsNum, testsCount, printAll));


            // Задание №3 - декомпозиция набора простых чисел:
            txtReport.AppendLine(MultiThrProcessing("Алгоритм №2 - декомпозиция набора простых чисел:", new Task_2Handler(limitsNum[0], threadsNum[0]), limitsNum, threadsNum, testsCount, printAll));

            
            // Задание №4 - применение пула потоков:
            txtReport.AppendLine(MultiThrProcessing("Алгоритм №3 - применение пула потоков:", new Task_3Handler(limitsNum[0], threadsNum[0]), limitsNum, testsCount, printAll));


            // Задание №5 - последовательный перебор простых чисел:
            txtReport.AppendLine(MultiThrProcessing("Алгоритм №4 - последовательный перебор простых чисел:", new Task_4Handler(limitsNum[0], threadsNum[0]), limitsNum, threadsNum, testsCount, printAll));


            using (StreamWriter output = new StreamWriter(Path.Combine(Path.GetFullPath(@"..\..\"), "CalcReport.txt")))
            {
                output.WriteLine(txtReport.ToString());
            }


            Console.Write("\nPress any key...");
            Console.ReadKey();
        }


        // однопоточный расчет:
        static string SingleThrProcessing(string strAlg, Func<uint, List<uint>> func, uint[] limits, uint testsCount, bool printAllResults)
        {
            string format1 = "Результат вычислений: число элементов -{0,10}, количество простых -{1,10}, время выполнения - {2} ms";
            string format2 = "Средний результат: число опытов -{0,3}  число элементов -{1,10}, количество простых -{2,8}, время выполнения - {3} ms";
            StringBuilder sbTemp = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            Console.WriteLine(strAlg);
            sbTemp.AppendLine(strAlg);
            double meanTime = 0;

            foreach (uint n in limits)
            {
                int countPrimes = 0;
                for (int j = 0; j < (1 < testsCount ? testsCount : 2); j++)
                {
                    sw.Start();
                    List<uint> res = func(n);
                    sw.Stop();
                    countPrimes = res.Count;
                    meanTime += (0 < j) ? sw.Elapsed.TotalMilliseconds : 0;

                    if (printAllResults)
                    {
                        Console.WriteLine(string.Format(format1, n, res.Count, sw.Elapsed.TotalMilliseconds));
                        sbTemp.AppendLine(string.Format(format1, n, res.Count, sw.Elapsed.TotalMilliseconds));
                        // Console.Write("result:\t");
                        // res.ForEach((uint r) => { Console.Write(r + "\t"); });    
                    }
                    sw.Reset();
                }
                Console.WriteLine(string.Format(format2, testsCount, n, countPrimes, (meanTime / (testsCount - 1))));
                sbTemp.AppendLine(string.Format(format2, testsCount, n, countPrimes, (meanTime / (testsCount - 1))));
            }
            Console.WriteLine();
            return sbTemp.ToString();
        }


        // многопоточный расчет:
        static string MultiThrProcessing(string strAlg, TaskHandler task, uint[] limits, uint[] threads, uint testsCount, bool printAllResults)
        {
            string format = "Средний результат: число опытов -{0,3}, число потоков -{1,2}, число элементов -{2,10}, количество простых -{3,10}, время выполнения - {4} ms";
            StringBuilder sb = new StringBuilder();
            Console.WriteLine(strAlg);
            sb.AppendLine(strAlg);
            for (int thr = 0; thr < threads.Length; thr++)
            {
                if (0 < thr)
                {
                    task.SetThreadCount(threads[thr]);
                }

                for (int limit = 0; limit < limits.Length; limit++)
                {
                    double meanTime = 0;
                    task.SetLimitNumber(limits[limit]);
                    for (int j = 0; j < testsCount; j++)
                    {
                        try
                        {
                            task.CalculateTask();
                            meanTime += (0 < j) ? task.GetTotalMilliseconds() : 0;
                            if (printAllResults)
                            {
                                Console.WriteLine(task.ToString());
                                sb.AppendLine(task.ToString());
                            }
                        }
                        catch (Exception exp)
                        {
                            if (printAllResults) 
                            {
                                Console.WriteLine(exp.Message);
                                sb.AppendLine(exp.Message);
                            }
                        }                      
                    }
                    Console.WriteLine(string.Format(format, testsCount, task.GetThreadCount(), task.GetLimitNumber(), task.GetPrimesCount(), meanTime / (testsCount - 1)));
                    sb.AppendLine(string.Format(format, testsCount, task.GetThreadCount(), task.GetLimitNumber(), task.GetPrimesCount(), meanTime / (testsCount - 1)));
                }
                Console.WriteLine();
            }
            return sb.ToString();
        }


        // многопоточный расчет (для пула потоков):
        static string MultiThrProcessing(string strAlg, Task_3Handler task, uint[] limits,  uint testsCount, bool printAllResults)
        {
            string format = "Средний результат: число опытов -{0,3}, число элементов -{2,10}, количество простых -{3,10}, время выполнения - {4} ms";
            StringBuilder sb = new StringBuilder();
            Console.WriteLine(strAlg);
            sb.AppendLine(strAlg);
            
            for (int limit = 0; limit < limits.Length; limit++)
            {
                double meanTime = 0;
                task.SetLimitNumber(limits[limit]);
                for (int j = 0; j < testsCount; j++)
                {
                    try
                    {
                        task.CalculateTask();
                        meanTime += (0 < j) ? task.GetTotalMilliseconds() : 0;
                        if (printAllResults)
                        {
                            Console.WriteLine(task.ToString());
                            sb.AppendLine(task.ToString());
                        }
                    }
                    catch (Exception exp)
                    {
                        if (printAllResults)
                        {
                            Console.WriteLine(exp.Message);
                            sb.AppendLine(exp.Message);
                        }
                    }
                }
                Console.WriteLine(string.Format(format, testsCount, task.GetThreadCount(), task.GetLimitNumber(), task.GetPrimesCount(), meanTime / (testsCount - 1)));
                sb.AppendLine(string.Format(format, testsCount, task.GetThreadCount(), task.GetLimitNumber(), task.GetPrimesCount(), meanTime / (testsCount - 1)));
            }
            Console.WriteLine();
            
            return sb.ToString();
        }
    }
}