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
            if (depth <= 0)
            {
                yield break;
            }

            var start = -_settings.TotalWidth / 2.0;

            var totalParts = _settings.Left + _settings.Right + _settings.Center;
            var partWidth = (double) _settings.TotalWidth / totalParts;

            // Концы частей рисунка.
            var left = start + _settings.Left * partWidth;
            var center = left + _settings.Center * partWidth;
            var right = center + _settings.Right * partWidth;
            
            if (depth == 1)
            {
                var pen = GetPen(graphics, depth);
                graphics.Graphics.DrawLine(pen, (float) start, 0, (float) left, 0);
                graphics.Graphics.DrawLine(pen, (float) center, 0, (float) right, 0);
                yield return new object();
            }

            var oldTransform = graphics.Graphics.Transform.Clone();
            
            var leftCenter = (start + left) / 2;
            var scaleLeft = (float) _settings.Left / totalParts;
            graphics.Graphics.TranslateTransform((float) leftCenter, 0);
            graphics.Graphics.ScaleTransform(scaleLeft, scaleLeft);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();
            
            var rightCenter = (center + right) / 2;
            var scaleRight = (float) _settings.Right / totalParts;
            graphics.Graphics.TranslateTransform((float) rightCenter, 0);
            graphics.Graphics.ScaleTransform(scaleRight, scaleRight);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;

            graphics.Graphics.Transform = oldTransform;
        }

        public override PointF Position => new PointF(0.5f, 0.5f);
        public override int MaxIterations => 14;

        public override long TotalWorkRequired(int depth)
        {
            return (int) Math.Pow(2, depth + 1);
        }
    }

    public class CantorSettings : Settings
    {
        public int Left => (int) _left.Value;
        public int Right => (int) _right.Value;
        public int Center => (int) _center.Value;
        public int TotalWidth => (int) _total.Value;

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
            Add(_total);
            Add(_left);
            Add(_right);
            Add(_center);
        }
    }
}