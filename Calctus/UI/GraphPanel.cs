using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.UI {
    class GraphPanel : Control {
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

        private bool _whiteBackMode = false;

        public GraphPanel() {
            if (DesignMode) return;
            DoubleBuffered = true;
            _plotter.SynchronizingObject = this;
            _plotter.Plotted += _worker_Plotted;
            PlotSettings.Changed += (sender, e) => { Replot(); };
        }

        public bool WhiteBackMode {
            get => _whiteBackMode;
            set {
                if (value == _whiteBackMode) return;
                _whiteBackMode = value;
                Invalidate();
            }
        }

        /// <summary>グラフ描画をリクエストする</summary>
        public void StartPlot(Sheet sheet, PlotCall[] calls) {
            var ps = PlotSettings;
            var graphArea = getGraphArea();
            if (graphArea.Width > 0) {
                ps.NumSamples = graphArea.Width;
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
                    try {
                        var ps = PlotSettings;
                        var graphArea = getGraphArea();
                        var dx = xScroll ? (ps.XAxis.PosRange) * (e.X - _mouseLastMovePos.X) / graphArea.Width : 0m;
                        var dy = yScroll ? (ps.YAxis.PosRange) * (_mouseLastMovePos.Y - e.Y) / graphArea.Height : 0m;
                        ps.XAxis.PosBottom -= dx;
                        ps.YAxis.PosBottom -= dy;
                        invalidateLayout();
                        Replot();
                    }
                    catch { }
                }
            }
            _mouseLastMovePos = e.Location;
            Invalidate();
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
                    try {
                        var ps = PlotSettings;
                        var graphArea = getGraphArea();
                        var xMin = ps.XAxis.PosBottom + (ps.XAxis.PosRange) * (decimal)(px0 - graphArea.X) / graphArea.Width;
                        var yMin = ps.YAxis.PosBottom + (ps.YAxis.PosRange) * (decimal)(graphArea.Bottom - py1) / graphArea.Height;
                        var xMax = ps.XAxis.PosBottom + (ps.XAxis.PosRange) * (decimal)(px1 - graphArea.X) / graphArea.Width;
                        var yMax = ps.YAxis.PosBottom + (ps.YAxis.PosRange) * (decimal)(graphArea.Bottom - py0) / graphArea.Height;
                        ps.XAxis.PosBottom = xMin;
                        ps.XAxis.PosRange = xMax - xMin;
                        ps.YAxis.PosBottom = yMin;
                        ps.YAxis.PosRange = yMax - yMin;
                        invalidateLayout();
                        Replot();
                    }
                    catch { }
                }
            }
            _mousePressedButtons = MouseButtons.None;
            _dragMode = DragMode.Idle;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);
            if (e.Delta == 0) return;

            var ps = PlotSettings;
            var graphArea = getGraphArea();
            if (getGraphArea().Contains(e.Location)) {
                zoom(ps.XAxis, graphArea.X, graphArea.Width, e.X, e.Delta);
                zoom(ps.YAxis, graphArea.Bottom, -graphArea.Height, e.Y, e.Delta);
                Replot();
            }
            else if (getXScaleArea().Contains(e.Location)) {
                zoom(ps.XAxis, graphArea.X, graphArea.Width, e.X, e.Delta);
                Replot();
            }
            else if (getYScaleArea().Contains(e.Location)) {
                zoom(ps.YAxis, graphArea.Bottom, -graphArea.Height, e.Y, e.Delta);
                Replot();
            }
        }

        private void zoom(AxisSettings axis, int offset, int size, int px, int delta) {
            try {
                var ps = PlotSettings;
                var flog10 = MathEx.FLog10Abs(axis.PosRange);
                var scale = Math.Max(0.5f, 1f - (float)delta / 1000);
                if ((flog10 > 24 && scale > 1) || (flog10 < -24 && scale < 1)) return;
                var graphArea = getGraphArea();
                var x = axis.PosBottom + (axis.PosRange) * (px - offset) / size;
                axis.PosBottom = (axis.PosBottom - x) * (decimal)scale + x;
                axis.PosRange *= (decimal)scale;
                invalidateLayout();
            }
            catch { }
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
            var xLines = generateScaleNotches(ps.XAxis, ClientSize.Width);
            var yLines = generateScaleNotches(ps.YAxis, ClientSize.Height);

            // レイアウトの調整 (マウスドラッグ中を除く)
            if (_layoutInvalidated && _mousePressedButtons == MouseButtons.None) {
                if (xLines.Length > 0) {
                    _xScaleHeight = xLines.Select(p => (int)g.MeasureString(p.Text, Font).Width).Max();
                }
                if (yLines.Length > 0) {
                    _yScaleWidth = yLines.Select(p => (int)g.MeasureString(p.Text, Font).Width).Max();
                }
                _layoutInvalidated = false;
            }

            var graphArea = getGraphArea();

            // 色の取得
            var whiteBack = _whiteBackMode;
            var backColor = whiteBack ? Color.White : BackColor;
            var textColor = whiteBack ? Color.Black : s.Appearance_Color_Text;
            var palette = new Color[] {
                whiteBack ? Color.FromArgb(192, 64, 64) : s.Appearance_Color_Parenthesis_1,
                whiteBack ? Color.FromArgb(64, 192, 64) : s.Appearance_Color_Parenthesis_2,
                whiteBack ? Color.FromArgb(64, 64, 192) : s.Appearance_Color_Parenthesis_3,
                whiteBack ? Color.FromArgb(192, 64, 192) : s.Appearance_Color_Parenthesis_4,
            };
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(backColor);

            using (var thickPen = new Pen(textColor))
            using (var thinPen = new Pen(Color.FromArgb(64, textColor)))
            using (var dottedPen = new Pen(Color.FromArgb(64, textColor)))
            using (var textBrush = new SolidBrush(textColor)) {
                dottedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // 横軸の目盛り
                foreach (var line in xLines) {
                    if (project(ps.XAxis, line.Value, graphArea.X, graphArea.Width, out float px)) {
                        var pen =
                            line.Value == 0 ? thickPen :
                            line.SubLine ? dottedPen : thinPen;
                        g.DrawLine(pen, px, graphArea.Y, px, graphArea.Bottom);
                        if (!string.IsNullOrEmpty(line.Text)) {
                            var sz = g.MeasureString(line.Text, Font);
                            var bkp2 = g.Save();
                            g.TranslateTransform(px, graphArea.Bottom);
                            g.RotateTransform(-90);
                            g.DrawString(line.Text, Font, textBrush, -sz.Width, -sz.Height / 2);
                            g.Restore(bkp2);
                        }
                    }
                }

                // 縦軸の目盛り
                foreach (var line in yLines) {
                    if (project(ps.YAxis, line.Value, graphArea.Bottom, -graphArea.Height, out float py)) {
                        var pen =
                            line.Value == 0 ? thickPen :
                            line.SubLine ? dottedPen : thinPen;
                        g.DrawLine(pen, graphArea.X, py, graphArea.Right, py);
                        if (!string.IsNullOrEmpty(line.Text)) {
                            var sz = g.MeasureString(line.Text, Font);
                            g.DrawString(line.Text, Font, textBrush, graphArea.X - sz.Width, py - sz.Height / 2);
                        }
                    }
                }

                // 枠線
                g.DrawRectangle(thickPen, graphArea);
            }

            // グラフの描画
            {
                var bkp = g.Save();
                g.IntersectClip(graphArea);
                int colorIndex = 0;
                var pts = new List<PointF>();
                foreach (var graphs in _graphs.Values) {
                    foreach (var graph in graphs) {
                        using (var pen = new Pen(palette[colorIndex], 2)) {
                            foreach (var polyline in graph.Polylines) {
                                pts.Clear();
                                for (int i = 0; i < polyline.Points.Length; i++) {
                                    var p = polyline.Points[i];
                                    float px = 0, py = 0;
                                    bool ok =
                                        project(ps.XAxis, p.X, graphArea.X, graphArea.Width, out px) &&
                                        project(ps.YAxis, p.Y, graphArea.Bottom, -graphArea.Height, out py);
                                    if (ok) pts.Add(new PointF(px, py));
                                }
                                if (pts.Count == 1) {
                                    // todo: impl
                                }
                                else {
                                    try {
                                        g.DrawLines(pen, pts.ToArray());
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

            if (graphArea.Contains(_mouseLastMovePos) && _mousePressedButtons == MouseButtons.None) {
                // カーソルの描画
                var px = _mouseLastMovePos.X;
                var posX = ps.XAxis.PosBottom + ps.XAxis.PosRange * (px - graphArea.X) / graphArea.Width;
                if (ps.XAxis.PosMin <= posX && posX <= ps.XAxis.PosMax) {
                    // 線
                    using (var pen = new Pen(textColor)) {
                        g.DrawLine(pen, px, graphArea.Y, px, graphArea.Bottom);
                    }

                    // X座標
                    var valX = ps.XAxis.PosToValue(posX);
                    var textX = siPrefix(valX, MathEx.FLog10Abs(valX), 3);
                    paintBalloon(g, textX, px, graphArea.Bottom, textColor, backColor);

                    // Y座標
                    int colorIndex = 0;
                    foreach (var graphs in _graphs.Values) {
                        foreach(var graph in graphs) {
                            try {
                                var e = new EvalContext(graph.Call.Context);
                                var valY = graph.Call.Function.Call(e, new RealVal(valX)).AsDecimal;
                                if (project(ps.YAxis, valY, graphArea.Bottom, -graphArea.Height, out float py)) {
                                    var textY = siPrefix(valY, MathEx.FLog10Abs(valY), 3);
                                    paintBalloon(g, textY, px, py, palette[colorIndex], backColor);
                                }
                            }
                            catch { }
                            colorIndex = (colorIndex + 1) % palette.Length;
                        }
                    }
                }
            }
        }

        private void paintBalloon(Graphics g, string text, float x, float y, Color backColor, Color textColor) {
            using (var backBrush = new SolidBrush(backColor))
            using (var textBrush = new SolidBrush(textColor)) {
                var sz = g.MeasureString(text, Font);
                var w = sz.Width;
                var h = sz.Height;
                var pts = new PointF[7];
                if (x < ClientSize.Width / 2) {
                    pts[0] = new PointF(x + h / 2, y - h);
                    pts[1] = new PointF(x + w + h / 2, y - h);
                    pts[2] = new PointF(x + w + h / 2, y);
                    pts[3] = new PointF(x + h / 2, y);
                    pts[4] = new PointF(x + h / 2, y - h / 4);
                    pts[5] = new PointF(x, y);
                    pts[6] = new PointF(x + h / 2, y - h * 3 / 4);
                }
                else {
                    pts[0] = new PointF(x - w - h / 2, y - h);
                    pts[1] = new PointF(x - h / 2, y - h);
                    pts[2] = new PointF(x - h / 2, y - h * 3 / 4);
                    pts[3] = new PointF(x, y);
                    pts[4] = new PointF(x - h / 2, y - h / 4);
                    pts[5] = new PointF(x - h / 2, y);
                    pts[6] = new PointF(x - w - h / 2, y);
                }
                g.FillPolygon(backBrush, pts);
                g.DrawString(text, Font, textBrush, pts[0]);
            }
        }

        private Gridline[] generateScaleNotches(AxisSettings axis, int clientSize) {
            switch(axis.Type) {
                case AxisType.Linear: {
                        // 目盛りの間隔
                        var range = axis.PosRange;
                        var max = axis.PosBottom + range;
                        var step = MathEx.Pow10(Math.Ceiling(MathEx.Log10(range)) - 1);
                        if (step * 2 > range) step /= 10;
                        else if (step * 4 > range) step /= 5;
                        else if (step * 8 > range) step /= 2;

                        // 目盛りの桁数
                        var flog10 = MathEx.FLog10Abs(Math.Max(Math.Abs(axis.PosBottom), Math.Abs(max)));
                        var logStep = (int)Math.Floor(MathEx.Log10(step));
                        var fracDigits = Math.Max(0, (int)Math.Floor((double)flog10 / 3) * 3 - logStep);

                        // 目盛りの生成
                        var origin = Math.Ceiling(axis.PosBottom / step) * step;
                        int n = (int)Math.Floor((max - origin) / step);
                        var lines = new Gridline[n + 1];
                        for (int i = 0; i <= n; i++) {
                            var x = origin + step * i;
                            var text = siPrefix(x, flog10, fracDigits);
                            lines[i] = new Gridline(x, text);
                        }
                        return lines;
                    }
                case AxisType.Log10: {
                        // 指数の開始値・終了値
                        var expStart = (int)Math.Max(AxisSettings.Log10PosMin, Math.Floor(axis.PosBottom));
                        var expEnd = (int)Math.Min(AxisSettings.Log10PosMax, Math.Ceiling(axis.PosTop));

                        // 分割数
                        int subNum, subDiv;
                        if (axis.PosRange < 1) { subNum = 90; subDiv = 100; }
                        else if (axis.PosRange < 10) { subNum = 9; subDiv = 10; }
                        else { subNum = 1; subDiv = 1; }

                        // 目盛りの生成
                        var lines = new List<Gridline>();
                        for (var exp = expStart; exp <= expEnd; exp++) {
                            var valStep = MathEx.Pow10(exp);
                            for (var sub = 0; sub < subNum; sub++) {
                                var val = valStep * (1 + (decimal)sub * 10 / subDiv);
                                var pos = MathEx.Log10(val);
                                if (axis.PosBottom <= pos && pos <= axis.PosTop) {
                                    bool isSub = (sub % 10 != 0);
                                    var text = isSub ? null : siPrefix(val, MathEx.FLog10Abs(val), 0);
                                    lines.Add(new Gridline(val, text, isSub));
                                }
                            }
                        }
                        return lines.ToArray();
                    }
                default:
                    throw new NotImplementedException();
            }
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

        private bool project(AxisSettings axis, decimal val, int offset, int size, out float pos) {
            try {
                if (axis.ValueToPos(val, out decimal dpos)) {
                    dpos = offset + (dpos - axis.PosBottom) * (size / axis.PosRange);
                    if (-65536 < dpos && dpos < 65536) {
                        pos = (float)dpos;
                        return true;
                    }
                }
            }
            catch { }
            pos = 0;
            return false;
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
            var frac = (decimal)(r / MathEx.Pow10(exp));
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

        private struct Gridline {
            public decimal Value;
            public string Text;
            public bool SubLine;
            public Gridline(decimal val, string text, bool sub = false) {
                Value = val;
                Text = text;
                SubLine = sub;
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
