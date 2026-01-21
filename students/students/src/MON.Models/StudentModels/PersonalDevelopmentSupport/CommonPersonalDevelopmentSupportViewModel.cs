namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System;
    using System.Collections.Generic;

    public class CommonPersonalDevelopmentSupportViewModel : CommonPersonalDevelopmentSupportModel
    {
        public string SchoolYearName { get; set; }
        public new IEnumerable<CommonPersonalDevelopmentSupportItemViewModel> Items { get; set; } = Array.Empty<CommonPersonalDevelopmentSupportItemViewModel>();
        public new IEnumerable<DocumentViewModel> Documents { get; set; } = Array.Empty<DocumentViewModel>();
        
    }
}
