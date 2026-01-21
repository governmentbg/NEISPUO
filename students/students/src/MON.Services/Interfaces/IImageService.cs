namespace MON.Services.Interfaces
{
    using MON.Models.Image;
    using MON.Shared.Enums;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.PixelFormats;
    using System.Threading.Tasks;

    public interface IImageService
    {
        Task<byte[]> Resize(byte[] imageBytes, ushort width, ushort height);
        Task<byte[]> Segment(byte[] imageBytes, ImageSegmentEnum segment);
        Task<byte[]> Compress(byte[] imageBytes, ImageCompressionLevelEnum compressionLevel);
        Image LoadImage(byte[] imageBytes, out IImageFormat imageFormat);
        Image<Rgba32> LoadRGBImage(byte[] imageBytes);
        ExifInformation ExtractExifInformation(byte[] imageBytes);
        Task<byte[]> Rotate(byte[] imageBytes, int angle);
        ImageDetailsModel AnalyseImage(Image<Rgba32> image, ExifInformation exifInfo = null);
        string ExtractFactoryNumber(Image<Rgba32> image);
    }
}
