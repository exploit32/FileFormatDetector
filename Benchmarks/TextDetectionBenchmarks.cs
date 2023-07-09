using BenchmarkDotNet.Attributes;
using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;
using static System.Net.Mime.MediaTypeNames;

namespace Benchmarks
{
    public class TextDetectionBenchmarks
    {
        private const string englishText = @"ASCII was developed from telegraph code. Its first commercial use was as a seven-bit teleprinter code promoted by Bell data services.[when?] Work on the ASCII standard began in May 1961, with the first meeting of the American Standards Association's (ASA) (now the American National Standards Institute or ANSI) X3.2 subcommittee. The first edition of the standard was published in 1963,[4][5] underwent a major revision during 1967,[6][7] and experienced its most recent update during 1986.[8] Compared to earlier telegraph codes, the proposed Bell code and ASCII were both ordered for more convenient sorting (i.e., alphabetization) of lists and added features for devices other than teleprinters.[8]
The use of ASCII format for Network Interchange was described in 1969.[9] That document was formally elevated to an Internet Standard in 2015.[10]
Originally based on the (modern) English alphabet, ASCII encodes 128 specified characters into seven-bit integers as shown by the ASCII chart above.[11] Ninety-five of the encoded characters are printable: these include the digits 0 to 9, lowercase letters a to z, uppercase letters A to Z, and punctuation symbols. In addition, the original ASCII specification included 33 non-printing control codes which originated with Teletype machines; most of these are now obsolete,[12] although a few are still commonly used, such as the carriage return, line feed, and tab codes.
For example, lowercase i would be represented in the ASCII encoding by binary 1101001 = hexadecimal 69 (i is the ninth letter) = decimal 105. ";

        private const string russianText = "Словарный запас Эллочки-людоедки, как известно, насчитывал 30 слов. А Лев Николаевич Толстой пользовался примерно 105 тысячами. Это сопоставимо со словником Словаря современного русского литературного языка, который издавался в 1948–1965 годах. Среди них встречаются очень редкие и необычные. Предлагаем вашему вниманию подборку интересных слов, знание которых поможет лучше понимать Толстого и чувствовать русский язык.";

        private const string mixedText = "UCS (англ. Universal Coded Character Set) представляет собой стандартный набор символов, определённый международным стандартом ISO/IEC 10646, который является основой многих символьных кодировок. UCS содержит чуть более 128 000 абстрактных символов, как и в Unicode 9.0, каждый из которых определяется однозначно сочетанием имени и целого числа (так называемый «кодовый пункт»).";

        private const int BufferSize = 10 * 1024 * 1024;

        [Benchmark]
        [ArgumentsSource(nameof(EncodingsTestData))]
        public async Task<FormatSummary?> DetectEncoding(byte[] buffer, string encoding)
        {
            TextFilesDetector detector = new TextFilesDetector();
            
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return await detector.ReadFormat(stream, null);
            }
        }

        [Benchmark]
        [ArgumentsSource(nameof(SkipStagesTestData))]
        public async Task<object?> SkipEncodingStages(byte[] buffer, bool skipAscii, bool skipUtf8, bool skipUtf16, bool skipUtf32)
        {
            NonBomEncodingDetector detector = new NonBomEncodingDetector()
            {
                SkipAsciiCheck = skipAscii,
                SkipUtf8Check = skipUtf8,
                SkipUtf16Check = skipUtf16,
                SkipUtf32Check = skipUtf32,
            };

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                return await detector.TryDetectEncoding(stream, null);
            }
        }

        public IEnumerable<object[]> EncodingsTestData()
        {
            yield return GetText(englishText, BufferSize, Encoding.ASCII);

            yield return GetText(russianText, BufferSize, new UTF8Encoding(false));
            yield return GetText(mixedText, BufferSize, new UTF8Encoding(false));

            yield return GetText(englishText, BufferSize, new UnicodeEncoding(true, false));
            yield return GetText(russianText, BufferSize, new UnicodeEncoding(true, false));
            yield return GetText(mixedText, BufferSize, new UnicodeEncoding(true, false));

            yield return GetText(englishText, BufferSize, new UnicodeEncoding(false, false));
            yield return GetText(russianText, BufferSize, new UnicodeEncoding(false, false));
            yield return GetText(mixedText, BufferSize, new UnicodeEncoding(false, false));

            yield return GetText(englishText, BufferSize, new UTF32Encoding(true, false));
            yield return GetText(russianText, BufferSize, new UTF32Encoding(true, false));
            yield return GetText(mixedText, BufferSize, new UTF32Encoding(true, false));

            yield return GetText(englishText, BufferSize, new UTF32Encoding(false, false));
            yield return GetText(russianText, BufferSize, new UTF32Encoding(false, false));
            yield return GetText(mixedText, BufferSize, new UTF32Encoding(false, false));
        }


        public IEnumerable<object[]> SkipStagesTestData()
        {
            yield return new object[] { GetEncodedText(englishText, BufferSize, Encoding.ASCII), false, false, false, false };
            yield return new object[] { GetEncodedText(englishText, BufferSize, Encoding.ASCII), true, false, false, false };
            yield return new object[] { GetEncodedText(englishText, BufferSize, Encoding.ASCII), false, true, false, false };
            yield return new object[] { GetEncodedText(englishText, BufferSize, Encoding.ASCII), false, false, true, false };
            yield return new object[] { GetEncodedText(englishText, BufferSize, Encoding.ASCII), false, false, false, true };
        }

        private object[] GetText(string text, int bufferSize, Encoding encoding)
        {
            return new object[] { GetEncodedText(text, bufferSize, encoding), encoding.EncodingName };
        }

        private byte[] GetEncodedText(string text, int bufferSize, Encoding encoding)
        {
            byte[] bytes;

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream, encoding))
            {
                do
                {
                    writer.Write(text);
                } while (stream.Position < bufferSize);

                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
