using BenchmarkDotNet.Attributes;
using FormatApi;
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
        private const int BufferSize = 10 * 1024 * 1024 / 7;

        [Benchmark]
        [ArgumentsSource(nameof(EncodingsTestData))]
        public async Task<List<FormatSummary?>> DetectEncoding(StringsCollection stringsCollection)
        {
            TextFilesDetector detector = new TextFilesDetector();
            
            List<FormatSummary?> formats = new List<FormatSummary?>();

            using (MemoryStream stream = new MemoryStream(stringsCollection.AsciiText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Windows125xText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Utf8Text))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Utf16BeText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Utf16LeText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Utf32BeText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            using (MemoryStream stream = new MemoryStream(stringsCollection.Utf32LeText))
                formats.Add(await detector.ReadFormat(stream, CancellationToken.None));

            return formats;
        }

        public IEnumerable<StringsCollection> EncodingsTestData()
        {
            yield return MakeStringCollection(SampleStrings.EnglishText, BufferSize, true, "English");
            yield return MakeStringCollection(SampleStrings.MixedText, BufferSize, true, "Russian");
            yield return MakeStringCollection(SampleStrings.RussianText, BufferSize, true, "Mixed");
        }

        private StringsCollection MakeStringCollection(string text, int size, bool includeAscii, string description)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            return new StringsCollection()
            {
                AsciiText = includeAscii ? 
                SampleStrings.GetEncodedText(text, size, Encoding.ASCII) 
                : SampleStrings.GetEncodedText(text, size, Encoding.GetEncoding(1251)),
                Windows125xText = SampleStrings.GetEncodedText(text, size, Encoding.GetEncoding(1251)),
                Utf8Text = SampleStrings.GetEncodedText(text, size, new UTF8Encoding(false)),
                Utf16BeText = SampleStrings.GetEncodedText(text, size, new UnicodeEncoding(true, false)),
                Utf16LeText = SampleStrings.GetEncodedText(text, size, new UnicodeEncoding(false, false)),
                Utf32BeText = SampleStrings.GetEncodedText(text, size, new UTF32Encoding(true, false)),
                Utf32LeText = SampleStrings.GetEncodedText(text, size, new UTF32Encoding(false, false)),
                Description = description,
            };
        }

        public class StringsCollection
        {
            public byte[] AsciiText { get; init; } = Array.Empty<byte>();

            public byte[] Windows125xText { get; init; } = Array.Empty<byte>();

            public byte[] Utf8Text { get; init; } = Array.Empty<byte>();

            public byte[] Utf16LeText { get; init; } = Array.Empty<byte>();

            public byte[] Utf16BeText { get; init; } = Array.Empty<byte>();

            public byte[] Utf32LeText { get; init; } = Array.Empty<byte>();

            public byte[] Utf32BeText { get; init; } = Array.Empty<byte>();

            public string Description { get; init; } = string.Empty;

            public override string ToString()
            {
                long size = AsciiText.Length + Windows125xText.Length + Utf8Text.Length + Utf16BeText.Length + Utf16LeText.Length + Utf32BeText.Length + Utf32LeText.Length;
                return $"{Description}, {size/1024}kb";
            }
        }
    }
}
