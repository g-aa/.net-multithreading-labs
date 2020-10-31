using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTh_Ch_Lab3
{
    class Writer
    {
        readonly private DataBuffer m_dataBuffer;

        readonly private string m_wName;
        
        private Queue<string> m_wBuffer;

        
        public Writer(string name, DataBuffer dataBuffer, string[] stringsData)
        {
            if (name is null || string.Empty.Equals(name) || dataBuffer is null || stringsData is null)
            {
                throw new ArgumentNullException("name, dataBuffer или stringsData = null, или name = string.Empty!");
            }

            m_wName = name;
            m_dataBuffer = dataBuffer;
            m_wBuffer = new Queue<string>(stringsData);
        }

        public Writer(string name, DataBuffer dataBuffer, string data, uint dataCount)
        {
            if (name is null || string.Empty.Equals(name) || dataBuffer is null || data is null || string.Empty.Equals(data) || dataCount == 0)
            {
                throw new ArgumentNullException("name, dataBuffer, data = null, или name, data = string.Empty, или dataCount = 0!");
            }

            m_wName = name;
            m_dataBuffer = dataBuffer;
            m_wBuffer = new Queue<string>((int)dataCount);
            for (uint i = 0; i < dataCount; i++)
            {
                m_wBuffer.Enqueue(string.Format("{0} - {1}", data, i + 1));
            }
        }


        public string GetName() => m_wName;

        public int GetValueCount() => m_wBuffer.Count;

        public void RunToWrite()
        {
            while (0 < m_wBuffer.Count)
            {
                if (m_dataBuffer.WriteValue(m_wBuffer.Peek()))
                {
                    string sTemp = m_wBuffer.Dequeue();
                    // string format = "{0}:\t{1}\n";
                    // Console.WriteLine(string.Format(format, Thread.CurrentThread.Name, sTemp));
                    if (m_wBuffer.Count == 0)
                    {
                        m_dataBuffer.RemoweWriter(this);
                    }
                }
            }
        }
    }
}
