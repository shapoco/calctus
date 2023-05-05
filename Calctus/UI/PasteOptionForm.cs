using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus.UI {
    public partial class PasteOptionForm : Form {
        public PasteOptionForm() {
            InitializeComponent();

            try {
                this.Font = new Font("Arial", SystemFonts.DefaultFont.Size);
            }
            catch { }

            Load += PasteOptionForm_Load;
            columnDelimiterText.TextChanged += ColumnDelimiterText_TextChanged;
            columnNumberText.TextChanged += ColumnNumberText_TextChanged;
            selectColumnButton.Click += SelectColumnButton_Click;
            removeCommaButton.Click += RemoveCommaButton_Click;
            removeRightHandsButton.Click += RemoveRightHandsButton_Click;
        }

        public string TextWillBePasted => textWillBePasted.Text;

        private void PasteOptionForm_Load(object sender, EventArgs e) {
            try {
                sourceText.Text = Clipboard.GetText();
            }
            catch {
                sourceText.Text = "(Clipboard.GetText() failed.)";
            }

            var text = sourceText.Text;

            if (text.IndexOf('\t') >= 0) {
                columnDelimiterText.Text = @"\t";
            }
            else if (text.IndexOf('|') >= 0) {
                columnDelimiterText.Text = "|";
            }
            else if (text.IndexOf(',') >= 0) {
                columnDelimiterText.Text = ",";
            }
            else {
                columnDelimiterText.Text = " ";
            }
            textWillBePasted.Text = sourceText.Text;
        }

        private void ColumnDelimiterText_TextChanged(object sender, EventArgs e) {
            var delimiter = getDelimiter();
            var numCols = 0;
            foreach (var line in sourceText.Text.Split('\n')) {
                var cols = splitLine(line, delimiter);
                numCols = Math.Max(numCols, cols.Length);
            }
            numColumnsLabel.Text = "/ " + numCols;
        }

        private void ColumnNumberText_TextChanged(object sender, EventArgs e) {
            bool enable = false;
            if (int.TryParse(columnNumberText.Text, out int n)) {
                enable = n > 0;
            }
            selectColumnButton.Enabled = enable;
        }

        private void SelectColumnButton_Click(object sender, EventArgs e) {
            var delimiter = getDelimiter();
            var sb = new StringBuilder();
            try {
                var colIndex = int.Parse(columnNumberText.Text) - 1;
                foreach (var l in sourceText.Text.Split('\n')) {
                    var line = l.Replace("\r", "");
                    line = line.TrimEnd();
                    if (string.IsNullOrEmpty(line)) continue;
                    var cols = splitLine(line, delimiter);
                    if (colIndex < cols.Length ) {
                        if (sb.Length > 0) sb.AppendLine();
                        sb.Append(cols[colIndex].Trim());
                    }
                }
            }
            catch { }
            textWillBePasted.Text = sb.ToString();
        }

        private char getDelimiter() {
            var delimiterStr = columnDelimiterText.Text.Replace(@"\t", "\t");
            if (delimiterStr.Length != 1) {
                return '\t';
            }
            return delimiterStr[0];
        }

        private string[] splitLine(string line , char delimiter) {
            if (delimiter == ' ') {
                return line.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            }
            else {
                return line.Split(delimiter);
            }
        }

        private void RemoveCommaButton_Click(object sender, EventArgs e) {
            textWillBePasted.Text = textWillBePasted.Text.Replace(",", "");
        }

        private void RemoveRightHandsButton_Click(object sender, EventArgs e) {
            var sb = new StringBuilder();
            foreach (var l in textWillBePasted.Text.Split('\n')) {
                var line = l.Replace("\r", "");
                line = line.TrimEnd();

                int equalPos = line.LastIndexOf('=');
                if (equalPos >= 0) {
                    line = line.Substring(0, equalPos);
                }

                if (sb.Length > 0) sb.AppendLine();
                sb.Append(line);
            }
            textWillBePasted.Text = sb.ToString();
        }

    }
}
