namespace UI.View
{
    public interface ILoadingScreenView : IPopupView
    {
        void SetProgress(float loadOperationProgress);
        void SetLabel(float percent);
    }
    public interface IPopupView
    {
        void ShowWindow();
        void HideWindow();
    }
}