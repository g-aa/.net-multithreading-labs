using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace MTh_Ch_Lab4
{
    class ParallelProcessingTxtFiles
    {
        public enum ParallelType { PARSTANDART, PLINQ }

        private readonly string m_directoryPath;

        private readonly char[] m_wordsDelimiters;
        
        private readonly List<string> m_txtFilePath;

        private ConcurrentDictionary<string, int> m_wordsDictionary;

        private readonly Stopwatch m_sw;

        private object m_lock;

        public ParallelType PType { get; set; }



        public ParallelProcessingTxtFiles(string directoryPath, char[] wordsDelimiters, ParallelType pType)
        {
            m_directoryPath = directoryPath;
            m_wordsDelimiters = wordsDelimiters;
            m_txtFilePath = new List<string>();
            m_wordsDictionary = new ConcurrentDictionary<string, int>();
            m_sw = new Stopwatch();
            PType = pType;
            m_lock = new object();

            this.txtFilesPathsearch(directoryPath);
        }



        private void txtFilesPathsearch(string targetDirectory)
        {
            // обрабатываем все файлы в текущем каталоге:
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                m_txtFilePath.Add(fileName);
            }

            // заходим в подкаталог:
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                this.txtFilesPathsearch(subdirectory);
            }
        }



        public bool GetFileContent(ref double calcTime, ref Dictionary<string, int> wordsDictionary)
        {
            bool flag = false;
            m_sw.Start();
            if (m_txtFilePath.Count != 0)
            {
                flag = true;
                if (PType != ParallelType.PLINQ)
                {
                    Parallel.ForEach(m_txtFilePath, СalcWordsStatistics);
                }
                else
                {
                    Parallel.ForEach(m_txtFilePath, СalcWordsStatisticsPLINQ);
                }
            }
            wordsDictionary = m_wordsDictionary.ToDictionary(p => p.Key, p => p.Value);
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }

        private void СalcWordsStatistics(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
            {
                string[] allWords = sr.ReadToEnd().ToLower().Split(m_wordsDelimiters);

                Parallel.ForEach(allWords, (string word) => 
                {
                    if (m_wordsDictionary.ContainsKey(word))
                    {
                        m_wordsDictionary[word]++;
                    }
                    else if(!"".Equals(word))
                    {
                        m_wordsDictionary.TryAdd(word, 1);
                    }
                });
            }
        }

        private void СalcWordsStatisticsPLINQ(string filePath) 
        {
            using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
            {
                string[] allWords = sr.ReadToEnd().ToLower().Split(m_wordsDelimiters);
                var qTempWordsDictionary = allWords.AsParallel().Where(w => !"".Equals(w)).GroupBy(x => x).OrderBy(k => k.Count());

                if (m_wordsDictionary.Count != 0)
                {
                    var qWd = from item in m_wordsDictionary.AsParallel().Union(qTempWordsDictionary.ToDictionary(p => p.Key, p => p.Count()).AsParallel())
                              group item by item.Key into gPair
                              select new { k = gPair.Key, v = gPair.Sum(p => p.Value) };

                    m_wordsDictionary = new ConcurrentDictionary<string, int>(qWd.ToDictionary(p => p.k, p => p.v));
                }
                else
                {
                    m_wordsDictionary = new ConcurrentDictionary<string, int>(qTempWordsDictionary.ToDictionary(p => p.Key, p => p.Count()));
                }
            }
        }



        public bool GetFirstNHighFrequently(ref double calcTime, ref Dictionary<string, int> outWordsDictionary, int n)
        {
            if (PType != ParallelType.PLINQ)
            {
                return this.subGetFirstNHighFrequently(ref calcTime, ref outWordsDictionary, n);
            }
            else
            {
                return this.subGetFirstNHighFrequentlyPLINQ(ref calcTime, ref outWordsDictionary, n);
            }
        }

        private bool subGetFirstNHighFrequently(ref double calcTime, ref Dictionary<string, int> outWordsDictionary, int n)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {
                flag = true;
                if (m_wordsDictionary.Count <= n)
                {
                    outWordsDictionary = new Dictionary<string, int>(m_wordsDictionary);
                }
                else
                {
                    List<KeyValuePair<string, int>> pTopNWords = new List<KeyValuePair<string, int>>();
                    Parallel.ForEach(m_wordsDictionary, (KeyValuePair<string, int> p) => {
                        if (pTopNWords.Count >= n)
                        {
                            // if (p.Value > pTopNWords.Last().Value)
                            // {
                                lock (m_lock)
                                {
                                    if (p.Value > pTopNWords.Last().Value)
                                    {
                                        for (int i = 0; i < pTopNWords.Count; i++)
                                        {
                                            if (p.Value >= pTopNWords[i].Value)
                                            {
                                                pTopNWords.Insert(i, p);
                                                pTopNWords.Remove(pTopNWords.Last());
                                                break;
                                            }
                                        }
                                    }
                                }
                            // }
                        }
                        else
                        {
                            lock (m_lock)
                            {
                                pTopNWords.Add(p);
                                if (pTopNWords.Count == n)
                                {
                                    pTopNWords.Sort((KeyValuePair<string, int> p1, KeyValuePair<string, int> p2) => { return -p1.Value.CompareTo(p2.Value); });
                                }
                            }
                        }
                    });
                    outWordsDictionary = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, int> word in pTopNWords)
                    {
                        outWordsDictionary.Add(word.Key, word.Value);
                    }
                }
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }

        private bool subGetFirstNHighFrequentlyPLINQ(ref double calcTime, ref Dictionary<string, int> outWordsDictionary, int n)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {
                flag = true;
                if (m_wordsDictionary.Count == 1)
                {
                    outWordsDictionary = new Dictionary<string, int>(m_wordsDictionary);
                }
                else
                {
                    var ordered = m_wordsDictionary.AsParallel().OrderBy(x => -x.Value).Take(n < m_wordsDictionary.Count ? n : m_wordsDictionary.Count);
                    outWordsDictionary = ordered.ToDictionary(t => t.Key, t => t.Value);
                }
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }



        public bool GetStatisticsOnWordLength(ref double calcTime, ref Dictionary<int, int> outLengthStatictic)
        {
            if (PType != ParallelType.PLINQ)
            {
                return this.subGetStatisticsOnWordLength(ref calcTime, ref outLengthStatictic);
            }
            else
            {
                return this.subGetStatisticsOnWordLengthPLINQ(ref calcTime, ref outLengthStatictic);
            }
        }

        private bool subGetStatisticsOnWordLength(ref double calcTime, ref Dictionary<int, int> outLengthStatictic)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {
                flag = true;
                ConcurrentDictionary<int, int> tempLengthStatictic = new ConcurrentDictionary<int, int>();
                Parallel.ForEach(m_wordsDictionary, (KeyValuePair<string, int> pair) => 
                {
                    if (tempLengthStatictic.ContainsKey(pair.Key.Length))
                    {
                        tempLengthStatictic[pair.Key.Length]++;
                    }
                    else
                    {
                        tempLengthStatictic.TryAdd(pair.Key.Length, 1);
                    }
                });
                outLengthStatictic = tempLengthStatictic.ToDictionary(p => p.Key, p => p.Value);
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }

        private bool subGetStatisticsOnWordLengthPLINQ(ref double calcTime, ref Dictionary<int, int> outLengthStatictic)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {
                flag = true;
                var wordsLengths = m_wordsDictionary.AsParallel().Select(w => w.Key.Length); // получить длины слов отсортировав по возврастанию
                var groupWords = wordsLengths.GroupBy(i => i).OrderBy(i=> i.Key); // сгруппировать похожие числа
                outLengthStatictic = groupWords.ToDictionary(k => k.Key, k => k.Count()); // подсчет числа встречаймости
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }
    }
}
