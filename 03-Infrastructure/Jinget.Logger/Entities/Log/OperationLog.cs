using Jinget.Core.Attributes;
using Jinget.Logger.Entities.Log.Base;

namespace Jinget.Logger.Entities.Log
{
    [Entity(ElasticSearchEnabled = true)]
    public class OperationLog : LogBaseEntity
    {
        public string Method { get; set; }

        public string Body { get; set; }

        public string Headers { get; set; }

        public string IP { get; set; }

        public bool IsResponse { get; set; }

        public string PageUrl { get; set; }

        public long ContentLength { get; set; }

        public string Detail { get; set; }

        public string UserName { get; set; }
    }
}