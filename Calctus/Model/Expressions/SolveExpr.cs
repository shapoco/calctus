using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;

namespace Shapoco.Calctus.Model.Expressions {
    class SolveExpr : Expr {
        public readonly Expr Equation;
        public readonly Token Variant;
        public readonly Expr ParamMin;
        public readonly Expr ParamMax;

        public SolveExpr(Token keyword, Expr equation, Token variant, Expr paramMin, Expr paramMax) : base(keyword) {
            // 等式が与えられた場合は減算にすり替える
            if (equation is BinaryOp binOp && (binOp.Method == OpDef.Assign || binOp.Method == OpDef.Equal)) {
                equation = new BinaryOp(new Token(TokenType.OperatorSymbol, TextPosition.Nowhere, "-"), binOp.A, binOp.B);
            }

            Equation = equation;
            Variant = variant;
            ParamMin = paramMin;
            ParamMax = paramMax;
        }

        protected override Val OnEval(EvalContext e) {
            // 初期値の範囲
            var pMinVal = ParamMin != null ? ParamMin.Eval(e) : new RealVal(-10m);
            var pMaxVal = ParamMax != null ? ParamMax.Eval(e) : new RealVal(10m);
            var pMin = (decimal)pMinVal.AsReal;
            var pMax = (decimal)pMaxVal.AsReal;
            if (pMin >= pMax) {
                throw new ArgumentException("Invalid parameter range.");
            }

            // 数値微分用の定数
            var h = Math.Max(1e-20m, (pMax - pMin) / 1e6m);

            // 収束条件
            var tol = Math.Max(1e-25m, (pMax - pMin) / 1e12m);

            // ニュートン法
            var scope = new EvalContext(e);
            var results = new List<decimal>();
            const int N = 100;
            for (int i = 0; i < N; i++) {
                try {
                    var init = pMin + (pMax - pMin) * i / N;
                    if (newtonMethod(scope, init, pMin, pMax, h, tol, out decimal r)) {
                        if (!results.Any(p => Math.Abs(p - r) < tol * 2)) {
                            results.Add(r);
                        }
                    }
                }
                catch { }
            }

            if (results.Count == 1) {
                return new RealVal(results[0]).Format(pMinVal.FormatHint);
            }
            else {
                results.Sort();
                return new ArrayVal(results.Select(p => new RealVal(p).Format(pMinVal.FormatHint)).ToArray());
            }
        }

        private bool newtonMethod(EvalContext e, decimal initVal, decimal pMin, decimal pMax, decimal h, decimal tol, out decimal result) {
            decimal p = initVal;
            for (int i = 0; i < 100; i++) {
                decimal slope = (evalEquation(e, p + h) - evalEquation(e, p - h)) / (2 * h);
                decimal nextP = p - evalEquation(e, p) / slope;
                if (nextP < pMin || pMax < nextP) {
                    result = 0;
                    return false;
                }
                if (Math.Abs(nextP - p) < tol) {
                    result = nextP;
                    return true;
                }
                p = nextP;
            }
            result = 0;
            return false;
        }

        private decimal evalEquation(EvalContext e, decimal param) {
            e.Ref(Variant.Text, true).Value = new RealVal(param);
            return Equation.Eval(e).AsReal;
        }
    }
}
