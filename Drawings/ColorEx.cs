using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Maths;

namespace Shapoco.Drawings {
    internal static class ColorEx {
        public static Color Grayish(this Color baseColor, int percent) {
            int brightness = (baseColor.R + baseColor.G + baseColor.B) / 3;
            Color goalColor = (brightness < 128) ? Color.White : Color.Black;
            return Color.FromArgb(
                baseColor.A,
                baseColor.R + (goalColor.R - baseColor.R) * percent / 100,
                baseColor.G + (goalColor.G - baseColor.G) * percent / 100,
                baseColor.B + (goalColor.B - baseColor.B) * percent / 100);
        }

        public static Color GrayScale(this Color color) {
            int gray = (int)Math.Round((0.3 * color.R) + (0.59 * color.G) + (0.11 * color.B));
            return Color.FromArgb(color.A, gray, gray, gray);
        }

        public static Color InvertHsvValue(this Color color) {
            int oldMin = Math.Min(Math.Min(color.R, color.G), color.B);
            int newMin = 255 - Math.Max(Math.Max(color.R, color.G), color.B);
            return Color.FromArgb(color.A,
                (color.R - oldMin) + newMin,
                (color.G - oldMin) + newMin,
                (color.B - oldMin) + newMin);
        }

        public static Color Blend(this Color c0, Color c1, float alpha = 0.5f) {
            float xAlpha = 1 - alpha;
            return Color.FromArgb(
                (int)Math.Round(c0.A * xAlpha + c1.A * alpha),
                (int)Math.Round(c0.R * xAlpha + c1.R * alpha),
                (int)Math.Round(c0.G * xAlpha + c1.G * alpha),
                (int)Math.Round(c0.B * xAlpha + c1.B * alpha));
        }

        public static void ToHsv(this Color c, out float h, out float s, out float v)
            => RgbToHsv(c.R, c.G, c.B, out h, out s, out v);

        public static void RgbToHsv(float r, float g, float b, out float h, out float s, out float v) {
            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            h = rgbToHue(r, g, b, min, max);
            if (max == 0) {
                s = 0;
            }
            else {
                s = 100 * (max - min) / max;
            }
            v = max * 100 / 255;
        }

        private static float rgbToHue(float r, float g, float b, float min, float max) {
            if (min == max) return 0;
            if (min == b) return 60 * (g - r) / (max - min) + 60;
            if (min == r) return 60 * (b - g) / (max - min) + 180;
            return 60 * (r - b) / (max - min) + 300;
        }

        public static Color HsvToRgb(float h, float s, float v, int alpha = 255) {
            HsvToRgb(h, s, v, out float r, out float g, out float b);
            return Color.FromArgb(alpha,
                MathEx.Clip(0, 255, (int)Math.Round(r)),
                MathEx.Clip(0, 255, (int)Math.Round(g)),
                MathEx.Clip(0, 255, (int)Math.Round(b)));
        }

        public static void HsvToRgb(float h, float s, float v, out float r, out float g, out float b) {
            h = h % 360;
            s = MathEx.Clip(0, 100, s) / 100;
            v = MathEx.Clip(0, 100, v) / 100;

            var f = (h / 60) % 1;
            var x = v * 255;
            var y = v * (1 - s) * 255;
            var z = v * (1 - s * f) * 255;
            var w = v * (1 - s * (1 - f)) * 255;

            if (s == 0) { r = x; g = x; b = x; }
            else if (h < 60) { r = x; g = w; b = y; }
            else if (h < 120) { r = z; g = x; b = y; }
            else if (h < 180) { r = y; g = x; b = w; }
            else if (h < 240) { r = y; g = z; b = x; }
            else if (h < 300) { r = w; g = y; b = x; }
            else { r = x; g = y; b = z; }
        }
    }
}
