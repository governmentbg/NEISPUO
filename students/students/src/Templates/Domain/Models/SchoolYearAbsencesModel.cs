namespace Domain.Models
{

    public class SchoolYearAbsencesModel
    {
        public string schoolYear { get; set; }

        public string firstTermExcused { get; set; }
        public string firstTermUnexcused { get; set; }

        public string secondTermExcused { get; set; }
        public string secondTermUnexcused { get; set; }

        public string totalExcused { get; set; }
        public string totalTermUnexcused { get; set; }
    }
}
