using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using Jinget.Core.ExtensionMethods.Enums;

namespace Jinget.Core.ExtensionMethods.Dapper
{
    public static class DynamicParametersExtensions
    {
        /// <summary>
        /// Convert `DynamicParameters` to `Dictionary`
        /// </summary>
        public static Dictionary<string, dynamic> GetKeyValues(this DynamicParameters parameters) => parameters.ParameterNames.ToDictionary(param => param, param => parameters.Get<dynamic>(param));

        /// <summary>
        /// Convert `DynamicParameters` values to SQL values
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<dynamic> GetSQLValues(this DynamicParameters parameters)
        {
            List<object> lstValues = [];
            var t = parameters.GetType().GetField("parameters", BindingFlags.NonPublic | BindingFlags.Instance);

            if (t == null)
                return lstValues;
#pragma warning disable CS8604 // Possible null reference argument.

            foreach (DictionaryEntry dictionaryEntry in (IDictionary)t.GetValue(parameters))
            {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                var dbType = (DbType)dictionaryEntry.Value.GetValue("DbType");
#pragma warning restore CS8605 // Unboxing a possibly null value.
                if (dbType.IsBooleanDbType())
                    lstValues.Add(parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) == true ? 1 : 0);
                else if (dbType.IsNumericDbType())
                    lstValues.Add(parameters.Get<dynamic>(dictionaryEntry.Key.ToString()));
                else if (dbType.IsUnicodeDbType())
                    lstValues.Add("N'" + parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) + "'");
                else
                    lstValues.Add("'" + parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) + "'");
            }
#pragma warning restore CS8604 // Possible null reference argument.

            return lstValues;
        }
    }
}