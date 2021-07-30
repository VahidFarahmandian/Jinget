using System.Data;

namespace Jinget.Extensions
{
    public static class DbTypeExtensions
    {
        public static bool IsNumericType(this DbType type)
        {
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.Binary:
                case DbType.Currency:
                case DbType.Date:
                case DbType.DateTime:
                case DbType.Guid:
                case DbType.Object:
                case DbType.String:
                case DbType.Time:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Check if the given type is an unicode string type such as 'nchar' or 'nvarchar'
        /// </summary>
        public static bool IsUnicodeType(this DbType type) => type == DbType.String;

        /// <summary>
        /// Check if the given type is a non-unicode string type such as 'char' or 'varchar'
        /// </summary>
        public static bool IsNonUnicodeType(this DbType type) => type == DbType.AnsiString;

    }
}
