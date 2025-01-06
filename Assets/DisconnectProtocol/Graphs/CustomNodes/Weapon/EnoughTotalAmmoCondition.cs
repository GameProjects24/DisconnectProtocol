using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "EnoughTotalAmmo", story: "[weapon] has total ammo [operator] [treshold]", category: "Weapon/Conditions", id: "8cfbad4fc254e5e4c82b82c088dcedef")]
public partial class EnoughTotalAmmoCondition : Condition
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
        return ConditionUtils.Evaluate(Weapon.Value.GetTotalAmmo(), Operator, Treshold);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
