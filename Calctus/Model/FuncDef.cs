using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Standard;

namespace Shapoco.Calctus.Model {
    class FuncDef {
        public readonly string Name;
        public readonly int ArgCount;
        public readonly Func<EvalContext, Val[], Val> Call;
        public readonly string Description;

        public FuncDef(string name, int nargs, Func<EvalContext, Val[], Val> method, string desc) {
            this.ArgCount = nargs;
            this.Name = name;
            this.Call = method;
            this.Description = desc;
        }

        public FuncDef(string name, Func<EvalContext, Val[], Val> method, string desc) : this(name, 1, method, desc) { }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(Name);
            sb.Append('(');
            for (int i = 0; i < ArgCount; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append((char)('a' + i));
            }
            sb.Append(')');
            return sb.ToString();
        }

        public static readonly FuncDef dec = new FuncDef("dec", (e, a) => a[0].FormatInt(), "convert the value to decimal representation");
        public static readonly FuncDef hex = new FuncDef("hex", (e, a) => a[0].FormatHex(), "convert the value to hexdecimal representation");
        public static readonly FuncDef bin = new FuncDef("bin", (e, a) => a[0].FormatBin(), "convert the value to binary representation");
        public static readonly FuncDef oct = new FuncDef("oct", (e, a) => a[0].FormatOct(), "convert the value to octal representation");
        public static readonly FuncDef si = new FuncDef("si", (e, a) => a[0].FormatSiPrefix(), "convert the value to Si prefix representation");
        public static readonly FuncDef char_1 = new FuncDef("char", (e, a) => a[0].FormatChar(), "convert the value to character representation");
        public static readonly FuncDef datetime = new FuncDef("datetime", (e, a) => a[0].FormatDateTime(), "convert the value to datetime representation");
        
