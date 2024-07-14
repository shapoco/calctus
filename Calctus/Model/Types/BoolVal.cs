﻿using Shapoco.Calctus.Model.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Types {
    class BoolVal : Val {
        public static readonly BoolVal True  = new BoolVal(true);
        public static readonly BoolVal False = new BoolVal(false);

        public static BoolVal FromBool(bool val) => val ? True : False;

        private bool _raw;
        private BoolVal(bool val, FormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }

        public override object Raw => _raw;

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override bool IsSerializable => true;

        public override real AsReal => throw new InvalidCastException();
        public override frac AsFrac => throw new InvalidCastException();
        public override double AsDouble => throw new InvalidCastException();
        public override long AsLong => throw new InvalidCastException();
        public override int AsInt => throw new InvalidCastException();
        public override char AsChar => throw new InvalidCastException();
        public override byte AsByte => throw new InvalidCastException();
        public override bool AsBool => _raw;

        public override real[] AsRealArray => throw new InvalidCastException();
        public override long[] AsLongArray => throw new InvalidCastException();
        public override int[] AsIntArray => throw new InvalidCastException();
        public override byte[] AsByteArray => throw new InvalidCastException();

        public override string ToString(FormatSettings fs) => BoolFormat.FormatAsStringLiteral(_raw);

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
        protected override Val OnEqual(EvalContext ctx, Val b) => new BoolVal(AsBool == b.AsBool);

        protected override Val OnBitNot(EvalContext ctx) => throw new InvalidOperationException();
        protected override Val OnBitAnd(EvalContext ctx, Val b) => new BoolVal(AsBool & b.AsBool);
        protected override Val OnBitXor(EvalContext e, Val b) => new BoolVal(AsBool ^ b.AsBool);
        protected override Val OnBitOr(EvalContext e, Val b) => new BoolVal(AsBool | b.AsBool);
        protected override Val OnLogicShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnLogicShiftR(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftL(EvalContext e, Val b) => throw new InvalidOperationException();
        protected override Val OnArithShiftR(EvalContext e, Val b) => throw new InvalidOperationException();

        protected override Val OnLogicNot(EvalContext ctx) => new BoolVal(!AsBool);
        protected override Val OnLogicAnd(EvalContext ctx, Val b) => new BoolVal(AsBool && b.AsBool);
        protected override Val OnLogicOr(EvalContext ctx, Val b) => new BoolVal(AsBool || b.AsBool);

        protected override Val OnFormat(FormatHint fmt) => new BoolVal(_raw, fmt);

        protected override Val OnUpConvert(EvalContext ctx, Val b) {
            if (b is BoolVal) return this;
            if (b is StrVal) return AsStrVal();
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }
    }
}
