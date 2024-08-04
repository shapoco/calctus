using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    class FracVal : BaseVal<Frac> {

        public FracVal(Frac val) : base(val) { }

        public override bool IsScalar => true;
        public override bool IsInteger => false;

        public override bool IsSerializable => true;

        public override decimal AsDecimal => (decimal)_raw;
        public override Frac AsFrac => _raw;
        public override double AsDouble => (double)_raw;
        public override long AsLong => MathEx.ToLong((decimal)_raw);
        public override int AsInt => MathEx.ToInt((decimal)_raw);
        public override char AsChar => MathEx.ToChar((decimal)_raw);
        public override byte AsByte => MathEx.ToByte((decimal)_raw);

        public override decimal[] AsDecimalArray => new decimal[] { (decimal)_raw };
        public override long[] AsLongArray => new long[] { (long)_raw };
        public override int[] AsIntArray => new int[] { (int)_raw };
        public override byte[] AsByteArray => new byte[] { (byte)_raw };

        public static Val Normalize(Frac f) {
            if (f.Deno == 1) {
                return new RealVal(f.Nume);
            }
            else {
                return new FracVal(f);
            }
        }

        protected override RealVal OnAsRealVal() => new RealVal((decimal)_raw);

        public override Val Add(EvalContext ctx, Val b) => Normalize(_raw + rawOf(b));
        public override Val Sub(EvalContext ctx, Val b) => Normalize(_raw - rawOf(b));
        public override Val Mul(EvalContext ctx, Val b) => Normalize(_raw * rawOf(b));
        public override Val Div(EvalContext ctx, Val b) => Normalize(_raw / rawOf(b));

        public override Val IDiv(EvalContext ctx, Val b) => new RealVal(Math.Truncate((decimal)_raw / b.AsDecimal));
        public override Val Mod(EvalContext ctx, Val b) => new RealVal((decimal)_raw % b.AsDecimal);

        public override Val UnaryPlus(EvalContext ctx) => this;
        public override Val ArithInv(EvalContext ctx) => Normalize(-_raw);

        public override bool Grater(EvalContext ctx, Val b) => _raw > rawOf(b);
        public override bool Equals(EvalContext ctx, Val b) => _raw == rawOf(b);

        public override Val BitNot(EvalContext ctx) => new RealVal(~this.AsLong);
        public override Val BitAnd(EvalContext ctx, Val b) => new RealVal(this.AsLong & b.AsLong);
        public override Val BitXor(EvalContext e, Val b) => new RealVal(this.AsLong ^ b.AsLong);
        public override Val BitOr(EvalContext e, Val b) => new RealVal(this.AsLong | b.AsLong);
        public override Val LogicShiftL(EvalContext e, Val b) => new RealVal(this.AsLong << b.AsInt);
        public override Val LogicShiftR(EvalContext e, Val b) => new RealVal((UInt64)this.AsLong >> b.AsInt);
        public override Val ArithShiftL(EvalContext e, Val b) {
            var a = this.AsLong;
            var sign = a & (1L << 63);
            var lshift = (a << b.AsInt) & 0x7fffffffffffffffL;
            return new RealVal(sign | lshift);
        }
        public override Val ArithShiftR(EvalContext e, Val b) => new RealVal(this.AsLong >> b.AsInt);
    }
}
