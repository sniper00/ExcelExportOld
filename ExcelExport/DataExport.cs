using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using XLua;

namespace ExcelExport
{
    public enum OutputType
    {
        XML,
        JSON
    }

    public class DataExport
    {
        Dictionary<string, string> dataType = new Dictionary<string, string>();//数据类型
        Dictionary<string, string> fieldConstraint = new Dictionary<string, string>();//字段约束-列名
        List<string> columnsName = new List<string>();//列名
        string tableName;

        LuaEnv luaEnv = new LuaEnv();

        [CSharpCallLua]
        public delegate void CodeGen(string codePath, string tableName, List<string> columnsName, Dictionary<string, string> dataType, Dictionary<string, string> keyType);

        [CSharpCallLua]
        public delegate string RowCheck(Dictionary<string, string> rowData);

        CodeGen codeGen;

        public string JsonPath
        {
            get; set;
        }

        public string CodePath
        {
            get; set;
        }

        public OutputType OType
        {
            get; set;
        }

        public void Print(string s)
        {
            MessageBox.Show(s);
        }

        public void BindLuaFunction()
        {
            if (null != codeGen)
            {
                return;
            }

            try
            {
                var luaString = File.ReadAllText("./CodeGen.lua");
                luaEnv.DoString(luaString);
                codeGen = luaEnv.Global.Get<CodeGen>("CodeGen");
                luaEnv.Global.Set("DataExport", this);
            }
            catch(Exception ex)
            {
                Form1.AddInfoMessage(ex.Message, InfoType.Error);
            }
        }

        public void Work(string excelFile)
        {
            BindLuaFunction();

            ExcelReader er = new ExcelReader();
            if (!er.Read(excelFile))
            {
                return;
            }

            var dt = er.GetFirstTable();
            if (null == dt)
                return;

            columnsName.Clear();
            dataType.Clear();
            fieldConstraint.Clear();

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                columnsName.Add(dt.Columns[j].ColumnName);
            }

            tableName = dt.TableName;

            // 第一行 列名
            // 第二行 数据类型
            // 第三行 字段约束

            //至少有一行数据
            if (dt.Rows.Count < 3)
                return;

            int totalRow = dt.Rows.Count + 1;

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (dt.Rows[0][j].ToString().Length != 0)
                    dataType.Add(columnsName[j], (dt.Rows[0][j].ToString()));

                if (dt.Rows[1][j].ToString().Length != 0)
                    fieldConstraint.Add(columnsName[j], dt.Rows[1][j].ToString());
            }

            dt.Rows.RemoveAt(0); //data type
            dt.Rows.RemoveAt(0); // keys

            while (dt.Columns.Count != 0 && dt.Rows[0][0].ToString().StartsWith("#"))
            {
                dt.Rows.RemoveAt(0);// skip # lines
            }

            if (OType == OutputType.XML)
            {
                //DoXml(dt);
            }
            else if (OType == OutputType.JSON)
            {
                DoJson(dt, totalRow - dt.Rows.Count, er.checkScript);
            }
        }

        //void DoXml(DataTable dt)
        //{
        //    StreamWriter sw = new StreamWriter(JsonPath + "\\" + tableName + ".xml", false, Encoding.GetEncoding("UTF-8"));
        //    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
        //    sw.WriteLine("<" + tableName + ">");
        //    for (int i = 1; i < dt.Rows.Count; i++)
        //    {
        //        if (dt.Columns.Count == 0 || dt.Rows[i][0].ToString().Length == 0)
        //            continue;

        //        sw.WriteLine("\t\t<" + tableName + ">");
        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            if (dataType.ContainsKey(columnsName[j]))
        //                sw.WriteLine("\t\t\t<" + columnsName[j] + ">" + dt.Rows[i][j].ToString() + "</" + columnsName[j] + ">");
        //        }
        //        sw.WriteLine("\t\t</" + tableName + ">");
        //    }
        //    sw.WriteLine("</" + tableName + ">");
        //    sw.Flush();
        //    sw.Close();
        //}

        void DoJson(DataTable dt, int skipRow,  string checkScript = "")
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(JsonPath + "\\" + tableName + ".json", false, Encoding.GetEncoding("UTF-8")))
                {
                    sw.Write("[");

                    LuaEnv checkEnv = null;
                    RowCheck rowCheck = null;

                    if (checkScript.Length > 0)
                    {
                        checkEnv = new LuaEnv();
                        checkEnv.DoString(checkScript);
                        rowCheck = checkEnv.Global.Get<RowCheck>("RowCheck");
                    }

                    Dictionary<string, string> rowData = new Dictionary<string, string>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Columns.Count == 0 || dt.Rows[i][0].ToString().Length == 0)
                            continue;

                        rowData.Clear();

                        if (i > 0) { sw.Write(","); }

                        sw.Write("{");
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j > 0) { sw.Write(","); }

                            var datatype = dataType[columnsName[j]];
                            if (IsBaseDataType(datatype))
                            {
                                sw.Write(string.Format("\"{0}\":{1}", columnsName[j], dt.Rows[i][j].ToString()));
                            }
                            else
                            {
                                sw.Write(string.Format("\"{0}\":\"{1}\"", columnsName[j], dt.Rows[i][j].ToString()));
                            }

                            rowData.Add(columnsName[j], dt.Rows[i][j].ToString());
                        }
                        sw.Write("}");

                        var sRet = rowCheck?.Invoke(rowData);
                        if (null != sRet && sRet != "ok")
                        {
                            Form1.AddInfoMessage(string.Format("Row {0} Column ", skipRow + i + 1) + sRet, InfoType.Error);
                            //break;
                        }
                    }
                    sw.Write("]");
                    sw.Flush();
                    sw.Close();
                    codeGen(CodePath, tableName, columnsName, dataType, fieldConstraint);
                }       
            }
            catch (Exception ex)
            {
                Form1.AddInfoMessage(ex.Message, InfoType.Error);
            }
        }

        bool IsBaseDataType(string s)
        {
            switch (s)
            {
                case "int":
                    return true;
                case "uint":
                    return true;
                case "long":
                    return true;
                case "ulong":
                    return true;
                case "float":
                    return true;
                case "double":
                    return true;
                case "bool":
                    return true;
            }
            return false;
        }
    }
}