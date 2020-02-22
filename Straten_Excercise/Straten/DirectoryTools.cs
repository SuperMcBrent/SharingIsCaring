using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Straten {
    class DirectoryTools {
        public static DirectoryInfo CreateDir(string path, string dirName) {
            DirectoryInfo dirInfo = null;
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(dirName)) {
                return dirInfo;
            }
            string directory = Path.Combine(path, dirName);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
                return new DirectoryInfo(directory);
            }
            return dirInfo;
        }
    }
}
