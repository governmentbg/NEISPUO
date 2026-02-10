namespace SB.Domain;

using System;

public record ParentMeetingsModel(
    ParentMeetingsModelParentMeeting[] ParentMeetings
);

public record ParentMeetingsModelParentMeeting(
    DateTime Date,
    string Title,
    string? Description
);
