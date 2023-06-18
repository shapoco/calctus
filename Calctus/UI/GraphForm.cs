using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model.Sheets;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Graphs;
using System.Runtime.InteropServices;

namespace Shapoco.Calctus.UI {
    partial class GraphForm : Form {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;

        public static readonly Dictionary<Sheet, PlotCall[]> Requests = new Dictionary<Sheet, PlotCall[]>();
        public static readonly Dictionary<string, GraphForm> Forms = new Dictionary<string, GraphForm>();

        /// <summary>グラフ描画リクエストをウィンドウ毎に振り分ける</summary>
        public static void RequestPlot(Sheet sheet, PlotCall[] reqs) {
            // 描画リクエストをウィンドウ名毎にまとめる
            var dic = new Dictionary<string, List<PlotCall>>();
            foreach (var req in reqs) {
                List<PlotCall> workList;
                if (!dic.TryGetValue(req.WindowName, out workList)) {
                    workList = new List<PlotCall>();
                    dic[req.WindowName] = workList;
                }
                workList.Add(req);
            }
            // ウィンドウ毎に描画リクエストを発行する
            foreach (var windowName in dic.Keys) {
                GraphForm form;
                if (!Forms.TryGetValue(windowName, out form)) {
                    form = new GraphForm();
                    form.Text = windowName;
                    Forms.Add(windowName, form);
                    form.Show();
                    var mainFormBounds = MainForm.Instance.Bounds;
                    var graphFormBounds = new Rectangle(mainFormBounds.X, mainFormBounds.Bottom, mainFormBounds.Width, mainFormBounds.Width * 3 / 4);
                    if (Screen.AllScreens.Any(p => p.Bounds.Contains(graphFormBounds))) {
                        form.Bounds = graphFormBounds;
                    }
                }
                form.StartPlot(sheet, dic[windowName].ToArray());
                form.Visible = true;
            }
        }

        public static void SetWindowStatusAll(FormWindowState state) {
            foreach(var form in Forms.Values) {
                form.WindowState = state;
            }
        }

        public static void ReloadSettingsAll() {
            foreach (var form in Forms.Values) {
                form.ReloadSettings();
            }
        }

        public static void HideAll() {
            foreach (var form in Forms.Values) {
                form.Visible = false;
            }
        }

        public static void ReshowAll() {
            foreach (var form in Forms.Values) {
                if (!form.ClosedByUser) {
                    form.Visible = true;
                }
            }
        }

        public static void SetTopMostAll(bool value) {
            foreach (var form in Forms.Values) {
                if (form.IsHandleCreated) {
                    var hWndInsertAfter = value ? HWND_TOPMOST : HWND_NOTOPMOST;
                    SetWindowPos(form.Handle, hWndInsertAfter, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOSIZE | SWP_NOMOVE);
                }
            }
        }

        public static void CloseAll() {
            foreach (var form in Forms.Values) {
                form.Close();
            }
            Forms.Clear();
        }

        //------------------------------------------------------------

        public bool ClosedByUser { get; private set; } = false;

        protected override bool ShowWithoutActivation => true;

        public GraphForm() {
            InitializeComponent();

            axisSettingsX.Label.Text = "X-Axis";
            axisSettingsY.Label.Text = "Y-Axis";

            axisSettingsX.AxisSettings = graphPanel.PlotSettings.XAxis;
            axisSettingsY.AxisSettings = graphPanel.PlotSettings.YAxis;

            KeyPreview = true;
            KeyDown += GraphForm_KeyDown;
            FormClosing += GraphForm_FormClosing;

            copyButton.Click += CopyButton_Click;
            whiteBackModeButton.Click += (sender, e) => { graphPanel.WhiteBackMode = !graphPanel.WhiteBackMode; };

            ReloadSettings();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            ClosedByUser = false;
        }

        /// <summary>グラフ描画をリクエストする</summary>
        public void StartPlot(Sheet sheet, PlotCall[] calls) {
            graphPanel.StartPlot(sheet, calls);
        }

        public void ReloadSettings() {
            var s = Settings.Instance;

            try {
                Font = new Font(s.Appearance_Font_Button_Name, s.Appearance_Font_Size);
            }
            catch { }

            BackColor = s.Appearance_Color_Background;
            ForeColor = s.Appearance_Color_Text;
            graphPanel.Invalidate();
        }

        private void GraphForm_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C) {
                copyButton.PerformClick();
            }
        }

        private void GraphForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
            ClosedByUser = true;
        }

        private void CopyButton_Click(object sender, EventArgs e) {
            graphPanel.Copy();
        }

    }
}
