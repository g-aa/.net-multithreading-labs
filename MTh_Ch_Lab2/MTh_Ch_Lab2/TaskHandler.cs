using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MTh_Ch_Lab2
{
    abstract class TaskHandler
    {
        protected uint m_N; // максимальное число исследуемых элементов
        protected uint m_sqrtN; // квадратный корень из числа иследуемых элементов

        protected const uint m_thrCapacity = 8; // максимальное число потоков для расчетов
        protected uint m_thrCount; // число используемых потоков для расчетов

        private Stopwatch m_sWatch;
        protected double m_calcTimeMs; // время выполнения ( ms )

        protected List<uint> m_primes; // простые числа


        public TaskHandler(uint n, uint countThread)
        {
            m_N = (10 < n) ? n : 10;
            m_sqrtN = (uint)Math.Sqrt(m_N);
            m_thrCount = (1 < countThread && countThread <= m_thrCapacity) ? countThread : 2;
            m_sWatch = new Stopwatch();
            m_calcTimeMs = 0;
            m_primes = new List<uint>();
        }


        public uint GetPrimesCount() => (uint)m_primes.Count;

        public List<uint> GetPrimes() => new List<uint>(m_primes);

        public double GetTotalMilliseconds() => m_calcTimeMs;

        public uint GetLimitNumber() => m_N;

        public uint GetThreadCount() => m_thrCount;


        public bool SetLimitNumber(uint n)
        {
            if (10 < n)
            {
                m_N = n;
                m_sqrtN = (uint)Math.Sqrt(m_N);
                return true;
            }
            return false;
        }

        public bool SetThreadCount(uint thrCount)
        {
            if (1 < thrCount && thrCount <= m_thrCapacity)
            {
                m_thrCount = thrCount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Запуск на выполнение вычислительной задачи
        /// </summary>
        public void CalculateTask()
        {
            m_sWatch.Start();
            if (100 <= m_N)
            {
                this.MultiStepCalculation();
            }
            else
            {
                m_primes = MyMath.EratosthenesAlgorithm(m_sqrtN);
            }
            m_sWatch.Stop();
            m_calcTimeMs = m_sWatch.Elapsed.TotalMilliseconds;
            m_sWatch.Reset();
        }


        /// <summary>
        /// Многошаговый расчет для заданий №2 - №4 реализуется в дочерних класса
        /// </summary>
        protected abstract void MultiStepCalculation();


        public override string ToString()
        {
            string format = "Результат вычислений: число потоков -{0,2}, число элементов -{1,10}, количество простых -{2,10}, время выполнения - {3} ms";
            return string.Format(format, m_thrCount, m_N, m_primes.Count, m_calcTimeMs);
        }
    }
}
