using System.Drawing;
using System.Windows.Forms;

namespace Fracta
{
    public abstract class FractalPage : TabPage
    {
        protected FractalPage(string text) : base(text)
        {
        }

        public abstract Fractal Fractal { get; }
        public abstract void UpdateSize();
        public abstract void Draw();
    }

    public class FractalPage<T> : FractalPage where T : Fractal, new()
    {
        public override Fractal Fractal => SpecificFractal;

        private T SpecificFractal { get; }

        private DrawingContext _drawing;
        private PictureBox _picbox;

        public FractalPage() : this(new T())
        {
        }

        private FractalPage(T fractal) : base(fractal.Name)
        {
            Dock = DockStyle.Fill;
            SpecificFractal = fractal;

            Controls.Add(fractal.Settings ?? new Control());

            var scroller = new Panel();
            scroller.AutoScroll = true;
            scroller.AutoSize = true;
            Controls.Add(scroller);
            
            _picbox = new PictureBox();
            scroller.Controls.Add(_picbox);

            UpdateSize();
        }

        private Bitmap CreateBitmap()
        {
            var offset = 5;

            if (Fractal.Settings != null)
            {
                offset += Fractal.Settings.Bounds.Height + 5;
            }

            var available_height = Bounds.Height - offset;
            if (available_height <= 0)
            {
                available_height = 1;
            }

            _picbox.SetBounds(5, offset, Bounds.Width - 5, available_height);
            return new Bitmap(Bounds.Width - 10, available_height);
        }

        public override void UpdateSize()
        {
            var image = CreateBitmap();
            _picbox.Image = image;
            _picbox.Size = image.Size;
            _drawing = new DrawingContext(image);
        }

        public override void Draw()
        {
            Fractal.DrawOn(_drawing);
            _picbox.Refresh();
        }
    }
}