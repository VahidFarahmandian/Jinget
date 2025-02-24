namespace Jinget.Core.ExtensionMethods.Dapper;

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
        List<dynamic> lstValues = []; 
        var parametersField = parameters.GetType().GetField("parameters", BindingFlags.NonPublic | BindingFlags.Instance);

        if (parametersField == null)
            return lstValues;

        if (parametersField.GetValue(parameters) is IDictionary parameterDictionary) 
        {
            foreach (DictionaryEntry dictionaryEntry in parameterDictionary)
            {
                var parameterValue = dictionaryEntry.Value;
                if (parameterValue != null)
                {
                    var dbTypeValue = parameterValue.GetType().GetProperty("DbType")?.GetValue(parameterValue);
                    if (dbTypeValue is DbType dbType) 
                    {
                        var parameterKeyValue = dictionaryEntry.Key.ToString();
                        if (parameterKeyValue != null)
                        {
                            var parameterValueFromParameters = parameters.Get<dynamic>(parameterKeyValue);

                            if (dbType.IsBooleanDbType())
                                lstValues.Add(parameterValueFromParameters == true ? 1 : 0);
                            else if (dbType.IsNumericDbType())
                                lstValues.Add(parameterValueFromParameters);
                            else if (dbType.IsUnicodeDbType())
                                lstValues.Add("N'" + parameterValueFromParameters + "'");
                            else
                                lstValues.Add("'" + parameterValueFromParameters + "'");
                        }
                    }
                }
            }
        }
        return lstValues;
    }
}