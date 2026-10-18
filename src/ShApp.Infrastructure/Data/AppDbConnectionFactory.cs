using SQLite;

namespace ShApp.Infrastructure.Data;

public class AppDbConnectionFactory
{
    public SQLiteAsyncConnection Connection { get; }

    public AppDbConnectionFactory(string databasePath)
    {
        Connection = new SQLiteAsyncConnection(databasePath);
    }
}
