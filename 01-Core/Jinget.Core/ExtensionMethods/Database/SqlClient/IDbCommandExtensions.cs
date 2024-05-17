namespace Jinget.Core.ExtensionMethods.Database.SqlClient;

public static class IDbCommandExtensions
{
    /// <summary>
    /// Replace Arabic ي and ك characters in CommandText and Parameters with their Farsi equalivants
    /// </summary>
    public static void ApplyCorrectYeKe(this IDbCommand command)
    {
        command.CommandText = command.CommandText.ApplyCorrectYeKe();

        foreach (IDataParameter parameter in command.Parameters)
        {
            if (parameter.Value is DBNull)
                continue;

            switch (parameter.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    if (parameter.Value != null)
                        parameter.Value = parameter.Value.ToString().ApplyCorrectYeKe();
                    break;
            }
        }
    }
}
