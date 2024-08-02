using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Standards {
    static class ColorSpace {
        public static int SatPack(real x, real y, real z) {
            int ix = Math.Max(0, Math.Min(255, (int)Math.Round(x)));
            int iy = Math.Max(0, Math.Min(255, (int)Math.Round(y)));
            int iz = Math.Max(0, Math.Min(255, (int)Math.Round(z)));
            return (ix << 16) | (iy << 8) | iz;
        }

        public static void Unpack(real xyz, out int x, out int y, out int z) {
            int ixyz = (int)xyz;
            x = (ixyz >> 16) & 0xff;
            y = (ixyz >> 8) & 0xff;
            z = ixyz & 0xff;
        }

        public static int Rgb444ToRgb888(int rgb) {
            var r = (rgb >> 8) & 0xf;
            var g = (rgb >> 4) & 0xf;
            var b = rgb & 0xf;
            r |= r << 4;
            g |= g << 4;
            b |= b << 4;
            return SatPack(r, g, b);
        }

        public static int Pack565(int r, int g, int b) {
            return
                (Math.Max(0, Math.Min(31, r)) << 11) |
                (Math.Max(0, Math.Min(63, g)) << 5) |
                Math.Max(0, Math.Min(31, b));
        }

        public static int[] Unpack565(int rgb) {
            return new int[] {
                (rgb >> 11) & 0x1f,
                (rgb >> 5) & 0x3f,
                rgb & 0x1f,
            };
        }

        public static int Rgb888To565(int rgb) {
            var r = Math.Min(31, (((rgb >> 18) & 0x3f) + 1) >> 1);
            var g = Math.Min(63, (((rgb >> 9) & 0x7f) + 1) >> 1);
            var b = Math.Min(31, (((rgb >> 2) & 0x3f) + 1) >> 1);
            return Pack565(r, g, b);
        }

        public static int Rgb565To888(int rgb) {
            var ch = Unpack565(rgb);
            var r = ch[0];
            var g = ch[1];
            var b = ch[2];
            r = (r << 3) | ((r >> 2) & 0x7);
            g = (g << 2) | ((g >> 4) & 0x3);
            b = (b << 3) | ((b >> 2) & 0x7);
            return SatPack(r, g, b);
        }

        private static real rgbToHue(real r, real g, real b, real min, real max) {
            if (min == max) {
                return 0;
            }
            else if (min == b) {
                return 60 * (g - r) / (max - min) + 60;
            }
            else if (min == r) {
                return 60 * (b - g) / (max - min) + 180;
            }
            else {
                return 60 * (r - b) / (max - min) + 300;
            }
        }

        public static real[] RgbToHsv(real r, real g, real b) {
            var min = RMath.Min(r, RMath.Min(g, b));
            var max = RMath.Max(r, RMath.Max(g, b));
            real h, s, v;
            h = rgbToHue(r, g, b, min, max);
            if (max == 0) {
                s = 0;
            }
            else {
                s = 100 * (max - min) / max;
            }
            v = max * 100 / 255;
            return new real[]{h, s, v};
        }
        public static real[] RgbToHsv(real rgb) {
            Unpack(rgb, out int r, out int g, out int b);
            return RgbToHsv(r, g, b);
        }

        public static void HsvToRgb(real h, real s, real v, out real r, out real g, out real b) {
            h = h % 360;
            s = RMath.Max(0, RMath.Min(100, s)) / 100;
            v = RMath.Max(0, RMath.Min(100, v)) / 100;

            var f = (h / 60) % 1;
            var x = v * 255;
            var y = v * (1 - s) * 255;
            var z = v * (1 - s * f) * 255;
            var w = v * (1 - s * (1 - f)) * 255;

            if (s == 0) {
                r = x; g = x; b = x;
            }
            else if (h < 60) {
                r = x; g = w; b = y;
            }
            else if (h < 120) {
                r = z; g = x; b = y;
            }
            else if (h < 180) {
                r = y; g = x; b = w;
            }
            else if (h < 240) {
                r = y; g = z; b = x;
            }
            else if (h < 300) {
                r = w; g = y; b = x;
            }
            else {
                r = x; g = y; b = z;
            }
        }
        public static int HsvToRgb(real h, real s, real v) { HsvToRgb(h, s, v, out real r, out real g, out real b); return SatPack(r, g, b); }
        public static real HsvToRgb_R(real h, real s, real v) { HsvToRgb(h, s, v, out real r, out _, out _); return r; }
        public static real HsvToRgb_G(real h, real s, real v) { HsvToRgb(h, s, v, out _, out real g, out _); return g; }
        public static real HsvToRgb_B(real h, real s, real v) { HsvToRgb(h, s, v, out _, out _, out real b); return b; }

        public static real[] RgbToHsl(real r, real g, real b) {
            var min = RMath.Min(r, RMath.Min(g, b));
            var max = RMath.Max(r, RMath.Max(g, b));
            real h, s, l;
            h = rgbToHue(r, g, b, min, max);
            var p = (255 - RMath.Abs(max + min - 255));
            if (p == 0) {
                s = 0;
            }
            else {
                s = 100 * (max - min) / p;
            }
            l = 100 * (max + min) / (255 * 2);
            return new[] { h, s, l };
        }
        public static real[] RgbToHsl(real rgb) {
            Unpack(rgb, out int r, out int g, out int b);
            return RgbToHsl(r, g, b);
        }

        public static void HslToRgb(real h, real s, real l, out real r, out real g, out real b) {
            h = h % 360;
            s = RMath.Max(0, RMath.Min(100, s)) / 100;
            l = RMath.Max(0, RMath.Min(100, l)) / 100;

            var f = (h / 60) % 1;
            var max = 255 * (l + s * (1 - RMath.Abs(2 * l - 1)) / 2);
            var min = 255 * (l - s * (1 - RMath.Abs(2 * l - 1)) / 2);
            var x = min + (max - min) * f;
            var y = min + (max - min) * (1 - f);

            if (s == 0) {
                r = max; g = max; b = max;
            }
            else if (h < 60) {
                r = max; g = x; b = min;
            }
            else if (h < 120) {
                r = y; g = max; b = min;
            }
            else if (h < 180) {
                r = min; g = max; b = x;
            }
            else if (h < 240) {
                r = min; g = y; b = max;
            }
            else if (h < 300) {
                r = x; g = min; b = max;
            }
            else {
                r = max; g = min; b = y;
            }
        }
        public static int HslToRgb(real h, real s, real l) { HslToRgb(h, s, l, out real r, out real g, out real b); return SatPack(r, g, b); }
        public static real HslToRgb_R(real h, real s, real l) { HslToRgb(h, s, l, out real r, out _, out _); return r; }
        public static real HslToRgb_G(real h, real s, real l) { HslToRgb(h, s, l, out _, out real g, out _); return g; }
        public static real HslToRgb_B(real h, real s, real l) { HslToRgb(h, s, l, out _, out _, out real b); return b; }

        public static void RgbToYuv(real r, real g, real b, out real y, out real u, out real v) {
            y = 0.257m * r + 0.504m * g + 0.098m * b + 16;
            u = -0.148m * r - 0.291m * g + 0.439m * b + 128;
            v = 0.439m * r - 0.368m * g - 0.071m * b + 128;
        }
        public static void RgbToYuv(real rgb, out real y, out real u, out real v) {
            Unpack(rgb, out int r, out int g, out int b);
            RgbToYuv(r, g, b, out y, out u, out v);
        }
        public static int RgbToYuv(real r, real g, real b) { RgbToYuv(r, g, b, out real y, out real u, out real v); return SatPack(y, u, v); }
        public static int RgbToYuv(real rgb) { Unpack(rgb, out int r, out int g, out int b); return RgbToYuv(r, g, b); }
        public static real RgbToYuv_Y(real rgb) { RgbToYuv(rgb, out real y, out _, out _); return y; }
        public static real RgbToYuv_U(real rgb) { RgbToYuv(rgb, out _, out real u, out _); return u; }
        public static real RgbToYuv_V(real rgb) { RgbToYuv(rgb, out _, out _, out real v); return v; }

        public static void YuvToRgb(real y, real u, real v, out real r, out real g, out real b) {
            y -= 16;
            u -= 128;
            v -= 128;
            r = 1.164383m * y + 1.596027m * v;
            g = 1.164383m * y - 0.391762m * u - 0.812968m * v;
            b = 1.164383m * y + 2.017232m * u;
        }
        public static void YuvToRgb(real yuv, out real r, out real g, out real b) {
            Unpack(yuv, out int y, out int u, out int v);
            YuvToRgb(y, u, v, out r, out g, out b);
        }
        public static int YuvToRgb(real y, real u, real v) { YuvToRgb(y, u, v, out real r, out real g, out real b); return SatPack(r, g, b); }
        public static int YuvToRgb(real yuv) { YuvToRgb(yuv, out real r, out real g, out real b); return SatPack(r, g, b); }
        public static real YuvToRgb_R(real yuv) { YuvToRgb(yuv, out real r, out _, out _); return r; }
        public static real YuvToRgb_G(real yuv) { YuvToRgb(yuv, out _, out real g, out _); return g; }
        public static real YuvToRgb_B(real yuv) { YuvToRgb(yuv, out _, out _, out real b); return b; }

    }
}

