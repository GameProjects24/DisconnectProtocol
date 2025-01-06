using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EnoughCurrentAmmo", story: "[weapon] has current ammo [operator] [treshold]", category: "Weapon/Conditions", id: "d1cf697f5bdf926beae26ef865d7c420")]
public partial class EnoughAmmoCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Weapon> Weapon;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<ConditionOperator> Operator;
    [SerializeReference] public BlackboardVariable<int> Treshold;

    public override bool IsTrue()
    {
		if (Weapon == null || Treshold < 0) {
			return false;
		}
        return ConditionUtils.Evaluate(Weapon.Value.GetCurrentAmmo(), Operator, Treshold);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
