using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// Helper class with signature checking methods
    /// </summary>
    public static class SignatureTools
    {
        /// <summary>
        /// Check that start of file contains signature
        /// </summary>
        /// <param name="fileStart">Span of first N bytes of file</param>
        /// <param name="signature">Signature to look for</param>
        /// <returns>True is signature was found, otherwise false</returns>
        public static bool CheckSignature(ReadOnlySpan<byte> fileStart, Signature signature)
        {
            for (int i = 0; i < signature.Value.Length; i++)
            {
                if (fileStart[i + signature.Offset] != signature.Value[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check that start of file contains any of given signatures
        /// </summary>
        /// <param name="fileStart">Span of first N bytes of file</param>
        /// <param name="signatures">Signatures to look for</param>
        /// <returns>True if any signature was found</returns>
        public static bool CheckSignatures(ReadOnlySpan<byte> fileStart, Signature[] signatures)
        {
            bool success = false;

            for (int i = 0; i < signatures.Length; i++)
            {
                var signature = signatures[i];

                if (fileStart.Length >= signature.Offset + signature.Value.Length)
                {
                    success |= CheckSignature(fileStart, signature);

                    if (success)
                        break;
                }
            }

            return success;
        }
    }
}
