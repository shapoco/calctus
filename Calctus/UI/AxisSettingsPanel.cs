using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.UI {
    class AxisSettingsPanel : ContainerControl {
        public readonly Label Label = new Label();
        private ComboBox axisType = new ComboBox();
        private TextBox topValue = new TextBox();
        private TextBox bottomValue = new TextBox();
        private AxisSettings _axisSettings;
        private bool _propChanging = false;
        private FormatHint _topFormatHint = null;
        private FormatHint _bottomFormatHint = null;

        public AxisSettingsPanel() {
            Label.Dock = DockStyle.Top;
            Label.TextAlign = ContentAlignment.MiddleLeft;
            axisType.IntegralHeight = false;
            axisType.Dock = DockStyle.Top;
            axisType.DropDownStyle = ComboBoxStyle.DropDownList;
            topValue.Dock = DockStyle.Top;
            topValue.TextAlign = HorizontalAlignment.Right;
            bottomValue.Dock = DockStyle.Top;
            bottomValue.TextAlign = HorizontalAlignment.Right;
            Controls.Add(bottomValue);
            Controls.Add(topValue);
            Controls.Add(axisType);
            Controls.Add(Label);
            var tabIndex = 0;
            Label.TabIndex = tabIndex++;
            axisType.TabIndex = tabIndex++;
            topValue.TabIndex = tabIndex++;
            bottomValue.TabIndex = tabIndex++;

            if (DesignMode) return;

            foreach (var type in Enum.GetValues(typeof(AxisType))) {
                axisType.Items.Add(type);
            }

            axisType.SelectedIndexChanged += AxisType_SelectedIndexChanged;
            topValue.TextChanged += TopBottomValue_TextChanged;
            bottomValue.TextChanged += TopBottomValue_TextChanged;

            AxisSettings = new AxisSettings();
        }

        public override Size GetPreferredSize(Size proposedSize) {
            using (var g = this.CreateGraphics()) {
                return new Size(
                    (int)(Math.Ceiling(g.MeasureString("Axis", this.Font).Width * 3)), 
                    Label.Height + axisType.Height + topValue.Height + bottomValue.Height);
            }
        }

        public AxisSettings AxisSettings {
            get => _axisSettings;
            set {
                if (value == _axisSettings) return;
                if (_axisSettings != null) {
                    _axisSettings.Changed -= AxisSettings_Changed;
                }
                _axisSettings = value;
                if (_axisSettings != null) {
                    _axisSettings.Changed += AxisSettings_Changed;
                }
                reloadSettings();
            }
        }

        private void AxisType_SelectedIndexChanged(object sender, EventArgs e) {
            if (_propChanging) return;
            _propChanging = true;
            _axisSettings.Type = (AxisType)((ComboBox)sender).SelectedItem;
            _propChanging = false;
        }

        private void TopBottomValue_TextChanged(object sender, EventArgs e) {
            if (_propChanging) return;
            try {
                var top = textToPos(topValue.Text, ref _topFormatHint);
                var bottom = textToPos(bottomValue.Text, ref _bottomFormatHint);
                if (top <= bottom) throw new ArgumentException();
                if (bottom < _axisSettings.PosMin) throw new ArgumentException();
                if (top > _axisSettings.PosMax) throw new ArgumentException();
                _propChanging = true;
                _axisSettings.PosBottom = bottom;
                _axisSettings.PosRange = top - bottom;
                topValue.BackColor = SystemColors.Window;
                bottomValue.BackColor = SystemColors.Window;
            }
            catch {
                topValue.BackColor = Color.Yellow;
                bottomValue.BackColor = Color.Yellow;
            }
            _propChanging = false;
        }

        private decimal textToPos(string text, ref FormatHint formatHint) {
            var val = Model.Parsers.Parser.Parse(text).Eval(new Model.Evaluations.EvalContext());
            formatHint = val.FormatHint;
            if (_axisSettings.ValueToPos(val.AsReal, out decimal pos)) {
                return pos;
            }
            else {
                throw new ArgumentException();
            }
        }

        private void AxisSettings_Changed(object sender, EventArgs e) {
            reloadSettings();
        }

        private void reloadSettings() {
            if (_propChanging) return;
            _propChanging = true;
            axisType.SelectedItem = _axisSettings.Type;
            posToText(_axisSettings.PosTop, _topFormatHint, topValue);
            posToText(_axisSettings.PosBottom, _bottomFormatHint, bottomValue);
            _propChanging = false;
        }

        private void posToText(decimal pos, FormatHint formatHint, TextBox textBox) {
            try {
                Val val = new RealVal(_axisSettings.PosToValue(pos));
                if (formatHint != null) val = val.Format(formatHint);
                textBox.Text = val.ToString();
            }
            catch {
                textBox.Text = "";
            }
        }

    }
}
