using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Maths.Types;

namespace Shapoco.Calctus.Model.Types {
    abstract class ValBase<T> : Val {
        protected readonly T _raw;
        public ValBase(T val, FormatHint fmt = null) : base(fmt) {
            this._raw = val;
        }

        protected override object OnGetRaw() => _raw;
        public new T Raw => _raw;
    }
}
