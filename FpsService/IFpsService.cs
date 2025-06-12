namespace Game.Runtime.Services.Analytics.FpsService
{
    using R3;
    using UniGame.GameFlow.Runtime;

    public interface IFpsService : IGameService
    {
        ReadOnlyReactiveProperty<float> CurrentFps { get; }
    }
}