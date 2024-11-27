using System;
using DG.Tweening;
namespace Game.Helper
{
    [Serializable]
    public sealed class TweenParams
    {
        public float Duration;
        public float Delay;
        public Ease Ease;
    }
}