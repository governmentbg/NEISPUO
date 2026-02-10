namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

internal class ConversationsAggregateRepository : ScopedAggregateRepository<Conversation>, IConversationAggregateRepository
{
    public ConversationsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<Conversation>, IQueryable<Conversation>>[] Includes =>
        new Func<IQueryable<Conversation>, IQueryable<Conversation>>[]
        {
            q => q.Include(c => c.Messages),
            q => q.Include(c => c.Participants)
        };

    public Task<Conversation?> FindByIdOrDefaultAsync(
        int schoolYear,
        int conversationId,
        CancellationToken ct)
    {
        return this.FindEntityOrDefaultAsync(
            this.DbContext.Set<Conversation>(),
            new object[] { schoolYear, conversationId },
            this.Includes,
            ct);
    }

    public async Task<ConversationMessage> AddAsync(ConversationMessage message, CancellationToken ct)
    {
        await this.DbContext.Set<ConversationMessage>().AddAsync(message, ct);

        return message;
    }
}
