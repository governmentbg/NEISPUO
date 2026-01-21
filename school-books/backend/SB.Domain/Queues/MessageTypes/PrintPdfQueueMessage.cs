namespace SB.Domain;

public record PrintPdfQueueMessage(
    PrintType PrintType,
    string PrintParamsStr,
    int PrintId,
    int HtmlBlobId
);
