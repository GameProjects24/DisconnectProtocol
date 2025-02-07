using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HoldDistance", story: "[Body] perpetually [isPerpetual] holds distance from [target]", category: "DisconnectProtocol/Actions", id: "a85448343e3e7947fa5bab0e822fccd8")]
public partial class HoldDistanceAction : Action
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
		Body.Value.HoldDistanceStart(Target, IsPerpetual);
		return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

