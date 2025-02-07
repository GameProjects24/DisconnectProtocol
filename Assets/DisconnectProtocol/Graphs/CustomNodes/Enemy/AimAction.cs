using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;
using System.Runtime.CompilerServices;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Aim", story: "[Body] perpetually [isPerpetual] aims to [target]", category: "DisconnectProtocol/Actions", id: "762514b674bc6df9e2e2716fd1f1ab1e")]
public partial class AimAction : Action
{
    [SerializeReference] public BlackboardVariable<SoldierBodyController> Body;
	[SerializeReference] public BlackboardVariable<bool> IsPerpetual;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Body.Value.AimStart(Target, IsPerpetual);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

