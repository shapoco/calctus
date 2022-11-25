using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model {
    struct real {
        public static readonly Regex NumberRegex = new Regex(@"^(-?\d+(\.\d+)?)([eE]([+\-]?\d+))?$");
        public static readonly real MaxValue = (real)decimal.MaxValue;
        public static readonly real MinValue = (real)decimal.MinValue;

        private decimal _raw;
        public decimal Raw => _raw;

        public real(decimal raw) {
            _raw = raw;
        }

        public override bool Equals(object obj) {
            if (obj is real obj1) {
                return _raw == obj1._raw;
            }
            else {
                return false;
            }
        }

        public static real Parse(string str) {
            var m = NumberRegex.Match(str);
            if (m.Success) {
                var frac = decimal.Parse(m.Groups[1].Value);
                var exp = 0;
                if (m.Groups[3].Success) {
                    exp = int.Parse(m.Groups[4].Value);
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

        public override int GetHashCode() => decimal.GetBits(_raw)[0];
        public override string ToString() => _raw.ToString();
        public string ToString(string format) => _raw.ToString(format);

        public static explicit operator double(real val) => (double)val._raw;
        public static implicit operator decimal(real val) => val._raw;
        public static explicit operator long(real val) => (long)val._raw;
        public static explicit operator int(real val) => (int)val._raw;

        public static implicit operator real(decimal val) => new real(val);
        public static implicit operator real(double val) => new real((decimal)val);
        public static implicit operator real(long val) => new real(val);
        public static implicit operator real(int val) => new real(val);

        public static real operator -(real val) => new real(-val._raw);
        public static real operator +(real a, real b) => new real(a._raw + b._raw);
        public static real operator -(real a, real b) => new real(a._raw - b._raw);
        public static real operator *(real a, real b) => new real(a._raw * b._raw);
        public static real operator /(real a, real b) => new real(a._raw / b._raw);
        public static real operator %(real a, real b) => new real(a._raw % b._raw);

        public static bool operator ==(real a, real b) => a._raw == b._raw;
        public static bool operator !=(real a, real b) => a._raw != b._raw;
        public static bool operator <(real a, real b) => a._raw < b._raw;
        public static bool operator >(real a, real b) => a._raw > b._raw;
        public static bool operator <=(real a, real b) => a._raw <= b._raw;
        public static bool operator >=(real a, real b) => a._raw >= b._raw;

    }
}
