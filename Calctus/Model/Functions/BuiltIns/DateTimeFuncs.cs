using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Types;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Mathematics;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    static class DateTimeFuncs {
        public static readonly BuiltInFuncDef datetime_3 = new BuiltInFuncDef("datetime(year, mon, day)", (e, a) => {
            return new RealVal(UnixTime.FromLocalTime(a[0].AsInt, a[1].AsInt, a[2].AsInt, 0, 0, 0)).FormatDateTime();
        }, "Returns datetime from year, month, and day.");

        public static readonly BuiltInFuncDef datetime_6 = new BuiltInFuncDef("datetime(year, mon, day, hour, min, sec)", (e, a) => {
            return new RealVal(UnixTime.FromLocalTime(a[0].AsInt, a[1].AsInt, a[2].AsInt, a[3].AsInt, a[4].AsInt, a[5].AsReal)).FormatDateTime();
        }, "Returns datetime from year, month, day, hour, minute, and second.");

        public static readonly BuiltInFuncDef yearOf = new BuiltInFuncDef("yearOf(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).Year);
        }, "Returns year component of datetime.");

        public static readonly BuiltInFuncDef monthOf = new BuiltInFuncDef("monthOf(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).Month);
        }, "Returns month component of datetime, expressed as 1..12.");

        public static readonly BuiltInFuncDef dayOfYear = new BuiltInFuncDef("dayOfYear(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).DayOfYear);
        }, "Returns day of year of datetime, expressed as 1..366.");

        public static readonly BuiltInFuncDef dayOfWeek = new BuiltInFuncDef("dayOfWeek(*t)", (e, a) => {
            return new RealVal(((int)UnixTime.ToLocalTime(a[0].AsReal).DayOfWeek)).FormatDayOfWeek();
        }, "Returns day of week of datetime, expressed as 0 (SUNDAY)..6 (SATURDAY).");

        public static readonly BuiltInFuncDef dayOfMonth = new BuiltInFuncDef("dayOfMonth(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).Day);
        }, "Returns day component of datetime, expressed as 1..31.");

        public static readonly BuiltInFuncDef hourOf = new BuiltInFuncDef("hourOf(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).Hour);
        }, "Returns hour component of datetime, expressed as 0..23.");

        public static readonly BuiltInFuncDef minuteOf = new BuiltInFuncDef("minuteOf(*t)", (e, a) => {
            return new RealVal(UnixTime.ToLocalTime(a[0].AsReal).Minute);
        }, "Returns minute component of datetime, expressed as 0..59.");

        public static readonly BuiltInFuncDef secondOf = new BuiltInFuncDef("secondOf(*t)", (e, a) => {
            var unixTime = a[0].AsReal;
            var subSec = unixTime - RMath.Floor(unixTime);
            return new RealVal(UnixTime.ToLocalTime(unixTime).Second + subSec);
        }, "Returns second component of datetime, expressed as 0..60.");

        public static readonly BuiltInFuncDef now = new BuiltInFuncDef("now()", (e, a) => {
            return new RealVal(UnixTime.FromLocalTime(DateTime.Now)).FormatDateTime();
        }, "Current epoch time.");

        public static readonly BuiltInFuncDef today = new BuiltInFuncDef("today()", (e, a) => {
            return new RealVal(UnixTime.FromLocalTime(DateTime.Today)).FormatDateTime();
        }, "Returns datetime of today's 00:00:00.");

        public static readonly BuiltInFuncDef toDays = new BuiltInFuncDef("toDays(*x)", (e, a) => {
            return a[0].Div(e, new RealVal(24 * 60 * 60)).FormatReal();
        }, "Converts from epoch time to days.");

        public static readonly BuiltInFuncDef toHours = new BuiltInFuncDef("toHours(*x)", (e, a) => {
            return a[0].Div(e, new RealVal(60 * 60)).FormatReal();
        }, "Converts from epoch time to hours.");

        public static readonly BuiltInFuncDef toMinutes = new BuiltInFuncDef("toMinutes(*x)", (e, a) => {
            return a[0].Div(e, new RealVal(60)).FormatReal();
        }, "Converts from epoch time to minutes.");

        public static readonly BuiltInFuncDef toSeconds = new BuiltInFuncDef("toSeconds(*x)", (e, a) => {
            return a[0].FormatReal();
        }, "Converts from epoch time to seconds.");

        public static readonly BuiltInFuncDef fromDays = new BuiltInFuncDef("fromDays(*x)", (e, a) => {
            return a[0].Mul(e, new RealVal(24 * 60 * 60)).FormatDateTime();
        }, "Converts from days to epoch time.");

        public static readonly BuiltInFuncDef fromHours = new BuiltInFuncDef("fromHours(*x)", (e, a) => {
            return a[0].Mul(e, new RealVal(60 * 60)).FormatDateTime();
        }, "Converts from hours to epoch time.");

        public static readonly BuiltInFuncDef fromMinutes = new BuiltInFuncDef("fromMinutes(*x)", (e, a) => {
            return a[0].Mul(e, new RealVal(60)).FormatDateTime();
        }, "Converts from minutes to epoch time.");

        public static readonly BuiltInFuncDef fromSeconds = new BuiltInFuncDef("fromSeconds(*x)", (e, a) => {
            return a[0].FormatDateTime();
        }, "Converts from seconds to epoch time.");
    }
}
