using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MTh_Ch_Lab2.MyMath;

namespace MTh_Ch_Lab2
{
    /// <summary>
    /// Задание №5 - последовательный перебор простых чисел потоками
    /// </summary>
    class Task_4Handler : TaskHandler
    {
        public Task_4Handler(uint n, uint countThread) :base(n, countThread) { }


        protected override void MultiStepCalculation()
        {
            // получить базовые простые числа:
            List<uint> primes = EratosthenesAlgorithm(m_sqrtN);

            // формирование интервала поиска и массива флагов простых чисел:
            IntervalValue interval = new IntervalValue(m_sqrtN, m_N);
            bool[] notPrimes = new bool[interval.Max - interval.Min + 1];

            // формирование потоков выполнения:
            CalcTask_4 calcTask = new CalcTask_4(notPrimes, primes, interval, SecondStageSievingAlgorithm);
            Thread[] threads = new Thread[m_thrCount];
            for (int i = 0; i < m_thrCount; i++)
            {
                threads[i] = new Thread(calcTask.CalculateResetEvent);
                threads[i].Start();
            }
            Array.ForEach(threads, (Thread thred) => { thred.Join(); });

            // обработка результатов расчета (получение простых чисел):
            for (uint i = 0; i < notPrimes.Length; i++)
            {
                if (!notPrimes[i])
                {
                    primes.Add(i + interval.Min);
                }
            }
            m_primes = primes;
        }

        
        private class CalcTask_4
        {
            private object m_basePrimesLock;    // замок на чтение базовых простых чисел

            private Queue<uint> m_basePrimes;   // коллекция базовых простых чисел
            private AutoResetEvent m_evlock;

            private ConcurrentQueue<uint> m_basePrimesC; // применение потокобезопасной коллекции
            
            private bool[] m_notPrimes;

            private IntervalValue m_calcSet;
            private Action<uint, bool[], uint, IntervalValue> m_calcFunct;


            public CalcTask_4(bool[] notPrimes, List<uint> basePrimes, IntervalValue calcSet, Action<uint, bool[], uint, IntervalValue> calcFunct)
            {
                m_basePrimesLock = new object();
                m_calcSet = calcSet;
                m_calcFunct = calcFunct;
                m_basePrimes = new Queue<uint>(basePrimes);
                m_basePrimesC = new ConcurrentQueue<uint>(basePrimes);
                m_notPrimes = notPrimes;
                m_evlock = new AutoResetEvent(true);
            }


            // использование конструкции lock:
            public void Calculate()
            {
                uint basePrime = 0;
                while (true)
                {
                    lock (m_basePrimesLock)
                    {
                        if (m_basePrimes.Count != 0)
                        {
                            basePrime = m_basePrimes.Dequeue();
                        }
                        else
                        {
                            break;
                        }
                    }
                    m_calcFunct(m_calcSet.Min, m_notPrimes, basePrime, m_calcSet);
                }
            }


            // использование потокобезопасной коллекции:
            public void CalculateConcurrent()
            {
                uint basePrime = 0;
                while (m_basePrimesC.Count != 0)
                {
                    if (m_basePrimesC.TryDequeue(out basePrime))
                    {
                        m_calcFunct(m_calcSet.Min, m_notPrimes, basePrime, m_calcSet);
                    }
                }
            }


            // использование AutoResetEvent:
            public void CalculateResetEvent()
            {
                uint basePrime = 0;
                while (m_basePrimes.Count != 0)
                {
                    m_evlock.WaitOne();
                    if (m_basePrimes.Count != 0)
                    {
                        basePrime = m_basePrimes.Dequeue();
                        m_evlock.Set();
                        m_calcFunct(m_calcSet.Min, m_notPrimes, basePrime, m_calcSet);
                    }
                }
                m_evlock.Set();
            }
        }
    }
}
