using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HoldDistance", story: "[self] holds at [distance] from [target]", category: "Action/Navigation", id: "a85448343e3e7947fa5bab0e822fccd8")]
public partial class HoldDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Self;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        Self.Value.isStopped = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Self.Value.SetDestination(Target.Value.position);
        Debug.Log($"{Self.Value.remainingDistance}");
        if (Self.Value.remainingDistance <= Distance) {
            Debug.Log("kyku");
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Self.Value.isStopped = true;
        Self.Value.ResetPath();
    }
}

