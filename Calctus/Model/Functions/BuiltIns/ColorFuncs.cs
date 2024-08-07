using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class ColorFuncs : BuiltInFuncCategory {
        private static ColorFuncs _instance = null;
        public static ColorFuncs Instance => _instance != null ? _instance : _instance = new ColorFuncs();
        private ColorFuncs() { }

        public readonly BuiltInFuncDef rgb_3 = new BuiltInFuncDef("rgb(r, g, b)",
            "Generates 24 bit color value from R, G, B.",
            (e, a) => ColorSpace.SatPack(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToColorVal());

        public readonly BuiltInFuncDef rgb_1 = new BuiltInFuncDef("rgb(*rgb)",
            "Converts the `rgb` to web-color representation.",
            (e, a) => a[0].Format(FormatHint.WebColor));

        public readonly BuiltInFuncDef hsv2rgb = new BuiltInFuncDef("hsv2rgb(h, s, v)",
            "Converts from H, S, V to 24 bit RGB color value.",
            (e, a) => ColorSpace.HsvToRgb(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToColorVal());

        public readonly BuiltInFuncDef rgb2hsv = new BuiltInFuncDef("rgb2hsv(*rgb)",
            "Converts the 24 bit RGB color value to HSV.",
            (e, a) => ColorSpace.RgbToHsv(a[0].AsDecimal).ToVal());

        public readonly BuiltInFuncDef hsl2rgb = new BuiltInFuncDef("hsl2rgb(h, s, l)",
            "Convert from H, S, L to 24 bit color RGB value.",
            (e, a) => ColorSpace.HslToRgb(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToColorVal());

        public readonly BuiltInFuncDef rgb2hsl = new BuiltInFuncDef("rgb2hsl(*rgb)",
            "Converts the 24 bit RGB color value to HSL.",
            (e, a) => ColorSpace.RgbToHsl(a[0].AsDecimal).ToVal());

        public readonly BuiltInFuncDef yuv2rgb_3 = new BuiltInFuncDef("yuv2rgb(y, u, v)",
            "Converts Y, U, V to 24 bit RGB color.",
            (e, a) => ColorSpace.YuvToRgb(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToColorVal());

        public readonly BuiltInFuncDef yuv2rgb_1 = new BuiltInFuncDef("yuv2rgb(*yuv)",
            "Converts the 24 bit YUV color to 24 bit RGB.",
            (e, a) => ColorSpace.YuvToRgb(a[0].AsDecimal).ToColorVal());

        public readonly BuiltInFuncDef rgb2yuv_3 = new BuiltInFuncDef("rgb2yuv(r, g, b)",
            "Converts R, G, B to 24 bit YUV color.",
            (e, a) => ColorSpace.RgbToYuv(a[0].AsDecimal, a[1].AsDecimal, a[2].AsDecimal).ToHexVal());

        public readonly BuiltInFuncDef rgb2yuv_1 = new BuiltInFuncDef("rgb2yuv(*rgb)",
            "Converts 24bit RGB color to 24 bit YUV.",
            (e, a) => ColorSpace.RgbToYuv(a[0].AsDecimal).ToHexVal());

        public readonly BuiltInFuncDef rgbTo565 = new BuiltInFuncDef("rgbTo565(*rgb)",
            "Downconverts RGB888 color to RGB565.",
            (e, a) => ColorSpace.Rgb888To565(a[0].AsInt).ToHexVal());

        public readonly BuiltInFuncDef rgbFrom565 = new BuiltInFuncDef("rgbFrom565(*rgb)",
            "Upconverts RGB565 color to RGB888.",
            (e, a) => ColorSpace.Rgb565To888(a[0].AsInt).ToColorVal());

        public readonly BuiltInFuncDef pack565 = new BuiltInFuncDef("pack565(x, y, z)",
            "Packs the 3 values to an RGB565 color.",
            (e, a) => ColorSpace.Pack565(a[0].AsInt, a[1].AsInt, a[2].AsInt).ToHexVal());

        public readonly BuiltInFuncDef unpack565 = new BuiltInFuncDef("unpack565(*x)",
            "Unpacks the RGB565 color to 3 values.",
            (e, a) => ColorSpace.Unpack565(a[0].AsInt).ToVal());
    }
}
