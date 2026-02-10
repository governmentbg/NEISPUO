namespace SB.Domain;

public record PrintHtmlQueueMessage(
    PrintType PrintType,
    string PrintParamsStr,
    int PrintId
);
