namespace MON.DataAccess.Dto
{
    using System;

    public class StudentByInstitutionOccupationIntervalDto
    {
        public int GroupNo { get; set; }
        public int PersonId { get; set; }
        public int InstitutionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
