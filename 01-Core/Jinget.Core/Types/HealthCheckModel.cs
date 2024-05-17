namespace Jinget.Core.Types;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
