namespace MON.Report.Model.Diploma
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Diploma_3_114Model : DiplomaDuplicateModel
    {
        public Diploma_3_114Model(bool dummy) : base(dummy)
        {
            if (dummy)
            {
                Graduated = "курс по обучение";
                Enactment = "Точно наименование на нормативния акт";
                StateGazetteNo = "52";
                StateGazetteDate = "06.12.2022";
                Ammendment = "изм. и доп. 07.12.202";
                Certification = "Правоспособност за шлосерство";
                Level = "Степен";
            }
        }

        // Завърши
        public string Graduated { get; set; }

        // Точно наименование на нормативния акт
        public string Enactment { get; set; }
        // Д.В Номер
        public string StateGazetteNo { get; set; }
        // Д.В Дата
        public string StateGazetteDate { get; set; }
        // Изм. и доп.
        public string Ammendment { get; set; }
        // Правоспособност
        public string Certification { get; set; }
        // Степен
        public string Level { get; set; }

    }
}
