namespace MON.Models.StudentModels
{
    public class StudentRelativeModel
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DropdownViewModel RelativeType { get; set; }
        public string Notes { get; set; }
        public DropdownViewModel WorkStatus { get; set; }
        public DropdownViewModel PinType { get; set; }
        public string Pin { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DropdownViewModel EducationType { get; set; }
    }
}
