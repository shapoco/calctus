using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public bool NumberFormat_Exp_Enabled { get; set; } = true;
        public bool NumberFormat_Exp_Alignment { get; set; } = false;
        public int NumberFormat_Exp_PositiveMin { get; set; } = 15;
        public int NumberFormat_Exp_NegativeMax { get; set; } = -5;
    }
}
