using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab3
{
    class DataBuffer
    {
        private string m_stringBuffer; // строковый буффер

        private bool m_bEmpty; // стапус буфера true - пустой / false - полный

        private List<Writer> m_writers; // регичтрация писателей

        private object m_rLock; // замок на чтение
        private object m_wLock; // замок на запись

        private AutoResetEvent m_evFull;
        private AutoResetEvent m_evEmpty;

        public DataBuffer()
        {
            m_stringBuffer = string.Empty;
            m_bEmpty = true; // пустой
            m_writers = new List<Writer>();

            m_rLock = new object();
            m_wLock = new object();

            m_evFull = new AutoResetEvent(false);
            m_evEmpty = new AutoResetEvent(true);
        }

        /// <summary>
        /// Добавить писателя для работы с буффером
        /// </summary>
        /// /// <param name="writer"></param>
        /// <returns>Возвращает true если писатель добавлен</returns>
        public bool AddNewWriter(Writer writer)
        {
            if (!m_writers.Contains(writer))
            {
                m_writers.Add(writer);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удалить писателя из списка писателей
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        public bool RemoweWriter(Writer writer)
        {
            if (m_writers.Contains(writer))
            {
                m_writers.Remove(writer);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool WriteIsFinished() => (m_writers.Count != 0)? false : true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() => m_bEmpty;


        public bool WriteValue(string stringValue)
        {
            lock (m_wLock)
            {
                if (m_bEmpty)
                {
                    m_stringBuffer = stringValue;
                    string format = "DataBuffer: {0}:\t{1}\n";
                    Console.WriteLine(string.Format(format, Thread.CurrentThread.Name, m_stringBuffer));
                    m_bEmpty = false; // буфер заполнен
                    return true; // запись прошла удачно 
                }
                return false; // запись прошла неудачно 
            }
        }

        public bool ReadValue(ref string stringValue)
        {
            lock (m_rLock)
            {
                if (!m_bEmpty)
                {
                    stringValue = m_stringBuffer;
                    string format = "\tDataBuffer: {0}:\t{1}\n";
                    Console.WriteLine(string.Format(format, Thread.CurrentThread.Name, m_stringBuffer));
                    m_bEmpty = true; // буфер пуст
                    return true; // чтение прошло успешно
                }
                return false; // чтение прошло неудачно
            }
        }
    }
}
