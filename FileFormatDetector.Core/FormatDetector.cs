using FormatApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormatDetector.Core
{
    public class FormatDetector
    {
        private readonly IBinaryFormatDetector[] _binaryFormats;
        private readonly ITextFormatDetector[] _textFormats;
        private readonly ITextBasedFormatDetector[] _textBasedFormats;
        private readonly bool _hasFomatsWithSignature = false;
        private readonly int _maxHeaderLength = 0;

        public FormatDetectorConfiguration Configuration { get; }

        public FormatDetector(FormatDetectorConfiguration configuration, IBinaryFormatDetector[] binaryFormats, ITextFormatDetector[] textFormats, ITextBasedFormatDetector[] textBasedFormats)
        {
            Configuration = configuration;
            _binaryFormats = binaryFormats ?? throw new ArgumentNullException(nameof(binaryFormats));
            _textFormats = textFormats ?? throw new ArgumentNullException(nameof(textFormats));
            _textBasedFormats = textBasedFormats ?? throw new ArgumentNullException(nameof(textBasedFormats));

            _hasFomatsWithSignature = _binaryFormats.Any(f => f.HasSignature);
            _maxHeaderLength = _hasFomatsWithSignature ? _binaryFormats.Where(f => f.HasSignature).Max(f => f.BytesToReadSignature) : 0;
        }

        public async Task<IEnumerable<RecognizedFile>> ScanFiles(CancellationToken cancellationToken)
        {
            var filesIterator = new FilesIterator(Configuration.Paths, Configuration.Recursive);

            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Configuration.Threads.HasValue ? Configuration.Threads.Value : Environment.ProcessorCount,
            };

            ConcurrentBag<RecognizedFile> recognizedFiles = new ConcurrentBag<RecognizedFile>();

            try
            {
                await Parallel.ForEachAsync(filesIterator, parallelOptions, async (f, token) =>
                {
                    var recognizedFile = await DetectFormat(f, token);

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

        private async Task<RecognizedFile?> DetectFormat(string path, CancellationToken cancellationToken)
        {
            FormatSummary? summary = null;

            try
            {
                if (!CheckFileExistsAndNotEmpty(path))
                    return null;

                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    summary = await TryDetectBinaryFormat(file, path, cancellationToken);

                    if (summary == null)
                    {
                        TextFormatSummary? textFormatSummary = await TryDetectTextFormat(file, path, cancellationToken);

                        if (textFormatSummary != null)
                        {
                            FormatSummary? textBasedFormatSummary = await TryDetectTextBasedFormat(file, path, textFormatSummary, cancellationToken);

                            if (textBasedFormatSummary != null)
                                summary = textBasedFormatSummary;
                            else
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

        private bool CheckFileExistsAndNotEmpty(string path)
        {
            if (!File.Exists(path))
                return false;

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length == 0)
                return false;

            return true;
        }

        private async Task<FormatSummary?> TryDetectBinaryFormat(Stream stream, string path, CancellationToken cancellationToken)
        {
            FormatSummary? summary = null;

            byte[] buffer = new byte[_maxHeaderLength];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            stream.Seek(0, SeekOrigin.Begin);

            foreach (var format in _binaryFormats)
            {
                try
                {
                    var isSignatureFound = format.CheckSignature(buffer.AsSpan(0, bytesRead));

                    if (isSignatureFound)
                    {
                        summary = await format.ReadFormat(stream, cancellationToken);

                        if (summary != null)
                            break;
                    }
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}. Probably file is malformed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
                }
            }

            return summary;
        }

        private async Task<TextFormatSummary?> TryDetectTextFormat(Stream stream, string path, CancellationToken cancellationToken)
        {
            TextFormatSummary? summary = null;

            foreach (var format in _textFormats)
            {
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    summary = await format.ReadFormat(stream, Configuration.FileScanSizeLimit, cancellationToken);

                    if (summary != null)
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
                }
            }

            return summary;
        }

        private async Task<FormatSummary?> TryDetectTextBasedFormat(Stream stream, string path, TextFormatSummary textFormatSummary, CancellationToken cancellationToken)
        {
            FormatSummary? summary = null;

            foreach (var format in _textBasedFormats)
            {
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    summary = await format.ReadFormat(stream, textFormatSummary, Configuration.FileScanSizeLimit);

                    if (summary != null)
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Detector {format.GetType()} threw an exception while processing file {path}: {ex.Message}");
                }
            }

            return summary;
        }


    }
}
