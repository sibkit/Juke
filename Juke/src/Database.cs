using Juke.Accessing;

namespace Juke;

public class Database {
    public Database(IDbDriver driver) {
       Driver = driver;
    }

    public IDbDriver Driver { get; }

    private readonly List<Session> _activeSessions = [];

    public IReadOnlyList<Session> ActiveSessions => _activeSessions;

    public Session CreateSession() {
        var result = new Session(Driver.createConnection(), this);
        _activeSessions.Add(result);
        return result;
    }

    internal void RemoveSession(Session session) {
        _activeSessions.Remove(session);
    }
}