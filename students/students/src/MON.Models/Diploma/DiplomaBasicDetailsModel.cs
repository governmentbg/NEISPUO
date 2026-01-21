namespace MON.Models.Diploma
{
    public class DiplomaBasicDetailsModel : BasicDocumentDetailsModel
    {
        public new int Id { get; set; }
        public bool IsSigned { get; set; }
        public bool IsEditable { get; set; }
        public bool IsFinalized { get; set; }
        public bool IsCancelled { get; set; }
        public bool CanBeEdit => IsEditable || (!IsSigned && !IsFinalized && !IsCancelled);
    }
}
