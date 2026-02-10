using Kontrax.RegiX.Core.TestStandard.Models;
using System.Collections.Generic;

namespace Kontrax.RegiX.Core.TestStandard.Services
{
    internal class CallService
    {
        internal static List<string> GetRegiXReportRequestConfigErrors(RegiXReportModel regiXReport)
        {
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrEmpty(regiXReport.Operation) ||
                string.IsNullOrEmpty(regiXReport.AdapterSubdirectory) ||
                string.IsNullOrEmpty(regiXReport.RequestXsd))
            {
                errorMessages.Add($"RegiX справката \"{regiXReport.FullName}\" не е конфигурирана.");
                if (regiXReport != null)
                {
                    // Ако конфигурацията е попълнена частично, се посочва точният проблем.
                    if (string.IsNullOrEmpty(regiXReport.Operation))
                    {
                        errorMessages.Add("Не е посочена операцията в RegiX.");
                    }
                    if (string.IsNullOrEmpty(regiXReport.AdapterSubdirectory))
                    {
                        errorMessages.Add("Не е посочена папката на адаптера.");
                    }
                    if (string.IsNullOrEmpty(regiXReport.RequestXsd))
                    {
                        errorMessages.Add("Не е посочена схемата на заявката.");
                    }
                }
            }
            return errorMessages;
        }
    }
}
