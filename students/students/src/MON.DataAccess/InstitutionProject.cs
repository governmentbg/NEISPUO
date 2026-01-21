using System;
using System.Collections.Generic;

namespace MON.DataAccess
{
    public partial class InstitutionProject
    {
        public InstitutionProject()
        {
            InstitutionProjectPartner = new HashSet<InstitutionProjectPartner>();
            InstitutionProjectPriorityArea = new HashSet<InstitutionProjectPriorityArea>();
        }

        public int InstitutionProjectId { get; set; }
        public int InstitutionId { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? ProjectProgramTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? PlanPreSchoolCount { get; set; }
        public int? PlanPrimarySchoolCount { get; set; }
        public int? PlanSecondarySchoolCount { get; set; }
        public int? PlanHighSchoolCount { get; set; }
        public int? ActualPreSchoolCount { get; set; }
        public int? ActualPrimarySchoolCount { get; set; }
        public int? ActualSecondarySchoolCount { get; set; }
        public int? ActualHighSchoolCount { get; set; }
        public int? InterPreSchoolCount { get; set; }
        public int? InterPrimarySchoolCount { get; set; }
        public int? InterSecondarySchoolCount { get; set; }
        public int? InterHighSchoolCount { get; set; }
        public string Goals { get; set; }
        public int? SysUserId { get; set; }

        public virtual InstitutionDetail Institution { get; set; }
        public virtual ICollection<InstitutionProjectPartner> InstitutionProjectPartner { get; set; }
        public virtual ICollection<InstitutionProjectPriorityArea> InstitutionProjectPriorityArea { get; set; }
    }
}
