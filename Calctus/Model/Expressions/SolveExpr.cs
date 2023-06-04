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
        public const decimal HMin = 1e-13m;
        public const decimal TolMin = 1e-18m;
        public const decimal HTolRatio = 1e5m;

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

            List<decimal> inits;
            decimal h;
            decimal tol;
            var xMin = decimal.MinValue;
            var xMax = decimal.MaxValue;

            if (paramVal0 == null && paramVal1 == null) {
                // 初期値が与えられなかった場合
                var cands = generateInitCandidates(e, 0);
                inits = filterInitCandidates(cands);
                determineHTol(inits, out h, out tol);
            }
            else if (paramVal0 != null && paramVal1 != null) {
                // 解の範囲が指定された場合
                xMin = paramVal0.AsReal;
                xMax = paramVal1.AsReal;
                var cands = generateInitCandidates(e, xMin, xMax);
                inits = filterInitCandidates(cands);
                determineHTol(xMin, xMax, out h, out tol);
            }
            else if (paramVal0 != null && paramVal1 == null) {
                // 初期値が直接指定された場合
                inits = new List<decimal>();
                if (paramVal0 is ArrayVal) {
                    // 配列で指定された場合
                    foreach (var val in paramVal0.AsRealArray) {
                        inits.Add(val);
                    }
                    var paramValArray = (Val[])paramVal0.Raw;
                    if (paramValArray.Length > 0) {
                        formatHint = paramValArray[0].FormatHint;
                    }
                }
                else {
                    // スカラ値で指定された場合
                    inits.Add(paramVal0.AsReal);
                    formatHint = paramVal0.FormatHint;
                }
                determineHTol(inits, out h, out tol);
            }
            else {
                throw new ArgumentException();
            }

#if DEBUG
            Console.WriteLine("Solve:");
            Console.WriteLine("  " + inits.Count + " inits, xMin=" + xMin + ", xMax=" + xMax + ", h=" + h + ", tol=" + tol);
#endif

            // ニュートン法
            var scope = new EvalContext(e);
            scope.Settings.AccuracyPriority = false; // 速度優先の計算方法を適用
            scope.Settings.FractionEnabled = false; // 分数は使用しない
            var sols = new List<decimal>();
            int numMaxIter = 0;
            foreach (var init in inits) {
                if (newtonMethod(scope, init, xMin, xMax, h, tol, out decimal sol, out int numIter)) {
                    sols.Add(sol);
                    numMaxIter = Math.Max(numMaxIter, numIter);
                }
            }
            sols.Sort();

#if DEBUG
            Console.WriteLine("  " + sols.Count + " raw sols, numMaxIter=" + numMaxIter);
