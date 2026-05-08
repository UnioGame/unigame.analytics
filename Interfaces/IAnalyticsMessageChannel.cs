namespace UniGame.Runtime.Analytics.Interfaces
{
    using System;

    public interface IAnalyticsMessageChannel :
        IObservable<IAnalyticsMessage>,
        IDisposable
    {
        void Publish(IAnalyticsMessage message);
    }
}