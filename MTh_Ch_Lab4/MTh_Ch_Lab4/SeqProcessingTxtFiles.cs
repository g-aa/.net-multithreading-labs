using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTh_Ch_Lab4
{
    class SeqProcessingTxtFiles
    {
        public enum SeqType { SEQSTANDARD, LINQ }
        
        private readonly string m_directoryPath;

        private readonly char[] m_wordsDelimiters;
        
        private readonly List<string> m_txtFilePath;
        
        private Dictionary<string, int> m_wordsDictionary;

        readonly Stopwatch m_sw;


        public SeqType SType { get; set; }



        public SeqProcessingTxtFiles(string directoryPath, char[] wordsDelimiters, SeqType sType)
        {
            m_directoryPath = directoryPath;
            m_wordsDelimiters = wordsDelimiters;
            m_txtFilePath = new List<string>();
            m_wordsDictionary = new Dictionary<string, int>();
            m_sw = new Stopwatch();
            SType = sType;

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
                if (SType != SeqType.LINQ)
                {
                    foreach (string filePath in m_txtFilePath)
                    {
                        this.СalcWordsStatistics(filePath);
                    }
                }
                else
                {
                    foreach (string filePath in m_txtFilePath)
                    {
                        this.СalcWordsStatisticsLINQ(filePath);
                    }
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
                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    textline = textline.ToLower();
                    string[] words = textline.ToLower().Split(m_wordsDelimiters);
                    foreach (string word in words)
                    {
                        if (m_wordsDictionary.ContainsKey(word))
                        {
                            m_wordsDictionary[word]++;
                        }
                        else if(!"".Equals(word)) 
                        {
                            m_wordsDictionary.Add(word, 1);
                        }
                    }
                }
            }
            m_wordsDictionary = m_wordsDictionary.OrderBy(k => k.Value).ToDictionary(p => p.Key, p => p.Value);
        }

        private void СalcWordsStatisticsLINQ(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
            {
                string[] allWords = sr.ReadToEnd().ToLower().Split(m_wordsDelimiters);
                var qTempWordsDictionary =  allWords.Where(w => !"".Equals(w)).GroupBy(w => w).OrderBy(k => k.Count());
                    
                if (m_wordsDictionary.Count != 0)
                {
                    var qWd =  from item in m_wordsDictionary.Union(qTempWordsDictionary.ToDictionary(p => p.Key, p => p.Count()))
                               group item by item.Key into gPair
                               select new { k = gPair.Key, v = gPair.Sum(p => p.Value) };

                    m_wordsDictionary = qWd.ToDictionary(p => p.k, p=> p.v);
                }
                else
                {
                    m_wordsDictionary = qTempWordsDictionary.ToDictionary(p => p.Key, p => p.Count());
                }
            }
        }



        public bool GetFirstNHighFrequently(ref double calcTime, ref Dictionary<string, int> outWordsDictionary, int n)
        {
            if (SType != SeqType.LINQ)
            {
                return this.subGetFirstNHighFrequently(ref calcTime, ref outWordsDictionary, n);
            }
            else
            {
                return this.subGetFirstNHighFrequentlyLINQ(ref calcTime, ref outWordsDictionary, n);
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
                    List<KeyValuePair<string, int>> topNWords = new List<KeyValuePair<string, int>>();
                    foreach (KeyValuePair<string, int> word in m_wordsDictionary)
                    {
                        if (topNWords.Count >= n)
                        {
                            if (word.Value > topNWords.Last().Value)
                            {
                                for (int i = 0; i < topNWords.Count; i++)
                                {
                                    if (word.Value >= topNWords[i].Value)
                                    {
                                        topNWords.Insert(i, word);
                                        topNWords.Remove(topNWords.Last());
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            topNWords.Add(word);
                            if (topNWords.Count == n)
                            {
                                topNWords.Sort((KeyValuePair<string, int> p1, KeyValuePair<string, int> p2) => { return -p1.Value.CompareTo(p2.Value); });
                            }
                        }
                    }
                    outWordsDictionary = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, int> word in topNWords)
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

        private bool subGetFirstNHighFrequentlyLINQ(ref double calcTime, ref Dictionary<string, int> outWordsDictionary, int n)
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
                    var ordered = m_wordsDictionary.OrderBy(x => -x.Value).Take(n < m_wordsDictionary.Count ? n : m_wordsDictionary.Count);
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
            if (SType != SeqType.LINQ)
            {
                return this.subGetStatisticsOnWordLength(ref calcTime, ref outLengthStatictic);
            }
            else
            {
                return this.subGetStatisticsOnWordLengthLINQ(ref calcTime, ref outLengthStatictic);
            }
        }

        private bool subGetStatisticsOnWordLength(ref double calcTime, ref Dictionary<int, int> outLengthStatictic)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {
                flag = true;
                Dictionary<int, int> tempLengthStatictic = new Dictionary<int, int>();
                foreach (KeyValuePair<string, int> pair in m_wordsDictionary)
                {
                    if (tempLengthStatictic.ContainsKey(pair.Key.Length))
                    {
                        tempLengthStatictic[pair.Key.Length]++;
                    }
                    else
                    {
                        tempLengthStatictic.Add(pair.Key.Length, 1);
                    }
                }
                List<KeyValuePair<int, int>> myList = tempLengthStatictic.ToList();
                myList.Sort((KeyValuePair<int, int> p1, KeyValuePair<int, int> p2) => { return p1.Key.CompareTo(p2.Key); });
                outLengthStatictic = new Dictionary<int, int>();
                foreach (KeyValuePair<int, int> item in myList)
                {
                    outLengthStatictic.Add(item.Key, item.Value);
                }
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }

        private bool subGetStatisticsOnWordLengthLINQ(ref double calcTime, ref Dictionary<int, int> outLengthStatictic)
        {
            bool flag = false;
            m_sw.Start();
            if (m_wordsDictionary.Count != 0)
            {                
                flag = true;
                var wordsLengths = m_wordsDictionary.Select(w => w.Key.Length); // получить длины слов отсортировав по возврастанию
                var groupWords = wordsLengths.GroupBy(i => i).OrderBy(i => i.Key); // сгруппировать похожие числа
                outLengthStatictic = groupWords.ToDictionary(k => k.Key, k => k.Count()); // подсчет числа встречаймости        
            }
            m_sw.Stop();
            calcTime = m_sw.Elapsed.TotalMilliseconds;
            m_sw.Reset();
            return flag;
        }
    }
}