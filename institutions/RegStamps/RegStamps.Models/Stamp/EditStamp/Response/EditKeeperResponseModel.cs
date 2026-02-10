namespace RegStamps.Models.Stamp.EditStamp.Response
{
    using System.ComponentModel.DataAnnotations;

    using Shared.Database;

    public class EditKeeperResponseModel
    {
        public int KeeperId { get; set; }
        public int IdType { get; set; }

        [Display(Name = "Вид идентификатор")]
        public string IdNumber { get; set; }

        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Display(Name = "Презиме")]
        public string SecondName { get; set; }

        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }

        public int OccupationId { get; set; }

        [Display(Name = "Длъжност")]
        public string OccupationName { get; set; }
        public IEnumerable<OccupationDatabaseModel> OccupationDropDown { get; set; } = new List<OccupationDatabaseModel>();
        public IEnumerable<IdTypeDatabaseModel> IdTypeDropDown { get; set; } = new List<IdTypeDatabaseModel>();
    }
}
