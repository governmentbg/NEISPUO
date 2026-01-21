namespace MON.Models.StudentModels
{
    using MON.Models.StudentModels.Update;

    public class StudentBasicDetailsModel : StudentBasicDetailsUpdateModel
    {
        public DropdownViewModel PermanentResidence { get; set; }
        public DropdownViewModel UsualResidence { get; set; }
    }
}
