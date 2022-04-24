namespace Morpeh
{
    internal sealed class FsmCustomTransitionRunner<TState, TEvent> : FsmRunnerBase
        where TState : struct, IComponent
        where TEvent : struct, IComponent
    {
        private readonly FsmTransitionCustom<TEvent> _transition;
        private readonly ComponentsCache<TEvent> _eventCache;
        private readonly ComponentsCache<TState> _stateCache;
        private readonly Filter _transitionFilter;

        public FsmCustomTransitionRunner(Fsm fsm, FsmTransitionCustom<TEvent> transition)
        {
            _transition = transition;
            _eventCache = fsm.World.GetCache<TEvent>();
            _stateCache = fsm.World.GetCache<TState>();
            _transitionFilter = fsm.Filter.With<TState>().With<TEvent>();
        }

        public override void Run()
        {
            foreach (var entity in _transitionFilter)
            {
                ref var evt = ref _eventCache.GetComponent(entity);

                _transition.Invoke(ref evt, entity);

                _stateCache.RemoveComponent(entity);
                _eventCache.RemoveComponent(entity);
            }
        }
    }

    public delegate void FsmTransitionCustom<TEvent>(ref TEvent evt, Entity entity);
}