namespace MON.Models.Dashboards
{
    using System;

    public class StudentToBeDischargedModel
    {
        public bool Selected { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PinType { get; set; }
        public string Pin { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public int PersonId { get; set; }
        public int? NewInstitutionId { get; set; }
        public string NewInstitution { get; set; }
        public string NewPosition { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public DateTime NoteDate { get; set; }
        public int? OldStudentClassid { get; set; }
        public int? OldClassGroupId { get; set; }
        public string OldClassName { get; set; }
        public int? OldInstitutionId { get; set; }
        public string OldInstitution { get; set; }
        public string OldPosition { get; set; }
        public int? RelocationDocumentId { get; set; }
        public int? DischargeDocumentId { get; set; }
        public string SelectionType { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за навигиране до ЛОД-a, меню Общи данни за обучението.
        /// </summary>
        public bool ShowStudentLodLinkBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за създаване на документ за преместване.
        /// </summary>
        public bool ShowRelocationDocumentCreateBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за редакция на документ за преместване.
        /// </summary>
        public bool ShowRelocationDocumentEditBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за потвърждаване на документ за преместване (промяна от чернова на потвърден).
        /// </summary>
        public bool ShowRelocationDocumentConfirmBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за изтриване на документ за преместване.
        /// </summary>
        public bool ShowRelocationDocumentDeleteBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за редакция на документ за отписване.
        /// </summary>
        public bool ShowDischargeDocumentEditBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за потвърждаване на документ за отписване (промяна от чернова на потвърден).
        /// </summary>
        public bool ShowDischargeDocumentConfirmBtn { get; set; }

        /// <summary>
        /// Определя видимостта на бутон за изтриване на документ за отписване.
        /// </summary>
        public bool ShowDischargeDocumentDeleteBtn { get; set; }
    }
}
