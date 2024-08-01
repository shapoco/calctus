﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Functions.BuiltIns;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.UI.Sheets {
    class SheetViewItem : GdiBox, IDisposable {
        private static readonly char[] _selectionCancelChars = OpInfo.Items
                .Select(p => p.Symbol[0])
                .Distinct()
                .ToArray();

        private SheetView _view;
        public readonly SheetItem SheetItem;
        public readonly ExprBoxCore ExprBox;
        public readonly ExprBoxCore AnsBox;
        public readonly EqualButton Equal;
        public bool IsFreshAnswer = false;
        private bool _isRpnOperand = false;
        private bool _disposed = false;
        private Size _lastPreferredSize = Size.Empty;
        private bool _ignoreExprChanged = false;

        public SheetViewItem(SheetView view, SheetItem bookItem) : base(view) {
            _view = view;
            SheetItem = bookItem;
            ExprBox = new ExprBoxCore(view);
            AnsBox = new ExprBoxCore(view);
            Equal = new EqualButton(view);
            AnsBox.ReadOnly = true;
            ExprBox.Text = bookItem.ExprText;
            ExprBox.InputCandidateProvider = view;
            ExprBox.TextChanged += ExprBox_TextChanged;
            ExprBox.KeyDown += ExprBox_KeyDown;
            ExprBox.KeyPress += ExprBox_KeyPress;
            bookItem.ExpressionChanged += BookItem_ExpressionChanged;
            bookItem.AnswerChanged += BookItem_AnswerChanged;
            Children.Add(ExprBox);
            Children.Add(AnsBox);
            Children.Add(Equal);
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    SheetItem.ExpressionChanged -= BookItem_ExpressionChanged;
                    SheetItem.AnswerChanged -= BookItem_AnswerChanged;
                    Children.Clear();
                    ExprBox.Dispose();
                    AnsBox.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public bool IsRpnOperand {
            get => _isRpnOperand;
            set {
                if (value == _isRpnOperand) return;
                _isRpnOperand = value;
                Invalidate();
            }
        }

        public void RelayoutText() {
            ExprBox.RelayoutText();
            AnsBox.RelayoutText();
        }

        public void ReplaceFormatterFunction(FuncDef newFunc) {
            var funcs = RepresentaionFuncs.Instance.Functions.Select(p => p.Name.Text).ToArray();
            var prefixPattern = new Regex(@"^ *(?<func>" + String.Join("|", funcs) + @") *\( *(?<body>.+)");
            var suffixPattern = new Regex(@" *\)$");
            var pm = prefixPattern.Match(ExprBox.Text);
            var body = ExprBox.Text;
            var selStart = ExprBox.SelectionStart;
            var selLength = ExprBox.SelectionLength;
            if (pm.Success) {
                var oldFuncName = pm.Groups["func"].Value;
                if (funcs.Contains(oldFuncName)) {
                    body = pm.Groups["body"].Value;
                    selStart -= pm.Groups["body"].Index;
                    var sm = suffixPattern.Match(body);
                    if (sm.Success) {
                        body = body.Substring(0, body.Length - sm.Length);
                    }
                }
            }
            if (newFunc == null) {
                ExprBox.Text = body;
                ExprBox.SelectionStart = selStart;
                ExprBox.SelectionLength = selLength;
            }
            else {
                ExprBox.Text = newFunc.Name.Text + "(" + body + ")";
                ExprBox.SelectionStart = newFunc.Name.Text.Length + 1 + selStart;
                ExprBox.SelectionLength = selLength;
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            var g = e.Graphics;
            var s = Settings.Instance;
            if (_isRpnOperand) {
                using (var pen = new Pen(s.Appearance_Color_RPN_Target, 2))
                using (var brush = new SolidBrush(Color.FromArgb(128, s.Appearance_Color_RPN_Target))) {
                    g.FillRectangle(brush, ClientBounds);
                    var rect = ClientBounds;
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private void ExprBox_TextChanged(object sender, EventArgs e) {
            if (_ignoreExprChanged) return;
            _view.Operator.ChangeExpression(_view.IndexOf(this), ExprBox.Text);
            IsFreshAnswer = false;
            var prefSize = GetPreferredSize();
            if (prefSize != _lastPreferredSize) {
                _view.InvalidateLayout();
                _lastPreferredSize = prefSize;
            }
        }

        private void ExprBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Home) {
                var s = Settings.Instance;
                if (s.Input_AutoInputAns && IsFreshAnswer && ExprBox.SelectionLength == ExprBox.Text.Length) {
                    ExprBox.Text = Sheet.LastAnsId;
                    ExprBox.SelectionStart = 0;
                    ExprBox.SelectionLength = 0;
                    e.Handled = true;
                }
            }
        }

        private void ExprBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (_selectionCancelChars.Contains(e.KeyChar)) {
                var s = Settings.Instance;
                if (s.Input_AutoInputAns && IsFreshAnswer && ExprBox.SelectionLength == ExprBox.Text.Length) {
                    ExprBox.Text = Sheet.LastAnsId;
                    ExprBox.SelectionStart = Sheet.LastAnsId.Length;
                    ExprBox.SelectionLength = 0;
                    e.Handled = true;
                }
            }
        }

        private void BookItem_ExpressionChanged(object sender, EventArgs e) {
            _ignoreExprChanged = true;
            var err = SheetItem.SyntaxError != null ? SheetItem.SyntaxError : SheetItem.EvalError;
            ExprBox.Text = SheetItem.ExprText;
            updateAnswerBox();
            _ignoreExprChanged = false;
        }

        private void BookItem_AnswerChanged(object sender, EventArgs e) {
            updateAnswerBox();
        }

        private void updateAnswerBox() {
            Exception err = null;
            if (SheetItem.SyntaxError != null) {
                err = SheetItem.SyntaxError;
                ExprBox.EvalError = null;
            }
            else {
                err = SheetItem.EvalError;
                ExprBox.EvalError = SheetItem.EvalError;
            }
            bool ansVisible;
            if (err == null) {
                var ansVal = SheetItem.AnsVal;
                var ansText = SheetItem.AnsText;
                ansText = (ansVal != null && ansVal.IsSerializable) ? ansText : "";
                AnsBox.Text = ansText;
                AnsBox.PlaceHolder = "";
                ansVisible = SheetItem.ExprTree.CausesValueChange() && !(SheetItem.AnsVal is NullVal) && !(SheetItem.AnsVal is FuncVal);
            }
            else {
                AnsBox.Text = "";
                AnsBox.PlaceHolder = "? " + err.Message;
                ansVisible = true;
            }
            if (AnsBox.Visible != ansVisible) {
                AnsBox.Visible = Equal.Visible = ansVisible;
                relayout();
                _view.InvalidateLayout();
            }
        }

        public override Size GetPreferredSize() {
            var exprSize = ExprBox.GetPreferredSize();
            var ansSize = AnsBox.GetPreferredSize();
            var indent = _view.EqualPosition;
            if (exprSize.Width <= indent || !AnsBox.Visible) {
                return new Size(exprSize.Width + ansSize.Width, Math.Max(exprSize.Height, ansSize.Height));
            }
            else {
                return new Size(Math.Max(exprSize.Width, ansSize.Width), exprSize.Height + ansSize.Height);
            }
        }

        protected override void OnResize() {
            base.OnResize();
            relayout();
        }

        public void Relayout() => relayout();

        private void relayout() {
            var exprSize = ExprBox.GetPreferredSize();
            var indent = _view.EqualPosition;
            var equalWidth = _view.EqualWidth;
            var ansLeft = indent + equalWidth;
            if (!AnsBox.Visible) {
                ExprBox.SetBounds(0, 0, Width, Height);
            }
            else if (Height < exprSize.Height * 3 / 2) {
                ExprBox.SetBounds(0, 0, indent, Height);
                Equal.SetBounds(indent, 0, equalWidth, Height);
                AnsBox.SetBounds(ansLeft, 0, Width - ansLeft, Height);
            }
            else {
                ExprBox.SetBounds(0, 0, Width, Height / 2);
                Equal.SetBounds(indent, ExprBox.Height, equalWidth, ExprBox.Height);
                AnsBox.SetBounds(ansLeft, ExprBox.Height, Width - ansLeft, Height - ExprBox.Height);
            }
        }

    }
}
