using System;
using System.Collections;
using UnityEngine;

namespace DisconnectProtocol
{
	public class Weapon : MonoBehaviour
	{
		public WeaponData weaponData;

		private int _currentAmmo;
		private int _totalAmmo;
		private bool _isReloading;
		private float _lastFireTime;
		private bool _isFiring;
		private Animator _animator;
		private AudioSource _fireAudioSource;


		private void Start()
		{
			_currentAmmo = weaponData.magazineSize;
			_totalAmmo = weaponData.maxAmmo;
			_isReloading = false;
			_isFiring = false;
			_animator = GetComponent<Animator>();

			// Добавляем AudioSource для воспроизведения звука стрельбы
			_fireAudioSource = GetComponent<AudioSource>();
			_fireAudioSource.clip = weaponData.fireSound;
			_fireAudioSource.loop = true;
			if (_fireAudioSource.isPlaying)
			{
				_fireAudioSource.Stop();
			}
		}

		private void FixedUpdate()
		{
			if (_isFiring && CanShoot())
			{
				Shoot();
			}
			if (_currentAmmo == 0)
			{
				StopFiring();
			}
		}

		public void StartFiring()
		{
			_isFiring = true;
		}

		public void StopFiring()
		{
			if (_fireAudioSource.isPlaying)
			{
				_fireAudioSource.Stop();
			}
			_isFiring = false;
		}

		public void Shoot()
		{
			if (!_fireAudioSource.isPlaying)
			{
				_fireAudioSource.Play();
			}

			if (_isReloading || Time.time - _lastFireTime < weaponData.fireRate || _currentAmmo == 0)
				return;

			_lastFireTime = Time.time;

			if (weaponData.projectilePrefab != null)
			{
				FireProjectile();
			}
			else
			{
				Debug.LogWarning("Projectile not found!");
				return;
				// FireHitscan();
			}

			_currentAmmo--;
			PlayMuzzleEffect();
			//PlayFireSound();
		}

		private void FireProjectile()
		{
			GameObject projectile = Instantiate(weaponData.projectilePrefab, transform.position, transform.rotation);
			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddForce(transform.forward * weaponData.projectileSpeed, ForceMode.VelocityChange);
			}

			Bullet bullet;
			if (!projectile.TryGetComponent<Bullet>(out bullet)) {
				bullet = projectile.AddComponent<Bullet>();
			}
			bullet.weaponDamage = weaponData.damage;
	}

		// private void FireHitscan()
		// {
		//     RaycastHit hit;
		//     if (Physics.Raycast(transform.position, transform.forward, out hit, weaponData.range))
		//     {
		//         // Example: Apply damage to the target if it has a health component
		//         var target = hit.collider.GetComponent<Target>();
		//         if (target != null)
		//         {
		//             target.TakeDamage(weaponData.damage);
		//         }
		//     }
		// }

		public void Reload()
		{
			if (_isReloading || _currentAmmo == weaponData.magazineSize || _totalAmmo <= 0)
				return;

			StartCoroutine(ReloadCoroutine());
		}

		private IEnumerator ReloadCoroutine()
		{
			_isReloading = true;
			PlayReloadSound();
			if (_animator)
			{
				_animator.Play("SimpleReloadAnim");
			}
			yield return new WaitForSeconds(weaponData.reloadTime);

			int ammoNeeded = weaponData.magazineSize - _currentAmmo;
			int ammoToReload = Mathf.Min(ammoNeeded, _totalAmmo);
			_currentAmmo += ammoToReload;
			_totalAmmo -= ammoToReload;

			_isReloading = false;
		}

		private void PlayMuzzleEffect()
		{
			if (weaponData.muzzleFlashEffect != null)
			{
				Instantiate(weaponData.muzzleFlashEffect, transform.position, transform.rotation);
			}
		}

		// private void PlayFireSound()
		// {
		//     if (weaponData.fireSound != null)
		//     {
		//         AudioSource.PlayClipAtPoint(weaponData.fireSound, transform.position);
		//     }
		// }

		private void PlayReloadSound()
		{
			if (weaponData.reloadSound != null)
			{
				AudioSource.PlayClipAtPoint(weaponData.reloadSound, transform.position);
			}
		}

		public bool CanShoot()
		{
			return !_isReloading && _currentAmmo > 0 && Time.time - _lastFireTime >= weaponData.fireRate;
		}

		public int GetCurrentAmmo() => _currentAmmo;

		public int GetTotalAmmo() => _totalAmmo;
	}
}