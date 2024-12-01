using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<CharacterController>();
        }
    }
}