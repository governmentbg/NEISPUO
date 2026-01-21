namespace MON.Models
{
    public class RelocationDocumentViewModel : RelocationDocumentModel
    {
        public string StatusName { get; set; }
        public int CurrentStudentClassBasicClassId { get; set; }
        public string CurrentStudentClassBasicClassName { get; set; }
        public string RelocationReasonType { get; set; }
        /// <summary>
        /// Път до документа за печат. Описан в таблица student.AppSettings, ключ RelocationDocumentToBasicClass.
        /// </summary>
        public string ReportPath { get; set; }
        /// <summary>
        /// Определя видимостта на бутона за добавяне(импорт) на оценки в грида с документи за преместване
        /// </summary>
        public bool CanAddEvaluations { get; set; }
        public string SchoolYearName { get; set; }
    }
}
