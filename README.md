# Morpeh.StateMachine [![Github license](https://img.shields.io/github/license/codewriter-packages/Morpeh.StateMachine.svg?style=flat-square)](#) [![Unity 2020.1](https://img.shields.io/badge/Unity-2020.1+-2296F3.svg?style=flat-square)](#) ![GitHub package.json version](https://img.shields.io/github/package-json/v/codewriter-packages/Morpeh.StateMachine?style=flat-square)
_State Machine for Morpeh ECS_

## How to use?

```csharp
using System;
using Morpeh;

// ==========================
// === Declare components ===
// ==========================

[Serializable]
public struct UnitComponent : IComponent { }

public static class UnitFsmEvents {
    [Serializable]
    public struct TargetFound : IComponent { public Entity target; }
    
    [Serializable]
    public struct TargetLost : IComponent { }
}

public static class UnitFsmStates {
    [Serializable]
    public struct Idle : IComponent { }

    [Serializable]
    public struct FollowTarget : IComponent { public Entity target; }
}

// ============================
// === Declare StateMachine ===
// ============================

public class UniBehaviourSystem : UpdateSystem {
    private Fsm fsm;

    public override void OnAwake() {
        this.fsm = new Fsm(World, Filter.With<UnitComponent>());

        this.fsm.AtState<UnitFsmStates.Idle>()
            .Transition((ref UnitFsmEvents.TargetFound evt) => new UnitFsmStates.FollowTarget {
                target = evt.target,
            });

        this.fsm.AtState<UnitFsmStates.FollowTarget>()
            .OnEnter(entity => Debug.Log("Enter FollowTarget state"))
            .OnExit(entity => Debug.Log("Exit FollowTarget state"))
            .Transition((ref UnitFsmEvents.TargetLost evt) => new UnitFsmStates.Idle());
    }

    public override void OnUpdate(float deltaTime) {
        this.fsm.Execute();
    }
}

// ===============================================
// === Write systems with logic for each state ===
// ===============================================

public class UnitIdleSystem : UpdateSystem {
    private Filter unitsFilter;

    public override void OnAwake() {
        this.unitsFilter = Filter.With<UnitComponent>().With<UnitFsmStates.Idle>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in this.unitsFilter) {
            if (TryGetTarget(out var target)) {
                entity.AddComponent<UnitFsmEvents.TargetFound>();
            }
        }
    }

    private bool TryGetTarget(out Entity target) { /* ... */ }
}

public class UnitFollowTargetSystem : UpdateSystem {
    private Filter unitsFilter;

    public override void OnAwake() {
        this.unitsFilter = Filter.With<UnitComponent>().With<UnitFsmStates.FollowTarget>();
    }

    public override void OnUpdate(float deltaTime) {
        foreach (var entity in this.unitsFilter) {
            ref var followTargetState = ref entity.GetComponent<UnitFsmStates.FollowTarget>();

            if (followTargetState.target.IsNullOrDisposed()) {
                entity.AddComponent<UnitFsmEvents.TargetLost>();
            }
        }
    }
}

// ======================================
// === Create Unit with initial state ===
// ======================================

public class UnitSpawnSystem : Initializer
{
    public override void OnAwake()
    {
        var entity = World.CreateEntity();
        entity.AddComponent<UnitComponent>();
        entity.AddComponent<UnitFsmStates.Idle>();
    }
}
```

## How to Install
Minimal Unity Version is 2020.1.

Library distributed as git package ([How to install package from git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html))
<br>Git URL: `https://github.com/codewriter-packages/Morpeh.StateMachine.git`

## License

Morpeh.StateMachine is [MIT licensed](./LICENSE.md).
