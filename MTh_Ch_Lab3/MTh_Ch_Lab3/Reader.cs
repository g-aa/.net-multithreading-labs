using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab3
{
    class Reader
    {
        readonly private DataBuffer m_dataBuffer; // ссфлка на буфер с данными
        
        readonly string m_rName; // Наименование читателя

        private List<string> m_rBuffer; // буфер читателя


        public Reader(string name, DataBuffer dataBuffer)
        {
            if (name is null || string.Empty.Equals(name) || dataBuffer is null)
            {
                throw new ArgumentNullException("name или dataBuffer = null, или name = string.Empty!");
            }

            m_dataBuffer = dataBuffer;
            m_rName = name;
            m_rBuffer = new List<string>();
        }


        public string GetReaderName() => m_rName;

        public void RunToRead()
        {
            while (!m_dataBuffer.WriteIsFinished())
            {
                string sTemp = string.Empty;
                if (m_dataBuffer.ReadValue(ref sTemp))
                {
                    m_rBuffer.Add(sTemp);
                    // string format = "\t{0}:\t{1}\n";
                    // Console.WriteLine(string.Format(format, Thread.CurrentThread.Name, sTemp));
                }
            }
        }


        public override string ToString()
        {
            m_rBuffer.Sort();
            StringBuilder buffer = new StringBuilder(string.Format("{0} результат захвата данных:\n", m_rName));
            m_rBuffer.ForEach((string item) => { buffer.AppendLine(item); });
            return buffer.ToString();
        }
    }
}
