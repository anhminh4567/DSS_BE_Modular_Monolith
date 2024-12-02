using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Pdfs.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Syncfusion.Drawing;
using System.Reflection;
using DiamondShop.Infrastructure.Databases.Configurations.PromoConfig;

namespace DiamondShop.Infrastructure.Services.Pdfs
{
    public class GeneratePdfService : IPdfService
    {
        private const string OrderInvoiceTemplateFileName = "OrderInvoiceTemplate.cshtml";
        private readonly IOptions<PublicBlobOptions> _publicBlobOptions;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;
        private readonly IOptions<LocationOptions> _locationOptions;
        static GeneratePdfService()
        {
            SelectPdf.GlobalProperties.EnableFallbackToRestrictedRenderingEngine = true;

        }

        public GeneratePdfService(IOptions<PublicBlobOptions> publicBlobOptions, IOptions<ExternalUrlsOptions> externalUrlsOptions, IOptions<LocationOptions> locationOptions)
        {
            _publicBlobOptions = publicBlobOptions;
            _externalUrlsOptions = externalUrlsOptions;
            _locationOptions = locationOptions;
        }

        public static Stream GeneratePdfDoc(string htmlString)
        {
            //Dink to pdf convertere
            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4Plus,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = htmlString,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] pdf = converter.Convert(doc);
            MemoryStream stream = new MemoryStream(pdf);
            stream.Position = 0;
            return stream;
            //Syncfucntion Convertere
            //HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            //BlinkConverterSettings settings = new BlinkConverterSettings();
            ////Set command line arguments to run without sandbox.
            //settings.CommandLineArguments.Add("--no-sandbox");
            //settings.CommandLineArguments.Add("--disable-setuid-sandbox");
            ////Assign Blink converter settings to the HTML converter 
            //htmlConverter.ConverterSettings = settings;
            //Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(htmlString, string.Empty);
            //MemoryStream stream = new MemoryStream();
            //document.Save(stream);
            //stream.Position = 0;
            ////Close the document
            //document.Close(true);
            //return stream;

            //SelectPdf
            //Stream returnStream = new MemoryStream();
            //returnStream.Position = 0;
            //HtmlToPdf converter = new HtmlToPdf();
            //PdfDocument doc = converter.ConvertHtmlString(htmlString);
            //doc.Save(returnStream);
            //returnStream.Position = 0;
            //return returnStream;
        }

        public string GetTemplateHtmlStringFromOrder(Order order, Account customerAccount)
        {
            var publicOption = _publicBlobOptions.Value;
            var externalOption = _externalUrlsOptions.Value;
            var iconPath = _publicBlobOptions.Value.GetPath(externalOption, publicOption.ShopIcon);
            var diamondRingIconPath = _publicBlobOptions.Value.GetPath(externalOption,publicOption.DiamondRingIcon);
            var diamondIconPath = _publicBlobOptions.Value.GetPath(externalOption,publicOption.DiamondIcon);
            string htmlString = RazorTemplateEngine.RenderAsync($"/RazorTemplate/InvoiceTemplate/{OrderInvoiceTemplateFileName}", new OrderInvoiceModels
            {
                Account = customerAccount,
                Order = order,
                DiamondIconPath = diamondIconPath,
                DiamondRingIconPath = diamondRingIconPath,
                IconPath = iconPath,
                ShopAddress = _locationOptions.Value.ShopOrignalLocation.OriginalLocationName
            }).Result;
            return htmlString;
        }

