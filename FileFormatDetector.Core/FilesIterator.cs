using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    /// <summary>
    /// Iterator that scans for files in specified path
    /// </summary>
    internal class FilesIterator : IEnumerable<string>
    {
        private Stack<string> _paths;

        private readonly bool _recursive;

        /// <summary>
        /// Constructs files iterator
        /// </summary>
        /// <param name="paths">Collection of files or directories</param>
        /// <param name="recursive">Scan recursively</param>
        public FilesIterator(IEnumerable<string> paths, bool recursive)
        {
            _paths = new Stack<string>(paths);
            _recursive = recursive;
        }

        /// <summary>
        /// Get enumerator that interates over given paths
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<string> GetEnumerator()
        {
            while (_paths.Any())
            {
                string path = _paths.Pop();

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
                        catch (IOException) { }
                    }

                    var files = Array.Empty<string>();

                    try
                    {
                        files = Directory.GetFiles(path);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (IOException) { }

                    foreach (var file in files)
                        yield return file;
                }
                else
                    yield return path;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
