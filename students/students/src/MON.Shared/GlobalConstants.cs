namespace MON.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class GlobalConstants
    {
        public const string PIN_ANONYMIZATION_STRING = "XXXXXXXXXXX";
        public static readonly IList<int> BasicDocumentsWithEctsGrade = new ReadOnlyCollection<int>(new List<int>
        {
           255,257,258
        });

        public static readonly IList<int?> SubjectTypesOfGeneralEduSubject = new List<int?> { 101 };
        public static readonly IList<int?> SubjectTypesOfProfileSubject = new List<int?> { 152, 153, 154, 155, 156 };
        // Община - други
        public const int MunicipalityOther = 265;
        // Област - други
        public const int RegionOther = 29;

        // Област - други
        public const int DippkSubjectId = 93;
        public static readonly IList<int> BasicDocumentsWithDippk = new ReadOnlyCollection<int>(new List<int>
        {
           17,31,94,193,194,195,253
        });

        public static readonly DateTime StudentClassEnrollmentMigrationDate = new DateTime(2021, 12, 20); // Преди тази дата смятаме, че данните са от миграция

        public static string LodIsFinalizedError(int? schoolYear)
        {
            return $"Записът не е разрешен! ЛОД е финализирано за учебната {schoolYear}/{schoolYear+1} година.";
        }

        // Възможност за въвеждане на ЕИК на работодател при записване в паралелка #1258
        // • Възможност за въвеждане на ЕИК на работодател при записване в паралелка на ученици от XI и XII клас във форма на професионално обучение чрез работа (дуална система на обучение);
        public const int DualEduFormId = 9;
        // • Полето ще е достъпно само при записване в професионални паралелки (ClassType = 20), 11-ти и 12-ти клас, в обучение чрез работа;
        public const int DualClassTypeId = 20;

    }
}
