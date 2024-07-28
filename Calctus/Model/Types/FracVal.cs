using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Types {
    class FracVal : ValBase<frac> {

        public FracVal(frac val, FormatHint fmt = null) : base(val,fmt) { }

        public override bool IsScalar => true;
        public override bool IsInteger => false;

        public override bool IsSerializable => true;

        public override decimal AsDecimal => (decimal)_raw;
        public override frac AsFrac => _raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => DMath.ToLong((decimal)_raw);
        public override int AsInt => DMath.ToInt((decimal)_raw);
        public override char AsChar => DMath.ToChar((decimal)_raw);
        public override byte AsByte => DMath.ToByte((decimal)_raw);
        public override bool AsBool => throw new InvalidCastException();

        public override decimal[] AsDecimalArray => new decimal[] { (decimal)_raw };
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

        protected override RealVal OnAsRealVal() => new RealVal((decimal)_raw, FormatHint);

        protected override Val OnAdd(EvalContext ctx, Val b) => Normalize(_raw + b.AsFrac, FormatHint);
        protected override Val OnSub(EvalContext ctx, Val b) => Normalize(_raw - b.AsFrac, FormatHint);
        protected override Val OnMul(EvalContext ctx, Val b) => Normalize(_raw * b.AsFrac, FormatHint);
        protected override Val OnDiv(EvalContext ctx, Val b) => Normalize(_raw / b.AsFrac, FormatHint);

        protected override Val OnIDiv(EvalContext ctx, Val b) => new RealVal(Math.Truncate((decimal)_raw / b.AsDecimal), FormatHint);
        protected override Val OnMod(EvalContext ctx, Val b) => new RealVal((decimal)_raw % b.AsDecimal, FormatHint);

        protected override Val OnUnaryPlus(EvalContext ctx) => this;
        protected override Val OnAtirhInv(EvalContext ctx) => Normalize(-_raw, FormatHint);

        protected override Val OnGrater(EvalContext ctx, Val b) => BoolVal.FromBool(AsDecimal > b.AsDecimal);
        protected override Val OnEqual(EvalContext ctx, Val b) => BoolVal.FromBool(AsDecimal == b.AsDecimal);

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
