using FormatApi;
using System.Collections.Concurrent;


namespace FileFormatDetector.Core
{
    /// <summary>
    /// Format detector
    /// </summary>
    public class FormatDetector
    {
        private readonly IFormatDetector[] _generalFormatDetectors;
        private readonly ITextBasedFormatDetector[] _textBasedFormatDetectors;
        private readonly bool _hasFomatsWithSignature = false;
        private readonly int _maxHeaderLength = 0;
        private readonly IFormatDetector[] _formatsWithMandatorySignature;
        private readonly IFormatDetector[] _formatsWithOptionalOrMissingSignature;

        /// <summary>
        /// Format detector's configuration
        /// </summary>
        public FormatDetectorConfiguration Configuration { get; }

        public FormatDetector(FormatDetectorConfiguration configuration, IFormatDetector[] generalFormats, ITextBasedFormatDetector[] textBasedFormats)
        {
            Configuration = configuration;
            _generalFormatDetectors = generalFormats ?? throw new ArgumentNullException(nameof(generalFormats));
            _textBasedFormatDetectors = textBasedFormats ?? throw new ArgumentNullException(nameof(textBasedFormats));

            _formatsWithMandatorySignature = _generalFormatDetectors.Where(f => f.HasSignature && f.SignatureIsMandatory).ToArray();
            _formatsWithOptionalOrMissingSignature = _generalFormatDetectors.Where(f => !f.HasSignature || f.HasSignature && !f.SignatureIsMandatory).ToArray();

            _hasFomatsWithSignature = _formatsWithMandatorySignature.Any();
            _maxHeaderLength = _hasFomatsWithSignature ? _generalFormatDetectors.Where(f => f.HasSignature).Max(f => f.BytesToReadSignature) : 0;
        }

        /// <summary>
        /// Scan provided paths and check files' formats
        /// </summary>
        /// <param name="paths">Collection of paths to scan. May contain directories or individual files</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Collection of recognized files</returns>
        public async Task<IEnumerable<FileRecognitionResult>> ScanFiles(string[] paths, CancellationToken cancellationToken)
        {
            var filesIterator = new FilesIterator(paths, Configuration.Recursive);

            ParallelOptions parallelOptions = new ParallelOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Configuration.Threads.HasValue ? Configuration.Threads.Value : Environment.ProcessorCount,
            };

            ConcurrentBag<FileRecognitionResult> recognizedFiles = new ConcurrentBag<FileRecognitionResult>();

            try
            {
                await Parallel.ForEachAsync(filesIterator, parallelOptions, async (f, token) =>
                {
                    var recognizedFile = await DetectFormat(f, token);
                    recognizedFiles.Add(recognizedFile);
                });
            }
            catch (OperationCanceledException)
            {
            }

            return recognizedFiles;
        }

        /// <summary>
        /// Detect format of a single file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        private async Task<FileRecognitionResult> DetectFormat(string path, CancellationToken cancellationToken)
        {
            FileRecognitionResult? recognitionResult = null;

            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Length > 0)
                {
                    FileStreamOptions options = new FileStreamOptions()
                    {
                        Mode = FileMode.Open,
                        Access = FileAccess.Read,
                        Share = FileShare.ReadWrite,
                        Options = FileOptions.Asynchronous,
                    };

                    using (var file = File.Open(path, options))
                    {
                        recognitionResult = await TryDetectGeneralFormat(file, path, cancellationToken);

                        if (recognitionResult != null && recognitionResult.IsTextFile)
                        {
                            var textBasedFormat = await TryDetectTextBasedFormat(path, file, (TextFormatSummary)recognitionResult.FormatSummary!, cancellationToken);

                            if (textBasedFormat != null)
                                recognitionResult = textBasedFormat;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                //If detection was cancelled, then the file is considered unknown
            }
            catch (Exception ex)
            {
                return FileRecognitionResult.Faulted(path, ex);
            }

            return recognitionResult ?? FileRecognitionResult.Unknown(path);
        }


        private async Task<FileRecognitionResult?> TryDetectGeneralFormat(Stream stream, string path, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[_maxHeaderLength];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            stream.Seek(0, SeekOrigin.Begin);

            Memory<byte> signature = buffer.AsMemory(0, bytesRead);

            //Trying formats with mandatory signature
            FileRecognitionResult? recognitionResult = await Detect(path, stream, signature, _formatsWithMandatorySignature, cancellationToken);

            //Trying formats with optional signature
            recognitionResult ??= await Detect(path, stream, signature, _formatsWithOptionalOrMissingSignature, cancellationToken);

            return recognitionResult;
        }

        private async Task<FileRecognitionResult?> Detect(string path, Stream stream, Memory<byte> signature, IEnumerable<IFormatDetector> detectors, CancellationToken cancellationToken)
        {
            foreach (var format in detectors)
            {
                try
                {
                    var isSignatureFound = format.HasSignature && format.CheckSignature(signature.Span);

                    if (isSignatureFound || !format.SignatureIsMandatory)
                    {
                        if (stream.Position != 0)
                            stream.Seek(0, SeekOrigin.Begin);

                        var summary = await format.ReadFormat(stream, cancellationToken);

                        if (summary != null)
                        {
                            return FileRecognitionResult.Recognized(path, summary);
                        }
                    }
                }
                catch (NotSupportedException)
                {
                    //File is considered unknown
                }
                catch (FileFormatException ex)
                {
                    return FileRecognitionResult.Faulted(path, new FileRecognitionException(ex.Message, format, ex));
                }
            }

            return null;
        }


        private async Task<FileRecognitionResult?> TryDetectTextBasedFormat(string path, Stream stream, TextFormatSummary textFormatSummary, CancellationToken cancellationToken)
        {
            foreach (var format in _textBasedFormatDetectors)
            {
                try
                {
                    if (stream.Position != 0)
                        stream.Seek(0, SeekOrigin.Begin);

                    var summary = await format.ReadFormat(stream, textFormatSummary, cancellationToken);

                    if (summary != null)
                    {
                        return FileRecognitionResult.Recognized(path, summary);
                    }
                }
                catch (NotSupportedException)
                {
                    //File is considered unknown
                }
                catch (FileFormatException ex)
                {
                    return FileRecognitionResult.Faulted(path, new FileRecognitionException(ex.Message, format, ex));
                }
            }

            return null;
        }
    }
}
