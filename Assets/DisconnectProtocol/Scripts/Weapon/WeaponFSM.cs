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
    private IWeaponState _currentState;
    private Dictionary<WeaponStateEnum, IWeaponState> _states = new Dictionary<WeaponStateEnum, IWeaponState>();

    public WeaponFSM(Weapon context)
    {
        _states.Add(WeaponStateEnum.Idle, new WeaponStateIdle(this, context));
        _states.Add(WeaponStateEnum.Fire, new WeaponStateFire(this, context));
        _states.Add(WeaponStateEnum.Reload, new WeaponStateReload(this, context));
        _states.Add(WeaponStateEnum.Empty, new WeaponStateEmpty(this));
    }

    public void ActivateState(WeaponStateEnum state)
    {
        Debug.Log($"FSM > activate: {state}");
        if (_currentState != null)
        {
            _currentState.Exit();
            _currentState = null;
        }


        if (_states.TryGetValue(state, out _currentState))
        {
            _currentState.Enter();
        }
    }

    public void StartFire()
    {
        _currentState?.StartFire();
    }

    public void StopFire()
    {
        _currentState?.StopFire();
    }

    public void Reload()
    {
        _currentState?.Reload();
    }

    public void Update()
    {
        _currentState?.Update();
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
    private readonly WeaponFSM _weaponFSM;
    private readonly Weapon _context;

    public WeaponStateIdle(WeaponFSM weaponFSM, Weapon context)
    {
        _weaponFSM = weaponFSM;
        _context = context;
    }

    public override void Reload()
    {
        _weaponFSM.ActivateState(WeaponStateEnum.Reload);
    }

    public override void StartFire()
    {
        if (_context.CanFire())
        {
            _weaponFSM.ActivateState(WeaponStateEnum.Fire);
        }
    }
}

public class WeaponStateFire : WeaponStateBase
{
    private float _timer;

    private readonly WeaponFSM _weaponFSM;
    private readonly Weapon _context;

    public WeaponStateFire(WeaponFSM weaponFSM, Weapon context)
    {
        _weaponFSM = weaponFSM;
        _context = context;
    }

    override public void Enter()
    {
        _timer = 0f;
        _context.Shoot();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _context.weaponData.fireRate)
        {
            if (!_context.HasBullet() && _context.weaponData.isAutoReload)
            {
                _weaponFSM.ActivateState(WeaponStateEnum.Reload);
            }
            else if (_context.weaponData.isAutomatic)
            {
                if (_context.CanFire())
                {
                    _context.Shoot();
                }

                if (_context.CanFire())
                {
                    _timer = 0;
                }
                else
                {
                    _weaponFSM.ActivateState(WeaponStateEnum.Empty);
                }
            }
            else
            {
                StopFire();
            }
        }
    }

    public override void StopFire()
    {
        _weaponFSM.ActivateState(WeaponStateEnum.Idle);
    }
}

public class WeaponStateReload : WeaponStateBase
{
    private float _timer;

    private readonly WeaponFSM _weaponFSM;
    private readonly Weapon _context;

    public WeaponStateReload(WeaponFSM weaponFSM, Weapon context)
    {
        _weaponFSM = weaponFSM;
        _context = context;
    }

    override public void Enter()
    {
        _timer = 0f;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _context.weaponData.reloadTime)
        {
            _context.ReloadComplete();
            _weaponFSM.ActivateState(WeaponStateEnum.Idle);
        }
    }
}

public class WeaponStateEmpty : WeaponStateBase
{
    private readonly WeaponFSM _weaponFSM;

    public WeaponStateEmpty(WeaponFSM weaponFSM)
    {
        _weaponFSM = weaponFSM;
    }

    public override void Reload()
    {
        _weaponFSM.ActivateState(WeaponStateEnum.Reload);
    }
}
