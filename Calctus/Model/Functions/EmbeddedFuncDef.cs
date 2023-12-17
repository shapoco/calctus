using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Graphs;

namespace Shapoco.Calctus.Model.Functions {
    class EmbeddedFuncDef : FuncDef {

        private static readonly Random rng = new Random((int)DateTime.Now.Ticks);

        public Func<EvalContext, Val[], Val> Method { get; protected set; }

        public EmbeddedFuncDef(string prototype, Func<EvalContext, Val[], Val> method, string desc) 
            : base(prototype , desc){
            this.Method = method;
        }

        protected override Val OnCall(EvalContext e, Val[] args) {
            return Method(e, args);
        }

        public static readonly EmbeddedFuncDef dec = new EmbeddedFuncDef("dec(x*)", (e, a) => a[0].FormatInt(), "Converts the value to decimal representation.");
        public static readonly EmbeddedFuncDef hex = new EmbeddedFuncDef("hex(x*)", (e, a) => a[0].FormatHex(), "Converts the value to hexdecimal representation.");
        public static readonly EmbeddedFuncDef bin = new EmbeddedFuncDef("bin(x*)", (e, a) => a[0].FormatBin(), "Converts the value to binary representation.");
        public static readonly EmbeddedFuncDef oct = new EmbeddedFuncDef("oct(x*)", (e, a) => a[0].FormatOct(), "Converts the value to octal representation.");
        public static readonly EmbeddedFuncDef si = new EmbeddedFuncDef("si(x*)", (e, a) => a[0].FormatSiPrefix(), "Converts the value to SI prefixed representation.");
        public static readonly EmbeddedFuncDef kibi = new EmbeddedFuncDef("kibi(x*)", (e, a) => a[0].FormatBinaryPrefix(), "Converts the value to binary prefixed representation.");
        public static readonly EmbeddedFuncDef char_1 = new EmbeddedFuncDef("char(x*)", (e, a) => a[0].FormatChar(), "Converts the value to character representation.");
        public static readonly EmbeddedFuncDef datetime = new EmbeddedFuncDef("datetime(x*)", (e, a) => a[0].FormatDateTime(), "Converts the value to datetime representation.");
        public static readonly EmbeddedFuncDef array = new EmbeddedFuncDef("array(x[]...)", (e, a) => a[0].FormatDefault(), "Converts the string value to array representation.");
        public static readonly EmbeddedFuncDef str = new EmbeddedFuncDef("str(x[]...)", (e, a) => a[0].FormatString(), "Converts the array value to string representation.");

        public static readonly FuncDef[] FormatterFunctions = new FuncDef[] { dec, hex, bin, oct, si, kibi, char_1, datetime, array, str };

