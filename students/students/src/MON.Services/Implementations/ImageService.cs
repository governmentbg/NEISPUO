namespace MON.Services.Implementations
{
    using DocumentFormat.OpenXml.Drawing.Charts;
    using MetadataExtractor;
    using MetadataExtractor.Formats.Exif;
    using Microsoft.Extensions.Logging;
    using MON.Models.Image;
    using MON.Services.Interfaces;
    using MON.Shared.Enums;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.ColorSpaces;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Bmp;
    using SixLabors.ImageSharp.Formats.Gif;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.Formats.Tga;
    using SixLabors.ImageSharp.Memory;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Processors.Quantization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageService : BaseService<ImageService>, IImageService
    {
        // Количество цветни пиксели от цялото изображение, така че да приемем че изображението е цветно
        private const double EPSILON_RATIO_COLOR_PIXELS = 0.005;
        public ImageService(DbServiceDependencies<ImageService> dependencies)
        : base(dependencies)
        {
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithModeratePooling();
        }

        public async Task<byte[]> Compress(byte[] imageBytes, ImageCompressionLevelEnum compressionLevel)
        {
            using Image image = LoadImage(imageBytes, out IImageFormat imageFormat);

            byte[] savedImageBytes = null;

            savedImageBytes = imageFormat.Name switch
            {
                "JPEG" => await SaveImageToByteArray(image, imageFormat, new JpegEncoder { Quality = compressionLevel == ImageCompressionLevelEnum.Low ? 90 : compressionLevel == ImageCompressionLevelEnum.Normal ? 75 : 45 }),
                "PNG" => await SaveImageToByteArray(image, imageFormat, new PngEncoder { CompressionLevel = compressionLevel == ImageCompressionLevelEnum.Low ? PngCompressionLevel.Level0 : compressionLevel == ImageCompressionLevelEnum.Normal ? PngCompressionLevel.DefaultCompression : PngCompressionLevel.BestCompression }),
                "GIF" => await SaveImageToByteArray(image, imageFormat, new GifEncoder { GlobalPixelSamplingStrategy = new DefaultPixelSamplingStrategy { }, Quantizer = KnownQuantizers.Octree }),
                "BMP" => await SaveImageToByteArray(image, imageFormat, new BmpEncoder { Quantizer = KnownQuantizers.Octree, SupportTransparency = false }),
                "TGA" => await SaveImageToByteArray(image, imageFormat, new TgaEncoder { Compression = compressionLevel == ImageCompressionLevelEnum.Low ? TgaCompression.None : TgaCompression.RunLength }),
                _ => throw new ArgumentException("Unsupported image type"),
            };

            return savedImageBytes;
        }

        public async Task<byte[]> Rotate(byte[] imageBytes, int angle)
        {
            using Image image = LoadImage(imageBytes, out IImageFormat imageFormat);
            image.Mutate(x => x.Rotate(angle));
            var savedImageBytes = await SaveImageToByteArray(image, imageFormat);
            return savedImageBytes;
        }

        public async Task<byte[]> Segment(byte[] imageBytes, ImageSegmentEnum segment)
        {
            using Image image = LoadImage(imageBytes, out IImageFormat imageFormat);
            int x = 0;
            int y = 0;
            int newWidth = 0;
            int newHeight = 0;
            switch (segment)
            {
                case ImageSegmentEnum.All: return imageBytes;
                case ImageSegmentEnum.LowerLeft: x = 0; y = 0; newHeight = image.Height / 2; newWidth = image.Width / 2; break;
                case ImageSegmentEnum.LowerLeftEight: x = 0; y = 0; newHeight = image.Height / 4; newWidth = image.Width / 4; break;
                case ImageSegmentEnum.UpperLeft: x = 0; y = 0; newHeight = image.Height / 2; newWidth = image.Width / 2; break;
                case ImageSegmentEnum.UpperRight: x = image.Width / 2; y = 0; newHeight = image.Height / 2; newWidth = image.Width - x; break;
                case ImageSegmentEnum.LowerRight: x = image.Width / 2; y = image.Height / 2; newHeight = image.Height - y; newWidth = image.Width - x; break;
            }

            image.Mutate(i => i.Crop(new Rectangle(x, y, newWidth, newHeight)));


            var savedImageBytes = await SaveImageToByteArray(image, imageFormat);
            return savedImageBytes;
        }

        public async Task<byte[]> Resize(byte[] imageBytes, ushort width, ushort height)
        {
            using Image image = LoadImage(imageBytes, out IImageFormat imageFormat);

            if (image.Height > image.Width)
            {
                image.Mutate(x => x.Resize(0, height));
            }
            else
            {
                image.Mutate(x => x.Resize(width, 0));
            }

            var savedImageBytes = await SaveImageToByteArray(image, imageFormat);
            return savedImageBytes;
        }


        private async Task<byte[]> SaveImageToByteArray(Image image, IImageFormat imageFormat, IImageEncoder encoder = null)
        {
            using var stream = new MemoryStream();

            switch (imageFormat.Name)
            {
                case "JPEG":
                    if (encoder == null)
                        await image.SaveAsJpegAsync(stream);
                    else
                        await image.SaveAsJpegAsync(stream, (JpegEncoder)encoder);
                    break;
                case "PNG":
                    if (encoder == null)
                        await image.SaveAsPngAsync(stream);
                    else
                        await image.SaveAsPngAsync(stream, (PngEncoder)encoder);
                    break;
                case "GIF":
                    if (encoder == null)
                        await image.SaveAsGifAsync(stream);
                    else
                        await image.SaveAsGifAsync(stream, (GifEncoder)encoder);
                    break;
                case "BMP":
                    if (encoder == null)
                        await image.SaveAsBmpAsync(stream);
                    else
                        await image.SaveAsBmpAsync(stream, (BmpEncoder)encoder);
                    break;
                case "TGA":
                    if (encoder == null)
                        await image.SaveAsTgaAsync(stream);
                    else
                        await image.SaveAsTgaAsync(stream, (TgaEncoder)encoder);
                    break;
                default: throw new ArgumentException("Unsupported image type");
            }

            stream.Position = 0;
            return stream.ToArray();
        }

        public Image<Rgba32> LoadRGBImage(byte[] imageBytes)
        {
            try
            {
                return Image.Load<Rgba32>(imageBytes);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Image cannot be loaded. Available decoders"))
                {
                    throw new Exception("Unsupported image type");
                }

                throw;
            }
        }

        private bool IsRed(Rgba32 pixel)
        {
            Rgba32 min = new Rgba32(150, 0, 0);
            Rgba32 max = new Rgba32(255, 200, 200);
            return pixel.R >= min.R && pixel.R <= max.R
                && pixel.G >= min.G && pixel.G <= max.G
                && pixel.B >= min.B && pixel.B <= max.B
                && pixel.R >= pixel.G && pixel.R >= pixel.B;
        }

        public ExifInformation ExtractExifInformation(byte[] imageBytes)
        {
            ExifInformation exifInfo = new ExifInformation();
            try
            {
                using (var ms = new MemoryStream(imageBytes))
                {
                    IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(ms);
                    // Look for the Exif Ifd0 directory (or ExifSubIfdDirectory for more detailed info)
                    var exifDirectory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();

                    if (exifDirectory != null)
                    {
                        // The Orientation tag ID is ExifDirectoryBase.TagOrientation (0x0112)
                        if (exifDirectory.ContainsTag(ExifDirectoryBase.TagOrientation))
                        {
                            // Get the raw integer value of the orientation tag
                            int orientationValue = exifDirectory.GetInt32(ExifDirectoryBase.TagOrientation);

                            // MetadataExtractor also provides a convenient description
                            string orientationDescription = exifDirectory.GetDescription(ExifDirectoryBase.TagOrientation);

                            exifInfo.OrientationValue = orientationValue;
                            exifInfo.OrientationDescription = orientationDescription;
                        }
                        else
                        {
                            exifInfo.OrientationValue = (int)ExifOrientationEnum.NotAvailable; // Default value if no orientation tag is present
                            exifInfo.OrientationDescription = "No Orientation Tag";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                exifInfo.OrientationValue = (int)ExifOrientationEnum.NotAvailable; // Default value if no orientation tag is present
                exifInfo.OrientationDescription = "No Orientation Tag extracted";
            }

            return exifInfo;
        }

        public ImageDetailsModel AnalyseImage(Image<Rgba32> image, ExifInformation exifInfo = null)
        {
            ImageDetailsModel details = new ImageDetailsModel();
            details.Height = image.Height;
            details.Width = image.Width;
            details.PageOrientation = (details.Height > details.Width) ? PageOrientationEnum.Portrait : PageOrientationEnum.Landscape;
            if (exifInfo != null && exifInfo.OrientationValue != (int)ExifOrientationEnum.NotAvailable)
            {
                switch (exifInfo.OrientationValue)
                {
                    case (int)ExifOrientationEnum.LeftTop:
                    case (int)ExifOrientationEnum.RightTop:
                    case (int)ExifOrientationEnum.LeftBottom:
                    case (int)ExifOrientationEnum.RightBottom:
                        // Променяме ориентацията
                        if (details.PageOrientation == PageOrientationEnum.Portrait)
                        {
                            details.PageOrientation = PageOrientationEnum.Landscape;
                        }
                        else
                        {
                            details.PageOrientation = PageOrientationEnum.Portrait;
                        }
                        break;
                }
            }
            int pixelsCount = image.Height * image.Width;
            int coloredPixels = 0;

            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    var pixel = image[w, h];
                    if (pixel.R == pixel.G && pixel.G == pixel.B)
                    {
                        // grayscale пиксел
                    }
                    else
                    {
                        // color пиксел
                        coloredPixels++;
                    }
                }
                if (((double)coloredPixels / (double)pixelsCount) > EPSILON_RATIO_COLOR_PIXELS) break;
            }

            details.HasColor = (((double)coloredPixels / (double)pixelsCount)) > EPSILON_RATIO_COLOR_PIXELS;


            return details;
        }

        public string ExtractFactoryNumber(Image<Rgba32> image)
        {
            Image<Rgba32> factoryNumberImage = new Image<Rgba32>(image.Width, image.Height);

            for (int h = 0; h < image.Height; h++)
            {
                for (int w = 0; w < image.Width; w++)
                {
                    var pixel = image[w, h];

                    if (IsRed(pixel))
                    {
                        factoryNumberImage[w, h] = pixel;
                    }
                    else
                    {
                        factoryNumberImage[w, h] = Color.White;
                    }
                }
            }


            return String.Empty;
        }

        public Image LoadImage(byte[] imageBytes, out IImageFormat imageFormat)
        {
            try
            {
                return Image.Load(imageBytes, out imageFormat);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Image cannot be loaded. Available decoders"))
                {
                    throw new Exception("Unsupported image type");
                }

                throw;
            }
        }
    }
}
