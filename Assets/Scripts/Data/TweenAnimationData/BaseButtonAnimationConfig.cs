using System;
using UnityEngine;
using Utils;
namespace Data.TweenAnimationData
{
    [CreateAssetMenu(fileName = "BaseButtonAnimation", menuName = "Configs/Animations/BaseButtonAnimation", order = 0)]
    public class BaseButtonAnimationConfig : ScriptableObject
    {
        public BaseButtonSettings Normal;
        public BaseButtonSettings Hover;
        public BaseButtonSettings Pressed;
    }


    [Serializable]
    public class BaseButtonSettings
    {
        public Color ShadowColor;
        public Color DecorLinesColor;
        public Color LabelButtonColor;
        [Range(0, 2)] public float ScaleFactor;
        public TweenParams Params;
    }
}