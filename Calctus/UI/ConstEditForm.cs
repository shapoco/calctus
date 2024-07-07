using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.UI {
    partial class ConstEditForm : Form {
        public readonly Regex IdRegex = new Regex("^" + Lexer.IdPattern + "$");
        private UserConstant _target;

        public ConstEditForm() {
            InitializeComponent();

            Windows.DwmApi.SetDarkModeEnable(this, Settings.Instance.GetIsDarkMode());

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            id.TextChanged += TextBox_TextChanged;
            valueStr.TextChanged += TextBox_TextChanged;
            desc.TextChanged += TextBox_TextChanged;
            closeButton.Click += CloseButton_Click;
        }

        public UserConstant Target {
            get => _target;
            set {
                if (_target == value) return;
                _target = value;
                if (_target != null) {
                    id.Text = _target.Id;
                    valueStr.Text = _target.ValueString;
                    desc.Text = _target.Description;
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e) {
            var idOk = IdRegex.IsMatch(id.Text);
            id.BackColor = idOk ? SystemColors.Window : Color.Yellow;

            var valueOk = false;
            try {
                var ctx = new EvalContext();
                var expr = Parser.Parse(valueStr.Text);
                expr.Eval(ctx);
                valueOk = true;
            } catch { }
            valueStr.BackColor = valueOk ? SystemColors.Window : Color.Yellow;

            closeButton.Enabled = idOk && valueOk;
        }

        private void CloseButton_Click(object sender, EventArgs e) {
            if (!closeButton.Enabled) return;
            if (_target == null) return;
            _target.Id = id.Text;
            _target.ValueString = valueStr.Text;
            _target.Description = desc.Text;
        }

    }
}
