namespace SB.Domain;

using System;

public record RemarksModel(
    RemarksModelStudent[] Students
);

public record RemarksModelStudent(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    RemarksModelRemark[] Remarks
);

public record RemarksModelRemark(
    DateTime Date,
    string RemarkType,
    string SubjectName,
    string SubjectTypeName,
    string Description
);
