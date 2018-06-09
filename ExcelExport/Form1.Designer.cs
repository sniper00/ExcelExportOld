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
            this.OpenExcel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.formTimer = new System.Windows.Forms.Timer(this.components);
            this.errorNum = new System.Windows.Forms.Label();
            this.StopBtn = new System.Windows.Forms.Button();
            this.minijson = new System.Windows.Forms.CheckBox();
            this.infoText = new System.Windows.Forms.RichTextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ExcelPath
            // 
            this.ExcelPath.Location = new System.Drawing.Point(438, 43);
            this.ExcelPath.Name = "ExcelPath";
            this.ExcelPath.Size = new System.Drawing.Size(240, 21);
            this.ExcelPath.TabIndex = 0;
            // 
            // OpenExcel
            // 
            this.OpenExcel.Location = new System.Drawing.Point(684, 43);
            this.OpenExcel.Name = "OpenExcel";
            this.OpenExcel.Size = new System.Drawing.Size(75, 23);
            this.OpenExcel.TabIndex = 3;
            this.OpenExcel.Text = "Open";
            this.OpenExcel.UseVisualStyleBackColor = true;
            this.OpenExcel.Click += new System.EventHandler(this.OpenExcel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(28, 257);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(731, 12);
            this.progressBar1.TabIndex = 6;
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(441, 200);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(72, 51);
            this.Start.TabIndex = 7;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(436, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "工作目录(Excel、Txt)：";
            // 
            // formTimer
            // 
            this.formTimer.Tick += new System.EventHandler(this.formTimer_Tick);
            // 
            // errorNum
            // 
            this.errorNum.Location = new System.Drawing.Point(26, 239);
            this.errorNum.Name = "errorNum";
            this.errorNum.Size = new System.Drawing.Size(309, 15);
            this.errorNum.TabIndex = 12;
            this.errorNum.Text = "errorNum";
            // 
            // StopBtn
            // 
            this.StopBtn.Location = new System.Drawing.Point(547, 200);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(72, 51);
            this.StopBtn.TabIndex = 13;
            this.StopBtn.Text = "Stop";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // minijson
            // 
            this.minijson.AutoSize = true;
            this.minijson.Location = new System.Drawing.Point(438, 81);
            this.minijson.Name = "minijson";
            this.minijson.Size = new System.Drawing.Size(108, 16);
            this.minijson.TabIndex = 14;
            this.minijson.Text = "最小化JSON数据";
            this.minijson.UseVisualStyleBackColor = true;
            this.minijson.CheckedChanged += new System.EventHandler(this.minijson_CheckedChanged);
            // 
            // infoText
            // 
            this.infoText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.infoText.Location = new System.Drawing.Point(28, 275);
            this.infoText.Name = "infoText";
            this.infoText.ReadOnly = true;
            this.infoText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.infoText.Size = new System.Drawing.Size(731, 154);
            this.infoText.TabIndex = 15;
            this.infoText.Text = "";
            // 
            // listView1
            // 
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(28, 28);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(388, 208);
            this.listView1.TabIndex = 16;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 12);
            this.label2.TabIndex = 17;
            this.label2.Text = "右键输出选项设置输出路径";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.infoText);
            this.Controls.Add(this.minijson);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.errorNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.OpenExcel);
            this.Controls.Add(this.ExcelPath);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 480);
            this.MinimumSize = new System.Drawing.Size(800, 480);
            this.Name = "Form1";
            this.Text = "DataExport";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExcelPath;
        private System.Windows.Forms.Button OpenExcel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer formTimer;
        private System.Windows.Forms.Label errorNum;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.CheckBox minijson;
        private System.Windows.Forms.RichTextBox infoText;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label2;
    }
}

