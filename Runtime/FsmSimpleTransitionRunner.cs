namespace Morpeh
{
    internal sealed class FsmSimpleTransitionRunner<TState, TEvent, TNextState> : FsmRunnerBase
        where TState : struct, IComponent
        where TEvent : struct, IComponent
        where TNextState : struct, IComponent
    {
        private readonly FsmTransition<TEvent, TNextState> _transition;
        private readonly ComponentsCache<TState> _stateCache;
        private readonly ComponentsCache<TEvent> _eventCache;
        private readonly ComponentsCache<TNextState> _nextStateCache;
        private readonly Filter _transitionFilter;

        public FsmSimpleTransitionRunner(Fsm fsm, FsmTransition<TEvent, TNextState> transition)
        {
            _transition = transition;
            _stateCache = fsm.World.GetCache<TState>();
            _eventCache = fsm.World.GetCache<TEvent>();
            _nextStateCache = fsm.World.GetCache<TNextState>();
            _transitionFilter = fsm.Filter.With<TState>().With<TEvent>();
        }

        public override void Run()
        {
            foreach (var entity in _transitionFilter)
            {
                ref var evt = ref _eventCache.GetComponent(entity);

                var nextState = _transition.Invoke(ref evt);
                _nextStateCache.SetComponent(entity, nextState);

                _stateCache.RemoveComponent(entity);
                _eventCache.RemoveComponent(entity);
            }
        }
    }

    // ReSharper disable once TypeParameterCanBeVariant
    public delegate TState FsmTransition<TEvent, TState>(ref TEvent evt);
}