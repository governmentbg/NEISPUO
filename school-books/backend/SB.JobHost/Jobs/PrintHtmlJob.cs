namespace SB.JobHost;

using SB.Domain;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Autofac;
using Microsoft.IO;
using System.Text;
using System.IO.Compression;
using static SB.Domain.BlobServiceClient;
using SB.Common;

public class PrintHtmlJob : QueueJob<PrintHtmlQueueMessage>
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public PrintHtmlJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PrintHtmlJob> logger,
        IOptions<JobHostOptions> optionsAccessor)
        : base(
            QueueMessageType.PrintHtml,
            serviceScopeFactory,
            logger,
            optionsAccessor.Value.PrintHtmlJobOptions)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<(QueueJobProcessingResult result, string? error)> HandleMessageAsync(
        PrintHtmlQueueMessage payload,
        CancellationToken ct)
    {
        PrintType printType = payload.PrintType;
        int printId = payload.PrintId;

        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var lifetimeScope = scope.ServiceProvider.GetRequiredService<ILifetimeScope>();
        var printService = lifetimeScope.ResolveKeyed<IPrintService>(printType);
        var queueMessagesService = scope.ServiceProvider.GetRequiredService<IQueueMessagesService>();
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var memoryStreamManager = lifetimeScope
            .ResolveNamed<RecyclableMemoryStreamManager>(JobHostModule.ClassBookPrintHtmlJobRMSM);

        try
        {
            using MemoryStream ms = memoryStreamManager.GetStream();
            using GZipStream gz = new(ms, CompressionLevel.Optimal, leaveOpen: true);
            using StreamWriter sw = new(gz, Encoding.UTF8, 1024);

            await this.RenderHtmlAsync(
                printService,
                payload.PrintParamsStr,
                sw,
                ct);
            await sw.DisposeAsync();
            ms.Position = 0;

            UploadBlobDO htmlBlob = await blobServiceClient.UploadBlobAsync(
                ms,
                $"PrintHtml-{printType}-{printId}.html.gz",
                ct);

            await this.PostMessageAndSaveAsync(
                queueMessagesService,
                new PrintPdfQueueMessage(
                    payload.PrintType,
                    payload.PrintParamsStr,
                    printId,
                    htmlBlob.BlobId),
                ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not QueueJobRetryErrorException)
        {
            // no cancellation here,
            // lets try to finish or the ClassBookPrint will end up in a Pending state forever

            await printService.FinalizePrintAsErroredAsync(payload.PrintParamsStr, printId, default);

            // rethrow the exception so that we have the QueueMessage in Errored state with the exception stored
            throw;
        }

        return (result: QueueJobProcessingResult.Success, error: null);
    }

    private async Task RenderHtmlAsync(
        IPrintService printHtmlService,
        string printParams,
        TextWriter textWriter,
        CancellationToken ct)
    {
        try
        {
            await printHtmlService.RenderHtmlAsync(printParams, textWriter, ct);
        }
        catch when (ct.IsCancellationRequested)
        {
            throw new QueueJobRetryErrorException("Cancelled");
        }
        catch (SqlException ex) when (ex.Number == SqlServerErrorCodes.Timeout)
        {
            // timeout from a query
            throw new QueueJobRetryErrorException("Timeout");
        }
    }

    private async Task PostMessageAndSaveAsync(
        IQueueMessagesService queueMessagesService,
        PrintPdfQueueMessage payload,
        CancellationToken ct)
    {
        try
        {
            await queueMessagesService.PostMessagesAndSaveAsync(
                new [] { payload },
                null,
                null,
                null,
                ct);
        }
        catch when (ct.IsCancellationRequested)
        {
            throw new QueueJobRetryErrorException("Cancelled");
        }
        catch (SqlException ex) when (ex.Number == SqlServerErrorCodes.Timeout)
        {
            // timeout from a query
            throw new QueueJobRetryErrorException("Timeout");
        }
    }
}
