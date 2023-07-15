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
        public async Task<IEnumerable<RecognizedFile>> ScanFiles(string[] paths, CancellationToken cancellationToken)
        {
            var filesIterator = new FilesIterator(paths, Configuration.Recursive);

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
            }

            return recognizedFiles;
        }

        /// <summary>
        /// Detect format of a single file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        private async Task<RecognizedFile?> DetectFormat(string path, CancellationToken cancellationToken)
        {
            FormatSummary? summary = null;

            try
            {
                if (!File.Exists(path))
                    return null;

                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Length == 0)
                    return null;

                using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    summary = await TryDetectGeneralFormat(file, path, cancellationToken);

                    if (summary != null && summary is TextFormatSummary)
                    {
                        FormatSummary? textBasedFormatSummary = await TryDetectTextBasedFormat(file, path, (TextFormatSummary)summary, cancellationToken);

                        if (textBasedFormatSummary != null)
                            summary = textBasedFormatSummary;
                    }

                    summary ??= new UnknownFormatSummary();
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException ex)
            {
                Console.WriteLine($"Error opening {path} : {ex.Message}");
            }


            return summary != null ? new RecognizedFile(path, summary) : null;
        }


        private async Task<FormatSummary?> TryDetectGeneralFormat(Stream stream, string path, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[_maxHeaderLength];

            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            stream.Seek(0, SeekOrigin.Begin);

            Memory<byte> signature = buffer.AsMemory(0, bytesRead);

            //Trying formats with mandatory signature
            var summary = await Detect(_formatsWithMandatorySignature, stream, path, signature, cancellationToken);

            //Trying formats with optional signature
            summary ??= await Detect(_formatsWithOptionalOrMissingSignature, stream, path, signature, cancellationToken);

            return summary;
        }

        private async Task<FormatSummary?> Detect(IEnumerable<IFormatDetector> detectors, Stream stream, string path, Memory<byte> signature, CancellationToken cancellationToken)
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
                            return summary;
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

            return null;
        }


        private async Task<FormatSummary?> TryDetectTextBasedFormat(Stream stream, string path, TextFormatSummary textFormatSummary, CancellationToken cancellationToken)
        {
            FormatSummary? summary = null;

            foreach (var format in _textBasedFormatDetectors)
            {
                try
                {
                    if (stream.Position != 0)
                        stream.Seek(0, SeekOrigin.Begin);

                    summary = await format.ReadFormat(stream, textFormatSummary, cancellationToken);

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
