using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Types {
    abstract class ScalarVal<T> : ValBase<T> {
        public ScalarVal(T val, FormatHint fmt = null) : base(val, fmt) { }

        public override bool IsScalar => true;
    }
}
