namespace Juke.ServiceLocation;

public class SimpleCreator<T> : IServiceCreator<T> where T : notnull {
    private readonly Func<IServiceLocator, T> _factory;
    
    public SimpleCreator(Func<IServiceLocator, T> factory) => _factory = factory;
    
    public T Create(IServiceLocator locator) => _factory(locator);
}