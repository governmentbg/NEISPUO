using System;
using System.Configuration;
using System.IO;

namespace Kontrax.RegiX.Core.TestStandard.Services
{
    internal class AdapterFileReader
    {
        public static string DemandAdaptersPath()
        {
            const string regiXAdaptersPathKey = "RegiXAdaptersPath";
            string adaptersPath = ConfigurationManager.AppSettings[regiXAdaptersPathKey];
            if (string.IsNullOrEmpty(adaptersPath))
            {
                throw new Exception($"Системната настройка {regiXAdaptersPathKey} не е попълнена.");
            }
            return adaptersPath;
        }

        public static string DemandXsdPath(string adapterSubdirectory, string xsdName, bool isResponse)
        {
            string adaptersPath = DemandAdaptersPath();
            string xsdPath = Path.Combine(adaptersPath, adapterSubdirectory ?? string.Empty, xsdName ?? string.Empty);
            if (!File.Exists(xsdPath))
            {
                string xsdType = isResponse ? "отговора" : "заявката";
                string xsdNameError = !string.IsNullOrEmpty(xsdName) ? $"Не съществува файлът със схемата на {xsdType} ({xsdName})" : $"Не е указан файлът със схемата на {xsdType}";
                string subdirError = !string.IsNullOrEmpty(adapterSubdirectory) ? $" в папката на адаптера ({adapterSubdirectory})." : $" и не е указана папката на адаптера.";
                throw new Exception(xsdNameError + subdirError);
            }
            return xsdPath;
        }
    }
}
