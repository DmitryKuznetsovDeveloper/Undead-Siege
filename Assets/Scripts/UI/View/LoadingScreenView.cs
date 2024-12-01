using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UI.View;

public sealed class LoadingScreenView : MonoBehaviour, ILoadingScreenView
{
    [SerializeField] private Slider _progress; // Слайдер прогресса
    [SerializeField] private TMP_Text _progressLabel; // Текст прогресса
    [SerializeField] private CanvasGroup _canvasGroup; // Для управления видимостью
    [SerializeField] private float _fadeDuration = 0.5f; // Время для анимации появления/скрытия

    public void ShowWindow()
    {
        //Костыль на время =)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.InOutQuad);
    }

    public void HideWindow()
    {
        _canvasGroup.DOFade(0f, _fadeDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void SetProgress(float loadOperationProgress)
    {
        _progress.DOKill(); // Убираем предыдущую анимацию
        _progress.DOValue(loadOperationProgress, 0.3f); // Обновляем прогресс
    }

    public void SetLabel(float percent) => _progressLabel.text = $"{Mathf.FloorToInt(percent)}%";
}