using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Fracta.Fractals
{
    /// <summary>
    /// Вычисляет, кэширует и рисует множество Кантора.
    /// </summary>
    public class CantorCache
    {
        /// <summary>
        /// Кэш.
        /// </summary>
        private readonly List<CantorState> _states;

        /// <summary>
        /// Создаёт CantorCache для мн-ва Кантора с указанными параметрами.
        /// </summary>
        public CantorCache(CantorDefinition settings)
        {
            _states = new List<CantorState>
            {
                CantorState.First(settings)
            };
        }

        /// <summary>
        /// Возвращает вычисленные параметры для мн-ва Кантора с указанным числом итераций.
        /// </summary>
        public CantorState GetState(int depth)
        {
            while (depth >= _states.Count)
            {
                _states.Add(_states.Last().Next());
            }

            return _states[depth];
        }
    }

    /// <summary>
    /// Параметры мн-ва Кантора.
    /// </summary>
    public struct CantorDefinition
    {
        /// <summary>
        /// Длина отрезка слева. Число от 0 до 1.
        /// </summary>
        public double Left;

        /// <summary>
        /// Длина отрезка справа. Число от 0 до 1.
        /// </summary>
        public double Right;
    }

    /// <summary>
    /// Множество Кантора на некоторой итерации.
    /// </summary>
    public class CantorState
    {
        /// <summary>
        /// Параметры мн-ва.
        /// </summary>
        private readonly CantorDefinition _definition;

        /// <summary>
        /// Отрезки у этой итерации множества. Все числа в диапазоне [-0.5, 0.5].
        /// </summary>
        private readonly List<(double, double)> _intervals = new List<(double, double)>();

        private CantorState(CantorDefinition definition)
        {
            _definition = definition;
        }

        /// <summary>
        /// Создает первое (нулевое) мн-во. Представляет из себя просто закрашенную линию.
        /// </summary>
        public static CantorState First(CantorDefinition definition)
        {
            var result = new CantorState(definition);
            result._intervals.Add((-0.5, 0.5));
            return result;
        }

        /// <summary>
        /// Вычисляет следующую итерацию мн-ва и возвращает её.
        /// </summary>
        public CantorState Next()
        {
            var result = new CantorState(_definition);
            foreach (var (start, end) in _intervals)
            {
                var length = end - start;
                var leftEnd = start + length * _definition.Left;
                var rightStart = end - length * _definition.Right;

                result._intervals.Add((start, leftEnd));
                result._intervals.Add((rightStart, end));
            }

            return result;
        }

        /// <summary>
        /// Отрисовывает мн-во в нуле координат переданным пером.
        /// </summary>
        /// <param name="width">Ширина ожидаемого мн-ва</param>
        /// <returns>См. <see cref="Fractal.Draw" /></returns>
        public IEnumerable Draw(DrawingContext graphics, Pen pen, double width = 1)
        {
            foreach (var (start, end) in _intervals)
            {
                graphics.Graphics.DrawLine(pen, (float) (start * width), 0, (float) (end * width), 0);
                yield return new object();
            }
        }
    }
}