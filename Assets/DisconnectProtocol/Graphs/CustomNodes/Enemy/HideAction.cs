using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Hide", story: "[Self] hides from [Target] behind objects with tag [tag]", category: "DisconnectProtocol/Actions", id: "bd8e54596aaa3ce2c75cd91e7761c61a")]
public partial class HideAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<string> Tag;

	private List<Collider> m_spots = new List<Collider>();
	private float m_spotOffset = 2f;
	private bool m_isHiding = false;
	private const float EPS = 0.1f;

    protected override Status OnStart()
    {
		Self.Value.isStopped = false;
		m_isHiding = false;
		foreach (var go in GameObject.FindGameObjectsWithTag(Tag)) {
			m_spots.Add(go.GetComponent<Collider>());
		}
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		if (m_isHiding) {
			if (Self.Value.remainingDistance <= Self.Value.stoppingDistance + EPS) {
				return Status.Success;
			}
		} else {
			m_isHiding = true;
			Self.Value.SetDestination(NearestHide());
		}
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

	private Vector3 NearestHide() {
		var minDist = Mathf.Infinity;
		var res = new Vector3(Self.Value.radius, 0, Self.Value.radius);
		foreach (var col in m_spots) {
			var curDist = Vector3.SqrMagnitude(Self.Value.transform.position - col.transform.position);
			if (curDist >= minDist) {
				continue;
			}
			minDist = curDist;

			var hideOffset = (col.transform.position - Target.Value.position).normalized * m_spotOffset;
			res += col.ClosestPointOnBounds(col.bounds.center + hideOffset);
		}

		return res;
	}
}

