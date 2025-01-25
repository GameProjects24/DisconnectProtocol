using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Initialize Variable", story: "Initialize [var] with [tag]", category: "DisconnectProtocol/Actions", id: "b85bb8bf7fc652735347918ac7cff052")]
public partial class InitializeVariableAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Var;
    [SerializeReference] public BlackboardVariable<string> Tag;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		if (Var.Value == null) {
			Var.Value = GameObject.FindGameObjectWithTag(Tag);
		}
        return Var.Value == null ? Status.Failure : Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

