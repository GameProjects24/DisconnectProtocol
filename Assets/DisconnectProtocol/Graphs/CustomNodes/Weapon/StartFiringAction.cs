using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartFiring", story: "Start firing [weapon]", category: "Weapon/Actions", id: "1d367187598f7eb07c2050efb8d8733d")]
public partial class StartFiringAction : Action
{
    [SerializeReference] public BlackboardVariable<Weapon> Weapon;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Weapon.Value.StartFiring();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

