/* Juke.Web.Tests/PageHandlerTests.cs */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Juke.Web.Core;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Render;

namespace Juke.Web.Tests;

// --- Заглушки для тестов ---

public class DummyResourceComponent : Component 
{
    public override ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        writer.Write("<div>Dummy</div>");
        return ValueTask.CompletedTask;
    }

    public override IReadOnlyList<IWebResource> GetResources() => [
        new DummyWebResource("css/test.css", WebResourceType.Css, "1.0")
    ];

    public override IReadOnlyList<InlineScript> GetInlineScripts() => [
        new InlineScript("init-script", "console.log('init');", ScriptPosition.DOMContentLoaded)
    ];
}

public class DummyWebResource : IWebResource 
{
    public string RelativePath { get; }
    public WebResourceType Type { get; }
    public string VersionHash { get; }

    public DummyWebResource(string path, WebResourceType type, string version) {
        RelativePath = path; Type = type; VersionHash = version;
    }
    public Task<Stream> OpenStreamAsync() => Task.FromResult(Stream.Null);
}

public class DummyPage : Component, IPage 
{
    public string Title => "Test Page";
    public string Language => "en";
    
    public bool InjectResourcesCalled { get; private set; }
    public int InjectedResourcesCount { get; private set; }
    public int InjectedScriptsCount { get; private set; }

    public void InjectResources(IReadOnlyList<IWebResource> resources, IReadOnlyList<InlineScript> scripts) {
        InjectResourcesCalled = true;
        InjectedResourcesCount = resources.Count;
        InjectedScriptsCount = scripts.Count;
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
        // Симулируем компоновку (Unidirectional Data Flow)
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
        var context = new MockHttpContext { 
            Request = new MockHttpRequest { Method = Method.GET, Path = "/" } 
        };

        // Act
        await handler.HandleAsync(context);

        // Assert: Проверяем, что фаза инъекции была вызвана
        Assert.True(handler.PageInstance.InjectResourcesCalled);
        
        // Assert: Проверяем, что ресурсы от дочерних компонентов дошли до корня (Page)
        Assert.Equal(1, handler.PageInstance.InjectedResourcesCount);
        Assert.Equal(1, handler.PageInstance.InjectedScriptsCount);

        // Assert: Проверяем, что рендер записал данные в поток (StreamWriter отработал)
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var resultHtml = await reader.ReadToEndAsync();
        
        Assert.Equal("<html>Test</html>", resultHtml);
    }
}