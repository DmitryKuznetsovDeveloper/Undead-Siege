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

        
            // Если у вас есть другие зависимости, добавьте их здесь
        }
    }
}