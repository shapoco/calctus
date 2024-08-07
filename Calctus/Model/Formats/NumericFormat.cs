using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    enum FormatStyle {
        Default = 0,
        Character = 1,
        SiPrefixed = 2,
        BinaryPrefixed = 3,
        DayOfWeek = 4,
        DateTime = 5, // todo 廃止 FormatStyle.DateTime
        TimeSpan = 6, // todo 廃止? FormatStyle.TimeSpan
        WebColor = 7,
    }

    [Flags]
    enum FormatOption {
        None = 0,
        ApFixedWithPoint = (1 << 0),
    }
}
