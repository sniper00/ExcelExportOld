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
        //保存配置
        static string configfile = "ExcelExport.json";

        int errors = 0;
        int warns = 0;
        bool bStop = false;
        bool miniJson = false;

        SimpleConfig config;
        ContextMenuStrip strip;
        string selectedRow;

        ConcurrentQueue<Action> eventQueue = new ConcurrentQueue<Action>();

        Thread thread;

        static Form1 instance_;

        public static Form1 Instance()
        {
            return instance_;
        }

        public Form1()
        {
            InitializeComponent();

            instance_ = this;

            formTimer.Start();

            SetErrorsAndWarns(0, 0);

            try
            {
                config = new SimpleConfig(configfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open Local Config File Failed.{0}" + ex.ToString());
            }

            listView1.Columns.Add("类型", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("输出路径", listView1.Width-120, HorizontalAlignment.Left);
            listView1.CheckBoxes = true;

            listView1.MouseClick += ListViewMouseClicked;
            listView1.SelectedIndexChanged += ListViewSelected;
            listView1.ItemChecked+= ListViewItemClicked;

            strip = new ContextMenuStrip();
            strip.Items.Add("设置输出路径",null, MenuClick);
            LoadGenerator();
        }


        void LoadGenerator()
        {
            if (!Directory.Exists("./Generator"))
            {
                _WriteInfo("Generator File Not Found.", InfoType.Error);
                return;
            }

            ExcelPath.Text = config.Get("Excel.Path", "");
            if(!Directory.Exists(ExcelPath.Text))
            {
                ExcelPath.Text = "";
            }

            listView1.BeginUpdate();

            listView1.Items.Clear();
            {
                string[] files = Directory.GetFiles("./Generator", "*.lua");
                foreach (var f in files)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = Path.GetFileNameWithoutExtension(f);
                    var outpath = config.Get("Generator." + lvi.Text + ".Path", "");
                    if(!Directory.Exists(outpath))
                    {
                        outpath = "";
                    }

                    lvi.SubItems.Add(outpath);
                    lvi.Checked = config.Get("Generator." + lvi.Text + ".Checked", false);
                    listView1.Items.Add(lvi);
                }
            }
            listView1.EndUpdate();
        }

        void ListViewMouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                strip.Show(listView1, e.Location);//鼠标右键按下弹出菜单
            }
        }

        void MenuClick(object sender, EventArgs e)
        {
            var path = "Generator." + selectedRow + ".Path";
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(config.Get(path, "")))
            {
                dialog.SelectedPath = config.Get(path, "");
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                config.Set(path, dialog.SelectedPath);
                LoadGenerator();
            }
        }

        void ListViewSelected(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView1.SelectedIndices;
                selectedRow = listView1.Items[c[0]].Text;
            }
        }

        void ListViewItemClicked(object sender, ItemCheckedEventArgs e)
        {
            config.Set("Generator." + e.Item.Text +".Checked", e.Item.Checked);
        }


        private void OpenExcel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (Directory.Exists(config.Get("Excel.Path", "")))
            {
                dialog.SelectedPath = config.Get("Excel.Path", "");
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                config.Set("Excel.Path", dialog.SelectedPath);
                ExcelPath.Text = dialog.SelectedPath;
            }
        }

        void Post(Action evt)
        {
            eventQueue.Enqueue(evt);
        }

        void _WriteInfo(string content, InfoType t = InfoType.Normal)
        {
            switch(t)
            {
                case InfoType.Normal:
                    {
                        infoText.SelectionColor = Color.Black;
                        break;
                    }
                case InfoType.Error:
                    {
                        errors++;
                        infoText.SelectionColor = Color.Red;
                        SetErrorsAndWarns(errors, warns);
                        break;
                    }
                case InfoType.Warn:
                    {
                        warns++;
                        infoText.SelectionColor = Color.Green;
                        SetErrorsAndWarns(errors, warns);
                        break;
                    }

            }

            infoText.AppendText(content + "\n");
            infoText.Select(infoText.TextLength, 0);
            infoText.ScrollToCaret();
        }

        public static void WriteInfo(string content, InfoType t = InfoType.Normal)
        {
            Instance().Post(() =>
            {
                Instance()._WriteInfo(content,t);
            });
        }

        public static void SetProgressBarMax(int max)
        {
            Instance().Post(() =>
            {
                Instance().progressBar1.Maximum = max;
            });
        }

        public static void SetProgressBar(int n)
        {
            Instance().Post(() =>
            {
                Instance().progressBar1.Value = n;
            });
        }

        void DoData()
        {
            WriteInfo("*****BEGIN " + DateTime.Now.ToString());
            string[] files1 = Directory.GetFiles(ExcelPath.Text, "*.xls*");
            string[] files2 = Directory.GetFiles(ExcelPath.Text, "*.txt*");

            string[] files = new string[files1.Length + files2.Length];

            files1.CopyTo(files, 0);
            files2.CopyTo(files, files1.Length);

            SetProgressBarMax(files.Length);

            DataExport de = new DataExport();
            de.GetStringConfig = config.Get;
            de.GetBoolConfig = config.Get;
            de.GetNumberConfig = config.Get;
            de.MinisizeJson = miniJson;

            int n = 0;
            foreach (var file in files)
            {
                if (bStop)
                {
                    break;
                }

                if (!Path.GetFileName(file).Contains("$"))
                {
                    WriteInfo("Handle " + file);
                    if(!de.Work(file))
                    {
                        break;
                    }
                }
                n++;
                SetProgressBar(n);
            }
            WriteInfo("*****END  " + DateTime.Now.ToString());
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(ExcelPath.Text))
            {
                MessageBox.Show("工作目录不存在！", "Error", MessageBoxButtons.OK);
                return;
            }

            if (null != thread &&  thread.ThreadState != ThreadState.Stopped)
            {
                MessageBox.Show("任务正在处理", "Warning", MessageBoxButtons.OK);
                return;
            }

            config.Set("Excel.Path", ExcelPath.Text);

            progressBar1.Value = 0;
            errors = 0;
            warns = 0;
            SetErrorsAndWarns(0,0);
            bStop = false;
            infoText.Text = "";

            thread = new Thread(DoData);
            thread.IsBackground = true;
            thread.Start();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            bStop = true;
        }

        private void formTimer_Tick(object sender, EventArgs e)
        {
            Action act;
            while (eventQueue.TryDequeue(out act))
            {
                act();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != thread && thread.ThreadState == ThreadState.Running)
            {
                bStop = true;
                thread.Join();
            }
            formTimer.Stop();
        }

        private void SetErrorsAndWarns(int nerr, int nwarn)
        {
            errorNum.Text = nerr.ToString() + " errors  ";
            errorNum.Text += nwarn.ToString() + " warns  ";
        }

        private void minijson_CheckedChanged(object sender, EventArgs e)
        {
            miniJson = minijson.Checked;
        }
    }
}
