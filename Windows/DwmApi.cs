using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Shapoco.Windows {
    static class DwmApi {
        public enum DWMWINDOWATTRIBUTE : Int32 {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_LAST
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref Int32 pvAttribute, Int32 cbAttribute);

        public static void SetDarkModeEnable(Form f, bool darkMode) {
            Int32 value = darkMode ? 1 : 0;
            DwmSetWindowAttribute(f.Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (Int32)Marshal.SizeOf(typeof(Int32)));
        }
    }
}
