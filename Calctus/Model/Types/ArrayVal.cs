﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Maths.Types;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Types {
    class ArrayVal : ArrayValBase<Val[], Val, Val> {
        public static void CheckArrayLength(int length) {
            int limit = Settings.Instance.Calculation_Limit_MaxArrayLength;
            if (length > limit) {
                throw new CalctusError("Array length exceeds limit (" + length + " > " + limit + ").");
            }
        }

        public ArrayVal(Val[] val, FormatHint fmt = null) : base(val, fmt) {
            CheckArrayLength(val.Length);
        }

        public Val this[int index] => _raw[index];

        public override bool IsSerializable => _raw.All(p => p.IsSerializable);

        public ArrayVal Slice(int from, int to) {
            if (from > to) throw new ArgumentOutOfRangeException();
            Val[] slice = new Val[to - from + 1];
            for (int i = 0; i < slice.Length; i++) {
                slice[i] = _raw[from + i];
            }
            return new ArrayVal(slice, FormatHint);
        }

        public ArrayVal Modify(int from, int to, Val[] newValue) {
            if (from > to) throw new ArgumentOutOfRangeException();
            if (newValue.Length != to - from + 1) throw new ArgumentOutOfRangeException();
            var array = new Val[_raw.Length];
            Array.Copy(_raw, 0, array, 0, _raw.Length);
            for (int i = from; i <= to; i++) {
                array[i] = newValue[i - from];
            }
            return new ArrayVal(array, FormatHint);
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
        public override bool AsBool => throw new InvalidCastException();
        
        public override decimal[] AsDecimalArray => _raw.Select(p => p.AsDecimal).ToArray();
        public override long[] AsLongArray => _raw.Select(p => p.AsLong).ToArray();
        public override int[] AsIntArray => _raw.Select(p => p.AsInt).ToArray();
        public override byte[] AsByteArray => _raw.Select(p => p.AsByte).ToArray();

        public override string ToString(FormatSettings fs) => FormatHint.Format.Format(this, fs);

        protected override RealVal OnAsRealVal() => throw new InvalidCastException();

        protected override Val OnAdd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnSub(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnMul(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnDiv(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnIDiv(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnMod(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnUnaryPlus(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnAtirhInv(EvalContext ctx) => throw new InvalidOperationException();

        protected override Val OnGrater(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnEqual(EvalContext ctx, Val b) {
            if (b is ArrayVal bArray) {
                return BoolVal.FromBool(_raw.Equals(bArray._raw));
            }
            else {
                throw new InvalidOperationException();
            }
        }

        protected override Val OnBitNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnBitAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnBitXor(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnBitOr(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicShiftR(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftR(EvalContext e, Val b) => throw new InvalidOperationException();

        protected override Val OnLogicNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnLogicAnd(EvalContext ctx, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicOr(EvalContext ctx, Val b) => throw new InvalidOperationException();

        protected override Val OnFormat(FormatHint fmt) => new ArrayVal(_raw, fmt);

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            if (b is ArrayVal) return this;
            if (b is StrVal) return AsStrVal();
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }
    }
}
