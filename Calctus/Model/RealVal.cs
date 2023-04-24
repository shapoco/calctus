using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Syntax;

namespace Shapoco.Calctus.Model {
    class RealVal : Val {

        private real _raw;
        public RealVal(real val, ValFormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }

        public override object Raw => _raw;

        public override bool IsScalar => true;
        public override bool IsInteger => (_raw == (long)_raw);

        protected override Val OnUpConvert(EvalContext e, Val b) {
            if (b is RealVal) return this;
            if (b is FracVal) return new FracVal(new frac(_raw.Raw, 1));
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }

        protected override Val OnUnaryPlus(EvalContext e) => this;
        protected override Val OnAtirhInv(EvalContext e) => new RealVal(-_raw, FormatHint);
        protected override Val OnBitNot(EvalContext e) => new RealVal(~this.AsLong, FormatHint);

        protected override Val OnAdd(EvalContext e, Val b) => new RealVal(_raw + b.AsReal, FormatHint);
        protected override Val OnSub(EvalContext e, Val b) => new RealVal(_raw - b.AsReal, FormatHint);
        protected override Val OnMul(EvalContext e, Val b) => new RealVal(_raw * b.AsReal, FormatHint);
        protected override Val OnDiv(EvalContext e, Val b) => new RealVal(_raw / b.AsReal, FormatHint);
        protected override Val OnIDiv(EvalContext e, Val b) => new RealVal(RMath.Truncate(_raw / b.AsReal), FormatHint);
        protected override Val OnMod(EvalContext e, Val b) => new RealVal(_raw % b.AsReal, FormatHint);

        protected override Val OnLogicShiftL(EvalContext e, Val b) => new RealVal(this.AsLong << b.AsInt, FormatHint);
        protected override Val OnLogicShiftR(EvalContext e, Val b) => new RealVal((UInt64)this.AsLong >> b.AsInt, FormatHint);
        protected override Val OnArithShiftL(EvalContext e, Val b) {
            var a = this.AsLong;
            var sign = a & (1L << 63);
            var lshift = (a << b.AsInt) & 0x7fffffffffffffffL;
            return new RealVal(sign | lshift, FormatHint);
        }
        protected override Val OnArithShiftR(EvalContext e, Val b) => new RealVal(this.AsLong >> b.AsInt, FormatHint);

        protected override Val OnBitAnd(EvalContext e, Val b) => new RealVal(this.AsLong & b.AsLong, FormatHint);
        protected override Val OnBitXor(EvalContext e, Val b) => new RealVal(this.AsLong ^ b.AsLong, FormatHint);
        protected override Val OnBitOr(EvalContext e, Val b) => new RealVal(this.AsLong | b.AsLong, FormatHint);

        protected override Val OnFormat(ValFormatHint fmt) => new RealVal(_raw, fmt);

        protected override RealVal OnAsRealVal() => new RealVal((real)Raw, FormatHint);
        public override real AsReal => _raw;
        public override frac AsFrac => (frac)_raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => (long)_raw; // todo: 丸め/切り捨ての明示は不要？
        public override int AsInt => (int)_raw; // todo: 丸め/切り捨ての明示は不要？

        public override string ToString(EvalContext e) => FormatHint.Formatter.Format(this, e);
        //public static implicit operator double(RealVal val) => val.AsDouble();
        //public static implicit operator RealVal(double val) => new RealVal(val);

        //public static explicit operator long(RealVal val) => val.AsLong();
        //public static explicit operator RealVal(long val) => new RealVal(val);
    }
}
