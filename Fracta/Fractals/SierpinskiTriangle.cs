using System;
using System.Collections;
using System.Drawing;

namespace Fracta.Fractals
{
    public class SierpinskiTriangle : Fractal
    {
        public override string Name => "Треугольник Серпинского";

        private SierpinskiTriangleSettings _settings;
        public override Settings Settings => _settings;

        public SierpinskiTriangle()
        {
            _settings = new SierpinskiTriangleSettings(this);
        }

        private static double Sin30 = Math.Sin(Math.PI / 6);
        private static double Cos30 = Math.Cos(Math.PI / 6);

        public override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                yield break;
            }

            var half = _settings.Size / 2;
            var offsetX = (float) (half * Cos30);
            var offsetY = (float) (half * Sin30);

            var bottom = new PointF(0, half);
            var topLeft = new PointF(-offsetX, -offsetY);
            var topRight = new PointF(+offsetX, -offsetY);

            var pen = GetPen(graphics, depth);
            graphics.Graphics.DrawLines(pen, new[] {bottom, topLeft, topRight, bottom});
            yield return new object();

            var oldTransform = graphics.Graphics.Transform.Clone();

            // Верхний треугольник.
            graphics.Graphics.TranslateTransform(0, -half);
            graphics.Graphics.ScaleTransform(0.5f, 0.5f);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();

            // Слева снизу.
            graphics.Graphics.TranslateTransform(-offsetX, offsetY);
            graphics.Graphics.ScaleTransform(0.5f, 0.5f);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();

            // Справа снизу.
            graphics.Graphics.TranslateTransform(+offsetX, offsetY);
            graphics.Graphics.ScaleTransform(0.5f, 0.5f);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform;
        }

        public override IEnumerable StartDrawing(DrawingContext graphics, int depth)
        {
            var radius = _settings.Size;
            var offsetY = (float) (radius * Sin30);
            var offsetX = (float) (radius * Cos30);

            var top = new PointF(0, -radius);
            var left = new PointF(-offsetX, offsetY);
            var right = new PointF(+offsetX, offsetY);

            var pen = GetPen(graphics, depth + 1);
            graphics.Graphics.DrawLines(pen, new[] {top, left, right, top});

            return base.StartDrawing(graphics, depth);
        }

        public override PointF Position => new PointF(0.5f, 0.5f);
        public override int MaxIterations => 9;

        public override FractalInfo GetInfo(int depth)
        {
            return new FractalInfo
            {
                TotalWork = (int) Math.Pow(3, depth + 1),
                Width = (int) _settings.Size * 2,
                Height = (int) _settings.Size * 2,
            };
        }
    }

    public class SierpinskiTriangleSettings : Settings
    {
        public float Size => (float) _size.Value;

        private NumberInput _size = new NumberInput
        {
            Label = "Размер треугольника",
            Minimum = 1,
            Maximum = 1000,
            Value = 500,
        };

        public SierpinskiTriangleSettings(Fractal fractal) : base(fractal)
        {
            Add(_size);
        }
    }
}