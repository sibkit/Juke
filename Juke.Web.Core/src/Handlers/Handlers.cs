namespace Juke.Web.Core.Handlers;

public interface IHandler{}

public interface IRequestHandler: IHandler {
    Task HandleAsync(IHttpContext context);
}

public interface IErrorHandler: IHandler {
    Task HandleAsync(IHttpContext context, Exception? exception);
}