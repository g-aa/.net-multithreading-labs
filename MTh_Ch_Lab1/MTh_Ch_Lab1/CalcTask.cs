using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTh_Ch_Lab1
{
    /// <summary>
    /// Вычислительная задача
    /// </summary>
    public class CalcTask
    {
        private CalcParams m_calcParams;
        private double[] m_inputArray;  // ссылка на обрабатываемый обьект
        private double[] m_outputArray; // результат обработки

        private long m_time_ms; // время выполнения ( ms )
        private Stopwatch m_stopwatch;

        private Func<double[], CalcParams, double[]> m_calcFunct; // иследуемая функция


        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="inputArray">исследуемый массив</param>
        /// <param name="startIdx">стартовый индекс для расчета в массиве</param>
        /// <param name="length">длина</param>
        /// <param name="calcFunct">вычислительная функция</param>
        public CalcTask(double[] inputArray, CalcParams calcParams, Func<double[], CalcParams, double[]> calcFunct)
        {
            m_calcParams = calcParams;
            m_inputArray = inputArray;
            m_outputArray = new double[0];
            m_calcFunct = calcFunct;
            m_time_ms = -1;
            m_stopwatch = new Stopwatch();
        }


        /// <summary>
        /// Запуск вычислительной задачи
        /// </summary>
        public void Calculate()
        {
            m_stopwatch.Start();
            m_outputArray = m_calcFunct(m_inputArray, m_calcParams);
            m_stopwatch.Stop();
            m_time_ms = m_stopwatch.ElapsedMilliseconds;
        }


        /// <summary>
        /// Получить время выполнения вычислительной задачи ( ms )
        /// </summary>
        /// <return></return>
        public double CalcTimeMilliseconds() => m_time_ms;


        /// <summary>
        /// получить результат расчета
        /// </summary>
        /// <returns></returns>
        public double[] GetResult() => m_outputArray;

        /// <summary>
        /// Параметры для вычисления
        /// </summary>
        public class CalcParams
        {
            /// <summary>
            /// Стартовый индекс
            /// </summary>
            public int StartIndex { get; set; }

            /// <summary>
            /// Число элементов
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            ///Параметр сложности для вычислений / используется как шаг в задании №6
            /// </summary>
            public int K { get; set; }
        }
    }
}
