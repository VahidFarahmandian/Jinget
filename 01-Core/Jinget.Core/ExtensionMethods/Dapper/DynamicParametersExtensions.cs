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
            List<object> lstValues = new();

            var t = parameters.GetType().GetField("parameters", BindingFlags.NonPublic | BindingFlags.Instance);

            if (t == null)
                return lstValues;
            foreach (DictionaryEntry dictionaryEntry in (IDictionary)t.GetValue(parameters))
            {
                var dbType = (DbType)dictionaryEntry.Value?.GetType().GetProperty("DbType")?.GetValue(dictionaryEntry.Value);
                if (dbType.IsBooleanType())
                    lstValues.Add(parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) == true ? 1 : 0);
                else if (dbType.IsNumericType())
                    lstValues.Add(parameters.Get<dynamic>(dictionaryEntry.Key.ToString()));
                else if (dbType.IsUnicodeType())
                    lstValues.Add("N'" + parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) + "'");
                else
                    lstValues.Add("'" + parameters.Get<dynamic>(dictionaryEntry.Key.ToString()) + "'");
            }

            return lstValues;
        }
    }
}