using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Fracta
{
    class MainForm : Form
    {
        private FractalPage[] pages;

        public MainForm()
        {
            ClientSize = new Size(200, 200);

            pages = new FractalPage[]
            {
                new FractalPage<Fractals.Tree>(),
                new FractalPage<Fractals.KochCurve>(),
                new FractalPage<Fractals.SierpinskiСarpet>(),
                new FractalPage<Fractals.SierpinskiTriangle>()
            };

            var tabs = new TabControl {Dock = DockStyle.Fill};

            tabs.TabPages.AddRange(pages);

            Controls.Add(tabs);

            ResizeEnd += (sender, args) => Redraw();

            Redraw();
        }

        private void Redraw()
        {
            foreach (var page in pages)
            {
                page.UpdateSize();
                page.Draw(true);
            }
        }

        static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}