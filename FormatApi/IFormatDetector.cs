namespace FormatApi
{
    /// <summary>
    /// Interface of general format detectors
    /// </summary>
    public interface IFormatDetector
    {
        /// <summary>
        /// Indicates that file of this format has a mandatory byte sequence identifies this format (sometimes called magic)
        /// </summary>
        bool HasSignature { get; }

        /// <summary>
        /// Indicates that file must contain signature
        /// </summary>
        bool SignatureIsMandatory { get; }

        /// <summary>
        /// Number of first bytes in the file to read Signature
        /// </summary>
        /// <see cref="HasSignature"/>
        int BytesToReadSignature { get; }

        /// <summary>
        /// Description of format detector
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Try to find signature in given file beginning
        /// </summary>
        /// <param name="fileStart">First bytes of the file</param>
        /// <returns>True in signature is found, otherwise False</returns>
        bool CheckSignature(ReadOnlySpan<byte> fileStart);

        /// <summary>
        /// Read entire file
        /// </summary>
        /// <param name="stream">File to detect format</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>FormatSummary if file was successfully parsed, otherwise null</returns>
        Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken);
    }
}