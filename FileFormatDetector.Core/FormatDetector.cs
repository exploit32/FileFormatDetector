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
        private readonly int _maxSignatureLength = 0;

        /// <summary>
        /// Format detector's configuration
        /// </summary>
        public FormatDetectorConfiguration Configuration { get; }

        /// <summary>
        /// Constructs detector with specified configuration and detector pluging
        /// </summary>
        /// <param name="configuration">Detector's configuration</param>
        /// <param name="generalFormats">General format parsers</param>
        /// <param name="textBasedFormats">Text-based format parsers</param>
        /// <exception cref="ArgumentNullException">Thrown if null collection is provided</exception>
        public FormatDetector(FormatDetectorConfiguration configuration, IFormatDetector[] generalFormats, ITextBasedFormatDetector[] textBasedFormats)
        {
            Configuration = configuration;

            if (generalFormats == null)
                throw new ArgumentNullException(nameof(generalFormats));

            _textBasedFormatDetectors = textBasedFormats ?? throw new ArgumentNullException(nameof(textBasedFormats));

            var formatsWithMandatorySignature = generalFormats.Where(f => f.HasSignature && f.SignatureIsMandatory);
            var formatsWithOptionalOrMissingSignature = generalFormats.Where(f => !f.HasSignature || f.HasSignature && !f.SignatureIsMandatory);

            // Reordered collection of format detectors
            _generalFormatDetectors = formatsWithMandatorySignature.Concat(formatsWithOptionalOrMissingSignature).ToArray();

            _hasFomatsWithSignature = _generalFormatDetectors.Any(f => f.HasSignature);
            _maxSignatureLength = _hasFomatsWithSignature ? _generalFormatDetectors.Where(f => f.HasSignature).Max(f => f.BytesToReadSignature) : 0;
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
                        recognitionResult = await TryDetectGeneralFormat(path, file, cancellationToken);

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

        /// <summary>
        /// Detect format without any knowledge about content of the file
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="stream">Opened file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Recognized file or null if none of the detectors recognized format</returns>
        private async Task<FileRecognitionResult?> TryDetectGeneralFormat(string path, Stream stream, CancellationToken cancellationToken)
        {
            Memory<byte> signature;

            if (_hasFomatsWithSignature)
            {
                byte[] buffer = new byte[_maxSignatureLength];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                stream.Seek(0, SeekOrigin.Begin);
                signature = buffer.AsMemory(0, bytesRead);
            }
            else
            {
                signature = new Memory<byte>();
            }

            FileRecognitionResult? recognitionResult = await Detect(path, stream, signature, _generalFormatDetectors, cancellationToken);

            return recognitionResult;
        }

        /// <summary>
        /// Run specified collection of detectors
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="stream">Opened file</param>
        /// <param name="signature">First N bytes of the file</param>
        /// <param name="detectors">Collection of detectors</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Recognized file or null if none of the detectors recognized format</returns>
        private async Task<FileRecognitionResult?> Detect(string path, Stream stream, Memory<byte> signature, IEnumerable<IFormatDetector> detectors, CancellationToken cancellationToken)
        {
            FileRecognitionException? fileRecognitionException = null;

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
                catch (IOException)
                {
                    throw; //Cannot handle io problems here
                }
                catch (UnauthorizedAccessException)
                {
                    throw; //Cannot handle io problems here
                }
                catch (FileFormatException ex)
                {
                    fileRecognitionException = new FileRecognitionException(format, FaultType.FileFormatError, ex.Message, ex);
                }
                catch (Exception ex)
                {
                    fileRecognitionException = new FileRecognitionException(format, FaultType.DetectorInternalError, ex.Message, ex);
                }
            }

            //File is considered faulted if any detector threw an exception and other detectors haven't parsed file
            if (fileRecognitionException != null)
                return FileRecognitionResult.Faulted(path, fileRecognitionException);
            else
                return null;
        }

        /// <summary>
        /// Try recognize text-based formats
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="stream">Opened file</param>
        /// <param name="textFormatSummary">Information about text encoding</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Recognized text format or null if none of the detectors recognized format</returns>
        private async Task<FileRecognitionResult?> TryDetectTextBasedFormat(string path, Stream stream, TextFormatSummary textFormatSummary, CancellationToken cancellationToken)
        {
            FileRecognitionException? fileRecognitionException = null;

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
                catch (IOException)
                {
                    throw; //Cannot handle io problems here
                }
                catch (UnauthorizedAccessException)
                {
                    throw; //Cannot handle io problems here
                }
                catch (FileFormatException ex)
                {
                    fileRecognitionException = new FileRecognitionException(format, FaultType.FileFormatError, ex.Message, ex);
                }
                catch (Exception ex)
                {
                    fileRecognitionException = new FileRecognitionException(format, FaultType.DetectorInternalError, ex.Message, ex);
                }
            }

            //File is considered faulted if any detector threw an exception and other detectors haven't parsed file
            if (fileRecognitionException != null)
                return FileRecognitionResult.Faulted(path, fileRecognitionException);
            else
                return null;
        }
    }
}
