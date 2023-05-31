using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI.Sheets {
    class GdiControl : Control {
        public readonly GdiBox RootBox;
        private bool _thisFocused = false;
        private GdiBox _focusedBox = null;
        private GdiBox _mouseCaptureBox = null;
        private MouseButtons _pressedMouseButton = MouseButtons.None;
        private bool _disposed = false;

        // C#でIMEの入力を受けるユーザーコントロールの作成 - Qiita
        // https://qiita.com/takao_mofumofu/items/24c060a1d4f6b3df5c73
        // [C#] IME変換領域(ウィンドウ)のフォントを設定する - iPentec
        // https://www.ipentec.com/document/csharp-call-immsetcompositionfont
        #region "IME関連"

        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_RESULTREADSTR = 0x0200;
        private const int WM_IME_STARTCOMPOSITION = 0x10D; // IME変換開始
        private const int WM_IME_ENDCOMPOSITION = 0x10E;   // IME変換終了
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_IME_SETCONTEXT = 0x0281;

        private enum ImmAssociateContextExFlags : uint {
            IACE_CHILDREN = 0x0001,
            IACE_DEFAULT = 0x0010,
            IACE_IGNORENOCONTEXT = 0x0020
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct C_RECT {
            public int _Left;
            public int _Top;
            public int _Right;
            public int _Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct C_POINT {
            public int x;
            public int y;
        }

        private const uint CFS_POINT = 0x0002;

        private struct COMPOSITIONFORM {
            public uint dwStyle;
            public C_POINT ptCurrentPos;
            public C_RECT rcArea;
        }

        //LOGFONT
        public enum FontWeight : int {
            FW_DONTCARE = 0,
            FW_THIN = 100,
            FW_EXTRALIGHT = 200,
            FW_LIGHT = 300,
            FW_NORMAL = 400,
            FW_MEDIUM = 500,
            FW_SEMIBOLD = 600,
            FW_BOLD = 700,
            FW_EXTRABOLD = 800,
            FW_HEAVY = 900,
        }

        public enum FontCharSet : byte {
            ANSI_CHARSET = 0,
            DEFAULT_CHARSET = 1,
            SYMBOL_CHARSET = 2,
            SHIFTJIS_CHARSET = 128,
            HANGEUL_CHARSET = 129,
            HANGUL_CHARSET = 129,
            GB2312_CHARSET = 134,
            CHINESEBIG5_CHARSET = 136,
            OEM_CHARSET = 255,
            JOHAB_CHARSET = 130,
            HEBREW_CHARSET = 177,
            ARABIC_CHARSET = 178,
            GREEK_CHARSET = 161,
            TURKISH_CHARSET = 162,
            VIETNAMESE_CHARSET = 163,
            THAI_CHARSET = 222,
            EASTEUROPE_CHARSET = 238,
            RUSSIAN_CHARSET = 204,
            MAC_CHARSET = 77,
            BALTIC_CHARSET = 186,
        }

        public enum FontPrecision : byte {
            OUT_DEFAULT_PRECIS = 0,
            OUT_STRING_PRECIS = 1,
            OUT_CHARACTER_PRECIS = 2,
            OUT_STROKE_PRECIS = 3,
            OUT_TT_PRECIS = 4,
            OUT_DEVICE_PRECIS = 5,
            OUT_RASTER_PRECIS = 6,
            OUT_TT_ONLY_PRECIS = 7,
            OUT_OUTLINE_PRECIS = 8,
            OUT_SCREEN_OUTLINE_PRECIS = 9,
            OUT_PS_ONLY_PRECIS = 10,
        }

        public enum FontClipPrecision : byte {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4),
        }

        public enum FontQuality : byte {
            DEFAULT_QUALITY = 0,
            DRAFT_QUALITY = 1,
            PROOF_QUALITY = 2,
            NONANTIALIASED_QUALITY = 3,
            ANTIALIASED_QUALITY = 4,
            CLEARTYPE_QUALITY = 5,
            CLEARTYPE_NATURAL_QUALITY = 6,
        }

        [Flags]
        public enum FontPitchAndFamily : byte {
            DEFAULT_PITCH = 0,
            FIXED_PITCH = 1,
            VARIABLE_PITCH = 2,
            FF_DONTCARE = (0 << 4),
            FF_ROMAN = (1 << 4),
            FF_SWISS = (2 << 4),
            FF_MODERN = (3 << 4),
            FF_SCRIPT = (4 << 4),
            FF_DECORATIVE = (5 << 4),
        }

        const int LF_FACESIZE = 32;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class LOGFONT {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public FontWeight lfWeight;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfItalic;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfUnderline;
            [MarshalAs(UnmanagedType.U1)]
            public bool lfStrikeOut;
            public FontCharSet lfCharSet;
            public FontPrecision lfOutPrecision;
            public FontClipPrecision lfClipPrecision;
            public FontQuality lfQuality;
            public FontPitchAndFamily lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE * 2)]
            public string lfFaceName;
        }
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        private static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);
        [DllImport("Imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmCreateContext();
        [DllImport("imm32.dll")]
        private static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr hIMC, ImmAssociateContextExFlags dwFlags);
        [DllImport("imm32.dll")]
        private static extern int ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompositionForm);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);
        [DllImport("imm32.dll")]
        private static extern int ImmSetCompositionFont(IntPtr hIMC, [In, Out] IntPtr lplf);

        #endregion

        private IntPtr himc = IntPtr.Zero;

        public GdiControl() {
            if (DesignMode) return;
            RootBox = new GdiBox(this);
            DoubleBuffered = true;
        }

        public GdiBox FocusedBox {
            get => _focusedBox;
            set => setFocusedBox(value);
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            setFocusedBox(_focusedBox);
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            setFocusedBox(_focusedBox);
        }

        protected override bool IsInputKey(Keys keyData) {
            switch (keyData) {
                case Keys.Tab:
                case Keys.Shift | Keys.Tab:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            _focusedBox?.PerformKeyDown(e);
            if (!e.Handled) {
                base.OnKeyDown(e);
            }
            if (!e.Handled) {
                if (e.Modifiers == Keys.None && e.KeyCode == Keys.Tab) {
                    if (FocusedBox == null || FocusedBox.Parent == null || !FocusedBox.Parent.SelectNextBox(FocusedBox, true)) {
                        Parent.SelectNextControl(this, true, true, true, true);
                    }
                }
                else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Tab) {
                    if (FocusedBox == null || FocusedBox.Parent == null || !FocusedBox.Parent.SelectNextBox(FocusedBox, false)) {
                        Parent.SelectNextControl(this, false, true, true, true);
                    }
                }
            }
#if DEBUG
            if (e.KeyCode == Keys.ControlKey) Invalidate();
#endif
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            _focusedBox?.PerformKeyUp(e);
            if (!e.Handled) {
                base.OnKeyUp(e);
            }
#if DEBUG
            if (e.KeyCode == Keys.ControlKey) Invalidate();
#endif
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            _focusedBox?.PerformKeyPress(e);
            if (!e.Handled) {
                base.OnKeyPress(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            var bounds = Rectangle.Empty;
            if (_pressedMouseButton == MouseButtons.None) {
                var hitBox = hitTest(e.Location, out bounds);
                if (hitBox != _mouseCaptureBox) {
                    _mouseCaptureBox = hitBox;
                    if (hitBox != null) {
                        this.Cursor = hitBox.Cursor;
                    }
                    else {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
            else if (_mouseCaptureBox != null) {
                bounds = _mouseCaptureBox.GetRootBounds();
            }
            _mouseCaptureBox?.PerformMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - bounds.X, e.Y - bounds.Y, e.Delta));
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            _pressedMouseButton |= e.Button;
            if (!Focused) Focus();
            if (_mouseCaptureBox != null) {
                if (_mouseCaptureBox.Focusable) {
                    setFocusedBox(_mouseCaptureBox);
                }
                var bounds = _mouseCaptureBox.GetRootBounds();
                _mouseCaptureBox.PerformMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X - bounds.X, e.Y - bounds.Y, e.Delta));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            _pressedMouseButton &= ~e.Button;
            if (_mouseCaptureBox != null) {
                var bounds = _mouseCaptureBox.GetRootBounds();
                _mouseCaptureBox.PerformMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X - bounds.X, e.Y - bounds.Y, e.Delta));
            }
        }

        protected override void OnDoubleClick(EventArgs e) {
            base.OnDoubleClick(e);
            _focusedBox?.PerformDoubleClick(e);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            RootBox.Bounds = ClientRectangle;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            paintRecursive(RootBox, e);
        }

        protected virtual void OnFocusedBoxChanged() { }

        // C#でIMEの入力を受けるユーザーコントロールの作成 - Qiita
        // https://qiita.com/takao_mofumofu/items/24c060a1d4f6b3df5c73
        // [C#] IME変換領域(ウィンドウ)のフォントを設定する - iPentec
        // https://www.ipentec.com/document/csharp-call-immsetcompositionfont
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case WM_IME_SETCONTEXT: {
                        //Imeを関連付ける
                        himc = ImmCreateContext();
                        ImmAssociateContextEx(this.Handle, himc, ImmAssociateContextExFlags.IACE_DEFAULT);
                        base.WndProc(ref m);
                        break;
                    }
                case WM_IME_STARTCOMPOSITION: {
                        //入力コンテキストにアクセスするためのお約束
                        IntPtr himc = ImmGetContext(this.Handle);

                        var cursorPos = Point.Empty;
                        if (FocusedBox != null) {
                            cursorPos = FocusedBox.GetCursorPosition();
                            cursorPos.Offset(FocusedBox.GetRootBounds().Location);
                        }

                        //コンポジションウィンドウの位置を設定
                        COMPOSITIONFORM info = new COMPOSITIONFORM();
                        info.dwStyle = CFS_POINT;
                        info.ptCurrentPos.x = cursorPos.X;
                        info.ptCurrentPos.y = cursorPos.Y;
                        ImmSetCompositionWindow(himc, ref info);

                        //コンポジションウィンドウのフォントを設定
                        IntPtr hHGlobalLOGFONT = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LOGFONT)));
                        IntPtr pLogFont = GlobalLock(hHGlobalLOGFONT);
                        LOGFONT logFont = new LOGFONT();
                        this.Font.ToLogFont(logFont);
                        logFont.lfFaceName = this.Font.Name;
                        Marshal.StructureToPtr(logFont, pLogFont, false);
                        GlobalUnlock(hHGlobalLOGFONT);
                        ImmSetCompositionFont(himc, hHGlobalLOGFONT);
                        Marshal.FreeHGlobal(hHGlobalLOGFONT);

                        //入力コンテキストへのアクセスが終了したらロックを解除する
                        ImmReleaseContext(Handle, himc);

                        base.WndProc(ref m);
                        break;
                    }
                default:
                    //IME以外のメッセージは元のプロシージャで処理
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    RootBox.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        private void paintRecursive(GdiBox parent, PaintEventArgs e) {
            var g = e.Graphics;
            var bkp = g.Save();
            g.TranslateTransform(parent.Left, parent.Top);
            var clipRect = e.ClipRectangle;
            clipRect.Offset(-parent.Left, -parent.Top);
            var args = new PaintEventArgs(e.Graphics, clipRect);
            parent.PerformPaint(args);
            foreach(var child in parent.Children) {
                paintRecursive(child, args);
            }
            g.Restore(bkp);
        }

        private GdiBox hitTest(Point pos, out Rectangle hitAbsBounds) {
            if (RootBox.HitTest(RootBox.Location, pos, out GdiBox hitBlock, out hitAbsBounds)) {
                return hitBlock;
            }
            return null;
        }

        private void setFocusedBox(GdiBox box) {
            if (box == _focusedBox && Focused == _thisFocused) return;
            if (_focusedBox != null && (box != _focusedBox || !Focused)) {
                _focusedBox.PerformLostFocus();
            }
            _focusedBox = box;
            _thisFocused = Focused;
            if (_focusedBox != null && Focused) {
                _focusedBox.PerformGotFocus();
            }
            OnFocusedBoxChanged();
        }

    }
}
