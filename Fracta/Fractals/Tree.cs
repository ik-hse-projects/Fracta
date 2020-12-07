using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Fracta.Fractals
{
    public class Tree : Fractal
    {
        private readonly TreeSettings _settings;

        /// <inheritdoc />
        public Tree()
        {
            _settings = new TreeSettings(this);
        }

        /// <inheritdoc />
        public override string Name => "Фрактальное дерево";

        /// <inheritdoc />
        public override Settings Settings => _settings;

        /// <inheritdoc />
        public override PointF Position => new PointF(0.5f, 1);

        /// <inheritdoc />
        public override int MaxIterations => 14;

        protected override IEnumerable Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                yield break;
            }

            var splitPoint = new PointF(0, -_settings.Length);

            // Теперь нарисуем одну единственную линию.
            var pen = GetPen(graphics, depth);
            graphics.Graphics.DrawLine(pen, PointF.Empty, splitPoint);
            yield return new object();

            var oldTransform = graphics.Graphics.Transform.Clone();
            // Сделаем так, чтобы начало координат лежало в конце только что нарисованного отрезка.
            graphics.Graphics.TranslateTransform(splitPoint.X, splitPoint.Y, MatrixOrder.Prepend);
            // Уменьшим поддерево в нужное число раз.
            graphics.Graphics.ScaleTransform(1 / _settings.Scaling, 1 / _settings.Scaling, MatrixOrder.Prepend);

            // Потом повернём всё вокруг нуля на нужный угол.
            graphics.Graphics.RotateTransform(-_settings.LeftAngle, MatrixOrder.Prepend);
            foreach (var i in Draw(graphics, depth - 1))
            {
                yield return i;
            }

            // И теперь в другую сторону. Нужно не забыть компенсировать предыдущее вращение.
            graphics.Graphics.RotateTransform(_settings.LeftAngle + _settings.RightAngle, MatrixOrder.Prepend);
            foreach (var i in Draw(graphics, depth - 1))
            {
                yield return i;
            }

            // И когда оба поддерева нарисованы, нужно обязательно всё вернуть как было,
            // так как после этого может быть нарисовано соседнее поддерево, а мы не хотим его сломать.
            graphics.Graphics.Transform = oldTransform;
        }

        /// <inheritdoc />
        public override FractalInfo GetInfo(int depth)
        {
            var radius = 0d;
            var scale = 1d;
            var alpha = _settings.LeftAngle * Math.PI / 180;
            var beta = _settings.RightAngle * Math.PI / 180;
            for (var i = 0; i < depth; i++)
            {
                var d = _settings.Length / scale;
                scale *= _settings.Scaling;
                var angle = Math.Max(
                    Math.Max(
                        Math.Abs(Math.Sin(i * alpha)),
                        Math.Abs(Math.Cos(i * alpha))
                    ), Math.Max(
                        Math.Abs(Math.Sin(-i * beta)),
                        Math.Abs(Math.Cos(-i * beta))
                    )
                );
                radius += d * angle;
            }

            return new FractalInfo
            {
                TotalWork = (int) Math.Pow(2, depth - 1),
                Width = (int) radius * 2,
                Height = (int) radius * 2
            };
        }

        private class TreeSettings : Settings
        {
            public TreeSettings(Fractal fractal) : base(fractal)
            {
                Add(_rightAngle);
                Add(_leftAngle);
                Add(_scaling);
                Add(_length);
            }

            public float RightAngle => (float) _rightAngle.Value;
            private readonly NumberInput _rightAngle = new NumberInput
            {
                Minimum = 0,
                Maximum = 180,
                Value = 60,
                Label = "Наклон правых"
            };

            public float LeftAngle => (float) _leftAngle.Value;
            private readonly NumberInput _leftAngle = new NumberInput
            {
                Minimum = 0,
                Maximum = 180,
                Value = 30,
                Label = "Наклон левых"
            };

            public float Scaling => (float) _scaling.Value;
            private readonly NumberInput _scaling = new NumberInput
            {
                Minimum = 0.2m,
                Maximum = 5,
                Value = 1.5m,
                Label = "Степень уменьшения",
                AllowFloat = true
            };

            public float Length => (int) _length.Value;
            private readonly NumberInput _length = new NumberInput
            {
                Minimum = 1,
                Maximum = 1000,
                Value = 300,
                Label = "Длина самой первой ветви"
            };
        }
    }
}