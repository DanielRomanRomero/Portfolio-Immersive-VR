using UnityEngine;

namespace laplahce.Projectiles
{

	public class GravityProjectile : AProjectile
	{
		private Vector3? dir;

		protected override void Move(float dt)
		{
			if (dir == null)
			{
				dir = transform.forward;
				rb.linearVelocity = dir.Value * speed;
			}
			// transform.position += dir.Value * (speed * dt);

			Rotate();
		}
	}

}