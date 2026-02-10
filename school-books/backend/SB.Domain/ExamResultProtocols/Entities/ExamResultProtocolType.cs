namespace SB.Domain;

using System.ComponentModel;

public enum ExamResultProtocolType
{
    [Description("3-80 Протокол за резултата от писмен, устен или практически изпит")]
    Exam = 1,

    [Description("3-80 ПОО Протокол за резултата от изпит за професионална квалификация")]
    Qualification = 2,

    [Description("3-80 Протокол за резултатите от изпита за проверка на способностите")]
    SkillsCheck = 3,

    [Description("3-84 Протокол за удостоверяване на завършен гимназиален етап")]
    HighSchoolCertificate = 4,

    [Description("3-81В Протокол за придобиване на професионална квалификация")]
    QualificationAcquisition = 5,

    [Description("3-81В ПОО Протокол за оценките от държавен изпит за придобиване на степен на професионална квалификация")]
    QualificationAcquisitionExamGrades = 6,

    [Description("3-81В ПОО Протокол за оценките от изпит за придобиване на професионална квалификация по част от професия")]
    QualificationAcquisitionStateExamGrades = 7,

    [Description("3-81Д Протокол на комисията за оценяване на изпит чрез защитa на дипломен проект - част по теория на професията")]
    GraduationThesisDefense = 8,
}
