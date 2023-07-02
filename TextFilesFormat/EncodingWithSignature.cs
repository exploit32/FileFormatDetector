using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class EncodingWithSignature
    {
        public Signature Signature { get; }

        public int CodePage { get; }

        public string EncodingName { get; }

        public string EncodingFullName { get; }

        public EncodingWithSignature(Signature signature, int codePage, string encodingName, string encodingFullName)
        {
            Signature = signature;
            CodePage = codePage;
            EncodingName = encodingName;
            EncodingFullName = encodingFullName;
        }
    }
}
