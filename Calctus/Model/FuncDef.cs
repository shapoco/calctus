using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.UnitSystem;
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

        public static readonly FuncDef pow = new FuncDef("pow", 2, (e, a) => {
            var pow = RMath.Pow(a[0].AsReal, a[1].AsReal);
            if (!a[0].IsDimless) {
                if (a[1].IsInteger) {
                    var newUnit = a[0].Unit.Pow(e, (int)a[1].AsReal);
                    return new RealVal(pow, a[0].FormatHint, newUnit);
                }
                else {
                    e.Warning(a[1], "べき乗の根に単位が指定されていますが、指数が整数ではありません");
                }
            }
            return new RealVal(pow, a[0].FormatHint);
        });
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

        public static readonly FuncDef floor = new FuncDef("floor", (e, a) => new RealVal(RMath.Floor(a[0].AsReal), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef ceil = new FuncDef("ceil", (e, a) => new RealVal(RMath.Ceiling(a[0].AsReal), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef trunc = new FuncDef("trunc", (e, a) => new RealVal(RMath.Truncate(a[0].AsReal), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef round = new FuncDef("round", (e, a) => new RealVal(RMath.Round(a[0].AsReal), a[0].FormatHint, a[0].Unit).FormatInt());
        
        public static readonly FuncDef abs = new FuncDef("abs", (e, a) => new RealVal(RMath.Abs(a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef sign = new FuncDef("sign", (e, a) => new RealVal(RMath.Sign(a[0].AsReal)).FormatInt());

        public static readonly FuncDef max = new FuncDef("max", 2, (e, a) => new RealVal(RMath.Max(a[0].AsReal, a[1].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef min = new FuncDef("min", 2, (e, a) => new RealVal(RMath.Min(a[0].AsReal, a[1].AsReal), a[0].FormatHint, a[0].Unit));

        public static readonly FuncDef e3floor = new FuncDef("e3floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e3ceil = new FuncDef("e3ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e3round = new FuncDef("e3round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e3ratiol = new FuncDef("e3ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[0]));
        public static readonly FuncDef e3ratioh = new FuncDef("e3ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)[1]));

        public static readonly FuncDef e6floor = new FuncDef("e6floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e6ceil = new FuncDef("e6ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e6round = new FuncDef("e6round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e6ratiol = new FuncDef("e6ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[0]));
        public static readonly FuncDef e6ratioh = new FuncDef("e6ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)[1]));

        public static readonly FuncDef e12floor = new FuncDef("e12floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e12ceil = new FuncDef("e12ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e12round = new FuncDef("e12round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e12ratiol = new FuncDef("e12ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[0]));
        public static readonly FuncDef e12ratioh = new FuncDef("e12ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)[1]));

        public static readonly FuncDef e24floor = new FuncDef("e24floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e24ceil = new FuncDef("e24ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e24round = new FuncDef("e24round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e24ratiol = new FuncDef("e24ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[0]));
        public static readonly FuncDef e24ratioh = new FuncDef("e24ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)[1]));

        public static readonly FuncDef e48floor = new FuncDef("e48floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e48ceil = new FuncDef("e48ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e48round = new FuncDef("e48round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e48ratiol = new FuncDef("e48ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[0]));
        public static readonly FuncDef e48ratioh = new FuncDef("e48ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)[1]));

        public static readonly FuncDef e96floor = new FuncDef("e96floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e96ceil = new FuncDef("e96ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e96round = new FuncDef("e96round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e96ratiol = new FuncDef("e96ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[0]));
        public static readonly FuncDef e96ratioh = new FuncDef("e96ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)[1]));

        public static readonly FuncDef e192floor = new FuncDef("e192floor", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e192ceil = new FuncDef("e192ceil", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e192round = new FuncDef("e192round", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint, a[0].Unit));
        public static readonly FuncDef e192ratiol = new FuncDef("e192ratiol", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[0]));
        public static readonly FuncDef e192ratioh = new FuncDef("e192ratioh", (e, a) => new RealVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)[1]));

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
