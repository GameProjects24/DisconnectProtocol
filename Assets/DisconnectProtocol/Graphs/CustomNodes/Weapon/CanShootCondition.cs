using System;
using Unity.Behavior;
using UnityEngine;
using DisconnectProtocol;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanShoot", story: "[Weapon] can be fired", category: "Weapon/Conditions", id: "fe026462ea167d3d73fae07b98b6ee69")]
public partial class CanShootCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Weapon> Weapon;

    public override bool IsTrue()
    {
        return Weapon.Value.CanShoot();
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
