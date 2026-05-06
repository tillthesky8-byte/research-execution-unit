using Contracts.Models;

namespace Contracts.Interfaces;

public interface IRecorder
{
    public void Record(DateTime timestamp, decimal equity);
    public SimulationResult BuildResult();
}