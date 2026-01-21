namespace RegStamps.Infrastructure.Extensions
{
    public static class ContentTypeExtensions
    {
        public static string ToContentType(this string fileName)
            => fileName.Substring(fileName.LastIndexOf('.')).ToLower() switch
            {
                ".pdf"            =>   "application/pdf",
                ".png"            =>   "image/png",
                ".jpeg" or ".jpg" =>   "image/jpeg",
                _                 =>   string.Empty
            };
    }
}
