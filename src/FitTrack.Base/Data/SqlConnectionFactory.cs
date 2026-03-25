using System.Data.Common;
using FitTrack.Core.Interfaces.Data;

namespace FitTrack.Base.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    public SqlConnectionFactory(string connectionString)
    {
    }

    public async Task<DbConnection> CreateOpenConnectionAsync()
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
