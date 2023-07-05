using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Benchmarks
{
    public class TextDetectionBenchmarks
    {
        private const string asciiText = @"ASCII was developed from telegraph code. Its first commercial use was as a seven-bit teleprinter code promoted by Bell data services.[when?] Work on the ASCII standard began in May 1961, with the first meeting of the American Standards Association's (ASA) (now the American National Standards Institute or ANSI) X3.2 subcommittee. The first edition of the standard was published in 1963,[4][5] underwent a major revision during 1967,[6][7] and experienced its most recent update during 1986.[8] Compared to earlier telegraph codes, the proposed Bell code and ASCII were both ordered for more convenient sorting (i.e., alphabetization) of lists and added features for devices other than teleprinters.[8]
The use of ASCII format for Network Interchange was described in 1969.[9] That document was formally elevated to an Internet Standard in 2015.[10]
Originally based on the (modern) English alphabet, ASCII encodes 128 specified characters into seven-bit integers as shown by the ASCII chart above.[11] Ninety-five of the encoded characters are printable: these include the digits 0 to 9, lowercase letters a to z, uppercase letters A to Z, and punctuation symbols. In addition, the original ASCII specification included 33 non-printing control codes which originated with Teletype machines; most of these are now obsolete,[12] although a few are still commonly used, such as the carriage return, line feed, and tab codes.
For example, lowercase i would be represented in the ASCII encoding by binary 1101001 = hexadecimal 69 (i is the ninth letter) = decimal 105. ";

        [Benchmark]
        [ArgumentsSource(nameof(SampleData))]
        public void DetectAsciiText(byte[] buffer)
        {
            NonBomEncodingDetector detector = new NonBomEncodingDetector();
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                var encoding = detector.TryDetectEncoding(stream, null);
            }
        }

        public IEnumerable<object> SampleData()
        {
            yield return Encoding.ASCII.GetBytes(asciiText);
        }
    }
}
