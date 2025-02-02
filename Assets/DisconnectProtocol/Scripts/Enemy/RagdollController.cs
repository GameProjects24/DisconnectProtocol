using System.Collections.Generic;
using UnityEngine;

namespace DisconnectProtocol
{
	public enum RCMode {
		Animator,
		Ragdoll,
		GravityRagdoll,
	}

    public static class RagdollController
    {
		public static void ChangeMode(RCMode mode, Animator animator, List<Rigidbody> rbs) {
			bool isAnimator = mode == RCMode.Animator;
			bool useGravity = mode == RCMode.GravityRagdoll;
			animator.enabled = isAnimator;
			foreach (var rb in rbs) {
				rb.isKinematic = isAnimator;
				rb.useGravity = useGravity;
			}
		}
    }
}