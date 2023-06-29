using System.Data;

namespace Jinget.Core.ExtensionMethods.Enums
{
    public static class DbTypeExtensions
    {
        /// <summary>
        /// Check if given <seealso cref="DbType"/> is a numeric type or not
        /// </summary>
        public static bool IsNumericType(this DbType type)
        {
        #if NET5_0_OR_GREATER
            return type switch
            {
                DbType.AnsiString
                or DbType.Binary
                or DbType.Currency
                or DbType.Date
                or DbType.DateTime
                or DbType.Guid
                or DbType.Object
                or DbType.String
                or DbType.Time
                or DbType.AnsiStringFixedLength
                or DbType.StringFixedLength
                or DbType.Xml
                or DbType.DateTime2
                or DbType.DateTimeOffset => false,
                _ => true,
            };
        #else
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
        #endif
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