using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Mathematics;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class ESeriesFuncs {
        public static readonly BuiltInFuncDef esFloor = new BuiltInFuncDef("esFloor(series,*x)", (e, a) => {
            return new RealVal(PreferredNumbers.Floor(ESeries.GetSeries(a[0].AsInt), a[1].AsReal), a[1].FormatHint);
        }, "Nearest E-series value less than or equal to `x` (series=3, 6, 12, 24, 48, 96, or 192).");

        public static readonly BuiltInFuncDef esCeil = new BuiltInFuncDef("esCeil(series,*x)", (e, a) => {
            return new RealVal(PreferredNumbers.Ceiling(ESeries.GetSeries(a[0].AsInt), a[1].AsReal), a[1].FormatHint);
        }, "Nearest E-series value greater than or equal to `x` (series=3, 6, 12, 24, 48, 96, or 192).");

        public static readonly BuiltInFuncDef esRound = new BuiltInFuncDef("esRound(series,*x)", (e, a) => {
            return new RealVal(PreferredNumbers.Round(ESeries.GetSeries(a[0].AsInt), a[1].AsReal), a[1].FormatHint);
        }, "Nearest E-series value (series=3, 6, 12, 24, 48, 96, or 192).");

        public static readonly BuiltInFuncDef esRatio = new BuiltInFuncDef("esRatio(series,*x)", (e, a) => {
            return new ArrayVal(PreferredNumbers.FindSplitPair(ESeries.GetSeries(a[0].AsInt), a[1].AsReal));
        }, "Two E-series resistor values that provide the closest value to the voltage divider ratio `x` (series=3, 6, 12, 24, 48, 96, or 192).");

        /*
        public static readonly EmbeddedFuncDef e3Floor = new EmbeddedFuncDef("e3Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series floor");
        public static readonly EmbeddedFuncDef e3Ceil = new EmbeddedFuncDef("e3Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series ceiling");
        public static readonly EmbeddedFuncDef e3Round = new EmbeddedFuncDef("e3Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E3, a[0].AsReal), a[0].FormatHint), "E3 series round");
        public static readonly EmbeddedFuncDef e3Ratio = new EmbeddedFuncDef("e3Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E3, a[0].AsReal)), "E3 series value of divider resistor");

        public static readonly EmbeddedFuncDef e6Floor = new EmbeddedFuncDef("e6Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series floor");
        public static readonly EmbeddedFuncDef e6Ceil = new EmbeddedFuncDef("e6Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series ceiling");
        public static readonly EmbeddedFuncDef e6Round = new EmbeddedFuncDef("e6Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E6, a[0].AsReal), a[0].FormatHint), "E6 series round");
        public static readonly EmbeddedFuncDef e6Ratio = new EmbeddedFuncDef("e6Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E6, a[0].AsReal)), "E6 series value of divider resistor");

        public static readonly EmbeddedFuncDef e12Floor = new EmbeddedFuncDef("e12Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series floor");
        public static readonly EmbeddedFuncDef e12Ceil = new EmbeddedFuncDef("e12Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series ceiling");
        public static readonly EmbeddedFuncDef e12Round = new EmbeddedFuncDef("e12Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E12, a[0].AsReal), a[0].FormatHint), "E12 series round");
        public static readonly EmbeddedFuncDef e12Ratio = new EmbeddedFuncDef("e12Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E12, a[0].AsReal)), "E12 series value of divider resistor");

        public static readonly EmbeddedFuncDef e24Floor = new EmbeddedFuncDef("e24Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series floor");
        public static readonly EmbeddedFuncDef e24Ceil = new EmbeddedFuncDef("e24Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series ceiling");
        public static readonly EmbeddedFuncDef e24Round = new EmbeddedFuncDef("e24Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E24, a[0].AsReal), a[0].FormatHint), "E24 series round");
        public static readonly EmbeddedFuncDef e24Ratio = new EmbeddedFuncDef("e24Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E24, a[0].AsReal)), "E24 series value of divider resistor");

        public static readonly EmbeddedFuncDef e48Floor = new EmbeddedFuncDef("e48Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series floor");
        public static readonly EmbeddedFuncDef e48Ceil = new EmbeddedFuncDef("e48Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series ceiling");
        public static readonly EmbeddedFuncDef e48Round = new EmbeddedFuncDef("e48Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E48, a[0].AsReal), a[0].FormatHint), "E48 series round");
        public static readonly EmbeddedFuncDef e48Ratio = new EmbeddedFuncDef("e48Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E48, a[0].AsReal)), "E48 series value of divider resistor");

        public static readonly EmbeddedFuncDef e96Floor = new EmbeddedFuncDef("e96Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series floor");
        public static readonly EmbeddedFuncDef e96Ceil = new EmbeddedFuncDef("e96Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series ceiling");
        public static readonly EmbeddedFuncDef e96Round = new EmbeddedFuncDef("e96Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E96, a[0].AsReal), a[0].FormatHint), "E96 series round");
        public static readonly EmbeddedFuncDef e96Ratio = new EmbeddedFuncDef("e96Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E96, a[0].AsReal)), "E96 series value of divider resistor");

        public static readonly EmbeddedFuncDef e192Floor = new EmbeddedFuncDef("e192Floor(*x)", (e, a) => new RealVal(PreferredNumbers.Floor(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series floor");
        public static readonly EmbeddedFuncDef e192Ceil = new EmbeddedFuncDef("e192Ceil(*x)", (e, a) => new RealVal(PreferredNumbers.Ceiling(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series ceiling");
        public static readonly EmbeddedFuncDef e192Round = new EmbeddedFuncDef("e192Round(*x)", (e, a) => new RealVal(PreferredNumbers.Round(Eseries.E192, a[0].AsReal), a[0].FormatHint), "E192 series round");
        public static readonly EmbeddedFuncDef e192Ratio = new EmbeddedFuncDef("e192Ratio(*x)", (e, a) => new ArrayVal(PreferredNumbers.FindSplitPair(Eseries.E192, a[0].AsReal)), "E192 series value of divider resistor");
        */
    }
}
