namespace SB.Data;

using System;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal class ClassBookPrintPdfJobRepository : Repository, IClassBookPrintPdfJobRepository
{
    public ClassBookPrintPdfJobRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<bool> IsFinalClassBookPrintAsync(
        string printParamsStr,
        int classBookPrintId,
        CancellationToken ct)
    {
        var printParams = JsonSerializer.Deserialize<ClassBookPrintParams>(printParamsStr) ?? throw new Exception("Invalid class book print params!");
        return await this.DbContext.Set<ClassBookPrint>()
            .Where(cbp =>
                cbp.SchoolYear == printParams.SchoolYear &&
                cbp.ClassBookId == printParams.ClassBookId &&
                cbp.ClassBookPrintId == classBookPrintId)
            .Select(cbp => cbp.IsFinal)
            .SingleAsync(ct);
    }
}
