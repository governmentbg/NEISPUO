namespace SB.Domain;

using System;

public record StudentRemarksModel(
    SchoolTerm Term,
    StudentRemarksModelRemark[] Remarks
);

public record StudentRemarksModelRemark(
    DateTime Date,
    string SubjectName,
    string SubjectTypeName,
    string Description
);
