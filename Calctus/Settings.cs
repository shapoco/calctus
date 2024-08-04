using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using Shapoco.Texts;
using Shapoco.Drawings;
using Shapoco.Calctus.Model;
using Shapoco.Calctus.Model.Standards;
using Shapoco.Calctus.Model.Evaluations;
using Shapoco.Calctus.UI;

namespace Shapoco.Calctus {
    internal class Settings {
        public static string PathInInstallDirectory => Path.Combine(AppDataManager.AssemblyPath, Filename);
        public static string PathInRoamingDirectory => Path.Combine(AppDataManager.RoamingUserDataPath, Filename);

        private const string ColorSettingNamePrefix = "Appearance_Color_";

        private static Settings _instance = null;
        public static readonly Settings Instance = _instance == null ? (_instance = new Settings(true)) : _instance;

        public static string Filename => Program.DebugMode ? "Settings.Debug.cfg" : "Settings.cfg";

        public Settings(bool loadSettings) {
            if (loadSettings) {
                try {
                    AppDataManager.UseAssemblyPath = File.Exists(PathInInstallDirectory);
#if DEBUG
                    Console.WriteLine("Setting file found in install directory: \"" + PathInInstallDirectory + "\"");
#endif
                }
                catch { }
#if DEBUG
                Console.WriteLine("Setting file path: \"" + AppDataManager.ActiveDataPath + "\"");
#endif
                var dic = new Dictionary<string, string>();
                AppDataManager.LoadPropertiesFromRoamingAppData(this, Filename, dic);
                migrateOldSettings(dic);
            }
        }

        private void migrateOldSettings(Dictionary<string, string> dic) {
            string s; bool b; float f;
            if (dic.TryGetValue("Appearance_Font_Button_Name", out s)) {
                Appearance_Font_Button.Face = CStyleEscaping.UnquoteAndUnescape(s);
            }
            if (dic.TryGetValue("Appearance_Font_Expr_Name", out s)) {
                Appearance_Font_Expr.Face = CStyleEscaping.UnquoteAndUnescape(s);
            }
            if (dic.TryGetValue("Appearance_Font_Size", out s) && float.TryParse(s, out f)) {
                Appearance_Font_Button.Size = f;
                Appearance_Font_Expr.Size = f * 1.25f;
            }
            if (dic.TryGetValue("Appearance_Font_Bold", out s) && bool.TryParse(s, out b)) {
                Appearance_Font_Button.Bold = b;
                Appearance_Font_Expr.Bold = b;
            }
        }

        public void Save() {
            AppDataManager.SavePropertiesToRoamingAppData(this, Filename);
        }

        public string LastVersion => Application.ProductVersion;

        public bool Startup_TrayIcon { get; set; } = false;

        public bool Hotkey_Enabled { get; set; } = false;
        public bool HotKey_Win { get; set; } = false;
        public bool HotKey_Alt { get; set; } = false;
        public bool HotKey_Ctrl { get; set; } = false;
        public bool HotKey_Shift { get; set; } = false;
        public Keys HotKey_KeyCode { get; set; } = Keys.None;

        public int History_KeepPeriod { get; set; } = 7;

        public int Calculation_Limit_MaxArrayLength { get; set; } = 256;
        public int Calculation_Limit_MaxStringLength { get; set; } = 256;
        public int Calculation_Limit_MaxCallRecursions { get; set; } = 64;

        public bool Input_IdAutoCompletion { get; set; } = true;
        public bool Input_AutoCloseBrackets { get; set; } = true;
        public bool Input_AutoInputAns { get; set; } = true;

        public int NumberFormat_Decimal_MaxLen { get; set; } = 9;

        public bool NumberFormat_Exp_Enabled { get; set; } = true;
        public bool NumberFormat_Exp_Alignment { get; set; } = false;
        public int NumberFormat_Exp_PositiveMin { get; set; } = 15;
        public int NumberFormat_Exp_NegativeMax { get; set; } = -5;

        public bool NumberFormat_Separator_Thousands { get; set; } = true;
        public bool NumberFormat_Separator_Hexadecimal { get; set; } = true;

        public FontSettings Appearance_Font_Button { get;} = new FontSettings("Arial", 12, FontStyle.Regular);
        public FontSettings Appearance_Font_Expr { get; } = new FontSettings("Consolas", 18, FontStyle.Regular);

