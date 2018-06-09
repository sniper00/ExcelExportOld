using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelExport
{
    class ExcelReader
    {
        public List<string> SheetNameList = new List<string>();
        DataSet dataSet;
        public bool Read(string filePath, bool hdr = true)
        {
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelReader;

                string ext = Path.GetExtension(filePath);
                if (ext == ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (ext == ".xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    return false;
                }

                excelReader.IsFirstRowAsColumnNames = hdr;
                dataSet = excelReader.AsDataSet();

                excelReader.Close();
                stream.Close();

                foreach (DataTable t in dataSet.Tables)
                {
                    SheetNameList.Add(t.TableName);
                }
                return true;
            }
            catch (Exception ex)
            {
                Form1.WriteInfo(ex.Message, InfoType.Error);
            }
            return false;
        }

        public DataTable GetFirstTable()
        {
            if (dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }
    }
}
