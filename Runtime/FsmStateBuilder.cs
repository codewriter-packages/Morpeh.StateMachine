using System;
using JetBrains.Annotations;

namespace Morpeh
{
    public readonly struct FsmStateBuilder<TState>
        where TState : struct, IComponent
    {
        internal readonly Fsm Fsm;
        internal readonly FsmStateRunner<TState> StateRunner;

        internal FsmStateBuilder(Fsm fsm, FsmStateRunner<TState> stateRunner)
        {
            Fsm = fsm;
            StateRunner = stateRunner;
        }

        [PublicAPI]
        public FsmStateBuilder<TState> OnEnter(Action<Entity> callback)
        {
            StateRunner.StateEnterCallbacks.Add(callback);
            return this;
        }

        [PublicAPI]
        public FsmStateBuilder<TState> OnExit(Action<Entity> callback)
        {
            StateRunner.StateExitCallbacks.Add(callback);
            return this;
        }

        [PublicAPI]
        public FsmStateBuilder<TState> Transition<TEvent, TNextState>(FsmTransition<TEvent, TNextState> t)
            where TEvent : struct, IComponent
            where TNextState : struct, IComponent
        {
            Fsm.TransitionRunners.Add(new FsmSimpleTransitionRunner<TState, TEvent, TNextState>(Fsm, t));
            return this;
        }

        [PublicAPI]
        public FsmStateBuilder<TState> Transition<TEvent>(FsmTransitionCustom<TEvent> t)
            where TEvent : struct, IComponent
        {
            Fsm.TransitionRunners.Add(new FsmCustomTransitionRunner<TState, TEvent>(Fsm, t));
            return this;
        }
    }
}