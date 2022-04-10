using EventBus.Options;
using Npgsql;
using System.Data;

namespace EventBus.Storage.Npsql
{
    public class NpsqlDbConnectionProvider : IDbConnectionProvider
    {
        private readonly IDbOptions dbOptions;

        public NpsqlDbConnectionProvider(IDbOptions dbOptions)
        {
            this.dbOptions = dbOptions;
        }
        public IDbConnection Connection => new NpgsqlConnection(dbOptions.ConnectionString);
    }
}
