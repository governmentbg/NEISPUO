using MON.DataAccess;
using MON.Models;
using MON.Models.ASP;
using MON.Models.Configuration;
using MON.Models.Diploma;
using MON.Models.HealthInsurance;
using MON.Shared.Extensions;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace MON.Services
{

    public static class DocumentExtensions
    {
        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

        public static DocumentViewModel ToViewModel(this Document doc, BlobServiceConfig blobServiceConfig)
        {
            if (doc == null || blobServiceConfig == null) return null;

            DocumentViewModel viewModel = new DocumentViewModel
            {
                Id = doc.Id,
                BlobId = doc.BlobId,
                NoteFileName = doc.FileName,
                NoteFileType = doc.ContentType,
                Description = doc.Description
            };

            if (viewModel.BlobId.HasValue)
            {
                CalcHmac(viewModel, blobServiceConfig);
            }

            return viewModel;
        }

        public static DiplomaDocumentModel ToDocumentModel(this DiplomaDocument doc, BlobServiceConfig blobServiceConfig)
        {
            if (doc == null || blobServiceConfig == null) return null;

            DiplomaDocumentModel viewModel = new DiplomaDocumentModel
            {
                Id = doc.Id,
                BlobId = doc.BlobId,
                Description = doc.Description,
                DiplomaId = doc.DiplomaId,
            };

            if (viewModel.BlobId.HasValue)
            {
                CalcHmac(viewModel, blobServiceConfig);
            }

            return viewModel;
        }

        public static Document ToDocument(this DocumentModel model, int? blobId)
        {
            return model == null
                ? null
                : new Document
                {
                    ContentType = model.NoteFileType,
                    FileName = model.NoteFileName,
                    Description = model.Description,
                    BlobId = blobId
                };
        }

        public static ASPMonthlyBenefitsImportFileModel ToImportFileModel(this AspmonthlyBenefitsImport doc, BlobServiceConfig blobServiceConfig)
        {
            if (doc == null || blobServiceConfig == null) return null;

            var viewModel = new ASPMonthlyBenefitsImportFileModel
            {
                Id = doc.Id,
                CreatedDate = doc.CreateDate,
                Month = CultureInfo.GetCultureInfo("bg-BG").DateTimeFormat.GetMonthName(doc.Month),
                SchoolYear = doc.SchoolYear,
                RecordsCount = doc.RecordsCount,
                FromDate = doc.FromDate,
                ToDate = doc.ToDate,
                ImportCompleted = doc.ImportCompleted,
                ImportFileMessages = doc.ImportFileMessages,
                FileStatusCheck = ((ASPFileStatusCheckEnum)doc.FileStatusCheck).GetEnumDescription(),
                FileStatusCheckId = doc.FileStatusCheck,
                IsActive = ((doc.FromDate <= DateTime.Now.Date) || doc.FromDate == null) && ((DateTime.Now.Date <= doc.ToDate) || doc.ToDate == null),
                BlobId = doc.ImportedBlobId,
                ExportedFile = new ASPMonthlyBenefitsImportFileModel
                {
                    BlobId = doc.ExportedBlobId,
                }
            };

            if (viewModel.BlobId.HasValue)
            {
                CalcHmac(viewModel, blobServiceConfig);
            }

            if (viewModel.ExportedFile.BlobId.HasValue)
            {
                CalcHmac(viewModel.ExportedFile, blobServiceConfig);
            }

            return viewModel;
        }

        public static ASPEnrolledStudentsExportFileModel ToImportFileModel(this AspenrolledStudentsExport doc, BlobServiceConfig blobServiceConfig)
        {
            if (doc == null || blobServiceConfig == null) return null;

            ASPEnrolledStudentsExportFileModel viewModel = new ASPEnrolledStudentsExportFileModel
            {
                Id = doc.Id,
                CreatedDate = doc.CreateDate,
                Month = doc.Month,
                SchoolYear = doc.SchoolYear,
                RecordsCount = doc.RecordsCount,
                BlobId = doc.BlobId,
            };

            if (viewModel.BlobId.HasValue)
            {
                CalcHmac(viewModel, blobServiceConfig);
            }

            return viewModel;
        }

        public static bool HasToAdd(this DocumentModel model)
        {
            return model != null && !model.Id.HasValue
                && model.NoteContents != null && model.NoteContents.Length > 0;
        }

        public static void CalcHmac(IBlobDownloadable doc, BlobServiceConfig blobServiceConfig)
        {
            if (doc == null || blobServiceConfig == null) return;

            string HMACKey = blobServiceConfig.HMACKey; //"BGrFkf9yQ9JJoA47oNBE";

            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string message = $"{doc.BlobId}/{unixTimeSeconds}";

            using HMACSHA256 hash = new HMACSHA256(USAsciiStrict.GetBytes(HMACKey));
            byte[] hmac = hash.ComputeHash(USAsciiStrict.GetBytes(message));
            string urlSafeBase64HMAC =
                Convert.ToBase64String(hmac)
                // Url-safe Base64 / RFC 4648
                // https://tools.ietf.org/html/rfc4648
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            doc.UnixTimeSeconds = unixTimeSeconds;
            doc.Hmac = urlSafeBase64HMAC;
            doc.BlobServiceUrl = blobServiceConfig.Url; //"http://blobs.neispuo.mon.bg";
        }
    }
}
