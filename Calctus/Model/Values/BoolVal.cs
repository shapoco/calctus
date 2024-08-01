using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    class BoolVal : BaseVal<bool> {
        public static readonly BoolVal True  = new BoolVal(true);
        public static readonly BoolVal False = new BoolVal(false);

        public static BoolVal From(bool val) => val ? True : False;

        private BoolVal(bool val) : base(val) { }

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override bool IsSerializable => true;

        public override decimal AsDecimal => throw new InvalidCastException();
        public override frac AsFrac => throw new InvalidCastException();
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

        public override bool Equals(EvalContext ctx, Val b) => _raw == rawOf(b);

        public override Val BitAnd(EvalContext ctx, Val b) => new BoolVal(_raw & rawOf(b));
        public override Val BitXor(EvalContext e, Val b) => new BoolVal(_raw ^ rawOf(b));
        public override Val BitOr(EvalContext e, Val b) => new BoolVal(_raw | rawOf(b));

        public override Val LogicNot(EvalContext ctx) => new BoolVal(!_raw);
        public override Val LogicAnd(EvalContext ctx, Val b) => new BoolVal(_raw && rawOf(b));
        public override Val LogicOr(EvalContext ctx, Val b) => new BoolVal(_raw || rawOf(b));
    }
}
