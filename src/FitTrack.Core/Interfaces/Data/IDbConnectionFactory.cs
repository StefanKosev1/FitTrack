using System.Data.Common;

namespace FitTrack.Core.Interfaces.Data;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync();
}
