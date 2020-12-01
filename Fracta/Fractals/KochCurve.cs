using System;
using System.Collections;
using System.Drawing;

namespace Fracta.Fractals
{
    public class KochCurve : Fractal
    {
        public override string Name => "Кривая Коха";

        private readonly KochCurveSettings _settings;

        public KochCurve()
        {
            _settings = new KochCurveSettings(this);
        }

        public override Settings Settings => _settings;

        public override PointF Position => new PointF(0.5f, 0.7f);
        
        public override int MaxIterations => 8;
        
        public override long TotalWorkRequired(int depth) => (long) Math.Pow(4, depth - 1);
        
        public override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                yield break;
            }

            var pen = GetPen(graphics, depth);

            var halfLine = _settings.LineLength / 2d;
            var left = new PointF((float) -halfLine, 0);
            var right = new PointF((float) halfLine, 0);

            var sixthLine = _settings.LineLength / 6d;
            var peakLeft = new PointF((float) -sixthLine, 0);
            var peakRight = new PointF((float) sixthLine, 0);

            var height = _settings.Height;
            var peak = new Point(0, -height);

            if (depth == 1)
            {
                graphics.Graphics.DrawLines(pen, new[]
                {
                    left,
                    peakLeft,
                    peak,
                    peakRight,
                    right
                });
                yield return new object();
                yield break;
            }

            var hypot2 = (height * height) + (sixthLine * sixthLine);
            var centerx = (float) (sixthLine / 2f);
            var centery = height / 2f;

            var angle = 180 / Math.PI * Math.Atan(height / sixthLine);
            var scaleAngled = (float) (Math.Sqrt(hypot2) / _settings.LineLength);
            var scaleNormal = 1 / 3f;

            // Сохраняем положение. Лучше см. Tree: там больше комментариев по поводу этого фокуса. 
            var oldTransform = graphics.Graphics.Transform.Clone();

            // Слева.
            graphics.Graphics.TranslateTransform((float) (-2 * sixthLine), 0);
            graphics.Graphics.ScaleTransform(scaleNormal, scaleNormal);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();

            // Справа.
            graphics.Graphics.TranslateTransform((float) (2 * sixthLine), 0);
            graphics.Graphics.ScaleTransform(scaleNormal, scaleNormal);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();

            // Слева под углом.
            graphics.Graphics.TranslateTransform(-centerx, -centery);
            graphics.Graphics.RotateTransform((float) (-angle));
            graphics.Graphics.ScaleTransform(scaleAngled, scaleAngled);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform.Clone();

            // Справа под углом.
            graphics.Graphics.TranslateTransform(centerx, -centery);
            graphics.Graphics.RotateTransform((float) angle);
            graphics.Graphics.ScaleTransform(scaleAngled, scaleAngled);
            foreach (var x in Draw(graphics, depth - 1))
                yield return x;
            graphics.Graphics.Transform = oldTransform;
        }
    }

    public class KochCurveSettings : Settings
    {
        public int LineLength => (int) _lineLength.Value;
        public int Height => (int) _height.Value;

        private NumberInput _lineLength;
        private NumberInput _height;

        public KochCurveSettings(Fractal fractal) : base(fractal)
        {
            Add(_lineLength = new NumberInput
            {
                Minimum = 1,
                Maximum = 3000,
                Value = 1000,
                Label = "Длина линии"
            });
            Add(_height = new NumberInput
            {
                Minimum = 10,
                Maximum = 1000,
                Value = 333,
                Label = "Высота пиков"
            });
        }
    }
}