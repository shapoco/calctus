using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    class NullVal : Val {
        public static readonly NullVal Instance = new NullVal();

        private NullVal() : base() { }

        protected override object OnGetRaw() => null;

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override bool IsSerializable => false;

        public override decimal AsDecimal => throw new InvalidCastException();
        public override Frac AsFrac => throw new InvalidCastException();
        public override double AsDouble => throw new InvalidCastException();
        public override long AsLong => throw new InvalidCastException();
        public override int AsInt => throw new InvalidCastException();
        public override char AsChar => throw new InvalidCastException();
        public override byte AsByte => throw new InvalidCastException();

        public override decimal[] AsDecimalArray => throw new InvalidCastException();
        public override long[] AsLongArray => throw new InvalidCastException();
        public override int[] AsIntArray => throw new InvalidCastException();
        public override byte[] AsByteArray => throw new InvalidCastException();

        protected override RealVal OnAsRealVal() => throw new InvalidCastException();

        public override bool Equals(EvalContext ctx, Val b) => b is NullVal;
    }
}
