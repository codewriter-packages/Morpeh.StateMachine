using System;
using System.Collections.Generic;

namespace Morpeh
{
    internal sealed class FsmStateRunner<TState> : FsmRunnerBase
        where TState : struct, IComponent
    {
        private readonly Filter _stateEnterFilter;
        private readonly Filter _stateExitFilter;
        private readonly ComponentsCache<StateProcessed> _stateProcessedCache;

        internal readonly List<Action<Entity>> StateEnterCallbacks = new List<Action<Entity>>();
        internal readonly List<Action<Entity>> StateExitCallbacks = new List<Action<Entity>>();

        public FsmStateRunner(Fsm fsm)
        {
            _stateEnterFilter = fsm.Filter.With<TState>().Without<StateProcessed>();
            _stateExitFilter = fsm.Filter.Without<TState>().With<StateProcessed>();
            _stateProcessedCache = fsm.World.GetCache<StateProcessed>();
        }

        public override void Run()
        {
            if (_stateEnterFilter.Length == 0 && _stateExitFilter.Length == 0)
            {
                return;
            }

            foreach (var entity in _stateEnterFilter)
            {
                _stateProcessedCache.AddComponent(entity);

                foreach (var enterCallback in StateEnterCallbacks)
                {
                    enterCallback.Invoke(entity);
                }
            }

            foreach (var entity in _stateExitFilter)
            {
                foreach (var exitCallback in StateExitCallbacks)
                {
                    exitCallback.Invoke(entity);
                }

                _stateProcessedCache.RemoveComponent(entity);
            }
        }

        // ReSharper disable once UnusedTypeParameter
        private struct StateProcessed : IComponent
        {
        }
    }
}