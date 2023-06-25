using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Formats {
    class FormatSettingss {
        public int DecimalLengthToDisplay { get; set; } = 9;
        public bool ENotationEnabled { get; set; } = true;
        public int ENotationExpPositiveMin { get; set; } = 15;
        public int ENotationExpNegativeMax { get; set; } = -5;
        public bool ENotationAlignment { get; set; } = false;

        public FormatSettingss() {
            var s = Settings.Instance;
            DecimalLengthToDisplay = s.NumberFormat_Decimal_MaxLen;
            ENotationEnabled = s.NumberFormat_Exp_Enabled;
            ENotationExpPositiveMin = s.NumberFormat_Exp_PositiveMin;
            ENotationExpNegativeMax = s.NumberFormat_Exp_NegativeMax;
            ENotationAlignment = s.NumberFormat_Exp_Alignment;
        }
    }
}
