using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.Parser {
    class StringMatchReader {
        private readonly string _text;
        private readonly Regex _whiteSpaces = new Regex(@"[\r\n\t 　]+");
        private TextPosition _pos;

        public StringMatchReader(string text, int start = 0) {
            this._text = text;
            this._pos = new TextPosition(start);
        }

        public TextPosition Position => _pos;
        public bool Eos => (_pos.Index >= _text.Length);

        public int Peek() {
            if (Eos) return -1;
            return _text[_pos.Index];
        }

        /// <summary>引数に指定された正規表現の配列のうち最も長くマッチするものを返す</summary>
        public bool Pop(Regex[] regexes, out int best_index, out Match best_match) {
            best_index = -1;
            int best_count = 0;
            best_match = null;
            int best_length = 0;
            for (int i = 0; i < regexes.Length; i++) {
                var r = regexes[i];
                var m = r.Match(_text, _pos.Index);
                if (m.Success && m.Groups[0].Index == _pos.Index) {
                    if (m.Groups[0].Length > best_length) {
                        best_match = m;
                        best_length = m.Groups[0].Length;
                        best_index = i;
                        best_count = 1;
                    }
                    else {
                        best_count += 1;
                    }
                }
            }

            if (best_count >= 2) {
                // todo: 同じ長さで複数の正規表現にマッチする場合をどう扱うか？ (123m は 123メートルか 123分かみたいな話)
                Console.WriteLine("*WARNING: " + best_count + "個の正規表現に同時にマッチします: '" + best_match.Groups[0].Value + "'");
            }

            if (best_match != null) {
                _pos.Count(best_match.Groups[0].Value);
                return true;
            }
            else {
                return false;
            }
        }

        public bool Pop(Regex r, out string token) {
            var m = r.Match(_text, _pos.Index);
            if (m.Success && m.Groups[0].Index == _pos.Index && m.Groups[0].Length > 0) {
                token = m.Groups[0].Value;
                _pos.Count(token);
                return true;
            }
            else {
                token = null;
                return false;
            }
        }

        public void SkipWhite() => Pop(_whiteSpaces, out _);
    }
}
