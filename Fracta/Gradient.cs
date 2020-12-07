using System;
using System.Drawing;
using System.Linq;

namespace Fracta
{
    /// <summary>
    /// Вид градиента.
    /// </summary>
    public enum GradientKind
    {
        Usual,
        HSV,
        Static,
        None
    }

    /// <summary>
    /// Реализация градиентов.
    /// </summary>
    public static class Gradient
    {
        /// <summary>
        /// Табличка довольно разных цветов. Отлично помогает при отладке.
        /// Источник: https://stackoverflow.com/a/20298116
        /// </summary>
        private static readonly Color[] colortable = new[]
        {
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
        }.Select(ColorTranslator.FromHtml).ToArray();

        /// <summary>
        /// Вычисляет цвет градиента указанного вида, который
        /// начинается с цвета <paramref name="a" /> и заканчивается цветом <paramref name="b" />.
        /// Позиция в градиенте вычисляется по значению дроби <see cref="numerator" />:<see cref="denominator" />
        /// </summary>
        public static Color GetColor(this GradientKind kind, Color a, Color b, int numerator, int denominator)
        {
            var position = (double) numerator / denominator;
            return kind switch
            {
                GradientKind.Usual => LinearGradient(a, b, position),
                GradientKind.HSV => HsvGradient(a, b, position),
                GradientKind.Static => StaticColor(numerator),
                GradientKind.None => a,
                _ => b
            };
        }

        private static double TransformNumber(double start, double end, double position)
        {
            return start + (end - start) * position;
        }

        /// <summary>
        /// Вычисляет цвет для линейного RGB-градиента.
        /// </summary>
        public static Color LinearGradient(Color a, Color b, double position)
        {
            return Color.FromArgb(
                (int) TransformNumber(a.R, b.R, position),
                (int) TransformNumber(a.G, b.B, position),
                (int) TransformNumber(a.G, b.B, position));
        }

        /// <summary>
        /// Вычисляет цвет для линейного HSV-градиента.
        /// </summary>
        public static Color HsvGradient(Color a, Color b, double position)
        {
            ColorToHSV(a, out var aHue, out var aSaturation, out var aValue);
            ColorToHSV(b, out var bHue, out var bSaturation, out var bValue);
            return ColorFromHSV(
                TransformNumber(aHue, bHue, position),
                TransformNumber(aSaturation, bSaturation, position),
                TransformNumber(aValue, bValue, position)
            );
        }

        /// <summary>
        /// Какой-то цвет по его номеру, ни капли не градиент.
        /// </summary>
        public static Color StaticColor(int number)
        {
            return colortable[number % colortable.Length];
        }

        // Далее немного адаптированная пара функций отсюда: https://stackoverflow.com/a/1626175

        /// <summary>
        /// Преобразовывает цвет в HSV.
        /// </summary>
        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = max == 0 ? 0 : 1d - 1d * min / max;
            value = max / 255d;
        }

        /// <summary>
        /// Создает цвет из HSV.
        /// </summary>
        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => Color.FromArgb(255, v, t, p),
                1 => Color.FromArgb(255, q, v, p),
                2 => Color.FromArgb(255, p, v, t),
                3 => Color.FromArgb(255, p, q, v),
                4 => Color.FromArgb(255, t, p, v),
                _ => Color.FromArgb(255, v, p, q)
            };
        }
    }
}