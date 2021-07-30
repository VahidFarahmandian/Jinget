using System.Data;

namespace Jinget.Extensions.Database.SqlClient
{
    public static class IDbConnectionExtensions
    {
        public static void SafeOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
            connection.Open();
        }
    }
}
