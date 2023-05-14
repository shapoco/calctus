using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Syntax {
    /// <summary>Val の表現に関するヒントを格納するクラス</summary>
    class ValFormatHint {
        public static readonly ValFormatHint Default = new ValFormatHint(NumberFormatter.CStyleReal);

        public readonly NumberFormatter Formatter;

        public ValFormatHint(NumberFormatter f) {
            this.Formatter = f;
        }

        public ValFormatHint Select(ValFormatHint b) {
            if (Formatter.Priority == FormatPriority.Neutral && b.Formatter.Priority == FormatPriority.NextPriority) {
                // 左辺が標準の整数または実数で右辺が接頭語付きの場合は接頭語を優先する
                return b;
            }
            // それ以外の場合は左辺を優先する
            return this;
        }
    }
}
