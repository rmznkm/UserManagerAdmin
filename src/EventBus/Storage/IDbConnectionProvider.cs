using System.Data;

namespace EventBus.Storage
{
    public interface IDbConnectionProvider
    {
        IDbConnection Connection { get; }
    }
}
