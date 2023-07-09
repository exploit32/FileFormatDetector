using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    internal class FilesIterator : IEnumerable<string>
    {
        private Stack<string> _paths;

        private readonly bool _recursive;

        public FilesIterator(IEnumerable<string> paths, bool recursive)
        {
            _paths = new Stack<string>(paths);
            _recursive = recursive;
        }

        public IEnumerator<string> GetEnumerator()
        {
            while (_paths.Any())
            {
                string path = _paths.Pop();

                if (File.Exists(path))
                    yield return path;

                if (Directory.Exists(path))
                {
                    if (_recursive)
                    {
                        try
                        {
                            var subdirs = Directory.GetDirectories(path);

                            foreach (var subdir in subdirs)
                            {
                                _paths.Push(subdir);
                            }
                        }
                        catch (UnauthorizedAccessException) { }
                    }

                    var files = Array.Empty<string>();

                    try
                    {
                        files = Directory.GetFiles(path);
                    }
                    catch (UnauthorizedAccessException) { }

                    foreach (var file in files)
                        yield return file;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
