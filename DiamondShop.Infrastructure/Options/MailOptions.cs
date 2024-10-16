using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Options
{
    internal class MailOptions
    {
        public static string Section = "MailSettings";
        public string Host { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string AppPassword { get; set; }
    }
}
