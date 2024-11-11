using Cysharp.Threading.Tasks;
using UI.View;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName);
    }
    public sealed class SceneLoader : ISceneLoader
    {

        private readonly ILoadingScreenView _loadingScreenView;

        public SceneLoader(ILoadingScreenView loadingScreenView) => _loadingScreenView = loadingScreenView;

        public async UniTask LoadSceneAsync(string sceneName)
        {
            ShowLoadingScreen();

            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                Debug.Log($"Raw progress: {asyncLoad.progress}");
                _loadingScreenView.SetProgress(asyncLoad.progress);
                _loadingScreenView.SetLabel(asyncLoad.progress * 100);
                await UniTask.Yield();
            }
            _loadingScreenView.SetProgress(1);
            _loadingScreenView.SetLabel(100);
            await UniTask.Delay(500);
            asyncLoad.allowSceneActivation = true;
            await UniTask.WaitUntil(() => asyncLoad.isDone);
            HideLoadingScreen();
        }

        private void ShowLoadingScreen() => _loadingScreenView.ShowWindow();

        private void HideLoadingScreen() => _loadingScreenView.HideWindow();
    }
}