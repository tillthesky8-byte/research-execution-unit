namespace Contracts.Interfaces;

public interface IComissionModel
{
    decimal CalculateCommission(decimal price, decimal quantity);
}