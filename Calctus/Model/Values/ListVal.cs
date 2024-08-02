using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;

using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Values {
    class ListVal : BaseVal<Val[]>, ICollectionVal {
        public static void CheckArrayLength(int length) {
            int limit = Settings.Instance.Calculation_Limit_MaxArrayLength;
            if (length > limit) {
                throw new CalctusError("Array length exceeds limit (" + length + " > " + limit + ").");
            }
        }

        public ListVal(Val[] val) : base(val) {
            CheckArrayLength(val.Length);
        }

        public Val this[int index] => _raw[index];

        public override bool IsSerializable => _raw.All(p => p.IsSerializable);

        public ListVal Slice(int from, int to) {
            if (from > to) throw new ArgumentOutOfRangeException();
            Val[] slice = new Val[to - from + 1];
            for (int i = 0; i < slice.Length; i++) {
                slice[i] = _raw[from + i];
            }
            return new ListVal(slice);
        }

        public ListVal Modify(int from, int to, Val[] newValue) {
            if (from > to) throw new ArgumentOutOfRangeException();
            if (newValue.Length != to - from + 1) throw new ArgumentOutOfRangeException();
            var array = new Val[_raw.Length];
            Array.Copy(_raw, 0, array, 0, _raw.Length);
            for (int i = from; i <= to; i++) {
                array[i] = newValue[i - from];
            }
            return new ListVal(array);
        }

        public int Length => _raw.Length;

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override decimal AsDecimal => throw new InvalidCastException();
        public override frac AsFrac => throw new InvalidCastException();
        public override double AsDouble => throw new InvalidCastException();
        public override long AsLong => throw new InvalidCastException();
        public override int AsInt => throw new InvalidCastException();
        public override char AsChar => throw new InvalidCastException();
        public override byte AsByte => throw new InvalidCastException();
        
        public override decimal[] AsDecimalArray => _raw.Select(p => p.AsDecimal).ToArray();
        public override long[] AsLongArray => _raw.Select(p => p.AsLong).ToArray();
        public override int[] AsIntArray => _raw.Select(p => p.AsInt).ToArray();
        public override byte[] AsByteArray => _raw.Select(p => p.AsByte).ToArray();

        protected override RealVal OnAsRealVal() => throw new InvalidCastException();

        public ListVal ToListVal() => this;
        public Val[] ToValArray() => (Val[])_raw.Clone();
        public Array ToRawArray() => (Val[])_raw.Clone();

        public Val GetElement(EvalContext e, int index)
            => _raw[this.NormalizeIndex(index)];
        
        public Val GetSlice(EvalContext e, int from, int to) {
            this.NormalizeIndex(ref from, ref to);
            var partLen = to - from;
            var ret = new Val[partLen];
            for(int i = 0; i < partLen; i++) {
                ret[i] = _raw[from + i];
            }
            return ret.ToVal();
        }

        public Val SetSelement(EvalContext e, int index, Val val) {
            var ret = (Val[])_raw.Clone();
            ret[this.NormalizeIndex(index)] = val;
            return ret.ToVal();
        }

        public Val SetRange(EvalContext e, int from, int to, Val val) {
            this.NormalizeIndex(ref from, ref to);
            var partLen = to - from;
            var valArray = ((ListVal)val).Raw;
            var ret = (Val[])_raw.Clone();
            for (int i = 0; i < partLen; i++) {
                ret[i + from] = valArray[i];
            }
            return ret.ToVal();
        }
    }
}
