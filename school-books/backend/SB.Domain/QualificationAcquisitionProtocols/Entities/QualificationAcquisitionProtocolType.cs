namespace SB.Domain;

using System.ComponentModel;

public enum QualificationAcquisitionProtocolType
{
    [Description("за придобиване на професионална квалификация")]
    QualificationAcquisition = 1,

    [Description("за оценките от държавен изпит за придобиване на степен на професионална квалификация")]
    QualificationAcquisitionExamGrades = 2,

    [Description("за придобиване на професионална квалификация по част от професия")]
    QualificationAcquisitionStateExamGrades = 3
}
