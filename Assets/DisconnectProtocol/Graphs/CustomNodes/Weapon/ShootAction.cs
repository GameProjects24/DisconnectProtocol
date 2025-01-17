using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DisconnectProtocol;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "Shoot [weapon]", category: "Weapon/Actions", id: "2b10202ace55d377690f4622220ad5a9")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<Weapon> Weapon;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
		Weapon.Value.Shoot();
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

