using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XLua;

namespace ExcelExport
{
    public class DataExport
    {
        LuaEnv luaEnv = new LuaEnv();
 
        [CSharpCallLua]
        public delegate void OnData(DataTable data, string checkScriptPath);

        OnData onData;

        public Func<string, string, string> GetStringConfig;
        public Func<string, bool, bool> GetBoolConfig;
        public Func<string, double, double> GetNumberConfig;

        public bool MinisizeJson
        {
            get; set;
        }

        public DataExport()
        {
            BindLuaFunction();
        }

        public void Print(string s)
        {
            MessageBox.Show(s);
        }

        public void PushInfo(int ntype, string msg)
        {
            if(ntype == (int)(InfoType.Error))
            {
                Form1.WriteInfo(msg, InfoType.Error);
            }
            else
            {
                Form1.WriteInfo(msg, InfoType.Normal);
            }
        }

        public void BindLuaFunction()
        {
            if (null != onData)
            {
                return;
            }

            try
            {
                var luaString = File.ReadAllText("./CodeGen.lua");
                luaEnv.DoString(luaString);
                onData = luaEnv.Global.Get<OnData>("OnData");
                luaEnv.Global.Set("DataExport", this);
            }
            catch(Exception ex)
            {
                Form1.WriteInfo(ex.Message, InfoType.Error);
            }
        }

        string GetCheckScript(string file)
        {
            var scriptName = Path.GetFileNameWithoutExtension(file) + ".checker.lua";
            var scriptFile = Path.GetDirectoryName(file) + "\\" + scriptName;
            if (File.Exists(scriptFile))
            {
                return File.ReadAllText(scriptFile);
            }
            return "";
        }

        public bool WriteFile(string path, string content)
        {
            if(!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Form1.WriteInfo(string.Format("Path: {0} does not exist.", path), InfoType.Error);
                return false;
            }

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(content);
                sw.Flush();
                sw.Close();
            }
            return true;
        }

        public bool Work(string file)
        {
            try
            {
                if(Path.GetExtension(file) == ".xlsx" )
                {
                    ExcelReader er = new ExcelReader();
                    if (!er.Read(file))
                    {
                        return true;
                    }

                    var dt = er.GetFirstTable();
                    if (null == dt)
                        return true;

                    onData(dt, GetCheckScript(file));
                }
                else if(Path.GetExtension(file) == ".txt")
                {
                    var dt = CsvReader.OpenCSV(file, new string[] { "\t" },1);

                    if (null == dt)
                        return true;

                    onData(dt, GetCheckScript(file));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Form1.WriteInfo(ex.Message, InfoType.Error);
            }
            return false;
        }
    }
}