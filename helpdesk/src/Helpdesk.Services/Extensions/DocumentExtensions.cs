namespace Helpdesk.Services.Extensions
{
    using Helpdesk.DataAccess;
    using Helpdesk.Models;
    using Helpdesk.Models.Configuration;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class DocumentExtensions
    {
        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

        public static bool HasToAdd(this DocumentModel model)
        {
            return model != null && !model.Id.HasValue
                && model.NoteContents != null && model.NoteContents.Length > 0;
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
