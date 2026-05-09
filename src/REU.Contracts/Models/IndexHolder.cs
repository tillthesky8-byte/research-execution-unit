namespace Contracts.Models;

public sealed class IndexHolder(IndexEntry[] entries)
{
    public IndexEntry[] Entries { get; init; } = entries;
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}