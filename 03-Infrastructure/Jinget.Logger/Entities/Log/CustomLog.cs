using Jinget.Core.Attributes;
using Jinget.Logger.Entities.Log.Base;

namespace Jinget.Logger.Entities.Log;

[Entity(ElasticSearchEnabled = true)]
public class CustomLog : LogBaseEntity
{
    public string CallerFilePath { get; set; }
    public long CallerLineNumber { get; set; }
    public string CallerMember { get; set; }
}
