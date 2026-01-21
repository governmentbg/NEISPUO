namespace MON.Shared.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum ExifOrientationEnum
    {
        NotAvailable = -1,
        TopLeft = 1,        // The image is oriented correctly (no rotation needed).
        TopRight = 2,       // The image is flipped horizontally.
        BottomRight = 3,    // The image is rotated 180 degrees.
        BottomLeft = 4,     // The image is flipped vertically.
        LeftTop = 5,        // The image is rotated 90 degrees clockwise and flipped vertically.
        RightTop = 6,       // The image is rotated 90 degrees clockwise.
        RightBottom = 7,    // The image is rotated 90 degrees counter-clockwise and flipped vertically.
        LeftBottom = 8      // The image is rotated 90 degrees counter-clockwise.
    }
}
