using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static MTh_Ch_Lab2.MyMath;

namespace MTh_Ch_Lab2
{
    /// <summary>
    /// Задание №3 - декомпозиция набора базовых простых чисел
    /// </summary>
    class Task_2Handler : TaskHandler
    {
        public Task_2Handler(uint n, uint countThread) : base(n, countThread) { }


        protected override void MultiStepCalculation()
        {
            // получить базовые простые числа:
            List<uint> basePrimes = EratosthenesAlgorithm(m_sqrtN);

            // число элементов на один поток:
            int thrsBPsCnt = basePrimes.Count / (int)m_thrCount;
            if (0 < thrsBPsCnt)
            {
                // формирование интервала поиска и массива флагов простых чисел:
                IntervalValue interval = new IntervalValue(m_sqrtN, m_N);
                bool[] notPrimes = new bool[interval.Max - interval.Min + 1];

                // дополнительнное колличество простых чисел на один поток (остаток от деления):
                int thrModBPsCnt = basePrimes.Count % (int)m_thrCount;

                // формирование групп простых чисел для отдельных потоков:
                List<uint>[] thrsBasePrime = new List<uint>[m_thrCount];
                Thread[] threads = new Thread[m_thrCount];
                CalcTask_2[] calcs = new CalcTask_2[m_thrCount];
                int offset = 0; // смещение при неравномерном распределении чисел по отдельным потокам
                for (int i = 0; i < m_thrCount; i++)
                {
                    if (0 < thrModBPsCnt)
                    {
                        thrsBasePrime[i] = new List<uint>(basePrimes.GetRange(offset, thrsBPsCnt + 1));
                        offset += thrsBPsCnt + 1;
                        thrModBPsCnt--;
                    }
                    else
                    {
                        thrsBasePrime[i] = new List<uint>(basePrimes.GetRange(offset, thrsBPsCnt));
                        offset += thrsBPsCnt;
                    }
                    calcs[i] = new CalcTask_2(m_sqrtN, notPrimes, thrsBasePrime[i], interval, SecondStageSievingAlgorithm);
                    threads[i] = new Thread(calcs[i].Calculate);
                    threads[i].Start();
                }
                Array.ForEach(threads, (Thread thred) => { thred.Join(); });

                // обработка результатов расчета (получение простых чисел):
                for (uint i = 0; i < notPrimes.Length; i++)
                {
                    if (!notPrimes[i])
                    {
                        basePrimes.Add(i + interval.Min);
                    }
                }
                m_primes = basePrimes;
            }
            else
            {
                throw new Exception("Число потоков больше числа обрабатываемых данных !!!");
            }
        }


        private class CalcTask_2
        {
            private uint m_offset;
            private bool[] m_notPrime;

            private List<uint> m_basePrime;
            private Action<uint, bool[], List<uint>,IntervalValue> m_calcFunct;
            private IntervalValue m_calcSet;

            
            public CalcTask_2(uint offset, bool[] notPrimes, List<uint> basePrimes, IntervalValue calcSet, Action<uint, bool[], List<uint>, IntervalValue> calcFunct)
            {
                m_offset = offset;
                m_notPrime = notPrimes;
                m_calcSet = calcSet;
                m_calcFunct = calcFunct;
                m_basePrime = basePrimes;
            }


            public void Calculate() => m_calcFunct(m_offset, m_notPrime, m_basePrime, m_calcSet);
        }
    }
}
