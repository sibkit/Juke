namespace Juke.Database;

public interface IConnection {
    void Close();
    bool IsOpen();
}