namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_34Model : DiplomaModel
    {
        public Diploma_3_34Model(bool dummy) : base(dummy) 
        {
            if (dummy)
            {
                GraduationCommissionMembers = new List<CommissionMember>()
                {
                    new CommissionMember() { Name = "Иван Стефанов Петров" },
                    new CommissionMember() { Name = "Петър Иванов Христов" },
                    new CommissionMember() { Name = "Мария Петрова Гроздева" },
                    new CommissionMember() { Name = "Добромир Петров Гроздев" }
                };

                AdditionalDZI = new List<DiplomaGradeModel>()
                {
                    new DiplomaGradeModel() {SubjectName = "Биология", GradeText = "Отличен", Grade = 5.87m },
                    new DiplomaGradeModel() {SubjectName = "Химия", GradeText = "Много добър", Grade = 5.47m },
                    new DiplomaGradeModel() {SubjectName = "Информатика", GradeText = "Отличен", Grade = 5.97m },
                    new DiplomaGradeModel() {SubjectName = "Философия", GradeText = "Добър", Grade = 4.37m },
                    new DiplomaGradeModel() {SubjectName = "История", GradeText = "Среден", Grade = 3.21m },
                    new DiplomaGradeModel() {SubjectName = "Психология", GradeText = "Отличен", Grade = 5.58m },
                    new DiplomaGradeModel() {SubjectName = "Механика", GradeText = "Отличен", Grade = 5.50m },
                    new DiplomaGradeModel() {SubjectName = "Техническо чертане", GradeText = "Добър", Grade = 4.25m },
                    new DiplomaGradeModel() {SubjectName = "Телекомуникация", GradeText = "Среден", Grade = 2.50m }
                };
            }
        }

        public int? ZIPProfBasicDocumentPartId { get; set; }
        public int? ZIPNoProfBasicDocumentPartId { get; set; }
        public int? SIPBasicDocumentPartId { get; set; }
        public int? MandatoryDZIBasicDocumentPartId { get; set; }
        public int? AdditionalDZIBasicDocumentPartId { get; set; }
        public List<DiplomaGradeModel> AdditionalDZI { get; set; }
    }
}
