using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class SignatureTools
    {
        public static bool CheckSignature(ReadOnlySpan<byte> fileStart, Signature signature)
        {
            for (int i = 0; i < signature.Value.Length; i++)
            {
                if (fileStart[i + signature.Offset] != signature.Value[i])
                    return false;
            }

            return true;
        }

        public static bool CheckSignatures(ReadOnlySpan<byte> fileStart, Signature[] signatures)
        {
            bool success = false;

            for (int i = 0; i < signatures.Length; i++)
            {
                success |= CheckSignature(fileStart, signatures[i]);

                if (success)
                    break;
            }

            return success;
        }
    }
}
