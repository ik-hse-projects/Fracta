using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Fracta.Fractals
{
    public class CantorCache
    {
        private readonly List<CantorState> _states;

        public CantorCache(CantorDefinition _definition)
        {
            _states = new List<CantorState>
            {
                CantorState.First(_definition)
            };
        }

        public CantorState GetState(int depth)
        {
            while (depth >= _states.Count)
            {
                _states.Add(_states.Last().Next());
            }

            return _states[depth];
        }
    }

    public struct CantorDefinition
    {
        public double Left;
        public double Right;
    }

    public class CantorState
    {
        public List<(double, double)> Intervals = new List<(double, double)>();
        private CantorDefinition _definition;

        private CantorState(CantorDefinition definition)
        {
            _definition = definition;
        }

        public static CantorState First(CantorDefinition definition)
        {
            var result = new CantorState(definition);
            result.Intervals.Add((-0.5, 0.5));
            return result;
        }

        public CantorState Next()
        {
            var result = new CantorState(_definition);
            foreach (var (start, end) in Intervals)
            {
                var length = end - start;
                var leftEnd = start + length * _definition.Left;
                var rightStart = end - length * _definition.Right;

                result.Intervals.Add((start, leftEnd));
                result.Intervals.Add((rightStart, end));
            }

            return result;
        }

        public IEnumerable Draw(DrawingContext graphics, Pen pen, double width = 1)
        {
            foreach (var (start, end) in Intervals)
            {
                graphics.Graphics.DrawLine(pen, (float) (start * width), 0, (float) (end * width), 0);
                yield return new object();
            }
        }
    }
}