using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopFiring", story: "Stop firing [weapon]", category: "Weapon/Actions", id: "ac8db65d42669dd303143cc80bcaf137")]
public partial class StopFiringAction : Action
{
    [SerializeReference] public BlackboardVariable<WeaponController> Weapon;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Weapon.Value.StopFire();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

