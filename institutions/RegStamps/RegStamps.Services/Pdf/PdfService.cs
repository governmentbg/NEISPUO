namespace RegStamps.Services.Pdf
{
    using DinkToPdf.Contracts;
    using DinkToPdf;
    using System.Text;

    public class PdfService : IPdfService
    {
        private readonly IConverter converter;

        public PdfService()
        {
            this.converter = new SynchronizedConverter(new PdfTools());
        }

        public byte[] CreatePdfFromHtml(string html, bool isLandscape = false, string headerHtml = null, string footerHtml = null)
        {
            GlobalSettings globalSettings = new GlobalSettings();
            ObjectSettings objectSettings = new ObjectSettings();


            globalSettings.Margins = new MarginSettings { Top = 20, Bottom = 15 };
            globalSettings.PaperSize = PaperKind.A4;

            if (isLandscape)
            {
                globalSettings.Orientation = Orientation.Landscape;
            }

            if (!string.IsNullOrWhiteSpace(headerHtml))
            {
                objectSettings.HeaderSettings = new HeaderSettings
                {
                    FontSize = 8,
                    Left = headerHtml,
                };
            }

            if (!string.IsNullOrWhiteSpace(footerHtml))
            {
                objectSettings.FooterSettings = new FooterSettings
                {
                    FontSize = 8,
                    Left = footerHtml,
                    Right = "Страница [page] от [toPage]"
                };
            }


            objectSettings.PagesCount = true;
            objectSettings.HtmlContent = html;
            objectSettings.WebSettings = new WebSettings { DefaultEncoding = nameof(Encoding.UTF8) };

            //var globalSettings = new GlobalSettings
            //{
            //    ColorMode = ColorMode.Color,
            //    Orientation = Orientation.Portrait,
            //    PaperSize = PaperKind.A4,
            //    Margins = new MarginSettings { Top = 10 },
            //    DocumentTitle = "PDF Report"
            //};

            //var objectSettings = new ObjectSettings
            //{
            //    PagesCount = true,
            //    HtmlContent = html,
            //    WebSettings = { DefaultEncoding = nameof(Encoding.UTF8) },
            //    //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            //    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }

            //};

            IDocument pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return converter.Convert(pdf);
        }
    }
}
