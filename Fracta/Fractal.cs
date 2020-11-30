using System.Drawing;
using System.Windows.Forms;
using Fracta.Fractals;

namespace Fracta
{
    public abstract class Fractal
    {
        public abstract string Name { get; }

        private Settings? defaultSettings;
        public virtual Settings Settings => defaultSettings ??= new Settings();

        public int RecursionDepth => Settings.Iterations;

        public abstract void Draw(DrawingContext graphics, int depth);
    }
}