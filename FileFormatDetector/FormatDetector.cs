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
        private readonly IBinaryFormatDetector[] _binaryFormats;
        private readonly ITextFormatDetector[] _textFormats;

        public AppConfiguration Configuration { get; }

        public FormatDetector(AppConfiguration configuration, IBinaryFormatDetector[] binaryFormats, ITextFormatDetector[] textFormats)
        {
            Configuration = configuration;
            _binaryFormats = binaryFormats ?? throw new ArgumentNullException(nameof(binaryFormats));
            _textFormats = textFormats ?? throw new ArgumentNullException(nameof(textFormats));
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

                //FileInfo fileInfo = new FileInfo(path);
                //if (fileInfo.Length < headerLength)
                //    return null;

                var detected = false;

                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    summary = TryDetectBinaryFormat(file, headerLength);

                    if (summary == null)
                    {
                        summary = TryDetectTextFormat(file);
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

        private FormatSummary? TryDetectBinaryFormat(Stream stream, int headerLength)
        {
            FormatSummary? summary = null;

            byte[] buffer = new byte[headerLength];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            stream.Seek(0, SeekOrigin.Begin);

            if (bytesRead < headerLength)
                return null;

            foreach (var format in _binaryFormats)
            {
                var isSignatureFound = format.CheckSignature(buffer);

                if (isSignatureFound)
                {                    
                    summary = format.ReadFormat(stream);

                    if (summary != null)
                    {
                        //Console.WriteLine("File {0} recognized as {1}", path, format.Description);
                        break;
                    }
                }
            }

            return summary;
        }

        private FormatSummary? TryDetectTextFormat(Stream stream)
        {
            FormatSummary? summary = null;

            foreach (var format in _textFormats)
            {
                summary = format.ReadFormat(stream, 4096);

                if (summary != null)
                {
                    //Console.WriteLine("File {0} recognized as {1}", path, format.Description);
                    break;
                }
            }

            return summary;
        }

        private bool HasFormatWithSignature() => _binaryFormats.Any(f => f.HasSignature);

        private int GetMinReadLength()
        {
            return _binaryFormats.Where(f => f.HasSignature).Max(f => f.BytesToReadSignature);
        }
    }
}
