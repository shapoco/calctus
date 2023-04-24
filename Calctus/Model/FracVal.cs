using Shapoco.Calctus.Model.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    class FracVal : Val {

        private frac _raw;
        public FracVal(frac val, ValFormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }

        public override object Raw => _raw;

        public override bool IsScalar => true;
        public override bool IsInteger => false;
        
        public override real AsReal => (real)_raw;
        public override frac AsFrac => _raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => (long)_raw;
        public override int AsInt => (int)_raw;

        public override string ToString(EvalContext e) => _raw.ToString();

        public static Val Normalize(frac f, ValFormatHint hint) {
            if (f.Deno == 1) {
                return new RealVal(f.Nume, hint);
            }
            else {
                return new FracVal(f, hint);
            }
        }

        protected override RealVal OnAsRealVal() => new RealVal((real)_raw, FormatHint);

        protected override Val OnAdd(EvalContext ctx, Val b) => Normalize(_raw + b.AsFrac, FormatHint);
        protected override Val OnSub(EvalContext ctx, Val b) => Normalize(_raw - b.AsFrac, FormatHint);
        protected override Val OnMul(EvalContext ctx, Val b) => Normalize(_raw * b.AsFrac, FormatHint);
        protected override Val OnDiv(EvalContext ctx, Val b) => Normalize(_raw / b.AsFrac, FormatHint);

        protected override Val OnIDiv(EvalContext ctx, Val b) => new RealVal(RMath.Truncate((real)_raw / b.AsReal), FormatHint);
        protected override Val OnMod(EvalContext ctx, Val b) => new RealVal((real)_raw % b.AsReal, FormatHint);

        protected override Val OnUnaryPlus(EvalContext ctx) => this;
        protected override Val OnAtirhInv(EvalContext ctx) => Normalize(-_raw, FormatHint);

        protected override Val OnBitNot(EvalContext ctx) => new RealVal(~this.AsLong, FormatHint);
        protected override Val OnBitAnd(EvalContext ctx, Val b) => new RealVal(this.AsLong & b.AsLong, FormatHint);
        protected override Val OnBitXor(EvalContext e, Val b) => new RealVal(this.AsLong ^ b.AsLong, FormatHint);
        protected override Val OnBitOr(EvalContext e, Val b) => new RealVal(this.AsLong | b.AsLong, FormatHint);
        protected override Val OnLogicShiftL(EvalContext e, Val b) => new RealVal(this.AsLong << b.AsInt, FormatHint);
        protected override Val OnLogicShiftR(EvalContext e, Val b) => new RealVal((UInt64)this.AsLong >> b.AsInt, FormatHint);
        protected override Val OnArithShiftL(EvalContext e, Val b) {
            var a = this.AsLong;
            var sign = a & (1L << 63);
            var lshift = (a << b.AsInt) & 0x7fffffffffffffffL;
            return new RealVal(sign | lshift, FormatHint);
        }
        protected override Val OnArithShiftR(EvalContext e, Val b) => new RealVal(this.AsLong >> b.AsInt, FormatHint);

        protected override Val OnFormat(ValFormatHint fmt) => new FracVal(_raw, fmt);

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            if (b is RealVal) return this;
            if (b is FracVal) return this;
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }
    }
}
