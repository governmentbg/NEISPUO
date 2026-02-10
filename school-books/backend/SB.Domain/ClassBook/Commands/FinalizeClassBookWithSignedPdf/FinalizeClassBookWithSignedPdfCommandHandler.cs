namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal partial record FinalizeClassBookWithSignedPdfCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore,
    BlobServiceClient BlobServiceClient,
    ILogger<FinalizeClassBookWithSignedPdfCommandHandler> Logger,
    IOptions<DomainOptions> DomainOptions)
    : IRequestHandler<FinalizeClassBookWithSignedPdfCommand, int>
{
    [GeneratedRegex("(?:^|_)cb(\\d+)(?:_|\\.|\\s)", RegexOptions.IgnoreCase)]
    private static partial Regex ClassBookIdRegex();

    private const int BufferSize = 64 * 1024;

    public async Task<int> Handle(FinalizeClassBookWithSignedPdfCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId;
        int blobId;
        List<(string issuer,
            string subject,
            string thumbprint,
            DateTime validFrom,
            DateTime validTo)> signatures = new();

        string? tempFileName = null;
        try
        {
            tempFileName = Path.GetTempFileName();

            // Copy the SignedClassBookPrintFile to a temporary file
            // so that we dont have to keep the whole file in memory.
            // Not using the "using declarations" here because
            // we need to close the file after copying to it
            // to be able to open it for reading with iTextSharp.
            using (FileStream tempFile = new(
                tempFileName,
                FileMode.Truncate,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                useAsync: true))
            {
                await command.SignedClassBookPrintFile!.CopyToAsync(
                    tempFile,
                    BufferSize,
                    ct);
            }

            if (!PdfUtils.CheckPdf(tempFileName))
            {
                throw new DomainValidationException(
                    new [] { "invalid_pdf" },
                    new [] { $"Прикаченият файл не е валиден PDF документ." });
            }

            string pdfFileName = command.SignedClassBookPrintFileName!;
            if (command.ExtractClassBookIdFromMetadataOrFileName == true)
            {
                if (PdfUtils.GetPdfXmpLabelMetadata(tempFileName) is string { Length: > 0 } xmpLabel &&
                    int.TryParse(xmpLabel, out int metadataClassBookId))
                {
                    classBookId = metadataClassBookId;
                }
                else if (ClassBookIdRegex().Match(pdfFileName) is Match { Success: true } match &&
                    int.TryParse(match.Groups[1].Value, out int fileNameClassBookId))
                {
                    classBookId = fileNameClassBookId;
                }
                else
                {
                    throw new DomainValidationException(
                        new [] { "class_book_id_not_found" },
                        new [] { "Не може да бъде намерен идентификатор на дневник в метаданните или името на файла." });
                }
            }
            else
            {
                classBookId = command.ClassBookId!.Value;
            }

            // TODO dotnet8: use type aliases
            IEnumerable<(
                byte[] signingCertificate,
                bool coversDocument,
                bool isTimestamp,
                DateTime signDate,
                string signedRevisionContentHash)> extractedSignatures;
            try
            {
                extractedSignatures = PdfUtils.ExtractPdfSignatures(tempFileName);
            }
            catch (ArgumentException ex) when (ex.Message == "can't decode PKCS7SignedData object")
            {
                throw new DomainValidationException(
                    new [] { "malformed_signature" },
                    new [] { $"Подписът върху документа не може да бъде разчетен." });
            }

            foreach (
                (byte[] signingCertificate,
                bool coversDocument,
                bool isTimestamp,
                DateTime signDate,
                string signedRevisionContentHash) in extractedSignatures)
            {
                (bool validAtTimeOfSigning,
                string? validationFailureChainStatus,
                string issuer,
                string subject,
                string thumbprint,
                DateTime validFrom,
                DateTime validTo) = PdfUtils.VerifyCertificate(signingCertificate, signDate);

                if (thumbprint != this.DomainOptions.Value.TestPdfSigningCertificateThumbprint
                    && !validAtTimeOfSigning)
                {
                    if (!string.IsNullOrWhiteSpace(validationFailureChainStatus))
                    {
                        this.Logger.LogWarning(
                            "Certificate verification failed for cert with thumbprint {thumb}.\nChain status:\n{chain}",
                            thumbprint,
                            validationFailureChainStatus);
                    }

                    throw new DomainValidationException(
                        new [] { "signature_validation_failed" },
                        new [] { $"Подписът върху документа не е от валиден КЕП." });
                }

                signatures.Add(
                    (issuer,
                    subject,
                    thumbprint,
                    validFrom,
                    validTo));
            }

            if (signatures.Count == 0)
            {
                throw new DomainValidationException(
                    new [] { "missing_required_signature" },
                    new [] { "Документът не е подписан." });
            }

            using (FileStream tempFile = new(
                tempFileName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None,
                BufferSize,
                useAsync: true))
            {
                var uploadedBlob = await this.BlobServiceClient.UploadBlobAsync(
                    tempFile,
                    pdfFileName,
                    ct);

                blobId = uploadedBlob.BlobId;
            }
        }
        finally
        {
            if (tempFileName != null)
            {
                File.Delete(tempFileName);
            }
        }

        var classBook = await this.ClassBookAggregateRepository.FindOrDefaultAsync(
            schoolYear,
            classBookId,
            ct);

        if (classBook == null || classBook.InstId != command.InstId!.Value)
        {
            throw new DomainValidationException(
                new [] { "class_book_not_found" },
                new [] { $"Дневникът с идентификатор {classBookId} не е намерен." });
        }

        if (classBook.IsFinalized)
        {
            throw new DomainValidationException(
                new [] { "class_book_already_finalized" },
                new [] { $"Дневникът '{classBook.FullBookName}' вече е приключен." });
        }

        var classBookPrint = classBook.FinalizeWithSignedPdf(
            blobId,
            signatures.ToArray(),
            command.SysUserId!.Value);
        await this.ClassBookAggregateRepository.AddAsync(
            classBookPrint,
            ct);

        await this.UnitOfWork.SaveAsync(ct);

        await this.ClassBookCachedQueryStore.ClearClassBookAsync(
            schoolYear,
            classBookId,
            // no cancellation
            default(CancellationToken));

        return classBookId;
    }
}
