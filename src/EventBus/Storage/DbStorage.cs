using Dapper;
using System.Data;

namespace EventBus.Storage
{
    public class DbStorage : IDbStorage
    {
        private readonly IDbConnectionProvider dbConnectionProvider;
        private IDbConnection connection;
        private IDbTransaction transaction;

        public DbStorage(IDbConnectionProvider dbConnectionProvider)
        {
            this.dbConnectionProvider = dbConnectionProvider;
        }

        private readonly object connectionLock = new object();

        protected IDbConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    lock (connectionLock)
                    {
                        if (connection == null)
                        {
                            connection = dbConnectionProvider.Connection;
                        }
                    }
                }
                return connection;
            }
        }

        private bool IsInTransaction => transaction != null;

        public bool InTransaction()
        {
            return IsInTransaction;
        }

        public void BeginTransaction()
        {
            Open();
            transaction = Connection.BeginTransaction();
        }

        public void Open()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public void Close()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        public void CommitTransaction()
        {
            if (!IsInTransaction)
            {
                throw new InvalidOperationException("Db not in any trancation");
            }
            transaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (!IsInTransaction)
            {
                throw new InvalidOperationException("Db not in any trancation");
            }
            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
            if (connection != null)
            {
                connection.Close();
            }
        }

        public IDbConnection GetStorageConnection()
        {
            return Connection;
        }

        public IDbTransaction GetStorageTransaction()
        {
            return transaction;
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Dispose();
            }
            transaction = null;
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public Task<int> ExecuteAsync(string sql)
        {
            return Connection.ExecuteAsync(sql);
        }

        public Task<int> ExecuteAsync(string sql, object param)
        {
            return Connection.ExecuteAsync(sql, param);
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql)
        {
            return Connection.ExecuteReaderAsync(sql);
        }

        public Task<IDataReader> ExecuteReaderAsync(string sql, object param)
        {
            return Connection.ExecuteReaderAsync(sql, param);
        }
    }
}
