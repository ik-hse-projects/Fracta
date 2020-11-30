using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fracta.Fractals
{
    public class Tree : Fractal
    {
        public Tree()
        {
            _settings = new TreeSettings();
        }

        public override string Name => "Фрактальное дерево";

        private TreeSettings _settings;
        public override Control Settings => _settings;

        public override void DrawOn(DrawingContext graphics)
        {
            var rect = new Rectangle(Point.Empty,
                new Size(graphics.Image.Size.Width - 1, graphics.Image.Size.Height - 1));
            graphics.Graphics.FillRectangle(new SolidBrush(Color.Chartreuse), rect);
            graphics.Graphics.DrawRectangle(Pens.Red, rect);
        }
    }

    public class TreeSettings : FlowLayoutPanel
    {
        public TreeSettings()
        {
            AutoSize = true;
            Controls.Add(new Button
            {
                Text = "Hello world"
            });
            Controls.Add(new Button
            {
                Text = "I'm test."
            });
        }
    }
}