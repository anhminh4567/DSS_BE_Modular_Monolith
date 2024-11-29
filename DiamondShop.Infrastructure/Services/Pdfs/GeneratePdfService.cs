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

namespace DiamondShop.Infrastructure.Services.Pdfs
{
    public class GeneratePdfService : IPdfService
    {
        private const string OrderInvoiceTemplateFileName = "OrderInvoiceTemplate.cshtml";
        private readonly IOptions<PublicBlobOptions> _publicBlobOptions;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;

        static GeneratePdfService()
        {
            SelectPdf.GlobalProperties.EnableFallbackToRestrictedRenderingEngine = true;

        }

        public GeneratePdfService(IOptions<PublicBlobOptions> publicBlobOptions, IOptions<ExternalUrlsOptions> externalUrlsOptions)
        {
            _publicBlobOptions = publicBlobOptions;
            _externalUrlsOptions = externalUrlsOptions;
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
                IconPath = iconPath
            }).Result;
            return htmlString;
        }

        public static string GetTemplateHtmlStringFromOrderGlobal(Order order, Account customerAccount)
        {
            var getInvoicePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "InvoiceTemplate", OrderInvoiceTemplateFileName);
            var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "ShopIcon.png");
            var diamondRingIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "RingIcon.png");
            var diamondIconPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "DiamondIcon.png");
            Image icon = Image.FromFile(iconPath);
            Image diamondRingIcon = Image.FromFile(diamondRingIconPath);
            Image diamondIcon = Image.FromFile(diamondIconPath);
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
        public static Stream CreatePdfInvoice(Order order, Account account)
        {
            throw new NotImplementedException();
            //return stream;
        }
    }
}
