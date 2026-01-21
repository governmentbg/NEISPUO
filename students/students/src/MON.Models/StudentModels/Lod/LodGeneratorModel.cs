namespace MON.Models.StudentModels.Lod
{
    using System.Collections.Generic;

    public class LodGeneratorModel
    {
        public int PersonId { get; set; }
        public List<int> SchoolYears { get; set; } = new List<int>();

        public int? ClassId { get; set; }

        /// <summary>
        /// В ЛОД в word липсва дата на отписване #1316.
        /// При създаването на док. за преместване или отписване първо се създава ЛОД, който се подписва, и след това се прикачва като файл към документа.
        /// В този случай ще подадем модела на док. за преместване или описваме пред да сме го записали в базата.
        /// </summary>
        public RelocationDocumentModel RelocationDocument { get; set; }
        public DischargeDocumentModel DischargeDocument { get; set; }
    }
}
