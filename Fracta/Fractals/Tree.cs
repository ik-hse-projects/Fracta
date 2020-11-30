using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Fracta.Fractals
{
    public class Tree : Fractal
    {
        public Tree()
        {
            _settings = new TreeSettings();
        }

        public override string Name => "Фрактальное дерево";

        private TreeSettings _settings;
        public override Settings Settings => _settings;

        private SizeF Rotate(SizeF direction, double angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            return new SizeF(
                (float) (direction.Width * cos - direction.Height * sin),
                (float) (direction.Width * sin - direction.Height * cos)
            );
        }
        
        private static string[] colortable = {
            "#000000", "#FFFF00", "#1CE6FF", "#FF34FF", "#FF4A46", "#008941", "#006FA6", "#A30059",
            "#FFDBE5", "#7A4900", "#0000A6", "#63FFAC", "#B79762", "#004D43", "#8FB0FF", "#997D87",
            "#5A0007", "#809693", "#FEFFE6", "#1B4400", "#4FC601", "#3B5DFF", "#4A3B53", "#FF2F80",
            "#61615A", "#BA0900", "#6B7900", "#00C2A0", "#FFAA92", "#FF90C9", "#B903AA", "#D16100",
            "#DDEFFF", "#000035", "#7B4F4B", "#A1C299", "#300018", "#0AA6D8", "#013349", "#00846F",
            "#372101", "#FFB500", "#C2FFED", "#A079BF", "#CC0744", "#C0B9B2", "#C2FF99", "#001E09",
            "#00489C", "#6F0062", "#0CBD66", "#EEC3FF", "#456D75", "#B77B68", "#7A87A1", "#788D66",
            "#885578", "#FAD09F", "#FF8A9A", "#D157A0", "#BEC459", "#456648", "#0086ED", "#886F4C",

            "#34362D", "#B4A8BD", "#00A6AA", "#452C2C", "#636375", "#A3C8C9", "#FF913F", "#938A81",
            "#575329", "#00FECF", "#B05B6F", "#8CD0FF", "#3B9700", "#04F757", "#C8A1A1", "#1E6E00",
            "#7900D7", "#A77500", "#6367A9", "#A05837", "#6B002C", "#772600", "#D790FF", "#9B9700",
            "#549E79", "#FFF69F", "#201625", "#72418F", "#BC23FF", "#99ADC0", "#3A2465", "#922329",
            "#5B4534", "#FDE8DC", "#404E55", "#0089A3", "#CB7E98", "#A4E804", "#324E72", "#6A3A4C",
            "#83AB58", "#001C1E", "#D1F7CE", "#004B28", "#C8D0F6", "#A3A489", "#806C66", "#222800",
            "#BF5650", "#E83000", "#66796D", "#DA007C", "#FF1A59", "#8ADBB4", "#1E0200", "#5B4E51",
            "#C895C5", "#320033", "#FF6832", "#66E1D3", "#CFCDAC", "#D0AC94", "#7ED379", "#012C58"
        };
        
        public override void Draw(DrawingContext graphics, int depth)
        {
            if (depth == 0)
            {
                return;
            }
            
            var splitPoint = new PointF(0, -_settings.Length);

            // Хочется, чтобы толщина пера всегда была равна требуемой.
            // Поэтому рассчитаем то, насколько масштабировано пространство, а затем поделим на это число.
            // Тогда когда толщина умножится на число (при выводе на экран), то она будет как раз примерно требуемая.
            // По крайней мере идея такая.
            var scale = (float) graphics.Graphics.Transform.GetAverageScale();
            var pen = new Pen(ColorTranslator.FromHtml(colortable[depth]), _settings.Width / scale);
            
            // Теперь нарисуем одну единственную линию.
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

        public float Width => (int) _width.Value;
        
        private NumberInput _rightAngle;
        private NumberInput _leftAngle;
        private NumberInput _scaling;
        private NumberInput _length;
        private NumberInput _width;
        
        public TreeSettings()
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
            Add(_width = new NumberInput
            {
                Minimum = 1,
                Maximum = 100,
                Value = 3,
                Label = "Толщина линий",
            });
        }
    }
}