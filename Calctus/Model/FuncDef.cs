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

        public FuncDef(string name, int nargs, Func<EvalContext, Val[], Val> method) {
            this.ArgCount = nargs;
            this.Name = name;
            this.Call = method;
        }

        public FuncDef(string name, Func<EvalContext, Val[], Val> method) : this(name, 1, method) { }

        public static readonly FuncDef dec = new FuncDef("dec", (e, a) => a[0].FormatInt());
        public static readonly FuncDef hex = new FuncDef("hex", (e, a) => a[0].FormatHex());
        public static readonly FuncDef bin = new FuncDef("bin", (e, a) => a[0].FormatBin());
        public static readonly FuncDef oct = new FuncDef("oct", (e, a) => a[0].FormatOct());
        public static readonly FuncDef char_1 = new FuncDef("char", (e, a) => a[0].FormatChar());
        public static readonly FuncDef datetime = new FuncDef("datetime", (e, a) => a[0].FormatDateTime());
        
        public static readonly FuncDef real = new FuncDef("real", (e, a) => a[0].AsRealVal());
        public static readonly FuncDef frac = new FuncDef("frac", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, decimal.MaxValue, decimal.MaxValue)));
        public static readonly FuncDef frac_2 = new FuncDef("frac", 2, (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal), a[0].FormatHint));
        public static readonly FuncDef frac_3 = new FuncDef("frac", 3, (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[2].AsReal), a[0].FormatHint));

        public static readonly FuncDef pow = new FuncDef("pow", 2, (e, a) => new RealVal(RMath.Pow(a[0].AsReal, a[1].AsReal), a[0].FormatHint));
        public static readonly FuncDef sqrt = new FuncDef("sqrt", (e, a) => new RealVal(RMath.Sqrt(a[0].AsReal)));
        public static readonly FuncDef log = new FuncDef("log", (e, a) => new RealVal(RMath.Log(a[0].AsReal)));
        public static readonly FuncDef log2 = new FuncDef("log2", (e, a) => new RealVal(RMath.Log(a[0].AsReal) / RMath.Log(2.0m)));
        public static readonly FuncDef log10 = new FuncDef("log10", (e, a) => new RealVal(RMath.Log10(a[0].AsReal)));
        public static readonly FuncDef clog2 = new FuncDef("clog2", (e, a) => new RealVal(RMath.Ceiling(RMath.Log(a[0].AsReal) / RMath.Log(2.0m))).FormatInt());
        public static readonly FuncDef clog10 = new FuncDef("clog10", (e, a) => new RealVal(RMath.Ceiling(RMath.Log10(a[0].AsReal))).FormatInt());

        public static readonly FuncDef sin = new FuncDef("sin", (e, a) => new RealVal(RMath.Sin(a[0].AsReal)));
        public static readonly FuncDef cos = new FuncDef("cos", (e, a) => new RealVal(RMath.Cos(a[0].AsReal)));
        public static readonly FuncDef tan = new FuncDef("tan", (e, a) => new RealVal(RMath.Tan(a[0].AsReal)));
        public static readonly FuncDef asin = new FuncDef("asin", (e, a) => new RealVal(RMath.Asin(a[0].AsReal)));
        public static readonly FuncDef acos = new FuncDef("acos", (e, a) => new RealVal(RMath.Acos(a[0].AsReal)));
        public static readonly FuncDef atan = new FuncDef("atan", (e, a) => new RealVal(RMath.Atan(a[0].AsReal)));
        public static readonly FuncDef atan2 = new FuncDef("atan2", 2, (e, a) => new RealVal(RMath.Atan2(a[0].AsReal, a[1].AsReal)));
        public static readonly FuncDef sinh = new FuncDef("sinh", (e, a) => new RealVal(RMath.Sinh(a[0].AsReal)));
        public static readonly FuncDef cosh = new FuncDef("cosh", (e, a) => new RealVal(RMath.Cosh(a[0].AsReal)));
        public static readonly FuncDef tanh = new FuncDef("tanh", (e, a) => new RealVal(RMath.Tanh(a[0].AsReal)));

        public static readonly FuncDef floor = new FuncDef("floor", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint).FormatInt());
        public static readonly FuncDef ceil = new FuncDef("ceil", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint).FormatInt());
        public static readonly FuncDef trunc = new FuncDef("trunc", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint).FormatInt());
        public static readonly FuncDef round = new FuncDef("round", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint).FormatInt());

        public static readonly FuncDef abs = new FuncDef("abs", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef sign = new FuncDef("sign", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt());

        public static readonly FuncDef gcd = new FuncDef("gcd", 2, (e, a) => new RealVal(RMath.Gcd(a[0].AsReal, a[1].AsReal), a[0].FormatHint));
        public static readonly FuncDef lcm = new FuncDef("lcm", 2, (e, a) => new RealVal(RMath.Lcm(a[0].AsReal, a[1].AsReal), a[0].FormatHint));

        public static readonly FuncDef max = new FuncDef("max", 2, (e, a) => new RealVal(RMath.Max(a[0].AsReal, a[1].AsReal), a[0].FormatHint));
        public static readonly FuncDef min = new FuncDef("min", 2, (e, a) => new RealVal(RMath.Min(a[0].AsReal, a[1].AsReal), a[0].FormatHint));

        public static readonly FuncDef now = new FuncDef("now",0, (e, a) => new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime());

        public static readonly FuncDef toyears = new FuncDef("toyears", (e, a) => a[0].Div(e, new RealVal(365.2425m * 24 * 60 * 60)).FormatReal());
        public static readonly FuncDef todays = new FuncDef("todays", (e, a) => a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal());
        public static readonly FuncDef tohours = new FuncDef("tohours", (e, a) => a[0].Div(e, new RealVal(60 * 60)).FormatReal());
        public static readonly FuncDef tominutes = new FuncDef("tominutes", (e, a) => a[0].Div(e, new RealVal(60)).FormatReal());
        public static readonly FuncDef toseconds = new FuncDef("toseconds", (e, a) => a[0].FormatReal());
        
        public static readonly FuncDef fromyears = new FuncDef("fromyears", (e, a) => a[0].Mul(e, new RealVal(365.2425m * 24 * 60 * 60)).FormatReal());
        public static readonly FuncDef fromdays = new FuncDef("fromdays", (e, a) => a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatReal());
        public static readonly FuncDef fromhours = new FuncDef("fromhours", (e, a) => a[0].Mul(e, new RealVal(60 * 60)).FormatReal());
        public static readonly FuncDef fromminutes = new FuncDef("fromminutes", (e, a) => a[0].Mul(e, new RealVal(60)).FormatReal());
        public static readonly FuncDef fromseconds = new FuncDef("fromseconds", (e, a) => a[0].FormatReal());

        public static readonly FuncDef e3floor = new FuncDef("e3floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e3ceil = new FuncDef("e3ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e3round = new FuncDef("e3round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e3ratio_l = new FuncDef("e3ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[0]));
        public static readonly FuncDef e3ratio_h = new FuncDef("e3ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[1]));

        public static readonly FuncDef e6floor = new FuncDef("e6floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e6ceil = new FuncDef("e6ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e6round = new FuncDef("e6round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e6ratio_l = new FuncDef("e6ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[0]));
        public static readonly FuncDef e6ratio_h = new FuncDef("e6ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[1]));

        public static readonly FuncDef e12floor = new FuncDef("e12floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e12ceil = new FuncDef("e12ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e12round = new FuncDef("e12round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e12ratio_l = new FuncDef("e12ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[0]));
        public static readonly FuncDef e12ratio_h = new FuncDef("e12ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[1]));

        public static readonly FuncDef e24floor = new FuncDef("e24floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e24ceil = new FuncDef("e24ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e24round = new FuncDef("e24round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e24ratio_l = new FuncDef("e24ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[0]));
        public static readonly FuncDef e24ratio_h = new FuncDef("e24ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[1]));

        public static readonly FuncDef e48floor = new FuncDef("e48floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e48ceil = new FuncDef("e48ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e48round = new FuncDef("e48round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e48ratio_l = new FuncDef("e48ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[0]));
        public static readonly FuncDef e48ratio_h = new FuncDef("e48ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[1]));

        public static readonly FuncDef e96floor = new FuncDef("e96floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e96ceil = new FuncDef("e96ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e96round = new FuncDef("e96round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e96ratio_l = new FuncDef("e96ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[0]));
        public static readonly FuncDef e96ratio_h = new FuncDef("e96ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[1]));

        public static readonly FuncDef e192floor = new FuncDef("e192floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e192ceil = new FuncDef("e192ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e192round = new FuncDef("e192round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint));
        public static readonly FuncDef e192ratio_l = new FuncDef("e192ratio_l", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[0]));
        public static readonly FuncDef e192ratio_h = new FuncDef("e192ratio_h", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[1]));

        public static readonly FuncDef rgb_3 = new FuncDef("rgb", 3, (e, a) => new RealVal(ColorSpace.SatPack(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor());
        public static readonly FuncDef rgb_1 = new FuncDef("rgb", (e, a) => a[0].FormatWebColor());

        public static readonly FuncDef hsv2rgb = new FuncDef("hsv2rgb", 3, (e, a) => new RealVal(ColorSpace.HsvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor());
        public static readonly FuncDef hsv2rgb_r = new FuncDef("hsv2rgb_r", (e, a) => new RealVal(ColorSpace.HsvToRgb_R(a[0].AsReal, a[1].AsReal, a[2].AsReal)));
        public static readonly FuncDef hsv2rgb_g = new FuncDef("hsv2rgb_g", (e, a) => new RealVal(ColorSpace.HsvToRgb_G(a[0].AsReal, a[1].AsReal, a[2].AsReal)));
        public static readonly FuncDef hsv2rgb_b = new FuncDef("hsv2rgb_b", (e, a) => new RealVal(ColorSpace.HsvToRgb_B(a[0].AsReal, a[1].AsReal, a[2].AsReal)));

        public static readonly FuncDef rgb2hsv_h = new FuncDef("rgb2hsv_h", (e, a) => new RealVal(ColorSpace.RgbToHsv_H(a[0].AsReal)));
        public static readonly FuncDef rgb2hsv_s = new FuncDef("rgb2hsv_s", (e, a) => new RealVal(ColorSpace.RgbToHsv_S(a[0].AsReal)));
        public static readonly FuncDef rgb2hsv_v = new FuncDef("rgb2hsv_v", (e, a) => new RealVal(ColorSpace.RgbToHsv_V(a[0].AsReal)));

        public static readonly FuncDef hsl2rgb = new FuncDef("hsl2rgb", 3, (e, a) => new RealVal(ColorSpace.HslToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor());
        public static readonly FuncDef hsl2rgb_r = new FuncDef("hsl2rgb_r", (e, a) => new RealVal(ColorSpace.HslToRgb_R(a[0].AsReal, a[1].AsReal, a[2].AsReal)));
        public static readonly FuncDef hsl2rgb_g = new FuncDef("hsl2rgb_g", (e, a) => new RealVal(ColorSpace.HslToRgb_G(a[0].AsReal, a[1].AsReal, a[2].AsReal)));
        public static readonly FuncDef hsl2rgb_b = new FuncDef("hsl2rgb_b", (e, a) => new RealVal(ColorSpace.HslToRgb_B(a[0].AsReal, a[1].AsReal, a[2].AsReal)));

        public static readonly FuncDef rgb2hsl_h = new FuncDef("rgb2hsl_h", (e, a) => new RealVal(ColorSpace.RgbToHsl_H(a[0].AsReal)));
        public static readonly FuncDef rgb2hsl_s = new FuncDef("rgb2hsl_s", (e, a) => new RealVal(ColorSpace.RgbToHsl_S(a[0].AsReal)));
        public static readonly FuncDef rgb2hsl_l = new FuncDef("rgb2hsl_l", (e, a) => new RealVal(ColorSpace.RgbToHsl_L(a[0].AsReal)));

        public static readonly FuncDef yuv2rgb_3 = new FuncDef("yuv2rgb", 3, (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor());
        public static readonly FuncDef yuv2rgb_1 = new FuncDef("yuv2rgb", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal)).FormatWebColor());
        public static readonly FuncDef yuv2rgb_r = new FuncDef("yuv2rgb_r", (e, a) => new RealVal(ColorSpace.YuvToRgb_R(a[0].AsReal)));
        public static readonly FuncDef yuv2rgb_g = new FuncDef("yuv2rgb_g", (e, a) => new RealVal(ColorSpace.YuvToRgb_G(a[0].AsReal)));
        public static readonly FuncDef yuv2rgb_b = new FuncDef("yuv2rgb_b", (e, a) => new RealVal(ColorSpace.YuvToRgb_B(a[0].AsReal)));

        public static readonly FuncDef rgb2yuv_3 = new FuncDef("rgb2yuv", 3, (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatHex());
        public static readonly FuncDef rgb2yuv_1 = new FuncDef("rgb2yuv", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal)).FormatHex());
        public static readonly FuncDef rgb2yuv_y = new FuncDef("rgb2yuv_y", (e, a) => new RealVal(ColorSpace.RgbToYuv_Y(a[0].AsReal)));
        public static readonly FuncDef rgb2yuv_u = new FuncDef("rgb2yuv_u", (e, a) => new RealVal(ColorSpace.RgbToYuv_U(a[0].AsReal)));
        public static readonly FuncDef rgb2yuv_v = new FuncDef("rgb2yuv_v", (e, a) => new RealVal(ColorSpace.RgbToYuv_V(a[0].AsReal)));

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
