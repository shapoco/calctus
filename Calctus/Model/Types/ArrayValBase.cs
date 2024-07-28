using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Types {
    abstract class ArrayValBase<TRawArray, TRawElem, TElemVal> : ValBase<TRawArray> where TRawArray : IEnumerable<TRawElem> {
        public ArrayValBase(TRawArray val, FormatHint fmt) : base(val, fmt) { }
    }
}
