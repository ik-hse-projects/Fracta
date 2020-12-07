using System.Collections;
using System.Drawing;

namespace Fracta
{
    /// <summary>
    /// Базовый класс для любого фрактала.
    /// </summary>
    public abstract class Fractal
    {
        /// <summary>
        /// Название фрактала.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Его настройки.
        /// </summary>
        public abstract Settings Settings { get; }

        // Идентично Settings.Iterations, но раз уж потребовали это сделать обязательно, то вот:
        // Интересный факт: это свойство совершенно нигде не используется.
        /// <summary>
        /// Глубина рекурсии фрактала.
        /// </summary>
        public int RecursionDepth => Settings.Iterations;

        /// <summary>
        /// Позиция на экране, где должен находиться начало координат фрактала. Обе координаты должны быть от 0 до 1.
        /// </summary>
        public abstract PointF Position { get; }

        /// <summary>
        /// Наибольшее кол-во итераций, считающееся разумным.
        /// </summary>
        public abstract int MaxIterations { get; }

        /// <summary>
        /// Рисует фрактал.
        /// </summary>
        /// <param name="graphics">Контекст, в котором происходит отрисовка.</param>
        /// <param name="depth">Сколько ещё предстоит итераций, включая эту.</param>
        /// <returns>
        /// IEnumerable чего бы то ни было.
        /// Используется для красивой замедленной отрисовки и превращения возможного StackOverflow в OutOfMemory
        /// </returns>
        protected abstract IEnumerable Draw(DrawingContext graphics, int depth);

        /// <summary>
        /// Начинает отрисовку фрактала.
        /// Позволяет нарисовать или подготовить что-либо до того, как вызовется хоть один Draw.
        /// В остальном аналог обычного Draw.
        /// </summary>
        public virtual IEnumerable StartDrawing(DrawingContext graphics, int depth)
        {
            return Draw(graphics, depth);
        }

        /// <summary>
        /// Вычисляет некоторые свойства фрактала.
        /// </summary>
        public abstract FractalInfo GetInfo(int depth);

        /// <summary>
        /// Возвращает перо нужного цвета и толщины, чтобы всё выглядело правильно и красиво.
        /// </summary>
        protected Pen GetPen(DrawingContext graphics, int depth, float? width = null)
        {
            var color = Settings.GradientKind.GetColor(Settings.StartColor, Settings.EndColor, depth,
                Settings.Iterations);

            // Хочется, чтобы толщина пера всегда была равна требуемой.
            // Поэтому рассчитаем то, насколько масштабировано пространство, а затем поделим на это число.
            // Тогда когда толщина умножится на число (при выводе на экран), то она будет как раз примерно требуемая.
            // По крайней мере идея такая.
            var scale = (float) graphics.Graphics.Transform.GetAverageScale();
            var pen = new Pen(color, (width ?? Settings.Thickness) / scale);
            return pen;
        }
    }

    /// <summary>
    /// Некоторые свойства фрактала.
    /// </summary>
    public struct FractalInfo
    {
        /// <summary>
        /// Ожидаемая ширина.
        /// Если на самом деле будет больше, то что-то не влезет на экран.
        /// Если меньше, то просто не очень красиво.
        /// </summary>
        public int Width;

        /// <summary>
        /// Ожидаемая высота. Аналогично Width.
        /// </summary>
        public int Height;

        /// <summary>
        /// Суммарное кол-во объектов которые вернёт итератор у метода Draw.
        /// Может быть приблизительным, используется для умного замедления отрисовки.
        /// </summary>
        public long TotalWork;
    }
}