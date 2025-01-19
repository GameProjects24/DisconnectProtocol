using UnityEngine;

public abstract class WeaponShootSO : ScriptableObject
{
    public abstract void Shoot(Vector3 position, Vector3 direction);
}
