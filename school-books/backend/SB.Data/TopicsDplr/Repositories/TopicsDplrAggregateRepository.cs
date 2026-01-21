namespace SB.Data;
using System.Linq;

using System;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class TopicsDplrAggregateRepository : ScopedAggregateRepository<TopicDplr>, ITopicsDplrAggregateRepository
{
    public TopicsDplrAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<TopicDplr>, IQueryable<TopicDplr>>[] Includes =>
        new Func<IQueryable<TopicDplr>, IQueryable<TopicDplr>>[]
        {
            (q) => q.Include(t => t.Teachers),
            (q) => q.Include(t => t.Students),
        };
}
