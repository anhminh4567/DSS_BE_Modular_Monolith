using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Utilities
{
    public class FileUltilities
    {
        public static bool IsImageFileContentType(string contentType)
        {
            if (!string.Equals(contentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        public static bool IsImageFileExtension(string fileExtension)
        {
            if (!string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        public static bool IsPdfFileContentType(string contentType)
        {
            if (!string.Equals(contentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        public static bool IsPdfFileExtension(string fileExtension)
        {
            if (!string.Equals(fileExtension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
        public static string? IsExcelFileExtension(string fileExtension)
        {
            if (!string.Equals(fileExtension, ".xlsx"))
            {
                return "only accept .xlsx file type of excel, sorry";
            }
            return null;
        }
        public static bool IsExcelContentType(string contentType)
        {
            if (!string.Equals(contentType, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
    }
}
