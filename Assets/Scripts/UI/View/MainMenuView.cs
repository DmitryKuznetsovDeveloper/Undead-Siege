using UI.View;
using UnityEngine;

public sealed class MainMenuView : MonoBehaviour
{
    public BaseButtonView StartButton => _startButton;
    public BaseButtonView SettingButton => _settingButton;
    public BaseButtonView LoadingButton => _loadingButton;
    public BaseButtonView ExitButton => _exitButton;
    
    [SerializeField] private BaseButtonView _startButton;
    [SerializeField] private BaseButtonView _settingButton;
    [SerializeField] private BaseButtonView _loadingButton;
    [SerializeField] private BaseButtonView _exitButton;
}
