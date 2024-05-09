using Jinget.Core.Attributes;
using Jinget.Logger.Entities.Log.Base;

namespace Jinget.Logger.Entities.Log;

[Entity(ElasticSearchEnabled = true)]
public class ErrorLog : LogBaseEntity
{
    public string Severity { get; set; }
}