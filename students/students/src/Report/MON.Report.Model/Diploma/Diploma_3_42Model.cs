namespace MON.Report.Model.Diploma
{
    using MON.Report.Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Diploma_3_42Model : DiplomaModel
    {
        public Diploma_3_42Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                GraduationCommissionMembers = new List<CommissionMember>()
                {
                    new CommissionMember() { Name = "Иван Стефанов Петров" },
                    new CommissionMember() { Name = "Петър Иванов Христов" },
                    new CommissionMember() { Name = "Мария Петрова Гроздева" }
                };

                Qualification = "Някаква пробна квалификация с по-дълъг текст за да се види визуализирането на дипломата в Telerik Report Designer!";
            }
        }

        public string Qualification { get; set; }
    }
}
