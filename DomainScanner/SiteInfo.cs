using System;

namespace DomainScanner;

public record SiteInfo(Uri Uri, string Title)
{
    public override string ToString()
    {
        return Title != "" ? $"{Uri} ({Title})" : Uri.ToString();
    }
};