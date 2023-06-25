using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Shapoco.Calctus.UI {
    class CustomProfessionalColors : ProfessionalColorTable {
        public override Color ToolStripGradientBegin { get { return Settings.Instance.Appearance_Color_Button_Face; } }
        public override Color ToolStripGradientMiddle { get { return Settings.Instance.Appearance_Color_Button_Face; } }
        public override Color ToolStripGradientEnd { get { return Settings.Instance.Appearance_Color_Button_Face; } }
        public override Color ToolStripBorder { get { return Settings.Instance.Appearance_Color_Button_Face; } }
    }
}
