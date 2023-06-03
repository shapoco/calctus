using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Expressions {
    class SolveExpr : Expr {
        public readonly Expr Equation;
        public readonly Token Variant;
        public readonly Expr Param0;
        public readonly Expr Param1;

        public SolveExpr(Token keyword, Expr equation, Token variant, Expr paramMin, Expr paramMax) : base(keyword) {
            // 等式が与えられた場合は減算にすり替える
            if (equation is BinaryOp binOp && (binOp.Method == OpDef.Assign || binOp.Method == OpDef.Equal)) {
                equation = new BinaryOp(new Token(TokenType.OperatorSymbol, TextPosition.Nowhere, "-"), binOp.A, binOp.B);
            }

            Equation = equation;
            Variant = variant;
            Param0 = paramMin;
            Param1 = paramMax;
        }

        protected override Val OnEval(EvalContext e) {
            // パラメータの評価
            var paramVal0 = Param0 != null ? Param0.Eval(e) : null;
            var paramVal1 = Param1 != null ? Param1.Eval(e) : null;
            var formatHint = new FormatHint(NumberFormatter.CStyleReal);

            var initVals = new List<decimal>();
            var h = 1e-5m;
            var tol = 1e-10m;
            var pMin = decimal.MinValue;
            var pMax = decimal.MaxValue;
            const int N = 100;

            if (paramVal0 != null && paramVal1 != null) {
                // 初期値の範囲が指定された場合
                // --> 範囲を N 分割して初期値を決定
                pMin = (decimal)paramVal0.AsReal;
                pMax = (decimal)paramVal1.AsReal;
                if (pMin >= pMax) {
                    throw new ArgumentException("Invalid parameter range.");
                }
                for (int i = 0; i <= N; i++) {
                    initVals.Add(pMin + (pMax - pMin) * i / N);
                }
                h = Math.Max(1e-16m, (pMax - pMin) / 1e6m);
                tol = Math.Max(1e-20m, (pMax - pMin) / 1e12m);
                formatHint = paramVal0.FormatHint;
            }
            else if (paramVal0 != null) {
                // 初期値が直接指定された場合
                if (paramVal0 is ArrayVal) {
                    // 初期値が配列で指定された場合
                    foreach(var val in paramVal0.AsRealArray) {
                        initVals.Add(val);
                    }
                    var paramValArray = (Val[])paramVal0.Raw;
                    if (paramValArray.Length > 0) {
                        formatHint = paramValArray[0].FormatHint;
                    }
                }
                else {
                    // 初期値がスカラ値で指定された場合
                    initVals.Add(paramVal0.AsReal);
                    formatHint = paramVal0.FormatHint;
                }
            }
            else {
                // 引数が指定されなかった場合
                for (int i = 0; i <= N; i++) {
                    initVals.Add(-10 + 20 * i / N);
                }
            }

            // ニュートン法
            var scope = new EvalContext(e);
            scope.Settings.AccuracyPriority = false; // 速度優先の計算方法を適用
            scope.Settings.FractionEnabled = false; // 分数は使用しない
            var results = new List<decimal>();
            foreach (var init in initVals) {
                if (newtonMethod(scope, init, pMin, pMax, h, tol, out decimal r)) {
                    if (!results.Any(p => Math.Abs(p - r) < tol * 2)) {
                        results.Add(r);
                    }
                }
            }

            if (results.Count == 1) {
                return new RealVal(results[0]).Format(formatHint);
            }
            else {
                results.Sort();
                return new ArrayVal(results.Select(p => new RealVal(p).Format(formatHint)).ToArray());
            }
        }

        private bool newtonMethod(EvalContext e, decimal initVal, decimal pMin, decimal pMax, decimal h, decimal tol, out decimal result) {
            result = 0;
            try {
                decimal p = initVal;
                for (int i = 0; i < 100; i++) {
                    decimal slope = (evalEquation(e, p + h) - evalEquation(e, p - h)) / (2 * h);
                    if (slope == 0) {
                        return false;
                    }
                    decimal nextP = p - evalEquation(e, p) / slope;
                    if (nextP < pMin || pMax < nextP) {
                        return false;
                    }
                    if (Math.Abs(nextP - p) < tol) {
                        result = nextP;
                        return true;
                    }
                    p = nextP;
                }
                return false;
            }
            catch {
                return false;
            }
        }

        private decimal evalEquation(EvalContext e, decimal param) {
            e.Ref(Variant.Text, true).Value = new RealVal(param);
            return Equation.Eval(e).AsReal;
        }
    }
}
