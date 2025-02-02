using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HoldDistance", story: "[Body] holds distance from [target]", category: "DisconnectProtocol/Actions", id: "a85448343e3e7947fa5bab0e822fccd8")]
public partial class HoldDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<BodyController> Body;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Body.Value.HoldDistance(Target);
		return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

