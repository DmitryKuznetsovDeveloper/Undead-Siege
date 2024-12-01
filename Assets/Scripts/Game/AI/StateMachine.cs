namespace Game.AI
{
    public class StateMachine<TContext>
    {
        private IState<TContext> _currentState;

        public void TransitionTo(IState<TContext> newState, TContext context)
        {
            _currentState?.Exit(context);
            _currentState = newState;
            _currentState?.Enter(context);
        }

        public void Update(TContext context) => _currentState?.Execute(context);
    }

    public interface IState<TContext>
    {
        void Enter(TContext context);
        void Execute(TContext context);
        void Exit(TContext context);
        bool IsBusy { get; }
    }
}