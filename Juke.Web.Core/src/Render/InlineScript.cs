namespace Juke.Web.Core.Render;

public enum ScriptPosition {
    Head,
    BodyEnd,
    DOMContentLoaded
}

public class InlineScript {
    public string Id { get; }
    public string Content { get; }
    public ScriptPosition Position { get; }

    public InlineScript(string id, string content, ScriptPosition position = ScriptPosition.DOMContentLoaded) {
        // Защита контракта: Id и Content не могут быть пустыми!
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        Id = id;
        Content = content;
        Position = position;
    }
}