        public static string GetTemplateHtmlStringFromOrderGlobal(Order order, Account customerAccount)
        {
            var getInvoicePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InvoiceTemplate", OrderInvoiceTemplateFileName);
            var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ShopIcon.png");
            var diamondRingIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "RingIcon.png");
            var diamondIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "DiamondIcon.png");
            System.Drawing.Image icon = System.Drawing.Image.FromFile(iconPath);
            System.Drawing.Image diamondRingIcon = System.Drawing.Image.FromFile(diamondRingIconPath);
            System.Drawing.Image diamondIcon = System.Drawing.Image.FromFile(diamondIconPath);
            MemoryStream m = new MemoryStream();
            try
            {
                icon.Save(m, icon.RawFormat);
                byte[] imageBytes = m.ToArray();
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                string iconData = string.Format("data:image/jpg;base64, {0}", base64String);
                m.Position = 0; m.SetLength(0);
                diamondRingIcon.Save(m, diamondRingIcon.RawFormat);
                imageBytes = m.ToArray();
                string diamondRingBase64String = Convert.ToBase64String(imageBytes);
                string diamondRingIconData = string.Format("data:image/jpg;base64, {0}", diamondRingBase64String);
                m.Position = 0; m.SetLength(0);
                diamondIcon.Save(m, diamondIcon.RawFormat);
                imageBytes = m.ToArray();
                string diamondBase64String = Convert.ToBase64String(imageBytes);
                string diamondIconData = string.Format("data:image/jpg;base64, {0}", diamondBase64String);
                string htmlString = RazorTemplateEngine.RenderAsync($"/RazorTemplate/InvoiceTemplate/{OrderInvoiceTemplateFileName}", new OrderInvoiceModels
                {
                    Account = customerAccount,
                    Order = order,
                    IconBase64 = iconData,
                    DiamondIconBase64 = diamondIconData,
                    DiamondRingIconBase64 = diamondRingIconData
                }).Result;
                return htmlString;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                icon.Dispose();
                diamondIcon.Dispose();
                diamondRingIcon.Dispose();
                m.Close();
            }
        }

        public Stream ParseHtmlToPdf(string htmlString)
        {
            //var converter = _converter;
            //var doc = new HtmlToPdfDocument()
            //{
            //    GlobalSettings = {
            //        ColorMode = ColorMode.Color,
            //        Orientation = Orientation.Portrait,
            //        PaperSize = PaperKind.A4,

            //    },
            //    Objects = {
            //        new ObjectSettings() {
            //            PagesCount = true,
            //            HtmlContent = htmlString,
            //            WebSettings = { LoadImages = true },
            //            LoadSettings = { JSDelay  =3000 },
            //            //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
            //        }
            //    },
            //};
            //byte[] pdf = converter.Convert(doc);
            ////doc.Objects.h
            //MemoryStream stream = new MemoryStream(pdf);
            //stream.Position = 0;
            //return stream;
            //Syncfucntion Convertere
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings settings = new BlinkConverterSettings();
            //Set command line arguments to run without sandbox.
            settings.CommandLineArguments.Add("--no-sandbox");
            settings.CommandLineArguments.Add("--disable-setuid-sandbox");
            //Assign Blink converter settings to the HTML converter 
            htmlConverter.ConverterSettings = settings;
            Syncfusion.Pdf.PdfDocument document = htmlConverter.Convert(htmlString, string.Empty);
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            //Close the document
            document.Close(true);
            return stream;
            return GeneratePdfDoc(htmlString);
        }
        public static Stream CreatePdfInvoice(Order order, Account account, LocationOptions shopLocation)
        {
        //    var shopIcon= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ShopIcon.png");
        //    var city = shopLocation.ShopOrignalLocation.OriginalProvince;
        //    var ward  = shopLocation.ShopOrignalLocation.OrignalWard;
        //    var district = shopLocation.ShopOrignalLocation.OrignalDistrict;
        //    var address = shopLocation.ShopOrignalLocation.OriginalLocationName;
        //    var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Fonts", "ARIAL.TTF");
        //    var boldFontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Fonts", "ARIALBD.TTF");
        //    Stream fontData = new FileStream(fontPath, FileMode.Open, FileAccess.Read);//Assembly.GetCallingAssembly().GetManifestResourceStream(fontPath);
        //    Stream boldFontData = new FileStream(boldFontPath, FileMode.Open, FileAccess.Read);
        //    if (fontData == null || boldFontData == null)
        //    {
        //        throw new Exception("Font not found");
        //    }
        //    PdfFont regularFont = new PdfTrueTypeFont(fontData,6);
        //    PdfFont boldFont = new PdfTrueTypeFont(boldFontData, 12);


        //    FileStream iconFile = new FileStream(shopIcon, FileMode.Open, FileAccess.Read);
        //    var pdfDocument = new PdfDocument();
        //    var currentPage = pdfDocument.Pages.Add();
        //    PdfGraphics graphics = currentPage.Graphics;
        //    Syncfusion.Drawing.RectangleF borders = new Syncfusion.Drawing.RectangleF(0, 0, currentPage.GetClientSize().Width, currentPage.GetClientSize().Height);
        //    PdfPen borderPen = new PdfPen(Syncfusion.Drawing.Color.Yellow, 1);
        //    borderPen.DashStyle = PdfDashStyle.Dot;
        //    graphics.DrawRectangle(borderPen,borders );

        //   Syncfusion.Drawing.RectangleF contentRectangle = new Syncfusion.Drawing.RectangleF(
        //       borders.X + 10, // Left padding
        //       borders.Y + 10, // Top padding
        //       borders.Width - 20, // Reduce width for left & right padding
        //       borders.Height - 20 // Reduce height for top & bottom padding
        //   );
        //    var rectangleOutline = new Syncfusion.Drawing.RectangleF(contentRectangle.X,contentRectangle.Y, contentRectangle.Width , 1);


        //    PdfImage image = PdfImage.FromStream(iconFile);
        //    var widthLenghRatio = ((decimal) image.Width / (decimal)image.Height);   
        //    int iconLength = 50;
        //    Syncfusion.Drawing.SizeF iconSize = new Syncfusion.Drawing.SizeF((int) Math.Floor(iconLength * widthLenghRatio), iconLength );
        //    Syncfusion.Drawing.PointF iconLocation = new Syncfusion.Drawing.PointF(contentRectangle.X + 10, contentRectangle.Y + 10);
        //    graphics.DrawImage(image, iconLocation, iconSize);

        //    PdfFont font = boldFont;//new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
        //    var text = new PdfTextElement("INVOICE", font);
        //    text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
        //    PdfLayoutResult result = text.Draw(currentPage, new Syncfusion.Drawing.PointF(contentRectangle.Width - 25, contentRectangle.Y + 10));
        //    //
        //    font = regularFont;//new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
        //    text = new PdfTextElement("Địa chỉ: " + address, font);
        //    result = text.Draw(currentPage, new Syncfusion.Drawing.PointF(14, result.Bounds.Bottom + 50));

        //    font = regularFont;//= new PdfStandardFont(PdfFontFamily.TimesRoman, 14,PdfFontStyle.Bold);
        //    text = new PdfTextElement("Địa chỉ người nhận: " + order.ShippingAddress, font);
        //    result = text.Draw(currentPage, new Syncfusion.Drawing.PointF(14, result.Bounds.Bottom + 3));

        //    font = regularFont;//= new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
        //    text = new PdfTextElement("Mã đơn: " + order.OrderCode, font);
        //    result = text.Draw(currentPage, new Syncfusion.Drawing.PointF(14, result.Bounds.Bottom + 3));

        //    PdfPen outlinePen = new PdfPen(Syncfusion.Drawing.Color.Yellow, 1);
        //    rectangleOutline.Y = result.Bounds.Bottom + 10;
        //    graphics.DrawRectangle(outlinePen, rectangleOutline);


        //    PdfGrid pdfGrid = new PdfGrid();
        //    List<object> dataSource = new List<object>();
        //    foreach (var item in order.Items)
        //    {
        //        dataSource.Add(new
        //        {
        //            SKU =  item.DiamondId != null ? item.Diamond.SerialCode : item.Jewelry.SerialCode,
        //            Type = item.DiamondId != null ? "Kim cương" : "trang sức",
        //            DiscountConfiguration = item.DiscountCode == null ? "khong co" : item.DiscountCode,
        //            Quantity = 1,
        //            Price = $"{item.OriginalPrice}\n{item.PurchasedPrice}",
        //        });
        //    }

        //    PdfFont boldFontHeader = new PdfTrueTypeFont(boldFontData, 8);

        //    pdfGrid.DataSource = dataSource;
        //    PdfGridStyle gridStyle = new PdfGridStyle();
        //    gridStyle.CellPadding = new PdfPaddings(2, 2, 2, 2);
        //    gridStyle.AllowHorizontalOverflow = true;
        //    PdfGridRowStyle pdfGridRowStyle = new PdfGridRowStyle();
        //    pdfGridRowStyle.BackgroundBrush = PdfBrushes.Yellow;
        //    pdfGridRowStyle.Font = boldFontHeader;
        //    pdfGrid.Headers.ApplyStyle(pdfGridRowStyle);
        //    PdfGridRow header = pdfGrid.Headers[0];
        //    foreach (PdfGridCell cell in header.Cells)
        //    {
        //        cell.Style.TextBrush = PdfBrushes.Black; // Set text color to black
        //        cell.Style.Borders.All = new PdfPen(new PdfColor(200, 200, 200), 0.5f);
        //    }
        //    // Customize the table cells
        //    foreach (PdfGridRow row in pdfGrid.Rows)
        //    {
        //        foreach (PdfGridCell cell in row.Cells)
        //        {
        //            cell.Style.Borders.All = new PdfPen(new PdfColor(200, 200, 200), 0.5f); // Lighter and thinner border
        //            cell.Style.Font = new PdfTrueTypeFont(fontData, 6);
        //        }
        //    }
        //    pdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap; // Ensures neat borders
        //    pdfGrid.Style.CellPadding.All = 2;
        //    foreach (PdfGridRow row in pdfGrid.Rows)
        //    {
        //        PdfGridCell priceCell = row.Cells[4];
        //        priceCell.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);

        //        string[] prices = priceCell.Value.ToString().Split('\n');
        //        string originalPrice = prices[0];
        //        string soldPrice = prices.Length > 1 ? prices[1] : string.Empty;

        //        float cellX = priceCell.Width;
        //        float cellY = priceCell.Bounds.Y;
        //        float cellWidth = priceCell.Bounds.Width;

        //        // Draw the original price with strikethrough
        //        PdfFont originalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        //        graphics.DrawString(originalPrice, originalFont, PdfBrushes.Gray, new Syncfusion.Drawing.PointF(cellX + 2, cellY + 2));
        //        float textWidth = originalFont.MeasureString(originalPrice).Width;
        //        graphics.DrawLine(PdfPens.Gray, new Syncfusion.Drawing.PointF(cellX + 2, cellY + 8), new Syncfusion.Drawing.PointF(cellX + 2 + textWidth, cellY + 8));

        //        // Draw the sell price below the original price
        //        PdfFont sellFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
        //        graphics.DrawString(sellPrice, sellFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(cellX + 2, cellY + 12));
        //    }
        //}
        //    //Apply style.
        //    //pdfGrid.Style = gridStyle;
        //    result = pdfGrid.Draw(currentPage, result.Bounds.X, result.Bounds.Bottom + 40,contentRectangle.Width);


        //    //draw a LINE   
        //    rectangleOutline.Y = result.Bounds.Bottom + 10;
        //    graphics.DrawRectangle(outlinePen, rectangleOutline);

            

        //    var stream = new MemoryStream();
        //    pdfDocument.Save(stream);
        //    pdfDocument.Close(true);
        //    stream.Position = 0;
        //    return stream;
            throw new NotImplementedException();
            //return stream;
        }
    }
}
