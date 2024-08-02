using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class DateTimeFuncs {
        public static readonly BuiltInFuncDef now = new BuiltInFuncDef("now()", (e, a) => new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime(), "Current epoch time");

        public static readonly BuiltInFuncDef toDays = new BuiltInFuncDef("toDays(*x)", (e, a) => a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal(), "Converts from epoch time to days.");
        public static readonly BuiltInFuncDef toHours = new BuiltInFuncDef("toHours(*x)", (e, a) => a[0].Div(e, new RealVal(60 * 60)).FormatReal(), "Converts from epoch time to hours.");
        public static readonly BuiltInFuncDef toMinutes = new BuiltInFuncDef("toMinutes(*x)", (e, a) => a[0].Div(e, new RealVal(60)).FormatReal(), "Converts from epoch time to minutes.");
        public static readonly BuiltInFuncDef toSeconds = new BuiltInFuncDef("toSeconds(*x)", (e, a) => a[0].FormatReal(), "Converts from epoch time to seconds.");

        public static readonly BuiltInFuncDef fromDays = new BuiltInFuncDef("fromDays(*x)", (e, a) => a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatDateTime(), "Converts from days to epoch time.");
        public static readonly BuiltInFuncDef fromHours = new BuiltInFuncDef("fromHours(*x)", (e, a) => a[0].Mul(e, new RealVal(60 * 60)).FormatDateTime(), "Converts from hours to epoch time.");
        public static readonly BuiltInFuncDef fromMinutes = new BuiltInFuncDef("fromMinutes(*x)", (e, a) => a[0].Mul(e, new RealVal(60)).FormatDateTime(), "Converts from minutes to epoch time.");
        public static readonly BuiltInFuncDef fromSeconds = new BuiltInFuncDef("fromSeconds(*x)", (e, a) => a[0].FormatDateTime(), "Converts from seconds to epoch time.");
    }
}
