namespace Juke.ServiceLocation;

public interface IScope : IServiceLocator, IDisposable
{
    IServiceLocator? Fallback { get; set; }
    void CacheInstance(Type type, object instance);
    bool TryGetInstance(Type type, out object instance);
}