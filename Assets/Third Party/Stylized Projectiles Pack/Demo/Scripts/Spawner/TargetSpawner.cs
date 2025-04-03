using System;
using UnityEngine;

namespace laplahce.Projectiles.Demo
{

	public class TargetSpawner : ASpawner
	{
		[SerializeField] private RotateToMouse rotation;

		protected override void Spawn(AProjectile projectile, Vector3 pos)
		{
			if (rotation == null)
				throw new UnassignedReferenceException("Rotation must be assigned.");

			var rot = rotation.GetRotation();
			Instantiate(projectile, pos, rot);
		}
	}

}
