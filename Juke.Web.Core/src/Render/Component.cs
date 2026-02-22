namespace Juke.Web.Core.Render;

public interface IComponent {
    // Phase 1: Two-step lifecycle (Database queries, state preparation, etc.)
    ValueTask InitAsync(IHttpContext context);
        
    // Phase 2: Zero-allocation stream writing.
    ValueTask RenderAsync(TextWriter writer, IHttpContext context);
        
    // CSS/JS dependencies required by this component.
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

    public virtual async ValueTask InitAsync(IHttpContext context) {
        await OnInitAsync(context);

        if (_children != null) {
            foreach (var t in _children) {
                await t.InitAsync(context);
            }
        }

        await OnAfterInitAsync(context);
    }

    protected virtual ValueTask OnInitAsync(IHttpContext context) {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask OnAfterInitAsync(IHttpContext context) {
        return ValueTask.CompletedTask;
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