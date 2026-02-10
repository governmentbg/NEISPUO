namespace SB.Domain;

using System.IO;

public interface ITopicPlanItemsExcelReaderWriter
{
    record TopicPlanItemDO(
        int Number,
        string Title,
        string? Note);

    string[] TryRead(Stream inputStream, out TopicPlanItemDO[] items);

    void Write(TopicPlanItemDO[] items, Stream outputStream);
}
