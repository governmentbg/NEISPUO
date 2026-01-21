namespace SB.Data;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

public sealed class SqlServerSequenceHiLoValueGenerator<TValue> : ValueGenerator<TValue>, IDisposable
    where TValue : struct
{
    private readonly ISequence sequence;
    private readonly int blockSize;

    private HiLoValueGeneratorState generatorState;

    public SqlServerSequenceHiLoValueGenerator(
        ISequence sequence,
        int blockSize)
    {
        this.sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
        this.blockSize = blockSize;

        this.generatorState = new HiLoValueGeneratorState(blockSize);
    }

    public override bool GeneratesTemporaryValues => false;

    public override TValue Next(EntityEntry entry) =>
        this.generatorState.Next<TValue>(() => this.GetNewLowValue(entry));

    public override ValueTask<TValue> NextAsync(EntityEntry entry, CancellationToken cancellationToken)
        => this.generatorState.NextAsync<TValue>(async (ct) => await this.GetNewLowValueAsync(entry, ct), cancellationToken);

    protected override object NextValue(EntityEntry entry)
        => this.Next(entry);

    protected override async ValueTask<object?> NextValueAsync(
        EntityEntry entry,
        CancellationToken cancellationToken)
        => await this.NextAsync(entry, cancellationToken);

    private long GetNewLowValue(EntityEntry entry)
    {
        if (!entry.Context.Database.IsSqlServer())
        {
            return 1;
        }

        var sql = this.GetSql();
        var parameters = this.GetParameters();

        entry.Context.Database.ExecuteSqlRaw(sql, parameters);

        return this.GetRangeFirstValue(parameters);
    }

    private async Task<long> GetNewLowValueAsync(EntityEntry entry, CancellationToken ct)
    {
        if (!entry.Context.Database.IsSqlServer())
        {
            return 1;
        }

        var sql = this.GetSql();
        var parameters = this.GetParameters();

        await entry.Context.Database.ExecuteSqlRawAsync(sql, parameters, ct);

        return this.GetRangeFirstValue(parameters);
    }

    private string GetSql()
        => "exec sp_sequence_get_range @sequence_name, @range_size, @range_first_value OUT";

    private object[] GetParameters()
        => new object[]
            {
                new SqlParameter("@sequence_name", $"{this.sequence.Schema}.{this.sequence.Name}"),
                new SqlParameter("@range_size", this.blockSize),
                new SqlParameter("@range_first_value", SqlDbType.Variant)
                {
                    Direction = ParameterDirection.Output
                },
            };

    private long GetRangeFirstValue(object[] parameters)
        => (long)Convert.ChangeType(((SqlParameter)parameters[2]).Value, typeof(long));

    public void Dispose()
        => this.generatorState.Dispose();
}
