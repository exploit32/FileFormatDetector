using BenchmarkDotNet.Running;
using System.Text;

namespace Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var report1 = BenchmarkRunner.Run<EncodingStagesBenchmark>();
            var report2 = BenchmarkRunner.Run<TextDetectionBenchmarks>();
        }
    }
}