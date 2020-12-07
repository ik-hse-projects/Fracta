using System;
using System.Drawing.Drawing2D;

namespace Fracta
{
    public static class TransformExt
    {
        /// <summary>
        /// Извлекает масштабирование из матрицы преобразования.
        /// </summary>
        public static (double scalex, double scaley) GetScale(this Matrix transformMatrix)
        {
            // Ссылка с формулой:
            // https://math.stackexchange.com/a/13165.

            // Ссылка с информацией о том, как хранится матрица в массиве:
            // https://docs.microsoft.com/en-US/dotnet/api/system.drawing.drawing2d.matrix

            // Хранится матрица вот так. Слева матрица, справа соответсвующие индексы массива.
            // |a  b |     |0 1|
            // |c  d |  ~  |2 3|
            // |tx ty|     |4 5|

            // Нам не нужны tx и ty.
            var a = transformMatrix.Elements[0];
            var b = transformMatrix.Elements[1];
            var c = transformMatrix.Elements[2];
            var d = transformMatrix.Elements[3];

            // Правда нас не интересует знак: он будет только мешать.
            var scalex = Math.Sqrt(a * a + b * b);
            var scaley = Math.Sqrt(c * c + d * d);

            return (scalex, scaley);
        }

        /// <summary>
        /// Вычисляет среднее масштабирование у переданной матрицы.
        /// </summary>
        public static double GetAverageScale(this Matrix transformationMatrix)
        {
            var (scalex, scaley) = transformationMatrix.GetScale();
            return (scalex + scaley) / 2;
        }
    }
}