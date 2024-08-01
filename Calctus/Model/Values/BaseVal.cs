using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths;


namespace Shapoco.Calctus.Model.Values {
    abstract class BaseVal<TRaw> : Val {
        protected readonly TRaw _raw;
        public BaseVal(TRaw val) {
            this._raw = val;
        }

        protected override object OnGetRaw() => _raw;
        public new TRaw Raw => _raw;

        protected static TRaw rawOf(Val val) => ((BaseVal<TRaw>)val)._raw;
    }
}
