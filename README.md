# FileFormatDetector

![Build status](https://github.com/exploit32/FileFormatDetector/actions/workflows/dotnet.yml/badge.svg)

Tool for file formats detection. Each format is a separate plugin.

App can detect:
* Text and XML files. Supported encodings: ASCII, Windows-1251, UTF8, UTF16 little endian/big endian, UTF32 little endian/big endian
* PE files. Detected: architecture, 32/64bit, endianness, presence of managed header
* Elf files. Detected: architecture, 32/64bit, endianness, interpreter
* Mach-O files. Detected: architecture, 32/64bit, endianness, presence of signature

App detects formats and prints summary like this:
```
98 - Text: no-BOM, ASCII
27 - Xml: no-BOM, UTF-8
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
1 - ELF: S390, 64bit-BE, /lib/ld64.so.1
33 - Unknown
```

# How to build?
This project uses project [binary-samples](https://github.com/JonathanSalwan/binary-samples) as a source of executable examples.

```bash
git clone --recurse-submodules https://github.com/exploit32/FileFormatDetector.git

cd FileFormatDetector

dotnet build
```

# How to run?

```FileFormatDetector.Console.exe [Options] (List of files or directories to scan)```

| Parameter | Meaning | Example |
| ----------| ------- | ------- |
| ```--help```  | Show help | |
| ```--threads N```  | Number of parallel threads (default is number of CPU cores) | ```--threads 1``` |
| ```--no-recursion```  | Scan directories non recursively |  |
| ```--verbose```  | Print summary about each file individually |  |
| ```--validate-full-xml```  | If set, XML documents will be validated completely |  |
| ```--file-scan-size-limit N```  | Number of bytes to scan to detect encoding of text files without BOM | ```--file-scan-size-limit 4096```  |

## Examples

```./FileFormatDetector.Console.exe --help```

```./FileFormatDetector.Console.exe --threads 1 /home/user/documents /home/user/Downloads --file-scan-size-limit 4096```

```./FileFormatDetector.Console.exe --no-recursion /home/user/Downloads --validate-full-xml```

```.\FileFormatDetector.Console.exe --no-recursion C:\Users\Konstantin\Documents```

# How to run tests?

```bash
dotnet test
```

# How to run benchmarks?

```bash
dotnet run --configuration Release --project Benchmarks
```
