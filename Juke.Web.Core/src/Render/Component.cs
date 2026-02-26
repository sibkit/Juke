using Juke.Web.Core.Assets;
using Juke.Web.Core.Http;

namespace Juke.Web.Core.Render;

public interface IComponent {
    
    ValueTask RenderAsync(TextWriter writer, IHttpContext context);
    
    IReadOnlyList<IAsset> GetAssets();
}

public abstract class Component : IComponent {
    private List<IComponent>? _children;
    // Ленивая инициализация списка дочерних элементов
    public IReadOnlyList<IComponent> Children => _children ?? (IReadOnlyList<IComponent>)Array.Empty<IComponent>();
    public void AddChild(IComponent child) {
        _children ??= [];
        _children.Add(child);
    }
    public abstract ValueTask RenderAsync(TextWriter writer, IHttpContext context);
    public virtual IReadOnlyList<IAsset> GetAssets() => [];
}