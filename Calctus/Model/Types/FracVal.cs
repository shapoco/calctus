using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Types {
    class FracVal : Val {

        private frac _raw;
        public FracVal(frac val, FormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }

        public override object Raw => _raw;

        public override bool IsScalar => true;
        public override bool IsInteger => false;

        public override bool IsSerializable => true;

        public override real AsReal => (real)_raw;
        public override frac AsFrac => _raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => RMath.ToLong((real)_raw);
        public override int AsInt => RMath.ToInt((real)_raw);
        public override char AsChar => RMath.ToChar((real)_raw);
        public override byte AsByte => RMath.ToByte((real)_raw);
        public override bool AsBool => throw new InvalidCastException();

        public override real[] AsRealArray => new real[] { (real)_raw };
        public override long[] AsLongArray => new long[] { (long)_raw };
        public override int[] AsIntArray => new int[] { (int)_raw };
        public override byte[] AsByteArray => new byte[] { (byte)_raw };

        public override string ToString(FormatSettings fs) => _raw.ToString();

        public static Val Normalize(frac f, FormatHint hint) {
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

        protected override Val OnGrater(EvalContext ctx, Val b) => BoolVal.FromBool(AsReal > b.AsReal);
        protected override Val OnEqual(EvalContext ctx, Val b) => BoolVal.FromBool(AsReal == b.AsReal);

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

        protected override Val OnLogicNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnLogicAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicOr(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnFormat(FormatHint fmt) => new FracVal(_raw, fmt);

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            if (b is RealVal) return this;
            if (b is FracVal) return this;
            if (b is StrVal) return AsStrVal();
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }
    }
}
