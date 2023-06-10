using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Graphs;

namespace Shapoco.Calctus.UI {
    class GraphPanel : Panel {
        private static readonly string prefixes = "ryzafpnum_kMGTPEZYR";
        private const int prefixOffset = 9;
        private const int GraphAreaMargin = 20;

        private Dictionary<Sheet, Graph[]> _graphs = new Dictionary<Sheet, Graph[]>();
        public readonly PlotSettings PlotSettings = new PlotSettings();
        private Plotter _plotter = new Plotter();

        private bool _layoutInvalidated = true;
        private int _yScaleWidth = 0;
        private int _xScaleHeight = 0;

        private DragMode _dragMode = DragMode.Idle;
        private Point _mouseLastMovePos = Point.Empty;
        private Point _mouseLastDownPos = Point.Empty;
        private MouseButtons _mousePressedButtons = MouseButtons.None;

        private bool _invertBrightness = false;

        public GraphPanel() {
            if (DesignMode) return;
            DoubleBuffered = true;
            _plotter.SynchronizingObject = this;
            _plotter.Plotted += _worker_Plotted;
        }

        public bool InvertBrightness {
            get => _invertBrightness;
            set {
                if (value == _invertBrightness) return;
                _invertBrightness = value;
                Invalidate();
            }
        }

        /// <summary>グラフ描画をリクエストする</summary>
        public void StartPlot(Sheet sheet, PlotCall[] calls) {
            var ps = PlotSettings;
            var graphArea = getGraphArea();
            if (graphArea.Width > 0) {
                ps.XNumSteps = graphArea.Width;
            }
            _plotter.StartPlot(new PlotRequest(sheet, calls, PlotSettings));
        }

        public void Replot() {
            foreach (var sheet in _graphs.Keys.ToArray()) {
                StartPlot(sheet, _graphs[sheet].Select(p => p.Call).ToArray());
            }
        }

