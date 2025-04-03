namespace laplahce.Projectiles
{

	public class SimpleProjectile : AProjectile
	{
		protected override void Move(float dt)
		{
			// transform.position += transform.forward * (speed * dt);
			rb.linearVelocity = transform.forward * speed;
		}
	}

}