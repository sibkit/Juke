using System.Text;
using Juke.Web.Core.Assets;
using StringContent = Juke.Web.Core.Assets.StringContent;

namespace Juke.Web.Core.Render;

public interface IPage : IComponent {
    string Title { get; }
    string Language { get; }
        

    void InjectAssets(
        IReadOnlyList<ExternalAsset> externalAssets, 
        IReadOnlyList<InlineAsset> inlineAssets
    );
}

public abstract class Page : Component, IPage {
    public virtual string Title { get; set; } = string.Empty;
    public virtual string Language { get; set; } = "en";

    // Делаем свойства публичными, чтобы шаблонизаторы могли их прочитать
    public string HeadAssetsHtml { get; private set; } = string.Empty;
    public string BodyAssetsHtml { get; private set; } = string.Empty;

    public virtual void InjectAssets(IReadOnlyList<ExternalAsset> externalAssets, IReadOnlyList<InlineAsset> inlineAssets) {
        var asm = typeof(Page).Assembly; // Сборка Juke.Web.Core
        var headBuilder = new StringBuilder();
        var bodyBuilder = new StringBuilder();
        var domReadyScripts = new StringBuilder();

        // Загружаем шаблоны (они уже в кэше, поэтому это O(1))
        var cssLinkTpl = AssemblyResources.GetTemplate("templates/css-link.html", asm);
        var jsLinkTpl = AssemblyResources.GetTemplate("templates/js-link.html", asm);
        var inlineCssTpl = AssemblyResources.GetTemplate("templates/inline-css.html", asm);
        var inlineJsTpl = AssemblyResources.GetTemplate("templates/inline-js.html", asm);
        var domReadyTpl = AssemblyResources.GetTemplate("templates/dom-ready.html", asm);

        // 1. Внешние ссылки
        foreach (var asset in externalAssets) {
            if (asset.Content is StringContent str) {
                if (str.Type == StringContentType.Css) {
                    cssLinkTpl.RenderTo(headBuilder, p => p switch {
                        "Path" => asset.RelativePath,
                        "Version" => asset.VersionHash,
                        _ => ""
                    });
                    headBuilder.AppendLine();
                } else if (str.Type == StringContentType.Js) {
                    jsLinkTpl.RenderTo(bodyBuilder, p => p switch {
                        "Path" => asset.RelativePath,
                        "Version" => asset.VersionHash,
                        _ => ""
                    });
                    bodyBuilder.AppendLine();
                }
            }
        }

        // 2. Инлайн скрипты и стили
        foreach (var asset in inlineAssets) {
            var contentText = asset.Content.Text;

            if (asset.Content.Type == StringContentType.Css) {
                inlineCssTpl.RenderTo(headBuilder, p => p == "Styles" ? contentText : "");
            } else if (asset.Content.Type == StringContentType.Js) {
                if (asset.Position == InlinePosition.DOMContentLoaded) {
                    domReadyScripts.AppendLine(contentText);
                } else {
                    var target = asset.Position == InlinePosition.Head ? headBuilder : bodyBuilder;
                    inlineJsTpl.RenderTo(target, p => p == "Scripts" ? contentText : "");
                }
            }
        }

        // 3. Обертка для DOMContentLoaded
        if (domReadyScripts.Length > 0) {
            domReadyTpl.RenderTo(bodyBuilder, p => p == "Scripts" ? domReadyScripts.ToString() : "");
        }

        HeadAssetsHtml = headBuilder.ToString();
        BodyAssetsHtml = bodyBuilder.ToString();
    }
}