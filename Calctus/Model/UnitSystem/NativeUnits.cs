using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    static class NativeUnits {

        // 無次元
        public static readonly DerivedUnit Dimless = new DerivedUnit();

        // 時間
        public static readonly BaseUnit Second = new BaseUnit(Dim.Time, new UnitSyntax("s"));
        public static readonly ScaledUnit Minute = new ScaledUnit(Second, 60, new UnitSyntax("min"));
        public static readonly ScaledUnit Hour = new ScaledUnit(Second, 3600, new UnitSyntax("h"));
        public static readonly ScaledUnit Day = new ScaledUnit(Second, 86400, new UnitSyntax("day"));
        public static readonly ScaledUnit Week = new ScaledUnit(Second, 7 * 86400, new UnitSyntax("week"));
        public static readonly ScaledUnit Month = new ScaledUnit(Second, 365.2425m * 86400 / 12, new UnitSyntax("month"));
        public static readonly ScaledUnit Year = new ScaledUnit(Second, 365.2425m * 86400, new UnitSyntax("year"));

        // 距離
        public static readonly BaseUnit Meter = new BaseUnit(Dim.Length, new UnitSyntax("m", "kmunp"));

        // 電流
        public static readonly BaseUnit Ampare = new BaseUnit(Dim.Current, new UnitSyntax("a", "GMkmunpf"));

        // 質量
        public static readonly BaseUnit Gram = new BaseUnit(Dim.Mass, new UnitSyntax("g", "kmu"));
        public static readonly ScaledUnit Ton = new ScaledUnit(Gram, 1e6m, new UnitSyntax("ton", "GMk"));

        // 温度
        public static readonly BaseUnit Kelvin = new BaseUnit(Dim.Temperature, new UnitSyntax("K"));
        public static readonly ScaledUnit DegreeCelsius = new ScaledUnit(Kelvin, 1, new UnitSyntax("degC"));
        public static readonly ScaledUnit DegreeFahrenheit = new ScaledUnit(Kelvin, 1, 1.8m, new UnitSyntax("degF"));

        public static readonly Unit[] List = {
            Second, Minute, Hour, Day, Week, Month, Year,
            Meter,
            Ampare,
            Gram, Ton,
            Kelvin, DegreeCelsius, DegreeFahrenheit,
        };
    }
}
