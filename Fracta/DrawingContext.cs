using System.Drawing;

namespace Fracta
{
    public class DrawingContext
    {
        public Graphics Graphics { get; }
        public Image Image { get; }

        public DrawingContext(Image image)
        {
            Graphics = Graphics.FromImage(image);
            Image = image;
        }
    }
}