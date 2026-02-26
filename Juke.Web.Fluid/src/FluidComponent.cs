/* Juke.Web.Fluid/FluidComponent.cs */
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Juke.Web.Core;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Juke.Web.Fluid;

public abstract class FluidComponent : Component 
{
    // Делаем метод доступным для FluidPage, чтобы переиспользовать тег {% render %}
    public static FluidParser CreateParser()
    {
        var parser = new FluidParser();
        
        // Регистрируем тег для Zero-Allocation потокового рендеринга дочерних компонентов
        parser.RegisterExpressionTag("render", async (expression, writer, encoder, templateContext) => 
        {
            var fluidValue = await expression.EvaluateAsync(templateContext);
            
            if (fluidValue.ToObjectValue() is IComponent component) 
            {
                if (templateContext.AmbientValues.TryGetValue("HttpContext", out var ctxObj) && 
                    ctxObj is IHttpContext httpContext)
                {
                    await component.RenderAsync(writer, httpContext);
                }
            }
            
            return Completion.Normal;
        });

        return parser;
    }

    private static readonly FluidParser _parser = CreateParser();
    private static readonly ConcurrentDictionary<Type, IFluidTemplate> _templateCache = new();

    protected abstract string GetTemplate();
    
    // QoL Улучшение: по умолчанию моделью выступает сам класс компонента!
    protected virtual object? GetModel() => this; 
    
    protected virtual void ConfigureTemplateContext(TemplateContext context) { }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context) 
    {
        var componentType = GetType();

        // 1. Берем скомпилированный шаблон из кэша (O(1))
        if (!_templateCache.TryGetValue(componentType, out var template)) 
        {
            if (_parser.TryParse(GetTemplate(), out template, out var error)) {
                _templateCache.TryAdd(componentType, template);
            } else {
                throw new InvalidOperationException($"Fluid parse error in {componentType.Name}: {error}");
            }
        }

        // 2. Формируем контекст данных
        var model = GetModel();
        var templateContext = model != null ? new TemplateContext(model) : new TemplateContext();
        
        // 3. Прокидываем HTTP-контекст для дочерних компонентов
        templateContext.AmbientValues["HttpContext"] = context;
        
        ConfigureTemplateContext(templateContext);

        // 4. Рендерим напрямую в TextWriter (который привязан к сетевому сокету)
        await template.RenderAsync(writer, HtmlEncoder.Default, templateContext);
    }
}