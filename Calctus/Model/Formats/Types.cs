using System;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Formats {
    // todo 削除: FormatPriority
    /*
    /// <summary>2項演算で結合するときのフォーマット選択の優先度</summary>
    enum FormatPriority {
        /// <summary>弱い優先度</summary>
        Weak,

        /// <summary>強い優先度</summary>
        Strong,

        /// <summary>常に左辺を選ぶ</summary>
        AlwaysLeft,
    }
    */

    [Flags]
    enum StringUsage {
        CharEscapingFlag = (1 << 0),
        StringQuotationFlag = (1 << 1),
        DateTimeQuotationFlag = (1 << 2),
        ForValue = 0,
        ForDisplay = CharEscapingFlag,
        ForLiteral = CharEscapingFlag | StringQuotationFlag | DateTimeQuotationFlag,
    }
}
