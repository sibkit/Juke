namespace Juke.ServiceLocation;

public interface IServiceCreator<out T>
{
    T Create(IServiceLocator locator);
}