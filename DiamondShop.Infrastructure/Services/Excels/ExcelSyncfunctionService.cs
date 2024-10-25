using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common.ValueObjects;
using FluentResults;
using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using System.IO;
using System.Reflection;

namespace DiamondShop.Infrastructure.Services.Excels
{
    public class ExcelSyncfunctionService : IExcelService
    {
        private string[] EXCEL_FILE_EXTENSION = { "xls", "csv", "xlsx" };
        protected IApplication ExcelApp = GetExcelApplication();
        protected IWorkbook CurrentWorkbook;

        public ExcelSyncfunctionService()
        {

        }

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
        public static IApplication GetExcelApplication()
        {
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication excelApp = excelEngine.Excel;
            excelApp.DefaultVersion = ExcelVersion.Xlsx;
            return excelApp;
        }

        public string[] GetAllSheets()
        {
            return CurrentWorkbook.Worksheets.Select(x => x.Name).ToArray();
        }

        public T ReadLine<T>(string workSheetName, int rowIndex, int columnStart) where T : class, new()
        {
            ArgumentNullException.ThrowIfNull(CurrentWorkbook);
            var getWorksheet = CurrentWorkbook.Worksheets[workSheetName];
            if(getWorksheet is null)
                throw new Exception("Worksheet not found");
            return getWorksheet.ReadLine<T>(rowIndex, columnStart);
        }

        public T WriteLine<T>(string workSheetName, int rowIndex, int columnStart, T objectToWrite) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Stream SaveToStream()
        {
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            CurrentWorkbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }

        public Result OpenFromStream(Stream inputStream, string fileName)
        {
            CurrentWorkbook = ExcelApp.OpenWorkBook(inputStream, fileName);
            if (CurrentWorkbook == null)
                throw new Exception("somehow the workbook still not open and dont throw exception, check logic");
            return Result.Ok();
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
        public static T ReadLine<T>(this IWorksheet worksheet, int rowIndex, int columnStart) where T : class, new()
        {
            PropertyInfo[] propertyInfo = typeof(T).GetProperties();
            var propertiesCount = propertyInfo.Length;

            //var row0 = worksheet.Rows[0];
            var row = worksheet.Rows[rowIndex];
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
        public static IWorksheet WriteLine<T>(this IWorksheet worksheet, T objectToWrite, int rowIndex, int columnStart)
        {
            //init red front
            IFont redTextBold = worksheet.Workbook.CreateFont();
            redTextBold.RGBColor = Color.Red;
            redTextBold.Bold = true;

            PropertyInfo[] propertyInfo = objectToWrite.GetType().GetProperties();
            //for (int i = 0; i < propertyInfo.Length; i++)
            //{
            //    var range = worksheet.Range[1, i + 1];
            //    range.Text = propertyInfo[i].Name;

            //}
            var row = worksheet.Rows[rowIndex];
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                var columns = row.Columns;
                var position = i + columnStart;
                PropertyInfo property = propertyInfo[i];
                var value = property.GetValue(objectToWrite);
                string thingsTobeWrittent;
                if (value is null)
                {
                    thingsTobeWrittent = "NULL!";
                    IRange range = columns[position]; // worksheet.Range[row, i];
                    range.Text = thingsTobeWrittent;
                    IRichTextString richText = range.RichText;
                    richText.SetFont(0, range.Text.Length, redTextBold);

                }
                else
                {
                    thingsTobeWrittent = value.ToString();
                    var range = columns[position];//worksheet.Range[row + 2, i + 1];
                    //worksheet.Range[columnIndex, i].Text = thingsTobeWrittent;
                    range.Text = thingsTobeWrittent;
                }
            }
            return worksheet;
        }
    }
}
