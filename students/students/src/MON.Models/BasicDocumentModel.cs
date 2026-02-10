namespace MON.Models
{
    public class BasicDocumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasSchema { get; set; }
        public bool IsValidation { get; set; }
        public string CodeClassName { get; set; }
        public bool HasBarcode { get; set; }
        /// <summary>
        /// Приложение към диплома
        /// </summary>
        public bool IsAppendix { get; set; }
        /// <summary>
        /// Дубликат на диплома
        /// </summary>
        public bool IsDuplicate { get; set; }
        public bool IsIncludedInRegister { get; set; }
        public bool IsRuoDoc { get; set; }
    }
}
