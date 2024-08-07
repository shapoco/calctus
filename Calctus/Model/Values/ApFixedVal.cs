using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Maths;

namespace Shapoco.Calctus.Model.Values {
    class ApFixedVal : ScalarVal<apfixed> {
        public ApFixedVal(apfixed val, FormatHint fmt = null) : base(val, fmt.OrDefault(FormatHint.Hexadecimal)) { }

        public override bool IsInteger => _raw.IsInteger;

        public override bool IsSerializable => true;

        public override decimal AsDecimal => throw new NotImplementedException();
        public override Frac AsFrac => throw new NotImplementedException();
        public override double AsDouble => throw new NotImplementedException();
        public override long AsLong => throw new NotImplementedException();
        public override int AsInt => throw new NotImplementedException();
        public override char AsChar => throw new NotImplementedException();
        public override byte AsByte => throw new NotImplementedException();
        public override decimal[] AsDecimalArray => throw new NotImplementedException();
        public override long[] AsLongArray => throw new NotImplementedException();
        public override int[] AsIntArray => throw new NotImplementedException();
        public override byte[] AsByteArray => throw new NotImplementedException();

        public override Val ArithInv(EvalContext ctx) => new ApFixedVal(-_raw, FormatHint);

        public override Val Add(EvalContext ctx, Val b) => new ApFixedVal(_raw + ((ApFixedVal)b)._raw, FormatHint);
        public override Val Sub(EvalContext ctx, Val b) => new ApFixedVal(_raw - ((ApFixedVal)b)._raw, FormatHint);

        public override bool Equals(EvalContext ctx, Val b) => _raw.Equals(((ApFixedVal)b)._raw);
        public override bool Grater(EvalContext ctx, Val b) => _raw.CompareTo(((ApFixedVal)b)._raw) > 0;

        protected override RealVal OnAsRealVal() => throw new NotImplementedException();
        protected override Val OnFormat(FormatHint fmt) => new ApFixedVal(Raw, fmt);

#if DEBUG
        public static void DoTest() {
            var e = new EvalContext();
            Test.AssertEqual(e, "0x1234u16", "0x1234s16");
            Test.AssertEqual(e, "0x7777s16 + 0x1234u16", "0x89abu16");
            Test.AssertEqual(e, "0x7777s16 - 0x1234u16", "0x6543u16");
            Test.AssertEqual(e, "-0x7777u16", "0x18889s17");
            Test.AssertEqual(e, "0x1234u16 - 0x7777u16", "0x19abds17");
            Test.AssertEqual(e, "0b0001001000110100u16", "0x1234u16");
            Test.AssertEqual(e, "0o011064u16", "0x1234u16");
        }
#endif
    }
}
