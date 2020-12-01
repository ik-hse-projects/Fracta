using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fracta
{
    public class NumberInput : FlowLayoutPanel
    {
        public decimal Value
        {
            get => upDown.Value;
            set => upDown.Value = value;
        }

        public decimal Minimum
        {
            get => upDown.Minimum;
            set => upDown.Minimum = value;
        }

        public decimal Maximum
        {
            get => upDown.Maximum;
            set => upDown.Maximum = value;
        }

        public bool AllowFloat
        {
            get => upDown.DecimalPlaces != 0;
            set => upDown.DecimalPlaces = value ? 2 : 0;
        }

        public string Label
        {
            get => label.Text;
            set
            {
                label.Text = value;
                label.Size = new Size(label.PreferredWidth, label.PreferredHeight);
            }
        }

        public event EventHandler ValueChanged
        {
            add => upDown.ValueChanged += value;
            remove => upDown.ValueChanged -= value;
        }

        private NumericUpDown upDown;
        private Label label;

        public NumberInput()
        {
            upDown = new NumericUpDown();
            label = new Label();

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FlowDirection = FlowDirection.LeftToRight;
            BorderStyle = BorderStyle.Fixed3D;

            Controls.Add(label);
            Controls.Add(upDown);
        }
    }

    public class Settings : FlowLayoutPanel
    {
        public int Iterations => (int) _iterations.Value;
        public float Width => (int) _width.Value;
        
        private NumberInput _iterations;
        private NumberInput _width;

        public Settings()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoScroll = true;
            Dock = DockStyle.Top;
            MaximumSize = new Size(0, 100);
            FlowDirection = FlowDirection.TopDown;
            _iterations = new NumberInput
            {
                Minimum = 1,
                Maximum = 12,
                Label = "число итераций"
            };
            Add(_iterations);
            
            _width = new NumberInput
            {
                Minimum = 1,
                Maximum = 100,
                Label = "Толщина линий",
                Value = 3,
            };
            Add(_width);
        }

        public event EventHandler? ValueChanged;

        public void Add(NumberInput input)
        {
            input.ValueChanged += (sender, args) => ValueChanged?.Invoke(sender, args);
            Controls.Add(input);
        }
    }
}