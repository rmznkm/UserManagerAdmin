using System.Data;

namespace EventBus.Storage
{
    public interface IDbStorage : IDisposable
    {
        void Open();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        IDbConnection GetStorageConnection();
        bool InTransaction();
        IDbTransaction GetStorageTransaction();
        void Close();
        Task<int> ExecuteAsync(string sql);
        Task<int> ExecuteAsync(string sql, object param);
        Task<IDataReader> ExecuteReaderAsync(string sql);
        Task<IDataReader> ExecuteReaderAsync(string sql, object param);
    }
}
