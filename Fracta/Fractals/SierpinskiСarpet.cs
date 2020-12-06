using System;
using System.Collections;
using System.Drawing;

namespace Fracta.Fractals
{
    public class SierpinskiСarpet : Fractal
    {
        public override string Name => "Ковёр Серпинского";

        private SierpinskiСarpetSettings _settings;

        public SierpinskiСarpet()
        {
            _settings = new SierpinskiСarpetSettings(this);
        }

        public override Settings Settings => _settings;

        public override PointF Position => new PointF(0.5f, 0.5f);

        public override int MaxIterations => 6;

        public override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                yield break;
            }

            var pen = GetPen(graphics, depth).Brush;
            var third = _settings.Size / 3f;
            graphics.Graphics.FillRectangle(pen, -third / 2, -third / 2, third, third);
            yield return new object();

            if (depth == 1)
            {
                yield break;
            }

            var oldTransform = graphics.Graphics.Transform.Clone();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    graphics.Graphics.TranslateTransform(dx * third, dy * third);
                    graphics.Graphics.ScaleTransform(1 / 3f, 1 / 3f);
                    foreach (var x in Draw(graphics, depth - 1))
                        yield return x;

                    graphics.Graphics.Transform = oldTransform.Clone();
                }
            }
        }
        
        public override FractalInfo GetInfo(int depth)
        {
            return new FractalInfo
            {
                TotalWork = (int) Math.Pow(8, depth + 1),
                Width = _settings.Size,
                Height = _settings.Size
            };
        }
    }

    public class SierpinskiСarpetSettings : Settings
    {
        public int Size => (int) _size.Value;

        private NumberInput _size = new NumberInput
        {
            Label = "Размер квадрата",
            Minimum = 1,
            Maximum = 10000,
            Value = 1000,
        };

        public SierpinskiСarpetSettings(Fractal fractal) : base(fractal)
        {
            Add(_size);
        }
    }
}