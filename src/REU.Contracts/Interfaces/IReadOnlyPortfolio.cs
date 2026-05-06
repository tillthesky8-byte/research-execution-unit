using Contracts.Models;
using Contracts.Enums;

namespace Contracts.Interfaces;

public interface IReadOnlyPortfolio
{
    IReadOnlyDictionary<string, Position> Positions { get; }
    decimal Cash { get; }
}