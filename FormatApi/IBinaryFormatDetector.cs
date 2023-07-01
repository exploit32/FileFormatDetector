﻿namespace FormatApi
{
    public interface IBinaryFormatDetector
    {
        bool HasSignature { get; }

        int BytesToReadSignature { get; }

        string Description { get; }

        bool CheckSignature(ReadOnlySpan<byte> fileStart);

        FormatSummary? ReadFormat(Stream stream);
    }
}