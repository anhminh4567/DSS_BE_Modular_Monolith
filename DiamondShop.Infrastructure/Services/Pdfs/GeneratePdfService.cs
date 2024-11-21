using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services.Pdfs.Models;
using Microsoft.Extensions.Options;
using Razor.Templating.Core;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services.Pdfs
{
    public class GeneratePdfService : IPdfService
    {
        private const string OrderInvoiceTemplateFileName = "OrderInvoiceTemplate.cshtml";
        private readonly IOptions<PublicBlobOptions> _publicBlobOptions;
        private readonly IOptions<ExternalUrlsOptions> _externalUrlsOptions;

        public GeneratePdfService(IOptions<PublicBlobOptions> publicBlobOptions, IOptions<ExternalUrlsOptions> externalUrlsOptions)
        {
            _publicBlobOptions = publicBlobOptions;
            _externalUrlsOptions = externalUrlsOptions;
        }

        public static Stream GeneratePdfDoc(string htmlString)
        {
            Stream returnStream = new MemoryStream();
            returnStream.Position = 0;
            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(htmlString);
            doc.Save(returnStream);
            returnStream.Position = 0;
            return returnStream;
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
            return GeneratePdfDoc(htmlString);
        }
    }
}
