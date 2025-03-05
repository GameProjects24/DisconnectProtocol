using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Blink Vertical", story: "[Body] blinks vertically", category: "DisconnectProtocol/Actions", id: "25bb79ed95b37dd2d38895bc55d328f9")]
public partial class BlinkVerticalAction : Action
{
    [SerializeReference] public BlackboardVariable<MonoBehaviour> Body;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		var bl = Body.Value as IBlinkVertical;
		if (bl != null) {
			return bl.TryStart() ? Status.Success : Status.Failure;
		}
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

