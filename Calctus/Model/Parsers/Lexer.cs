using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Shapoco.Calctus.Model.Expressions;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Parsers {
    class Lexer {
        //public const string IdPattern = @"[\p{L}_][\p{L}\p{N}_]*";
        //private static readonly Regex _wordRegexes = new Regex(IdPattern);

        // 演算子以外の記号
        //private static readonly Regex GeneralSymbolRule = new Regex(@"[()\[\],:?]");

        // キーワード
        //private static readonly Regex KeywordRule
        //    = new Regex(@"(" + String.Join("|", Keyword.EnumKeywords().Select(p => p.Token)) + @")\b");

        private StringReader _sr;
        private bool _eosReaded = false;

        // 長い順に並べた演算子記号
        //private readonly Regex OpSymbolRule;

        // 数値リテラル
        //private readonly ValFormat[] _numberFormatters;
        //private readonly Regex[] _literalRegexes;


        public Lexer(string exprStr, int pos = 0) {
            //// 全ての記号にマッチする正規表現の生成
            //var opSymbols = OpDef.AllOperatorSymbols
            //    .OrderByDescending(p => p.Length)
            //    .Select(p => "(" + Regex.Escape(p) + ")")
            //    .ToArray();
            //OpSymbolRule = new Regex("(" + string.Join("|", opSymbols) + ")");
            //
            //_numberFormatters = ValFormat.NativeFormats;
            //_literalRegexes = _numberFormatters.Select(p => p.Pattern).ToArray();

            _sr = new StringReader(exprStr, pos);

        }

        public TextPosition Position => _sr.Position;
        public bool Eos {
            get {
                _sr.SkipWhite();
                return _sr.Eos;
            }
        }

        /// <summary>トークンを1つ読み出す</summary>
        public Token Pop() {
            _sr.SkipWhite();

            if (Eos) {
                if (!_eosReaded) {
                    _eosReaded = true;
                    return new Token(TokenType.Eos, Position, null);
                }
                else {
                    throw new LexerError(Position, "Internal error: Parser overrun");
                }
            }

            _sr.Init();
            Token tok;
            if (readIfNumericLiteral(out tok)) {
                return tok;
            }
            else if (readIfIdOrKeyword(out tok)) {
                return tok;
            }
            else if (SymbolLexer.Instance.TryRead(_sr, out tok)) {
                return tok;
            }
            else if (readIfStringLiteral(out tok)) {
                return tok;
            }
            else if (readIfCharLiteral(out tok)) {
                return tok;
            }
            else if (readIfDateTimeOrTimeSpanOrWebColor(out tok)) {
                return tok;
            }

            throw new LexerError(Position, "Unknown token starts with " + CalctusUtils.ToString((char)_sr.Peek()));
        }

        private bool readIfNumericLiteral(out Token tok) {
            var pos = _sr.Position;
            if (NumberLexer.TryParseChar(_sr, Radix.Decimal, out char first)) {
                if (first == '0' && (_sr.ReadIf('x'))) {
                    tok = hexBinOctLiteralFollowing(Radix.Hexadecimal);
                    return true;
                }
                else if (first == '0' && (_sr.ReadIf('b'))) {
                    tok = hexBinOctLiteralFollowing(Radix.Binary);
                    return true;
                }
                else if (first == '0' && (_sr.ReadIf('o'))) {
                    tok = hexBinOctLiteralFollowing(Radix.Octal);
                    return true;
                }
                else {
                    var num = new NumberSequence(Radix.Decimal, pos);
                    num.Append(first);
                    tok = decLiteralFollowing(num);
                    return true;
                }
            }
            else {
                tok = null;
                return false;
            }
        }

        private Token decLiteralFollowing(NumberSequence integBuf) {
            decimal val = 0;

            NumberLexer.ReadFollowing(_sr, integBuf, true);
            val = integBuf.ToDecimal();

            if (_sr.ReadIf('.')) {
                var next = _sr.Peek();
                if (next < '0' || '9' < next) {
                    // .. と ..= のために巻き戻す
                    _sr.Trackback();
                    return _sr.FinishToken(TokenType.Literal, new RealVal(val));
                }
                val += NumberLexer.Expect(_sr, Radix.Decimal, true).ToFraction();
            }

            var postfixPos = _sr.Position;
            if (_sr.ReadIf('e')) {
                int sign = 1;
                if (_sr.ReadIf('-')) sign = -1;
                var exp = sign * NumberLexer.Expect(_sr, Radix.Decimal, true).ToInt();
                val *= MathEx.Pow10(exp);
                var postfixLen = _sr.Position.Index - postfixPos.Index;
                return _sr.FinishToken(TokenType.Literal, new RealVal(val), postfixLen);
            }
            else if (readIfId(out string postfix)) {
                if (postfix.Length == 1 && SiPrefix.TryCharToExp(postfix[0], out int siExp)) {
                    val *= MathEx.Pow10(siExp * 3);
                    return _sr.FinishToken(TokenType.Literal, new RealVal(val, FormatFlags.SiPrefixed), postfix.Length);
                }
                else if (postfix.Length == 2 && BinaryPrefix.TryCharToExp(postfix[0], out int kibiExp) && postfix[1] == 'i') {
                    if (kibiExp >= 0) {
                        val *= (1L << (kibiExp * 10));
                    }
                    else {
                        val /= (1L << (-kibiExp * 10));
                    }
                    return _sr.FinishToken(TokenType.Literal, new RealVal(val, FormatFlags.BinaryPrefixed), postfix.Length);
                }
                else {
                    throw new LexerError(postfixPos, _sr.Position.Index - postfixPos.Index, "Invalid postfix: " + CalctusUtils.ToString(postfix));
                }
            }
            else {
                return _sr.FinishToken(TokenType.Literal, new RealVal(val));
            }
        }

        // 0x、0b、0o の続き
        private Token hexBinOctLiteralFollowing(Radix radix) {
            var dec = NumberLexer.Expect(_sr, radix, true).ToDecimal(radix + " value", 0, ulong.MaxValue);
            FormatFlags fmt;
            switch (radix) {
                case Radix.Hexadecimal: fmt = FormatFlags.Hexadecimal; break;
                case Radix.Binary: fmt = FormatFlags.Binary; break;
                case Radix.Octal: fmt = FormatFlags.Octal; break;
                default: throw new NotImplementedException("Bad radix: " + radix);
            }
            if (dec > long.MaxValue) dec -= ((decimal)0x100000000) * ((decimal)0x100000000);
            return _sr.FinishToken(TokenType.Literal, new RealVal(dec, fmt));
        }

        // 識別子またはキーワード
        private bool readIfIdOrKeyword(out Token tok) {
            if (readIfId(out string id)) {
                if (Keyword.Dictionary.TryGetValue(id, out Keyword keyword)) {
                    if (keyword.IsLiteral) {
                        tok = _sr.FinishToken(TokenType.Literal, keyword.LiteralValue);
                    }
                    else {
                        tok = _sr.FinishToken(TokenType.Keyword);
                    }
                }
                else {
                    tok = _sr.FinishToken(TokenType.Identifier);
                }
                return true;
            }
            else {
                tok = null;
                return false;
            }
        }

        // 識別子の読み出し
        private bool readIfId(out string id) {
            if (readIfStartOfId(out char c)) {
                var idBuf = new StringBuilder();
                idBuf.Append((char)c);
                readIdFollowing(idBuf);
                id = idBuf.ToString();
                return true;
            }
            else {
                id = null;
                return false;
            }
        }

        // 識別子の先頭
        private bool readIfStartOfId(out char c) {
            if (_sr.ReadIf('a', 'z', out c) || _sr.ReadIf('A', 'Z', out c)) {
                return true;
            }
            else if (_sr.ReadIf('_')) {
                c = '_';
                return true;
            }
            else {
                return false;
            }
        }

        // 識別子の2文字目以降
        private void readIdFollowing(StringBuilder idBuf) {
            char c;
            while (readIfStartOfId(out c) || NumberLexer.TryParseChar(_sr, Radix.Decimal, out c)) {
                idBuf.Append(c);
            }
        }

        // 文字列リテラル
        private bool readIfStringLiteral(out Token token) {
            if (_sr.ReadIf('"')) {
                var strVal = new StringBuilder();
                while (!_sr.ReadIf('"')) {
                    strVal.Append(expectStringChar());
                }
                token = _sr.FinishToken(TokenType.Literal, new StrVal(strVal.ToString()));
                return true;
            }
            else {
                token = null;
                return false;
            }
        }

        // 文字リテラル
        private bool readIfCharLiteral(out Token token) {
            if (_sr.ReadIf('\'')) {
                char c = expectStringChar();
                _sr.Expect('\'');
                token = _sr.FinishToken(TokenType.Literal, new RealVal(c, FormatFlags.Character));
                return true;
            }
            else {
                token = null;
                return false;
            }
        }

        // 文字列リテラルまたは文字リテラルの中身
        private char expectStringChar() {
            if (_sr.ReadIf('\\')) {
                int ci = _sr.Read();
                char c = (char)ci;
                NumberSequence code;
                switch (ci) {
                    case 'a': return '\a';
                    case 'b': return '\b';
                    case 'f': return '\f';
                    case 'n': return '\n';
                    case 'r': return '\r';
                    case 't': return '\t';
                    case 'v': return '\v';
                    case '\\': return '\\';
                    case '\'': return '\'';
                    case '\"': return '"';
                    case '0': return '\0';
                    case 'x':
                        code = new NumberSequence(Radix.Hexadecimal, _sr.Position);
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        return (char)code.ToInt();
                    case 'u':
                        code = new NumberSequence(Radix.Hexadecimal, _sr.Position);
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        code.Append(NumberLexer.ExpectChar(_sr, Radix.Hexadecimal));
                        return (char)code.ToInt();
                    default:
                        throw new LexerError(_sr.TokenPosition, "Unrecognized escaped char: \\" + CalctusUtils.ToString(c));
                }
            }
            else {
                return (char)_sr.Read();
            }
        }

        // 日付時刻/時間差リテラル
        private bool readIfDateTimeOrTimeSpanOrWebColor(out Token token) {
            if (_sr.ReadIf('#')) {
                if (readIfSign(out int sign)) {
                    token = readTimeSpanFollowing(sign);
                }
                else {
                    token = readDateTimeFollowingOrWebColor();
                }
                return true;
            }
            else {
                token = null;
                return false;
            }
        }

        // todo 日本語対応 readDateTimeFollowing()
        // 日付リテラルの続き
        private Token readDateTimeFollowingOrWebColor() {
            const int NOW = -1;
            var num0 = NumberLexer.Expect(_sr, Radix.Hexadecimal, false);
            int y = NOW, m = NOW, d = NOW;
            decimal t = NOW;
            if (_sr.ReadIf('/')) {
                num0.ChangeRadix(Radix.Decimal);
                var num1 = NumberLexer.Expect(_sr, Radix.Decimal, false);
                if (_sr.ReadIf('/')) {
                    var num2 = NumberLexer.Expect(_sr, Radix.Decimal, false);
                    if (num0.Length == 4) {
                        y = num0.ToInt("Year");
                        m = num1.ToInt("Month", 1, 12);
                        d = num2.ToInt("Day", 1, 31);
                    }
                    else {
                        m = num0.ToInt("Month", 1, 12);
                        d = num1.ToInt("Day", 1, 31);
                        y = num2.ToInt("Year");
                    }
                }
                else {
                    m = num0.ToInt("Month", 1, 12);
                    d = num1.ToInt("Day", 1, 31);
                }
                if (_sr.ReadIf(' ') || _sr.ReadIf('T')) {
                    t = expectedTime();
                }
            }
            else if (_sr.ReadIf(':')) {
                num0.ChangeRadix(Radix.Decimal);
                t = 3600 + num0.ToInt("Hour", 0, 23);
                t += readTimeFollowing(false);
            }
            else if (num0.Length == 3 || num0.Length == 6) {
                var col = (long)num0.ToDecimal("Color code");
                if (num0.Length == 3) {
                    col =
                        ((col & 0xf00) << 12) | ((col & 0xf00) << 8) |
                        ((col & 0xf0) << 8) | ((col & 0xf0) << 4) |
                        ((col & 0xf) << 4) | (col & 0xf);
                }
                return _sr.FinishToken(TokenType.Literal, new RealVal(col, FormatFlags.WebColor));
            }
            else {
                throw _sr.InvalidToken();
            }

            if (readIfSign(out int sign)) {
                // todo readDateTimeFollowing() タイムゾーン対応
                throw new NotImplementedException();
            }
            _sr.Expect('#');

            var today = DateTime.Today;
            if (y == NOW) y = today.Year;
            if (m == NOW) m = today.Month;
            if (d == NOW) d = today.Day;
            var unixTime = UnixTime.FromLocalTime(y, m, d);

            if (t == NOW) {
                unixTime += (decimal)(DateTime.Now - today).TotalSeconds;
            }
            else {
                unixTime += t;
            }

            return _sr.FinishToken(TokenType.Literal, new RealVal(unixTime, FormatFlags.DateTime));
        }

        private decimal expectedTime() {
            decimal t = 3600 * NumberLexer.Expect(_sr, Radix.Decimal, false).ToInt("Hour", 0, 23);
            _sr.Expect(':');
            t += readTimeFollowing(false);
            return t;
        }

        // todo 日本語対応 readTimeSpanFollowing()
        // 時間差リテラルの続き
        private Token readTimeSpanFollowing(int sign) {
            var num0 = NumberLexer.Expect(_sr, Radix.Decimal, false);
            decimal t = 0;
            if (_sr.ReadIf('.')) {
                t += 86400 * num0.ToInt("Day"); 
                var num1 = NumberLexer.Expect(_sr, Radix.Decimal, false);
                if (_sr.ReadIf(':')) {
                    t += 3600 * num1.ToInt("Hour", 0, 23);
                    t += readTimeFollowing(true);
                }
                else {
                    t += 86400 * num1.ToFraction();
                }
            }
            else if (_sr.ReadIf(':')) {
                t += 3600 * num0.ToInt("Hour", 0, 23);
                t += readTimeFollowing(true);
            }
            else {
                t += 86400 * num0.ToInt("Day");
            }
            _sr.Expect('#');

            t *= sign;
            return _sr.FinishToken(TokenType.Literal, new RealVal(t, FormatFlags.TimeSpan));
        }

        // 分、秒
        private decimal readTimeFollowing(bool timeSpan) {
            decimal t = 60 * NumberLexer.Expect(_sr, Radix.Decimal, false).ToInt("Minute", 0, 59);
            if (_sr.ReadIf(':')) {
                t += NumberLexer.Expect(_sr, Radix.Decimal, false).ToInt("Second", 0, 59);
                if (_sr.ReadIf('.')) {
                    t += NumberLexer.Expect(_sr, Radix.Decimal, false).ToFraction();
                }
            }
            return t;
        }

        private bool readIfSign(out int sign) {
            sign = 1;
            if (_sr.ReadIf('+')) {
                return true;
            }
            else if (_sr.ReadIf('-')) {
                sign = -1;
                return true;
            }
            return false;
        }

        /// <summary>文字列の末端まで全てのトークンを読み出す</summary>
        public TokenQueue PopToEnd() {
            var queue = new TokenQueue();
            while (!_eosReaded) {
                queue.PushBack(Pop());
            }
            return queue;
        }

        /// <summary>
        /// 文字列が演算子のみで構成されていればそれを返す
        /// </summary>
        public static bool TryGetRpnSymbols(string expr, out Token[] symbols) {
            try {
                var lexer = new Lexer(expr);
                var list = new List<Token>();
                while (!lexer.Eos) {
                    var t = lexer.Pop();
                    if (t.Type != TokenType.OperatorSymbol) {
                        symbols = null;
                        return false;
                    }
                    list.Add(t);
                }
                symbols = list.ToArray();
                return symbols.Length > 0;
            }
            catch {
                symbols = null;
                return false;
            }
        }

        public static bool IsFirstIdChar(char c) => char.IsLetter(c) || c == '_';
        public static bool IsFollowingIdChar(char c) => char.IsLetter(c) || char.IsDigit(c) || c == '_';

        public static void Test(string exprStr) {
            Lexer lex = new Lexer(exprStr);
            while (!lex.Eos) {
                Console.Write("'" + lex.Pop().Text + "'\t");
            }
            Console.WriteLine();
        }

    }
}
