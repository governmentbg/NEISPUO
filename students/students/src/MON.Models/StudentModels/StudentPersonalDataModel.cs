namespace MON.Models.StudentModels
{
    public class StudentPersonalDataModel : StudentCreateModel
    {
        public string PublicEduNumber { get; set; }
        public DropdownViewModel PinType { get; set; }
        public DropdownViewModel Gender { get; set; }
    }
}
