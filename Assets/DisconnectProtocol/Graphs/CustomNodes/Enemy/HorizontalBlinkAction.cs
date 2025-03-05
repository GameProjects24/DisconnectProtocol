using DisconnectProtocol;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Horizontal Blink", story: "[Body] horizontally blinks at [target]", category: "DisconnectProtocol/Actions", id: "dfaf66fb59e104ecb8f36de7d0266d48")]
public partial class HorizontalBlinkAction : Action
{
    [SerializeReference] public BlackboardVariable<MonoBehaviour> Body;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		var blih = Body.Value as IBlinkHorizontal;
		if (blih != null) {
			return blih.TryStart(Target) ? Status.Success : Status.Failure;
		}
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

