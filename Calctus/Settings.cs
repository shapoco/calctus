using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapoco.Calctus {
    internal class Settings {
        public static readonly Settings Instance = new Settings();
        private const string Filename = "Settings.cfg";

        private Settings() {
            AppDataManager.LoadPropertiesFromRoamingAppData(this, Filename);
        }

        public void Save() {
            AppDataManager.SavePropertiesToRoamingAppData(this, Filename);
        }

        public bool Hotkey_Enabled { get; set; } = false;
        public bool HotKey_Alt { get; set; } = false;
        public bool HotKey_Ctrl { get; set; } = false;
        public bool HotKey_Shift { get; set; } = false;
        public Keys HotKey_KeyCode { get; set; } = Keys.None;

        public bool NumberFormat_Exp_Enabled { get; set; } = true;
        public bool NumberFormat_Exp_Alignment { get; set; } = false;
        public int NumberFormat_Exp_PositiveMin { get; set; } = 15;
        public int NumberFormat_Exp_NegativeMax { get; set; } = -5;

        public string Appearance_Font_Button_Name { get; set; } = "Arial";
        public string Appearance_Font_Expr_Name { get; set; } = "Consolas";
        public int Appearance_Font_Size { get; set; } = 9;
        public bool Appearance_Font_Bold { get; set; } = false;
    }
}
