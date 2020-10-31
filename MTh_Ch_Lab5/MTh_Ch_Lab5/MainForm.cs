using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTh_Ch_Lab5
{
    public partial class MainForm : Form
    {
        private double m_calcTime_ms; // время выполнения расчета [ ms ]
        private int m_timeDelay_ms;  // задержка в выполнении отдельных операций для тестирования UI [ ms ]  

        private int m_maxTimeDelay_ms = 10_000; // максимальная задержка [10 s]
        private int m_minSetCount = 100;  // минимально возможное количество элементов для сортировки
        private int m_maxSetCount = 2_000_000;  // максимальное число элементов в наборе


        private int m_setMaxValue;  // максимальное значение элемента из набора
        private int m_setCount;     // количество элементов в наборе
        
        private NewMySorts.SetType m_setType; // тип используемого набора при генерации
        private NewMySorts.SortType m_sortType; // тип функции сортировки

        // входной набор сортируемых чисел:
        string m_txtInputSet;
        private List<int> m_inputSet;

        // отсортированный набор чисел:
        string m_txtSortedSet;
        private List<int> m_sortedSet;

        // для асинхронной обработки:
        private CancellationTokenSource m_cTokenSource;
        private CancellationToken m_token;

        // фоновые задачи:
        private BackgroundWorker m_bgw_generateSet;
        private BackgroundWorker m_bgw_sortSet;


        public MainForm(int setCount, int setMaxValue, NewMySorts.SetType setType, NewMySorts.SortType sortType, int timeDelayMs)
        {
            InitializeComponent();

            m_calcTime_ms = 0;
            m_timeDelay_ms = (0 <= timeDelayMs && timeDelayMs <= m_maxTimeDelay_ms) ? timeDelayMs : 0;
            m_setCount = (m_minSetCount <= setCount && setCount <= m_maxSetCount) ? setCount : m_minSetCount;
            m_setMaxValue = (m_minSetCount <= setMaxValue && setMaxValue <= m_maxSetCount) ? setMaxValue : m_minSetCount;

            m_setType = setType;
            m_sortType = sortType;

            m_inputSet = new List<int>();
            m_sortedSet = new List<int>();

            m_btn_generateSet.Click += m_btn_generateSet_Click;
            m_btn_cancel.Enabled = false;
            m_btn_cancel.Click += m_btn_cancel_Click;
            m_btn_sort.Click += m_btn_sortSet_Click;

            m_tbx_setMaxValue.Text = m_setMaxValue.ToString();
            m_tbx_setMaxValue.TextChanged += m_tbx_setMaxValue_TextChanged;

            m_tbx_setCount.Text = m_setCount.ToString();
            m_tbx_setCount.TextChanged += m_tbx_setCount_TextChanged;

            m_tbx_timeDelay.Text = m_timeDelay_ms.ToString();
            m_tbx_timeDelay.TextChanged += m_tbx_timeDelay_TextChanged;

            m_cbx_setType.Items.AddRange(Enum.GetNames(typeof(NewMySorts.SetType)));
            m_cbx_setType.SelectedIndex = (int)m_setType;
            m_cbx_setType.SelectedValueChanged += m_cbx_setType_SelectedValueChanged;

            m_cbx_sortType.Items.AddRange(Enum.GetNames(typeof(NewMySorts.SortType)));
            m_cbx_sortType.SelectedIndex = (int)m_sortType;
            m_cbx_sortType.SelectedValueChanged += m_cbx_sortType_SelectedValueChanged;

            m_bgw_generateSet = new BackgroundWorker();
            m_bgw_generateSet.WorkerReportsProgress = true;
            m_bgw_generateSet.WorkerSupportsCancellation = true;
            m_bgw_generateSet.DoWork += m_bgw_generateSet_DoWork;
            m_bgw_generateSet.ProgressChanged += m_bgw_generateSet_ProgressChanged;
            m_bgw_generateSet.RunWorkerCompleted += m_bgw_generateSet_RunWorkerCompleted;

            m_bgw_sortSet = new BackgroundWorker();
            m_bgw_sortSet.WorkerReportsProgress = true;
            m_bgw_sortSet.WorkerSupportsCancellation = true;
            m_bgw_sortSet.DoWork += m_bgw_sortSet_DoWork;
            m_bgw_sortSet.ProgressChanged += m_bgw_sortSet_ProgressChanged;
            m_bgw_sortSet.RunWorkerCompleted += m_bgw_sortSet_RunWorkerCompleted;

            m_cTokenSource = new CancellationTokenSource();
            m_token = m_cTokenSource.Token;
        }

        


        #region bgw_sortSet

        private void m_bgw_sortSet_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            m_txtSortedSet = string.Empty;
            m_sortedSet.Clear();
            m_sortedSet = NewMySorts.SampleSort(m_inputSet, m_sortType, worker, e, m_token, m_timeDelay_ms);
            
            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }

            worker.ReportProgress(6);
            if (0 < m_timeDelay_ms)
            {
                Thread.Sleep(m_timeDelay_ms);
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }
            sw.Stop();

            m_txtSortedSet = ListToString(m_sortedSet);
            m_calcTime_ms = sw.Elapsed.TotalMilliseconds;
        }

        private void m_bgw_sortSet_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    if (m_sortType == NewMySorts.SortType.Serial)
                    {
                        m_prgBar.Value = 50;
                        m_lbl_progress.Text = "Сортировка: сортировка набора...";
                    }
                    else
                    {
                        m_prgBar.Value = 0;
                        m_lbl_progress.Text = "Сортировка: подготовка индексов для расчета...";
                    }
                    m_tbx_sortedSet.Text = string.Empty;
                    break;
                case 1:
                    m_prgBar.Value = 15;
                    m_lbl_progress.Text = "Сортировка: формирование блоков для предварительного расчета...";
                    break;
                case 2:
                    m_prgBar.Value = 30;
                    m_lbl_progress.Text = "Сортировка: предварительная сортировка блоков...";
                    break;
                case 3:
                    m_prgBar.Value = 45;
                    m_lbl_progress.Text = "Сортировка: подготовка глобальных образцов...";
                    break;
                case 4:
                    m_prgBar.Value = 60;
                    m_lbl_progress.Text = "Сортировка: формирование блоков для финальной сортировки...";
                    break;
                case 5:
                    m_prgBar.Value = 75;
                    m_lbl_progress.Text = "Сортировка: заключительная сортировка блоков...";
                    break;
                case 6:
                    m_prgBar.Value = 85;
                    m_lbl_progress.Text = "Сортировка: вывод набора в UI...";
                    break;
            }
        }

        private void m_bgw_sortSet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                m_prgBar.Value = 0;
                m_lbl_progress.Text = "Сортировка: операция отменена!";
            }
            else if (e.Error != null)
            {
                m_prgBar.Value = 0;
                m_lbl_progress.Text = "Сортировка: аварийное завершение операции!";
                MessageBox.Show(e.Error.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                m_btn_cancel.Enabled = false;
                m_tbx_sortedSet.Text = m_txtSortedSet;
                m_prgBar.Value = 100;
                m_lbl_progress.Text = string.Format("Сортировка: операция завершена! ({0} ms)", m_calcTime_ms);
            }
            m_cTokenSource.Dispose();
        }

        #endregion


        #region bgw_generateSet

        private void m_bgw_generateSet_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            CancellationTokenSource cTokenSource = new CancellationTokenSource();
            CancellationToken token = cTokenSource.Token;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            worker.ReportProgress(0);
            if (0 < m_timeDelay_ms)
            {
                Thread.Sleep(m_timeDelay_ms);
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }

            m_txtSortedSet = string.Empty;
            m_sortedSet.Clear();

            m_txtInputSet = string.Empty;
            m_inputSet.Clear();
            m_inputSet = NewMySorts.GenerateTestIntegersSet(m_setCount, m_setMaxValue, m_setType, token);

            worker.ReportProgress(1);
            if (0 < m_timeDelay_ms)
            {
                Thread.Sleep(m_timeDelay_ms);
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }
            sw.Stop();

            m_txtInputSet = ListToString(m_inputSet);
            m_calcTime_ms = sw.Elapsed.TotalMilliseconds;
        }

        private void m_bgw_generateSet_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 0:
                    m_prgBar.Value = 0;
                    m_lbl_progress.Text = "Генерация: формирование набора чисел...";
                    m_tbx_inputSet.Text = string.Empty;
                    break;
                case 1:
                    m_prgBar.Value = 50;
                    m_lbl_progress.Text = "Генерация: вывод набора в UI...";
                    break;
            }
        }

        private void m_bgw_generateSet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                m_prgBar.Value = 0;
                m_lbl_progress.Text = "Генерация: операция отменена!";
            }
            else if (e.Error != null)
            {
                m_prgBar.Value = 0;
                m_lbl_progress.Text = "Генерация: аварийное завершение операции!";
                MessageBox.Show(e.Error.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                m_tbx_inputSet.Text = m_txtInputSet;
                m_tbx_sortedSet.Text = string.Empty;
                m_prgBar.Value = 100;
                m_lbl_progress.Text = string.Format("Генерация: формирование набора завершено! ({0} ms)", m_calcTime_ms);
            }
        }
        #endregion


        #region combobox
        private void m_cbx_sortType_SelectedValueChanged(object sender, EventArgs e)
        {
            m_sortType = (NewMySorts.SortType)((ComboBox)sender).SelectedIndex;
        }

        private void m_cbx_setType_SelectedValueChanged(object sender, EventArgs e)
        {
            m_setType = (NewMySorts.SetType)((ComboBox)sender).SelectedIndex;
        }
        #endregion


        #region textBox
        private void m_tbx_setCount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                m_setCount = int.Parse(((TextBox)sender).Text);
            }
            catch (Exception exp)
            {
                m_tbx_setCount.Text = m_setCount.ToString();
                MessageBox.Show(exp.Message, "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void m_tbx_setMaxValue_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int temp = int.Parse(((TextBox)sender).Text);
                if (!(m_minSetCount <= temp && temp <= m_maxSetCount))
                {
                    throw new Exception(string.Format("Максимальный элемент из набора может принимать значения из интервала [{0} - {1}]!", m_minSetCount, m_maxSetCount));
                }
                m_setMaxValue = temp;
            }
            catch (Exception exp)
            {
                m_tbx_setMaxValue.Text = m_setMaxValue.ToString();
                MessageBox.Show(exp.Message, "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void m_tbx_timeDelay_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int temp = int.Parse(((TextBox)sender).Text);
                if (!(0 <= temp && temp <= m_maxTimeDelay_ms))
                {
                    throw new Exception(string.Format("Задержка во времени может принимать значения из интервала [{0} - {1}]!", 0, m_maxTimeDelay_ms));
                }
                m_timeDelay_ms = temp;
            }
            catch (Exception exp)
            {
                m_tbx_timeDelay.Text = m_timeDelay_ms.ToString();
                MessageBox.Show(exp.Message, "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion


        #region button
        private void m_btn_generateSet_Click(object sender, EventArgs e)
        {
            // проверка на выполнение сортировки:
            if (m_bgw_sortSet.IsBusy != true)
            {
                // проверка выполняется ли генерация набора целых чисел:
                if (m_bgw_generateSet.IsBusy != true)
                {
                    // проверка на мин/макс коллчества элементов в наборе:
                    if (m_minSetCount <= m_setCount && m_setCount <= m_maxSetCount)
                    {
                        // проверка на соотношение числа элементов в наборе и максимально возможное значение элемента в наборе:
                        if (m_setCount <= m_setMaxValue)
                        {
                            m_bgw_generateSet.RunWorkerAsync(); // запуск на выполнение 
                        }
                        else
                        {
                            MessageBox.Show("Количество элементов в наборе должно быть не больше максимального значения элемента из набора!", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Количество элементов в наборе может принимать значения из интервала [{0} - {1}]!", m_minSetCount, m_maxSetCount), "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Операция генерации набора чисел запущена!", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Нельзя сгенерировать новый набор, выполняется сортировка!", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void m_btn_sortSet_Click(object sender, EventArgs e)
        {
            // проверка на выполнение операции генерации набора чисел:
            if (m_bgw_generateSet.IsBusy != true)
            {
                // проверка на выполнение сортировки:
                if (m_bgw_sortSet.IsBusy != true)
                {
                    // проверка сгенерировал ли набор:
                    if (m_inputSet.Count != 0)
                    {
                        m_btn_cancel.Enabled = true; // сделать кнопку отмены видимой
                        m_cTokenSource = new CancellationTokenSource();
                        m_token = m_cTokenSource.Token;
                        m_bgw_sortSet.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Отсутствует набор сортируемых чисел!", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Операция сортировки набора чисел уже запущена!", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Нельзя начать операцию сортировки, выполняется операция генерации набора чисел! ", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void m_btn_cancel_Click(object sender, EventArgs e)
        {
            if (m_bgw_sortSet.WorkerSupportsCancellation != false)
            {
                m_bgw_sortSet.CancelAsync();
                m_cTokenSource.Cancel();    // нужно еще токен сгенерить наверно
                m_btn_cancel.Enabled = false;
                MessageBox.Show("Операция сортировки набора чисел отменена!", "Информация!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion


        #region otherMethods
        private string ListToString(List<int> inputList)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inputList.Count; i++)
            {
                sb.AppendLine(i + ": " + inputList[i]);
            }
            return sb.ToString();
        }
        #endregion
    }
}
