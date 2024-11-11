using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data.TweenAnimationData;
using DG.Tweening;
using TMPro;
using UI.TweenAnimation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UI.View
{
    public enum BaseButtonStates { Normal, Hover, Pressed }
    [RequireComponent(typeof(Button))]
    public class BaseButtonView : UIStateAnimationController<BaseButtonStates, BaseButtonSettings, BaseButtonAnimationConfig>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public event Action OnButtonClicked;

        [SerializeField] private Button _button;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Image _shadow;
        [SerializeField] private Image _decorLines;
        [SerializeField] private TMP_Text _labelButton;
        private bool _isAnimating;

        private CancellationTokenSource _cancellationToken;

        private void OnEnable() => _cancellationToken = new CancellationTokenSource();
        private void OnDisable()
        {
            _cancellationToken.Cancel();
            _cancellationToken?.Dispose();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_button.interactable && !_isAnimating)
                PlayAnimation(BaseButtonStates.Hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_button.interactable && !_isAnimating)
                PlayAnimation(BaseButtonStates.Normal);
        }

        public async void OnPointerDown(PointerEventData eventData)
        {
            if (_button.interactable && !_isAnimating)
            {
                _isAnimating = true;
                try
                {
                    await PlayAnimationOnCompleted(BaseButtonStates.Pressed, OnButtonClicked, _cancellationToken.Token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Анимация была отменена.");
                }
                finally
                {
                    PlayAnimation(BaseButtonStates.Normal);
                    _isAnimating = false; 
                }
            }
            
        }
        
        protected override void InitializeSequences()
        {
            AnimationSequences = new Dictionary<BaseButtonStates, Sequence>()
            {
                { BaseButtonStates.Normal, CreateSequence(_config.Normal) },
                { BaseButtonStates.Hover, CreateSequence(_config.Hover) },
                { BaseButtonStates.Pressed, CreateSequence(_config.Pressed) },
            };
        }
        protected override Sequence CreateSequence(BaseButtonSettings stateConfig)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_root.DOScale(stateConfig.ScaleFactor, stateConfig.Params.Duration).SetLoops(2, LoopType.Yoyo));
            sequence.Join(_shadow.DOColor(stateConfig.ShadowColor, stateConfig.Params.Duration));
            sequence.Join(_decorLines.DOColor(stateConfig.DecorLinesColor, stateConfig.Params.Duration));
            sequence.Join(_labelButton.DOColor(stateConfig.LabelButtonColor, stateConfig.Params.Duration));
            sequence.SetDelay(stateConfig.Params.Delay);
            sequence.SetEase(stateConfig.Params.Ease);
            sequence.SetRecyclable(true);
            sequence.SetAutoKill(false);
            sequence.SetUpdate(true);
            sequence.Pause();
            return sequence;
        }
        protected override BaseButtonStates GetDefaultState() => BaseButtonStates.Normal;
    }
}