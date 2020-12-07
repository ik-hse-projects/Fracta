using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Fracta
{
    /// <summary>
    /// TabPage, которая выступает хранилищем какого-то фрактала.
    /// Этот класс лучше не использовать, вместо него есть TabPage<T>, который является хранилищем конкретного фрактала.
    /// </summary>
    public abstract class FractalPage : TabPage
    {
        /// <inheritdoc cref="TabPage" />
        protected FractalPage(string text) : base(text)
        {
        }

        /// <summary>
        /// Сам фрактал.
        /// </summary>
        public abstract Fractal Fractal { get; }

        /// <summary>
        /// Обновляет размеры поля для рисования.
        /// </summary>
        public abstract void UpdateSize();

        /// <summary>
        /// Перерисовывает фрактал.
        /// </summary>
        /// <param name="fast">
        /// Следует ли рисовать максимально быстро, или же надо учитывать указанную пользователем медленность отрисовки.
        /// </param>
        public abstract void Draw(bool fast);
    }

    /// <summary>
    /// TabPage, которая выступает хранилищем фрактала типа T.
    /// </summary>
    /// <typeparam name="T">Тип фрактала.</typeparam>
    public class FractalPage<T> : FractalPage where T : Fractal, new()
    {
        /// <summary>
        /// То, где происходит отрисовка.
        /// </summary>
        private DrawingContext _drawing;

        /// <summary>
        /// Control, который отображает картинку.
        /// </summary>
        private readonly PictureBox _picbox;

        /// <summary>
        /// Создаёт FractalPage для указанного в типе фрактала.
        /// </summary>
        public FractalPage() : this(new T())
        {
        }

        /// <summary>
        /// Создаёт FractalPage для указанного фрактала.
        /// Стоит заметить, что это приватный конструктор (без особой на то причины).
        /// </summary>
        private FractalPage(T fractal) : base(fractal.Name)
        {
            Dock = DockStyle.Fill;
            SpecificFractal = fractal;

            Controls.Add(fractal.Settings);
            fractal.Settings.ValueChanged += () => Draw(false);
            fractal.Settings.OnSaveButtonClick += (sender, e) => SaveImage();

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

        /// <inheritdoc />
        public override Fractal Fractal => SpecificFractal;

        /// <summary>
        /// Фрактал вполне конкретного типа.
        /// </summary>
        public T SpecificFractal { get; }

        /// <summary>
        /// Подготавливает Bitmap для рисования.
        /// </summary>
        private Bitmap CreateBitmap()
        {
            var offset = 5;

            offset += Fractal.Settings.Bounds.Height + 5;

            var availableHeight = Bounds.Height - offset;
            if (availableHeight <= 0)
            {
                availableHeight = 1;
            }

            _picbox.SetBounds(5, offset, Bounds.Width - 5, availableHeight);
            return new Bitmap(Bounds.Width - 10, availableHeight);
        }

        /// <summary>
        /// Сохраняет картинку в файл.
        /// </summary>
        private void SaveImage()
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "png",
                ValidateNames = true,
                Filter = "*.png",
                FilterIndex = 0
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

        /// <inheritdoc />
        public override void UpdateSize()
        {
            var image = CreateBitmap();
            _picbox.Image = image;
            _picbox.Size = image.Size;
            _drawing = new DrawingContext(image);
            _drawing.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        }

        /// <inheritdoc />
        public override void Draw(bool fast)
        {
            _drawing.Graphics.Clear(Color.White);

            var info = Fractal.GetInfo(Fractal.Settings.Iterations);

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

            var counter = 0;
            foreach (var o in Fractal.StartDrawing(_drawing, Fractal.Settings.Iterations))
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