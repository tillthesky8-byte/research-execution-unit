using Contracts.Models;
namespace Contracts.Interfaces;

public interface IReadOnlyPortfolio
{
    IReadOnlyDictionary<string, Position> Positions { get; }
    decimal Cash { get; }
}