using Game.Input;
using VContainer;
using VContainer.Unity;
namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Регистрация классов ввода
            builder.Register<MoveInput>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<JumpInput>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WeaponInput>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PauseInput>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MouseInput>(Lifetime.Singleton).AsImplementedInterfaces();
        
            // Если у вас есть другие зависимости, добавьте их здесь
        }
    }
}