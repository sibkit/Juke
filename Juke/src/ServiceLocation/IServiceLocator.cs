namespace Juke.ServiceLocation;

public interface IServiceLocator
{
    T Get<T>();
}