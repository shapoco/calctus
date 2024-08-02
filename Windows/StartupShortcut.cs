using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

// Windows Script Host Object Model を参照設定すること
using Wsh = IWshRuntimeLibrary;

namespace Shapoco.Windows {
    public static class StartupShortcut {
        /// <summary>
        /// 現在の実行ファイルに対するショートカットがスタートアップフォルダに存在するか否かを返す
        /// </summary>
        public static bool CheckStartupRegistration() {
            return CheckStartupRegistration(out string[] dummy);
        }

        /// <summary>
        /// 現在の実行ファイルに対するショートカットがスタートアップフォルダに存在するか否かを返す
        /// </summary>
        public static bool CheckStartupRegistration(out string[] shortcutPath) {
            shortcutPath =
                FindShortcut(StartupPath, System.Windows.Forms.Application.ExecutablePath)
                .ToArray();
            return (shortcutPath.Length > 0);
        }

        /// <summary>
        /// 現在の実行ファイルに対するショートカットがスタートアップフォルダに存在するか否かを確認し、
        /// 存在しなければ作成する
        /// </summary>
        public static void SetStartupRegistration(
            bool registrationState,
            string defaultShortcutName = null,
            string arguments = null,
            string workDir = null) {
            var appPath = System.Windows.Forms.Application.ExecutablePath;

            if (string.IsNullOrEmpty(defaultShortcutName)) {
                defaultShortcutName = Path.GetFileNameWithoutExtension(appPath) + ".lnk";
            }

            if (registrationState) {
                if (!CheckStartupRegistration()) {
                    // 作成する場合
                    var shortcutPath = Path.Combine(StartupPath, defaultShortcutName);

                    // ファイル名の衝突を回避する
                    int fileNumber = 1;
                    while (File.Exists(shortcutPath)) {
                        var baseName = Path.GetFileNameWithoutExtension(defaultShortcutName);
                        var ext = Path.GetExtension(defaultShortcutName);
                        shortcutPath = Path.Combine(StartupPath, baseName + (fileNumber++) + ext);
                    }

                    CreateShortcut(
                        shortcutPath: shortcutPath,
                        targetPath: appPath,
                        workDir: workDir,
                        arguments: arguments);
                }
            }
            else {
                // 削除する場合
                foreach (var shortcutPath in FindShortcut(StartupPath, appPath)) {
                    try {
                        File.Delete(shortcutPath);
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// ショートカットファイルを作成する
        /// </summary>
        public static void CreateShortcut(
            string shortcutPath,
            string targetPath,
            string workDir = null,
            string arguments = "",
            string iconLocation = null) {
            Wsh.WshShell shell = null;
            Wsh.IWshShortcut shortcut = null;
            try {
                shell = new Wsh.WshShell();
                shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;

                if (!string.IsNullOrEmpty(workDir)) shortcut.WorkingDirectory = workDir;
                if (!string.IsNullOrEmpty(arguments)) shortcut.Arguments = arguments;
                //shortcut.Hotkey = "Ctrl+Alt+Shift+F12";
                //shortcut.WindowStyle = 1;
                //shortcut.Description = "テストのアプリケーション";
                if (!string.IsNullOrEmpty(iconLocation)) shortcut.IconLocation = iconLocation;

                shortcut.Save();
            }
            catch (Exception ex) { throw ex; }
            finally {
                if (shortcut != null) Marshal.FinalReleaseComObject(shortcut);
                if (shell != null) Marshal.FinalReleaseComObject(shell);
            }
        }

        /// <summary>
        /// 指定されたディレクトリから、指定されたファイルをターゲットとするショートカットファイルを検索する
        /// </summary>
        public static IEnumerable<string> FindShortcut(string searchDir, string targetPath) {
            targetPath = targetPath.ToLower(); // 大小文字同一視のため小文字化

            var shell = new Wsh.IWshShell_Class();
            foreach (var linkFilePath in Directory.GetFiles(StartupPath)) {
                bool hit = false;
                Wsh.IWshShortcut_Class shortcut = null;
                try {
                    shortcut = (Wsh.IWshShortcut_Class)shell.CreateShortcut(linkFilePath);
                    hit = (shortcut.TargetPath.ToLower() == targetPath); // 小文字化して比較
                }
                catch (Exception) { }
                finally {
                    if (shortcut != null) Marshal.FinalReleaseComObject(shortcut);
                }

                // try-catch内ではyield returnできないので外で
                if (hit) yield return linkFilePath;
            }

            if (shell != null) Marshal.FinalReleaseComObject(shell);

        }

        /// <summary>
        /// スタートアップフォルダの場所
        /// </summary>
        public static string StartupPath {
            get {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                    "Startup");
            }
        }

        /// <summary>
        /// 指定されたファイルのリンク先パスを取得する
        /// </summary>
        public static string GetLinkTargetOf(string linkFilePath) {
            Wsh.IWshShell_Class shell = null;
            Wsh.IWshShortcut_Class shortcut = null;
            try {
                shell = new Wsh.IWshShell_Class();
                shortcut = (Wsh.IWshShortcut_Class)shell.CreateShortcut(linkFilePath);
                return shortcut.TargetPath;
            }
            catch (Exception ex) { throw ex; }
            finally {
                if (shortcut != null) Marshal.FinalReleaseComObject(shortcut);
                if (shell != null) Marshal.FinalReleaseComObject(shell);
            }
        }
    }
}
