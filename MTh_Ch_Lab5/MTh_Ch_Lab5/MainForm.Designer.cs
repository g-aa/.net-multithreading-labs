using System.Windows.Forms;

namespace MTh_Ch_Lab5
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_btn_generateSet = new System.Windows.Forms.Button();
            this.m_btn_cancel = new System.Windows.Forms.Button();
            this.m_btn_sort = new System.Windows.Forms.Button();
            this.m_cbx_setType = new System.Windows.Forms.ComboBox();
            this.m_tbx_setMaxValue = new System.Windows.Forms.TextBox();
            this.m_tbx_setCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.m_tbx_inputSet = new System.Windows.Forms.TextBox();
            this.m_tbx_sortedSet = new System.Windows.Forms.TextBox();
            this.m_prgBar = new System.Windows.Forms.ProgressBar();
            this.m_cbx_sortType = new System.Windows.Forms.ComboBox();
            this.m_lbl_progress = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.m_tbx_timeDelay = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_btn_generateSet
            // 
            this.m_btn_generateSet.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_btn_generateSet.Location = new System.Drawing.Point(338, 12);
            this.m_btn_generateSet.Name = "m_btn_generateSet";
            this.m_btn_generateSet.Size = new System.Drawing.Size(148, 34);
            this.m_btn_generateSet.TabIndex = 0;
            this.m_btn_generateSet.Text = "сгенерировать";
            this.m_btn_generateSet.UseVisualStyleBackColor = true;
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_btn_cancel.Location = new System.Drawing.Point(853, 52);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(148, 34);
            this.m_btn_cancel.TabIndex = 1;
            this.m_btn_cancel.Text = "отмена";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            // 
            // m_btn_sort
            // 
            this.m_btn_sort.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_btn_sort.Location = new System.Drawing.Point(853, 12);
            this.m_btn_sort.Name = "m_btn_sort";
            this.m_btn_sort.Size = new System.Drawing.Size(148, 34);
            this.m_btn_sort.TabIndex = 2;
            this.m_btn_sort.Text = "сортировать";
            this.m_btn_sort.UseVisualStyleBackColor = true;
            // 
            // m_cbx_setType
            // 
            this.m_cbx_setType.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_cbx_setType.FormattingEnabled = true;
            this.m_cbx_setType.Location = new System.Drawing.Point(211, 82);
            this.m_cbx_setType.Name = "m_cbx_setType";
            this.m_cbx_setType.Size = new System.Drawing.Size(121, 27);
            this.m_cbx_setType.TabIndex = 3;
            // 
            // m_tbx_setMaxValue
            // 
            this.m_tbx_setMaxValue.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_tbx_setMaxValue.Location = new System.Drawing.Point(211, 44);
            this.m_tbx_setMaxValue.Name = "m_tbx_setMaxValue";
            this.m_tbx_setMaxValue.Size = new System.Drawing.Size(121, 26);
            this.m_tbx_setMaxValue.TabIndex = 7;
            // 
            // m_tbx_setCount
            // 
            this.m_tbx_setCount.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_tbx_setCount.Location = new System.Drawing.Point(211, 12);
            this.m_tbx_setCount.Name = "m_tbx_setCount";
            this.m_tbx_setCount.Size = new System.Drawing.Size(121, 26);
            this.m_tbx_setCount.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(16, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "Количество элементов";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 19);
            this.label2.TabIndex = 11;
            this.label2.Text = "Максимальное значение";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(106, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 19);
            this.label3.TabIndex = 12;
            this.label3.Text = "Тип набора";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(7, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 19);
            this.label4.TabIndex = 13;
            this.label4.Text = "Исходный набор:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(676, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 19);
            this.label5.TabIndex = 14;
            this.label5.Text = "Результат сортировки:";
            // 
            // m_tbx_generatedSet
            // 
            this.m_tbx_inputSet.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_tbx_inputSet.Location = new System.Drawing.Point(11, 148);
            this.m_tbx_inputSet.Multiline = true;
            this.m_tbx_inputSet.Name = "m_tbx_generatedSet";
            this.m_tbx_inputSet.ReadOnly = true;
            this.m_tbx_inputSet.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_tbx_inputSet.Size = new System.Drawing.Size(321, 313);
            this.m_tbx_inputSet.TabIndex = 17;
            // 
            // m_tbx_sortedSet
            // 
            this.m_tbx_sortedSet.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_tbx_sortedSet.Location = new System.Drawing.Point(680, 148);
            this.m_tbx_sortedSet.Multiline = true;
            this.m_tbx_sortedSet.Name = "m_tbx_sortedSet";
            this.m_tbx_sortedSet.ReadOnly = true;
            this.m_tbx_sortedSet.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_tbx_sortedSet.Size = new System.Drawing.Size(321, 313);
            this.m_tbx_sortedSet.TabIndex = 18;
            // 
            // m_prgBar
            // 
            this.m_prgBar.Location = new System.Drawing.Point(11, 467);
            this.m_prgBar.Name = "m_prgBar";
            this.m_prgBar.Size = new System.Drawing.Size(321, 23);
            this.m_prgBar.TabIndex = 19;
            // 
            // m_cbx_sortType
            // 
            this.m_cbx_sortType.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_cbx_sortType.FormattingEnabled = true;
            this.m_cbx_sortType.Location = new System.Drawing.Point(726, 11);
            this.m_cbx_sortType.Name = "m_cbx_sortType";
            this.m_cbx_sortType.Size = new System.Drawing.Size(121, 27);
            this.m_cbx_sortType.TabIndex = 20;
            // 
            // m_lbl_progress
            // 
            this.m_lbl_progress.AutoSize = true;
            this.m_lbl_progress.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_lbl_progress.Location = new System.Drawing.Point(338, 471);
            this.m_lbl_progress.Name = "m_lbl_progress";
            this.m_lbl_progress.Size = new System.Drawing.Size(0, 19);
            this.m_lbl_progress.TabIndex = 21;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(396, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(324, 19);
            this.label6.TabIndex = 22;
            this.label6.Text = "Задержка на выполнение этапа [ ms ]";
            // 
            // m_tbx_timeDelay
            // 
            this.m_tbx_timeDelay.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.m_tbx_timeDelay.Location = new System.Drawing.Point(726, 83);
            this.m_tbx_timeDelay.Name = "m_tbx_timeDelay";
            this.m_tbx_timeDelay.Size = new System.Drawing.Size(121, 26);
            this.m_tbx_timeDelay.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(585, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(135, 19);
            this.label7.TabIndex = 24;
            this.label7.Text = "Тип сортировки";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 502);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.m_tbx_timeDelay);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.m_lbl_progress);
            this.Controls.Add(this.m_cbx_sortType);
            this.Controls.Add(this.m_prgBar);
            this.Controls.Add(this.m_tbx_sortedSet);
            this.Controls.Add(this.m_tbx_inputSet);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_tbx_setCount);
            this.Controls.Add(this.m_tbx_setMaxValue);
            this.Controls.Add(this.m_cbx_setType);
            this.Controls.Add(this.m_btn_sort);
            this.Controls.Add(this.m_btn_cancel);
            this.Controls.Add(this.m_btn_generateSet);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button m_btn_generateSet;
        private Button m_btn_cancel;
        private Button m_btn_sort;
        private ComboBox m_cbx_setType;
        private TextBox m_tbx_setMaxValue;
        private TextBox m_tbx_setCount;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox m_tbx_inputSet;
        private TextBox m_tbx_sortedSet;
        private ProgressBar m_prgBar;
        private ComboBox m_cbx_sortType;
        private Label m_lbl_progress;
        private Label label6;
        private TextBox m_tbx_timeDelay;
        private Label label7;
    }
}

