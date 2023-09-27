using System.Collections.Generic;

namespace Jinget.Core.Types
{
    public class HealthCheckResponseModel
    {
        public string Status { get; set; }
        public string Duration { get; set; }
        public ICollection<Entry> Entries { get; set; }

        public class Entry
        {
            public string Key { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
            public string[] Tags { get; set; }
            public string Duration { get; set; }
            public string Exception { get; set; }
        }
    }
}