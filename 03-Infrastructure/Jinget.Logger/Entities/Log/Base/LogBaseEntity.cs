using System;

namespace Jinget.Logger.Entities.Log.Base
{
    public abstract class LogBaseEntity : BaseEntity<int>
    {
        public DateTime When { get; set; }

        public string Url { get; set; }
        public string Description { get; set; }

        public string SubSystem { get; set; }

        public Guid RequestId { get; set; } = Guid.Empty;
    }
}