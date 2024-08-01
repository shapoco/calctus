using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Values {
    abstract class ScalarVal<TRaw> : BaseVal<TRaw> where TRaw : IComparable<TRaw> {
        private readonly FormatFlags _numericFormat;

        public ScalarVal(TRaw val, FormatFlags fmt =  FormatFlags.Default) : base(val) {
            _numericFormat = fmt;
        }

        public override bool IsScalar => true;

        public override FormatFlags FormatFlags => _numericFormat;
        public override Val Format(FormatFlags fmt) {
            if (this.FormatFlags == fmt) {
                return this;
            }
            else {
                return OnFormat(fmt);
            }
        }
        protected abstract Val OnFormat(FormatFlags fmt);
    }
}
