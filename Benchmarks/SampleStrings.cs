using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    internal class SampleStrings
    {
        public const string EnglishText = @"ASCII was developed from telegraph code. Its first commercial use was as a seven-bit teleprinter code promoted by Bell data services.
Work on the ASCII standard began in May 1961, with the first meeting of the American Standards Association's (ASA) (now the American National Standards Institute or ANSI) X3.2 subcommittee.
The first edition of the standard was published in 1963, underwent a major revision during 1967,[6][7] and experienced its most recent update during 1986.[8]
Compared to earlier telegraph codes, the proposed Bell code and ASCII were both ordered for more convenient sorting (i.e., alphabetization) of lists and added features for devices other than teleprinters.[8]
The use of ASCII format for Network Interchange was described in 1969.[9] That document was formally elevated to an Internet Standard in 2015.[10]
Originally based on the (modern) English alphabet, ASCII encodes 128 specified characters into seven-bit integers as shown by the ASCII chart above.[11] Ninety-five of the encoded characters are printable: these include the digits 0 to 9, lowercase letters a to z, uppercase letters A to Z, and punctuation symbols. In addition, the original ASCII specification included 33 non-printing control codes which originated with Teletype machines; most of these are now obsolete,[12] although a few are still commonly used, such as the carriage return, line feed, and tab codes.
For example, lowercase i would be represented in the ASCII encoding by binary 1101001 = hexadecimal 69 (i is the ninth letter) = decimal 105. ";

        public const string RussianText = @"Словарный запас Эллочки-людоедки, как известно, насчитывал 30 слов.
А Лев Николаевич Толстой пользовался примерно 105 тысячами. Это сопоставимо со словником Словаря современного русского литературного языка, который издавался в 1948–1965 годах.
Среди них встречаются очень редкие и необычные. Предлагаем вашему вниманию подборку интересных слов, знание которых поможет лучше понимать Толстого и чувствовать русский язык.";

        public const string MixedText = @"UCS (англ. Universal Coded Character Set) представляет собой стандартный набор символов, определённый международным стандартом ISO/IEC 10646,
который является основой многих символьных кодировок. UCS содержит чуть более 128 000 абстрактных символов, как и в Unicode 9.0,
каждый из которых определяется однозначно сочетанием имени и целого числа (так называемый «кодовый пункт»).";

        /// <summary>
        /// Create string of size at least N bytes
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="bufferSize">Desired buffer size</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Encoded string</returns>
        public static byte[] GetEncodedText(string text, int bufferSize, Encoding encoding)
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
