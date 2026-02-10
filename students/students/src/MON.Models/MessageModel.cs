namespace MON.Models
{
    using System.Collections.Generic;

    public class MessageModel
    {
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public List<MessageAttachmentModel> MessageAttachment { get; set; }
    }
}
