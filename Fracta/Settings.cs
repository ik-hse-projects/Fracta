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
        public int Slowness => (int) _slowness.Value;
        public float Width => (int) _width.Value;

        private readonly NumberInput _iterations;
        private readonly NumberInput _slowness;
        private readonly NumberInput _width;

        public Color StartColor => _startColor;
        public Color EndColor => _endColor;
        public GradientKind GradientKind => (_colorKind.SelectedItem as GradientKind?) ?? GradientKind.None;

        private Color _startColor = Color.Red;
        private Color _endColor = Color.Blue;
        private ComboBox _colorKind;

        public event EventHandler? OnSaveButtonClick;

        public Settings(Fractal fractal)
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoScroll = true;
            Dock = DockStyle.Top;
            MaximumSize = new Size(0, 100);
            FlowDirection = FlowDirection.TopDown;
            Button saveButton = new Button
            {
                Text = "Сохранить",
                AutoSize = true,
            };
            saveButton.Click += (sender, e) => OnSaveButtonClick?.Invoke(sender, e);
            Controls.Add(saveButton);

            _colorKind = new ComboBox();
            _colorKind.Items.Add(GradientKind.None);
            _colorKind.Items.Add(GradientKind.Usual);
            _colorKind.Items.Add(GradientKind.HSV);
            _colorKind.Items.Add(GradientKind.Static);
            _colorKind.SelectedIndex = 2;
            Add(_colorKind);

            var startColor = new Button
            {
                Text = "Начальный цвет",
                AutoSize = true,
            };
            startColor.Click += (sender, args) => AskForColor(ref _startColor);
            Controls.Add(startColor);
            
            var endColor = new Button
            {
                Text = "Конечный цвет",
                AutoSize = true,
            };
            endColor.Click += (sender, args) => AskForColor(ref _endColor);
            Controls.Add(endColor);

            _iterations = new NumberInput
            {
                Minimum = 1,
                Maximum = fractal.MaxIterations,
                Value = 3,
                Label = "Число итераций"
            };
            Add(_iterations);

            _slowness = new NumberInput
            {
                Minimum = 1,
                Maximum = 1 << 16,
                Value = 100,
                Label = "Медленность отрисовки"
            };
            Add(_slowness);

            _width = new NumberInput
            {
                Minimum = 1,
                Maximum = 100,
                Label = "Толщина линий",
                Value = 3,
            };
            Add(_width);
        }

        public event Action? ValueChanged;

        public void Add(NumberInput input)
        {
            input.ValueChanged += (sender, args) => ValueChanged?.Invoke();
            Controls.Add(input);
        }

        public void Add(ComboBox input)
        {
            input.SelectedValueChanged += (sender, args) => ValueChanged?.Invoke();
            Controls.Add(input);
        }

        private void AskForColor(ref Color color)
        {
            var dialog = new ColorDialog {Color = color};
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (color != dialog.Color)
                {
                    ValueChanged?.Invoke();
                }

                color = dialog.Color;
            }
        }
    }
}