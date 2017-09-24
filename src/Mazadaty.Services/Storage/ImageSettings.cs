using System.Drawing;

namespace Mazadaty.Services.Storage
{
    public class ImageSettings
    {
        public int[] Widths { get; set; }
        public bool ForceSquare { get; set; }
        public Color BackgroundColor { get; set; }
        public string Watermark { get; set; }

        public ImageSettings()
        {
            Widths = new[] { -1 };
            ForceSquare = false;
            BackgroundColor = Color.Black;
            Watermark = "";
        }
    }
}