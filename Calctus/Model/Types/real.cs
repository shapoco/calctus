using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Types {
    struct real : IComparable<real> {
        public static readonly Regex NumberRegex = new Regex(@"^(-?\d*(\.\d+)?)([eE]([+\-]?\d+))?$");
        public static readonly real MaxValue = (real)decimal.MaxValue;
        public static readonly real MinValue = (real)decimal.MinValue;

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

        public static real Parse(string str) {
            var m = NumberRegex.Match(str);
            if (m.Success) {
                var frac = decimal.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                var exp = 0;
                if (m.Groups[3].Success) {
                    exp = int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
                }
                if (exp >= 0) {
                    return frac * Math.Round((decimal)Math.Pow(10, exp));
                }
                else {
                    return frac / Math.Round((decimal)Math.Pow(10, -exp));
                }
            }
            else {
                throw new Calctus.Model.CalctusError("Invalid number format.");
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
