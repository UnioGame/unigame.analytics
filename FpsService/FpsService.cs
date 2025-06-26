namespace Game.Runtime.Services.Analytics.FpsService
{
    using System;
    using System.Linq;
    using R3;
    using UniGame.GameFlow.Runtime;
    using UnityEngine;

    [Serializable]
    public class FpsService : GameService, IFpsService
    {
        [SerializeField]
        private int bufferSize = 5;
        [SerializeField]
        private int skipFrame  = 20;

        public static float Fps;
        
        public ReadOnlyReactiveProperty<float> CurrentFps { get; }

        public FpsService()
        {
            CurrentFps = new ReactiveProperty<float>().AddTo(LifeTime);
            CurrentFps = Observable.EveryUpdate()
                .Select(_ => Time.unscaledDeltaTime)
                .Chunk(bufferSize,skipFrame)
                .Select(x => 1 / x.Average())
                .Do(x => Fps = x)
                .ToReadOnlyReactiveProperty();
        }
    }
}
