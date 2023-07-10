using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class DiskFilesFactory : IFilesFactory
    {
        public bool CheckFileExistsAndNotEmpty(string path)
        {
            if (!File.Exists(path))
                return false;

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length == 0)
                return false;

            return true;
        }

        public Stream Open(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}