        public void Copy() {
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);
            using (var g = Graphics.FromImage(bmp)) {
                render(g);
            }
            try {
                Clipboard.SetImage(bmp);
            }
            catch {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void _worker_Plotted(Plotter sender, PlottedEventArgs e) {
            // プロット結果を保持
            if (e.Graphs.Length == 0) {
                _graphs.Remove(e.Sheet);
            }
            else {
                _graphs[e.Sheet] = e.Graphs;
            }
            Invalidate();
        }

        protected override void OnResize(EventArgs eventargs) {
            base.OnResize(eventargs);
            Replot();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            var ps = PlotSettings;
            var graphArea = getGraphArea();
            _mouseLastDownPos = e.Location;
            _mousePressedButtons |= e.Button;

            _dragMode = DragMode.Idle;
            switch (_mousePressedButtons) {
                case MouseButtons.Left: 
                    if (getGraphArea().Contains(e.Location)) {
                        _dragMode = DragMode.XYScroll;
                    }
                    else if (getXScaleArea().Contains(e.Location)) {
                        _dragMode = DragMode.XScroll;
                    }
                    else if (getYScaleArea().Contains(e.Location)) {
                        _dragMode = DragMode.YScroll;
                    }
                    break;
                case MouseButtons.Right:
                    if (getGraphArea().Contains(e.Location)) {
                        _dragMode = DragMode.ZoomSelection;
                    }
                    break;
                case MouseButtons.Middle:
                    if (getGraphArea().Contains(e.Location)) {
                        _dragMode = DragMode.XYScroll;
                    }
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (_dragMode != DragMode.Idle) {
                bool xScroll = _dragMode == DragMode.XYScroll || _dragMode == DragMode.XScroll;
                bool yScroll = _dragMode == DragMode.XYScroll || _dragMode == DragMode.YScroll;
                if (xScroll || yScroll) {
                    var ps = PlotSettings;
                    var graphArea = getGraphArea();
                    var dx = xScroll ? (ps.XMax - ps.XMin) * (e.X - _mouseLastMovePos.X) / graphArea.Width : 0m;
                    var dy = yScroll ? (ps.YMax - ps.YMin) * (_mouseLastMovePos.Y - e.Y) / graphArea.Height : 0m;
                    ps.XMin -= dx;
                    ps.XMax -= dx;
                    ps.YMin -= dy;
                    ps.YMax -= dy;
                    invalidateLayout();
                    Replot();
                }
                Invalidate();
            }
            _mouseLastMovePos = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            if (_dragMode == DragMode.ZoomSelection) {
                var px0 = Math.Min(_mouseLastDownPos.X, _mouseLastMovePos.X);
                var py0 = Math.Min(_mouseLastDownPos.Y, _mouseLastMovePos.Y);
                var px1 = Math.Max(_mouseLastDownPos.X, _mouseLastMovePos.X);
                var py1 = Math.Max(_mouseLastDownPos.Y, _mouseLastMovePos.Y);
                if (px1 - px0 >= 5 || py1 - py0 >= 5) {
                    // 選択された領域を拡大する
                    var ps = PlotSettings;
                    var graphArea = getGraphArea();
                    var xMin = ps.XMin + (ps.XMax - ps.XMin) * (decimal)(px0 - graphArea.X) / graphArea.Width;
                    var yMin = ps.YMin + (ps.YMax - ps.YMin) * (decimal)(graphArea.Bottom - py1) / graphArea.Height;
                    var xMax = ps.XMin + (ps.XMax - ps.XMin) * (decimal)(px1 - graphArea.X) / graphArea.Width;
                    var yMax = ps.YMin + (ps.YMax - ps.YMin) * (decimal)(graphArea.Bottom - py0) / graphArea.Height;
                    ps.XMin = xMin;
                    ps.XMax = xMax;
                    ps.YMin = yMin;
                    ps.YMax = yMax;
                    invalidateLayout();
                    Replot();
                }
            }
            _mousePressedButtons = MouseButtons.None;
            _dragMode = DragMode.Idle;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if (e.Delta == 0) return;

            if (ModifierKeys == Keys.None) {
                if (getGraphArea().Contains(e.Location)) {
                    yScroll(e.Delta);
                }
                else if (getXScaleArea().Contains(e.Location)) {
                    xScroll(e.Delta);
                    Replot();
                }
                else if (getYScaleArea().Contains(e.Location)) {
                    yScroll(e.Delta);
                }
            }
            else if (ModifierKeys == Keys.Shift) {
                if (getGraphArea().Contains(e.Location)) {
                    xScroll(e.Delta);
                    Replot();
                }
            }
            else if (ModifierKeys == Keys.Control) {
                if (getGraphArea().Contains(e.Location)) {
                    xZoom(e.X, e.Delta);
                    yZoom(e.Y, e.Delta);
                    Replot();
                }
                else if (getXScaleArea().Contains(e.Location)) {
                    xZoom(e.X, e.Delta);
                    Replot();
                }
                else if (getYScaleArea().Contains(e.Location)) {
                    yZoom(e.Y, e.Delta);
                    Replot();
                }
            }
            
        }

        private void xScroll(int delta) {
            var ps = PlotSettings;
            if (delta > 0 && ps.XMin < decimal.MinValue / 2) return;
            if (delta < 0 && ps.XMax > decimal.MaxValue / 2) return;
            var shift = (ps.XMax - ps.XMin) * delta / 3000;
            ps.XMin += shift;
            ps.XMax += shift;
            invalidateLayout();
        }

        private void yScroll(int delta) {
            var ps = PlotSettings;
            if (delta > 0 && ps.YMin < decimal.MinValue / 2) return;
            if (delta < 0 && ps.YMax > decimal.MaxValue / 2) return;
            var shift = (ps.YMax - ps.YMin) * delta / 3000;
            ps.YMin += shift;
            ps.YMax += shift;
            invalidateLayout();
        }

        private void xZoom(int px, int delta) {
            var ps = PlotSettings;
            var flog10 = RMath.FLog10(ps.XMax - ps.XMin);
            var scale = Math.Max(0.5f, 1f - (float)delta / 1000);
            if ((flog10 > 24 && scale > 1) || (flog10 < -24 && scale < 1)) return;
            var graphArea = getGraphArea();
            var x = ps.XMin + (ps.XMax - ps.XMin) * (px - graphArea.X) / graphArea.Width;
            ps.XMin = (ps.XMin - x) * (decimal)scale + x;
            ps.XMax = (ps.XMax - x) * (decimal)scale + x;
            invalidateLayout();
        }

        private void yZoom(int py, int delta) {
            var ps = PlotSettings;
            var flog10 = RMath.FLog10(ps.YMax - ps.YMin);
            var scale = Math.Max(0.5f, 1f - (float)delta / 1000);
            if ((flog10 > 24 && scale > 1) || (flog10 < -24 && scale < 1)) return;
            var graphArea = getGraphArea();
            var y = ps.YMin + (ps.YMax - ps.YMin) * (graphArea.Bottom - py) / graphArea.Height;
            ps.YMin = (ps.YMin - y) * (decimal)scale + y;
            ps.YMax = (ps.YMax - y) * (decimal)scale + y;
            invalidateLayout();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (DesignMode) return;

            var g = e.Graphics;
            var s = Settings.Instance;

            render(g);

            // 選択範囲
            if (_dragMode == DragMode.ZoomSelection) {
                using (var brush = new SolidBrush(Color.FromArgb(128, s.Appearance_Color_Selection))) {
                    int xMin = Math.Min(_mouseLastDownPos.X, _mouseLastMovePos.X);
                    int xMax = Math.Max(_mouseLastDownPos.X, _mouseLastMovePos.X);
                    int yMin = Math.Min(_mouseLastDownPos.Y, _mouseLastMovePos.Y);
                    int yMax = Math.Max(_mouseLastDownPos.Y, _mouseLastMovePos.Y);
                    g.FillRectangle(brush, Rectangle.FromLTRB(xMin, yMin, xMax, yMax));
                }
            }
        }

        private void render(Graphics g) {
            var ps = PlotSettings;
            var s = Settings.Instance;

            // 目盛りの生成
            var xNotches = generateScaleNotches(ps.XMin, ps.XMax, ClientSize.Width);
            var yNotches = generateScaleNotches(ps.YMin, ps.YMax, ClientSize.Height);

            // レイアウトの調整 (マウスドラッグ中を除く)
            if (_layoutInvalidated && _mousePressedButtons == MouseButtons.None) {
                _xScaleHeight = xNotches.Select(p => (int)g.MeasureString(p.Text, Font).Width).Max();
                _yScaleWidth = yNotches.Select(p => (int)g.MeasureString(p.Text, Font).Width).Max();
                _layoutInvalidated = false;
            }

            var graphArea = getGraphArea();

            // 色の取得
            var invert = _invertBrightness;
            var backColor = invert ? ColorUtils.InvertHsvValue(BackColor) : BackColor;
            var textColor = invert ? ColorUtils.InvertHsvValue(s.Appearance_Color_Text) : s.Appearance_Color_Text;
            var palette = new Color[] {
                invert ? ColorUtils.InvertHsvValue(s.Appearance_Color_Parenthesis_1) : s.Appearance_Color_Parenthesis_1,
                invert ? ColorUtils.InvertHsvValue(s.Appearance_Color_Parenthesis_2) : s.Appearance_Color_Parenthesis_2,
                invert ? ColorUtils.InvertHsvValue(s.Appearance_Color_Parenthesis_3) : s.Appearance_Color_Parenthesis_3,
                invert ? ColorUtils.InvertHsvValue(s.Appearance_Color_Parenthesis_4) : s.Appearance_Color_Parenthesis_4,
            };
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(backColor);

            using (var thickPen = new Pen(textColor))
            using (var thinPen = new Pen(Color.FromArgb(64, textColor)))
            using (var textBrush = new SolidBrush(textColor)) {
                // 横軸の目盛り
                foreach (var notch in xNotches) {
                    var px = graphArea.X + (float)(graphArea.Width * (notch.Value - ps.XMin) / (ps.XMax - ps.XMin));
                    g.DrawLine(notch.Value == 0 ? thickPen : thinPen, px, graphArea.Top, px, graphArea.Bottom);
                    var sz = g.MeasureString(notch.Text, Font);
                    var bkp2 = g.Save();
                    g.TranslateTransform(px, graphArea.Bottom);
                    g.RotateTransform(-90);
                    g.DrawString(notch.Text, Font, textBrush, -sz.Width, -sz.Height / 2);
                    g.Restore(bkp2);
                }

                // 縦軸の目盛り
                foreach (var notch in yNotches) {
                    var py = graphArea.Bottom - (float)(graphArea.Height * (notch.Value - ps.YMin) / (ps.YMax - ps.YMin));
                    g.DrawLine(notch.Value == 0 ? thickPen : thinPen, graphArea.X, py, graphArea.Right, py);
                    var sz = g.MeasureString(notch.Text, Font);
                    g.DrawString(notch.Text, Font, textBrush, graphArea.X - sz.Width, py - sz.Height / 2);
                }

                // 枠線
                g.DrawRectangle(thickPen, graphArea);
            }

            // グラフの描画
            var bkp = g.Save();
            g.IntersectClip(graphArea);
            int colorIndex = 0;
            var xRange = ps.XMax - ps.XMin;
            var yRange = ps.YMax - ps.YMin;
            foreach (var graphs in _graphs.Values) {
                foreach (var graph in graphs) {
                    using (var pen = new Pen(palette[colorIndex], 2)) {
                        foreach (var polyline in graph.Polylines) {
                            var pts = new PointF[polyline.Points.Length];
                            for (int i = 0; i < pts.Length; i++) {
                                var p = polyline.Points[i];
                                pts[i] = new PointF(
                                    (float)(graphArea.X + (p.X - ps.XMin) * graphArea.Width / xRange),
                                    (float)(graphArea.Bottom - (p.Y - ps.YMin) * graphArea.Height / yRange));
                            }
                            if (pts.Length == 1) {
                                // todo: impl
                            }
                            else {
                                try {
                                    g.DrawLines(pen, pts);
                                }
                                catch { }
                            }
                        }
                    }
                    colorIndex = (colorIndex + 1) % palette.Length;
                }
            }
            g.Restore(bkp);

        }

        private void calcRulerStep(decimal min, decimal max, out decimal lineStep, out int flog10, out int fracDigits) {
            // 目盛りの間隔
            var range = max - min;
            lineStep = RMath.Pow10(RMath.Ceiling(RMath.Log10(range)) - 1);
            if (lineStep * 2 > range) lineStep /= 10;
            else if (lineStep * 4 > range) lineStep /= 5;
            else if (lineStep * 8 > range) lineStep /= 2;

            // 目盛りの桁数
            flog10 = (int)RMath.Floor(RMath.Log10(Math.Max(Math.Abs(min), Math.Abs(max))));
            var logStep = (int)RMath.Floor(RMath.Log10(lineStep));
            fracDigits = Math.Max(0, (int)Math.Floor((double)flog10 / 3) * 3 - logStep);
        }

        private Notch[] generateScaleNotches(decimal min, decimal max, int clientSize) {
            // 目盛りの間隔
            var range = max - min;
            var step = RMath.Pow10(RMath.Ceiling(RMath.Log10(range)) - 1);
            if (step * 2 > range) step /= 10;
            else if (step * 4 > range) step /= 5;
            else if (step * 8 > range) step /= 2;

            // 目盛りの桁数
            var flog10 = (int)RMath.Floor(RMath.Log10(Math.Max(Math.Abs(min), Math.Abs(max))));
            var logStep = (int)RMath.Floor(RMath.Log10(step));
            var fracDigits = Math.Max(0, (int)Math.Floor((double)flog10 / 3) * 3 - logStep);

            // 目盛りの列挙
            var origin = Math.Ceiling(min / step) * step;
            int n = (int)Math.Ceiling((max - origin) / step);
            var notches = new Notch[n];
            for (int i = 0; i < n; i++) {
                var x = origin + step * i;
                var text = siPrefix(x, flog10, fracDigits);
                notches[i] = new Notch(x, text);
            }
            return notches;
        }

        private void invalidateLayout() {
            _layoutInvalidated = true;
            Invalidate();
        }

        private Rectangle getGraphArea() => Rectangle.FromLTRB(
            GraphAreaMargin + _yScaleWidth,
            GraphAreaMargin, 
            ClientSize.Width - GraphAreaMargin, 
            ClientSize.Height - GraphAreaMargin - _xScaleHeight);
        private Rectangle getXScaleArea() {
            var graphArea = getGraphArea();
            return Rectangle.FromLTRB(graphArea.X, graphArea.Bottom, graphArea.Right, ClientSize.Height);
        }
        private Rectangle getYScaleArea() {
            var graphArea = getGraphArea();
            return Rectangle.FromLTRB(0, graphArea.Top, graphArea.X, graphArea.Bottom);
        }

        private static string siPrefix(decimal r, int flog10, int fracDigits) {
            int prefixIndex = prefixOffset;
            if (r != 0) {
                prefixIndex = (int)Math.Floor((double)flog10 / 3) + prefixOffset;
            }
            if (prefixIndex < 0) {
                prefixIndex = 0;
            }
            else if (prefixIndex >= prefixes.Length) {
                prefixIndex = prefixes.Length - 1;
            }
            var exp = (prefixIndex - prefixOffset) * 3;
            var frac = (decimal)(r / RMath.Pow10(exp));
            var format = new StringBuilder();
            format.Append("0.");
            if (fracDigits == 0) {
                format.Append('#');
            }
            else {
                while (fracDigits-- > 0) {
                    format.Append('0');
                }
            }

            if (prefixIndex == prefixOffset) {
                return frac.ToString(format.ToString());
            }
            else {
                return frac.ToString(format.ToString()) + prefixes[prefixIndex];
            }
        }

        private struct Notch {
            public decimal Value;
            public string Text;
            public Notch(decimal val, string text) {
                Value = val;
                Text = text;
            }
        }

        private enum DragMode {
            Idle,
            ZoomSelection,
            XYScroll,
            XScroll,
            YScroll,
        }
    }
}
