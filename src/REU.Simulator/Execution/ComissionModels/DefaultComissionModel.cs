
using Contracts.Interfaces;
namespace Simulator.Execution.ComissionModels;
public class DefaultComissionModel : IComissionModel
{
    public decimal CalculateCommission(decimal price, decimal quantity) => 0; // No commission applied, return 0

}