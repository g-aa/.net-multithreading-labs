using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTh_Ch_Lab2
{
    public static class MyMath
    {
        /// <summary>
        /// Интервальная величина для задания №2
        /// </summary>
        public struct IntervalValue
        {
            public uint Min { get; set; }
            public uint Max { get; set; }

            public IntervalValue(uint min, uint max)
            {
                Max = min < max ? max : min;
                Min = min < max ? min : max;
            }

            public override string ToString() => string.Format("min: {0}, max: {1}", Min, Max);
        }


        /// <summary>
        /// Алгоритм Эратосфена (задание №1)
        /// </summary>
        /// <param name="n">Натуральное число</param>
        /// <returns>Коллекция простых чисел</returns>
        public static List<uint> EratosthenesAlgorithm(uint n)
        {    
            if (2 < n)
            {
                List<uint> result = new List<uint>();
                bool[] notPrimes = new bool[n + 1]; // все числа простые (не учитываются четные)!!!
                result.Add(2);
                for (uint i = 3; i < notPrimes.Length; i += 2)
                {
                    if (!notPrimes[i]) // проверка простое ли число
                    {
                        for (uint j = 1; i * j < notPrimes.Length; j += 2)
                        {
                            notPrimes[i * j] = true;
                        }
                        result.Add(i);
                    }
                }
                return result;
            }
            return new List<uint>();
        }


        /// <summary>
        /// Модифицированный алгоритм Эратосфена (в два этапа) (задание №1)
        /// </summary>
        /// <param name="n">Натуральное число</param>
        /// <returns>Коллекция простых чисел</returns>
        public static List<uint> ModEratosthenesAlgorithm(uint n)
        {
            List<uint> primes = null;
            if (n > 10)
            {
                // предворительный поиск простых чисел:
                var sqrtN = (uint)Math.Sqrt(n);
                primes = MyMath.EratosthenesAlgorithm(sqrtN);

                // флаги второго прогона: 
                // false - число простое 
                // true - число составное
                bool[] notPrimes = new bool[n - sqrtN + 1];
                foreach (uint prime in primes)
                {
                    uint stop = (n - sqrtN) - n % prime;
                    for (uint i = (sqrtN % prime) != 0 ? (prime - sqrtN % prime) : 0; i <= stop; i += prime)
                    {
                        notPrimes[i] = true;
                    }
                }

                // выгрузка простых чисел:
                for (uint i = 0; i < notPrimes.Length; i++)
                {
                    if (!notPrimes[i])
                    {
                        primes.Add(i + sqrtN);
                    }
                }
            }
            else 
            {
                primes = MyMath.EratosthenesAlgorithm(n);
            }
            return primes;
        }


        /// <summary>
        /// Поиск простых чисел в заданном интервале (задание №2 - декомпозиция по набору данных)
        /// </summary>
        /// <param name="basePrimes">Базовый комплекс простых чисел</param>
        /// <param name="interval">Интервал поиска</param>
        /// <returns>Коллекция простых чисел заданного интервалла поиска</returns>
        public static List<uint> DoubleScreeningAlgorithm(List<uint> basePrimes, IntervalValue interval)
        {
            if (basePrimes.Max<uint>() <= interval.Min)
            {
                List<uint> primes = new List<uint>();

                // флаги второго прогона: 
                // false - число простое 
                // true - число составное
                bool[] notPrimes = new bool[interval.Max - interval.Min + 1];
                notPrimes[0] = true;
                foreach (uint prime in basePrimes)
                {
                    uint i = (interval.Min % prime) != 0 ? (prime - interval.Min % prime) : 0;
                    uint stop = (interval.Max - interval.Min) - interval.Max % prime;
                    for (; i <= stop; i += prime)
                    {
                        notPrimes[i] = true;
                    }
                }

                // выгрузка простых чисел:
                for (uint i = 0; i < notPrimes.Length; i++)
                {
                    if (!notPrimes[i])
                    {
                        primes.Add(i + interval.Min);
                    }
                }
                return primes;
            }
            return new List<uint>();
        }


        /// <summary>
        /// Второй этап алгоритма просеивания Эратосфена (для задания №)
        /// </summary>
        /// <param name="offset">Смещение, целое число с которого начинается поиск простых чисел</param>
        /// <param name="notPrimes">Массив флагов простых чисел в исследуемом интервале interval</param>
        /// <param name="basePrime">Базовое простое число в интервалле [2, sqrt(N)]</param>
        /// <param name="interval">Интервалл поиска простых чисел</param>
        public static void SecondStageSievingAlgorithm(uint offset, bool[] notPrimes, uint basePrime, IntervalValue interval)
        {
            // флаги второго прогона: 
            // false - число простое 
            // true - число составное
            if (basePrime <= offset && offset <= interval.Min)
            {
                notPrimes[0] = true;
                uint i = (interval.Min % basePrime) != 0 ? (basePrime - interval.Min % basePrime) : 0; // ближайшее непростое число
                uint stop = (interval.Max - interval.Min) - interval.Max % basePrime; // крайнее непростое число
                for (; i <= stop; i += basePrime)
                {
                    notPrimes[i] = true; // непростое число
                }
            }
        }


        /// <summary>
        /// Второй этап алгоритма просеивания Эратосфена (для задания №3)
        /// </summary>
        /// <param name="offset">Смещение, целое число с которого начинается поиск простых чисел</param>
        /// <param name="notPrimes">Массив флагов простых чисел в исследуемом интервале interval</param>
        /// <param name="basePrimes">Список базовох простых чисел в интервалле [2, sqrt(N)]</param>
        /// <param name="interval">Интервалл поиска простых чисел</param>
        public static void SecondStageSievingAlgorithm(uint offset, bool[] notPrimes, List<uint> basePrimes, IntervalValue interval)
        {
            // флаги второго прогона:
            // false - число простое
            // true - число составное
            if (basePrimes.Max() <= offset && offset <= interval.Min)
            {
                notPrimes[0] = true;
                foreach (uint prime in basePrimes)
                {
                    uint i = (interval.Min % prime) != 0 ? (prime - interval.Min % prime) : 0; // ближайшее непростое число
                    uint stop = (interval.Max - interval.Min) - interval.Max % prime; // крайнее непростое число
                    for (; i <= stop; i += prime)
                    {
                        notPrimes[i] = true; // непростое число
                    }
                }
            }
        }
    }
}
