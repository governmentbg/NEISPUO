namespace MON.Models.StudentModels
{
    public class SelfGovernmentModel
    {
        public int? Id { get; set; }
        public short SchoolYear { get; set; }
        public int PersonId { get; set; }
        public int ParticipationId { get; set; }
        public int PositionId { get; set; }
        public string ParticipationAdditionalInformation { get; set; }
        public string AdditionalInformation { get; set; }
        public int? InstitutionId { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public int? StudentClassId { get; set; }
        public string SchoolYearName { get; set; }
        public string ParticipationName { get; set; }
        public string PositionName { get; set; }
    }
}
