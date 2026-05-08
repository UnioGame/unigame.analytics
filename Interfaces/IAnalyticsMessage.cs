namespace UniGame.Runtime.Analytics.Interfaces
{
    using System.Collections.Generic;

    public interface IAnalyticsMessage
    {
        string GroupId { get; }
        string Name { get; }
        Dictionary<string, string> Parameters { get; }

        string this[string key] { get; set; }
    }
}