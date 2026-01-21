using System.Xml;

namespace Kontrax.RegiX.Standard.Client
{
    public class RawMessage
    {
        public UniqueId MessageId { get; }
        public UniqueId RelatesToMessageId { get; }
        public string MessageContent { get; }

        public RawMessage()
        {
        }

        public RawMessage(UniqueId id, UniqueId relatesTo, string content)
        {
            MessageId = id;
            RelatesToMessageId = relatesTo;
            MessageContent = content;
        }
    }
}
