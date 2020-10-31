using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Forms;

namespace MTh_Ch_Lab4
{
    public class Program
    {
        static object objLock = new object();
        
        static void Main(string[] args)
        {
            char[] wordsDelimiters = { ' ', '\n', '<', '>', ',', '.', '?', '!', '\r', '\'', '\\', '\t', '-', ':', ';', '(', ')', '[', ']' };

            string dirPath = "D:\\Repository\\Csh\\VisualStudio\\Labs\\Multithreading\\txtFiles\\";
            string sPath = "D:\\Repository\\Csh\\VisualStudio\\Labs\\Multithreading\\txtResult\\wd_{0}.txt";

            double calcTime = 0;
            
            Dictionary<string, int> wordsDictionary = new Dictionary<string, int>();
            
            Dictionary<int, int> wordsLengthStatictic = new Dictionary<int, int>();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            SeqProcessingTxtFiles seqProcessing = new SeqProcessingTxtFiles(dirPath, wordsDelimiters, SeqProcessingTxtFiles.SeqType.SEQSTANDARD);

            if (seqProcessing.GetFileContent(ref calcTime, ref wordsDictionary))
            {
                //foreach (KeyValuePair<string, int> word in wordsDictionary)
                //{
                //    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                //}
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.SEQSTANDARD.ToString(), wordsDictionary.Count, calcTime));    
            }
            DictionaryPrintToFile(string.Format(sPath, Enum.GetName(typeof(SeqProcessingTxtFiles.SeqType), seqProcessing.SType)), wordsDictionary);

            seqProcessing.SType = SeqProcessingTxtFiles.SeqType.LINQ;
            if (seqProcessing.GetFileContent(ref calcTime, ref wordsDictionary))
            {
                //foreach (KeyValuePair<string, int> word in wordsDictionary)
                //{
                //    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                //}
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.LINQ.ToString(), wordsDictionary.Count, calcTime));
            }
            DictionaryPrintToFile(string.Format(sPath, Enum.GetName(typeof(SeqProcessingTxtFiles.SeqType), seqProcessing.SType)), wordsDictionary);

            seqProcessing.SType = SeqProcessingTxtFiles.SeqType.SEQSTANDARD;
            if (seqProcessing.GetFirstNHighFrequently(ref calcTime, ref wordsDictionary, 10))
            {
                foreach (KeyValuePair<string, int> word in wordsDictionary)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.SEQSTANDARD.ToString(), wordsDictionary.Count, calcTime));
            }

            seqProcessing.SType = SeqProcessingTxtFiles.SeqType.LINQ;
            if (seqProcessing.GetFirstNHighFrequently(ref calcTime, ref wordsDictionary, 10))
            {
                foreach (KeyValuePair<string, int> word in wordsDictionary)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.LINQ.ToString(), wordsDictionary.Count, calcTime));
            }

            seqProcessing.SType = SeqProcessingTxtFiles.SeqType.SEQSTANDARD;
            if (seqProcessing.GetStatisticsOnWordLength(ref calcTime, ref wordsLengthStatictic))
            {
                foreach (KeyValuePair<int, int> item in wordsLengthStatictic)
                {
                    Console.WriteLine(string.Format("Длина слова: {0,10}, частота встречаймости: {1,10}", item.Key, item.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.SEQSTANDARD.ToString(), wordsLengthStatictic.Count, calcTime));
            }

            seqProcessing.SType = SeqProcessingTxtFiles.SeqType.LINQ;
            if (seqProcessing.GetStatisticsOnWordLength(ref calcTime, ref wordsLengthStatictic))
            {
                foreach (KeyValuePair<int, int> item in wordsLengthStatictic)
                {
                    Console.WriteLine(string.Format("Длина слова: {0,10}, частота встречаймости: {1,10}", item.Key, item.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", SeqProcessingTxtFiles.SeqType.LINQ.ToString(), wordsLengthStatictic.Count, calcTime));
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ParallelProcessingTxtFiles parallelProcessing = new ParallelProcessingTxtFiles(dirPath, wordsDelimiters, ParallelProcessingTxtFiles.ParallelType.PARSTANDART);

            if (parallelProcessing.GetFileContent(ref calcTime, ref wordsDictionary))
            {
                //foreach (KeyValuePair<string, int> word in wordsDictionary)
                //{
                //    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                //}
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PARSTANDART.ToString(), wordsDictionary.Count, calcTime));
            }
            DictionaryPrintToFile(string.Format(sPath, Enum.GetName(typeof(ParallelProcessingTxtFiles.ParallelType), parallelProcessing.PType)), wordsDictionary);

            parallelProcessing.PType = ParallelProcessingTxtFiles.ParallelType.PLINQ;
            if (parallelProcessing.GetFileContent(ref calcTime, ref wordsDictionary))
            {
                //foreach (KeyValuePair<string, int> word in wordsDictionary)
                //{
                //    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                //}
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PLINQ.ToString(), wordsDictionary.Count, calcTime));
            }
            DictionaryPrintToFile(string.Format(sPath, Enum.GetName(typeof(ParallelProcessingTxtFiles.ParallelType), parallelProcessing.PType)), wordsDictionary);

            parallelProcessing.PType = ParallelProcessingTxtFiles.ParallelType.PARSTANDART;
            if (parallelProcessing.GetFirstNHighFrequently(ref calcTime, ref wordsDictionary, 10))
            {
                foreach (KeyValuePair<string, int> word in wordsDictionary)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PARSTANDART.ToString(), wordsDictionary.Count, calcTime));
            }

            parallelProcessing.PType = ParallelProcessingTxtFiles.ParallelType.PLINQ;
            if (parallelProcessing.GetFirstNHighFrequently(ref calcTime, ref wordsDictionary, 10))
            {
                foreach (KeyValuePair<string, int> word in wordsDictionary)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", word.Key, word.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PLINQ.ToString(), wordsDictionary.Count, calcTime));
            }

            parallelProcessing.PType = ParallelProcessingTxtFiles.ParallelType.PARSTANDART;
            if (parallelProcessing.GetStatisticsOnWordLength(ref calcTime, ref wordsLengthStatictic))
            {
                foreach (KeyValuePair<int, int> item in wordsLengthStatictic)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", item.Key, item.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PARSTANDART.ToString(), wordsDictionary.Count, calcTime));
            }

            parallelProcessing.PType = ParallelProcessingTxtFiles.ParallelType.PLINQ;
            if (parallelProcessing.GetStatisticsOnWordLength(ref calcTime, ref wordsLengthStatictic))
            {
                foreach (KeyValuePair<int, int> item in wordsLengthStatictic)
                {
                    Console.WriteLine(string.Format("Слово: {0,25}, частота встречаймости: {1,5}", item.Key, item.Value));
                }
                Console.WriteLine(string.Format("Подсчет слов: алгоритм - {0,15}, колличество слов - {1,10}, время выполнения - {2} мс\n", ParallelProcessingTxtFiles.ParallelType.PLINQ.ToString(), wordsDictionary.Count, calcTime));
            }


            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }


        /// <summary>
        /// Печать слова в файл
        /// </summary>
        /// <typeparam name="tKey"></typeparam>
        /// <typeparam name="tValue"></typeparam>
        /// <param name="path"></param>
        /// <param name="dictionary"></param>
        public static void DictionaryPrintToFile<tKey, tValue>(string path, Dictionary<tKey, tValue> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<tKey, tValue> item in dictionary)
            {
                sb.AppendLine(string.Format("{0,-40}:{1,10}", item.Key, item.Value));
            }
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(sb.ToString());
            }
        }
    }
}
