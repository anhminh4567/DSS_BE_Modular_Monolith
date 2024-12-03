using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Models
{
    public class FileDownloadData
    {
        private string fileName;
        private string fileExtension;
        private string contentType;
        private DateTime timeStamp;
        private Stream Stream;

        public FileDownloadData(string fileName, string fileExtension, string contentType, DateTime timeStamp, Stream stream)
        {
            this.fileName = fileName;
            this.fileExtension = fileExtension;
            this.contentType = contentType;
            this.timeStamp = timeStamp;
            Stream = stream;
        }

        public string GetFormatedName()
        {
            var timestamep = timeStamp.ToString("yyyyMMddHHmmss");
            var correctExtensiont =fileExtension.Replace(".", "");
            return $"{fileName}_{timestamep}.{correctExtensiont}";
        }
        public Stream GetFileStream()
        {
            Stream.Position = 0;
            return Stream;
        }
        public string GetContentType()
        {
            return contentType;
        }
    }
}
