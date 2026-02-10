namespace MON.Models.Institution
{
    using System;

    public class StudentInfoExternalModel : StudentInfoModel
    {
        public int? Id { get; set; }
        public string InstitutionName { get; set; }
        public string PositionName { get; set; }
        public int InstitutionId { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
        public int BasicClassId { get; set; }
        public string InstitutionTown { get; set; }
        public DocumentModel Document { get; set; }
        public string[] UsernamesList => string.IsNullOrWhiteSpace(Usernames) ? Array.Empty<string>() : Usernames.Split("|", System.StringSplitOptions.RemoveEmptyEntries);
        public string[] InitialPasswordsList => string.IsNullOrWhiteSpace(InitialPasswords) ? Array.Empty<string>() : InitialPasswords.Split("|", System.StringSplitOptions.RemoveEmptyEntries);
        /// <summary>
        /// Неподписани ЛОД-ве
        /// </summary>
        public string LodNotFinalizationYears { get; set; }
        public string[] LodNotFinalizationYearsList => string.IsNullOrWhiteSpace(LodNotFinalizationYears) ? Array.Empty<string>() : LodNotFinalizationYears.Split("|", System.StringSplitOptions.RemoveEmptyEntries);

        public short SchoolYear { get; set; }
        public bool IsCurrent { get; set; }
    }
}