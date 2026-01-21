namespace SB.Domain;

using System;

public record StudentIndividualWorksModel(
    StudentIndividualWorksModelIndividualWork[] IndividualWorks
);

public record StudentIndividualWorksModelIndividualWork(
    DateTime Date,
    string IndividualWorkActivity
);
