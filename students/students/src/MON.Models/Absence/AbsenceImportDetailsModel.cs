namespace MON.Models.Absence
{
    using System.Collections.Generic;

    public class AbsenceImportDetailsModel : AbsenceExportFileModel
    {
        public List<AbsenceImportModel> Records { get; set; }

        public int TotalRecords => Records?.Count ?? default;

        public string CreatorUsername { get; set; }
        public string SignerUsername { get; set; }
    }
}
