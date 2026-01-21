namespace Helpdesk.Models
{
    public class DocumentModel
    {
        public int? Id { get; set; }

        public int? BlobId { get; set; }

        public string NoteFileName { get; set; }

        public string NoteFileType { get; set; }

        public byte[] NoteContents { get; set; }

        public string Description { get; set; }

        public bool? Deleted { get; set; }
    }

    public class DocumentViewModel : DocumentModel, IBlobDownloadable
    {
        public long UnixTimeSeconds { get; set; }

        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }

        public string BlobServiceUrl { get; set; }
    }
}
