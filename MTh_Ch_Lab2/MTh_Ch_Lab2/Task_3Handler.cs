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
    /// Задание №4 - применение пула потоков для расчета
    /// </summary>
    class Task_3Handler : TaskHandler
    {
        public Task_3Handler(uint n, uint countThread) :base(n, countThread) { }


        protected override void MultiStepCalculation()
        {
            // получить базовые простые числа:
            List<uint> primes = EratosthenesAlgorithm(m_sqrtN);

            // формирование интервала поиска и массива флагов простых чисел:
            IntervalValue interval = new IntervalValue(m_sqrtN, m_N);
            bool[] notPrimes = new bool[interval.Max - interval.Min + 1];

            // Объявляем массив сигнальных сообщений и создание вычислительных задач:
            ManualResetEvent[] events = new ManualResetEvent[primes.Count];
            CalcTask_3[] calcs = new CalcTask_3[primes.Count];

            // запуск задач на выполнение:
            for (int i = 0; i < primes.Count; i++)
            {
                calcs[i] = new CalcTask_3(notPrimes, primes[i], interval, SecondStageSievingAlgorithm);
                events[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(calcs[i].Calculate, events[i]);
            }
            // WaitHandle.WaitAll(events);
            Array.ForEach(events, (ManualResetEvent ev) => { ev.WaitOne(); });

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

        
        private class CalcTask_3
        {
            private uint m_basePrime;
            private IntervalValue m_calcSet;
            private Action<uint, bool[], uint, IntervalValue> m_calcFunct;

            private bool[] m_notPrimes;
            

            public CalcTask_3(bool[] notPrimes, uint basePrime, IntervalValue calcSet, Action<uint, bool[], uint, IntervalValue> calcFunct)
            {
                m_calcSet = calcSet;
                m_calcFunct = calcFunct;
                m_basePrime = basePrime;
                m_notPrimes = notPrimes;
            }


            public void Calculate(object obj)
            {
                ManualResetEvent resetEvent = (ManualResetEvent)obj;
                m_calcFunct(m_calcSet.Min, m_notPrimes, m_basePrime, m_calcSet);
                resetEvent.Set();
            }
        }
    }
}
