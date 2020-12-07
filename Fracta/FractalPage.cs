using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
            fractal.Settings.ValueChanged += () => Draw(false);
            fractal.Settings.OnSaveButtonClick += SaveImage;

            var scroller = new Panel
            {
                AutoScroll = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Dock = DockStyle.Fill
            };
            Controls.Add(scroller);

            _picbox = new PictureBox();
            scroller.Controls.Add(_picbox);
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

        private void SaveImage(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "png",
                ValidateNames = true,
                Filter = "*.png",
                FilterIndex = 0,
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    _picbox.Image.Save(dialog.FileName, ImageFormat.Png);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Не удалось сохранить: {exception.Message}", "Ошибка");
                }
            }
        }

        public override void UpdateSize()
        {
            var image = CreateBitmap();
            _picbox.Image = image;
            _picbox.Size = image.Size;
            _drawing = new DrawingContext(image);
            _drawing.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        public override void Draw(bool fast)
        {
            _drawing.Graphics.Clear(Color.White);

            var info = Fractal.GetInfo(Fractal.RecursionDepth);

            _drawing.Graphics.ResetTransform();
            _drawing.Graphics.TranslateTransform(
                _picbox.Size.Width * Fractal.Position.X,
                _picbox.Size.Height * Fractal.Position.Y
            );
            var k = 0.9f;
            var scale = Math.Min(
                _drawing.Image.Width * k / info.Width,
                _drawing.Image.Height * k / info.Height
            );
            _drawing.Graphics.ScaleTransform(scale, scale);

            var redrawEvery = info.TotalWork / Fractal.Settings.Slowness;
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