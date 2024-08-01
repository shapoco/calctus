using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Parsers;

namespace Shapoco.Calctus.Model.Functions {
    class ArgDef {
        public readonly Token Name;

        public ArgDef(string name) {
            Name = new Token(TokenType.Identifier, TextPosition.Nowhere, name);
        }

        public ArgDef(Token name) {
            Name = name;
        }

        public override string ToString() => Name.Text;
    }
}
