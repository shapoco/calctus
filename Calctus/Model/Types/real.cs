using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Types {
    struct real : IComparable<real> {
        public static readonly Regex Pattern = new Regex(@"^(?<frac>-?([1-9][0-9]*(_[0-9]+)*|0)*(\.[0-9]+(_[0-9]+)*)?)(?<exppart>(?<echar>e|E)(?<exp>[+\-]?[0-9]+(_[0-9]+)*))?$");
        public static readonly real MaxValue = (real)decimal.MaxValue;
        public static readonly real MinValue = (real)decimal.MinValue;
        public static readonly real Zero = (real)0;
        public static readonly real One = (real)1;

        public readonly decimal Raw;

        public real(decimal raw) {
            Raw = raw;
        }

        public override bool Equals(object obj) {
            if (obj is real realObj) {
                return Raw == realObj.Raw;
            }
            else if (obj is frac fracObj) {
                return Raw == (decimal)fracObj;
            }
            else {
                return false;
            }
        }

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

        public static real Parse(string str) {
            Parse(str, out decimal frac, out _, out int exp);
            if (exp >= 0) {
                return frac * Math.Round((decimal)Math.Pow(10, exp));
            }
            else {
                return frac / Math.Round((decimal)Math.Pow(10, -exp));
            }
        }

        public override int GetHashCode() => decimal.GetBits(Raw)[0];
        public override string ToString() => Raw.ToString();
        public string ToString(string format) => Raw.ToString(format);

        public int CompareTo(real other) => Raw.CompareTo(other.Raw);

        public static explicit operator double(real val) => (double)val.Raw;
        public static implicit operator decimal(real val) => val.Raw;
        public static explicit operator long(real val) => (long)val.Raw;
        public static explicit operator int(real val) => (int)val.Raw;

        public static implicit operator real(decimal val) => new real(val);
        public static explicit operator real(double val) => new real((decimal)val);
        public static implicit operator real(long val) => new real(val);
        public static implicit operator real(int val) => new real(val);
        public static implicit operator real(frac val) => new real((decimal)val);

        public static real operator -(real val) => new real(-val.Raw);
        public static real operator +(real a, real b) => new real(a.Raw + b.Raw);
        public static real operator -(real a, real b) => new real(a.Raw - b.Raw);
        public static real operator *(real a, real b) => new real(a.Raw * b.Raw);
        public static real operator /(real a, real b) => new real(a.Raw / b.Raw);
        public static real operator %(real a, real b) => new real(a.Raw % b.Raw);

        public static bool operator ==(real a, real b) => a.Raw == b.Raw;
        public static bool operator !=(real a, real b) => a.Raw != b.Raw;
        public static bool operator <(real a, real b) => a.Raw < b.Raw;
        public static bool operator >(real a, real b) => a.Raw > b.Raw;
        public static bool operator <=(real a, real b) => a.Raw <= b.Raw;
        public static bool operator >=(real a, real b) => a.Raw >= b.Raw;

    }
}
