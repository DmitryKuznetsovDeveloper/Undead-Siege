using UI.Mediator;
using VContainer;
using VContainer.Unity;
namespace DI
{
    public class MetaLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainMenuView>();
            builder.Register<MainMenuMediator>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}