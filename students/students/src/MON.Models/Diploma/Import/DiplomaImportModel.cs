namespace MON.Models.Diploma.Import
{
    public class DiplomaImportModel
    {
        public string ArchiveName { get; set; }
        public string FileName { get; set; }
        public string RawData { get; set; }

        /// <summary>
        /// Сетва се при ръчно редактиране на дипломи.
        /// Целта е при валидацията да се избегне грешката 'За ученик с PersonalId: 1449266548 и PersonalIdType: 0 съществува документ от тип: 14. Позволен е един документ от този тип.'
        /// </summary>
        public int? DiplomaId { get; set; }

        /// <summary>
        /// Посочва дали се използва ръчно създаване/редакция на диплома през UI-а на ИСУПО
        /// или се импортира чрез xml
        /// </summary>
        public bool IsManualCreateOrUpdate { get; set; }
        public DiplomaImportParseModel ParseModel { get; set; }
    }
}
