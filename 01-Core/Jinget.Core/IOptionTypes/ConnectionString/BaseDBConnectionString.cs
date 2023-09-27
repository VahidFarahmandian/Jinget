namespace Jinget.Core.IOptionTypes.ConnectionString
{
    public class BaseDBConnectionString
    {
        /// <summary>
        /// queries using this connection string can change data
        /// </summary>
        public string CommandDatabase { get; set; }

        /// <summary>
        /// queries using this connection string should not change data
        /// If Always On is used, then `ApplicationIntent=ReadOnly` can be added to this connection string
        /// </summary>
        public string QueryDatabase { get; set; }
    }
}
