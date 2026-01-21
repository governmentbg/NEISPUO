namespace SB.Domain;

using System;

public record IndividualWorksModel(
    IndividualWorksModelIndividualWork[] IndividualWorks
);

public record IndividualWorksModelIndividualWork(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    DateTime Date,
    string IndividualWorkActivity
);
