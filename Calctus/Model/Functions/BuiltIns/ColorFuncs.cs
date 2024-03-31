using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class ColorFuncs {
        public static readonly BuiltInFuncDef rgb_3 = new BuiltInFuncDef("rgb(r, g, b)", (e, a) => new RealVal(ColorSpace.SatPack(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Generates 24 bit color value from R, G, B.");
        public static readonly BuiltInFuncDef rgb_1 = new BuiltInFuncDef("rgb(*rgb)", (e, a) => a[0].FormatWebColor(), "Converts the `rgb` to web-color representation.");

        public static readonly BuiltInFuncDef hsv2rgb = new BuiltInFuncDef("hsv2rgb(h, s, v)", (e, a) => new RealVal(ColorSpace.HsvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts from H, S, V to 24 bit RGB color value.");
        public static readonly BuiltInFuncDef rgb2hsv = new BuiltInFuncDef("rgb2hsv(*rgb)", (e, a) => new ArrayVal(ColorSpace.RgbToHsv(a[0].AsReal)), "Converts the 24 bit RGB color value to HSV.");

        public static readonly BuiltInFuncDef hsl2rgb = new BuiltInFuncDef("hsl2rgb(h, s, l)", (e, a) => new RealVal(ColorSpace.HslToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Convert from H, S, L to 24 bit color RGB value.");
        public static readonly BuiltInFuncDef rgb2hsl = new BuiltInFuncDef("rgb2hsl(*rgb)", (e, a) => new ArrayVal(ColorSpace.RgbToHsl(a[0].AsReal)), "Converts the 24 bit RGB color value to HSL.");

        public static readonly BuiltInFuncDef yuv2rgb_3 = new BuiltInFuncDef("yuv2rgb(y, u, v)", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts Y, U, V to 24 bit RGB color.");
        public static readonly BuiltInFuncDef yuv2rgb_1 = new BuiltInFuncDef("yuv2rgb(*yuv)", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal)).FormatWebColor(), "Converts the 24 bit YUV color to 24 bit RGB.");

        public static readonly BuiltInFuncDef rgb2yuv_3 = new BuiltInFuncDef("rgb2yuv(r, g, b)", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatHex(), "Converts R, G, B to 24 bit YUV color.");
        public static readonly BuiltInFuncDef rgb2yuv_1 = new BuiltInFuncDef("rgb2yuv(*rgb)", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal)).FormatHex(), "Converts 24bit RGB color to 24 bit YUV.");

        public static readonly BuiltInFuncDef rgbto565 = new BuiltInFuncDef("rgbTo565(*rgb)", (e, a) => new RealVal(ColorSpace.Rgb888To565(a[0].AsInt)).FormatHex(), "Downconverts RGB888 color to RGB565.");
        public static readonly BuiltInFuncDef rgbfrom565 = new BuiltInFuncDef("rgbFrom565(*rgb)", (e, a) => new RealVal(ColorSpace.Rgb565To888(a[0].AsInt)).FormatWebColor(), "Upconverts RGB565 color to RGB888.");

        public static readonly BuiltInFuncDef pack565 = new BuiltInFuncDef("pack565(x, y, z)", (e, a) => new RealVal(ColorSpace.Pack565(a[0].AsInt, a[1].AsInt, a[2].AsInt)).FormatHex(), "Packs the 3 values to an RGB565 color.");
        public static readonly BuiltInFuncDef unpack565 = new BuiltInFuncDef("unpack565(*x)", (e, a) => new ArrayVal(ColorSpace.Unpack565(a[0].AsInt)), "Unpacks the RGB565 color to 3 values.");

    }
}
