using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Parsers {
    class Keyword {
        public readonly string Token;
        public readonly string Description;

        private Keyword(string token, string desc) {
            this.Token = token;
            this.Description = desc;
        }

        public static readonly Keyword Def = new Keyword("def", "Start of user function definition");

        public static readonly Keyword True = new Keyword("true", "Value of true");
        public static readonly Keyword False = new Keyword("false", "Value of false");

        public static readonly string[] WeekdayTokens = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        public static readonly Keyword Sunday = new Keyword(WeekdayTokens[0], "Value of Sunday");
        public static readonly Keyword Monday = new Keyword(WeekdayTokens[1], "Value of Monday");
        public static readonly Keyword Tuesday = new Keyword(WeekdayTokens[2], "Value of Tuesday");
        public static readonly Keyword Wednesday = new Keyword(WeekdayTokens[3], "Value of Wednesday");
        public static readonly Keyword Thursday = new Keyword(WeekdayTokens[4], "Value of Thursday");
        public static readonly Keyword Friday = new Keyword(WeekdayTokens[5], "Value of Friday");
        public static readonly Keyword Saturday = new Keyword(WeekdayTokens[6], "Value of Saturday");

        public static IEnumerable<Keyword> EnumKeywords()
            => from p in typeof(Keyword).GetFields()
               where p.IsStatic && p.FieldType == typeof(Keyword)
               select (Keyword)p.GetValue(null);

        private static IReadOnlyDictionary<string, Keyword> generateDictionary() {
            var dict = new Dictionary<string, Keyword>();
            foreach (var k in EnumKeywords()) {
                dict.Add(k.Token, k);
            }
            return dict;
        }
        public static IReadOnlyDictionary<string, Keyword> Dictionary = generateDictionary();
    }
}

