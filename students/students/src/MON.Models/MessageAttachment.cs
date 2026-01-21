namespace MON.Models
{
    public class MessageAttachmentModel
    {
        public string FileName { get; set; }

        public int FileSize { get; set; }

        public byte[] NoteContents { get; set; }

        public int BlodId { get; set; }

        public string ContentType { get; set; }
    }
}