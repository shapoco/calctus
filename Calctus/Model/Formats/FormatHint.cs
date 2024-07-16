using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    /// <summary>Val の表現に関するヒントを格納するクラス</summary>
    class FormatHint {
        public static readonly FormatHint CStyleInt = new FormatHint(ValFormat.CStyleInt);
        public static readonly FormatHint CStyleHex = new FormatHint(ValFormat.CStyleHex);
        public static readonly FormatHint CStyleOct = new FormatHint(ValFormat.CStyleOct);
        public static readonly FormatHint CStyleBin = new FormatHint(ValFormat.CStyleBin);
        public static readonly FormatHint CStyleString = new FormatHint(ValFormat.CStyleString);
        public static readonly FormatHint CStyleChar = new FormatHint(ValFormat.CStyleChar);
        public static readonly FormatHint CStyleReal = new FormatHint(ValFormat.CStyleReal);
        public static readonly FormatHint SiPrefixed = new FormatHint(ValFormat.SiPrefixed);
        public static readonly FormatHint BinaryPrefixed = new FormatHint(ValFormat.BinaryPrefixed);
        public static readonly FormatHint WebColor = new FormatHint(ValFormat.WebColor);
        public static readonly FormatHint DateTime = new FormatHint(ValFormat.DateTime);
        public static readonly FormatHint RelativeTime = new FormatHint(ValFormat.RelativeTime);
        public static readonly FormatHint Weekday = new FormatHint(ValFormat.Weekday);
        
        public static readonly FormatHint Default = CStyleReal;

        public readonly ValFormat Format;

        public FormatHint(ValFormat f) {
            System.Diagnostics.Debug.Assert(f != null);
            this.Format = f;
        }

        public FormatHint Select(FormatHint b) {
            if (Format.Priority == FormatPriority.Weak && b.Format.Priority == FormatPriority.Strong) {
                // 左辺が標準の整数または実数で右辺が接頭語付きの場合は接頭語を優先する
                return b;
            }
            // それ以外の場合は左辺を優先する
            return this;
        }
    }
}
