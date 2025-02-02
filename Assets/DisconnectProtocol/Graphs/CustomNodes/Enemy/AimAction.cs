using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Aim", story: "[Body] aims to [target]", category: "DisconnectProtocol/Actions", id: "762514b674bc6df9e2e2716fd1f1ab1e")]
public partial class AimAction : Action
{
    [SerializeReference] public BlackboardVariable<BodyController> Body;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Body.Value.Aim(Target);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

