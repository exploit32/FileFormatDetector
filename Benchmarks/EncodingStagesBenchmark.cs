using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Benchmarks
{
    public class EncodingStagesBenchmark
    {
        private const int BufferSize = 10 * 1024 * 1024;

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
                return await detector.TryDetectEncoding(stream, null, CancellationToken.None);
            }
        }

        public IEnumerable<object[]> SkipStagesTestData()
        {
            yield return new object[] { SampleStrings.GetEncodedText(SampleStrings.EnglishText, BufferSize, Encoding.ASCII), false, false, false, false };
            yield return new object[] { SampleStrings.GetEncodedText(SampleStrings.EnglishText, BufferSize, Encoding.ASCII), true, false, false, false };
            yield return new object[] { SampleStrings.GetEncodedText(SampleStrings.EnglishText, BufferSize, Encoding.ASCII), false, true, false, false };
            yield return new object[] { SampleStrings.GetEncodedText(SampleStrings.EnglishText, BufferSize, Encoding.ASCII), false, false, true, false };
            yield return new object[] { SampleStrings.GetEncodedText(SampleStrings.EnglishText, BufferSize, Encoding.ASCII), false, false, false, true };
        }
    }
}