#endif

            // 近接する解をまとめる
            sols = reduceSols(sols, tol);

            if (sols.Count == 1) {
                return new RealVal(sols[0]).Format(formatHint);
            }
            else {
                return new ArrayVal(sols.Select(p => new RealVal(p).Format(formatHint)).ToArray());
            }
        }

        /// <summary>解が分布するX座標の中心を指定して初期値の候補を生成</summary>
        private List<Sample> generateInitCandidates(EvalContext e, decimal center) {
            var cands = new List<Sample>();
            const int scaleFine = 4;
            const int scaleRange = 18 * scaleFine;
            // center の左側
            for (int i = scaleRange; i >= -scaleRange; i--) {
                try {
                    var x = center - (decimal)Math.Pow(10, (double)i / scaleFine);
                    cands.Add(new Sample(x, evalEquation(e, x)));
                }
                catch { }
            }
            // center
            try {
                cands.Add(new Sample(0, evalEquation(e, 0)));
            }
            catch { }
            // center の右側
            for (int i = -scaleRange; i < scaleRange; i++) {
                try {
                    var x = center + (decimal)Math.Pow(10, (double)i / scaleFine);
                    cands.Add(new Sample(x, evalEquation(e, x)));
                }
                catch { }
            }
            return cands;
        }

        /// <summary>解が分布する範囲を指定して初期値の候補を生成</summary>
        private List<Sample> generateInitCandidates(EvalContext e, decimal xMin, decimal xMax) {
            var cands = new List<Sample>();
            const int N = 100;
            for (int i = 0; i <= N; i++) {
                try {
                    var x = xMin + (xMax - xMin) * i / N;
                    cands.Add(new Sample(x, evalEquation(e, x)));
                }
                catch { }
            }
            return cands;
        }

        /// <summary>解に繋がりそうな初期値を抽出する</summary>
        private List<decimal> filterInitCandidates(List<Sample> cands) {
            var map = new Dictionary<int, decimal>();
            int n = cands.Count;
            for (int i = 0; i < n; i++) {
                if (cands[i].Y == 0) {
                    // X軸上の点
                    map[i] = cands[i].X;
                    // ...とその両隣
                    if (i > 0) map[i - 1] = cands[i - 1].X;
                    if (i < n - 1) map[i + 1] = cands[i + 1].X;
                }
                else if (i < n - 1 && Math.Sign(cands[i].Y) != Math.Sign(cands[i + 1].Y)) {
                    // X軸を挟んで取りなう点
                    map[i] = cands[i].X;
                    map[i + 1] = cands[i + 1].X;
                    // ...とその両隣
                    if (i > 0) map[i - 1] = cands[i - 1].X;
                    if (i < n - 2) map[i + 2] = cands[i + 2].X;
                }
                else if (0 < i && i < n - 1) {
                    var y0 = cands[i - 1].Y;
                    var y1 = cands[i].Y;
                    var y2 = cands[i + 1].Y;
                    var posPeak = (y1 > 0) && (y1 <= Math.Min(y0, y2)) && (y1 * 1.1m <= Math.Max(y0, y2));
                    var negPeak = (y1 < 0) && (y1 >= Math.Max(y0, y2)) && (y1 * 1.1m >= Math.Min(y0, y2));
                    if (posPeak || negPeak) {
                        // X軸に向かって突き出ているピークとその両隣
                        map[i - 1] = cands[i - 1].X;
                        map[i] = cands[i].X;
                        map[i + 1] = cands[i + 1].X;
                    }
                }
            }
            return map.Values.ToList();
        }
        
        /// <summary>H, TOL の値を決定する</summary>
        private void determineHTol(List<decimal> inits, out decimal h, out decimal tol) {
            if (inits.Count <= 0) {
                h = 1e-6m;
                tol = h / HTolRatio;
            }
            else if (inits.Count == 1) {
                h = Math.Max(HMin, inits[0] / 1e6m);
                tol = Math.Max(TolMin, h / HTolRatio);
            }
            else {
                determineHTol(inits.Min(), inits.Max(), out h, out tol);
            }
        }

        /// <summary>H, TOL の値を決定する</summary>
        private void determineHTol(decimal xMin, decimal xMax, out decimal h, out decimal tol) {
            h = Math.Max(HMin, (xMax - xMin) / 1e6m);
            tol = Math.Max(TolMin, h / HTolRatio);
        }

        /// <summary>ニュートン法</summary>
        private bool newtonMethod(EvalContext e, decimal init, decimal pMin, decimal pMax, decimal h, decimal tol, out decimal result, out int numIter) {
            result = 0;
            numIter = 0;
            try {
                // ニュートン法
                var x = init;
                for (int i = 0; i < 50; i++) {
                    numIter = i + 1;

                    // 傾き
                    var s = (evalEquation(e, x + h) - evalEquation(e, x - h)) / (2 * h);
                    if (s == 0) return false;

                    // X軸との交点
                    var y = evalEquation(e, x);
                    decimal nextX = x - y / s;
                    if (nextX < pMin || pMax < nextX) return false;

                    // 終了判定
                    if (Math.Abs(nextX - x) < tol) {
                        result = nextX;
                        return true;
                    }

                    x = nextX;
                }
            }
            catch { }
            return false;
        }

        /// <summary>方程式を評価する</summary>
        private decimal evalEquation(EvalContext e, decimal x) {
            e.Ref(Variant.Text, true).Value = new RealVal(x);
            return Equation.Eval(e).AsReal;
        }

        /// <summary>近接する解をまとめて平均をとる</summary>
        private List<decimal> reduceSols(List<decimal> cands, decimal tol) {
            var sols = new List<decimal>();
            var solSum = 0m;
            var numSol = 0;
            var lastSol = 0m;
            for (int i = 0; i < cands.Count; i++) {
                var sol = cands[i];
                if (i == 0) {
                    solSum = sol;
                    numSol = 1;
                }
                else { 
                    if (sol - lastSol < tol * 10) {
                        solSum += sol;
                        numSol++;
                    }
                    else {
                        sols.Add(solSum / numSol);
                        solSum = sol;
                        numSol = 1;
                    }
                }
                lastSol = sol;
            }
            if (numSol > 0) {
                sols.Add(solSum / numSol);
            }
            return sols;
        }

        struct Sample {
            public decimal X;
            public decimal Y;
            public Sample(decimal x, decimal y) {
                X = x;
                Y = y;
            }
        }
    }
}
