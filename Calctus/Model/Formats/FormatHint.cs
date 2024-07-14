﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    /// <summary>Val の表現に関するヒントを格納するクラス</summary>
    class FormatHint {
        public static readonly FormatHint Default = new FormatHint(ValFormat.CStyleReal);

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
