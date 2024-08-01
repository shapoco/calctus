﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Values {
    class FuncVal : BaseVal<FuncDef> {
        public FuncVal(FuncDef val) : base(val) { }

        public override bool IsScalar => false;
        public override bool IsInteger => false;

        public override bool IsSerializable => false;

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
    }
}
