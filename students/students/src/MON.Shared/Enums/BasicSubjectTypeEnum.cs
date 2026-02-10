namespace MON.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    public enum BasicSubjectTypeEnum
    {
	[Description("Задължителна подготовка")]
    Mandatory = 1,
    [Description("Задължителноизбираема подготовка")]
    MandatoryElective = 2,
    [Description("Свободноизбираема подготовка")]
    OptionalTrainingCourses = 3,
    [Description("Часове извън учебния план")]
    ClassesOutsodeCurriculum = 5,
    [Description("Задължителни учебни часове")]
    CompulsoryCourses = 11,
    [Description("Избираеми учебни часове")]
    ElectiveCourses = 12,
    [Description("Факултативни учебни часове")]
    OptionalCourses = 13,
    [Description("Целодневна организация/ Личностно развитие")]
    AllDay_PersonalDevelopment = 14,
    [Description("Специални предмети (за спец.училища със СУ)")]
    SpecialSubjects = 15
    }
}
