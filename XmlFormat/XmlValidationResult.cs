namespace XmlFormat
{
    /// <summary>
    /// Internal representation of xml validation result
    /// </summary>
    internal class XmlValidationResult
    {
        /// <summary>
        /// Indicates that xml file is valid
        /// </summary>
        public bool Valid { get; init; }

        /// <summary>
        /// Encoding found in xml declaration node (if present)
        /// </summary>
        public string? XmlDeclarationEncoding { get; init; }
    }
}
