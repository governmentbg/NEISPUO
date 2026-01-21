namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ITopicPlanItemsExcelReaderWriter;

class TopicPlanItemExcelService : ITopicPlanItemExcelService
{
    private IUnitOfWork unitOfWork;
    private ITopicPlansQueryRepository topicPlansQueryRepository;
    private IAggregateRepository<TopicPlanItem> topicPlanItemsAggregateRepository;
    private BlobServiceClient blobServiceClient;
    private ITopicPlanItemsExcelReaderWriter topicPlanItemsExcelReaderWriter;
    public TopicPlanItemExcelService(
        IUnitOfWork unitOfWork,
        ITopicPlansQueryRepository topicPlansQueryRepository,
        IAggregateRepository<TopicPlanItem> topicPlanItemsAggregateRepository,
        BlobServiceClient blobServiceClient,
        ITopicPlanItemsExcelReaderWriter topicPlanItemsExcelReaderWriter)
    {
        this.unitOfWork = unitOfWork;
        this.topicPlansQueryRepository = topicPlansQueryRepository;
        this.topicPlanItemsAggregateRepository = topicPlanItemsAggregateRepository;
        this.blobServiceClient = blobServiceClient;
        this.topicPlanItemsExcelReaderWriter = topicPlanItemsExcelReaderWriter;
    }

    public async Task ExportAsync(int sysUserId, int topicPlanId, Stream outputStream, CancellationToken ct)
    {
        var topicPlanItems = await this.topicPlansQueryRepository.GetExcelDataAsync(sysUserId, topicPlanId, ct);
        this.topicPlanItemsExcelReaderWriter.Write(
            topicPlanItems.Select(tpi => new TopicPlanItemDO(tpi.Number, tpi.Title, tpi.Note)).ToArray(),
            outputStream);
    }

    public async Task<string[]> ImportFromExcelBlobAsync(int blobId, int topicPlanId, CancellationToken ct)
    {
        using var ms = new MemoryStream();

        await this.blobServiceClient.DownloadBlobToStreamAsync(blobId, ms, ct);

        var errors = this.topicPlanItemsExcelReaderWriter.TryRead(ms, out TopicPlanItemDO[] items);

        if (!errors.Any())
        {
            foreach (var item in items)
            {
                await this.topicPlanItemsAggregateRepository.AddAsync(
                    new TopicPlanItem(
                        topicPlanId,
                        item.Number,
                        item.Title,
                        item.Note),
                    ct);
            }

            await this.unitOfWork.SaveAsync(ct);
        }

        return errors;
    }
}
