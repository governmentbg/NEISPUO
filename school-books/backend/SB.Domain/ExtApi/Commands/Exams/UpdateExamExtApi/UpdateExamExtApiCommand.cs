namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateExamExtApiCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? ExamId { get; init; }

    public BookExamType? Type { get; init; }
    public int? CurriculumId { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Exam);
    [JsonIgnore]public virtual int? ObjectId => this.ExamId;
}
