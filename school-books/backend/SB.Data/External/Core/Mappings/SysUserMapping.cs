namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.Domain;

class SysUserMapping : EntityMapping
{
    public SysUserMapping(IOptions<DataOptions> options)
        : base(options)
    {
    }

    public override void AddFluentMapping(ModelBuilder modelBuilder)
    {
        var schema = "core";
        var tableName = "SysUser";

        var builder = modelBuilder.Entity<SysUser>();

        builder.ToTable(tableName, schema);

        builder.HasKey(e => e.SysUserId);
    }
}
