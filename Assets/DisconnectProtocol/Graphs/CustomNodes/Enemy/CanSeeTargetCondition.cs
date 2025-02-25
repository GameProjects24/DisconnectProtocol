using System;
using DisconnectProtocol;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Can see target", story: "[Body] can see [target]", category: "DisconnectProtocol/Conditions", id: "1579321f13324c849574f5bff137ccae")]
public partial class InViewAngleCondition : Condition
{
	[SerializeReference] public BlackboardVariable<MonoBehaviour> Body;
	[SerializeReference] public BlackboardVariable<Transform> Target;

	public override bool IsTrue()
	{
		if (Body == null || Target == null) {
			return false;
		}
		var ics = Body.Value as ICanSeeTarget;
		return ics == null ? false : ics.Eval(Target);
	}

	public override void OnStart()
	{
	}

	public override void OnEnd()
	{
	}
}
