namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MultiYearDiplomaGradeModel: IGradeWithSubjectName
    {
        public int DocumentPartId { get; set; }
        public string DocumentPartName { get; set; }
        public int Position { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public DiplomaGradeModel Grade5
        {
            get
            {
                return Get(5);
            }
        }
        public DiplomaGradeModel Grade6
        {
            get
            {
                return Get(6);
            }
        }
        public DiplomaGradeModel Grade7
        {
            get
            {
                return Get(7);
            }
        }
        public int GradeCategory { get; set; }
        public List<DiplomaGradeModel> Grades { get; set; }
        public DiplomaGradeModel Get(int basicClassId)
        {
            return Grades?.FirstOrDefault(i => i.BasicClassId == basicClassId);
        }
    }
}
