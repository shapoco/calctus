using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Formats;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus.Model.Parsers {
    class Keyword  {
        public readonly string String;
        public readonly string Description;
        public readonly Val LiteralValue;

        private Keyword(string token, string desc, Val literalVal = null) {
            this.String = token;
            this.Description = desc;
            this.LiteralValue = literalVal;
        }

        public bool IsLiteral => LiteralValue != null;

        public static readonly Keyword Def = new Keyword("def", "Start of user function definition");

        public static readonly Keyword True = new Keyword("true", "Value of true", BoolVal.True);
        public static readonly Keyword False = new Keyword("false", "Value of false", BoolVal.False);
        public static readonly Keyword Null = new Keyword("null", "Null value", NullVal.Instance);

        public static IEnumerable<Keyword> EnumKeywords()
            => from p in typeof(Keyword).GetFields()
               where p.IsStatic && p.FieldType == typeof(Keyword)
               select (Keyword)p.GetValue(null);

        private static IReadOnlyDictionary<string, Keyword> generateDictionary() {
            var dict = new Dictionary<string, Keyword>();
            foreach (var k in EnumKeywords()) {
                dict.Add(k.String, k);
            }
            return dict;
        }
        public static IReadOnlyDictionary<string, Keyword> Dictionary = generateDictionary();
    }
}

