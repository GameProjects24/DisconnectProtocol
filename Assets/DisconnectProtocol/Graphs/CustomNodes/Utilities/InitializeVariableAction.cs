using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Initialize var", story: "Initialize [var] with [tag] , in children of [object] only", category: "DisconnectProtocol/Actions", id: "b85bb8bf7fc652735347918ac7cff052")]
public partial class InitializeVariableAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Var;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<GameObject> Object;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		if (Var.Value == null) {
			if (Object.Value != null) {
				Var.Value = FindTransformWithTag(Object.Value.transform, Tag).gameObject;
			} else {
				Var.Value = GameObject.FindGameObjectWithTag(Tag);
			}
		}
        return Var.Value == null ? Status.Failure : Status.Success;
    }

    protected override void OnEnd()
    {
    }

	private Transform FindTransformWithTag(Transform parent, string tag) {
		if (parent.CompareTag(tag)) {
			return parent;
		}
		for (int i = 0; i < parent.childCount; ++i) {
			Transform res = parent.GetChild(i);
			if (res.CompareTag(tag)) {
				return res;
			}
			if ((res = FindTransformWithTag(res, tag)) != null) {
				return res;
			}
		}
		return null;
	}
}

