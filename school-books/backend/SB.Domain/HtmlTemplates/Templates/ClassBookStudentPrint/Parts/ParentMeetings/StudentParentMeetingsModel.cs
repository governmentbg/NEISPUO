namespace SB.Domain;

using System;

public record StudentParentMeetingsModel(
    StudentParentMeetingsModelParentMeeting[] ParentMeetings
);

public record StudentParentMeetingsModelParentMeeting(
    DateTime Date,
    string Title,
    string? Description
);
