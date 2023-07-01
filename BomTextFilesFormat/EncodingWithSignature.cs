using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomTextFilesFormat
{
    internal class EncodingWithSignature
    {
        public Signature Signature { get; }

        public int CodePage { get; }

        public EncodingWithSignature(Signature signature, int codePage)
        {
            Signature = signature;
            CodePage = codePage;
        }
    }
}
