using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IExcelService
    {
        string[] GetAllSheets();
        T ReadLine<T>(string workSheetName, int rowIndex, int columnStart) where T : class, new();
        T WriteLine<T>(string workSheetName, int rowIndex, int columnStart, T objectToWrite) where T : class, new();
        Stream SaveToStream();
        Result OpenFromStream(Stream inputStream, string fileName);

    }
}
