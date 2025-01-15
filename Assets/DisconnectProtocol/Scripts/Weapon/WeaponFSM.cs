using System.Collections.Generic;
using UnityEngine;

public enum WeaponStateEnum { Idle, Fire, Reload, Empty }

public interface IWeaponState
{
    void Enter();
    void Exit();
    void Update();
    void StartFire();
    void StopFire();
    void Reload();
}

public class WeaponFSM
{
    private IWeaponState currentState;
    private readonly Dictionary<WeaponStateEnum, IWeaponState> states = new Dictionary<WeaponStateEnum, IWeaponState>();

    public WeaponFSM(Weapon context)
    {
        // Инициализация всех состояний
        states.Add(WeaponStateEnum.Idle, new WeaponStateIdle(this, context));
        states.Add(WeaponStateEnum.Fire, new WeaponStateFire(this, context));
        states.Add(WeaponStateEnum.Reload, new WeaponStateReload(this, context));
        states.Add(WeaponStateEnum.Empty, new WeaponStateEmpty(this, context));
    }

    public void ActivateState(WeaponStateEnum newState)
    {
        // Проверка на уже активное состояние
        if (currentState != null && states.TryGetValue(newState, out var newStateInstance) && currentState == newStateInstance)
        {
            Debug.Log($"FSM > State '{newState}' is already active.");
            return;
        }

        // Завершаем текущее состояние
        currentState?.Exit();

        // Переходим в новое состояние
        if (states.TryGetValue(newState, out currentState))
        {
            currentState.Enter();
        }
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void StartFire()
    {
        currentState?.StartFire();
    }

    public void StopFire()
    {
        currentState?.StopFire();
    }

    public void Reload()
    {
        currentState?.Reload();
    }
}

public abstract class WeaponStateBase : IWeaponState
{
    protected readonly WeaponFSM weaponFSM;
    protected readonly Weapon context;

    protected WeaponStateBase(WeaponFSM weaponFSM, Weapon context)
    {
        this.weaponFSM = weaponFSM;
        this.context = context;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void StartFire() { }
    public virtual void StopFire() { }
    public virtual void Reload() { }
}

public class WeaponStateIdle : WeaponStateBase
{
    public WeaponStateIdle(WeaponFSM weaponFSM, Weapon context) : base(weaponFSM, context) { }

    public override void StartFire()
    {
        if (context.CanFire())
        {
            weaponFSM.ActivateState(WeaponStateEnum.Fire);
        }
        else
        {
            weaponFSM.ActivateState(WeaponStateEnum.Empty);
        }
    }

    public override void Reload()
    {
        if (context.CanReload())
        {
            weaponFSM.ActivateState(WeaponStateEnum.Reload);
        }
    }
}

public class WeaponStateFire : WeaponStateBase
{
    public WeaponStateFire(WeaponFSM weaponFSM, Weapon context) : base(weaponFSM, context) { }

    public override void Enter()
    {
        context.StartShooting();
    }

    public override void Update()
    {
        if (!context.CanFire())
        {
            weaponFSM.ActivateState(WeaponStateEnum.Empty);
        }
    }

    public override void StopFire()
    {
        weaponFSM.ActivateState(WeaponStateEnum.Idle);
    }
}

public class WeaponStateReload : WeaponStateBase
{
    private float reloadTimer;

    public WeaponStateReload(WeaponFSM weaponFSM, Weapon context) : base(weaponFSM, context) { }

    public override void Enter()
    {
        reloadTimer = 0f;
        context.StartReloading();
    }

    public override void Update()
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= context.weaponData.reloadTime)
        {
            context.CompleteReload();
            weaponFSM.ActivateState(WeaponStateEnum.Idle);
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Reload state.");
    }
}

public class WeaponStateEmpty : WeaponStateBase
{
    public WeaponStateEmpty(WeaponFSM weaponFSM, Weapon context) : base(weaponFSM, context) { }

    public override void Reload()
    {
        if (context.CanReload())
        {
            weaponFSM.ActivateState(WeaponStateEnum.Reload);
        }
    }

    public override void StartFire()
    {
        Debug.Log("Cannot fire: no ammo.");
    }
}
