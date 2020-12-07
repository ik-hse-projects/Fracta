using System;
using System.Collections;
using System.Drawing;

namespace Fracta.Fractals
{
    public class KochCurve : Fractal
    {
        private readonly CantorCache _cache = new CantorCache(new CantorDefinition
        {
            Left = 1 / 3d,
            Right = 1 / 3d
        });

        private readonly KochCurveSettings _settings;

        public KochCurve()
        {
            _settings = new KochCurveSettings(this);
        }

        /// <inheritdoc />
        public override string Name => "Кривая Коха";

        /// <inheritdoc />
        public override Settings Settings => _settings;

        /// <inheritdoc />
        public override PointF Position => new PointF(0.5f, 0.7f);

        /// <inheritdoc />
        public override int MaxIterations => 8;

        /// <inheritdoc />
        protected override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                yield break;
            }

            var sixthLine = _settings.LineLength / 6d;

            var height = _settings.Height;

            var hypot2 = height * height + sixthLine * sixthLine;
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
            {
                yield return x;
            }

            graphics.Graphics.Transform = oldTransform.Clone();

            // Справа.
            graphics.Graphics.TranslateTransform((float) (2 * sixthLine), 0);
            graphics.Graphics.ScaleTransform(scaleNormal, scaleNormal);
            foreach (var x in Draw(graphics, depth - 1))
            {
                yield return x;
            }

            graphics.Graphics.Transform = oldTransform.Clone();

            var cantor = _cache.GetState(depth - 1);

            // Слева под углом.
            graphics.Graphics.TranslateTransform(-centerx, -centery);
            graphics.Graphics.RotateTransform((float) -angle);
            graphics.Graphics.ScaleTransform(scaleAngled, scaleAngled);
            var pen = GetPen(graphics, depth);
            foreach (var x in cantor.Draw(graphics, pen, _settings.LineLength))
            {
                yield return x;
            }

            foreach (var x in Draw(graphics, depth - 1))
            {
                yield return x;
            }

            graphics.Graphics.Transform = oldTransform.Clone();

            // Справа под углом.
            graphics.Graphics.TranslateTransform(centerx, -centery);
            graphics.Graphics.RotateTransform((float) angle);
            graphics.Graphics.ScaleTransform(scaleAngled, scaleAngled);
            foreach (var x in cantor.Draw(graphics, pen, _settings.LineLength))
            {
                yield return x;
            }

            foreach (var x in Draw(graphics, depth - 1))
            {
                yield return x;
            }

            graphics.Graphics.Transform = oldTransform;
        }

        /// <inheritdoc />
        public override IEnumerable StartDrawing(DrawingContext graphics, int depth)
        {
            var pen = GetPen(graphics, depth);
            foreach (var x in _cache.GetState(depth).Draw(graphics, pen, _settings.LineLength))
            {
                yield return x;
            }

            foreach (var x in base.StartDrawing(graphics, depth))
            {
                yield return x;
            }
        }

        /// <inheritdoc />
        public override FractalInfo GetInfo(int depth)
        {
            return new FractalInfo
            {
                TotalWork = (int) Math.Pow(4, depth + 1),
                Width = _settings.LineLength,
                Height = _settings.Height
            };
        }

        private class KochCurveSettings : Settings
        {
            public KochCurveSettings(Fractal fractal) : base(fractal)
            {
                Add(_lineLength);
                Add(_height);
            }

            // Документация опущена, см. соответстувующие Label.
            public int LineLength => (int) _lineLength.Value;
            private readonly NumberInput _lineLength = new NumberInput
            {
                Minimum = 1,
                Maximum = 3000,
                Value = 1000,
                Label = "Длина линии"
            };

            public int Height => (int) _height.Value;
            private readonly NumberInput _height = new NumberInput
            {
                Minimum = 10,
                Maximum = 1000,
                Value = 333,
                Label = "Высота пиков"
            };
        }
    }
}