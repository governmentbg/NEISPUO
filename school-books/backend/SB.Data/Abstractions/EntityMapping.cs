namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal abstract class EntityMapping : IEntityMapping
{
    protected int HiLoBlockSize { get; }

    public EntityMapping(IOptions<DataOptions> options)
    {
        this.HiLoBlockSize = options.Value.HiLoBlockSize;
    }

    public abstract void AddFluentMapping(ModelBuilder builder);
}
