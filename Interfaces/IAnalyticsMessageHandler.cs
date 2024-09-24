namespace Game.Runtime.Services.Analytics.Interfaces
{
    using Cysharp.Threading.Tasks;

    public interface IAnalyticsMessageHandler
    {
        UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message);
    }
}