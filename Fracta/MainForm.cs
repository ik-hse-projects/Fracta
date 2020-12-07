using System.Drawing;
using System.Windows.Forms;
using Fracta.Fractals;

namespace Fracta
{
    /// <summary>
    /// Главная формочка, в которой всё происходит.
    /// </summary>
    internal class MainForm : Form
    {
        /// <summary>
        /// Control вкладок, в котором лежат вкладки с фракталами.
        /// </summary>
        private readonly TabControl tabs;

        public MainForm()
        {
            ClientSize = new Size(200, 200);

            FractalPage[] pages =
            {
                new FractalPage<Tree>(),
                new FractalPage<KochCurve>(),
                new FractalPage<SierpinskiСarpet>(),
                new FractalPage<SierpinskiTriangle>(),
                new FractalPage<Cantor>()
            };

            tabs = new TabControl {Dock = DockStyle.Fill};
            tabs.TabPages.AddRange(pages);
            Controls.Add(tabs);

            ResizeEnd += (sender, args) => Redraw();
            tabs.Selected += (sender, args) => Redraw();

            Redraw();
        }

        /// <summary>
        /// Перерисовывает ту вкладку, которая сфокусирована на данный момент.
        /// </summary>
        private void Redraw()
        {
            if (tabs.SelectedTab is FractalPage fractalPage)
            {
                fractalPage.UpdateSize();
                fractalPage.Draw(true);
            }
        }

        /// <summary>
        /// Точка входа.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}