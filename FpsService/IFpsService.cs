namespace Game.Runtime.Services.Analytics.FpsService
{
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;

    public interface IFpsService : IGameService
    {
        IReadOnlyReactiveProperty<float> CurrentFps { get; }
    }
}