using System.Collections.Generic;

namespace MON.Models.Diploma
{

    public class DiplomaSectionsSubjectsModel
    {
        public int DiplomaId { get; set; }
        public int? BasicDocumentId { get; set; }
        public List<DiplomaSectionModel> Sections { get; set; }
    }

    public class DiplomaSectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHorariumHidden { get; set; }
        public string BasicSubjectType { get; set; }
        public List<int> SubjectTypes { get; set; }
        public List<int> GradeTypes { get; set; }
        public List<DiplomaSubjectModel> Subjects { get; set; }
        public List<DiplomaTemplateSubjectPartModel> TemplatePartsWithSubjects { get; set; }
    }
}
