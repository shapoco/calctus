using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;


namespace Shapoco.Calctus.Model.Values {
    class RealVal : ScalarVal<decimal> {
        public static readonly RealVal Zero = new RealVal(0);
        public static readonly RealVal One = new RealVal(1);

        public static FormatFlags FormatFrom(Val val) {
            if (val is RealVal realVal) return realVal.FormatFlags;
            return FormatFlags.Decimal;
        }

        public RealVal(decimal val, FormatFlags fmt = FormatFlags.Default) : base(val, fmt) { }

        protected override Val OnFormat(FormatFlags fmt) => new RealVal(_raw, fmt);

        public override bool IsInteger => _raw.IsInteger();

        public override bool IsSerializable => true;

        public override Val UnaryPlus(EvalContext e) => this;
        public override Val ArithInv(EvalContext e) => new RealVal(-_raw, FormatFlags);
        public override Val BitNot(EvalContext e) => new RealVal(~this.AsLong, FormatFlags);

        public override Val Add(EvalContext e, Val b) => new RealVal(_raw + rawOf(b), FormatFlags);
        public override Val Sub(EvalContext e, Val b) => new RealVal(_raw - rawOf(b), FormatFlags);
        public override Val Mul(EvalContext e, Val b) => new RealVal(_raw * rawOf(b), FormatFlags);
        public override Val Div(EvalContext e, Val b) => new RealVal(_raw / rawOf(b), FormatFlags);
        public override Val IDiv(EvalContext e, Val b) => new RealVal(Math.Truncate(_raw / rawOf(b)), FormatFlags);
        public override Val Mod(EvalContext e, Val b) => new RealVal(_raw % rawOf(b), FormatFlags);

        public override bool Grater(EvalContext ctx, Val b) => _raw > rawOf(b);
        public override bool Equals(EvalContext ctx, Val b) => _raw == rawOf(b);

        public override Val LogicShiftL(EvalContext e, Val b) => new RealVal(this.AsLong << b.AsInt, FormatFlags);
        public override Val LogicShiftR(EvalContext e, Val b) => new RealVal((UInt64)this.AsLong >> b.AsInt, FormatFlags);
        public override Val ArithShiftL(EvalContext e, Val b) {
            var a = this.AsLong;
            var sign = a & (1L << 63);
            var lshift = (a << b.AsInt) & 0x7fffffffffffffffL;
            return new RealVal(sign | lshift, FormatFlags);
        }
        public override Val ArithShiftR(EvalContext e, Val b) => new RealVal(this.AsLong >> b.AsInt, FormatFlags);

        public override Val BitAnd(EvalContext e, Val b) => new RealVal(this.AsLong & b.AsLong, FormatFlags);
        public override Val BitXor(EvalContext e, Val b) => new RealVal(this.AsLong ^ b.AsLong, FormatFlags);
        public override Val BitOr(EvalContext e, Val b) => new RealVal(this.AsLong | b.AsLong, FormatFlags);

        public override Val LogicNot(EvalContext ctx) => throw new InvalidOperationException();
        public override Val LogicAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        public override Val LogicOr(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override RealVal OnAsRealVal() => new RealVal((decimal)Raw, FormatFlags);
        public override decimal AsDecimal => _raw;
        public override frac AsFrac => (frac)_raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => MathEx.ToLong(_raw);
        public override int AsInt => MathEx.ToInt(_raw);
        public override char AsChar => MathEx.ToChar(_raw);
        public override byte AsByte => MathEx.ToByte(_raw);
        
        public override decimal[] AsDecimalArray => new decimal[] { _raw };
        public override long[] AsLongArray => new long[] { (long)_raw }; // todo: 丸め/切り捨ての明示は不要？
        public override int[] AsIntArray => new int[] { (int)_raw };
        public override byte[] AsByteArray => new byte[] { (byte)_raw };

        public static implicit operator decimal(RealVal val) => (decimal)val.Raw;
        public static implicit operator RealVal(decimal val) => new RealVal(val);
    }
}
