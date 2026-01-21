namespace MON.Models.Image
{
    using MON.Shared.Enums;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ImageDetailsModel
    {
        public bool HasColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public PageOrientationEnum PageOrientation { get; set; }
    }
}
