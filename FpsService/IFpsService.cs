namespace UniGame.Runtime.Analytics.FpsService
{
    using R3;
    using UniGame.GameFlow.Runtime;

    public interface IFpsService : IGameService
    {
        ReadOnlyReactiveProperty<float> CurrentFps { get; }
    }
}