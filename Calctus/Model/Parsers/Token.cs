﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;

namespace Shapoco.Calctus.Model.Parsers {
    class Token {
        public static readonly Token Empty = new Token(TokenType.Empty, DeprecatedTextPosition.Nowhere, "");

        public static bool IsNullOrEmpty(Token t) => t == null || string.IsNullOrEmpty(t.Text);

        public readonly TokenType Type;
        public readonly DeprecatedTextPosition Position;
        public readonly string Text;

        public Token(TokenType t, DeprecatedTextPosition pos, string text) {
            this.Type = t;
            this.Position = pos;
            this.Text = text;
        }

        public override string ToString() {
            if (Type == TokenType.Eos) {
                return "[EOS]";
            }
            else { 
                return "'" + Text + "'";
            }
        }

        public static Token FromWord(string s) => new Token(TokenType.Identifier, DeprecatedTextPosition.Nowhere, s);
    }
}
