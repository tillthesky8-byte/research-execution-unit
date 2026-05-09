namespace Contracts.Interfaces;

public interface IManager
{
    void SaveSeries<T>(IEnumerable<T> series, string name);
    void SaveObject<T>(T obj, string objectName);
}