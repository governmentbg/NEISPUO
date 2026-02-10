namespace MON.Models.LeadTeacher
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LeadTeacherViewModel: LeadTeacherModel
    {
        public string LeadTeacherName { get; set; }
        public string ClassGroupName { get; set; }
        public bool? CurrentlyValid { get; set; }
    }
}