        public static readonly EmbeddedFuncDef real = new EmbeddedFuncDef("real(x*)", (e, a) => a[0].AsRealVal().FormatReal(), "Converts the value to a real number.");
        public static readonly EmbeddedFuncDef rat = new EmbeddedFuncDef("rat(x*)", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal)), "Rational fraction approximation.");
        public static readonly EmbeddedFuncDef rat_2 = new EmbeddedFuncDef("rat(x*, max)", (e, a) => new FracVal(RMath.FindFrac(a[0].AsReal, a[1].AsReal, a[1].AsReal), a[0].FormatHint), "Rational fraction approximation.");

        public static readonly EmbeddedFuncDef pow = new EmbeddedFuncDef("pow(x*, y)", (e, a) => new RealVal(RMath.Pow(a[0].AsReal, a[1].AsReal), a[0].FormatHint), "Power");
        public static readonly EmbeddedFuncDef exp = new EmbeddedFuncDef("exp(x*)", (e, a) => new RealVal(RMath.Exp(a[0].AsReal)), "Exponential");
        public static readonly EmbeddedFuncDef sqrt = new EmbeddedFuncDef("sqrt(x*)", (e, a) => new RealVal(RMath.Sqrt(a[0].AsReal)), "Square root");
        public static readonly EmbeddedFuncDef log = new EmbeddedFuncDef("log(x*)", (e, a) => new RealVal(RMath.Log(a[0].AsReal)), "Logarithm");
        public static readonly EmbeddedFuncDef log2 = new EmbeddedFuncDef("log2(x*)", (e, a) => new RealVal(RMath.Log2(a[0].AsReal, e.EvalSettings.AccuracyPriority)), "Binary logarithm");
        public static readonly EmbeddedFuncDef log10 = new EmbeddedFuncDef("log10(x*)", (e, a) => new RealVal(RMath.Log10(a[0].AsReal)), "Common logarithm");
        public static readonly EmbeddedFuncDef clog2 = new EmbeddedFuncDef("clog2(x*)", (e, a) => new RealVal(RMath.Ceiling(RMath.Log2(a[0].AsReal, e.EvalSettings.AccuracyPriority))).FormatInt(), "Ceiling of binary logarithm");
        public static readonly EmbeddedFuncDef clog10 = new EmbeddedFuncDef("clog10(x*)", (e, a) => new RealVal(RMath.Ceiling(RMath.Log10(a[0].AsReal))).FormatInt(), "Ceiling of common logarithm");

        public static readonly EmbeddedFuncDef sin = new EmbeddedFuncDef("sin(x*)", (e, a) => new RealVal(RMath.Sin(a[0].AsReal)), "Sine");
        public static readonly EmbeddedFuncDef cos = new EmbeddedFuncDef("cos(x*)", (e, a) => new RealVal(RMath.Cos(a[0].AsReal)), "Cosine");
        public static readonly EmbeddedFuncDef tan = new EmbeddedFuncDef("tan(x*)", (e, a) => new RealVal(RMath.Tan(a[0].AsReal)), "Tangent");
        public static readonly EmbeddedFuncDef asin = new EmbeddedFuncDef("asin(x*)", (e, a) => new RealVal(RMath.Asin(a[0].AsReal)), "Arcsine");
        public static readonly EmbeddedFuncDef acos = new EmbeddedFuncDef("acos(x*)", (e, a) => new RealVal(RMath.Acos(a[0].AsReal)), "Arccosine");
        public static readonly EmbeddedFuncDef atan = new EmbeddedFuncDef("atan(x*)", (e, a) => new RealVal(RMath.Atan(a[0].AsReal)), "Arctangent");
        public static readonly EmbeddedFuncDef atan2 = new EmbeddedFuncDef("atan2(a, b)", (e, a) => new RealVal(RMath.Atan2(a[0].AsReal, a[1].AsReal)), "Arctangent of a / b");
        public static readonly EmbeddedFuncDef sinh = new EmbeddedFuncDef("sinh(x*)", (e, a) => new RealVal(RMath.Sinh(a[0].AsReal)), "Hyperbolic sine");
        public static readonly EmbeddedFuncDef cosh = new EmbeddedFuncDef("cosh(x*)", (e, a) => new RealVal(RMath.Cosh(a[0].AsReal)), "Hyperbolic cosine");
        public static readonly EmbeddedFuncDef tanh = new EmbeddedFuncDef("tanh(x*)", (e, a) => new RealVal(RMath.Tanh(a[0].AsReal)), "Hyperbolic tangent");

        public static readonly EmbeddedFuncDef floor = new EmbeddedFuncDef("floor(x*)", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint).FormatInt(), "Largest integral value less than or equal to a");
        public static readonly EmbeddedFuncDef ceil = new EmbeddedFuncDef("ceil(x*)", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint).FormatInt(), "Smallest integral value greater than or equal to a");
        public static readonly EmbeddedFuncDef trunc = new EmbeddedFuncDef("trunc(x*)", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint).FormatInt(), "Integral part of a");
        public static readonly EmbeddedFuncDef round = new EmbeddedFuncDef("round(x*)", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint).FormatInt(), "Nearest integer to a");

        public static readonly EmbeddedFuncDef abs = new EmbeddedFuncDef("abs(x*)", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint), "Absolute");
        public static readonly EmbeddedFuncDef sign = new EmbeddedFuncDef("sign(x*)", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt(), "Returns 1 for positives, -1 for negatives, 0 otherwise.");

        public static readonly EmbeddedFuncDef gcd = new EmbeddedFuncDef("gcd(x...)", (e, a) => new RealVal(RMath.Gcd(a.Select(p => (decimal)p.AsReal).ToArray()), a[0].FormatHint), "Greatest common divisor");
        public static readonly EmbeddedFuncDef lcm = new EmbeddedFuncDef("lcm(x...)", (e, a) => new RealVal(RMath.Lcm(a.Select(p => (decimal)p.AsReal).ToArray()), a[0].FormatHint), "Least common multiple");

        public static readonly EmbeddedFuncDef max = new EmbeddedFuncDef("max(x...)", (e, a) => new RealVal(a.Max(p => p.AsReal), a[0].FormatHint), "Maximum value of the arguments");
        public static readonly EmbeddedFuncDef min = new EmbeddedFuncDef("min(x...)", (e, a) => new RealVal(a.Min(p => p.AsReal), a[0].FormatHint), "Minimum value of the arguments");

        public static readonly EmbeddedFuncDef sum = new EmbeddedFuncDef("sum(x...)", (e, a) => new RealVal(a.Sum(p => p.AsReal), a[0].FormatHint), "Sum of the arguments");
        public static readonly EmbeddedFuncDef ave = new EmbeddedFuncDef("ave(x...)", (e, a) => new RealVal(a.Average(p => p.AsReal), a[0].FormatHint), "Arithmetic mean of the arguments");
        public static readonly EmbeddedFuncDef invSum = new EmbeddedFuncDef("invSum(x...)", (e, a) => new RealVal(1m / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Inverse of the sum of the inverses");
        public static readonly EmbeddedFuncDef harMean = new EmbeddedFuncDef("harMean(x...)", (e, a) => new RealVal((real)a.Length / a.Sum(p => 1m / p.AsReal), a[0].FormatHint), "Harmonic mean of the arguments");
        public static readonly EmbeddedFuncDef geoMean = new EmbeddedFuncDef("geoMean(x...)", (e, a) => {
            var prod = (real)1;
            foreach (var p in a) prod *= p.AsReal;
            return new RealVal(RMath.Pow(prod, 1m / a.Length), a[0].FormatHint);
        }, "Geometric mean of the arguments");

        public static readonly EmbeddedFuncDef pack = new EmbeddedFuncDef("pack(b, array[]...)", (e, a) => new RealVal(LMath.Pack(a[0].AsInt, a[1].AsLongArray)).FormatHex(), "Packs the array elements to a value.");
        public static readonly EmbeddedFuncDef unpack = new EmbeddedFuncDef("unpack(b, x)", (e, a) => new ArrayVal(LMath.Unpack(a[0].AsInt, a[1].AsLong)).FormatInt(), "Unpacks the value to an array.");

        public static readonly EmbeddedFuncDef swapNib = new EmbeddedFuncDef("swapNib(x*)", (e, a) => new RealVal(LMath.SwapNibbles(a[0].AsLong), a[0].FormatHint), "Swaps the nibble of each byte.");
        public static readonly EmbeddedFuncDef swap2 = new EmbeddedFuncDef("swap2(x*)", (e, a) => new RealVal(LMath.Swap2(a[0].AsLong), a[0].FormatHint), "Swaps even and odd bytes.");
        public static readonly EmbeddedFuncDef swap4 = new EmbeddedFuncDef("swap4(x*)", (e, a) => new RealVal(LMath.Swap4(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 4 bytes.");
        public static readonly EmbeddedFuncDef swap8 = new EmbeddedFuncDef("swap8(x*)", (e, a) => new RealVal(LMath.Swap8(a[0].AsLong), a[0].FormatHint), "Reverses the order of each 8 bytes.");
        public static readonly EmbeddedFuncDef reverseBits = new EmbeddedFuncDef("reverseBits(b, x*)", (e, a) => new RealVal(LMath.Reverse(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Reverses the lower b bits of x.");
        public static readonly EmbeddedFuncDef reverseBytewise = new EmbeddedFuncDef("reverseBytewise(b, x*)", (e, a) => new RealVal(LMath.ReverseBytes(a[0].AsLong), a[0].FormatHint), "Reverses the order of bits of each byte.");
        public static readonly EmbeddedFuncDef rotateL = new EmbeddedFuncDef("rotateL(b, x*)", (e, a) => new RealVal(LMath.RotateLeft(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Rotates left the lower b bits of a.");
        public static readonly EmbeddedFuncDef rotateR = new EmbeddedFuncDef("rotateR(b, x*)", (e, a) => new RealVal(LMath.RotateRight(a[0].AsInt, a[1].AsLong), a[1].FormatHint), "Rotates right the lower b bits of a.");
        public static readonly EmbeddedFuncDef count1 = new EmbeddedFuncDef("count1(x*)", (e, a) => new RealVal(LMath.CountOnes(a[0].AsLong)).FormatInt(), "Number of bits that have the value 1.");

        public static readonly EmbeddedFuncDef xorReduce = new EmbeddedFuncDef("xorReduce(x*)", (e, a) => new RealVal(LMath.XorReduce(a[0].AsLong)).FormatInt(), "Reduction XOR (Same as even parity).");
        public static readonly EmbeddedFuncDef oddParity = new EmbeddedFuncDef("oddParity(x*)", (e, a) => new RealVal(LMath.OddParity(a[0].AsLong)).FormatInt(), "Odd parity.");

        public static readonly EmbeddedFuncDef eccWidth = new EmbeddedFuncDef("eccWidth(b*)", (e, a) => new RealVal(LMath.EccWidth(a[0].AsInt)).FormatInt(), "Width of ECC for b-bit data.");
        public static readonly EmbeddedFuncDef eccEnc = new EmbeddedFuncDef("eccEnc(b, x*)", (e, a) => new RealVal(LMath.EccEncode(a[0].AsInt, a[1].AsLong)).FormatHex(), "Generate ECC code (b: data width, x: data)");
        public static readonly EmbeddedFuncDef eccDec = new EmbeddedFuncDef("eccDec(b, ecc, x)", (e, a) => new RealVal(LMath.EccDecode(a[0].AsInt, a[1].AsInt, a[2].AsLong)).FormatInt(), "Check ECC code (b: data width, ecc: ECC code, x: data)");

        public static readonly EmbeddedFuncDef toGray = new EmbeddedFuncDef("toGray(x*)", (e, a) => new RealVal(LMath.ToGray(a[0].AsLong), a[0].FormatHint), "Converts the value from binary to gray-code.");
        public static readonly EmbeddedFuncDef fromGray = new EmbeddedFuncDef("fromGray(x*)", (e, a) => new RealVal(LMath.FromGray(a[0].AsLong), a[0].FormatHint), "Converts the value from gray-code to binary.");

        public static readonly EmbeddedFuncDef now = new EmbeddedFuncDef("now()", (e, a) => new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime(), "Current epoch time");

        public static readonly EmbeddedFuncDef toDays = new EmbeddedFuncDef("toDays(x*)", (e, a) => a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal(), "Converts from epoch time to days.");
        public static readonly EmbeddedFuncDef toHours = new EmbeddedFuncDef("toHours(x*)", (e, a) => a[0].Div(e, new RealVal(60 * 60)).FormatReal(), "Converts from epoch time to hours.");
        public static readonly EmbeddedFuncDef toMinutes = new EmbeddedFuncDef("toMinutes(x*)", (e, a) => a[0].Div(e, new RealVal(60)).FormatReal(), "Converts from epoch time to minutes.");
        public static readonly EmbeddedFuncDef toSeconds = new EmbeddedFuncDef("toSeconds(x*)", (e, a) => a[0].FormatReal(), "Converts from epoch time to seconds.");

        public static readonly EmbeddedFuncDef fromDays = new EmbeddedFuncDef("fromDays(x*)", (e, a) => a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatDateTime(), "Converts from days to epoch time.");
        public static readonly EmbeddedFuncDef fromHours = new EmbeddedFuncDef("fromHours(x*)", (e, a) => a[0].Mul(e, new RealVal(60 * 60)).FormatDateTime(), "Converts from hours to epoch time.");
        public static readonly EmbeddedFuncDef fromMinutes = new EmbeddedFuncDef("fromMinutes(x*)", (e, a) => a[0].Mul(e, new RealVal(60)).FormatDateTime(), "Converts from minutes to epoch time.");
        public static readonly EmbeddedFuncDef fromSeconds = new EmbeddedFuncDef("fromSeconds(x*)", (e, a) => a[0].FormatDateTime(), "Converts from seconds to epoch time.");

        public static readonly EmbeddedFuncDef e3Floor = new EmbeddedFuncDef("e3Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series floor");
        public static readonly EmbeddedFuncDef e3Ceil = new EmbeddedFuncDef("e3Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series ceiling");
        public static readonly EmbeddedFuncDef e3Round = new EmbeddedFuncDef("e3Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series round");
        public static readonly EmbeddedFuncDef e3Ratio = new EmbeddedFuncDef("e3Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)), "E3 series value of divider resistor");

        public static readonly EmbeddedFuncDef e6Floor = new EmbeddedFuncDef("e6Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series floor");
        public static readonly EmbeddedFuncDef e6Ceil = new EmbeddedFuncDef("e6Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series ceiling");
        public static readonly EmbeddedFuncDef e6Round = new EmbeddedFuncDef("e6Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series round");
        public static readonly EmbeddedFuncDef e6Ratio = new EmbeddedFuncDef("e6Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)), "E6 series value of divider resistor");

        public static readonly EmbeddedFuncDef e12Floor = new EmbeddedFuncDef("e12Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series floor");
        public static readonly EmbeddedFuncDef e12Ceil = new EmbeddedFuncDef("e12Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series ceiling");
        public static readonly EmbeddedFuncDef e12Round = new EmbeddedFuncDef("e12Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series round");
        public static readonly EmbeddedFuncDef e12Ratio = new EmbeddedFuncDef("e12Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)), "E12 series value of divider resistor");

        public static readonly EmbeddedFuncDef e24Floor = new EmbeddedFuncDef("e24Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series floor");
        public static readonly EmbeddedFuncDef e24Ceil = new EmbeddedFuncDef("e24Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series ceiling");
        public static readonly EmbeddedFuncDef e24Round = new EmbeddedFuncDef("e24Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series round");
        public static readonly EmbeddedFuncDef e24Ratio = new EmbeddedFuncDef("e24Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)), "E24 series value of divider resistor");

        public static readonly EmbeddedFuncDef e48Floor = new EmbeddedFuncDef("e48Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series floor");
        public static readonly EmbeddedFuncDef e48Ceil = new EmbeddedFuncDef("e48Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series ceiling");
        public static readonly EmbeddedFuncDef e48Round = new EmbeddedFuncDef("e48Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series round");
        public static readonly EmbeddedFuncDef e48Ratio = new EmbeddedFuncDef("e48Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)), "E48 series value of divider resistor");

        public static readonly EmbeddedFuncDef e96Floor = new EmbeddedFuncDef("e96Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series floor");
        public static readonly EmbeddedFuncDef e96Ceil = new EmbeddedFuncDef("e96Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series ceiling");
        public static readonly EmbeddedFuncDef e96Round = new EmbeddedFuncDef("e96Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series round");
        public static readonly EmbeddedFuncDef e96Ratio = new EmbeddedFuncDef("e96Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)), "E96 series value of divider resistor");

        public static readonly EmbeddedFuncDef e192Floor = new EmbeddedFuncDef("e192Floor(x*)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series floor");
        public static readonly EmbeddedFuncDef e192Ceil = new EmbeddedFuncDef("e192Ceil(x*)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series ceiling");
        public static readonly EmbeddedFuncDef e192Round = new EmbeddedFuncDef("e192Round(x*)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series round");
        public static readonly EmbeddedFuncDef e192Ratio = new EmbeddedFuncDef("e192Ratio(x*)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)), "E192 series value of divider resistor");

        public static readonly EmbeddedFuncDef rgb_3 = new EmbeddedFuncDef("rgb(r, g, b)", (e, a) => new RealVal(ColorSpace.SatPack(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Generates 24 bit color value from R, G, B.");
        public static readonly EmbeddedFuncDef rgb_1 = new EmbeddedFuncDef("rgb(rgb*)", (e, a) => a[0].FormatWebColor(), "Converts the value to web-color representation.");

        public static readonly EmbeddedFuncDef hsv2rgb = new EmbeddedFuncDef("hsv2rgb(h, s, v)", (e, a) => new RealVal(ColorSpace.HsvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts from H, S, V to 24 bit RGB color value.");
        public static readonly EmbeddedFuncDef rgb2hsv = new EmbeddedFuncDef("rgb2hsv(rgb*)", (e, a) => new ArrayVal(ColorSpace.RgbToHsv(a[0].AsReal)), "Converts the 24 bit RGB color value to HSV.");

        public static readonly EmbeddedFuncDef hsl2rgb = new EmbeddedFuncDef("hsl2rgb(h, s, l)", (e, a) => new RealVal(ColorSpace.HslToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Convert from H, S, L to 24 bit color RGB value.");
        public static readonly EmbeddedFuncDef rgb2hsl = new EmbeddedFuncDef("rgb2hsl(rgb*)", (e, a) => new ArrayVal(ColorSpace.RgbToHsl(a[0].AsReal)), "Converts the 24 bit RGB color value to HSL.");

        public static readonly EmbeddedFuncDef yuv2rgb_3 = new EmbeddedFuncDef("yuv2rgb(y, u, v)", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatWebColor(), "Converts Y, U, V to 24 bit RGB color.");
        public static readonly EmbeddedFuncDef yuv2rgb_1 = new EmbeddedFuncDef("yuv2rgb(yuv*)", (e, a) => new RealVal(ColorSpace.YuvToRgb(a[0].AsReal)).FormatWebColor(), "Converts the 24 bit YUV color to 24 bit RGB.");

        public static readonly EmbeddedFuncDef rgb2yuv_3 = new EmbeddedFuncDef("rgb2yuv(r, g, b)", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal, a[1].AsReal, a[2].AsReal)).FormatHex(), "Converts R, G, B to 24 bit YUV color.");
        public static readonly EmbeddedFuncDef rgb2yuv_1 = new EmbeddedFuncDef("rgb2yuv(rgb*)", (e, a) => new RealVal(ColorSpace.RgbToYuv(a[0].AsReal)).FormatHex(), "Converts 24bit RGB color to 24 bit YUV.");

        public static readonly EmbeddedFuncDef rgbto565 = new EmbeddedFuncDef("rgbto565(rgb*)", (e, a) => new RealVal(ColorSpace.Rgb888To565(a[0].AsInt)).FormatHex(), "Downconverts RGB888 color to RGB565.");
        public static readonly EmbeddedFuncDef rgbfrom565 = new EmbeddedFuncDef("rgbfrom565(rgb*)", (e, a) => new RealVal(ColorSpace.Rgb565To888(a[0].AsInt)).FormatWebColor(), "Upconverts RGB565 color to RGB888.");

        public static readonly EmbeddedFuncDef pack565 = new EmbeddedFuncDef("pack565(x, y, z)", (e, a) => new RealVal(ColorSpace.Pack565(a[0].AsInt, a[1].AsInt, a[2].AsInt)).FormatHex(), "Packs the 3 values to an RGB565 color.");
        public static readonly EmbeddedFuncDef unpack565 = new EmbeddedFuncDef("unpack565(x*)", (e, a) => new ArrayVal(ColorSpace.Unpack565(a[0].AsInt)), "Unpacks the RGB565 color to 3 values.");

        public static readonly EmbeddedFuncDef prime = new EmbeddedFuncDef("prime(x*)", (e, a) => new RealVal(RMath.Prime(a[0].AsInt)), "Returns x-th prime number.");
        public static readonly EmbeddedFuncDef isPrime = new EmbeddedFuncDef("isPrime(x*)", (e, a) => new BoolVal(RMath.IsPrime(a[0].AsReal)), "Returns whether the value is prime or not.");
        public static readonly EmbeddedFuncDef primeFact = new EmbeddedFuncDef("primeFact(x*)", (e, a) => new ArrayVal(RMath.PrimeFactors(a[0].AsReal), a[0].FormatHint), "Returns prime factors.");

        public static readonly EmbeddedFuncDef rand = new EmbeddedFuncDef("rand()", (e, a) => new RealVal((real)rng.NextDouble()), "Generates a random value between 0.0 and 1.0.");
        public static readonly EmbeddedFuncDef rand_2 = new EmbeddedFuncDef("rand(min, max)", (e, a) => {
            var min = a[0].AsReal;
            var max = a[1].AsReal;
            return new RealVal(min + (real)rng.NextDouble() * (max - min));
        }, "Generates a random value between min and max.");
        public static readonly EmbeddedFuncDef rand32 = new EmbeddedFuncDef("rand32()", (e, a) => new RealVal(rng.Next()), "Generates a 32bit random integer.");
        public static readonly EmbeddedFuncDef rand64 = new EmbeddedFuncDef("rand64()", (e, a) => new RealVal((((long)rng.Next()) << 32) | ((long)rng.Next())), "Generates a 64bit random integer.");

        public static readonly EmbeddedFuncDef len = new EmbeddedFuncDef("len(array)", (e, a) => new RealVal(((ArrayVal)a[0]).Length), "Length of array");
        public static readonly EmbeddedFuncDef range_2 = new EmbeddedFuncDef("range(start, stop)", (e, a) => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, 0, false)), "Generate number sequence.");
        public static readonly EmbeddedFuncDef range_3 = new EmbeddedFuncDef("range(start, stop, step)", (e, a) => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, a[2].AsReal, false)), "Generate number sequence.");
        public static readonly EmbeddedFuncDef rangeInclusive_2 = new EmbeddedFuncDef("rangeInclusive(start, stop)", (e, a) => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, 0, true)), "Generate number sequence.");
        public static readonly EmbeddedFuncDef rangeInclusive_3 = new EmbeddedFuncDef("rangeInclusive(start, stop, step)", (e, a) => new ArrayVal(RMath.Range(a[0].AsReal, a[1].AsReal, a[2].AsReal, true)), "Generate number sequence.");
        public static readonly EmbeddedFuncDef reverseArray = new EmbeddedFuncDef("reverseArray(array)", (e, a) => {
            var array = (Val[])((ArrayVal)a[0]).Raw;
            Array.Reverse(array);
            return new ArrayVal(array, a[0].FormatHint);
        }, "Reverses the order of array elements");

        public static readonly EmbeddedFuncDef utf8Enc = new EmbeddedFuncDef("utf8Enc(str)", (e, a) => new ArrayVal(Encoding.UTF8.GetBytes(a[0].AsString)), "Encode string to UTF8 byte sequence.");
        public static readonly EmbeddedFuncDef utf8Dec = new EmbeddedFuncDef("utf8Dec(bytes[]...)", (e, a) => new ArrayVal(Encoding.UTF8.GetString(a[0].AsByteArray)), "Decode UTF8 byte sequence.");

        public static readonly EmbeddedFuncDef urlEnc = new EmbeddedFuncDef("urlEnc(str)", (e, a) => new ArrayVal(System.Web.HttpUtility.UrlEncode(a[0].AsString)), "Escape URL string.");
        public static readonly EmbeddedFuncDef urlDec = new EmbeddedFuncDef("urlDec(str)", (e, a) => new ArrayVal(System.Web.HttpUtility.UrlDecode(a[0].AsString)), "Decode URL string.");

        public static readonly EmbeddedFuncDef base64Enc = new EmbeddedFuncDef("base64Enc(str)", (e, a) => new ArrayVal(Convert.ToBase64String(Encoding.UTF8.GetBytes(a[0].AsString))), "Encode string to Base64.");
        public static readonly EmbeddedFuncDef base64Dec = new EmbeddedFuncDef("base64Dec(str)", (e, a) => new ArrayVal(Encoding.UTF8.GetString(Convert.FromBase64String(a[0].AsString))), "Decode Base64 to string.");
        public static readonly EmbeddedFuncDef base64EncBytes = new EmbeddedFuncDef("base64EncBytes(bytes[]...)", (e, a) => new ArrayVal(Convert.ToBase64String(a[0].AsByteArray)), "Encode byte-array to Base64.");
        public static readonly EmbeddedFuncDef base64DecBytes = new EmbeddedFuncDef("base64DecBytes(str)", (e, a) => new ArrayVal(Convert.FromBase64String(a[0].AsString)), "Decode Base64 to byte-array.");

        public static readonly EmbeddedFuncDef map = new EmbeddedFuncDef("map(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Select(p => func.Call(e, p)).ToArray());
        }, "Map an array using a converter function.");

        public static readonly EmbeddedFuncDef filter = new EmbeddedFuncDef("filter(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Where(p => func.Call(e, p).AsBool).ToArray(), a[0].FormatHint);
        }, "Filter an array using a tester function.");

        public static readonly EmbeddedFuncDef count = new EmbeddedFuncDef("count(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new RealVal(array.Count(p => func.Call(e, p).AsBool));
        }, "Count specific elements in array using a tester function.");

        public static readonly EmbeddedFuncDef sort_1 = new EmbeddedFuncDef("sort(array)", (e, a) => {
            var array = (Val[])a[0].Raw;
            return new ArrayVal(array.OrderBy(p => p, new ValComparer(e)).ToArray(), a[0].FormatHint);
        }, "Sort the array.");

        public static readonly EmbeddedFuncDef sort_2 = new EmbeddedFuncDef("sort(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.OrderBy(p => func.Call(e, p).AsReal).ToArray(), a[0].FormatHint);
        }, "Sort the array using a converter function.");

        public static readonly EmbeddedFuncDef extend = new EmbeddedFuncDef("extend(array,func,count)", (e, a) => {
            var seedArray = (ArrayVal)a[0];
            var list =((Val[])seedArray.Raw).ToList();
            var func = (FuncDef)a[1].Raw;
            var countReal = a[2].AsReal;
            if (!RMath.IsInteger(countReal) || countReal <= 0) throw new ArgumentOutOfRangeException();
            int count = (int)countReal;
            ArrayVal.CheckArrayLength(list.Count + count);
            for (int i = 0; i < count; i++) {
                list.Add(func.Call(e, new ArrayVal(list.ToArray())));
            }
            return new ArrayVal(list.ToArray(), seedArray.FormatHint);
        }, "Extends the array using converter function.");

        public static readonly EmbeddedFuncDef aggregate = new EmbeddedFuncDef("aggregate(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return array.Aggregate((p, q) => func.Call(e, p, q));
        }, "Apply the aggregate function for the array.");

        public static readonly EmbeddedFuncDef all_1 = new EmbeddedFuncDef("all(array)", (e, a) => {
            var array = (Val[])a[0].Raw;
            return new BoolVal(array.All(p => p.AsBool));
        }, "Returns true if all array elements are true.");

        public static readonly EmbeddedFuncDef all_2 = new EmbeddedFuncDef("all(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new BoolVal(array.All(p => func.Call(e, p).AsBool));
        }, "Returns true if func returns true for all elements of the array.");

        public static readonly EmbeddedFuncDef any_1 = new EmbeddedFuncDef("any(array)", (e, a) => {
            var array = (Val[])a[0].Raw;
            return new BoolVal(array.Any(p => p.AsBool));
        }, "Returns true if at least one element is true.");

        public static readonly EmbeddedFuncDef any_2 = new EmbeddedFuncDef("any(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new BoolVal(array.Any(p => func.Call(e, p).AsBool));
        }, "Return true if func returns true for at least one element of the array.");

        public static readonly EmbeddedFuncDef unique_1 = new EmbeddedFuncDef("unique(array)", (e, a) => {
            var array = (Val[])a[0].Raw;
            return new ArrayVal(array.Distinct(new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Return unique elements.");

        public static readonly EmbeddedFuncDef unique_2 = new EmbeddedFuncDef("unique(array,func)", (e, a) => {
            var array = (Val[])a[0].Raw;
            var func = (FuncDef)a[1].Raw;
            return new ArrayVal(array.Distinct(new EqualityComparerFunc(e, func)).ToArray(), a[0].FormatHint);
        }, "Return unique elements using comparer function.");

        public static readonly EmbeddedFuncDef except = new EmbeddedFuncDef("except(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].Raw;
            var array1 = (Val[])a[1].Raw;
            return new ArrayVal(array0.Except(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the difference set of two arrays.");

        public static readonly EmbeddedFuncDef intersect = new EmbeddedFuncDef("intersect(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].Raw;
            var array1 = (Val[])a[1].Raw;
            return new ArrayVal(array0.Intersect(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the product set of two arrays.");

        public static readonly EmbeddedFuncDef union = new EmbeddedFuncDef("union(array0,array1)", (e, a) => {
            var array0 = (Val[])a[0].Raw;
            var array1 = (Val[])a[1].Raw;
            return new ArrayVal(array0.Union(array1, new ValEqualityComparer(e)).ToArray(), a[0].FormatHint);
        }, "Returns the union of two arrays.");

        public static readonly EmbeddedFuncDef indexOf = new EmbeddedFuncDef("indexOf(array,val*)", (e, a) => {
            var array = (Val[])a[0].Raw;
            if (a[1] is FuncVal fVal) {
                var func = (FuncDef)fVal.Raw;
                for (int i = 0; i < array.Length; i++) {
                    if (func.Call(e, array[i]).AsBool) return new RealVal(i);
                }
            }
            else {
                for (int i = 0; i < array.Length; i++) {
                    if (array[i].Equals(e, a[1]).AsBool) return new RealVal(i);
                }
            }
            return new RealVal(-1);
        }, "Returns the index of the first element whose value matches val.");

        public static readonly EmbeddedFuncDef assert = new EmbeddedFuncDef("assert(x)", (e, a) => {
            if (!a[0].AsBool) {
                throw new CalctusError("Assertion failed.");
            }
            return a[0];
        }, "Highlights the expression if the argument is false.");

        public static readonly EmbeddedFuncDef solve_1 = new EmbeddedFuncDef("solve(func)", (e, a) 
            => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw), "Solve function using Newton's Method.");
        public static readonly EmbeddedFuncDef solve_2 = new EmbeddedFuncDef("solve(func,init)", (e, a) 
            => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1]), "Solve function using Newton's Method with initial value.");
        public static readonly EmbeddedFuncDef solve_3 = new EmbeddedFuncDef("solve(func,min,max)", (e, a) 
            => NewtonsMethod.Solve(e, (FuncDef)a[0].Raw, a[1], a[2]), "Solve function using Newton's Method with range of initial value.");

        public static readonly EmbeddedFuncDef plot = new EmbeddedFuncDef("plot(func)", (e, a) => {
            var func = (FuncDef)a[0].Raw;
            var req = new PlotCall(e, PlotCall.DefaultWindowName, func);
            e.PlotCalls.Add(req);
            return NullVal.Instance;
        }, "Plot graph.");

        /// <summary>ネイティブ関数の一覧</summary>
        public static EmbeddedFuncDef[] NativeFunctions = enumEmbeddedFunctions().ToArray();
        private static IEnumerable<EmbeddedFuncDef> enumEmbeddedFunctions() {
            return
                typeof(EmbeddedFuncDef)
                .GetFields()
                .Where(p => p.IsStatic && (p.FieldType == typeof(EmbeddedFuncDef)))
                .Select(p => (EmbeddedFuncDef)p.GetValue(null));
        }

        //public static IEnumerable<FuncDef> EnumAllFunctions() {
        //    if (Settings.Instance.Script_Enable) {
        //        return ExtFuncDef.ExternalFunctions.Select(p => (FuncDef)p).Concat(NativeFunctions);
        //    }
        //    else {
        //        return NativeFunctions;
        //    }
        //}
        //
        ///// <summary>指定された条件にマッチするネイティブ関数を返す</summary>
        //public static FuncDef Match(Token tok, Val[] args, bool allowExtermals) {
        //    var funcs = allowExtermals ? EnumAllFunctions() : NativeFunctions;
        //    var f = funcs.FirstOrDefault(p => p.Match(tok.Text, args));
        //    if (f == null) {
        //        throw new LexerError(tok.Position, "function " + tok + "(" + args.Length + ") was not found.");
        //    }
        //    return f;
        //}

    }
}
