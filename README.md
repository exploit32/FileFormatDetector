# FileFormatDetector

Tool for file formats detection. Each format is a separate plugin.

App can detect:
* Text and XML files. Supported encodings: ASCII, Windows-1251, UTF8, UTF16 little endian/big endian, UTF32 little endian/big endian
* PE files. Detected: architecture, 32/64bit, endianness, presence of managed header
* Elf files. Detected: architecture, 32/64bit, endianness, interpreter
* Mach-O files. Detected: architecture, 32/64bit, endianness, presence of signature

App detects formats and prints summary like this:
```
98 - Text: no-BOM, ASCII
27 - Xml
20 - Unknown
15 - Text: no-BOM, utf-8
10 - Text: BOM, utf-8
6 - Text: BOM, utf-16LE
5 - Text: no-BOM, Windows-125x
4 - ELF: AMD64, 64bit-LE, /lib64/ld-linux-x86-64.so.2
4 - Mach-O: X86_64, 64bit-LE, unsigned
4 - Text: BOM, utf-16BE
3 - PE: I386, 32bit-LE, Unmanaged
3 - ELF: ARM, 32bit-LE
3 - Text: no-BOM, utf-16BE
```

# How to build?
This project uses project [binary-samples](https://github.com/JonathanSalwan/binary-samples) as a source of executable examples.

```bash
git clone --recurse-submodules https://github.com/exploit32/FileFormatDetector.git

cd FileFormatDetector

dotnet build
```
# How to run tests?

```bash
dotnet test
```
