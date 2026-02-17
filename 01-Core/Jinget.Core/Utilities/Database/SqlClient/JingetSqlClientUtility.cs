using Jinget.Core.ExtensionMethods.Database.SqlClient;

using Microsoft.Data.SqlClient;

using System.Threading;
namespace Jinget.Core.Utilities.Database.SqlClient;

public class JingetSqlClientUtility
{
    public JingetSqlClientUtility(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));
        ConnectionString = connectionString;
    }

    private string ConnectionString { get; }

    /// <summary>
    /// Gets or sets the wait time (in seconds) before terminating the attempt to execute
    /// a command and generating an error. The default is 60 seconds.
    /// </summary>
    public int CommandTimeout { get; set; } = 60;

    private async Task<SqlCommand> CreateCommandAsync(string command)
    {
        var sqlConnection = new SqlConnection(ConnectionString);
        await sqlConnection.SafeOpenAsync();
        var sqlCommand = new SqlCommand(command, sqlConnection)
        {
            CommandType = CommandType.Text,
            CommandTimeout = CommandTimeout
        };
        return sqlCommand;
    }

    public async Task<int> ExecuteNonQueryAsync(
        string command,
        SqlParameter[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var sqlCommand = await CreateCommandAsync(command);

        if (parameters?.Length > 0)
        {
            sqlCommand.Parameters.AddRange(parameters);
        }

        return await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<DataTable> ExecuteQueryAsync(
        string command,
        SqlParameter[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable();

        await using var sqlCommand = await CreateCommandAsync(command);

        if (parameters?.Length > 0)
        {
            sqlCommand.Parameters.AddRange(parameters);
        }

        await using var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
        dataTable.Load(reader);

        return dataTable;
    }
}