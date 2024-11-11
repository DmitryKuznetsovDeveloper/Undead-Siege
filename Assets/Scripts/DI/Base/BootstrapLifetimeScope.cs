using Game;
using UI.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class BootstrapLifetimeScope : LifetimeScope
{
    [SerializeField] private LoadingScreenView _loadingScreenPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance<ILoadingScreenView,LoadingScreenView>(_loadingScreenPrefab);
        builder.Register<ISceneLoader,SceneLoader>(Lifetime.Singleton).WithParameter<ILoadingScreenView>(_loadingScreenPrefab);
        
        builder.Register<IApplicationExitHandler, ApplicationExitHandler>(Lifetime.Singleton);
    }
}