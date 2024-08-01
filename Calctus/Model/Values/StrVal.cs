using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    // TElem を ApFixedVal にする
    class StrVal : BaseVal<string>, ICollectionVal {
        public static readonly StrVal Empty = new StrVal("");

        public static void CheckStringLength(int length) {
            if (length > Settings.Instance.Calculation_Limit_MaxStringLength) throw new CalctusError("String length exceeds limit.");
        }

        public StrVal(string val) : base (val) {
            CheckStringLength(val.Length);
        }

        public int Length => _raw.Length;

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

        public override Val Add(EvalContext e, Val b) => new StrVal(_raw + ((StrVal)b).Raw);
        public override Val Mul(EvalContext e, Val b) {
            int n = b.AsInt;
            if (n < 0) throw new ArgumentOutOfRangeException();
            CheckStringLength(_raw.Length * n);
            var sb = new StringBuilder();
            for (int i = 0; i < b.AsInt; i++) {
                sb.Append(_raw);
            }
            return new StrVal(sb.ToString());
        }

        public override bool Grater(EvalContext e, Val b) => _raw.CompareTo(rawOf(b)) > 0;
        public override bool Equals(EvalContext e, Val b) => _raw == rawOf(b);

        public Array ToRawArray() => _raw.ToCharArray();
        public Val[] ToValArray() => _raw.ToCharArray().ToValArray();
        public ListVal ToListVal() => new ListVal(ToValArray());
    }
}
