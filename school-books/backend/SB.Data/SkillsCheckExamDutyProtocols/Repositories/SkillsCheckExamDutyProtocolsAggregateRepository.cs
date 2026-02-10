namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class SkillsCheckExamDutyProtocolsAggregateRepository : ScopedAggregateRepository<SkillsCheckExamDutyProtocol>
{
    public SkillsCheckExamDutyProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<SkillsCheckExamDutyProtocol>, IQueryable<SkillsCheckExamDutyProtocol>>[] Includes =>
        new Func<IQueryable<SkillsCheckExamDutyProtocol>, IQueryable<SkillsCheckExamDutyProtocol>>[]
        {
            (q) => q.Include(e => e.Supervisors)
        };
}
