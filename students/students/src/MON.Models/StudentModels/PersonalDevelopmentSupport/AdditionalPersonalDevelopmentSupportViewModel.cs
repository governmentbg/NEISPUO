namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System;
    using System.Collections.Generic;

    public class AdditionalPersonalDevelopmentSupportViewModel : AdditionalPersonalDevelopmentSupportModel
    {
        public string SchoolYearName { get; set; }
        public string FinalSchoolYearName { get; set; }
        public string PeriodTypeName { get; set; }
        public string StudentTypeName { get; set; }
        public new IEnumerable<AdditionalPersonalDevelopmentSupportItemViewModel> Items { get; set; } = new List<AdditionalPersonalDevelopmentSupportItemViewModel>();
        public new IEnumerable<DocumentViewModel> Orders { get; set; } = Array.Empty<DocumentViewModel>();
        public new IEnumerable<DocumentViewModel> Scorecards { get; set; } = Array.Empty<DocumentViewModel>();
        public new IEnumerable<DocumentViewModel> Plans { get; set; } = Array.Empty<DocumentViewModel>();
        public new IEnumerable<DocumentViewModel> Documents { get; set; } = Array.Empty<DocumentViewModel>();
        public new IEnumerable<SopDetailsViewModel> Sop { get; set; } = Array.Empty<SopDetailsViewModel>();
    }
}
