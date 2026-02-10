namespace SB.Domain;

using System;

public record ReplrParticipationsModel(
    ReplrParticipationsModelReplrParticipation[] ReplrParticipations
);

public record ReplrParticipationsModelReplrParticipation(
    string ReplrParticipationType,
    DateTime Date,
    string? Topic,
    string? InstName,
    string Attendees
);
