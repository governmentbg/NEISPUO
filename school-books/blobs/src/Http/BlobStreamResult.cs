namespace SB.Blobs;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

public class BlobStreamResult : ActionResult
{
    public BlobStreamResult(int blobId)
    {
        this.BlobId = blobId;
    }

    public int BlobId { get; init; }

    public override Task ExecuteResultAsync(ActionContext context)
    {
        context = context ?? throw new ArgumentNullException(nameof(context));

        var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<BlobStreamResult>>();
        return executor.ExecuteAsync(context, this);
    }
}
