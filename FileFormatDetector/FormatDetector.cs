using FormatApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector
{
    internal class FormatDetector
    {
        private readonly IBinaryFormatDetector[] _binaryFormats;
        private readonly ITextFormatDetector[] _textFormats;
        private readonly ITextBasedFormatDetector[] _textBasedFormats;

        public AppConfiguration Configuration { get; }

        public FormatDetector(AppConfiguration configuration, IBinaryFormatDetector[] binaryFormats, ITextFormatDetector[] textFormats, ITextBasedFormatDetector[] textBasedFormats)
        {
            Configuration = configuration;
            _binaryFormats = binaryFormats ?? throw new ArgumentNullException(nameof(binaryFormats));
            _textFormats = textFormats ?? throw new ArgumentNullException(nameof(textFormats));
            _textBasedFormats = textBasedFormats ?? throw new ArgumentNullException(nameof(textBasedFormats));
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
                Parallel.ForEach(filesIterator, parallelOptions, async f =>
                {
                    var recognizedFile = await DetectFormat(f, minReadLength);

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

        private async Task<RecognizedFile?> DetectFormat(string path, int headerLength)
        {
            //Console.WriteLine("Processing file {0}", path);

            FormatSummary? summary = null;

            try
            {
                if (!File.Exists(path))
                    return null;

                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    summary = TryDetectBinaryFormat(file, path, headerLength);

                    if (summary == null)
                    {
                        TextFormatSummary? textFormatSummary = await TryDetectTextFormat(file, path);

                        if (textFormatSummary != null)
                        {
                            summary = await TryDetectTextBasedFormat(file, path, textFormatSummary);
                        }
                        else
                        {
                            summary = textFormatSummary;
                        }
                    }

                    if (summary == null)
                        summary = new UnknownFormatSummary();
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException ex) 
            {
                Console.WriteLine($"Error opening {path} : {ex.Message}");
            }


            return summary != null ? new RecognizedFile(path, summary) : null;
        }

        private FormatSummary? TryDetectBinaryFormat(Stream stream, string path, int headerLength)
        {
            FormatSummary? summary = null;

            byte[] buffer = new byte[headerLength];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            stream.Seek(0, SeekOrigin.Begin);

            if (bytesRead < headerLength)
                return null;

            foreach (var format in _binaryFormats)
            {
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
                }
            }

            return summary;
        }

        private async Task<TextFormatSummary?> TryDetectTextFormat(Stream stream, string path)
        {
            TextFormatSummary? summary = null;

            foreach (var format in _textFormats)
            {
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    summary = await format.ReadFormat(stream, 4096);

                    if (summary != null)
                    {
                        //Console.WriteLine("File {0} recognized as {1}", path, format.Description);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
                }
            }

            return summary;
        }

        private async Task<FormatSummary?> TryDetectTextBasedFormat(Stream stream, string path, TextFormatSummary textFormatSummary)
        {
            FormatSummary? summary = null;

            foreach (var format in _textBasedFormats)
            {
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    summary = await format.ReadFormat(stream, textFormatSummary, 4096);

                    if (summary != null)
                    {
                        //Console.WriteLine("File {0} recognized as {1}", path, format.Description);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
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
