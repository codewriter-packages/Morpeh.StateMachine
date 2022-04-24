using System.Collections.Generic;
using JetBrains.Annotations;

namespace Morpeh
{
    public sealed class Fsm
    {
        internal readonly World World;
        internal readonly Filter Filter;
        internal readonly List<FsmRunnerBase> StateRunners = new List<FsmRunnerBase>();
        internal readonly List<FsmRunnerBase> TransitionRunners = new List<FsmRunnerBase>();

        public Fsm(World world, Filter filter)
        {
            World = world;
            Filter = filter;
        }

        [PublicAPI]
        public FsmStateBuilder<TState> AtState<TState>()
            where TState : struct, IComponent
        {
            var stateRunner = new FsmStateRunner<TState>(this);

            StateRunners.Add(stateRunner);

            return new FsmStateBuilder<TState>(this, stateRunner);
        }

        [PublicAPI]
        public void Execute()
        {
            foreach (var runner in StateRunners)
            {
                runner.Run();
            }

            foreach (var runner in TransitionRunners)
            {
                runner.Run();
            }
        }
    }

    internal abstract class FsmRunnerBase
    {
        public abstract void Run();
    }
}