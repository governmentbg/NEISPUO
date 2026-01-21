using Kontrax.RegiX.Core.TestStandard.Models;
using System;
using System.Data;

namespace Kontrax.RegiX.Core.TestStandard.ModelExtensions
{
    public static class RegiXReportModelExtension
    {
        public static RegiXReportModel RowToModel(this DataRow row)
        {
            if (row == null)
                return null;

            RegiXReportModel model = new RegiXReportModel
            {
                Id = row.Field<int>("Id"),
                ProviderName = row.Field<string>("ProviderName"),
                RegisterName = row.Field<string>("RegisterName"),
                ReportName = row.Field<string>("ReportName"),
                AdapterSubdirectory = row.Field<string>("AdapterSubdirectory"),
                OperationName = row.Field<string>("OperationName"),
                RequestXsd = row.Field<string>("RequestXsd"),
                ResponseXsd = row.Field<string>("ResponseXsd"),
                Operation = row.Field<string>("Operation"),
                IsDeleted = row.Field<bool>("IsDeleted")

            };

            return model;
        }

        public static string ToFullString(this RegiXReportModel model)
        {
            string info = String.Format(@"Id: {0}
ProviderName: {1}
RegisterName: {2}
ReportName: {3}
AdapterSubdirectory: {4}
OperationName: {5}
RequestXsd: {6}
ResponseXsd: {7}
Operation: {8}
IsDeleted: {9}",
model.Id, model.ProviderName, model.RegisterName, model.ReportName, model.AdapterSubdirectory,
model.OperationName, model.RequestXsd, model.ResponseXsd, model.Operation, model.IsDeleted);

            return info;
        }
    }
}
