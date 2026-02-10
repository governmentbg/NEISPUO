namespace MON.Models.Absence
{
    using System.Collections.Generic;
    public class InstitutionsResponseModel
    {
        public IEnumerable<InstitutionImportedAbsencesOutputModel> Institutions { get; set; }

        public int InstitutionsTotalCount { get; set; }
    }
}
