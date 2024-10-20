using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using System.Reflection;

namespace DiamondShop.Infrastructure.Services
{
    public class ExcelSyncfunctionService
    {
        private string[] EXCEL_FILE_EXTENSION = { "xls", "csv", "xlsx" }; 
        
        public Stream SaveWorkbookToStream(IWorkbook workbook)
        {
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
        /// <summary>
        /// REMEMBER to put it in using(ExcelEngine = ....)
        /// </summary>
        /// <returns></returns>
        public IApplication GetExcelApplication()
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication excelApp = excelEngine.Excel;
            excelApp.DefaultVersion = ExcelVersion.Xlsx;
            return excelApp;
        }
        /// <summary>
        /// NOTE THIS THING Can only return the next of max 1 char, if reach Z then no more
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public char NextChar(char c)
        {

            if (c < 'A' || c > 'Z')
                throw new Exception("invalid char");
            if (c == 'Z')
            {
                return 'A';
            }
            char incremented = (char)(c + 1);
            return incremented;
        }
        
    }
    public static class ExcelSyncFunctionExtension 
    {
        private static string[] EXCEL_FILE_EXTENSION = { "xls", "csv", "xlsx" };

        public static IWorkbook CreateWorkBook(this IApplication excelApp)
        {
            return excelApp.Workbooks.Create(1);
        }
        public static IWorkbook OpenWorkBook(this IApplication excelApp, Stream fileStream, string fileName)
        {
            var getExtension = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Last();
            bool isCorrect = false;
            foreach (var ext in EXCEL_FILE_EXTENSION)
            {
                if (getExtension == ext)
                    isCorrect = true;
            }
            if (isCorrect is false)
                throw new Exception("this file is not excel type");
            return excelApp.Workbooks.Open(fileStream);
        }
        public static T ReadLine<T>(IWorksheet worksheet, int rowIndex, int columnStart) where T : class, new()
        {
            PropertyInfo[] propertyInfo = typeof(T).GetProperties();
            var propertiesCount = propertyInfo.Length;
                
            //var row0 = worksheet.Rows[0];
            var row  = worksheet.Rows[rowIndex ];
            T result = new T();
            for (int i = 0; i < propertiesCount; i++)
            {
                try
                {
                    var cell = row.Columns[i];
                    //var cell = row[ rowIndex + 1,  i + 1];
                    var text = cell.Text;
                    var property = propertyInfo[i + columnStart];
                    object convertedValue = null;
                    if (property.PropertyType.IsEnum)
                    {
                        // Convert the cell value to the corresponding enum value
                        convertedValue = Enum.Parse(property.PropertyType, text, true);
                    }
                    else
                    {
                        // Convert the cell value to the property type (e.g., int, string, DateTime, etc.)
                        convertedValue = Convert.ChangeType(text, property.PropertyType);
                    }
                    //var convertedValue = Convert.ChangeType(text, property.PropertyType);
                    property.SetValue(result, convertedValue);
                }
                catch
                {
                    var cell = row.Columns[i];
                    cell.CellStyle.Borders.Color = ExcelKnownColors.Red;
                    cell.Comment.Text = "This cell value is invalid";
                    var property = propertyInfo[i + columnStart];
                    property.SetValue(result, null);
                }
            }
            return result;
        }
        public static IWorksheet WritePropertiesName(this IWorksheet worksheet, Type type, int row)
        {
            PropertyInfo[] propertyInfo = type.GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                var range = worksheet.Range[1, i + 1];
                range.Text = propertyInfo[i].Name;
            }
            return worksheet;
        }
        public static IWorksheet WriteLine<T>(this IWorksheet worksheet,T objectToWrite, int row)
        {
            //init red front
            IFont redTextBold = worksheet.Workbook.CreateFont();
            redTextBold.RGBColor = Color.Red;
            redTextBold.Bold = true;

            PropertyInfo[] propertyInfo = objectToWrite.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                var range = worksheet.Range[1, i + 1];
                range.Text = propertyInfo[i].Name;
                
            }
            for (int i = 0; i < propertyInfo.Length; i ++)
            {
              
                PropertyInfo property = propertyInfo[i];
                var value = property.GetValue(objectToWrite);
                string thingsTobeWrittent = null;

                if (value is null)
                {
                    thingsTobeWrittent = "NULL!";
                    IRange range = worksheet.Range[row, i];
                    range.Text = thingsTobeWrittent;
                    IRichTextString richText = range.RichText;
                    richText.SetFont(0,range.Text.Length,redTextBold);

                }
                else
                {
                    thingsTobeWrittent = value.ToString();
                    var range = worksheet.Range[row + 2 , i + 1];
                    //worksheet.Range[columnIndex, i].Text = thingsTobeWrittent;
                    range.Text = thingsTobeWrittent;
                }
            }
            return worksheet;
        }
    }
}
