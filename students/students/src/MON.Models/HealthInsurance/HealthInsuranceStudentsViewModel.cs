namespace MON.Models.HealthInsurance
{
    using System.Security.Claims;
    using System;
    using MON.Shared;
    using System.Linq;
    using System.Globalization;
    using MON.Shared.Extensions;

    public class HealthInsuranceStudentsViewModel : HealthInsuranceDto
    {
        public int StartDayNumber { get; set; }
        public int EndDayNumber { get; set; }
        public decimal MinimalInsuranceIncomeRate { get; set; }
        public decimal InsuranceContributionPercentage { get; set; }
        public string Currency {  get; set; }
        public string AltCurrency { get; set; }
        public decimal AltMinimalInsuranceIncomeRate { get; set; }

        /// <summary>
        /// Дните за осигуряване по дневната база за осигураване
        /// </summary>
        public decimal InsurancePayRate => decimal.Round(MonthDays.HasValue
            ? (MinimalInsuranceIncomeRate / MonthDays.Value) * (EndDayNumber - StartDayNumber + 1)
            : 0, 2);

        /// <summary>
        /// Дните за осигуряване по дневната база за осигураване
        /// </summary>
        public decimal AltInsurancePayRate => decimal.Round(MonthDays.HasValue
            ? (AltMinimalInsuranceIncomeRate / MonthDays.Value) * (EndDayNumber - StartDayNumber + 1)
            : 0, 2);

        /// <summary>
        /// Код корекция:
        /// 0 - редовни данни
        /// 1-коригиращи данни
        /// 8-заличаващи данни
        /// </summary>
        public int CorrectionCode { get; set; }

        /// <summary>
        /// Избран от потребителя да не се вкл. в експорта към НАП
        /// </summary>
        public bool IsExcludeFromList { get; set; }

        public string MinimalInsuranceIncomeRateStr => MinimalInsuranceIncomeRate.ToExtString(Currency, AltMinimalInsuranceIncomeRate, AltCurrency);

        public string ToFileLine(int year, int month, string systemVatNumber, string institutionVatNumber, int insuranceType)
        {
            string line = $"{month:00},{year:00},\"{(institutionVatNumber ?? "").Truncate(13).PadRight(13,' ')}\",{insuranceType:00},\"{Pin.Truncate(10).PadLeft(10, '0')}\"," +
                $"{(PinTypeId != null && PinTypeId == 1 ? 2 : PinTypeId):0},\"{(LastName ?? "").Truncate(25).ToUpper()}\",\"{$"{FirstName.FirstOrDefault()}{(MiddleName.IsNullOrEmpty() ? "" : MiddleName.FirstOrDefault().ToString())}".ToUpper()}\"," +
                $"{StartDayNumber},{EndDayNumber},{InsurancePayRate.ToString("#####.00", CultureInfo.InvariantCulture)},{InsuranceContributionPercentage.ToString("###.00", CultureInfo.InvariantCulture)}," +
                $"{CorrectionCode},\"{systemVatNumber}\"";

            return line;
        }
    }
}
