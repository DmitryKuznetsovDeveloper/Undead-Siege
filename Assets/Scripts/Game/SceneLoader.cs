using System.Threading;
using Cysharp.Threading.Tasks;
using UI.View;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName, CancellationToken token);
    }
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly ILoadingScreenView _loadingScreenView;

        public SceneLoader(ILoadingScreenView loadingScreenView)
        {
            _loadingScreenView = loadingScreenView;
        }

        public async UniTask LoadSceneAsync(string sceneName, CancellationToken token)
        {
            // Показываем окно загрузки
            _loadingScreenView.ShowWindow();

            // Запускаем асинхронную загрузку
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f)
            {
                // Обновляем прогресс
                float progress = asyncLoad.progress / 0.9f; // Нормализуем прогресс до 1.0
                _loadingScreenView.SetProgress(progress);
                _loadingScreenView.SetLabel(progress * 100);

                Debug.Log($"SceneLoader Progress: {progress}");
                await UniTask.Yield(token);
            }

            // Устанавливаем 100% прогресса
            _loadingScreenView.SetProgress(1f);
            _loadingScreenView.SetLabel(100);

            // Делаем паузу перед активацией сцены
            await UniTask.Delay(500, cancellationToken: token);

            // Разрешаем активацию сцены
            asyncLoad.allowSceneActivation = true;

            // Ждём полной активации сцены
            await asyncLoad;

            // Скрываем окно загрузки
            _loadingScreenView.HideWindow();
        }
    }
}