using PEFormat;
using PEFormat.Structs;

namespace Tests.Pe
{
    public class FormatTests
    {
        [Fact]
        public void X64App()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("pe-Windows-x64-cmd");
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<PEFormatSummary>(formatSummary);

            var peFormatSummary = (PEFormatSummary)formatSummary;

            Assert.Equal(Machine.AMD64.ToString(), peFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, peFormatSummary.Endianness);
            Assert.Equal(64, peFormatSummary.Bits);
            Assert.False(peFormatSummary.HasClrHeader);
        }

        [Fact]
        public void i386App()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("pe-Windows-x86-cmd");
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<PEFormatSummary>(formatSummary);

            var peFormatSummary = (PEFormatSummary)formatSummary;

            Assert.Equal(Machine.I386.ToString(), peFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, peFormatSummary.Endianness);
            Assert.Equal(32, peFormatSummary.Bits);
            Assert.False(peFormatSummary.HasClrHeader);
        }

        [Fact]
        public void ArmApp()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("pe-Windows-ARMv7-Thumb2LE-HelloWorld");
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<PEFormatSummary>(formatSummary);

            var peFormatSummary = (PEFormatSummary)formatSummary;

            Assert.Equal(Machine.ARMNT.ToString(), peFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, peFormatSummary.Endianness);
            Assert.Equal(32, peFormatSummary.Bits);
            Assert.False(peFormatSummary.HasClrHeader);
        }

        [Fact]
        public void i386App2()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("pe-cygwin-ls.exe");
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<PEFormatSummary>(formatSummary);

            var peFormatSummary = (PEFormatSummary)formatSummary;

            Assert.Equal(Machine.I386.ToString(), peFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, peFormatSummary.Endianness);
            Assert.Equal(32, peFormatSummary.Bits);
            Assert.False(peFormatSummary.HasClrHeader);
        }

        [Fact]
        public void i386App3()
        {
            //Arrange
            var stream = Utilities.GetBinaryStream("pe-mingw32-strip.exe");
            PEFormatDetector detector = new PEFormatDetector();

            //Act
            var formatSummary = detector.ReadFormat(stream);

            //Assert
            Assert.NotNull(formatSummary);
            Assert.IsType<PEFormatSummary>(formatSummary);

            var peFormatSummary = (PEFormatSummary)formatSummary;

            Assert.Equal(Machine.I386.ToString(), peFormatSummary.Architecture);
            Assert.Equal(FormatApi.Endianness.LittleEndian, peFormatSummary.Endianness);
            Assert.Equal(32, peFormatSummary.Bits);
            Assert.False(peFormatSummary.HasClrHeader);
        }

        [Fact]
        public void MalformedPE()
        {
            //Arrange
            byte[] file = new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00 };

            PEFormatDetector detector = new PEFormatDetector();

            using (var stream = new MemoryStream(file))
            {
                //Act
                //Assert
                Assert.Throws<FormatException>(() => detector.ReadFormat(stream));
            }
        }
    }
}
