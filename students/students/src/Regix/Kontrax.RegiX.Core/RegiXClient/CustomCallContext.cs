using RegiXServiceReference;

namespace Kontrax.RegiX.Core.TestStandard
{
    public static class CustomCallContext
    {
        public static CallContext GetCallContext()
        {
            CallContext callContext = new CallContext
            {
                AdministrationName = "Администрация",
                AdministrationOId = "1.2.3.4.5.6.7.8.9",
                EmployeeIdentifier = "myusername@administration.domain",
                EmployeeNames = "Първо Второ Трето",
                EmployeePosition = "Експерт в отдел",
                LawReason = "На основание чл. X от Наредба/Закон/Нормативен акт",
                ServiceURI = "123-12345-01.01.2017",
                ServiceType = "За административна услуга",
                Remark = "За тестване на системата",
                EmployeeAditionalIdentifier = null,
                ResponsiblePersonIdentifier = null
            };

            return callContext;
        }
    }
}