        private Color _backColor = Color.FromArgb(28, 32, 36);
        public bool Appearance_IsDarkTheme { get; private set; }
        public Color Appearance_Color_Background { 
            get => _backColor;
            set {
                _backColor = value;
                Appearance_IsDarkTheme = value.GrayScale().R < 128;
            }
        } 
        public Color Appearance_Color_Active_Background { get; set; } = Color.FromArgb(0, 0, 0);
        public Color Appearance_Color_Button_Face { get; set; } = Color.FromArgb(56, 64, 72);
        public Color Appearance_Color_Text { get; set; } = Color.FromArgb(255, 255, 255);
        public Color Appearance_Color_Selection { get; set; } = Color.FromArgb(0, 128, 255);
        public Color Appearance_Color_RPN_Target { get; set; } = Color.FromArgb(0, 64, 255);
        public Color Appearance_Color_Symbols { get; set; } = Color.FromArgb(64, 192, 255);
        public Color Appearance_Color_Identifiers { get; set; } = Color.FromArgb(192, 255, 128);
        public Color Appearance_Color_Special_Literals { get; set; } = Color.FromArgb(255, 192, 64);
        public Color Appearance_Color_SI_Prefix { get; set; } = Color.FromArgb(224, 160, 255);
        public Color Appearance_Color_Parenthesis_1 { get; set; } = Color.FromArgb(64, 192, 255);
        public Color Appearance_Color_Parenthesis_2 { get; set; } = Color.FromArgb(192, 128, 255);
        public Color Appearance_Color_Parenthesis_3 { get; set; } = Color.FromArgb(255, 128, 192);
        public Color Appearance_Color_Parenthesis_4 { get; set; } = Color.FromArgb(255, 192, 64);
        public Color Appearance_Color_Error { get; set; } = Color.FromArgb(255, 128, 128);

        private static Dictionary<string, System.Reflection.PropertyInfo> _colorProperties = null;
        public static Dictionary<string, System.Reflection.PropertyInfo> ColorProperties {
            get {
                if (_colorProperties == null) {
                    var props = typeof(Settings).GetProperties()
                        .Where(p => p.Name.StartsWith(ColorSettingNamePrefix) && p.PropertyType.Equals(typeof(Color)));
                    _colorProperties = new Dictionary<string, System.Reflection.PropertyInfo>();
                    foreach (var prop in props) {
                        var displayNemae = prop.Name.Substring(ColorSettingNamePrefix.Length);
                        if (displayNemae == "SI_Prefix") {
                            displayNemae = "Exponent / SI Prefix";
                        }
                        else {
                            displayNemae = displayNemae.Replace('_', ' ');
                        }
                        _colorProperties.Add(displayNemae, prop);
                    }
                }
                return _colorProperties;
            }
        }

        public void InvertColors() {
            foreach(var prop in ColorProperties.Values) {
                var color = (Color)prop.GetValue(this);
                prop.SetValue(this, ColorEx.InvertHsvValue(color));
            }
        }

        public void RotateColorHue(PropertyInfo prop, int delta) {
            var color = (Color)prop.GetValue(this);
            color.ToHsv(out float h, out float s, out float v);
            prop.SetValue(this, ColorEx.HsvToRgb(h + delta, s, v));
        }

        public void SwapColorRb(PropertyInfo prop) {
            var color = (Color)prop.GetValue(this);
            prop.SetValue(this, Color.FromArgb(255, color.B, color.G, color.R));
        }

        public string UserConstants { get; set; } = "";
        public UserConstant[] GetUserConstants() {
            var tsv = UserConstants;
            var rows = tsv.Split('\n');
            var list = new List<UserConstant>();
            foreach (var row in rows) {
                var cols = row.Split('\t');
                if (cols.Length == 3) {
                    for (int i = 0; i < cols.Length; i++) {
                        cols[i] = CStyleEscaping.Unescape(cols[i]);
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
                    cols[i] = CStyleEscaping.Escape(cols[i]);
                }
                sb.Append(string.Join("\t", cols) + '\n');
            }
            UserConstants = sb.ToString();
        }

        public bool Window_RememberPosition { get; set; } = true;
        public int Window_Dpi { get; set; } = 96;
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
                        cols[i] = CStyleEscaping.Unescape(cols[i]);
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
                    cols[i] = CStyleEscaping.Escape(cols[i]);
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

        public ValuePickupJoinMethod ValuePickup_JoinMethod { get; set; } = ValuePickupJoinMethod.WithOperator;
        public string ValuePickup_JoinOperator { get; set; } = "+";
        public bool ValuePickup_RemoveComma { get; set; } = true;
        public ValuePickupFormatting ValuePickup_ValueFormatting { get; set; } = ValuePickupFormatting.NoChange;
    }
}
