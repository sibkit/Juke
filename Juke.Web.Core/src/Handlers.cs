namespace Juke.Web.Core;

public interface IHandler{}

public interface IRequestHandler: IHandler {
    void Handle(IHttpContext context);
}

public interface IErrorHandler: IHandler {
    void Handle(IHttpContext context, Exception? exception);
}