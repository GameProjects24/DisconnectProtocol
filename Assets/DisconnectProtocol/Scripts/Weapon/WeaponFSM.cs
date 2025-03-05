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
    private Weapon _context;
    public bool IsFiring { get; private set; } = false;

    public WeaponFSM(Weapon context)
    {
        _context = context;

        _states.Add(WeaponStateEnum.Idle, new WeaponStateIdle(this, context));
        _states.Add(WeaponStateEnum.Fire, new WeaponStateFire(this, context));
        _states.Add(WeaponStateEnum.Reload, new WeaponStateReload(this, context));
        _states.Add(WeaponStateEnum.Empty, new WeaponStateEmpty(this));
    }

    public void ActivateState(WeaponStateEnum state)
    {
        if (_currentState != null)
        {
            if (_states[state] == _currentState) return;

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
        IsFiring = true;
        _currentState?.StartFire();
    }

    public void StopFire()
    {
        IsFiring = false;
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

    public virtual void Exit() { }

    public virtual void Reload() { }

    public virtual void StartFire() { }

    public virtual void StopFire() { }

    public virtual void Update() { }
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
        if (_context.HasAmmo())
        {
            _weaponFSM.ActivateState(WeaponStateEnum.Fire);
        }
        else
        {
            _weaponFSM.ActivateState(WeaponStateEnum.Empty);
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

    public override void Enter()
    {
        _timer = 0f;
        _context.Shoot();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        // Ждём окончания задержки перед следующим действием
        if (_timer >= _context.weaponData.fireRate)
        {
            if (_context.HasAmmo())
            {
                if (_weaponFSM.IsFiring)
                {
                    if (_context.weaponData.isAutomatic)
                    {
                        _context.Shoot();
                        _timer = 0f; // Сброс таймера, если оружие автоматическое и кнопка зажата

                    }
                }
                else
                {
                    // Если кнопка не нажата, переходим в Idle
                    _weaponFSM.ActivateState(WeaponStateEnum.Idle);
                }
            }
            else
            {
                _weaponFSM.ActivateState(WeaponStateEnum.Empty); // Если нет патронов, уходим в Empty
            }
        }
    }

    public override void StopFire()
    {
        // Только если задержка прошла, мы выходим в Idle
        if (_timer >= _context.weaponData.fireRate)
        {
            _weaponFSM.ActivateState(WeaponStateEnum.Idle);
        }
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
        _context.Reload();
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
