using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Reload", story: "Reload [weapon]", category: "Weapon/Actions", id: "ff46efac040a58b5e91b26ecc4dc6a09")]
public partial class ReloadAction : Action
{
    [SerializeReference] public BlackboardVariable<WeaponController> Weapon;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Weapon.Value.Reload();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

