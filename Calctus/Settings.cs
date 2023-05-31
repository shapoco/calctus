using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Evaluations;

namespace Shapoco.Calctus {
    internal class Settings {
        public static readonly Settings Instance = new Settings();
#if DEBUG
        private const string Filename = "Settings.Debug.cfg";
#else
        private const string Filename = "Settings.cfg";
#endif

        private Settings() {
            AppDataManager.LoadPropertiesFromRoamingAppData(this, Filename);
        }

        public void Save() {
            AppDataManager.SavePropertiesToRoamingAppData(this, Filename);
        }

        public bool Startup_TrayIcon { get; set; } = false;

        public bool Hotkey_Enabled { get; set; } = false;
        public bool HotKey_Win { get; set; } = false;
        public bool HotKey_Alt { get; set; } = false;
        public bool HotKey_Ctrl { get; set; } = false;
        public bool HotKey_Shift { get; set; } = false;
        public Keys HotKey_KeyCode { get; set; } = Keys.None;

        public bool Input_IdAutoCompletion { get; set; } = true;
        public bool Input_AutoCloseBrackets { get; set; } = true;
        public bool Input_AutoInputAns { get; set; } = true;

        public int NumberFormat_Decimal_MaxLen { get; set; } = 9;

        public bool NumberFormat_Exp_Enabled { get; set; } = true;
        public bool NumberFormat_Exp_Alignment { get; set; } = false;
        public int NumberFormat_Exp_PositiveMin { get; set; } = 15;
        public int NumberFormat_Exp_NegativeMax { get; set; } = -5;

        public string Appearance_Font_Button_Name { get; set; } = "Arial";
        public string Appearance_Font_Expr_Name { get; set; } = "Consolas";
        public int Appearance_Font_Size { get; set; } = 9;
        public bool Appearance_Font_Bold { get; set; } = false;

        public string UserConstants { get; set; } = "";
        public UserConstant[] GetUserConstants() {
            var tsv = UserConstants;
            var rows = tsv.Split('\n');
            var list = new List<UserConstant>();
            foreach (var row in rows) {
                var cols = row.Split('\t');
                if (cols.Length == 3) {
                    for (int i = 0; i < cols.Length; i++) {
                        cols[i] = AppDataManager.UnescapeValue(cols[i]);
                    }
                    list.Add(new UserConstant(cols[0], cols[1], cols[2]));
                }
            }
            return list.ToArray();
        }
        public void SetUserConstants(IEnumerable<UserConstant> consts) {
            var sb = new StringBuilder();
            foreach (var c in consts) {
                var cols = new string[] { c.Id, c.ValueString, c.Description };
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = AppDataManager.EscapeValue(cols[i]);
                }
                sb.Append(string.Join("\t", cols) + '\n');
            }
            UserConstants = sb.ToString();
        }

        public bool Window_RememberPosition { get; set; } = true;
        public int Window_X { get; set; } = 100;
        public int Window_Y { get; set; } = 100;
        public int Window_Width { get; set; } = 640;
        public int Window_Height { get; set; } = 480;

        public bool Script_Enable { get; set; } = false;
        public string Script_FolderPath { get; set; } = 
            System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Application.ProductName), "Scripts");
        public string Script_Filter { get; set; } = "*.ps1\tpowershell\t-File \\\"%s\\\" %p\n*.py\tpython\t";
        public ScriptFilter[] GetScriptFilters() {
            var tsv = Script_Filter;
            var rows = tsv.Split('\n');
            var list = new List<ScriptFilter>();
            foreach (var row in rows) {
                var cols = row.Split('\t');
                if (cols.Length == 3) {
                    for (int i = 0; i < cols.Length; i++) {
                        cols[i] = AppDataManager.UnescapeValue(cols[i]);
                    }
                    list.Add(new ScriptFilter(cols[0], cols[1], cols[2]));
                }
            }
            return list.ToArray();
        }
        public void SetScriptFilters(IEnumerable<ScriptFilter> filters) {
            var sb = new StringBuilder();
            foreach (var sf in filters) {
                var cols = new string[] { sf.Filter, sf.Command, sf.Parameter };
                for (int i = 0; i < cols.Length; i++) {
                    cols[i] = AppDataManager.EscapeValue(cols[i]);
                }
                sb.Append(string.Join("\t", cols) + '\n');
            }
            Script_Filter = sb.ToString();
        }
        public ScriptFilter GetScriptFilterFromPath(string path) {
            var filters = GetScriptFilters();
            foreach(var sf in filters) {
                if (ScriptFilter.IsWildcardMatch(sf.Filter, System.IO.Path.GetFileName( path))) {
                    return sf;
                }
            }
            return null;
        }

    }
}
