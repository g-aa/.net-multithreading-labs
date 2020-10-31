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
    /// Задание №2 - декомпозиция по набору данных (базовых простых чисел)
    /// </summary>
    class Task_1Handler : TaskHandler
    {
        public Task_1Handler(uint n, uint countThread) : base(n, countThread) { }


        protected override void MultiStepCalculation()
        {
            // подготовка интерваллов для расчета:
            IntervalValue[] intervals = new IntervalValue[m_thrCount];
            for (uint i = 0, offset = m_sqrtN, step = (m_N - m_sqrtN) / m_thrCount, mod = (m_N - m_sqrtN) % m_thrCount; i < m_thrCount; i++)
            { 
                intervals[i] = new IntervalValue(offset, offset += 0 < mod ? step + 1 : step);
                mod -= (0 < mod) ? (uint)1 : 0;
            }

            // получение базовых простых чисел:
            List<uint> primes = EratosthenesAlgorithm(m_sqrtN);

            // запуск алгоритма вторичного просеивания:
            CalcTask[] tasks = new CalcTask[m_thrCount];
            Thread[] threads = new Thread[m_thrCount];
            for (int i = 0; i < m_thrCount; i++)
            {
                tasks[i] = new CalcTask(primes, intervals[i], DoubleScreeningAlgorithm);
                threads[i] = new Thread(tasks[i].Calculate);
                threads[i].Start();
            }
            Array.ForEach(threads, (Thread thred) => { thred.Join(); });

            // получение полного набора простых чисел:
            Array.ForEach(tasks, (CalcTask task) => { primes.AddRange(task.GetResult()); });
            m_primes = primes;
        }


        private class CalcTask
        {
            private List<uint> m_basePrime;
            private IntervalValue m_calcSet;
            private Func<List<uint>, IntervalValue, List<uint>> m_calcFunct;

            private List<uint> m_calcResult;


            public CalcTask(List<uint> basePrimes, IntervalValue calcSet, Func<List<uint>, IntervalValue, List<uint>> calcFunct)
            {
                m_basePrime = basePrimes;
                m_calcSet = calcSet;
                m_calcFunct = calcFunct;
                m_calcResult = null;
            }


            public void Calculate() => m_calcResult = m_calcFunct(m_basePrime, m_calcSet);

            public List<uint> GetResult() => m_calcResult;
        }
    }
}
