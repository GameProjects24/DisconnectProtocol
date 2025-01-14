using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Aim", story: "[Self] aims to [target]", category: "Action/Enemy", id: "762514b674bc6df9e2e2716fd1f1ab1e")]
public partial class AimAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Self.Value.LookAt(Target.Value.transform);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

