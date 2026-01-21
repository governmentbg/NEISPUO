
namespace MON.Models
{
    using System;
    using System.Collections.Generic;

    public class MessageViewModel
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public string SenderName { get; set; }

        public int ReceiverId { get; set; }

        public string Content { get; set; }

        public string Subject { get; set; }

        public DateTime SendDate { get; set; }

        public bool IsRead { get; set; }

        public List<MessageAttachmentViewModel> Attachment { get; set; }
    }

    public class MessageAttachmentViewModel
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }
    }
}
