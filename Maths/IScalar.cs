using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Maths {
    interface IScalar {
        decimal ToDecimal(CastOptions opt, out bool degraded);
        rational ToFrac(CastOptions opt, out bool degraded);
        apfixed ToApFixed(CastOptions opt, out bool degraded);

        int CompareTo(decimal other);
        int CompareTo(rational other);
        int CompareTo(apfixed other);

        bool IsInteger { get; }
    }

    interface IScalar<T> : IScalar, IComparable<T> {

    }

    static class IScalarExtensions {
        public static bool Equals(this IScalar a, decimal b) => a.CompareTo(b) == 0;
        public static bool Equals(this IScalar a, rational b) => a.CompareTo(b) == 0;
        public static bool Equals(this IScalar a, apfixed b) => a.CompareTo(b) == 0;
    }
}
