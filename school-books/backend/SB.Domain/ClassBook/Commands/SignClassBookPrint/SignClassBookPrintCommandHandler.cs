namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal record SignClassBookPrintCommandHandler(
    IUnitOfWork UnitOfWork,
    IClassBooksAggregateRepository ClassBookAggregateRepository,
    IQueueMessagesService QueueMessagesService,
    IClassBookCachedQueryStore ClassBookCachedQueryStore,
    IClassBooksQueryRepository ClassBooksQueryRepository,
    BlobServiceClient BlobServiceClient,
    ILogger<SignClassBookPrintCommandHandler> Logger,
    IOptions<DomainOptions> DomainOptions)
    : IRequestHandler<SignClassBookPrintCommand>
{
    private const int BufferSize = 64 * 1024;

    public async Task Handle(SignClassBookPrintCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;
        int classBookPrintId = command.ClassBookPrintId!.Value;

        string? tempFileName = null;
        int blobId;
        List<(string issuer,
            string subject,
            string thumbprint,
            DateTime validFrom,
            DateTime validTo)> signatures = new();
        try
        {
            tempFileName = Path.GetTempFileName();

            // Copy the SignedClassBookPrintFileBase64 to a temporary file
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
                using FromBase64Transform base64Transform = new();
                using CryptoStream cryptoStream = new(tempFile, base64Transform, CryptoStreamMode.Write);

                await command.SignedClassBookPrintFileBase64!.CopyToAsync(
                    cryptoStream,
                    BufferSize,
                    ct);
            }

            string? contentHash =
                await this.ClassBooksQueryRepository.GetClassBookPrintContentHashAsync(
                    schoolYear,
                    classBookId,
                    classBookPrintId,
                    ct);

            if (contentHash is null)
            {
                throw new DomainValidationException("ClassBookPrint should have a non-null content hash.");
            }

            foreach (
                (byte[] signingCertificate,
                bool coversDocument,
                bool isTimestamp,
                DateTime signDate,
                string signedRevisionContentHash) in PdfUtils.ExtractPdfSignatures(tempFileName))
            {
                if (signedRevisionContentHash != contentHash)
                {
                    throw new DomainValidationException("The provided signed pdf is different from the one in the ClassBookPrint.");
                }

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
                        new [] { $"Подписа върху документа не е от валиден КЕП." });
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
                    "print_signed.pdf",
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

        var classBook = await this.ClassBookAggregateRepository.FindAsync(
            schoolYear,
            classBookId,
            ct);

        classBook.SignClassBookPrint(
            classBookPrintId,
            blobId,
            signatures.ToArray(),
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        await this.ClassBookCachedQueryStore.ClearClassBookAsync(
            schoolYear,
            classBookId,
            // no cancellation
            default(CancellationToken));
    }
}
