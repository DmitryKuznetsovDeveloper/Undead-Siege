using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TweenParams = Utils.TweenParams;
namespace UI.View
{
    public class LoadingScreenView : MonoBehaviour , ILoadingScreenView
    {
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private Slider _progress;
        [SerializeField] private TweenParams _params;
        
        public void SetProgress(float loadOperationProgress) => _progress.DOValue(loadOperationProgress, _params.Duration).Kill();
        
        public void SetLabel(float loadOperationProgress) => _progressLabel.text = $"{loadOperationProgress:F0}%";
        
        public void ShowWindow() => gameObject.SetActive(true);
        
        public void HideWindow() => gameObject.SetActive(false);
    }
}