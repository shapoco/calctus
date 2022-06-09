using Shapoco.Calctus.Model.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class Mat : Val {
        private Val[] _raw;

        /// <summary>行数</summary>
        public readonly int M;

        /// <summary>列数</summary>
        public readonly int N;

        public Mat(int m, int n, Val[] elms) {
            M = m;
            N = n;
            _raw = elms;
        }

        public Mat(int m, int n) {
            M = m;
            N = n;
            _raw = new Val[m * n];
            var zero = new RealVal(0, new ValFormatHint(NumberFormatter.CStyleInt));
            var one = new RealVal(1, new ValFormatHint(NumberFormatter.CStyleInt));
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < n; j++) {
                    _raw[i * n + j] = (i == j) ? one : zero;
                }
            }
        }

        public Val this[int i] {
            get { return _raw[i]; }
            private set { _raw[i] = value; }
        }

        public Val this[int i, int j] {
            get {
                if (i < 0 || M <= i || j < 0 || N <= j) throw new IndexOutOfRangeException();
                return _raw[i * N + j];
            }

            private set {
                if (i < 0 || M <= i || j < 0 || N <= j) throw new IndexOutOfRangeException();
                _raw[i * N + j] = value;
            }
        }

        public override object Raw => _raw;
        public override bool IsScalar => false;
        public override bool IsInteger => false;
        public override double AsDouble => throw new CalctusError("行列またはベクトルを実数値に変換できません");
        public override long AsLong => throw new CalctusError("行列またはベクトルを整数値に変換できません");
        public override int AsInt => throw new CalctusError("行列またはベクトルを整数値に変換できません");

        public enum Operation {
            Add, Sub, CrossMul, DotMul, Div, IDiv, Mod, Pow
        };

        /// <summary>外積</summary>
        public static Mat Mul(EvalContext ctx, Val aVal, Val bVal) {
            var A = (Mat)aVal;
            var B = (Mat)bVal;
            if (A.N != B.M) {
                throw new CalctusError("Matrix shape mismatch.");
            }
            int anbm = A.N;
            var R = new Mat(A.M, B.N);
            for (int ai = 0; ai < A.M; ai++) {
                for (int bj = 0; bj < B.N; bj++) {
                    var retval = A[ai, 0].Mul(ctx, B[0, bj]);
                    for (int ajbi = 1; ajbi < anbm; ajbi++) {
                        retval = retval.Add(ctx, A[ai, ajbi].Mul(ctx, B[ajbi, bj]));
                    }
                    R[ai, bj] = retval.AsRealVal();
                }
            }
            return R;
        }

        /// <summary>内積</summary>
        public static Val Dot(EvalContext ctx, Val aVal, Val bVal) {
            var A = (Mat)aVal;
            var B = (Mat)bVal;
            if (A.N != B.N || A.M != B.M) {
                throw new CalctusError("Matrix shape mismatch.");
            }
            var ret = A[0].Mul(ctx, B[0]);
            for (int i = 1; i < A.M * A.N; i++) {
                ret = ret.Add(ctx, A[i].Mul(ctx, B[i]));
            }
            return ret;
        }

        /// <summary>転置</summary>
        public Mat Transpose(EvalContext ctx) {
            var R = new Mat(N, M);
            for (int i = 0; i < M; i++) {
                for (int j = 0; j < N; j++) {
                    R[j, i] = (Val)this[i, j].Clone();
                }
            }
            return R;
        }

        /// <summary>逆行列</summary>
        public Mat Invert(EvalContext ctx) {
            const double MAX_ERR = 1.0e-10;

            var W = new Mat(N, N * 2);
            var I = new Mat(N, N);

            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    W[i, j] = this[i, j];
                    W[i, N + j] = I[i, j];
                }
            }

            for (int k = 0; k < N; k++) {
                double max = Math.Abs(W[k, k].AsDouble);
                int max_i = k;

                for (int i = k + 1; i < N; i++) {
                    var abs_ik = Math.Abs(W[i, k].AsDouble);
                    if (abs_ik > max) {
                        max = abs_ik;
                        max_i = i;
                    }
                }

                if (Math.Abs(W[max_i, k].AsDouble) <= MAX_ERR) {
                    throw new CalctusError("Too large error.");
                }

                if (k != max_i) {
                    for (int j = 0; j < N * 2; j++) {
                        var tmp = W[max_i, j];
                        W[max_i, j] = W[k, j];
                        W[k, j] = tmp;
                    }
                }

                {
                    var tmp = (Val)W[k, k].Clone();
                    for (int j = 0; j < N * 2; j++) {
                        W[k, j] = W[k, j].Div(ctx, tmp);
                    }
                }

                for (int i = 0; i < N; i++) {
                    if (i == k) continue;
                    var tmp = W[i, k];
                    for (int j = 0; j < N * 2; j++) {
                        W[i, j] = W[i, j].Sub(ctx, W[k, j].Mul(ctx, tmp));
                    }
                }
            }

            var R = new Mat(N, N);
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    R[i, j] = W[i, N + j];
                }
            }
            return R;
        }

        public static Val Operate(EvalContext ctx, Mat a, Val bVal, Operation op) {
            if (bVal is Mat b) {
                if (op == Operation.CrossMul) {
                    return Mul(ctx, a, b);
                }
                else if (op == Operation.DotMul) {
                    return Dot(ctx, a, b);
                }
                else {
                    throw new CalctusError(op + " cannot be applied with matrix value.");
                }
            }
            else if (bVal.IsScalar) {
                var retval = (Mat)a.Clone();
                for (int i = 0; i < retval._raw.Length; i++) {
                    switch (op) {
                        case Operation.Add: retval._raw[i] = retval._raw[i].Add(ctx, bVal).AsRealVal(); break;
                        case Operation.Sub: retval._raw[i] = retval._raw[i].Sub(ctx, bVal).AsRealVal(); break;
                        case Operation.CrossMul: retval._raw[i] = retval._raw[i].Mul(ctx, bVal).AsRealVal(); break;
                        case Operation.DotMul: retval._raw[i] = retval._raw[i].DotMul(ctx, bVal).AsRealVal(); break;
                        case Operation.Div: retval._raw[i] = retval._raw[i].Div(ctx, bVal).AsRealVal(); break;
                        case Operation.IDiv: retval._raw[i] = retval._raw[i].IDiv(ctx, bVal).AsRealVal(); break;
                        case Operation.Mod: retval._raw[i] = retval._raw[i].Mod(ctx, bVal).AsRealVal(); break;
                        default: throw new CalctusError(op + " cannot be applied with scalar value.");
                    }
                }
                return retval;
            }
            else {
                throw new CalctusError(op + " can only be applied with scalar value.");
            }
        }

        protected override Val OnAdd(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.Add);
        protected override Val OnSub(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.Sub);
        protected override Val OnMul(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.CrossMul);
        protected override Val OnDotMul(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.DotMul);
        protected override Val OnDiv(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.Div);
        protected override Val OnIDiv(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.IDiv);
        protected override Val OnMod(EvalContext ctx, Val b) => Operate(ctx, this, b, Operation.Mod);
        protected override Val OnPow(EvalContext ctx, Val b) {
            if (!b.IsInteger) throw new CalctusError(b + " must be integer.");
            int n = b.AsInt;
            if (n > 0) {
                Val R = this;
                for (int i = 1; i < n; i++) {
                    R = R.Mul(ctx, R);
                }
                return R;
            }
            else if (n == 0) {
                return new Mat(M, N);
            }
            else if (n % 2 == 0) {
                return (Mat)this.Clone();
            }
            else {
                return this.Invert(ctx);
            }
        }

        protected override Val OnArithShiftL(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnArithShiftR(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnAsDimless(EvalContext ctx) => throw new NotImplementedException();
        protected override Val OnAtirhInv(EvalContext ctx) => throw new NotImplementedException();
        protected override Val OnBitAnd(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnBitNot(EvalContext ctx) => throw new NotImplementedException();
        protected override Val OnBitOr(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnBitXor(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnLogicShiftL(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnLogicShiftR(EvalContext ctx, Val b) => throw new NotImplementedException();
        protected override Val OnUnaryPlus(EvalContext ctx) => throw new NotImplementedException();

        protected override Val OnFormat(ValFormatHint fmt) {
            var elms = new Val[M * N];
            for ( int i = 0; i < M * N; i++) {
                elms[i] = _raw[i].Format(fmt).AsRealVal();
            }
            return new Mat(M, N, elms);
        }

        protected override RealVal OnAsRealVal() {
            throw new InvalidOperationException();
        }

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            return this; // todo: check
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < M; i++) {
                for (int j = 0; j < N; j++) {
                    sb.Append(_raw[i * N + j].ToString());
                    if (j + 1 < N) sb.Append(", ");
                }
                if (i + 1 < M) sb.Append("; ");
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
