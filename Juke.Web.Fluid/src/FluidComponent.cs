using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using Fluid;
using Juke.Web.Core;
using Juke.Web.Core.Render;

namespace Juke.Web.Fluid;

public abstract class FluidComponent : Component {
    private static readonly FluidParser _parser = new FluidParser();
    private static readonly ConcurrentDictionary<Type, IFluidTemplate> _templateCache = new();

    protected abstract string GetTemplate();

    protected virtual object? GetModel() => null;

    protected virtual void ConfigureTemplateContext(TemplateContext context) { }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        var model = GetModel();
        await RenderCachedTemplateAsync(writer, model);
    }

    protected async ValueTask RenderCachedTemplateAsync(TextWriter writer, object? model) {
        var componentType = GetType();

        if (!_templateCache.TryGetValue(componentType, out var template)) {
            var templateSource = GetTemplate();
            if (_parser.TryParse(templateSource, out template, out var error)) {
                _templateCache.TryAdd(componentType, template);
            } else {
                throw new InvalidOperationException($"Fluid parse error in {componentType.Name}: {error}");
            }
        }

        var templateContext = model != null ? new TemplateContext(model) : new TemplateContext();
        ConfigureTemplateContext(templateContext);

        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }
}