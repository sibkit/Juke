namespace Juke.Web.Core.Assets;

public interface IContent {}

public enum StringContentType { Css, Js, Html, Other }

public enum BinaryContentType { Image, Font, Other }

public class StringContent : IContent {
    public StringContentType Type { get; }
    public string Text { get; }

    public StringContent(StringContentType type, string text) {
        Type = type;
        Text = text;
    }
}

public class BinaryContent : IContent {
    public BinaryContentType Type { get; }
    public ReadOnlyMemory<byte> Data { get; }

    public BinaryContent(BinaryContentType type, ReadOnlyMemory<byte> data) {
        Type = type;
        Data = data;
    }
}