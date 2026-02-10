namespace MON.Models.StudentModels.PersonalDevelopmentSupport
{
    using System;
    using System.Collections.Generic;

    public class CommonPersonalDevelopmentSupportModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public IEnumerable<CommonPersonalDevelopmentSupportItemModel> Items { get; set; } = new List<CommonPersonalDevelopmentSupportItemModel>();
        public IEnumerable<DocumentModel> Documents { get; set; } = Array.Empty<DocumentModel>();
    }
}
