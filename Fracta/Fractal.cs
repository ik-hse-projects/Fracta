using System.Drawing;
using System.Windows.Forms;
using Fracta.Fractals;

namespace Fracta
{
    public abstract class Fractal
    {
        public abstract string Name { get; }

        public virtual Control? Settings => null;

        public virtual int RecursionDepth { get; set; }

        public abstract void Draw(DrawingContext graphics, int depth);
    }
}