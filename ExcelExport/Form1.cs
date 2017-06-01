using SimpleJSON;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ExcelExport
{
    public partial class Form1 : Form
    {
        //保存上次打开路径
        static string configfile = "ExcelExport.data";
        OutputType OType = OutputType.JSON;
        delegate void DoDataDelegate();
        ToolTip tips;

        int errors = 0;
        bool bProcessing = false;
        bool bStop = false;

        static public ConcurrentQueue<InfoContext> infoQueue = new ConcurrentQueue<InfoContext>();

        public Form1()
        {
            InitializeComponent();

            tips = new ToolTip();

            formTimer.Start();

            SetErrors(0);

            try
            {
                OpenConfig();
            }
            catch (Exception ex)
            {
                AddInfoMessage("Open Local Config File Failed.{0}" + ex.ToString());
            }
        }

        private void OpenConfig()
        {
            if (File.Exists(configfile))
            {
                var content = File.ReadAllText(configfile);
                JSONNode json = JSON.Parse(content);
                if (json != null)
                {
                    foreach (var c in json.Childs)
                    {
                        if (!Directory.Exists(c.Value))
                            continue;
                        if (c.Key == "ExcelPath")
                        {
                            ExcelPath.Text = c.Value;
                        }
                        else if (c.Key == "JsonPath")
                        {
                            JsonPath.Text = c.Value;
                        }
                        else if (c.Key == "codePath")
                        {
                            codePath.Text = c.Value;
                        }
                    }
                }
            }
        }

        void SaveConfigFile()
        {
            JSONClass jc = new JSONClass();
            {
                JSONData jd = new JSONData(ExcelPath.Text);
                jc["ExcelPath"] = jd;
            }

            {
                JSONData jd = new JSONData(JsonPath.Text);
                jc["JsonPath"] = jd;
            }

            {
                JSONData jd = new JSONData(codePath.Text);
                jc["codePath"] = jd;
            }

            var str = jc.ToString();

            using (StreamWriter sw = new StreamWriter(configfile, false))
            {
                sw.Write(str);
                sw.Flush();
                sw.Close();
            }
        }

        private void OpenExcel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExcelPath.Text = dialog.SelectedPath;
            }
        }

        private void SaveXml_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(JsonPath.Text))
            {
                dialog.SelectedPath = JsonPath.Text;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OType = OutputType.JSON;
                JsonPath.Text = dialog.SelectedPath;
            }
        }

        private void SaveDTC_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(JsonPath.Text))
            {
                dialog.SelectedPath = codePath.Text;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                codePath.Text = dialog.SelectedPath;
            }
        }

        void DoData()
        {
            if (progressBar1.InvokeRequired)
            {
                DoDataDelegate d = DoData;
                progressBar1.Invoke(d);
            }
            else
            {
                AddInfoMessage("*****BEGIN " + DateTime.Now.ToString());
                string[] fn = Directory.GetFiles(ExcelPath.Text, "*.xls*");
                progressBar1.Maximum = fn.Length;

                DataExport de = new DataExport();
                de.CodePath = codePath.Text;
                de.JsonPath = JsonPath.Text;
                de.OType = OType;

                foreach (var file in fn)
                {
                    if(bStop)
                    {
                        break;
                    }

                    if(!Path.GetFileName(file).Contains("$"))
                    {
                        AddInfoMessage("Handle " + file);
                        de.Work(file);
                    }       
                    progressBar1.Value = progressBar1.Value + 1;
                    Application.DoEvents();
                }
                bProcessing = false;
                progressBar1.Value = progressBar1.Maximum;
                AddInfoMessage("*****END  " + DateTime.Now.ToString());
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(ExcelPath.Text) || !Directory.Exists(JsonPath.Text) || !Directory.Exists(codePath.Text))
            {
                MessageBox.Show("Excel 目录 或 保存 目录不正确！", "Error", MessageBoxButtons.OK);
                return;
            }

            if (bProcessing)
            {
                MessageBox.Show("任务正在处理", "Warning", MessageBoxButtons.OK);
                return;
            }

            SaveConfigFile();

            progressBar1.Value = 0;
            errors = 0;
            SetErrors(0);
            bStop = false;
            listBox1.Items.Clear();

            Thread thread = new Thread(DoData);
            thread.IsBackground = true;
            thread.Start();
            bProcessing = true;
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            bStop = true;
        }

        static public void AddInfoMessage(string content, InfoType t = InfoType.Normal)
        {
            infoQueue.Enqueue(new InfoContext(content, t));
        }

        private void formTimer_Tick(object sender, EventArgs e)
        {
            InfoContext ctx;
            while (infoQueue.TryDequeue(out ctx))
            {
                if(ctx.infoType == InfoType.Error)
                {
                    errors++;
                    SetErrors(errors);
                }

                listBox1.Items.Add(ctx.content);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            formTimer.Stop();
        }

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox lb = ((ListBox)(sender));

            int idx = lb.IndexFromPoint(e.Location);// 获取鼠标所在的项索引
            if (idx == -1 || idx >= lb.Items.Count) //鼠标所在位置没有 项
            {
                return;
            }

            if(tips.GetToolTip(lb) != lb.Items[idx].ToString())
            {
                string txt = ((ListBox)(sender)).GetItemText(((ListBox)(sender)).Items[idx]); //获取项文本
                tips.SetToolTip((ListBox)sender, txt);
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0)
                return;

            Brush mybsh = Brushes.Black;
            if (listBox1.Items[e.Index].ToString().StartsWith("ERROR"))
            {
                mybsh = Brushes.Red;
            }

            e.DrawFocusRectangle();
            e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, mybsh, e.Bounds, StringFormat.GenericDefault);
        }

        private void SetErrors(int n)
        {
            errorNum.Text = n.ToString() + " errors";
        }
    }
}
