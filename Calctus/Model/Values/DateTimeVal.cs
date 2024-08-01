using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapoco.Calctus.Model.Formats;

namespace Shapoco.Calctus.Model.Values {
    class DateTimeVal {
        public static readonly RealVal Sunday = new RealVal((int)DayOfWeek.Sunday, FormatFlags.DayOfWeek);
        public static readonly RealVal Monday = new RealVal((int)DayOfWeek.Monday, FormatFlags.DayOfWeek);
        public static readonly RealVal Tuesday = new RealVal((int)DayOfWeek.Tuesday, FormatFlags.DayOfWeek);
        public static readonly RealVal Wednesday = new RealVal((int)DayOfWeek.Wednesday, FormatFlags.DayOfWeek);
        public static readonly RealVal Thursday = new RealVal((int)DayOfWeek.Thursday, FormatFlags.DayOfWeek);
        public static readonly RealVal Friday = new RealVal((int)DayOfWeek.Friday, FormatFlags.DayOfWeek);
        public static readonly RealVal Saturday = new RealVal((int)DayOfWeek.Saturday, FormatFlags.DayOfWeek);
        public static readonly ListVal DayOfWeekList = new ListVal(new RealVal[] { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday });
    }
}
