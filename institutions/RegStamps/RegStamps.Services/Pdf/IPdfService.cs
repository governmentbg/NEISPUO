namespace RegStamps.Services.Pdf
{
    public interface IPdfService
    {
        byte[] CreatePdfFromHtml(string html, bool isLandscape = false, string headerHtml = null, string footerHtml = null);
    }
}
