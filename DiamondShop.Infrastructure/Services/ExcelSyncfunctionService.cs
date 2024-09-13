using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
        public static IWorksheet WriteLine<T>(this IWorksheet worksheet,T objectToWrite, int columnIndex)
        {
            //init red front
            IFont redTextBold = worksheet.Workbook.CreateFont();
            redTextBold.RGBColor = Color.Red;
            redTextBold.Bold = true;

            PropertyInfo[] propertyInfo = objectToWrite.GetType().GetProperties(); 
            for (int i = 0; i < propertyInfo.Length; i ++)
            {
                PropertyInfo property = propertyInfo[i];
                var value = property.GetValue(property);
                string thingsTobeWrittent = null;
                if (value is null)
                {
                    thingsTobeWrittent = "NULL!";
                    IRange range = worksheet.Range[columnIndex, i];
                    range.Text = thingsTobeWrittent;
                    IRichTextString richText = range.RichText;
                    richText.SetFont(0,range.Text.Length,redTextBold);

                }
                else
                {
                    thingsTobeWrittent = value.ToString();
                    worksheet.Range[columnIndex, i].Text = thingsTobeWrittent;
                }
            }
            return worksheet;
        }
    }
}
