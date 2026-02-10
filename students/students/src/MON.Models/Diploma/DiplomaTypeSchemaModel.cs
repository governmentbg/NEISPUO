using System.Collections.Generic;

namespace MON.Models.Diploma
{
    public class DiplomaTypeSchemaModel : DiplomaTypeModel
    {
        public List<BasicDocumentPartModel> BasicDocumentParts { get; set; }
    }

    public class BasicDocumentPartModel
    {
        public int? Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Uid { get; set; }
        public string BasicClass { get; set; }
        public bool? IsHorariumHidden { get; set; }
        public short? BasicSubjectTypeId { get; set; }
        public List<BasicDocumentSubjectModel> BasicDocumentSubjects { get; set; }
    }
}
