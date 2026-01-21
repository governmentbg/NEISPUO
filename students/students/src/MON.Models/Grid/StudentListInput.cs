namespace MON.Models
{
    public class StudentListInput : PagedListInput
    {
        public StudentListInput()
        {
            SortBy = "Id desc";
        }

        public int? StudentId { get; set; }
    }
}
