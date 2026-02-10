namespace MON.Shared.Enums
{
    public enum PermissionGroupEnum
    {
        Reader = 0,
        Owner = 1,
        LeadTeacher = 2,
        DiplomaSpecial = 3,
        LodReader = 4,
        // Служи само за записване на ученик
        AdmissionDocCreator,
        DiplomaCreator,
        // Определя видимостта на функционалността за искане на разрешениe за записване
        AdmissionPermissionRequestManager,
        // Определя видимостта на функционалността за масово записване/отписване в клас
        MassEntrollmentManager,
        PersonalDataReader,
        PartialPersonalDataReader,
        // Дава достъп до Бутон(в timeline-а) за преглед на учебен план. Необходимо е за РУО и ЦИО
        StudentCurriculumReader
    }
}
