namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class SkillsCheckExamResultProtocol : IAggregateRoot
{
    // EF constructor
    private SkillsCheckExamResultProtocol()
    {
        this.Version = null!;
    }

    public SkillsCheckExamResultProtocol(
        int schoolYear,
        int instId,
        string? protocolNumber,
        int subjectId,
        DateTime? date,
        int studentsCapacity,
        int createdBySysUserId)
    {
        this.SchoolYear = schoolYear;
        this.InstId = instId;
        this.ProtocolNumber = protocolNumber;
        this.SubjectId = subjectId;
        this.Date = date;
        this.StudentsCapacity = studentsCapacity;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
        this.Version = null!;
    }


    public int SchoolYear { get; private set; }

    public int SkillsCheckExamResultProtocolId { get; private set; }

    public int InstId { get; private set; }

    public string? ProtocolNumber { get; private set; }

    public int SubjectId { get; private set; }

    public DateTime? Date { get; private set; }

    public int StudentsCapacity { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public DateTime ModifyDate { get; private set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; }

    // relations
    private readonly List<SkillsCheckExamResultProtocolEvaluator> evaluators = new();
    public IReadOnlyCollection<SkillsCheckExamResultProtocolEvaluator> Evaluators => this.evaluators.AsReadOnly();

    public void UpdateData(
        string? protocolNumber,
        int subjectId,
        DateTime? date,
        int studentsCapacity,
        int modifiedBySysUserId)
    {
        this.ProtocolNumber = protocolNumber;
        this.SubjectId = subjectId;
        this.Date = date;
        this.StudentsCapacity = studentsCapacity;

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SkillsCheckExamResultProtocolEvaluator AddEvaluator(
        string name,
        int modifiedBySysUserId)
    {
        var evaluator = new SkillsCheckExamResultProtocolEvaluator(
            this,
            name);

        this.evaluators.Add(evaluator);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return evaluator;
    }

    public void UpdateEvaluator(
        int skillsCheckExamResultProtocolEvaluatorId,
        string name,
        int modifiedBySysUserId)
    {
        var evaluator = this.evaluators.Single(s => s.SkillsCheckExamResultProtocolEvaluatorId == skillsCheckExamResultProtocolEvaluatorId);
        evaluator.UpdateData(name);

        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;
    }

    public SkillsCheckExamResultProtocolEvaluator RemoveEvaluator(int skillsCheckExamResultProtocolEvaluatorId, int modifiedBySysUserId)
    {
        var evaluator = this.evaluators.Single(s => s.SkillsCheckExamResultProtocolEvaluatorId == skillsCheckExamResultProtocolEvaluatorId);

        this.evaluators.Remove(evaluator);
        this.ModifyDate = DateTime.Now;
        this.ModifiedBySysUserId = modifiedBySysUserId;

        return evaluator;
    }
}
