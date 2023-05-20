using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Standard;

namespace Shapoco.Calctus.Model {
    class FuncDef {
        public const int Variadic = -1;

        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public readonly string Name;
        public readonly int ArgCount;
        public Func<EvalContext, Val[], Val> Call { get; protected set; }
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
            if (ArgCount == Variadic) {
                sb.Append("...");
            }
            else {
                for (int i = 0; i < ArgCount; i++) {
                    if (i > 0) sb.Append(", ");
                    sb.Append((char)('a' + i));
                }
            }
            sb.Append(')');
            return sb.ToString();
        }

        public static readonly FuncDef dec = new FuncDef("dec", (e, a) => a[0].FormatInt(), "Converts the value to decimal representation.");
        public static readonly FuncDef hex = new FuncDef("hex", (e, a) => a[0].FormatHex(), "Converts the value to hexdecimal representation.");
        public static readonly FuncDef bin = new FuncDef("bin", (e, a) => a[0].FormatBin(), "Converts the value to binary representation.");
        public static readonly FuncDef oct = new FuncDef("oct", (e, a) => a[0].FormatOct(), "Converts the value to octal representation.");
        public static readonly FuncDef si = new FuncDef("si", (e, a) => a[0].FormatSiPrefix(), "Converts the value to SI prefixed representation.");
        public static readonly FuncDef bi = new FuncDef("bi", (e, a) => a[0].FormatBinaryPrefix(), "Converts the value to binary prefixed representation.");
        public static readonly FuncDef char_1 = new FuncDef("char", (e, a) => a[0].FormatChar(), "Converts the value to character representation.");
        public static readonly FuncDef datetime = new FuncDef("datetime", (e, a) => a[0].FormatDateTime(), "Converts the value to datetime representation.");

