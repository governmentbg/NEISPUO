namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;

internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private IEnumerable<IEntityMapping> mappings;

    public DbContext(DbContextOptions<DbContext> options,
        IEnumerable<IEntityMapping> mappings)
        : base(options)
    {
        this.mappings = mappings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var mapping in this.mappings)
        {
            mapping.AddFluentMapping(modelBuilder);
        }

        this.RegisterQoTypes(modelBuilder, Assembly.GetAssembly(typeof(SBModule))!);
        this.RegisterQoTypes(modelBuilder, Assembly.GetAssembly(typeof(DataModule))!);
        this.RegisterQoTypes(modelBuilder, Assembly.GetAssembly(typeof(DomainModule))!);

        modelBuilder.HasDbFunction(() => StringUtils.JoinNames(default, default, default));
        modelBuilder.HasDbFunction(() => StringUtils.JoinNames(default, default));
        modelBuilder.HasDbFunction(() => DateExtensions.GetDateFromIsoWeek(default, default, default));
    }

    private void RegisterQoTypes(ModelBuilder modelBuilder, Assembly assembly)
    {
        foreach (var qoType in assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(KeylessAttribute), true).Length > 0))
        {
            modelBuilder.Entity(qoType);
        }
    }
}
