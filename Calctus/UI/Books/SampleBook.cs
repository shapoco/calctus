using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Shapoco.Calctus.UI.Books {
    class SampleBook : Book {
        public const string SampleFolderName = "Samples";

        // サンプルフォルダがありそうな場所を探す
        private static bool _directorySearchDone = false;
        private static string _directorySearchResult = null;
        public static bool FindSampleFolder(out string path) {
            if (!_directorySearchDone) {
                try {
                    var binPath = AppDataManager.AssemblyPath;
                    string sampleDir;
                    sampleDir = Path.Combine(binPath, SampleFolderName);
                    _directorySearchResult = null;
                    if (Directory.Exists(sampleDir)) {
                        _directorySearchResult = sampleDir;
                    }
                    else if ((binPath.EndsWith(@"\bin\Debug") || binPath.EndsWith(@"\bin\Release"))
                        && Directory.Exists(sampleDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(binPath)), SampleFolderName))) {
                        _directorySearchResult = sampleDir;
                    }
                }
                catch {
                    _directorySearchResult = null;
                }
                _directorySearchDone = true;
            }
            path = _directorySearchResult;
            return !string.IsNullOrEmpty(path);
        }

        public SampleBook() : base(SampleFolderName, SampleFolderName, SortMode.ByName) { }

        public override string DirectoryPath {
            get {
                if (FindSampleFolder(out var path)) {
                    return path;
                }
                else {
                    return null;
                }
            }
        }
    }
}
