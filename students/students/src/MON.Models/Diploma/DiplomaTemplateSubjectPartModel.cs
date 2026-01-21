namespace MON.Models.Diploma
{
    using System.Collections.Generic;

    public class DiplomaTemplateSubjectPartModel
    {
        public int BasicDocumentPartId { get; set; }
        public bool IsHorariumHidden { get; set; }
        public int Position { get; set; }
        public string BasicDocumentPartName { get; set; }
        public List<DiplomaTemplateSubjectModel> Subjects { get; set; }
    }
}
