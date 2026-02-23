/* Juke.Web.Fluid/FluidComponent.cs */
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Juke.Web.Core;
using Juke.Web.Core.Render;

namespace Juke.Web.Fluid;

public abstract class FluidComponent : Component {
    private static readonly FluidParser _parser = CreateParser();
    private static readonly ConcurrentDictionary<Type, IFluidTemplate> _templateCache = new();

    private static FluidParser CreateParser() {
        var parser = new FluidParser();

        parser.RegisterExpressionTag("render", async (expression, writer, encoder, templateContext) => {
            var fluidValue = await expression.EvaluateAsync(templateContext);

            if (fluidValue.ToObjectValue() is IComponent component) {
                if (templateContext.AmbientValues.TryGetValue("HttpContext", out var ctxObj) &&
                    ctxObj is IHttpContext httpContext) {
                    await component.RenderAsync(writer, httpContext);
                }
            }
            return Completion.Normal;
        });

        return parser;
    }

    protected abstract string GetTemplate();

    protected virtual object? GetModel() => null;

    protected virtual void ConfigureTemplateContext(TemplateContext context) { }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        var model = GetModel();
        await RenderCachedTemplateAsync(writer, model, context);
    }

    protected async ValueTask RenderCachedTemplateAsync(TextWriter writer, object? model, IHttpContext context) {
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

        templateContext.AmbientValues["HttpContext"] = context;

        ConfigureTemplateContext(templateContext);

        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }
}