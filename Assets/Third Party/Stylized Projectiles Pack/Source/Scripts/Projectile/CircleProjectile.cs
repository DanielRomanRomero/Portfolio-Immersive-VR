using UnityEngine;

namespace laplahce.Projectiles
{

	public class CircleProjectile : AProjectile
	{
		[Tooltip("The radius of the circle it should move in.")]
		[SerializeField] private float radius = 5f;
		private Vector3 center;
		private float angle;


		protected override void Awake()
		{
			base.Awake();

			center = transform.position - Vector3.right * radius;
			angle = 0;
		}

		protected override void Move(float dt)
		{
			angle += speed / radius * dt; // Update angle based on speed and radius

			// Calculate the new position and tangential velocity
			Vector3 newPosition = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
			Vector3 tangentialVelocity = new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle)) * speed;

			rb.position = newPosition; // Ensure position is correct
			rb.linearVelocity = tangentialVelocity; // Apply the tangential velocity

			Rotate();
		}
	}

}