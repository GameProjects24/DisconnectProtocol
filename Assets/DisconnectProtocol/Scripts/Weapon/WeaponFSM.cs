using System.Collections.Generic;
using UnityEngine;

public enum WeaponStateEnum { Idle, Fire, Reload, Empty }

public interface IWeaponState
{
    void Enter();
    void Exit();
    void StartFire();
    void StopFire();
    void Reload();
    void Update();
}

public class WeaponFSM
{
    private IWeaponState currentState;
    private Dictionary<WeaponStateEnum, IWeaponState> states = new Dictionary<WeaponStateEnum, IWeaponState>();

    public WeaponFSM(Weapon context)
    {
        states.Add(WeaponStateEnum.Idle, new WeaponStateIdle(this, context));
        states.Add(WeaponStateEnum.Fire, new WeaponStateFire(this, context));
        states.Add(WeaponStateEnum.Reload, new WeaponStateReload(this, context));
        states.Add(WeaponStateEnum.Empty, new WeaponStateEmpty(this));
    }

    public void ActivateState(WeaponStateEnum state)
    {
        currentState?.Exit();
        if (states.TryGetValue(state, out currentState))
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
    public virtual void Enter() { }

    public virtual void Exit()
    {
    }

    public virtual void Reload()
    {
    }

    public virtual void StartFire() { }

    public virtual void StopFire()
    {
    }

    public virtual void Update()
    {
    }
}

public class WeaponStateIdle : WeaponStateBase
{
    private WeaponFSM fsm;
    private Weapon context;

    public WeaponStateIdle(WeaponFSM fsm, Weapon context)
    {
        this.fsm = fsm;
        this.context = context;
    }

    public override void StartFire()
    {
        if (context.CanFire())
        {
            fsm.ActivateState(WeaponStateEnum.Fire);
        }

        context.StartFire();
    }

    public override void Reload()
    {
        fsm.ActivateState(WeaponStateEnum.Reload);
    }
}

public class WeaponStateFire : WeaponStateBase
{
    private WeaponFSM fsm;
    private Weapon context;

    public WeaponStateFire(WeaponFSM fsm, Weapon context)
    {
        this.fsm = fsm;
        this.context = context;
    }

    public override void Enter()
    {
        context.Shoot();
    }

    public override void Update()
    {
        if (!context.HasAmmo())
        {
            fsm.ActivateState(WeaponStateEnum.Empty);
        }
        else
        {
            fsm.ActivateState(WeaponStateEnum.Idle);
        }
    }

    public override void StopFire()
    {
        fsm.ActivateState(WeaponStateEnum.Idle);
        context.StopFire();
    }
}

public class WeaponStateReload : WeaponStateBase
{
    private WeaponFSM fsm;
    private Weapon context;
    private float timer;

    public WeaponStateReload(WeaponFSM fsm, Weapon context)
    {
        this.fsm = fsm;
        this.context = context;
    }

    public override void Enter()
    {
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= context.weaponData.reloadTime)
        {
            context.ReloadComplete();
            fsm.ActivateState(WeaponStateEnum.Idle);
        }
    }
}

public class WeaponStateEmpty : WeaponStateBase
{
    private WeaponFSM fsm;

    public WeaponStateEmpty(WeaponFSM fsm)
    {
        this.fsm = fsm;
    }

    public override void Reload()
    {
        fsm.ActivateState(WeaponStateEnum.Reload);
    }
}
