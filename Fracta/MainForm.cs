using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Fracta
{
    class MainForm : Form
    {
        private FractalPage[] pages;
        private TabControl tabs;

        public MainForm()
        {
            ClientSize = new Size(200, 200);

            pages = new FractalPage[]
            {
                new FractalPage<Fractals.Tree>(),
                new FractalPage<Fractals.KochCurve>(),
                new FractalPage<Fractals.SierpinskiСarpet>(),
                new FractalPage<Fractals.SierpinskiTriangle>(),
                new FractalPage<Fractals.Cantor>(),
            };

            tabs = new TabControl {Dock = DockStyle.Fill};

            tabs.TabPages.AddRange(pages);

            Controls.Add(tabs);

            ResizeEnd += (sender, args) => Redraw();
            tabs.Selected += (sender, args) => Redraw();

            Redraw();
        }

        private void Redraw()
        {
            if (tabs.SelectedTab is FractalPage fractalPage)
            {
                fractalPage.UpdateSize();
                fractalPage.Draw(true);
            }
        }

        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}