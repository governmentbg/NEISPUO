namespace MON.Services.Interfaces
{
    using System.Collections.Generic;
    using System.DrawingCore.Imaging;
    using System.Threading.Tasks;
    using ZXing;

    public interface IBarcodeService
    {
        (Result ressult, System.DrawingCore.Bitmap bitmap) Decode(byte[] contents, List<BarcodeFormat> vectors = null);
        Result Decode(System.DrawingCore.Bitmap bitmap, List<BarcodeFormat> vectors = null);
        Task<Result> DecodeTryHarderAsync(byte[] imageContents, List<BarcodeFormat> vectors = null, int minBarcodeLength = 4);
        byte[] Encode(string text, BarcodeFormat barcodeFormat, ImageFormat imageFormat, int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppRgb);
    }
}
