using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    class UnitSyntax {
        public static readonly UnitSyntax Unnamed = new UnitSyntax("");
        public static readonly UnitSyntax Dimless = new UnitSyntax("");

        public readonly string Symbol;
        public readonly Regex SymbolPattern;
        public readonly char[] Prefixes;
        public readonly string Description;

        public UnitSyntax(string symbol, Regex pattern, string prefixes = "", string desc = "") {
            this.Symbol = symbol;
            this.SymbolPattern = pattern;
            this.Prefixes = prefixes.ToArray();
            this.Description = desc;
        }

        public UnitSyntax(string unit, string prefixes = "", string description = "")
            : this(unit, new Regex(Regex.Escape(unit)), prefixes, description) { }
    }
}
