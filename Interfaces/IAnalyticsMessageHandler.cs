namespace UniGame.Runtime.Analytics.Interfaces
{
    using Cysharp.Threading.Tasks;

    public interface IAnalyticsMessageHandler
    {
        UniTask<IAnalyticsMessage> UpdateMessageAsync(IAnalyticsMessage message);
    }
}