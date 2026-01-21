namespace MON.Services.Implementations
{
    using MON.Services.Interfaces;
    using MON.Shared;
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.DrawingCore.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ZXing;
    using ZXing.Common;

    public class BarcodeService : IBarcodeService
    {
        protected IImageService _imageService;

        public BarcodeService(IImageService imageService)
        {
            _imageService = imageService;
        }

        public (Result ressult, System.DrawingCore.Bitmap bitmap) Decode(byte[] contents, List<BarcodeFormat> vectors = null)
        {
            using MemoryStream ms = new MemoryStream(contents);
            var bitMap = (System.DrawingCore.Bitmap)System.DrawingCore.Image.FromStream(ms);
            var result = Decode(bitMap, vectors);

            return (result, bitMap);
        }

        /// <summary>
        /// Завърта изображението и се пробва да го скалира
        /// </summary>
        /// <param name="imageContents"></param>
        /// <param name="minBarcodeLength">Минимален брой символи, които да търсим като дължина на баркод. По подразбиране поне 4</param>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public async Task<Result> DecodeTryHarderAsync(byte[] imageContents, List<BarcodeFormat> vectors = null, int minBarcodeLength = 4)
        {
            // Todo: _imageService.Resize да се забърза. Ако е възможно да приема Bitmap или Image, който сме създали още при първото извикване на Decode
            Result result = null;

            Dictionary<(ushort size, int rotation, ImageSegmentEnum segment), byte[]> sourceMatrix = new Dictionary<(ushort, int, ImageSegmentEnum), byte[]>
                        {
                            { (0, 0, ImageSegmentEnum.All), null }, // original, 0 rotate
                            { (0, 0, ImageSegmentEnum.LowerLeft), null }, // original, lower left, 0 rotate
                            { (0, 0, ImageSegmentEnum.LowerLeftEight), null }, // original, lower left eight, 0 rotate
                            { (0, 90, ImageSegmentEnum.All), null }, // original, 90 rotate
                            { (0, 180, ImageSegmentEnum.All), null }, // original, 180 rotate
                            { (0, 270, ImageSegmentEnum.All), null }, // original, 270 rotate

                            { (2000, 0, ImageSegmentEnum.All), null }, // 2000x2000, 0 rotate
                            { (2000, 0, ImageSegmentEnum.LowerLeft), null }, // 2000x2000, lower left, 0 rotate
                            { (2000, 90, ImageSegmentEnum.All), null }, // 2000x2000, 90 rotate
                            { (2000, 180, ImageSegmentEnum.All), null }, // 2000x2000, 180 rotate
                            { (2000, 270, ImageSegmentEnum.All), null }, // 2000x2000, 270 rotate

                            { (1000, 0, ImageSegmentEnum.All), null }, // 1000x1000, 0 rotate
                            { (1000, 0, ImageSegmentEnum.LowerLeft), null }, // 1000x1000, lower left, 0 rotate
                            { (1000, 90, ImageSegmentEnum.All), null }, // 1000x1000, 90 rotate
                            { (1000, 180, ImageSegmentEnum.All), null }, // 1000x1000, 180 rotate
                            { (1000, 270, ImageSegmentEnum.All), null }, // 1000x1000, 270 rotate
                        };

            if (result == null || result.Text.IsNullOrWhiteSpace())
            {
                var keys = sourceMatrix.Keys.ToList();
                foreach (var key in keys)
                {
                    if (sourceMatrix[key] == null)
                    {
                        (ushort size, int angle, ImageSegmentEnum segment) = key;
                        byte[] source = imageContents;

                        if (size != 0)
                        {
                            source = await _imageService.Resize(source, size, size);
                        }

                        if (segment != ImageSegmentEnum.All)
                        {
                            source = await _imageService.Segment(source, segment);
                        }

                        if (angle != 0)
                        {
                            source = await _imageService.Rotate(source, angle);
                        }

                        sourceMatrix[key] = source;
                    }

                    (Result resultNew, _) = Decode(sourceMatrix[key], vectors);

                    result = resultNew;
                    if (result != null && !result.Text.IsNullOrWhiteSpace() && result.Text.Length >= minBarcodeLength)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public Result Decode(System.DrawingCore.Bitmap bitMap, List<BarcodeFormat> vectors = null)
        {
            if (bitMap == null) throw new ArgumentNullException(nameof(bitMap));

            var reader = new ZXing.ZKWeb.BarcodeReader();
            var hints = new Dictionary<DecodeHintType, object>();
            var vector = vectors ?? new List<BarcodeFormat>()
                    {
                        BarcodeFormat.CODE_128,
                       //BarcodeFormat.UPC_A,
                       //BarcodeFormat.UPC_E,
                       //BarcodeFormat.EAN_13,
                       //BarcodeFormat.EAN_8,
                       //BarcodeFormat.RSS_14,
                       //BarcodeFormat.RSS_EXPANDED,
                       //BarcodeFormat.CODE_39,
                       // BarcodeFormat.CODE_93,
                       // BarcodeFormat.ITF,
                       // BarcodeFormat.QR_CODE,
                       // BarcodeFormat.DATA_MATRIX,
                       // BarcodeFormat.AZTEC,
                       // BarcodeFormat.PDF_417,
                       // BarcodeFormat.CODABAR,
                       // BarcodeFormat.MAXICODE,
                       // BarcodeFormat.IMB
                       };

            hints[DecodeHintType.POSSIBLE_FORMATS] = vector;
            reader.Options.PossibleFormats = vector;
            reader.Options.TryHarder = true;

            var result = reader.Decode(bitMap);

            return result;
        }

        public byte[] Encode(string contents, BarcodeFormat barcodeFormat, ImageFormat imageFormat, int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppRgb)
        {
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = barcodeFormat,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                }
            };

            var pixelData = barcodeWriter.Write(contents);
            using var bitmap = new System.DrawingCore.Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            using var ms = new MemoryStream();
            var bitmapData = bitmap.LockBits(new System.DrawingCore.Rectangle(0, 0, pixelData.Width, pixelData.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            if (PixelFormat.Format32bppRgb == pixelFormat)
            {
                bitmap.Save(ms, imageFormat);
            }
            else
            {
                var targetBitmap = bitmap.Clone(new System.DrawingCore.Rectangle(0, 0, bitmap.Width, bitmap.Height), pixelFormat);
                targetBitmap.Save(ms, imageFormat);
            }

            return ms.ToArray();
        }
    }
}
