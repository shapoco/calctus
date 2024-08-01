using System;
using System.Globalization;
using Shapoco.Calctus.Model.Standards;

namespace Shapoco.Calctus.Model.Formats {
    static class DateTimeFormat {
        public static string FormatAsStringLiteral(decimal unixTime, bool quotation) {
            return FormatAsStringLiteral(UnixTime.ToLocalTime(unixTime), quotation);
        }
        
        public static string FormatAsStringLiteral(DateTime dateTime, bool quotation) {
            string str;
            if (dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0) {
                str = dateTime.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
            else {
                str = dateTime.ToString("yyyy/MM/dd HH:mm:ss.FFF", CultureInfo.InvariantCulture);
            }
            if (quotation) {
                return '#' + str + '#';
            }
            else {
                return str;
            }
        }
    }
}