        public static readonly FuncDef real = new FuncDef("real", (e, a) => a[0].AsRealVal().FormatReal(), "Converts the value to a real number.");
        public static readonly FuncDef rat = new FuncDef("rat", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal)), "Rational fraction approximation.");
        public static readonly FuncDef rat_2 = new FuncDef("rat", 2, (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal), a[0].FormatHint), "Rational fraction approximation.");

        public static readonly FuncDef pow = new FuncDef("pow", 2, (e, a) => new RealVal(RMath.Pow(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "Power");
        public static readonly FuncDef sqrt = new FuncDef("sqrt", (e, a) => new RealVal(RMath.Sqrt(a[0].AsReal)), "Square root");
        public static readonly FuncDef log = new FuncDef("log", (e, a) => new RealVal(RMath.Log(a[0].AsReal)), "Logarithm");
        public static readonly FuncDef log2 = new FuncDef("log2", (e, a) => new RealVal(RMath.Log2(a[0].AsReal)), "Binary logarithm");
        public static readonly FuncDef log10 = new FuncDef("log10", (e, a) => new RealVal(RMath.Log10(a[0].AsReal)), "Common logarithm");
        public static readonly FuncDef clog2 = new FuncDef("clog2", (e, a) => new RealVal(RMath.Ceiling(RMath.Log2(a[0].AsReal))).FormatInt(), "Ceiling of binary logarithm");
        public static readonly FuncDef clog10 = new FuncDef("clog10", (e, a) => new RealVal(RMath.Ceiling(RMath.Log10(a[0].AsReal))).FormatInt(), "Ceiling of common logarithm");

        public static readonly FuncDef sin = new FuncDef("sin", (e, a) => new RealVal(RMath.Sin(a[0].AsReal)), "Sine");
        public static readonly FuncDef cos = new FuncDef("cos", (e, a) => new RealVal(RMath.Cos(a[0].AsReal)), "Cosine");
        public static readonly FuncDef tan = new FuncDef("tan", (e, a) => new RealVal(RMath.Tan(a[0].AsReal)), "Tangent");
        public static readonly FuncDef asin = new FuncDef("asin", (e, a) => new RealVal(RMath.Asin(a[0].AsReal)), "Arcsine");
        public static readonly FuncDef acos = new FuncDef("acos", (e, a) => new RealVal(RMath.Acos(a[0].AsReal)), "Arccosine");
        public static readonly FuncDef atan = new FuncDef("atan", (e, a) => new RealVal(RMath.Atan(a[0].AsReal)), "Arctangent");
        public static readonly FuncDef atan2 = new FuncDef("atan2", 2, (e, a) => new RealVal(RMath.Atan2(a[0].AsReal, a[1].AsReal)), "Arctangent of a / b");
        public static readonly FuncDef sinh = new FuncDef("sinh", (e, a) => new RealVal(RMath.Sinh(a[0].AsReal)), "Hyperbolic sine");
        public static readonly FuncDef cosh = new FuncDef("cosh", (e, a) => new RealVal(RMath.Cosh(a[0].AsReal)), "Hyperbolic cosine");
        public static readonly FuncDef tanh = new FuncDef("tanh", (e, a) => new RealVal(RMath.Tanh(a[0].AsReal)), "Hyperbolic tangent");

        public static readonly FuncDef floor = new FuncDef("floor", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint).FormatInt(), "Largest integral value less than or equal to a");
        public static readonly FuncDef ceil = new FuncDef("ceil", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint).FormatInt(), "Smallest integral value greater than or equal to a");
        public static readonly FuncDef trunc = new FuncDef("trunc", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint).FormatInt(), "Integral part of a");
        public static readonly FuncDef round = new FuncDef("round", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint).FormatInt(), "Nearest integer to a");

        public static readonly FuncDef abs = new FuncDef("abs", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint), "Absolute");
        public static readonly FuncDef sign = new FuncDef("sign", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt(), "Returns 1 for positives, -1 for negatives, 0 otherwise.");

        public static readonly FuncDef gcd = new FuncDef("gcd", 2, (e, a) => new RealVal(RMath.Gcd(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "Greatest common divisor");
        public static readonly FuncDef lcm = new FuncDef("lcm", 2, (e, a) => new RealVal(RMath.Lcm(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "Least common multiple");

        public static readonly FuncDef max = new FuncDef("max", Variadic, (e, a) => new RealVal(a.Max(p => p.AsReal), a[0].FormatHint), "Maximum value of the arguments");
        public static readonly FuncDef min = new FuncDef("min", Variadic, (e, a) => new RealVal(a.Min(p => p.AsReal), a[0].FormatHint), "Minimum value of the arguments");

        public static readonly FuncDef sum = new FuncDef("sum", Variadic, (e, a) => new RealVal(a.Sum(p => p.AsReal), a[0].FormatHint), "Sum of the arguments");
        public static readonly FuncDef ave = new FuncDef("ave", Variadic, (e, a) => new RealVal(a.Average(p => p.AsReal), a[0].FormatHint), "Arithmetic mean of the arguments");
        public static readonly FuncDef invsum = new FuncDef("invsum", Variadic, (e, a) => new RealVal(1m / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Inverse of the sum of the inverses");
        public static readonly FuncDef harmean = new FuncDef("harmean", Variadic, (e, a) => new RealVal((real)a.Length / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Harmonic mean of the arguments");
        public static readonly FuncDef geomean = new FuncDef("geomean", Variadic, (e, a) => {
            var prod = (real)1;
            foreach (var p in a) prod *= p.AsReal;
            return new RealVal(RMath.Pow(prod, 1m / a.Length), a[0].FormatHint);
        }, "Geometric mean of the arguments");

        public static readonly FuncDef pack = new FuncDef("pack", 2, (e, a) => new RealVal(LMath.Pack(a[0].AsInt, a[1].AsLongArray)).FormatHex(), "Packs the array elements to a value.");
        public static readonly FuncDef unpack = new FuncDef("unpack", 2, (e, a) => new ArrayVal(LMath.Unpack(a[0].AsInt, a[1].AsLong)).FormatInt(), "Unpacks the value to an array.");

        public static readonly FuncDef swapnib = new FuncDef("swapnib", (e, a) => new RealVal(LMath.SwapNibbles(a[0].AsLong), a[0].FormatHint), "Swaps the nibble of each byte.");
        public static readonly FuncDef swap2 = new FuncDef("swap2", (e, a) => new RealVal(LMath.Swap2(a[0].AsLong), a[0].FormatHint), "Swaps even and odd bytes.");
        public static readonly FuncDef swap4 = new FuncDef("swap4", (e, a) => new RealVal(LMath.Swap4(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 4 bytes.");
        public static readonly FuncDef swap8 = new FuncDef("swap8", (e, a) => new RealVal(LMath.Swap8(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 8 bytes.");
        public static readonly FuncDef reverse = new FuncDef("reverse", 2, (e, a) => new RealVal(LMath.Reverse(a[0].AsLong, a[1].AsInt), a[0].FormatHint), "Reverses the lower b bits of a.");
        public static readonly FuncDef reverseb = new FuncDef("reverseb", (e, a) => new RealVal(LMath.ReverseBytes(a[0].AsLong), a[0].FormatHint), "Reverses the order of bits of each byte.");
        public static readonly FuncDef rotatel = new FuncDef("rotatel", 2, (e, a) => new RealVal(LMath.RotateLeft(a[0].AsLong, a[1].AsInt), a[0].FormatHint), "Rotates left the lower b bits of a.");
        public static readonly FuncDef rotater = new FuncDef("rotater", 2, (e, a) => new RealVal(LMath.RotateRight(a[0].AsLong, a[1].AsInt), a[0].FormatHint), "Rotates right the lower b bits of a.");
        public static readonly FuncDef count1 = new FuncDef("count1", (e, a) => new RealVal(LMath.CountOnes(a[0].AsLong)).FormatInt(), "Number of bits that have the value 1.");

        public static readonly FuncDef xorreduce = new FuncDef("xorreduce", (e, a) => new RealVal(LMath.XorReduce(a[0].AsLong)).FormatInt(), "Reduction XOR (Same as even parity).");
        public static readonly FuncDef parity = new FuncDef("parity", (e, a) => new RealVal(LMath.OddParity(a[0].AsLong)).FormatInt(), "Odd parity.");

        public static readonly FuncDef eccwidth = new FuncDef("eccwidth", (e, a) => new RealVal(LMath.EccWidth(a[0].AsInt)).FormatInt(), "Width of ECC for a-bit data.");
        public static readonly FuncDef eccenc = new FuncDef("eccenc", 2, (e, a) => new RealVal(LMath.EccEncode(a[0].AsLong, a[1].AsInt)).FormatHex(), "Generate ECC code (a: data, b: data width)");
        public static readonly FuncDef eccdec = new FuncDef("eccdec", 3, (e, a) => new RealVal(LMath.EccDecode(a[0].AsInt, a[1].AsLong, a[2].AsInt)).FormatInt(), "Check ECC code (a: ECC code, b: data, c: data width)");

        public static readonly FuncDef togray = new FuncDef("togray", (e, a) => new RealVal(LMath.ToGray(a[0].AsLong), a[0].FormatHint), "Converts the value from binary to gray-code.");
        public static readonly FuncDef fromgray = new FuncDef("fromgray", (e, a) => new RealVal(LMath.FromGray(a[0].AsLong), a[0].FormatHint), "Converts the value from gray-code to binary.");

        public static readonly FuncDef now = new FuncDef("now", 0, (e, a) => new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime(), "Current epoch time");

        public static readonly FuncDef todays = new FuncDef("todays", (e, a) => a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal(), "Converts from epoch time to days.");
        public static readonly FuncDef tohours = new FuncDef("tohours", (e, a) => a[0].Div(e, new RealVal(60 * 60)).FormatReal(), "Converts from epoch time to hours.");
        public static readonly FuncDef tominutes = new FuncDef("tominutes", (e, a) => a[0].Div(e, new RealVal(60)).FormatReal(), "Converts from epoch time to minutes.");
        public static readonly FuncDef toseconds = new FuncDef("toseconds", (e, a) => a[0].FormatReal(), "Converts from epoch time to seconds.");

        public static readonly FuncDef fromdays = new FuncDef("fromdays", (e, a) => a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatDateTime(), "Converts from days to epoch time.");
        public static readonly FuncDef fromhours = new FuncDef("fromhours", (e, a) => a[0].Mul(e, new RealVal(60 * 60)).FormatDateTime(), "Converts from hours to epoch time.");
        public static readonly FuncDef fromminutes = new FuncDef("fromminutes", (e, a) => a[0].Mul(e, new RealVal(60)).FormatDateTime(), "Converts from minutes to epoch time.");
        public static readonly FuncDef fromseconds = new FuncDef("fromseconds", (e, a) => a[0].FormatDateTime(), "Converts from seconds to epoch time.");

        public static readonly FuncDef e3floor = new FuncDef("e3floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series floor");
        public static readonly FuncDef e3ceil = new FuncDef("e3ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series ceiling");
        public static readonly FuncDef e3round = new FuncDef("e3round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series round");
        public static readonly FuncDef e3ratio = new FuncDef("e3ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)), "E3 series value of divider resistor");

        public static readonly FuncDef e6floor = new FuncDef("e6floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series floor");
        public static readonly FuncDef e6ceil = new FuncDef("e6ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series ceiling");
        public static readonly FuncDef e6round = new FuncDef("e6round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series round");
        public static readonly FuncDef e6ratio = new FuncDef("e6ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)), "E6 series value of divider resistor");

        public static readonly FuncDef e12floor = new FuncDef("e12floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series floor");
        public static readonly FuncDef e12ceil = new FuncDef("e12ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series ceiling");
        public static readonly FuncDef e12round = new FuncDef("e12round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series round");
        public static readonly FuncDef e12ratio = new FuncDef("e12ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)), "E12 series value of divider resistor");

        public static readonly FuncDef e24floor = new FuncDef("e24floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series floor");
        public static readonly FuncDef e24ceil = new FuncDef("e24ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series ceiling");
        public static readonly FuncDef e24round = new FuncDef("e24round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series round");
        public static readonly FuncDef e24ratio = new FuncDef("e24ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)), "E24 series value of divider resistor");

        public static readonly FuncDef e48floor = new FuncDef("e48floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series floor");
        public static readonly FuncDef e48ceil = new FuncDef("e48ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series ceiling");
        public static readonly FuncDef e48round = new FuncDef("e48round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series round");
        public static readonly FuncDef e48ratio = new FuncDef("e48ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)), "E48 series value of divider resistor");

        public static readonly FuncDef e96floor = new FuncDef("e96floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series floor");
        public static readonly FuncDef e96ceil = new FuncDef("e96ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series ceiling");
        public static readonly FuncDef e96round = new FuncDef("e96round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series round");
        public static readonly FuncDef e96ratio = new FuncDef("e96ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)), "E96 series value of divider resistor");

        public static readonly FuncDef e192floor = new FuncDef("e192floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series floor");
        public static readonly FuncDef e192ceil = new FuncDef("e192ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series ceiling");
        public static readonly FuncDef e192round = new FuncDef("e192round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series round");
        public static readonly FuncDef e192ratio = new FuncDef("e192ratio", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)), "E192 series value of divider resistor");

        public static readonly FuncDef rgb_3 = new FuncDef("rgb", 3, (e, a) => new RealVal(ColorSpace.SatPack(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Generates 24 bit color value from R, G, B.");
        public static readonly FuncDef rgb_1 = new FuncDef("rgb", (e, a) => a[0].FormatWebColor(), "Converts the value to web-color representation.");

        public static readonly FuncDef hsv2rgb = new FuncDef("hsv2rgb", 3, (e, a) => new RealVal(ColorSpace.HsvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts from H, S, V to 24 bit RGB color value.");
        public static readonly FuncDef rgb2hsv = new FuncDef("rgb2hsv", (e, a) => new ArrayVal(ColorSpace.RgbToHsv(a[0].AsReal)), "Converts the 24 bit RGB color value to HSV.");

        public static readonly FuncDef hsl2rgb = new FuncDef("hsl2rgb", 3, (e, a) => new RealVal(ColorSpace.HslToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Convert from H, S, L to 24 bit color RGB value.");
        public static readonly FuncDef rgb2hsl = new FuncDef("rgb2hsl", (e, a) => new ArrayVal(ColorSpace.RgbToHsl(a[0].AsReal)), "Converts the 24 bit RGB color value to HSL.");

        public static readonly FuncDef yuv2rgb_3 = new FuncDef("yuv2rgb", 3, (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts Y, U, V to 24 bit RGB color.");
        public static readonly FuncDef yuv2rgb_1 = new FuncDef("yuv2rgb", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal)).FormatWebColor(), "Converts the 24 bit YUV color to 24 bit RGB.");

        public static readonly FuncDef rgb2yuv_3 = new FuncDef("rgb2yuv", 3, (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatHex(), "Converts R, G, B to 24 bit YUV color.");
        public static readonly FuncDef rgb2yuv_1 = new FuncDef("rgb2yuv", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal)).FormatHex(), "Converts 24bit RGB color to 24 bit YUV.");

        public static readonly FuncDef prime = new FuncDef("prime", (e, a) => new RealVal(RMath.Prime(a[0].AsInt)), "Returns a-th prime number.");
        public static readonly FuncDef isprime = new FuncDef("isprime", (e, a) => new BoolVal(RMath.IsPrime(a[0].AsReal)), "Returns whether the value is prime or not.");
        public static readonly FuncDef primefact = new FuncDef("primefact", (e, a) => new ArrayVal(RMath.PrimeFactors(a[0].AsReal), a[0].FormatHint), "Returns prime factors.");

        public static readonly FuncDef rand = new FuncDef("rand", 0, (e, a) => new RealVal((real)rng.NextDouble()), "Generates a random value between 0.0 and 1.0.");
        public static readonly FuncDef rand_2 = new FuncDef("rand", 2, (e, a) => {
            var min = a[0].AsReal;
            var max = a[1].AsReal;
            return new RealVal(min + (real)rng.NextDouble() * (max - min));
        }, "Generates a random value between min and max.");
        public static readonly FuncDef rand32 = new FuncDef("rand32", 0, (e, a) => new RealVal(rng.Next()), "Generates a 32bit random integer.");
        public static readonly FuncDef rand64 = new FuncDef("rand64", 0, (e, a) => new RealVal((((long)rng.Next()) << 32) | ((long)rng.Next())), "Generates a 64bit random integer.");

        public static readonly FuncDef assert = new FuncDef("assert", (e, a) => { if (!a[0].AsBool) { e.RequestHighlight(); } return a[0]; }, "Highlights the expression if the argument is false.");

        public static readonly FuncDef poll = new FuncDef("poll", (e, a) => { e.RequestRecalc(); return a[0]; }, "Requests recalculation after 1 second.");
        public static readonly FuncDef alarm = new FuncDef("alarm", (e, a) => {
            var t = RMath.Floor(a[0].AsReal - UnixTime.FromLocalTime(DateTime.Now));
            e.RequestRecalc();
            if (t <= 0) {
                e.RequestHighlight();
                e.RequestBeep();
            }
            return new RealVal(t).FormatInt();
        }, "Alarms at a specified time.");

        /// <summary>ネイティブ関数の一覧</summary>
        public static FuncDef[] NativeFunctions = EnumNativeFunctions().ToArray();
        private static IEnumerable<FuncDef> EnumNativeFunctions() {
            return
                typeof(FuncDef)
                .GetFields()
                .Where(p => p.IsStatic && (p.FieldType == typeof(FuncDef)))
                .Select(p => (FuncDef)p.GetValue(null));
        }

        public static IEnumerable<FuncDef> EnumAllFunctions() {
            if (Settings.Instance.Script_Enable) {
                return ExtFuncDef.ExternalFunctions.Concat(NativeFunctions);
            }
            else {
                return NativeFunctions;
            }
        }

        /// <summary>指定された条件にマッチするネイティブ関数を返す</summary>
        public static FuncDef Match(Token tok, int numArgs, bool allowExtermals) {
            var funcs = allowExtermals ? EnumAllFunctions() : NativeFunctions;
            var f = funcs.FirstOrDefault(p => p.Name == tok.Text && (p.ArgCount == numArgs || p.ArgCount == Variadic));
            if (f == null) {
                throw new Shapoco.Calctus.Parser.LexerError(tok.Position, "function " + tok + "(" + numArgs + ") was not found.");
            }
            return f;
        }

    }
}
