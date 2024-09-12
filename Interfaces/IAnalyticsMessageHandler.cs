namespace Game.Runtime.Services.Analytics.Interfaces
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime;

    public interface IAnalyticsMessageHandler
    {
        UniTask Initialize(IContext context, ILifeTime lifeTime);

        UniTask<TMessage> UpdateEventAsync<TMessage>(TMessage message) where TMessage : IAnalyticsMessage;
    }
}