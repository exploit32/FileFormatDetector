using FormatApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector
{
    internal class FormatDetector
    {
        private readonly IBinaryFormatDetector[] _formats;

        public AppConfiguration Configuration { get; }

        public FormatDetector(AppConfiguration configuration, IBinaryFormatDetector[] formats)
        {
            Configuration = configuration;
            _formats = formats ?? throw new ArgumentNullException(nameof(formats));
        }

        public async Task<IEnumerable<RecognizedFile>> ScanFiles(CancellationToken cancellationToken)
        {
            var paths = Configuration.Paths.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var filesIterator = new FilesIterator(paths, Configuration.Recursive);

            int minReadLength = GetMinReadLength();

            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = cancellationToken;
            parallelOptions.MaxDegreeOfParallelism = Configuration.Threads == 0 ? System.Environment.ProcessorCount : Configuration.Threads;

            ConcurrentBag<RecognizedFile> recognizedFiles = new ConcurrentBag<RecognizedFile>();

            try
            {
                Parallel.ForEach(filesIterator, parallelOptions, f =>
                {
                    var recognizedFile = DetectFormat(f, minReadLength);

                    if (recognizedFile != null)
                        recognizedFiles.Add(recognizedFile);
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Detection cancelled");
            }

            return recognizedFiles;
        }

        private RecognizedFile? DetectFormat(string path, int headerLength)
        {
            //Console.WriteLine("Processing file {0}", path);

            FormatSummary? summary = null;

            try
            {

                if (!File.Exists(path))
                    return null;

                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Length < headerLength)
                    return null;

                byte[] buffer = new byte[headerLength];

                var detected = false;

                //using (var file = File.OpenRead(path))
                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int bytesRead = file.Read(buffer, 0, buffer.Length);

                    if (bytesRead < headerLength)
                        return null;

                    foreach (var format in _formats)
                    {
                        var isSignatureFound = format.CheckSignature(buffer);

                        if (isSignatureFound)
                        {
                            file.Seek(0, SeekOrigin.Begin);
                            summary = format.ReadFormat(file);
                        }

                        detected |= summary != null;

                        if (detected)
                        {
                            //Console.WriteLine("File {0} recognized as {1}", path, format.Description);
                            break;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException ex) 
            {
                Console.WriteLine($"Error opening {path} : {ex.Message}");
            }


            return summary != null ? new RecognizedFile(path, summary) : null;
        }

        private bool HasFormatWithSignature() => _formats.Any(f => f.HasSignature);

        private int GetMinReadLength()
        {
            return _formats.Where(f => f.HasSignature).Max(f => f.BytesToReadSignature);
        }
    }
}
