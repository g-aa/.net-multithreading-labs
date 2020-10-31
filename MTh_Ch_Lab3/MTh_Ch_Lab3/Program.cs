using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System.Threading;
using System.Collections.Concurrent;

namespace MTh_Ch_Lab3
{
    class Program
    {
        private static string m_stringBuffer; // общий буфер
        private static ConcurrentQueue<string> m_queue;

        private object m_printLock; // замок на печать читателей 

        private object m_rLock; // замок на чтение
        private object m_wLock; // замок на запись

        private AutoResetEvent m_evFull;
        private AutoResetEvent m_evEmpty;

        private Semaphore m_wSemaphore;
        private Semaphore m_rSemaphore;

        private Int32 m_iFull;
        private Int32 m_iEmpty;

        private bool m_bwFinish; // статус завершения записи данных в буфер
        private bool m_bEmpty; // статус на буфера: true - пустой / false - полный
        
        private bool m_brPrint; 

        
        public Program(bool brPrint)
        {
            m_stringBuffer = string.Empty;
            m_queue = new ConcurrentQueue<string>();

            m_printLock = new object();

            m_rLock = new object();
            m_wLock = new object();

            m_evFull = new AutoResetEvent(false);
            m_evEmpty = new AutoResetEvent(true);

            m_wSemaphore = new Semaphore(1, 1);
            m_rSemaphore = new Semaphore(0, 1);

            m_iEmpty = 1;
            m_iFull = 0;

            m_bwFinish = false;
            m_bEmpty = true;

            m_brPrint = brPrint;
        }


        void Write(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                if (m_bEmpty)
                {
                    m_stringBuffer = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                    Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                    cnt++;
                    m_bEmpty = false;
                }
            }
        }
        
