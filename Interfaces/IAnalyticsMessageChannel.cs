namespace Game.Runtime.Services.Analytics.Interfaces
{
    using System;

    public interface IAnalyticsMessageChannel :
        IObservable<IAnalyticsMessage>,
        IDisposable
    {
        void Publish(IAnalyticsMessage message);
    }
}