        public static readonly FuncDef real = new FuncDef("real", (e, a) => a[0].AsRealVal().FormatReal(), "convert the value to a real number");
        public static readonly FuncDef frac = new FuncDef("frac", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, decimal.MaxValue, decimal.MaxValue)), "convert value to a fraction");
        public static readonly FuncDef frac_2 = new FuncDef("frac", 2, (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal), a[0].FormatHint), "convert value to a fraction");
        public static readonly FuncDef frac_3 = new FuncDef("frac", 3, (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[2].AsReal), a[0].FormatHint), "convert value to a fraction");

        public static readonly FuncDef pow = new FuncDef("pow", 2, (e, a) => new RealVal(RMath.Pow(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "power");
        public static readonly FuncDef sqrt = new FuncDef("sqrt", (e, a) => new RealVal(RMath.Sqrt(a[0].AsReal)), "square root");
        public static readonly FuncDef log = new FuncDef("log", (e, a) => new RealVal(RMath.Log(a[0].AsReal)), "logarithm");
        public static readonly FuncDef log2 = new FuncDef("log2", (e, a) => new RealVal(RMath.Log2(a[0].AsReal)), "binary logarithm");
        public static readonly FuncDef log10 = new FuncDef("log10", (e, a) => new RealVal(RMath.Log10(a[0].AsReal)), "common logarithm");
        public static readonly FuncDef clog2 = new FuncDef("clog2", (e, a) => new RealVal(RMath.Ceiling(RMath.Log2(a[0].AsReal))).FormatInt(), "ceiling of binary logarithm");
        public static readonly FuncDef clog10 = new FuncDef("clog10", (e, a) => new RealVal(RMath.Ceiling(RMath.Log10(a[0].AsReal))).FormatInt(), "ceiling of common logarithm");

        public static readonly FuncDef sin = new FuncDef("sin", (e, a) => new RealVal(RMath.Sin(a[0].AsReal)), "sine");
        public static readonly FuncDef cos = new FuncDef("cos", (e, a) => new RealVal(RMath.Cos(a[0].AsReal)), "cosine");
        public static readonly FuncDef tan = new FuncDef("tan", (e, a) => new RealVal(RMath.Tan(a[0].AsReal)), "tangent");
        public static readonly FuncDef asin = new FuncDef("asin", (e, a) => new RealVal(RMath.Asin(a[0].AsReal)), "arcsine");
        public static readonly FuncDef acos = new FuncDef("acos", (e, a) => new RealVal(RMath.Acos(a[0].AsReal)), "arccosine");
        public static readonly FuncDef atan = new FuncDef("atan", (e, a) => new RealVal(RMath.Atan(a[0].AsReal)), "arctangent");
        public static readonly FuncDef atan2 = new FuncDef("atan2", 2, (e, a) => new RealVal(RMath.Atan2(a[0].AsReal, a[1].AsReal)), "arctangent");
        public static readonly FuncDef sinh = new FuncDef("sinh", (e, a) => new RealVal(RMath.Sinh(a[0].AsReal)), "hyperbolic sine");
        public static readonly FuncDef cosh = new FuncDef("cosh", (e, a) => new RealVal(RMath.Cosh(a[0].AsReal)), "hyperbolic cosine");
        public static readonly FuncDef tanh = new FuncDef("tanh", (e, a) => new RealVal(RMath.Tanh(a[0].AsReal)), "hyperbolic tangent");

        public static readonly FuncDef floor = new FuncDef("floor", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint).FormatInt(), "floor");
        public static readonly FuncDef ceil = new FuncDef("ceil", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint).FormatInt(), "ceiling");
        public static readonly FuncDef trunc = new FuncDef("trunc", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint).FormatInt(), "truncate");
        public static readonly FuncDef round = new FuncDef("round", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint).FormatInt(), "round");

        public static readonly FuncDef abs = new FuncDef("abs", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint), "absolute");
        public static readonly FuncDef sign = new FuncDef("sign", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt(), "sign");

        public static readonly FuncDef gcd = new FuncDef("gcd", 2, (e, a) => new RealVal(RMath.Gcd(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "greatest common divisor");
        public static readonly FuncDef lcm = new FuncDef("lcm", 2, (e, a) => new RealVal(RMath.Lcm(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "least common multiple");

        public static readonly FuncDef max = new FuncDef("max", 2, (e, a) => new RealVal(RMath.Max(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "maximum");
        public static readonly FuncDef min = new FuncDef("min", 2, (e, a) => new RealVal(RMath.Min(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "minimum");

        public static readonly FuncDef now = new FuncDef("now",0, (e, a) => new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime(), "now");

        public static readonly FuncDef toyears = new FuncDef("toyears", (e, a) => a[0].Div(e, new RealVal(365.2425m * 24 * 60 * 60)).FormatReal(), "convert from epoch time to years");
        public static readonly FuncDef todays = new FuncDef("todays", (e, a) => a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal(), "convert from epoch time to days");
        public static readonly FuncDef tohours = new FuncDef("tohours", (e, a) => a[0].Div(e, new RealVal(60 * 60)).FormatReal(), "convert from epoch time to hours");
        public static readonly FuncDef tominutes = new FuncDef("tominutes", (e, a) => a[0].Div(e, new RealVal(60)).FormatReal(), "convert from epoch time to minutes");
        public static readonly FuncDef toseconds = new FuncDef("toseconds", (e, a) => a[0].FormatReal(), "convert from epoch time to seconds");
        
        public static readonly FuncDef fromyears = new FuncDef("fromyears", (e, a) => a[0].Mul(e, new RealVal(365.2425m * 24 * 60 * 60)).FormatDateTime(), "convert from years to epoch time");
        public static readonly FuncDef fromdays = new FuncDef("fromdays", (e, a) => a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatDateTime(), "convert from days to epoch time");
        public static readonly FuncDef fromhours = new FuncDef("fromhours", (e, a) => a[0].Mul(e, new RealVal(60 * 60)).FormatDateTime(), "convert from hours to epoch time");
        public static readonly FuncDef fromminutes = new FuncDef("fromminutes", (e, a) => a[0].Mul(e, new RealVal(60)).FormatDateTime(), "convert from minutes to epoch time");
        public static readonly FuncDef fromseconds = new FuncDef("fromseconds", (e, a) => a[0].FormatDateTime(), "convert from seconds to epoch time");

        public static readonly FuncDef e3floor = new FuncDef("e3floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series floor");
        public static readonly FuncDef e3ceil = new FuncDef("e3ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series ceiling");
        public static readonly FuncDef e3round = new FuncDef("e3round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series round");
        public static readonly FuncDef e3ratio_l = new FuncDef("e3ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[0]), "low-side of E3 series value of divider resistor");
        public static readonly FuncDef e3ratio_h = new FuncDef("e3ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[1]), "high-side of E3 series value of divider resistor");

        public static readonly FuncDef e6floor = new FuncDef("e6floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series floor");
        public static readonly FuncDef e6ceil = new FuncDef("e6ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series ceiling");
        public static readonly FuncDef e6round = new FuncDef("e6round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series round");
        public static readonly FuncDef e6ratio_l = new FuncDef("e6ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[0]), "low-side of E6 series value of divider resistor");
        public static readonly FuncDef e6ratio_h = new FuncDef("e6ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[1]), "high-side of E6 series value of divider resistor");

        public static readonly FuncDef e12floor = new FuncDef("e12floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series floor");
        public static readonly FuncDef e12ceil = new FuncDef("e12ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series ceiling");
        public static readonly FuncDef e12round = new FuncDef("e12round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series round");
        public static readonly FuncDef e12ratio_l = new FuncDef("e12ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[0]), "low-side of E12 series value of divider resistor");
        public static readonly FuncDef e12ratio_h = new FuncDef("e12ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[1]), "high-side of E12 series value of divider resistor");

        public static readonly FuncDef e24floor = new FuncDef("e24floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series floor");
        public static readonly FuncDef e24ceil = new FuncDef("e24ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series ceiling");
        public static readonly FuncDef e24round = new FuncDef("e24round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series round");
        public static readonly FuncDef e24ratio_l = new FuncDef("e24ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[0]), "low-side of E24 series value of divider resistor");
        public static readonly FuncDef e24ratio_h = new FuncDef("e24ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[1]), "high-side of E24 series value of divider resistor");

        public static readonly FuncDef e48floor = new FuncDef("e48floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series floor");
        public static readonly FuncDef e48ceil = new FuncDef("e48ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series ceiling");
        public static readonly FuncDef e48round = new FuncDef("e48round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series round");
        public static readonly FuncDef e48ratio_l = new FuncDef("e48ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[0]), "low-side of E48 series value of divider resistor");
        public static readonly FuncDef e48ratio_h = new FuncDef("e48ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[1]), "high-side of E48 series value of divider resistor");

        public static readonly FuncDef e96floor = new FuncDef("e96floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series floor");
        public static readonly FuncDef e96ceil = new FuncDef("e96ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series ceiling");
        public static readonly FuncDef e96round = new FuncDef("e96round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series round");
        public static readonly FuncDef e96ratio_l = new FuncDef("e96ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[0]), "low-side of E96 series value of divider resistor");
        public static readonly FuncDef e96ratio_h = new FuncDef("e96ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[1]), "high-side of E96 series value of divider resistor");

        public static readonly FuncDef e192floor = new FuncDef("e192floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series floor");
        public static readonly FuncDef e192ceil = new FuncDef("e192ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series ceiling");
        public static readonly FuncDef e192round = new FuncDef("e192round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series round");
        public static readonly FuncDef e192ratio_l = new FuncDef("e192ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[0]), "low-side of E192 series value of divider resistor");
        public static readonly FuncDef e192ratio_h = new FuncDef("e192ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[1]), "high-side of E192 series value of divider resistor");

        public static readonly FuncDef rgb_3 = new FuncDef("rgb", 3, (e, a) => new RealVal(ColorSpace.SatPack(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "generate 24 bit value from R, G, B");
        public static readonly FuncDef rgb_1 = new FuncDef("rgb", (e, a) => a[0].FormatWebColor(), "convert the value to web-color representation");

        public static readonly FuncDef hsv2rgb = new FuncDef("hsv2rgb", 3, (e, a) => new RealVal(ColorSpace.HsvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "convert from H, S, V to 24 bit RGB value");
        public static readonly FuncDef hsv2rgb_r = new FuncDef("hsv2rgb_r", (e, a) => new RealVal(ColorSpace.HsvToRgb_R(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSV value to R");
        public static readonly FuncDef hsv2rgb_g = new FuncDef("hsv2rgb_g", (e, a) => new RealVal(ColorSpace.HsvToRgb_G(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSV value to G");
        public static readonly FuncDef hsv2rgb_b = new FuncDef("hsv2rgb_b", (e, a) => new RealVal(ColorSpace.HsvToRgb_B(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSV value to B");

        public static readonly FuncDef rgb2hsv_h = new FuncDef("rgb2hsv_h", (e, a) => new RealVal(ColorSpace.RgbToHsv_H(a[0].AsReal)), "convert the 24 bit RGB value to H");
        public static readonly FuncDef rgb2hsv_s = new FuncDef("rgb2hsv_s", (e, a) => new RealVal(ColorSpace.RgbToHsv_S(a[0].AsReal)), "convert the 24 bit RGB value to S");
        public static readonly FuncDef rgb2hsv_v = new FuncDef("rgb2hsv_v", (e, a) => new RealVal(ColorSpace.RgbToHsv_V(a[0].AsReal)), "convert the 24 bit RGB value to V");

        public static readonly FuncDef hsl2rgb = new FuncDef("hsl2rgb", 3, (e, a) => new RealVal(ColorSpace.HslToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "convert from H, S, L to 24 bit RGB value");
        public static readonly FuncDef hsl2rgb_r = new FuncDef("hsl2rgb_r", (e, a) => new RealVal(ColorSpace.HslToRgb_R(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSL value to R");
        public static readonly FuncDef hsl2rgb_g = new FuncDef("hsl2rgb_g", (e, a) => new RealVal(ColorSpace.HslToRgb_G(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSL value to G");
        public static readonly FuncDef hsl2rgb_b = new FuncDef("hsl2rgb_b", (e, a) => new RealVal(ColorSpace.HslToRgb_B(a[0].AsReal, a[1].AsReal, a[2].AsReal)), "convert the 24 bit HSL value to B");

        public static readonly FuncDef rgb2hsl_h = new FuncDef("rgb2hsl_h", (e, a) => new RealVal(ColorSpace.RgbToHsl_H(a[0].AsReal)), "convert the 24 bit RGB value to H");
        public static readonly FuncDef rgb2hsl_s = new FuncDef("rgb2hsl_s", (e, a) => new RealVal(ColorSpace.RgbToHsl_S(a[0].AsReal)), "convert the 24 bit RGB value to S");
        public static readonly FuncDef rgb2hsl_l = new FuncDef("rgb2hsl_l", (e, a) => new RealVal(ColorSpace.RgbToHsl_L(a[0].AsReal)), "convert the 24 bit RGB value to L");

        public static readonly FuncDef yuv2rgb_3 = new FuncDef("yuv2rgb", 3, (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "convert Y, U, V to 24 bit RGB");
        public static readonly FuncDef yuv2rgb_1 = new FuncDef("yuv2rgb", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal)).FormatWebColor(), "convert the 24 bit YUV to 24 bit RGB");
        public static readonly FuncDef yuv2rgb_r = new FuncDef("yuv2rgb_r", (e, a) => new RealVal(ColorSpace.YuvToRgb_R(a[0].AsReal)), "convert the 24 bit YUV to R");
        public static readonly FuncDef yuv2rgb_g = new FuncDef("yuv2rgb_g", (e, a) => new RealVal(ColorSpace.YuvToRgb_G(a[0].AsReal)), "convert the 24 bit YUV to G");
        public static readonly FuncDef yuv2rgb_b = new FuncDef("yuv2rgb_b", (e, a) => new RealVal(ColorSpace.YuvToRgb_B(a[0].AsReal)), "convert the 24 bit YUV to B");

        public static readonly FuncDef rgb2yuv_3 = new FuncDef("rgb2yuv", 3, (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatHex(), "convert R, G, B to 24 bit YUV");
        public static readonly FuncDef rgb2yuv_1 = new FuncDef("rgb2yuv", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal)).FormatHex(), "convert 24bit RGB to 24 bit YUV");
        public static readonly FuncDef rgb2yuv_y = new FuncDef("rgb2yuv_y", (e, a) => new RealVal(ColorSpace.RgbToYuv_Y(a[0].AsReal)), "convert the 24 bit RGB to Y");
        public static readonly FuncDef rgb2yuv_u = new FuncDef("rgb2yuv_u", (e, a) => new RealVal(ColorSpace.RgbToYuv_U(a[0].AsReal)), "convert the 24 bit RGB to U");
        public static readonly FuncDef rgb2yuv_v = new FuncDef("rgb2yuv_v", (e, a) => new RealVal(ColorSpace.RgbToYuv_V(a[0].AsReal)), "convert the 24 bit RGB to V");

        /// <summary>ネイティブ関数の一覧</summary>
        public static FuncDef[] NativeFunctions = EnumFunctions().ToArray();
        private static IEnumerable<FuncDef> EnumFunctions() {
            return
                typeof(FuncDef)
                .GetFields()
                .Where(p => p.IsStatic && (p.FieldType == typeof(FuncDef)))
                .Select(p => (FuncDef)p.GetValue(null));
        }

        /// <summary>指定された条件にマッチするネイティブ関数を返す</summary>
        public static FuncDef Match(Token tok, int numArgs) {
            var f = NativeFunctions.FirstOrDefault(p => p.Name == tok.Text && p.ArgCount == numArgs);
            if (f == null) {
                throw new Shapoco.Calctus.Parser.SyntaxError(tok.Position, "function " + tok + "(" + numArgs + ") was not found.");
            }
            return f;
        }
    }
}
