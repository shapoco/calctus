using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Model {
    class ArrayVal : Val {

        private Val[] _raw;
        public ArrayVal(Val[] val, ValFormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }
        public ArrayVal(real[] val, ValFormatHint fmt = null) : base(fmt) {
            var array = new Val[val.Length];
            for (int i = 0; i < val.Length; i++) {
                array[i] = new RealVal(val[i], fmt);
            }
            this._raw = array;
        }

        public override object Raw => _raw;

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override real AsReal => throw new InvalidCastException();
        public override frac AsFrac => throw new InvalidCastException();
        public override double AsDouble => throw new InvalidCastException();
        public override long AsLong => throw new InvalidCastException();
        public override int AsInt => throw new InvalidCastException();
        public override bool AsBool => throw new InvalidCastException();

        public override string ToString(EvalContext e) {
            var sb = new StringBuilder();
            sb.Append("[");
            for(int i = 0; i < _raw.Length; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append(_raw[i].ToString(e));
            }
            sb.Append("]");
            return sb.ToString();
        }

        protected override RealVal OnAsRealVal() => throw new InvalidCastException();

        protected override Val OnAdd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnSub(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnMul(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnDiv(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnIDiv(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnMod(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnUnaryPlus(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnAtirhInv(EvalContext ctx) => throw new InvalidOperationException();

        protected override Val OnGrater(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnEqual(EvalContext ctx, Val b) {
            if (b is ArrayVal bArray) {
                return new BoolVal(_raw.Equals(bArray._raw));
            }
            else {
                throw new InvalidOperationException();
            }
        }

        protected override Val OnBitNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnBitAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnBitXor(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnBitOr(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicShiftR(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftR(EvalContext e, Val b) => throw new InvalidOperationException();

        protected override Val OnLogicNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnLogicAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicOr(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnFormat(ValFormatHint fmt) => new ArrayVal(_raw, fmt);

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            if (b is ArrayVal) return this;
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }
    }
}
