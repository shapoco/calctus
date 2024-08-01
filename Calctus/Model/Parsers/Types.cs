using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Parsers {
    enum TokenType {
        Literal,
        OperatorSymbol,
        GeneralSymbol,
        Keyword,
        Identifier,
        Eos,
        Empty
    }
}
