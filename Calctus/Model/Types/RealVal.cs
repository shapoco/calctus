using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Types {
    class RealVal : ScalarVal<decimal> {
        public static readonly RealVal Zero = new RealVal(0);
        public static readonly RealVal One = new RealVal(1);

        public const int WeekdayMin = 0;
        public const int WeekdayMax = 6;
        public static readonly RealVal Sunday = (RealVal)(new RealVal(0, FormatHint.Weekday));
        public static readonly RealVal Monday = (RealVal)(new RealVal(1, FormatHint.Weekday));
        public static readonly RealVal Tuesday = (RealVal)(new RealVal(2, FormatHint.Weekday));
        public static readonly RealVal Wednesday = (RealVal)(new RealVal(3, FormatHint.Weekday));
        public static readonly RealVal Thursday = (RealVal)(new RealVal(4, FormatHint.Weekday));
        public static readonly RealVal Friday = (RealVal)(new RealVal(5, FormatHint.Weekday));
        public static readonly RealVal Saturday = (RealVal)(new RealVal(6, FormatHint.Weekday));
        public static readonly RealVal[] Weekdays
            = { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };

        public RealVal(decimal val, FormatHint fmt = null) : base(val, fmt) { }

        public override bool IsInteger => (_raw == (long)_raw);

        public override bool IsSerializable => true;

        protected override Val OnUpConvert(EvalContext e, Val b) {
            if (b is RealVal) return this;
            if (b is FracVal) return new FracVal(new frac(_raw, 1));
            if (b is StrVal) return AsStrVal();
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }

        protected override Val OnUnaryPlus(EvalContext e) => this;
        protected override Val OnAtirhInv(EvalContext e) => new RealVal(-_raw, FormatHint);
        protected override Val OnBitNot(EvalContext e) => new RealVal(~this.AsLong, FormatHint);

        protected override Val OnAdd(EvalContext e, Val b) => new RealVal(_raw + b.AsDecimal, FormatHint);
        protected override Val OnSub(EvalContext e, Val b) => new RealVal(_raw - b.AsDecimal, FormatHint);
        protected override Val OnMul(EvalContext e, Val b) => new RealVal(_raw * b.AsDecimal, FormatHint);
        protected override Val OnDiv(EvalContext e, Val b) => new RealVal(_raw / b.AsDecimal, FormatHint);
        protected override Val OnIDiv(EvalContext e, Val b) => new RealVal(Math.Truncate(_raw / b.AsDecimal), FormatHint);
        protected override Val OnMod(EvalContext e, Val b) => new RealVal(_raw % b.AsDecimal, FormatHint);

        protected override Val OnGrater(EvalContext ctx, Val b) => BoolVal.FromBool(AsDecimal > b.AsDecimal);
        protected override Val OnEqual(EvalContext ctx, Val b) => BoolVal.FromBool(AsDecimal == b.AsDecimal);

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

        protected override Val OnLogicNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnLogicAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicOr(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnFormat(FormatHint fmt) => new RealVal(_raw, fmt);

        protected override RealVal OnAsRealVal() => new RealVal((decimal)Raw, FormatHint);
        public override decimal AsDecimal => _raw;
        public override frac AsFrac => (frac)_raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => DMath.ToLong(_raw);
        public override int AsInt => DMath.ToInt(_raw);
        public override char AsChar => DMath.ToChar(_raw);
        public override byte AsByte => DMath.ToByte(_raw);
        public override bool AsBool => throw new InvalidCastException();
        public override string AsString {
            get {
                if (FormatHint.Format == CharFormat.Instance && char.MinValue <= _raw && _raw <= char.MaxValue) {
                    return ((char)_raw).ToString();
                }
                else {
                    return base.AsString;
                }
            }
        }

        public override decimal[] AsDecimalArray => new decimal[] { _raw };
        public override long[] AsLongArray => new long[] { (long)_raw }; // todo: 丸め/切り捨ての明示は不要？
        public override int[] AsIntArray => new int[] { (int)_raw };
        public override byte[] AsByteArray => new byte[] { (byte)_raw };

        public override string ToString(FormatSettings fs) => FormatHint.Format.Format(this, fs);

        public static implicit operator decimal(RealVal val) => (decimal)val.Raw;
        public static implicit operator RealVal(decimal val) => new RealVal(val);
    }
}
