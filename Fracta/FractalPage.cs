using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
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
        public abstract void Draw(bool fast);
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

            Controls.Add(fractal.Settings);
            fractal.Settings.ValueChanged += (sender, args) => Draw(false);

            var scroller = new Panel
            {
                AutoScroll = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Dock = DockStyle.Fill
            };
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
            var position = Fractal.Position;
            _drawing.Graphics.TranslateTransform(_picbox.Size.Width * position.X, _picbox.Size.Height * position.Y);
            _drawing.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        }
        public override void Draw(bool fast)
        {
            _drawing.Graphics.Clear(Color.White);

            var redrawEvery = Fractal.TotalWorkRequired(Fractal.RecursionDepth) / Fractal.Settings.Slowness;
            if (fast)
            {
                redrawEvery = long.MaxValue;
            }
            
            int counter = 0;
            foreach (var o in Fractal.StartDrawing(_drawing, Fractal.RecursionDepth))
            {
                counter++;
                if (counter >= redrawEvery)
                {
                    // https://stackoverflow.com/a/2568723
                    _picbox.Refresh();
                    counter = 0;
                }
            }

            _picbox.Refresh();
        }
    }
}