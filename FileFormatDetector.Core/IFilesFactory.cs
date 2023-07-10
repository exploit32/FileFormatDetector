using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public interface IFilesFactory
    {
        bool CheckFileExistsAndNotEmpty(string path);

        Stream Open(string path);        
    }
}
