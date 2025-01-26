using System;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Can see target", story: "[self] with view angel of [angle] can see [target]", category: "DisconnectProtocol/Conditions", id: "1579321f13324c849574f5bff137ccae")]
public partial class InViewAngleCondition : Condition
{
	[SerializeReference] public BlackboardVariable<Transform> Self;
	[SerializeReference] public BlackboardVariable<Transform> Target;
	[SerializeReference] public BlackboardVariable<float> Angle;

	private bool m_wasSeen = false;

	public override bool IsTrue()
	{
		if (Self == null || Target == null) {
			return false;
		}

		var dir = Target.Value.position - Self.Value.position;
		bool inAngle = Vector3.Angle(Self.Value.forward, dir) <= Angle;
		if (inAngle && !m_wasSeen) {
			bool noObstacles;
			if (Physics.Linecast(Self.Value.position, Target.Value.position, out var hit)) {
				noObstacles = hit.transform.root.CompareTag("Player");
			} else {
				noObstacles = true;
			}
			return m_wasSeen = noObstacles;
		}

		return inAngle;
	}

	public override void OnStart()
	{
	}

	public override void OnEnd()
	{
	}
}