        void WriteLock(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                lock (m_wLock)
                {
                    if (m_bEmpty)
                    {
                        m_stringBuffer = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                        Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                        cnt++;
                        m_bEmpty = false;
                    }
                }
            }
        }

        void WriteResetEvent(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                m_evEmpty.WaitOne();
                m_stringBuffer = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                cnt++;
                m_evFull.Set();
            }
        }

        void WriteSemaphore(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                m_wSemaphore.WaitOne();
                m_stringBuffer = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                cnt++;
                m_rSemaphore.Release();
            }
        }

        void WriteInterlock(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                if (0 < m_iEmpty)
                {
                    if (0 < Interlocked.CompareExchange(ref m_iEmpty, 0, 1))
                    {
                        m_stringBuffer = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                        Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                        cnt++;
                        Interlocked.Increment(ref m_iFull);
                    }
                }
            }
        }

        void WriteConcurrent(object obj)
        {
            TestData dTemp = (TestData)obj;
            int cnt = 0;
            while (cnt < dTemp.RecordsCount)
            {
                // if (m_queue.Count == 0)
                // {
                    string sTemp = string.Format("{0} - {1}", dTemp.StringData, cnt + 1);
                    m_queue.Enqueue(sTemp);
                    Console.WriteLine(string.Format("{0}:\t{1}\n", Thread.CurrentThread.Name, sTemp));
                    cnt++;
                // }
            }
        }


        void Read()
        {
            List<string> readerBuffer = new List<string>();
            while (!m_bwFinish)
            {
                if (!m_bEmpty)
                {
                    string sTemp = m_stringBuffer;
                    readerBuffer.Add(sTemp);
                    Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, sTemp));
                    m_bEmpty = true;
                }   
            }

            if (m_brPrint)
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        void ReadLock()
        {
            List<string> readerBuffer = new List<string>();
            while (!m_bwFinish)
            {
                if (!m_bEmpty)
                {
                    lock (m_rLock)
                    {
                        if (!m_bEmpty)
                        {
                            readerBuffer.Add(m_stringBuffer);
                            Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                            m_bEmpty = true;
                        }
                    }
                }
            }

            if (m_brPrint)
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        void ReadResetEvent()
        {
            List<string> readerBuffer = new List<string>();
            while (!m_bwFinish)
            {
                m_evFull.WaitOne();
                if (m_bwFinish)
                {
                    m_evFull.Set();
                    break;
                }

                readerBuffer.Add(m_stringBuffer);
                Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                m_evEmpty.Set();
            }

            if (m_brPrint)
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        void ReadSemaphore()
        {
            List<string> readerBuffer = new List<string>();
            while (!m_bwFinish)
            {
                m_rSemaphore.WaitOne();
                if (m_bwFinish)
                {
                    m_rSemaphore.Release();
                    break;
                }

                readerBuffer.Add(m_stringBuffer);
                Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                m_wSemaphore.Release();
            }

            if (m_brPrint)
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        void ReadInterlock()
        {
            List<string> readerBuffer = new List<string>();
            while (!m_bwFinish)
            {  
                if (0 < m_iFull)
                {
                    if (0 < Interlocked.CompareExchange(ref m_iFull, 0, 1))
                    {
                        readerBuffer.Add(m_stringBuffer);
                        Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, m_stringBuffer));
                        Interlocked.Increment(ref m_iEmpty);
                    }
                }
            }

            if (m_brPrint) 
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        void ReadConcurrent()
        {
            List<string> readerBuffer = new List<string>();
            string sTemp = string.Empty;
            while (!m_bwFinish)
            {
                if (m_queue.TryDequeue(out sTemp))
                {
                    readerBuffer.Add(sTemp);
                    Console.WriteLine(string.Format("\t{0}:\t{1}\n", Thread.CurrentThread.Name, sTemp));
                }
            }

            if (m_brPrint)
            {
                // вывод информации читателей:
                lock (m_printLock)
                {
                    readerBuffer.Sort();
                    Console.WriteLine(Thread.CurrentThread.Name + " результаты захвата данных:");
                    readerBuffer.ForEach((string s) => { Console.WriteLine(s); });
                    Console.WriteLine();
                }
            }
        }

        static void Main(string[] args)
        {
            // исходные данные:
            uint testCount = 20;
            uint readersCount = 3;
            TestData[] testDatas = { new TestData("AAAA", 4), new TestData("BBBB", 2), new TestData("CCCC", 5) , new TestData("DDDD", 1) };

            StringBuilder stringBuilder = new StringBuilder();

            // тестирование (Lock):
            stringBuilder.AppendLine(Program.Test2_Funct(testCount, testDatas, readersCount, false));

            // тестирование (ResetEvent):
            stringBuilder.AppendLine(Program.Test3_Funct(testCount, testDatas, readersCount, false));

            // тестирование (Semaphore):
            stringBuilder.AppendLine(Program.Test4_Funct(testCount, testDatas, readersCount, false));

            // тестирование (Interlocked):
            stringBuilder.AppendLine(Program.Test5_Funct(testCount, testDatas, readersCount, false));

            // тестирование (потокобезопастных колекций):
            stringBuilder.AppendLine(Program.Test6_Funct(testCount, testDatas, readersCount, false));

            Console.WriteLine(stringBuilder);

            
            // тестирование (без синхронизации):
            Program.Test1_Funct(testDatas, readersCount, true);
            


            /////////////////////////////////////////////////////////////
            string[] writersData = { "AAAA", "BBBB", "CCCC", "DDDD" };
            DataBuffer data = new DataBuffer();
            Writer[] writers = new Writer[writersData.Length];
            for (int i = 0; i < writers.Length; i++)
            {
                writers[i] = new Writer(string.Format("Писатель №{0}", i + 1), data, writersData[i], 6);
                data.AddNewWriter(writers[i]);
            }

            Reader[] readers = new Reader[readersCount];
            for (int i = 0; i < readersCount; i++)
            {
                readers[i] = new Reader(string.Format("Читатель №{0}", i + 1), data);
            }

            // Запуск потоков на выполнение:
            Thread[] wThreads2 = new Thread[writers.Length];
            for (int i = 0; i < wThreads2.Length; i++)
            {
                wThreads2[i] = new Thread(writers[i].RunToWrite);
                wThreads2[i].Name = writers[i].GetName();
                wThreads2[i].Start();
            }

            Thread[] rThreads2 = new Thread[readers.Length];
            for (int i = 0; i < rThreads2.Length; i++)
            {
                rThreads2[i] = new Thread(readers[i].RunToRead);
                rThreads2[i].Name = readers[i].GetReaderName();
                rThreads2[i].Start();
            }

            Array.ForEach(wThreads2, (Thread t) => { t.Join(); });
            Array.ForEach(rThreads2, (Thread t) => { t.Join(); });
            Array.ForEach(readers, (Reader r) => { Console.WriteLine(r.ToString() + "\n"); });

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }


        static void Test1_Funct(TestData[] testData, uint readersCount, bool r_bPrint)
        {
            Program example = new Program(r_bPrint);
            Thread[] rThreads = new Thread[readersCount];
            Thread[] wThreads = new Thread[testData.Length];

            // параметры читателей писателей:
            for (int i = 0; i < wThreads.Length; i++)
            {
                wThreads[i] = new Thread(example.Write);
                wThreads[i].Name = "Писатель №" + i;
                wThreads[i].Start(testData[i]);
            }

            for (int i = 0; i < rThreads.Length; i++)
            {
                rThreads[i] = new Thread(example.Read);
                rThreads[i].Name = "Читатель №" + i;
                rThreads[i].Start();
            }

            Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });

            // окончание работы писателей:
            example.m_bwFinish = true;

            Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
        }

        static string Test2_Funct(uint testCount, TestData[] testData, uint readersCount, bool r_bPrint)
        {
            double meanTime = 0;
            testCount = (1 < testCount) ? testCount : 2;
            for (int tc = 0; tc < testCount; tc++)
            {
                Stopwatch sw = new Stopwatch();
                Program example = new Program(r_bPrint);
                Thread[] rThreads = new Thread[readersCount];
                Thread[] wThreads = new Thread[testData.Length];

                sw.Start();
                // параметры читателей писателей:
                for (int i = 0; i < wThreads.Length; i++)
                {
                    wThreads[i] = new Thread(example.WriteLock);
                    wThreads[i].Name = "Писатель №" + i;
                    wThreads[i].Start(testData[i]);
                }

                for (int i = 0; i < rThreads.Length; i++)
                {
                    rThreads[i] = new Thread(example.ReadLock);
                    rThreads[i].Name = "Читатель №" + i;
                    rThreads[i].Start();
                }

                Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });

                // окончание работы писателей:
                example.m_bwFinish = true;

                Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
                sw.Stop();
                meanTime += (0 < tc) ? sw.Elapsed.TotalMilliseconds : 0;
            }
            return string.Format("Lock:\t\tчисло прогонов - {0}, число писателей - {1}, число читателей - {2}, средее время выполнение - {3} мс", testCount, testData.Length, readersCount, meanTime / (testCount - 1));
        }

        static string Test3_Funct(uint testCount, TestData[] testData, uint readersCount, bool r_bPrint)
        {
            double meanTime = 0;
            testCount = (1 < testCount) ? testCount : 2;
            for (int tc = 0; tc < testCount; tc++)
            {
                Stopwatch sw = new Stopwatch();
                Program example = new Program(r_bPrint);
                Thread[] rThreads = new Thread[readersCount];
                Thread[] wThreads = new Thread[testData.Length];

                sw.Start();
                // параметры читателей писателей:
                for (int i = 0; i < wThreads.Length; i++)
                {
                    wThreads[i] = new Thread(example.WriteResetEvent);
                    wThreads[i].Name = "Писатель №" + i;
                    wThreads[i].Start(testData[i]);
                }

                for (int i = 0; i < rThreads.Length; i++)
                {
                    rThreads[i] = new Thread(example.ReadResetEvent);
                    rThreads[i].Name = "Читатель №" + i;
                    rThreads[i].Start();
                }

                Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });

                // окончание работы писателей:
                example.m_bwFinish = true;
                example.m_evFull.Set();

                Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
                sw.Stop();
                meanTime += (0 < tc) ? sw.Elapsed.TotalMilliseconds : 0;
            }
            return string.Format("ResetEvent:\tчисло прогонов - {0}, число писателей - {1}, число читателей - {2}, средее время выполнение - {3} мс", testCount, testData.Length, readersCount, meanTime / (testCount - 1));
        }

        static string Test4_Funct(uint testCount, TestData[] testData, uint readersCount, bool r_bPrint)
        {
            double meanTime = 0;
            testCount = (1 < testCount) ? testCount : 2;
            for (int tc = 0; tc < testCount; tc++)
            {
                Stopwatch sw = new Stopwatch();
                Program example = new Program(r_bPrint);
                Thread[] rThreads = new Thread[readersCount];
                Thread[] wThreads = new Thread[testData.Length];

                sw.Start();
                // параметры читателей писателей:
                for (int i = 0; i < wThreads.Length; i++)
                {
                    wThreads[i] = new Thread(example.WriteSemaphore);
                    wThreads[i].Name = "Писатель №" + i;
                    wThreads[i].Start(testData[i]);
                }

                for (int i = 0; i < rThreads.Length; i++)
                {
                    rThreads[i] = new Thread(example.ReadSemaphore);
                    rThreads[i].Name = "Читатель №" + i;
                    rThreads[i].Start();
                }

                Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });
                
                // окончание работы писателей:
                example.m_bwFinish = true;
                example.m_rSemaphore.Release(); 

                Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
                sw.Stop();
                meanTime += (0 < tc) ? sw.Elapsed.TotalMilliseconds : 0;
            }
            return string.Format("Semaphore:\tчисло прогонов - {0}, число писателей - {1}, число читателей - {2}, средее время выполнение - {3} мс", testCount, testData.Length, readersCount, meanTime / (testCount - 1));
        }

        static string Test5_Funct(uint testCount, TestData[] testData, uint readersCount, bool r_bPrint)
        {
            double meanTime = 0;
            testCount = (1 < testCount) ? testCount : 2;
            for (int tc = 0; tc < testCount; tc++)
            {
                Stopwatch sw = new Stopwatch();
                Program example = new Program(r_bPrint);
                Thread[] rThreads = new Thread[readersCount];
                Thread[] wThreads = new Thread[testData.Length];

                sw.Start();
                // параметры читателей писателей:
                for (int i = 0; i < wThreads.Length; i++)
                {
                    wThreads[i] = new Thread(example.WriteInterlock);
                    wThreads[i].Name = "Писатель №" + i;
                    wThreads[i].Start(testData[i]);
                }

                for (int i = 0; i < rThreads.Length; i++)
                {
                    rThreads[i] = new Thread(example.ReadInterlock);
                    rThreads[i].Name = "Читатель №" + i;
                    rThreads[i].Start();
                }

                Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });
                example.m_bwFinish = true; // окончание работы писателей

                Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
                sw.Stop();
                meanTime += (0 < tc) ? sw.Elapsed.TotalMilliseconds : 0;
            }
            return string.Format("Interlock:\tчисло прогонов - {0}, число писателей - {1}, число читателей - {2}, средее время выполнение - {3} мс", testCount, testData.Length, readersCount, meanTime / (testCount - 1));
        }

        static string Test6_Funct(uint testCount, TestData[] testData, uint readersCount, bool r_bPrint)
        {
            double meanTime = 0;
            testCount = (1 < testCount) ? testCount : 2;
            for (int tc = 0; tc < testCount; tc++)
            {
                Stopwatch sw = new Stopwatch();
                Program example = new Program(r_bPrint);
                Thread[] rThreads = new Thread[readersCount];
                Thread[] wThreads = new Thread[testData.Length];

                sw.Start();
                // параметры читателей писателей:
                for (int i = 0; i < wThreads.Length; i++)
                {
                    wThreads[i] = new Thread(example.WriteConcurrent);
                    wThreads[i].Name = "Писатель №" + i;
                    wThreads[i].Start(testData[i]);
                }

                for (int i = 0; i < rThreads.Length; i++)
                {
                    rThreads[i] = new Thread(example.ReadConcurrent);
                    rThreads[i].Name = "Читатель №" + i;
                    rThreads[i].Start();
                }

                Array.ForEach(wThreads, (Thread writer) => { writer.Join(); });

                // окончание работы писателей:
                example.m_bwFinish = true;

                Array.ForEach(rThreads, (Thread reader) => { reader.Join(); });
                sw.Stop();
                meanTime += (0 < tc) ? sw.Elapsed.TotalMilliseconds : 0;
            }
            return string.Format("Concurrent:\tчисло прогонов - {0}, число писателей - {1}, число читателей - {2}, средее время выполнение - {3} мс", testCount, testData.Length, readersCount, meanTime / (testCount - 1));
        }

        class TestData 
        {
            public string StringData { get; private set; }
            public uint RecordsCount { get; private set; }
            
            public TestData(string data, uint records)
            {
                StringData = data;
                RecordsCount = records;
            }
        }
    }
}