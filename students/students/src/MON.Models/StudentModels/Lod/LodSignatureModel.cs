namespace MON.Models.StudentModels.Lod
{
    public class LodSignatureModel
    {
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public string Signature { get; set; }
        public int? ClassId { get; set; }
    }
}
