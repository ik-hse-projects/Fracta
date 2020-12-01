using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Fracta.Fractals
{
    public class Tree : Fractal
    {
        public override string Name => "Фрактальное дерево";

        private readonly TreeSettings _settings;

        public Tree()
        {
            _settings = new TreeSettings(this);
        }

        public override Settings Settings => _settings;

        public override PointF Position => new PointF(0.5f, 1);

        public override int MaxIterations => 14;

        private SizeF Rotate(SizeF direction, double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            return new SizeF(
                (float) (direction.Width * cos - direction.Height * sin),
                (float) (direction.Width * sin - direction.Height * cos)
            );
        }

        public override void Draw(DrawingContext graphics, int depth)
        {
            if (depth <= 0)
            {
                return;
            }

            var splitPoint = new PointF(0, -_settings.Length);

            // Теперь нарисуем одну единственную линию.
            var pen = GetPen(graphics, depth);
            graphics.Graphics.DrawLine(pen, PointF.Empty, splitPoint);

            var oldTransform = graphics.Graphics.Transform.Clone();
            // Сделаем так, чтобы начало координат лежало в конце только что нарисованного отрезка.
            graphics.Graphics.TranslateTransform(splitPoint.X, splitPoint.Y, MatrixOrder.Prepend);
            // Уменьшим поддерево в нужное число раз.
            graphics.Graphics.ScaleTransform(1 / _settings.Scaling, 1 / _settings.Scaling, MatrixOrder.Prepend);

            // Потом повернём всё вокруг нуля на нужный угол.
            graphics.Graphics.RotateTransform(-_settings.LeftAngle, MatrixOrder.Prepend);
            Draw(graphics, depth - 1);

            // И теперь в другую сторону. Нужно не забыть компенсировать предыдущее вращение.
            graphics.Graphics.RotateTransform(_settings.LeftAngle + _settings.RightAngle, MatrixOrder.Prepend);
            Draw(graphics, depth - 1);

            // И когда оба поддерева нарисованы, нужно обязательно всё вернуть как было,
            // так как после этого может быть нарисовано соседнее поддерево, а мы не хотим его сломать.
            graphics.Graphics.Transform = oldTransform;
        }
    }

    public class TreeSettings : Settings
    {
        public float RightAngle => (float) _rightAngle.Value;
        public float LeftAngle => (float) _leftAngle.Value;
        public float Scaling => (float) _scaling.Value;
        public float Length => (int) _length.Value;

        private NumberInput _rightAngle;
        private NumberInput _leftAngle;
        private NumberInput _scaling;
        private NumberInput _length;

        public TreeSettings(Fractal fractal) : base(fractal)
        {
            Add(_rightAngle = new NumberInput
            {
                Minimum = 0,
                Maximum = 180,
                Value = 60,
                Label = "Наклон правых",
            });
            Add(_leftAngle = new NumberInput
            {
                Minimum = 0,
                Maximum = 180,
                Value = 30,
                Label = "Наклон левых",
            });
            Add(_scaling = new NumberInput
            {
                Minimum = 0.2m,
                Maximum = 5,
                Value = 1.5m,
                Label = "Степень уменьшения",
                AllowFloat = true,
            });
            Add(_length = new NumberInput
            {
                Minimum = 1,
                Maximum = 1000,
                Value = 300,
                Label = "Длина самой первой ветви",
            });
        }
    }
}