using System;
using System.Threading;
using Game;
using VContainer.Unity;

namespace UI.Mediator
{
    public sealed class MainMenuMediator : IStartable, IDisposable
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IApplicationExitHandler _applicationExit;
        private readonly MainMenuView _mainMenuView;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MainMenuMediator(ISceneLoader sceneLoader, IApplicationExitHandler applicationExit, MainMenuView mainMenuView)
        {
            _sceneLoader = sceneLoader;
            _applicationExit = applicationExit;
            _mainMenuView = mainMenuView;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _mainMenuView.StartButton.OnButtonClicked += LoadGame;
            _mainMenuView.ExitButton.OnButtonClicked += _applicationExit.Exit;
        }

        public void Dispose()
        {
            _mainMenuView.StartButton.OnButtonClicked -= LoadGame;
            _mainMenuView.ExitButton.OnButtonClicked -= _applicationExit.Exit;

            // Отменяем токен при уничтожении объекта
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void LoadGame()
        {
            try
            {
                // Передаём токен отмены в загрузку сцены
                await _sceneLoader.LoadSceneAsync("Game", _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Загрузка сцены была отменена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке сцены: {ex.Message}");
            }
        }
    }
}