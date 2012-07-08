using DrawingColor = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;

namespace Utilities.Extension
{
    public static class ColorEx
    {
        public static DrawingColor FromMediaColor(this MediaColor color)
        {
            return DrawingColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static MediaColor FromDrawingColor(this DrawingColor color)
        {
            return MediaColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}