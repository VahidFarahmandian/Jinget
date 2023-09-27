namespace Jinget.Core.IOptionTypes.ConnectionString
{
    /// <summary>
    /// If you need to use multiple databases(principal db, log db, app db etc) 
    /// then you can define a class for each type and make it inherit from `BaseDBConnectionString`
    /// </summary>
    public class PrincipalDBConnectionString: BaseDBConnectionString
    {
    }
}
