namespace SB.JobHost;

using SB.Domain;
using Microsoft.Extensions.Options;
using CliWrap;
using System.Text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using System.IO.Compression;
using System.IO.Pipelines;
using SB.Common;
using Autofac;

public class PrintPdfJob : QueueJob<PrintPdfQueueMessage>
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public PrintPdfJob(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<PrintPdfJob> logger,
        IOptions<JobHostOptions> optionsAccessor)
        : base(
            QueueMessageType.PrintPdf,
            serviceScopeFactory,
            logger,
            optionsAccessor.Value.PrintPdfJobOptions)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<(QueueJobProcessingResult result, string? error)> HandleMessageAsync(
        PrintPdfQueueMessage payload,
        CancellationToken ct)
    {
        PrintType printType = payload.PrintType;
        int printId = payload.PrintId;
        int htmlBlobId = payload.HtmlBlobId;

        string htmlFile = GetTempFileName(".html");
        string pdfFile = GetTempFileName(".pdf");

        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var classBookPrintPdfJobRepository = scope.ServiceProvider.GetRequiredService<IClassBookPrintPdfJobRepository>();
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var printService = scope.ServiceProvider.GetRequiredService<ILifetimeScope>()
            .ResolveKeyed<IPrintService>(printType);

        try
        {
            Pipe pipe = new();

            static async Task WriteToFileAsync(
                Pipe pipe,
                string file,
                CancellationToken ct)
            {
                await using FileStream fs = new(
                    file,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    useAsync: true);
                await using GZipStream gz = new(pipe.Reader.AsStream(), CompressionMode.Decompress);

                await gz.CopyToAsync(fs, ct);
            }

            static async Task DownloadBlobAsync(
                Pipe pipe,
                BlobServiceClient blobServiceClient,
                int blobId,
                CancellationToken ct)
            {
                // Disposing the writer stream is mandatory
                // otherwise the reader will never complete
                // and the WriteToFileAsync method will wait forever
                await using Stream stream = pipe.Writer.AsStream();

                await blobServiceClient.DownloadBlobToStreamAsync(
                    blobId,
                    stream,
                    ct);
            }

            await Task.WhenAll(
                WriteToFileAsync(pipe, htmlFile, ct),
                DownloadBlobAsync(pipe, blobServiceClient, htmlBlobId, ct));

            await this.TransformHtmlToPdf(htmlFile, pdfFile, ct);

            string? contentHash = null;

            if (payload.PrintType == PrintType.ClassBook &&
                await classBookPrintPdfJobRepository.IsFinalClassBookPrintAsync(
                payload.PrintParamsStr,
                printId,
                ct))
            {
                using PdfReader pdfReader = new(pdfFile);
                contentHash = Convert.ToHexString(pdfReader.GetPdfContentSha1Hash());
            }

            await using FileStream pdfStream = new(
                pdfFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);
            int blobId = await this.UploadBlobAsync(
                pdfStream,
                "print.pdf",
                ct);

            await printService.FinalizePrintAsProcessedAsync(payload.PrintParamsStr, printId, blobId, contentHash, ct);

            try
            {
                // the finalization was successful, delete the html blob, no cancellation here
                await blobServiceClient.DeleteBlobAsync(htmlBlobId, default);
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "Failed to delete blob {htmlBlobId}", htmlBlobId);
            }

            return (result: QueueJobProcessingResult.Success, error: null);
        }
        catch when (ct.IsCancellationRequested)
        {
            return (result: QueueJobProcessingResult.RetryError, error: "Cancelled");
        }
        catch (SqlException ex) when (ex.Number == SqlServerErrorCodes.Timeout)
        {
            // timeout from a query
            return (result: QueueJobProcessingResult.RetryError, error: "Timeout");
        }
        catch (DomainUpdateSqlException ex) when (ex.SqlException.Number == SqlServerErrorCodes.Timeout)
        {
            // timeout from unitOfWork.SaveAsync
            return (result: QueueJobProcessingResult.RetryError, error: "Timeout");
        }
        catch
        {
            // no cancellation here,
            // lets try to finish or the ClassBookPrint will end up in a Pending state forever

            await printService.FinalizePrintAsErroredAsync(payload.PrintParamsStr, printId, default);

            // rethrow the exception so that we have the QueueMessage in Errored state with the exception stored
            throw;
        }
        finally
        {
            if (File.Exists(htmlFile))
            {
                File.Delete(htmlFile);
            }

            if (File.Exists(pdfFile))
            {
                File.Delete(pdfFile);
            }
        }
    }

    private async Task<int> UploadBlobAsync(
        Stream blobStream,
        string fileName,
        CancellationToken ct)
    {
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();

        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var blob = await blobServiceClient.UploadBlobAsync(
            blobStream,
            fileName,
            ct);

        return blob.BlobId;
    }

    private async Task TransformHtmlToPdf(string? inputFile, string? outputFile, CancellationToken ct)
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();
        var result = await Cli.Wrap("wkhtmltopdf")
            .WithValidation(CommandResultValidation.None)
            .WithArguments($"--log-level warn -L 0 -R 0 -B 0 -T 0 -s A4 --disable-smart-shrinking {inputFile} {outputFile}")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync(ct);

        if (result.ExitCode != 0)
        {
            // On macOS/Linux, when CTRL+C is pressed, a SIGINT signal is sent to the entire process group.
            // Not sure if this applies to the kubernetes SIGTERM as well but it is safe to wait 100ms
            // an see if the cancellation token is cancelled before returning an error (the exit code of a cancelled process is not 0)
            await Task.Delay(100, ct);
            if (!ct.IsCancellationRequested)
            {
                throw new Exception($"wkhtmltopdf failed\n{stdOutBuffer}\n{stdErrBuffer}");
            }
        }
        else if (stdErrBuffer.Length > 0)
        {
            this.Logger.LogWarning("wkhtmltopdf warnings\n{stdErrBuffer}", stdErrBuffer);
        }
    }

    private static string GetTempFileName(string extension)
    {
        return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extension);
    }
}
