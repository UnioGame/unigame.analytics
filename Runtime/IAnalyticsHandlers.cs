namespace Game.Runtime.Services.Analytics.Runtime
{
    using System;
    using System.Collections.Generic;
    using Interfaces;

    public interface IAnalyticsHandlers
    {
        IDisposable RegisterMessageHandler(IAnalyticsMessageHandler handler);
        void UpdateHandlers(IEnumerable<IAnalyticsMessageHandler> messageHandlers);
    }
}