using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.UnitSystem {
    /*
    /// <summary>基本単位</summary>
    class _old_BaseUnit : _old_ScalarUnit {
        private readonly Dim _dim;
        private readonly double _mult;
        private readonly double _div;
        private readonly int _exp;
        private readonly double _offset;
        private readonly UnitVector _dic;

        public override Dim Dimension => _dim;
        public override double Mult => _mult;
        public override double Div => _div;
        public override int Exponent => _exp;
        public override double Offset => _offset;
        public override UnitVector _old_Dictionary => _dic;

        public _old_BaseUnit(Dim dim, double mult = 1, double div = 1, int exp = 1, double offset = 0, UnitSyntax syn = null) : base(syn) {
            this._dim = dim;
            this._mult = mult;
            this._div = div;
            this._exp = exp;
            this._offset = offset;
            this._dic = new UnitVector(this);
        }

        public _old_BaseUnit(Dim dim, double mult, UnitSyntax syn = null) : this(dim, mult, 1, 1, 0, syn) { }

        public _old_BaseUnit(Dim dim, UnitSyntax syn = null) : this(dim, 1, 1, 1, 0, syn) { }

        public override _old_BaseUnit Base => this;

        // 無次元
        public static readonly _old_BaseUnit Dimless = new _old_BaseUnit(Dim.Dimless, double.NaN, double.NaN, 0, 0, UnitSyntax.Dimless);

        // 時間
        public static readonly _old_BaseUnit Second = new _old_BaseUnit(Dim.Time, new UnitSyntax("s"));
        public static readonly _old_BaseUnit Minute = new _old_BaseUnit(Dim.Time, 60, new UnitSyntax("min"));
        public static readonly _old_BaseUnit Hour = new _old_BaseUnit(Dim.Time, 3600, new UnitSyntax("h"));
        public static readonly _old_BaseUnit Day = new _old_BaseUnit(Dim.Time, 86400, new UnitSyntax("day"));
        public static readonly _old_BaseUnit Week = new _old_BaseUnit(Dim.Time, 7 * 86400, new UnitSyntax("week"));
        public static readonly _old_BaseUnit Month = new _old_BaseUnit(Dim.Time, 365.2425 * 86400 / 12, new UnitSyntax("month"));
        public static readonly _old_BaseUnit Year = new _old_BaseUnit(Dim.Time, 365.2425 * 86400, new UnitSyntax("year"));

        // 距離
        public static readonly _old_BaseUnit Meter = new _old_BaseUnit(Dim.Length, new UnitSyntax("m", "kmunp"));

        // 電流
        public static readonly _old_BaseUnit Ampare = new _old_BaseUnit(Dim.Current, new UnitSyntax("a", "GMkmunpf"));

        // 質量
        public static readonly _old_BaseUnit Gram = new _old_BaseUnit(Dim.Mass, new UnitSyntax("g", "kmu"));
        public static readonly _old_BaseUnit Ton = new _old_BaseUnit(Dim.Mass, 1e6, new UnitSyntax("ton", "GMk"));

        // 温度
        public static readonly _old_BaseUnit Kelvin = new _old_BaseUnit(Dim.Temperature, new UnitSyntax("K"));
        public static readonly _old_BaseUnit DegreeCelsius = new _old_BaseUnit(Dim.Temperature, 1, 1, 1, -273.15, new UnitSyntax("degc"));

        public static readonly _old_BaseUnit[] _old_NativeUnits = {
            Second, Minute, Hour, Day, Week, Month, Year,
            Meter,
            Ampare,
            Gram, Ton,
            Kelvin, DegreeCelsius,
        };
    }
    */
}
