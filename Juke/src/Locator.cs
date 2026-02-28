using Juke.ServiceLocation;

namespace Juke;

public static class Locator {
    
    internal class ServiceDescriptor {
        public ServiceLifetime Lifetime { get; }
        public Func<IServiceLocator, object> Factory { get; }
        public object? SingletonInstance { get; set; }
        public object SyncLock { get; } = new object();

        public ServiceDescriptor(ServiceLifetime lifetime, Func<IServiceLocator, object> factory) {
            Lifetime = lifetime;
            Factory = factory;
        }
    }

    private static readonly Dictionary<Type, ServiceDescriptor> _descriptors = new();
    private static bool _isFrozen = false;
    private static readonly IServiceLocator _root = new RootLocator();
    
    public static void Register<T>(ServiceLifetime lifetime, IServiceCreator<T> creator) where T : notnull {
        if (_isFrozen) throw new InvalidOperationException("Locator is frozen!");
        _descriptors[typeof(T)] = new ServiceDescriptor(lifetime, loc => creator.Create(loc));
    }
    
    public static void Register<T>(ServiceLifetime lifetime, Func<IServiceLocator, T> factory) where T : notnull {
        if (_isFrozen) throw new InvalidOperationException("Locator is frozen!");
        _descriptors[typeof(T)] = new ServiceDescriptor(lifetime, loc => factory(loc));
    }
    
    public static void AddTransient<T>(Func<IServiceLocator, T> factory) where T : notnull => Register(ServiceLifetime.Transient, factory);
    public static void AddScoped<T>(Func<IServiceLocator, T> factory) where T : notnull => Register(ServiceLifetime.Scoped, factory);
    public static void AddSingleton<T>(Func<IServiceLocator, T> factory) where T : notnull => Register(ServiceLifetime.Singleton, factory);

    public static void AddTransient<T>(IServiceCreator<T> creator) where T : notnull => Register(ServiceLifetime.Transient, creator);
    public static void AddScoped<T>(IServiceCreator<T> creator) where T : notnull => Register(ServiceLifetime.Scoped, creator);
    public static void AddSingleton<T>(IServiceCreator<T> creator) where T : notnull => Register(ServiceLifetime.Singleton, creator);

    public static void Freeze() => _isFrozen = true;

    public static T Get<T>() => _root.Get<T>();

    public static IScope CreateScope() => new DefaultScope();

    class RootLocator : IServiceLocator {
        public T Get<T>() {
            if (_descriptors.TryGetValue(typeof(T), out var descriptor)) {
                
                if (descriptor.Lifetime == ServiceLifetime.Scoped) {
                    throw new InvalidOperationException($"Cannot resolve Scoped service {typeof(T).Name} from Root locator.");
                }

                if (descriptor.Lifetime == ServiceLifetime.Singleton) {
                    if (descriptor.SingletonInstance == null) {
                        lock (descriptor.SyncLock) {
                            descriptor.SingletonInstance ??= descriptor.Factory(this);
                        }
                    }
                    return (T)descriptor.SingletonInstance;
                }

                // Transient
                return (T)descriptor.Factory(this);
            }

            throw new InvalidOperationException($"Service {typeof(T).Name} not registered in Root ServiceLocator!");
        }
    }

    class DefaultScope : IScope {
        private readonly Dictionary<Type, object> _scopedInstances = new();
        public IServiceLocator? Fallback { get; set; }

        T IServiceLocator.Get<T>() {

            if (_scopedInstances.TryGetValue(typeof(T), out var instance)) return (T)instance;
            if (_descriptors.TryGetValue(typeof(T), out var descriptor)) {
                if (descriptor.Lifetime == ServiceLifetime.Singleton) {
                    return _root.Get<T>();
                }

                var newInstance = (T)descriptor.Factory(this);
                
                if (descriptor.Lifetime == ServiceLifetime.Scoped) {
                    _scopedInstances[typeof(T)] = newInstance;
                }

                return newInstance;
            }

            return Fallback != null ? 
                Fallback.Get<T>() : 
                throw new InvalidOperationException($"Service {typeof(T).Name} not registered in ServiceLocator and no Fallback handled it!");
        }

        public void CacheInstance(Type type, object instance) => _scopedInstances[type] = instance;

        public bool TryGetInstance(Type type, out object instance) => _scopedInstances.TryGetValue(type, out instance!);

        public void Dispose() {
            var disposables = _scopedInstances.Values.OfType<IDisposable>().ToList();
            foreach (var disposable in disposables) {
                disposable.Dispose();
            }
            _scopedInstances.Clear();
        }
    }
}