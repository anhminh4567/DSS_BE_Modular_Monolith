using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DiamondShop.Test.General
{
    public class ExcelTest
    {
        [Fact(Skip = "not now")]
        public void TestPrecendentCell()
        {
            string fileName = "DirectPrecedents.xlsx";
            string path = Directory.GetCurrentDirectory();
            string fullPath = Path.Combine(path, fileName);
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                FileStream inputStream = new FileStream("C:\\Users\\anhmi\\Downloads\\SU24 Danh sach SV du dieu kien xet Ton vinh (up FAP).xlsx", FileMode.Open, FileAccess.Read);
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                IWorksheet sheet = workbook.Worksheets[0];
                var character1 = sheet["A2"].Text;
                var character2 = sheet["B2"].Text;
                var character3 = sheet["C2"].Text;

                //Getting precedent cells from the worksheet
                IRange[] results1 = sheet["A1"].GetDirectPrecedents();

                //Getting precedent cells from the workbook
                IRange[] results2 = sheet["A1"].GetDirectPrecedents(true);

                //Getting precedent cells from the worksheet 2
                IRange[] results3 = sheet["B5"].GetDirectPrecedents(false);
                //Saving the workbook as stream
                FileStream file = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                workbook.SaveAs(file);
                file.Dispose();
            }
            Assert.True(true);
        }
    }
}
