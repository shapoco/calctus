using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    internal static class ColorUtils {
        public static Color Grayish(Color baseColor, int percent) {
            int brightness = (baseColor.R + baseColor.G + baseColor.B) / 3;
            Color goalColor = (brightness < 128) ? Color.White : Color.Black;
            return Color.FromArgb(
                baseColor.A,
                baseColor.R + (goalColor.R - baseColor.R) * percent / 100,
                baseColor.G + (goalColor.G - baseColor.G) * percent / 100,
                baseColor.B + (goalColor.B - baseColor.B) * percent / 100);
        }
    }
}
