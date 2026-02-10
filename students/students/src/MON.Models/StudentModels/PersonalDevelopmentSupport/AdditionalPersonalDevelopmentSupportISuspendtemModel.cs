namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System;
    using System.Collections.Generic;

    public class AdditionalPersonalDevelopmentSupportISuspendtemModel : AdditionalPersonalDevelopmentSupportItemModel
    {
        public IEnumerable<DocumentModel> Documents { get; set; } = Array.Empty<DocumentModel>();
    }
}
