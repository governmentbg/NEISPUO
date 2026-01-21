namespace MON.Models.Grid
{
    public class StudentEnvCharacteristicListInput : PagedListInput
    {
        public StudentEnvCharacteristicListInput()
        {
            SortBy = "BasicClassId asc, ClassName asc, FullName asc";
        }
    }
}
