namespace ExcelExport
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ExcelPath = new System.Windows.Forms.TextBox();
            this.codePath = new System.Windows.Forms.TextBox();
            this.JsonPath = new System.Windows.Forms.TextBox();
            this.OpenExcel = new System.Windows.Forms.Button();
            this.SaveXml = new System.Windows.Forms.Button();
            this.SaveCode = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.formTimer = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.errorNum = new System.Windows.Forms.Label();
            this.StopBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExcelPath
            // 
            this.ExcelPath.Location = new System.Drawing.Point(350, 48);
            this.ExcelPath.Name = "ExcelPath";
            this.ExcelPath.Size = new System.Drawing.Size(240, 21);
            this.ExcelPath.TabIndex = 0;
            // 
            // codePath
            // 
            this.codePath.Location = new System.Drawing.Point(350, 203);
            this.codePath.Name = "codePath";
            this.codePath.Size = new System.Drawing.Size(240, 21);
            this.codePath.TabIndex = 1;
            // 
            // JsonPath
            // 
            this.JsonPath.Location = new System.Drawing.Point(350, 126);
            this.JsonPath.Name = "JsonPath";
            this.JsonPath.Size = new System.Drawing.Size(240, 21);
            this.JsonPath.TabIndex = 2;
            // 
            // OpenExcel
            // 
            this.OpenExcel.Location = new System.Drawing.Point(643, 45);
            this.OpenExcel.Name = "OpenExcel";
            this.OpenExcel.Size = new System.Drawing.Size(75, 23);
            this.OpenExcel.TabIndex = 3;
            this.OpenExcel.Text = "Open";
            this.OpenExcel.UseVisualStyleBackColor = true;
            this.OpenExcel.Click += new System.EventHandler(this.OpenExcel_Click);
            // 
            // SaveXml
            // 
            this.SaveXml.Location = new System.Drawing.Point(643, 126);
            this.SaveXml.Name = "SaveXml";
            this.SaveXml.Size = new System.Drawing.Size(75, 23);
            this.SaveXml.TabIndex = 4;
            this.SaveXml.Text = "Save";
            this.SaveXml.UseVisualStyleBackColor = true;
            this.SaveXml.Click += new System.EventHandler(this.SaveXml_Click);
            // 
            // SaveCode
            // 
            this.SaveCode.Location = new System.Drawing.Point(643, 201);
            this.SaveCode.Name = "SaveCode";
            this.SaveCode.Size = new System.Drawing.Size(75, 23);
            this.SaveCode.TabIndex = 5;
            this.SaveCode.Text = "Save";
            this.SaveCode.UseVisualStyleBackColor = true;
            this.SaveCode.Click += new System.EventHandler(this.SaveDTC_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(351, 343);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(240, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(615, 328);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(72, 47);
            this.Start.TabIndex = 7;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(349, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Excel 目录：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(349, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "JSON导出目录：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(349, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "代码输出目录：";
            // 
            // formTimer
            // 
            this.formTimer.Tick += new System.EventHandler(this.formTimer_Tick);
            // 
            // listBox1
            // 
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(21, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(309, 388);
            this.listBox1.TabIndex = 11;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseMove);
            // 
            // errorNum
            // 
            this.errorNum.Location = new System.Drawing.Point(21, 407);
            this.errorNum.Name = "errorNum";
            this.errorNum.Size = new System.Drawing.Size(309, 15);
            this.errorNum.TabIndex = 12;
            this.errorNum.Text = "errorNum";
            // 
            // StopBtn
            // 
            this.StopBtn.Location = new System.Drawing.Point(700, 328);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(72, 47);
            this.StopBtn.TabIndex = 13;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.errorNum);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.SaveCode);
            this.Controls.Add(this.SaveXml);
            this.Controls.Add(this.OpenExcel);
            this.Controls.Add(this.JsonPath);
            this.Controls.Add(this.codePath);
            this.Controls.Add(this.ExcelPath);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 480);
            this.MinimumSize = new System.Drawing.Size(800, 480);
            this.Name = "Form1";
            this.Text = "ExcelExport";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExcelPath;
        private System.Windows.Forms.TextBox codePath;
        private System.Windows.Forms.TextBox JsonPath;
        private System.Windows.Forms.Button OpenExcel;
        private System.Windows.Forms.Button SaveXml;
        private System.Windows.Forms.Button SaveCode;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer formTimer;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label errorNum;
        private System.Windows.Forms.Button StopBtn;
    }
}

