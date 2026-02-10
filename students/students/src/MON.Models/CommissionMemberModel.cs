namespace MON.Models
{
    public class CommissionMemberModel
    {
        public int? Id { get; set; }
        public int? TemplateId { get; set; }
        public string Uid { get; set; }
        public int? DiplomaId { get; set; }
        public string FullName { get; set; }
        public string FullNameLatin { get; set; }
        public int Position { get; set; }
    }
}
