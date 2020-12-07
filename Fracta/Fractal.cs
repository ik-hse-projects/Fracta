using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Fracta
{
    public abstract class Fractal
    {
        public abstract string Name { get; }

        public abstract Settings Settings { get; }

        public int RecursionDepth => Settings.Iterations;

        public abstract IEnumerable Draw(DrawingContext graphics, int depth);

        public virtual IEnumerable StartDrawing(DrawingContext graphics, int depth)
        {
            return Draw(graphics, depth);
        }
        
        public abstract PointF Position { get; }
        
        public abstract int MaxIterations { get; }

        public abstract FractalInfo GetInfo(int depth);

        protected Pen GetPen(DrawingContext graphics, int depth, float? width = null)
        {
            var color = Settings.GradientKind.GetColor(Settings.StartColor, Settings.EndColor, depth, Settings.Iterations);
            
            // Хочется, чтобы толщина пера всегда была равна требуемой.
            // Поэтому рассчитаем то, насколько масштабировано пространство, а затем поделим на это число.
            // Тогда когда толщина умножится на число (при выводе на экран), то она будет как раз примерно требуемая.
            // По крайней мере идея такая.
            var scale = (float) graphics.Graphics.Transform.GetAverageScale();
            var pen = new Pen(color, (width ?? Settings.Width) / scale);
            return pen;
        }
    }

    public struct FractalInfo
    {
        public int Width;
        public int Height;
        public long TotalWork;
    }
}