using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab5
{
    public static class NewMySorts
    {
        private static int m_maxBlockElements; // пороговое значение элементов для многопоточной обработки
        private static int m_recursionDepth; // глубина рекурсии
        private static int m_pCount; // число процессоров ПК

        public enum SetType { Direct, Reverse, Random }

        public enum SortType { Serial, Parallel };


        static NewMySorts()
        {
            m_recursionDepth = Environment.ProcessorCount;
            m_pCount = Environment.ProcessorCount;
            m_maxBlockElements = 200;
        }


        public static List<int> GenerateTestIntegersSet(int setCount, int setMaxValue, SetType setType, CancellationToken token)
        {
            // при срабатывании CancellationToken возвращает пустой лист !
            if (setCount < 2)
            {
                throw new ArgumentException("в наборе должно быть как минимум 2 элемента!");
            }

            if (setMaxValue < setCount)
            {
                throw new ArgumentException("Число элементов не должно быть меньше максимального числа элементов в наборе!");
            }

            // проверка стоит ли запускать многопоточную обработку:
            if (setCount / m_pCount < m_maxBlockElements)
            {
                // однопоточное выполнение:
                List<int> resultSet = NewMySorts.SubGenerateTestIntegersSet(setType == SetType.Reverse ? setCount : 0, setCount, setMaxValue, setType, token);
                return resultSet != null ? resultSet : new List<int>();
            }
            else
            {
                // многопоточное выполнение:
                GenerateTasck[] generateTascks = new GenerateTasck[m_pCount];
                Action[] actions = new Action[m_pCount];
                for (
                    int i = 0,
                    blockCount = setCount / m_pCount, // число элементов на один блок
                    modCount = setCount % m_pCount, // сколько осталось элементов
                    startValue = setType == SetType.Reverse ? setCount : 0;
                    i < m_pCount;
                    i++)
                {
                    if (i < m_pCount - 1)
                    {
                        generateTascks[i] = new GenerateTasck(startValue, blockCount, setMaxValue, setType, token);
                    }
                    else
                    {
                        generateTascks[i] = new GenerateTasck(startValue, blockCount + modCount, setMaxValue, setType, token);
                    }
                    actions[i] = generateTascks[i].Generate;
                    startValue = setType == SetType.Reverse ? startValue - blockCount : startValue + blockCount;
                }
                Parallel.Invoke(actions);

                if (token.IsCancellationRequested)
                {
                    return new List<int>();
                }
                else
                {
                    List<int> resultSet = new List<int>(setCount);
                    foreach (var t in generateTascks)
                    {
                        List<int> k = t.Block;
                        resultSet.AddRange(k);
                    }
                    return resultSet;
                }
            }
        }

        private static List<int> SubGenerateTestIntegersSet(int startValue, int setCount, int setMaxValue, SetType setType, CancellationToken token)
        {
            // при срабатывании CancellationToken возвращает null !
            List<int> resultSet = new List<int>(setCount);
            if (setType == SetType.Random)
            {
                // генерация случайного набора целых чисел:
                Random random = new Random();
                for (int i = 0; i < setCount; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return null;
                    }
                    resultSet.Add(random.Next(setMaxValue));
                }
            }
            else if (setType == SetType.Direct)
            {
                for (int i = 0; i < setCount; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return null;
                    }
                    resultSet.Add(i + startValue);
                }
            }
            else
            {
                for (int i = 1; i <= setCount; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return null;
                    }
                    resultSet.Add(startValue - i);
                }
            }
            return resultSet;
        }

        private class GenerateTasck
        {
            private int m_startValue;
            private int m_blockCount;
            private int m_setMaxValue;
            private SetType m_setType;
            private CancellationToken m_token;

            public List<int> Block {get; private set;}

            public GenerateTasck(int startValue, int blockCount, int setMaxValue, SetType setType, CancellationToken token)
            {
                m_startValue = startValue;
                m_blockCount = blockCount;
                m_setMaxValue = setMaxValue;
                m_setType = setType;
                m_token = token;
            }


            public void Generate()
            {
                Block = SubGenerateTestIntegersSet(m_startValue, m_blockCount, m_setMaxValue, m_setType, m_token);
            }
        }


        public static List<int> SampleSort(List<int> sortableItems, SortType sortType, BackgroundWorker worker, DoWorkEventArgs e, CancellationToken token, int timeDelay_ms)
        {
            if (sortType != SortType.Parallel)
            {
                // последовательная обработка:
                worker.ReportProgress(0);
                List<int> sortResult = new List<int>(sortableItems);
                sortResult.Sort();
                if (token.IsCancellationRequested)
                {
                    return new List<int>();
                }
                return sortResult;
            }
            else
            {
                // параллельная обработка:
                NewMySorts.HandlerSampleSort sampleSort = new HandlerSampleSort(sortableItems, worker, token, timeDelay_ms);
                sampleSort.RunSortingScript();
                if (token.IsCancellationRequested)
                {
                    return new List<int>();
                }
                return sampleSort.GetResult();
            }
        }


        private class HandlerSampleSort
        {
            private BackgroundWorker m_worker;  // для индикации процесса выполнения
            private CancellationToken m_token;  
            private object m_gLock; // замок для синхроннго доступа к глобальным образцам

            private int m_timeDelay_ms;

            private int[] m_gsIndices; // индексы глобальных образцов
            private int[] m_lsIndices; // индексы локальных образцов

            private List<int> m_sortableSet;
            private List<int>[] m_preSortBlocks;   // блок данных для предварительной сортировки
            private List<int>[] m_finalSortBlocks; // блок данных для финальной сортировки
            private List<int> m_allSamples;     // глобальный набор образцов

            private BlockTasck[] m_blockTascks; // перечень задач для выполнения отдельными блоками
            private Action[] m_preSortActions;
            private Action[] m_getBlocksForFinalActions;
            private Action[] m_finalSortActions;


            public HandlerSampleSort(List<int> sortableSet, BackgroundWorker worker, CancellationToken token, int timeDelay_ms)
            {
                m_worker = worker;
                m_token = token;
                m_gLock = new object();

                m_timeDelay_ms = timeDelay_ms;

                m_sortableSet = sortableSet;
                m_allSamples = new List<int>();

                m_blockTascks = new BlockTasck[m_pCount];
                m_preSortActions = new Action[m_pCount];
                m_getBlocksForFinalActions = new Action[m_pCount];
                m_finalSortActions = new Action[m_pCount];

                for (int i = 0; i < m_pCount; i++)
                {
                    m_blockTascks[i] = new BlockTasck(i, this);
                    m_preSortActions[i] = m_blockTascks[i].BlockPreSorting;
                    m_getBlocksForFinalActions[i] = m_blockTascks[i].GetBlocksForFinalSorting;
                    m_finalSortActions[i] = m_blockTascks[i].BlockFinalSorting;
                }
            }


            private void GetSampleIndices()
            {
                // расчет глобальных индексов:
                m_gsIndices = new int[m_pCount - 1];
                for (int i = 0; i < m_pCount - 1; i++)
                {
                    m_gsIndices[i] = (i + 1) * m_pCount + m_pCount / 2 - (i != m_gsIndices.Length - 1 ? 1 : 0);
                }

                // расчет локальных индексов:
                m_lsIndices = new int[m_pCount];
                int m = (int)Math.Round((double)m_sortableSet.Count / (m_pCount * m_pCount));
                for (int i = 1; i < m_pCount; i++)
                {
                    m_lsIndices[i] = m * i;
                }
            }

            private void GetSortingSets()
            {
                // число элементов на один набор:
                int setElemCount = m_sortableSet.Count / m_pCount;

                // // остаток от деления (используется для выравнивания наборов):
                int mod = m_sortableSet.Count % m_pCount;
                
                m_preSortBlocks = new List<int>[m_pCount];
                m_finalSortBlocks = new List<int>[m_pCount]; 
                for (int i = 0, startIdx = 0; i < m_pCount; i++)
                {
                    // проверка на отмену операции сортировки: 
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                    
                    int length = mod > 0 ? setElemCount + 1 : setElemCount;
                    m_preSortBlocks[i] = new List<int>(length);
                    m_preSortBlocks[i].AddRange(m_sortableSet.GetRange(startIdx, length));
                    m_finalSortBlocks[i] = new List<int>(); // инициализация финальных блоков для сортировки

                    // обработка индексов:
                    startIdx += length;
                    mod--;
                }
            }

            private void BlockPreSorting(int blockIndex)
            {
                // сортировка блока:
                m_preSortBlocks[blockIndex].Sort(); // может на свою поменяю !!!

                // проверка на отмену операции сортировки:
                if (m_token.IsCancellationRequested)
                {
                    return;
                }

                // заполнение глобального списка образцов:
                lock (m_gLock)
                {
                    foreach (int idx in m_lsIndices)
                    {
                        // проверка на отмену операции сортировки:
                        if (m_token.IsCancellationRequested)
                        {
                            return;
                        }

                        m_allSamples.Add(m_preSortBlocks[blockIndex][idx]);
                    }
                }
            }

            private void GetBlocksForFinalSorting(int blockIndex)
            {
                int c = 0; // индекс для обхода массива индексов глобальных образцов :) 
                List<int> subBlock = new List<int>();

                // обходим компоненты своего блока:
                foreach (int element in m_preSortBlocks[blockIndex])
                {
                    // проверка на выполнение отмены операции сортировки:
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }

                    // условие заполнения компонентов блоков для финальной сортировки:
                    if (c < m_gsIndices.Length && m_allSamples[m_gsIndices[c]] < element)
                    {
                        //заполнение финальных блоков:
                        lock (m_gLock)
                        {
                            m_finalSortBlocks[c].AddRange(subBlock);
                        }
                        subBlock.Clear();
                        c++;
                    }
                    subBlock.Add(element);
                }

                // проверка на выполнение отмены операции сортировки:
                if (m_token.IsCancellationRequested)
                {
                    return;
                }

                // заполнение последнего из финальных блоков:
                lock (m_gLock)
                {
                    m_finalSortBlocks[c].AddRange(subBlock);
                }
            }

            private void BlockFinalSorting(int blockIndex)
            {
                // сортировка блока:
                m_finalSortBlocks[blockIndex].Sort(); // может на свою поменяю !!!
            }

            private void SortGlobalSamples()
            {
                m_allSamples.Sort();
            }


            public void RunSortingScript()
            {
                ParallelOptions options = new ParallelOptions();
                options.CancellationToken = m_token;


                // подготовка индексов:
                m_worker.ReportProgress(0);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                this.GetSampleIndices();

                if (m_token.IsCancellationRequested)
                {
                    return;
                }


                // генерация блоков для расчета в многопоточной среде:
                m_worker.ReportProgress(1);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                this.GetSortingSets();

                if (m_token.IsCancellationRequested)
                {
                    return;
                }


                // предварительная сортировка + получение глобальных образцов:
                m_worker.ReportProgress(2);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                Parallel.Invoke(options, this.m_preSortActions);

                if (m_token.IsCancellationRequested)
                {
                    return;
                }


                // сортировка глобальных образцов:
                m_worker.ReportProgress(3);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                this.SortGlobalSamples();

                if (m_token.IsCancellationRequested)
                {
                    return;
                }


                // формирование блоков для финальной сортировки:
                m_worker.ReportProgress(4);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                Parallel.Invoke(options, m_getBlocksForFinalActions);

                if (m_token.IsCancellationRequested)
                {
                    return;
                }


                // финальная сортировка блоков:
                m_worker.ReportProgress(5);
                if (0 < m_timeDelay_ms)
                {
                    Thread.Sleep(m_timeDelay_ms);
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                Parallel.Invoke(options, m_finalSortActions);
            }

            public List<int> GetResult()
            {
                List<int> sortResult = new List<int>(m_sortableSet.Count);
                foreach (List<int> block in m_finalSortBlocks)
                {
                    if (m_token.IsCancellationRequested)
                    {
                        return new List<int>();
                    }
                    sortResult.AddRange(block);
                }
                return sortResult;
            }


            private class BlockTasck
            {
                // индекс блока:
                private int m_blockIndex;

                // ссылка на родителя:
                private HandlerSampleSort m_handlerSort; 
                

                public BlockTasck(int blockIndex, HandlerSampleSort handlerSort)
                {
                    m_blockIndex = blockIndex;
                    m_handlerSort = handlerSort;
                }


                public void BlockPreSorting()
                {
                    m_handlerSort.BlockPreSorting(m_blockIndex);
                }

                
                public void GetBlocksForFinalSorting()
                {
                    m_handlerSort.GetBlocksForFinalSorting(m_blockIndex);
                }


                public void BlockFinalSorting()
                {
                    m_handlerSort.BlockFinalSorting(m_blockIndex);
                }
            }
        }


    }
}
