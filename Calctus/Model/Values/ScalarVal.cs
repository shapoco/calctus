using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Values {
    abstract class ScalarVal<TRaw> : BaseVal<TRaw> where TRaw : IComparable<TRaw> {
        private readonly FormatHint _numericFormat;

        public ScalarVal(TRaw val, FormatHint fmt = null) : base(val) {
            if (fmt == null) fmt = FormatHint.Default;
            _numericFormat = fmt;
        }

        public override bool IsScalar => true;

        public override FormatHint FormatHint => _numericFormat;
        public override Val Format(FormatHint fmt) {
            if (this.FormatHint.Equals(fmt)) {
                return this;
            }
            else {
                return OnFormat(fmt);
            }
        }
        protected abstract Val OnFormat(FormatHint fmt);
    }
}
