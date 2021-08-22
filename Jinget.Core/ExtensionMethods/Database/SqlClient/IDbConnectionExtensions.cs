using System.Data;

namespace Jinget.Core.ExtensionMethods.Database.SqlClient
{
    public static class IDbConnectionExtensions
    {
        /// <summary>
        /// Opens a connection to database safely
        /// </summary>
        /// <remarks>This method first check if the connection state is already equal to <seealso cref="ConnectionState.Closed"/> or not, if not, 
        /// first it tries to Close the connection and then reopen it</remarks>
        public static void SafeOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
            connection.Open();
        }
    }
}
