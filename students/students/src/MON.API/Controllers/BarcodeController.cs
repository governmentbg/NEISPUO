namespace MON.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MON.Models;
    using MON.Services.Interfaces;
    using System;
    using System.DrawingCore.Imaging;
    using System.Text.Json;
    using System.Threading.Tasks;
    using ZXing;

    public class BarcodeController : BaseApiController
    {
        private readonly IBarcodeService _barcodeService;
        private readonly IAppConfigurationService _configurationService;

        public BarcodeController(IBarcodeService barcodeService, IAppConfigurationService configurationService)
        {
            _barcodeService = barcodeService;
            _configurationService = configurationService;
        }

        /// <summary>
        /// Генерира баркод като графичен файл по зададени параметри
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<FileContentResult> Encode(string text, string title)
        {
            var contents = await _configurationService.GetValueByKey("BarcodeGeneration");
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            BarcodeGenerationModel model = string.IsNullOrWhiteSpace(contents)
                ? null
                : JsonSerializer.Deserialize<BarcodeGenerationModel>(contents, options);

            var barcodeFormat = model.Format.ToLower() switch
            {
                "imb" => BarcodeFormat.IMB,
                "qr" => BarcodeFormat.QR_CODE,
                "code_128" => BarcodeFormat.CODE_128,
                "ean_13" => BarcodeFormat.EAN_13,
                _ => BarcodeFormat.IMB,
            };

            ImageFormat fileFormat = model.FileFormat.ToLower() switch
            {
                "png" => ImageFormat.Png,
                "jpeg" => ImageFormat.Jpeg,
                _ => ImageFormat.Png
            };

            PixelFormat pixelFormat = PixelFormat.Format32bppRgb;
            if (!String.IsNullOrWhiteSpace(model.PixelFormat))
            {
                pixelFormat = (PixelFormat)Enum.Parse(typeof(PixelFormat), model.PixelFormat);
            }

            string contentType = model.FileFormat.ToLower() switch
            {
                "png" => "image/png",
                "jpeg" => "image/jpeg",
                _ => "image/png"
            };

            var barcodeData = _barcodeService.Encode(text, barcodeFormat, fileFormat, model.Width, model.Height, pixelFormat);
            return File(barcodeData, contentType, $"{title ?? "barcode"}.{model.FileFormat}");
        }
    }
}
