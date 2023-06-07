using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    /// <summary>2項演算で結合するときのフォーマット選択の優先度</summary>
    enum FormatPriority {
        /// <summary>弱い優先度</summary>
        Weak,

        /// <summary>強い優先度</summary>
        Strong,

        /// <summary>常に左辺を選ぶ</summary>
        AlwaysLeft,
    }
}
