namespace Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CourseModel
    {
        public string orderNum { get; set; }
        public string courseName { get; set; }
        public string sectionASession0Grade { get; set; }
        public string sectionASession1Grade { get; set; }
        public string sectionASession2Grade { get; set; }
        public string sectionASession3Grade { get; set; }
        public string sectionAFinalGrade { get; set; }
        public string sectionBVSession0Grade { get; set; }
        public string sectionBVSession1Grade { get; set; }
        public string sectionBVSession2Grade { get; set; }
        public string sectionBVSession3Grade { get; set; }
        public string sectionBVFinalGrade { get; set; }
        public string sectionAHours { get; set; }
        public string sectionBVHours { get; set; }
    }
}
