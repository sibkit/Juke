using Juke.Accessing;

namespace Juke;

public class Database {
    public Database(IDatabaseDriver driver) {
       Driver = driver;
    }

    private IDatabaseDriver Driver { get; }

}