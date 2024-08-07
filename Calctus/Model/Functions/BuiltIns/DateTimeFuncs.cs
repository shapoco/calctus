using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Values;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Maths;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Functions.BuiltIns {
    class DateTimeFuncs : BuiltInFuncCategory {
        private static DateTimeFuncs _instance = null;
        public static DateTimeFuncs Instance => _instance != null ? _instance : _instance = new DateTimeFuncs();
        private DateTimeFuncs() { }

        public readonly BuiltInFuncDef datetime_3 = new BuiltInFuncDef("datetime(year, mon, day)",
            "Returns datetime from year, month, and day.",
            (e, a) => UnixTime.FromLocalTime(a[0].AsInt, a[1].AsInt, a[2].AsInt, 0, 0, 0).ToDateTimeVal());

        public readonly BuiltInFuncDef datetime_6 = new BuiltInFuncDef("datetime(year, mon, day, hour, min, sec)",
            "Returns datetime from year, month, day, hour, minute, and second.",
            (e, a) => UnixTime.FromLocalTime(a[0].AsInt, a[1].AsInt, a[2].AsInt, a[3].AsInt, a[4].AsInt, a[5].AsDecimal).ToDateTimeVal());

        public readonly BuiltInFuncDef yearOf = new BuiltInFuncDef("yearOf(*t)",
            "Returns year component of datetime.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).Year.ToIntVal());

        public readonly BuiltInFuncDef monthOf = new BuiltInFuncDef("monthOf(*t)",
            "Returns month component of datetime, expressed as 1..12.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).Month.ToIntVal());

        public readonly BuiltInFuncDef dayOfYear = new BuiltInFuncDef("dayOfYear(*t)",
            "Returns day of year of datetime, expressed as 1..366.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).DayOfYear.ToIntVal());

        public readonly BuiltInFuncDef dayOfWeek = new BuiltInFuncDef("dayOfWeek(*t)",
            "Returns day of week of datetime, expressed as 0 (Sunday)..6 (Saturday).",
            (e, a) => ((int)UnixTime.ToLocalTime(a[0].AsDecimal).DayOfWeek).ToRealVal(FormatHint.DayOfWeek));

        public readonly BuiltInFuncDef dayOfMonth = new BuiltInFuncDef("dayOfMonth(*t)",
            "Returns day component of datetime, expressed as 1..31.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).Day.ToIntVal());

        public readonly BuiltInFuncDef hourOf = new BuiltInFuncDef("hourOf(*t)",
            "Returns hour component of datetime, expressed as 0..23.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).Hour.ToIntVal());

        public readonly BuiltInFuncDef minuteOf = new BuiltInFuncDef("minuteOf(*t)",
            "Returns minute component of datetime, expressed as 0..59.",
            (e, a) => UnixTime.ToLocalTime(a[0].AsDecimal).Minute.ToIntVal());

        public readonly BuiltInFuncDef secondOf = new BuiltInFuncDef("secondOf(*t)",
            "Returns second component of datetime, expressed as 0..60.",
            (e, a) => {
                var unixTime = a[0].AsDecimal;
                var subSec = unixTime - Math.Floor(unixTime);
                return (UnixTime.ToLocalTime(unixTime).Second + subSec).ToVal();
            });

        public readonly BuiltInFuncDef now = new BuiltInFuncDef("now()",
            "Returns current datetime.",
            (e, a) => UnixTime.FromLocalTime(DateTime.Now).ToDateTimeVal());

        public readonly BuiltInFuncDef today = new BuiltInFuncDef("today()",
            "Returns datetime of today's 00:00:00.",
            (e, a) => UnixTime.FromLocalTime(DateTime.Today).ToDateTimeVal());

        public readonly BuiltInFuncDef toDays = new BuiltInFuncDef("toDays(*x)",
            "Converts from epoch time to days.",
            (e, a) => (a[0].AsDecimal / (24 * 60 * 60)).ToVal(),
            new FuncTest("#+123.12:34:56.789#", "123+(12+(34+56.789/60)/60)/24"));

        public readonly BuiltInFuncDef toHours = new BuiltInFuncDef("toHours(*x)",
            "Converts from epoch time to hours.",
            (e, a) => (a[0].AsDecimal / (60 * 60)).ToVal(),
            new FuncTest("#+123.12:34:56.789#", "(123*24)+12+(34+56.789/60)/60"));

        public readonly BuiltInFuncDef toMinutes = new BuiltInFuncDef("toMinutes(*x)",
            "Converts from epoch time to minutes.",
            (e, a) => (a[0].AsDecimal / 60).ToVal(),
            new FuncTest("#+123.12:34:56.789#", "((123*24)+12)*60+34+56.789/60"));

        public readonly BuiltInFuncDef toSeconds = new BuiltInFuncDef("toSeconds(*x)",
            "Converts from epoch time to seconds.",
            (e, a) => a[0].Format(FormatHint.Default),
            new FuncTest("#+123.12:34:56.789#", "(((123*24)+12)*60+34)*60+56.789"));

        public readonly BuiltInFuncDef fromDays = new BuiltInFuncDef("fromDays(*x)",
            "Converts from days to epoch time.",
            (e, a) => (a[0].AsDecimal * (24 * 60 * 60)).ToTimeSpanVal(),
            new FuncTest("123.45", "#+123.10:48:00#"));

        public readonly BuiltInFuncDef fromHours = new BuiltInFuncDef("fromHours(*x)",
            "Converts from hours to epoch time.",
            (e, a) => (a[0].AsDecimal * (60 * 60)).ToTimeSpanVal(),
            new FuncTest("123.45", "#+5.3:27:00#"));

        public readonly BuiltInFuncDef fromMinutes = new BuiltInFuncDef("fromMinutes(*x)",
            "Converts from minutes to epoch time.",
            (e, a) => (a[0].AsDecimal * 60).ToTimeSpanVal(),
            new FuncTest("123.45", "#+2:03:27#"));

        public readonly BuiltInFuncDef fromSeconds = new BuiltInFuncDef("fromSeconds(*x)",
            "Converts from seconds to epoch time.",
            (e, a) => a[0].Format(FormatHint.TimeSpan),
            new FuncTest("123.45", "#+0:02:03.45#"));
    }
}
