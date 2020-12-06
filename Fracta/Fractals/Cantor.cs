using System;
using System.Collections;
using System.Drawing;

namespace Fracta.Fractals
{
    public class Cantor : Fractal
    {
        public override string Name => "Множество Кантора";

        private CantorSettings _settings;

        public Cantor()
        {
            _settings = new CantorSettings(this);
        }

        public override Settings Settings => _settings;

        public override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            var totalParts = (double) (_settings.Left + _settings.Center + _settings.Right);
            var cache = new CantorCache(new CantorDefinition
            {
                Left = _settings.Left / totalParts,
                Right = _settings.Right / totalParts,
            });

            var oldTransform = graphics.Graphics.Transform.Clone();

            for (var i = 0; i < depth; i++)
            {
                var state = cache.GetState(i);
                var pen = GetPen(graphics, i);
                foreach (var x in state.Draw(graphics, pen, _settings.TotalWidth))
                    yield return x;
                graphics.Graphics.TranslateTransform(0, _settings.Distance);
            }

            graphics.Graphics.Transform = oldTransform;
        }

        public override PointF Position => new PointF(0.5f, 0.0f);
        public override int MaxIterations => 14;

        public override FractalInfo GetInfo(int depth)
        {
            return new FractalInfo
            {
                TotalWork = (int) Math.Pow(2, depth + 1),
                Width = _settings.TotalWidth,
                Height = (int) ((depth - 1) * (_settings.Distance + _settings.Width))
            };
        }
    }

    public class CantorSettings : Settings
    {
        public int Left => (int) _left.Value;
        public int Right => (int) _right.Value;
        public int Center => (int) _center.Value;
        public int TotalWidth => (int) _total.Value;

        public int Distance => (int) _distance.Value;

        private NumberInput _distance = new NumberInput
        {
            Minimum = 0,
            Maximum = 1000,
            Value = 100,
            Label = "Расстояние между итерациями"
        };

        private NumberInput _total = new NumberInput
        {
            Minimum = 3,
            Maximum = 5000,
            Value = 500,
            Label = "Длина всего отрезка, в пикселях"
        };

        private NumberInput _left = new NumberInput
        {
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            Label = "Длина левого закрашенного участка, в частях"
        };

        private NumberInput _right = new NumberInput
        {
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            Label = "Длина правого закрашенного участка, в частях"
        };

        private NumberInput _center = new NumberInput
        {
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            Label = "Длина центрального пустого участка, в частях"
        };

        public CantorSettings(Fractal fractal) : base(fractal)
        {
            Add(_distance);
            Add(_total);
            Add(_left);
            Add(_right);
            Add(_center);
        }
    }
}