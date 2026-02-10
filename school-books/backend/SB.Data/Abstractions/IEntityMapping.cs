namespace SB.Data;

using Microsoft.EntityFrameworkCore;

internal interface IEntityMapping
{
    void AddFluentMapping(ModelBuilder builder);
}
