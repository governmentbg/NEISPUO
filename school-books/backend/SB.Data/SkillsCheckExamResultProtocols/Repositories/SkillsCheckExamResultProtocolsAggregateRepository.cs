namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;

internal class SkillsCheckExamResultProtocolsAggregateRepository : ScopedAggregateRepository<SkillsCheckExamResultProtocol>
{
    public SkillsCheckExamResultProtocolsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<SkillsCheckExamResultProtocol>, IQueryable<SkillsCheckExamResultProtocol>>[] Includes =>
        new Func<IQueryable<SkillsCheckExamResultProtocol>, IQueryable<SkillsCheckExamResultProtocol>>[]
        {
            (q) => q.Include(e => e.Evaluators)
        };
}
