namespace Helpdesk.Models.Issue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InstitutionModel
    {
        public InstitutionModel()
        {
            Departments = new List<Department>();
        }

        public int InstitutionId { get; set; }
        public short SchoolYear { get; set; }
        public string InstitutionName { get; set; }
        public int? TownId { get; set; }
        public string Town { get; set; }
        public int? MunicipalityId { get; set; }
        public string Municipality { get; set; }
        public int? RegionId { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public List<Department> Departments { get; set; }
    }

    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? TownId { get; set; }
        public string Town { get; set; }
    }

}
