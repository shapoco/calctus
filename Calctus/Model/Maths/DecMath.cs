using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Shapoco.Calctus.Model.Maths {
    static class DecMath {
        public static readonly Regex Pattern = new Regex(@"^(?<frac>-?([1-9][0-9]*(_[0-9]+)*|0)*(\.[0-9]+(_[0-9]+)*)?)(?<exppart>(?<echar>e|E)(?<exp>[+\-]?[0-9]+(_[0-9]+)*))?$");
        public const decimal PI = 3.1415926535897932384626433833m;
        public const decimal E = 2.7182818284590452353602874714m;
        public const decimal LogE2 = 0.6931471805599453094172321215m;
        public const decimal LogE10 = 2.3025850929940456840179914547m;

        public static bool TryParse(string str, out decimal frac, out char eChar, out int exp) {
            frac = 0;
            eChar = '\0';
            exp = 0;

            var m = Pattern.Match(str);
            if (!m.Success) {
                return false;
            }
            frac = decimal.Parse(m.Groups["frac"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            if (m.Groups["exppart"].Success) {
                eChar = m.Groups["echar"].Value[0];
                exp = int.Parse(m.Groups["exp"].Value.Replace("_", ""), CultureInfo.InvariantCulture);
            }
            return true;
        }

        public static void Parse(string str, out decimal frac, out char eChar, out int exp) {
            if (!TryParse(str, out frac, out eChar, out exp)) {
                throw new CalctusError("Invalid number format.");
            }
        }

        public static decimal Parse(string str) {
            Parse(str, out decimal frac, out _, out int exp);
            if (exp >= 0) {
                return frac * Math.Round((decimal)Math.Pow(10, exp));
            }
            else {
                return frac / Math.Round((decimal)Math.Pow(10, -exp));
            }
        }
    }

}
