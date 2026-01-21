namespace MON.Models
{
    public class BarcodeGenerationModel
    {
        public string Format { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string FileFormat { get; set; }
        public string PixelFormat { get; set; }
    }
}
