namespace Juke.Web.Core.Render;

public interface IComponent {
    
    ValueTask RenderAsync(TextWriter writer, IHttpContext context);
    
    IReadOnlyList<IWebResource> GetResources();
    IReadOnlyList<InlineScript> GetInlineScripts();
}

public abstract class Component : IComponent {
    private List<IComponent>? _children;

    public IReadOnlyList<IComponent> Children => _children ?? (IReadOnlyList<IComponent>)Array.Empty<IComponent>();

    public void AddChild(IComponent child) {
        _children ??= [];
        _children.Add(child);
    }
    
    public abstract ValueTask RenderAsync(TextWriter writer, IHttpContext context);

    // Zero-allocation defaults. Array.Empty<T> is a singleton array under the hood.
    public virtual IReadOnlyList<IWebResource> GetResources() {
        return [];
    }

    public virtual IReadOnlyList<InlineScript> GetInlineScripts() {
        return [];
    }
}