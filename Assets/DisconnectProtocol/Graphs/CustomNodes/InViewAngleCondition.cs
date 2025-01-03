using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "In view angle", story: "angle between [self] and [target] [operator] [threshold]", category: "Conditions", id: "1579321f13324c849574f5bff137ccae")]
public partial class InViewAngleCondition : Condition
{
	[SerializeReference] public BlackboardVariable<Transform> Self;
	[SerializeReference] public BlackboardVariable<Transform> Target;
	[Comparison(comparisonType: ComparisonType.All)]
	[SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
	[SerializeReference] public BlackboardVariable<float> Threshold;

	public override bool IsTrue()
	{
		if (Self == null || Target == null) {
			return false;
		}

		var start = Self.Value.position + Self.Value.gameObject.GetComponentInChildren<Collider>().bounds.extents;
		if (Physics.Linecast(start, Target.Value.position, out var hit)) {
			if (hit.transform.root.CompareTag("Player")) {
				var dir = Target.Value.position - Self.Value.position;
				var angle = Vector3.Angle(Self.Value.forward, dir);
				return ConditionUtils.Evaluate(angle, Operator, Threshold);
			}
		}

		return false;
	}

	public override void OnStart()
	{
	}

	public override void OnEnd()
	{
	}
}
