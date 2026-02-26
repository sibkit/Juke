/* Juke.Web.Tests/PageHandlerTests.cs */
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;
using StringContent = Juke.Web.Core.Assets.StringContent;

namespace Juke.Web.Tests;

// --- Заглушки для тестов ---

public class DummyResourceComponent : Component 
{
    public override ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        writer.Write("<div>Dummy</div>");
        return ValueTask.CompletedTask;
    }

    // Возвращаем плоский список ресурсов по-новому!
    public override IReadOnlyList<IAsset> GetAssets() => [
        new ExternalAsset("css/test.css", new StringContent(StringContentType.Css, "")),
        new InlineAsset("init-script", InlinePosition.DOMContentLoaded, new StringContent(StringContentType.Js, "console.log('init');"))
    ];
}

public class DummyPage : Component, IPage 
{
    public string Title => "Test Page";
    public string Language => "en";
    
    public bool InjectResourcesCalled { get; private set; }
    public int InjectedExternalCount { get; private set; }
    public int InjectedInlineCount { get; private set; }

    public void InjectAssets(IReadOnlyList<ExternalAsset> externalAssets, IReadOnlyList<InlineAsset> inlineAssets) {
        InjectResourcesCalled = true;
        InjectedExternalCount = externalAssets.Count;
        InjectedInlineCount = inlineAssets.Count;
    }

    public override ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        writer.Write("<html>Test</html>");
        return ValueTask.CompletedTask;
    }
}

public class TestPageHandler : PageHandler 
{
    public DummyPage PageInstance { get; } = new DummyPage();

    protected override ValueTask<IPage> CreatePageAsync(IHttpContext context) 
    {
        // Симулируем компоновку
        var child = new DummyResourceComponent();
        PageInstance.AddChild(child);
        
        return ValueTask.FromResult<IPage>(PageInstance);
    }
}

// --- Тесты ---

public class PageHandlerTests 
{
    [Fact]
    public async Task HandleAsync_ExecutesPipeline_AndInjectsResources() 
    {
        // Arrange
        var handler = new TestPageHandler();
        
        // Подготавливаем DI-контейнер для PageHandler (ему нужен AssetRegistry)
        var services = new DummyServiceProvider();
        services.AddService(new AssetRegistry());

        var context = new MockHttpContext { 
            Request = new MockHttpRequest { Method = Method.GET, Path = "/" },
            RequestServices = services // Прокидываем сервисы
        };

        // Act
        await handler.HandleAsync(context);

        // Assert: Проверяем, что фаза инъекции была вызвана
        Assert.True(handler.PageInstance.InjectResourcesCalled);
        
        // Assert: Проверяем, что Pattern Matching правильно разложил ассеты по корзинам
        Assert.Equal(1, handler.PageInstance.InjectedExternalCount);
        Assert.Equal(1, handler.PageInstance.InjectedInlineCount);

        // Assert: Проверяем, что рендер записал данные в поток
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var resultHtml = await reader.ReadToEndAsync();
        
        Assert.Equal("<html>Test</html>", resultHtml);
    }
}