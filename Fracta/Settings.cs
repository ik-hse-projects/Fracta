using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fracta
{
    /// <summary>
    /// Control для ввода чисел, которое удобно настривать и у которого есть Label.
    /// </summary>
    public class NumberInput : FlowLayoutPanel
    {
        /// <summary>
        /// Подпись к всему этому.
        /// </summary>
        private readonly Label label;

        /// <summary>
        /// Control, который собственно отображает число и позволяет его менять.
        /// </summary>
        private readonly NumericUpDown upDown;

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

        /// <summary>
        /// Введённое значение. Всегда в диапазоне [Minimum; Maximum].
        /// </summary>
        public decimal Value
        {
            get
            {
                var value = upDown.Value;

                if (value < Minimum)
                {
                    value = Minimum;
                }

                if (value > Maximum)
                {
                    value = Maximum;
                }

                return value;
            }
            set => upDown.Value = value;
        }

        /// <summary>
        /// Минимальное значение.
        /// </summary>
        public decimal Minimum
        {
            get => upDown.Minimum;
            set => upDown.Minimum = value;
        }

        /// <summary>
        /// Максимальное значение.
        /// </summary>
        public decimal Maximum
        {
            get => upDown.Maximum;
            set => upDown.Maximum = value;
        }

        /// <summary>
        /// Позволить ли вводить нецелые числа.
        /// </summary>
        public bool AllowFloat
        {
            get => upDown.DecimalPlaces != 0;
            set => upDown.DecimalPlaces = value ? 2 : 0;
        }

        /// <summary>
        /// Подпись к полю.
        /// </summary>
        public string Label
        {
            get => label.Text;
            set
            {
                label.Text = value;
                label.Size = new Size(label.PreferredWidth, label.PreferredHeight);
            }
        }

        /// <summary>
        /// Событие срабаывает тогда, когда значение меняется.
        /// </summary>
        public event EventHandler ValueChanged
        {
            add => upDown.ValueChanged += value;
            remove => upDown.ValueChanged -= value;
        }
    }

    /// <summary>
    /// Базовые настройки фрактала, которые есть у всех.
    /// </summary>
    public class Settings : FlowLayoutPanel
    {
        private readonly NumberInput _iterations;

        private readonly NumberInput _slowness;

        private readonly NumberInput _thickness;

        private readonly ComboBox _colorKind;

        private Color _endColor = Color.Blue;

        private Color _startColor = Color.Red;

        /// <summary>
        /// Создаёт настройки для переданного фрактала.
        /// </summary>
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
                AutoSize = true
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
                AutoSize = true
            };
            startColor.Click += (sender, args) => AskForColor(ref _startColor);
            Controls.Add(startColor);

            var endColor = new Button
            {
                Text = "Конечный цвет",
                AutoSize = true
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

            _thickness = new NumberInput
            {
                Minimum = 1,
                Maximum = 100,
                Label = "Толщина линий",
                Value = 3
            };
            Add(_thickness);
        }

        /// <summary>
        /// Кол-во итераций.
        /// </summary>
        public int Iterations => (int) _iterations.Value;

        /// <summary>
        /// Медленность отрисовки.
        /// </summary>
        public int Slowness => (int) _slowness.Value;

        /// <summary>
        /// Толщина линий.
        /// </summary>
        public float Thickness => (int) _thickness.Value;

        /// <summary>
        /// Начальный увет градиента.
        /// </summary>
        public Color StartColor => _startColor;

        /// <summary>
        /// Конечный цвет градиента.
        /// </summary>
        public Color EndColor => _endColor;

        /// <summary>
        /// Выбранный вид градиента.
        /// </summary>
        public GradientKind GradientKind => _colorKind.SelectedItem as GradientKind? ?? GradientKind.None;

        /// <summary>
        /// Событие срабатывает, когда пользователь нажимает по кнопке сохранения.
        /// </summary>
        public event EventHandler? OnSaveButtonClick;

        /// <summary>
        /// Событие срабатывает, когда меняется одно из полей.
        /// </summary>
        public event Action? ValueChanged;

        /// <summary>
        /// Добавляет NumberInput и начинает следить за изменениями числа.
        /// </summary>
        public void Add(NumberInput input)
        {
            input.ValueChanged += (sender, args) => ValueChanged?.Invoke();
            Controls.Add(input);
        }

        /// <summary>
        /// Добавляет ComboBox и начинает следить за изменениями выбранного.
        /// </summary>
        public void Add(ComboBox input)
        {
            input.SelectedValueChanged += (sender, args) => ValueChanged?.Invoke();
            Controls.Add(input);
        }

        /// <summary>
        /// Спршивает у пользователя цвет и изменяет Color.
        /// </summary>
        /// <returns>true, если пользователь что-то выбрал, и false в противном случае.</returns>
        private bool AskForColor(ref Color color)
        {
            var dialog = new ColorDialog {Color = color};
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            if (color != dialog.Color)
            {
                ValueChanged?.Invoke();
            }

            color = dialog.Color;
            return true;
        }
    }
}