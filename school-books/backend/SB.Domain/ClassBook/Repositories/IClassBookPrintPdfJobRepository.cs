namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookPrintPdfJobRepository
{
    Task<bool> IsFinalClassBookPrintAsync(
        string printParamsStr,
        int classBookPrintId,
        CancellationToken ct);
}
