namespace MON.DataAccess.Dto
{

    public class StudentSearchDto
    {
        public int PersonId { get; set; }
        public int? EducationalStateId { get; set; }
        public string Pin { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PublicEduNumber { get; set; }
        public string Municipality { get; set; }
        public string District { get; set; }
        public string School { get; set; }
        public int? Age { get; set; }
        public bool IsOwner { get; set; }
        //public string FullName { get; set; }
        public int? PositionID { get; set; }
        public int? InstitutionID { get; set; }
        public int? PinType { get; set; }
    }
}
