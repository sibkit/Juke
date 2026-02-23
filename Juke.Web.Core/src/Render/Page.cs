namespace Juke.Web.Core.Render;

public interface IPage : IComponent {
    string Title { get; }
    string Language { get; }
        

    void InjectResources(
        IReadOnlyList<IWebResource> resources, 
        IReadOnlyList<InlineScript> scripts
    );
}