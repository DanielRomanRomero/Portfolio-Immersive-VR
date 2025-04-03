using System;
using UnityEngine;

namespace laplahce.Projectiles.Demo
{

	public class DirectionSpawner : ASpawner
	{
		[SerializeField] private RotateForGravityTarget rotation;

		protected override void Spawn(AProjectile projectile, Vector3 pos)
		{
			Instantiate(projectile, pos, rotation.GetRotation(projectile.Speed));
		}
	}

}
