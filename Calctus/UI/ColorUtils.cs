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

        public static Color GrayScale(Color color) {
            int gray = (int)Math.Round((0.3 * color.R) + (0.59 * color.G) + (0.11 * color.B));
            return Color.FromArgb(color.A, gray, gray, gray);
        }

        public static Color InvertHsvValue(Color color) {
            int oldMin = Math.Min(Math.Min(color.R, color.G), color.B);
            int newMin = 255 - Math.Max(Math.Max(color.R, color.G), color.B);
            return Color.FromArgb(color.A,
                (color.R - oldMin) + newMin,
                (color.G - oldMin) + newMin,
                (color.B - oldMin) + newMin);
        }

        public static Color Blend(Color c0, Color c1, float alpha = 0.5f) {
            float xAlpha = 1 - alpha;
            return Color.FromArgb(
                (int)Math.Round(c0.A * xAlpha + c1.A * alpha),
                (int)Math.Round(c0.R * xAlpha + c1.R * alpha),
                (int)Math.Round(c0.G * xAlpha + c1.G * alpha),
                (int)Math.Round(c0.B * xAlpha + c1.B * alpha));
        }
    }
}
