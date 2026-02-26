/* Juke.Web.Fluid/FluidPage.cs */
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Juke.Web.Core;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Juke.Web.Fluid;

public abstract class FluidPage : Page 
{
    // Парсер и кэш (можем переиспользовать те же, что и во FluidComponent)
    private static readonly FluidParser _parser = FluidComponent.CreateParser();
    private static readonly ConcurrentDictionary<Type, IFluidTemplate> _templateCache = new();

    protected abstract string GetTemplate();
    protected virtual object? GetModel() => this; // По умолчанию модель — это сама страница!

    // Пробрасываем системные свойства в движок
    protected virtual void ConfigureTemplateContext(TemplateContext context) 
    {
        context.SetValue("Title", Title);
        context.SetValue("Language", Language);
        context.SetValue("HeadAssetsHtml", HeadAssetsHtml);
        context.SetValue("BodyAssetsHtml", BodyAssetsHtml);
    }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context) 
    {
        var componentType = GetType();

        if (!_templateCache.TryGetValue(componentType, out var template)) 
        {
            if (_parser.TryParse(GetTemplate(), out template, out var error)) {
                _templateCache.TryAdd(componentType, template);
            } else {
                throw new InvalidOperationException($"Fluid parse error in {componentType.Name}: {error}");
            }
        }

        var model = GetModel();
        var templateContext = model != null ? new TemplateContext(model) : new TemplateContext();
        
        templateContext.AmbientValues["HttpContext"] = context;
        ConfigureTemplateContext(templateContext);

        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }
}