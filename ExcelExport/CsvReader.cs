using System;
using System.Data;
using System.IO;

namespace ExcelExport
{
    public static class CsvReader
    {
        /// <summary>
        /// 写入CSV文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="fileName">文件全名</param>
        /// <returns>是否写入成功</returns>
        public static bool SaveCSV(DataTable dt, string fullFileName)
        {
            bool r = false;
            FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();

            r = true;
            return r;
        }

        /// <summary>
        /// 打开CSV 文件
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <returns>DataTable</returns>
        public static DataTable OpenCSV(string fullFileName)
        {
            return OpenCSV(fullFileName,null,0, 0, 0, 0, true);
        }

        /// <summary>
        /// 打开CSV 文件
        /// </summary>
        /// <param name="fileName">文件全名</param>
        /// <param name="firstRow">开始行</param>
        /// <param name="firstColumn">开始列</param>
        /// <param name="getRows">获取多少行</param>
        /// <param name="getColumns">获取多少列</param>
        /// <param name="haveTitleRow">是有标题行</param>
        /// <returns>DataTable</returns>
        public static DataTable OpenCSV(string fullFileName, string[] separators = null,  int firstRow = 0, int firstColumn = 0, int getRows = 0, int getColumns = 0, bool haveTitleRow = false)
        {
            DataTable dt = new DataTable();
            dt.TableName = Path.GetFileNameWithoutExtension(fullFileName);
            FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //是否已建立了表的字段
            bool bCreateTableColumns = false;
            //第几行
            int iRow = 1;

            //去除无用行
            if (firstRow > 0)
            {
                for (int i = 1; i < firstRow; i++)
                {
                    sr.ReadLine();
                }
            }

            // { ",", ".", "!", "?", ";", ":", " " };
            if(null == separators)
            {
                separators = new string[]{ "," };
            }

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                strLine = strLine.Trim();
                aryLine = strLine.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                if (bCreateTableColumns == false)
                {
                    bCreateTableColumns = true;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = firstColumn; i < (getColumns == 0 ? columnCount : firstColumn + getColumns); i++)
                    {
                        DataColumn dc
                            = new DataColumn(haveTitleRow == true ? aryLine[i] : "COL" + i.ToString());
                        dt.Columns.Add(dc);
                    }

                    bCreateTableColumns = true;

                    if (haveTitleRow == true)
                    {
                        continue;
                    }
                }


                DataRow dr = dt.NewRow();
                for (int j = firstColumn; j < (getColumns == 0 ? columnCount : firstColumn + getColumns); j++)
                {
                    if(j>= aryLine.Length)
                    {
                        dr[j - firstColumn] = "";
                    }
                    else
                    {
                        dr[j - firstColumn] = aryLine[j];
                    }
                }
                dt.Rows.Add(dr);

                iRow = iRow + 1;
                if (getRows > 0)
                {
                    if (iRow > getRows)
                    {
                        break;
                    }
                }

            }

            sr.Close();
            fs.Close();
            return dt;
        }
    }
}