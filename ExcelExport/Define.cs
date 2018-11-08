
namespace ExcelExport
{
    public enum InfoType
    {
        Normal,
        Error,
        Warn
    }

    public class InfoContext
    {
        public InfoContext(string c, InfoType i)
        {
            if (i == InfoType.Error)
            {
                content = "ERROR:" + c;
            }
            else
            {
                content = c;
            }

            infoType = i;
        }

        public string content;
        public InfoType infoType;
    }
}
