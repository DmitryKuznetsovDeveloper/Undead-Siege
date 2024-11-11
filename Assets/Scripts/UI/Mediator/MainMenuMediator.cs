using System;
using Game;
using UnityEngine;
using VContainer.Unity;

namespace UI.Mediator
{
    public class MainMenuMediator : IStartable, IDisposable
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IApplicationExitHandler _applicationExit;
        private readonly MainMenuView _mainMenuView;
        
        public MainMenuMediator(ISceneLoader sceneLoader, IApplicationExitHandler applicationExit, MainMenuView mainMenuView)
        {
            _sceneLoader = sceneLoader;
            _applicationExit = applicationExit;
            _mainMenuView = mainMenuView;
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
        }

        private void LoadGame() => _sceneLoader.LoadSceneAsync("Game");
    }
}