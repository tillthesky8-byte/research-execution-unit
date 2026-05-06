namespace Contracts.Interfaces;

public interface IFactory<T>
{
    T Create(params object[] args);
}