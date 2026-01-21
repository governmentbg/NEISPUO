namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentIndividualWorksVO(
        DateTime Date,
        string IndividualWorkActivity
    );
}
