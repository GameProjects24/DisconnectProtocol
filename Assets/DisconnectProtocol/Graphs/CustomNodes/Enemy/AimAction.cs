using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Aim", story: "[Self] aims to [target] with rotation speed [speed]", category: "DisconnectProtocol/Actions", id: "762514b674bc6df9e2e2716fd1f1ab1e")]
public partial class AimAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
	[SerializeReference] public BlackboardVariable<float> Speed;

	private const float EPS = 1f;
	private Vector3 m_prevPos = Vector3.zero;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		// var dir = Target.Value.position - Self.Value.position;
		// var rotDir = Vector3.RotateTowards(Self.Value.forward, dir, Speed * Time.deltaTime - m_time, 0f);
		// var rot = Quaternion.LookRotation(rotDir);
		// var ea = rot.eulerAngles;
		// ea.x = ea.z = 0f;
		// rot.eulerAngles = ea;
		// if (Mathf.Abs(rot.eulerAngles.y - Self.Value.rotation.eulerAngles.y) > EPS) {
		// 	Self.Value.rotation = rot;
		// 	return Status.Running;
		// }
		var pos = Target.Value.position;
		pos.y = Self.Value.position.y;
		Self.Value.LookAt(pos);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

