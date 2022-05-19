using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.UnitSystem;

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
            double af = a[0].AsDouble;
            double bf = a[1].AsDouble;
            double powf = Math.Pow(af, bf);
            if (!a[0].IsDimless) {
                if (a[1].IsInteger) {
                    var newUnit = a[0].Unit.Pow(e, (int)bf);
                    return new RealVal(powf, a[0].FormatHint, newUnit);
                }
                else {
                    e.Warning(a[1], "べき乗の根に単位が指定されていますが、指数が整数ではありません");
                }
            }
            return new RealVal(powf, a[0].FormatHint);
        });
        public static readonly FuncDef sqrt = new FuncDef("sqrt", (e, a) => new RealVal(Math.Sqrt(a[0].AsDouble)));
        public static readonly FuncDef log = new FuncDef("log", (e, a) => new RealVal(Math.Log(a[0].AsDouble)));
        public static readonly FuncDef log2 = new FuncDef("log2", (e, a) => new RealVal(Math.Log(a[0].AsDouble) / Math.Log(2.0)));
        public static readonly FuncDef log10 = new FuncDef("log10", (e, a) => new RealVal(Math.Log(a[0].AsDouble) / Math.Log(10.0)));
        public static readonly FuncDef clog2 = new FuncDef("clog2", (e, a) => new RealVal(Math.Ceiling(Math.Log(a[0].AsDouble) / Math.Log(2.0))).FormatInt());
        public static readonly FuncDef clog10 = new FuncDef("clog10", (e, a) => new RealVal(Math.Ceiling(Math.Log(a[0].AsDouble) / Math.Log(10.0))).FormatInt());

        public static readonly FuncDef sin = new FuncDef("sin", (e, a) => new RealVal(Math.Sin(a[0].AsDouble)));
        public static readonly FuncDef cos = new FuncDef("cos", (e, a) => new RealVal(Math.Cos(a[0].AsDouble)));
        public static readonly FuncDef tan = new FuncDef("tan", (e, a) => new RealVal(Math.Tan(a[0].AsDouble)));
        public static readonly FuncDef asin = new FuncDef("asin", (e, a) => new RealVal(Math.Asin(a[0].AsDouble)));
        public static readonly FuncDef acos = new FuncDef("acos", (e, a) => new RealVal(Math.Acos(a[0].AsDouble)));
        public static readonly FuncDef atan = new FuncDef("atan", (e, a) => new RealVal(Math.Atan(a[0].AsDouble)));
        public static readonly FuncDef atan2 = new FuncDef("atan2", 2, (e, a) => new RealVal(Math.Atan2(a[0].AsDouble, a[1].AsDouble)));

        public static readonly FuncDef floor = new FuncDef("floor", (e, a) => new RealVal(Math.Floor(a[0].AsDouble), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef ceil = new FuncDef("ceil", (e, a) => new RealVal(Math.Ceiling(a[0].AsDouble), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef trunc = new FuncDef("trunc", (e, a) => new RealVal(Math.Truncate(a[0].AsDouble), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef round = new FuncDef("round", (e, a) => new RealVal(Math.Round(a[0].AsDouble), a[0].FormatHint, a[0].Unit).FormatInt());
        public static readonly FuncDef sign = new FuncDef("sign", (e, a) => new RealVal(Math.Sign(a[0].AsDouble)).FormatInt());

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
