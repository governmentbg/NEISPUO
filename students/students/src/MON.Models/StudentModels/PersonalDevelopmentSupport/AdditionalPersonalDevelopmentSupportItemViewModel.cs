namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System.Collections.Generic;
    using System;

    public class AdditionalPersonalDevelopmentSupportItemViewModel : AdditionalPersonalDevelopmentSupportItemModel
    {
        public string TypeName { get; set; }
        public new IEnumerable<DocumentViewModel> SuspensionDocuments { get; set; } = Array.Empty<DocumentViewModel>();
    }
}
