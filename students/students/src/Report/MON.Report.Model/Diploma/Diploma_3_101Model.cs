namespace MON.Report.Model.Diploma
{
    using MON.Report.Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_101Model : DiplomaModel
    {
        public Diploma_3_101Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                GraduationCommissionMembers = new List<CommissionMember>()
                {
                    new CommissionMember() { Name = "Иван Стефанов Петров" },
                    new CommissionMember() { Name = "Петър Иванов Христов" },
                    new CommissionMember() { Name = "Мария Петрова Гроздева" }
                };

                Session = "есенна";

                MainGrade = new DiplomaGradeModel()
                {
                    DocumentPartName = "Задължителни учебни часове",
                    DocumentPartId = 14,
                    SubjectId = 2,
                    SubjectName = "Математика",
                    GradeText = "Отличен",
                    Grade = 5.93m,
                    Horarium = 135
                };
            }
        }

        public string Session { get; set; }
        public DiplomaGradeModel MainGrade { get; set; }
    }
}
