using System;
using UnityEngine;

namespace laplahce.Projectiles.Demo
{

	public abstract class ASpawner : MonoBehaviour
	{
		[Tooltip("Speed at which the player can fire when holding down right-click.")]
		[SerializeField] private float firingRate;
		private float Delay => firingRate == 0 ? throw new DivideByZeroException() : 1 / firingRate;
		[Tooltip("The point the projectiles should spawn at.")]
		[SerializeField] private Transform firingPoint;

		private float t;

		private void Awake()
		{
			t = 0;
		}

		protected virtual void Update()
		{
			t -= Time.deltaTime;
			if (Input.GetMouseButtonDown(0) || (t <= 0 && Input.GetMouseButton(1)))
			{
				Fire();
				t = Delay;
			}
		}

		private void Fire()
		{
			if (firingPoint == null)
				throw new UnassignedReferenceException("Firing Point must be assigned.");

			Spawn(Navigator.GetCurrent(), firingPoint.transform.position);
		}

		/// <summary>
		/// What should happen when the spawner request it to spawn a projectile.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="pos"></param>
		protected abstract void Spawn(AProjectile projectile, Vector3 pos);
	}

}
