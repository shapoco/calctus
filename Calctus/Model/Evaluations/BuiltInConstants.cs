using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Maths;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Parsers;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Graphs;
using Shapoco.Calctus.Model.Functions;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Evaluations {
    static class BuiltInConstants {
        private static Var constVar(string name, Val val, string desc)
            => new Var(new Token(TokenType.Identifier, DeprecatedTextPosition.Nowhere, name), val, true, desc);

        private static Var constVarReal(string name, decimal value, string desc)
            => constVar(name, value.ToVal(), desc);

        private static Var constVarHex(string name, decimal value, string desc)
            => constVar(name, value.ToHexVal(), desc);

        private static Var constVarDayOfWeek(DayOfWeek dayOfWeek)
            => constVar(dayOfWeek.ToString(), dayOfWeek.ToVal(), "Value of " + dayOfWeek.ToString());

        public static readonly Var PI = constVarReal("PI", DecMath.PI, "Circle ratio");
        public static readonly Var E = constVarReal("E", DecMath.E, "Base of natural logarithm");
        public static readonly Var IntMin = constVarHex("INT_MIN", Int32.MinValue, "Minimum value of 32 bit signed integer");
        public static readonly Var IntMax = constVarHex("INT_MAX", Int32.MaxValue, "Maximum value of 32 bit signed integer");
        public static readonly Var UIntMin = constVarHex("UINT_MIN", UInt32.MinValue, "Minimum value of 32 bit unsigned integer");
        public static readonly Var UIntMax = constVarHex("UINT_MAX", UInt32.MaxValue, "Maximum value of 32 bit unsigned integer");
        public static readonly Var LongMin = constVarHex("LONG_MIN", Int64.MinValue, "Minimum value of 64 bit signed integer");
        public static readonly Var LongMax = constVarHex("LONG_MAX", Int64.MaxValue, "Maximum value of 64 bit signed integer");
        public static readonly Var ULongMin = constVarHex("ULONG_MIN", UInt64.MinValue, "Minimum value of 64 bit unsigned integer");
        public static readonly Var ULongMax = constVarHex("ULONG_MAX", UInt64.MaxValue, "Maximum value of 64 bit unsigned integer");
        public static readonly Var DecimalMin = constVarReal("DECIMAL_MIN", decimal.MinValue, "Minimum value of Decimal");
        public static readonly Var DecimalMax = constVarReal("DECIMAL_MAX", decimal.MaxValue, "Maximum value of Decimal");

        // todo DayOfWeek: 列挙型を新設する？
        public static readonly Var Sunday = constVarDayOfWeek(DayOfWeek.Sunday);
        public static readonly Var Monday = constVarDayOfWeek(DayOfWeek.Monday);
        public static readonly Var Tuesday = constVarDayOfWeek(DayOfWeek.Tuesday);
        public static readonly Var Wednesday = constVarDayOfWeek(DayOfWeek.Wednesday);
        public static readonly Var Thursday = constVarDayOfWeek(DayOfWeek.Thursday);
        public static readonly Var Friday = constVarDayOfWeek(DayOfWeek.Friday);
        public static readonly Var Saturday = constVarDayOfWeek(DayOfWeek.Saturday);
        //public static readonly Var DayOfWeekConst = constVar("DayOfWeek", DateTimeVal.DayOfWeekList, "List of day of week values");

        public static IEnumerable<Var> EnumConstants()
            => from p in typeof(BuiltInConstants).GetFields()
               where p.IsStatic && p.FieldType == typeof(Var)
               select (Var)p.GetValue(null);
    }
}
