namespace Juke.Web.Core.Render;

public interface IPage : IComponent {
    string Title { get; }
    string Language { get; }
        

    void InjectDependencies(
        IReadOnlyList<IWebResource> resources, 
        IReadOnlyList<InlineScript> scripts
    );
}