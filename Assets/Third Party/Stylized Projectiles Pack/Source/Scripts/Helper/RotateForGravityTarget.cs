using UnityEngine;

namespace laplahce.Projectiles
{

	public class RotateForGravityTarget : ARotator
	{
		[Tooltip("Object that should be placed at target location.")]
		[SerializeField] private Optional<Transform> targetIndicator;

		private Vector3 target;

		protected override void RotateTowards(Vector3 target)
		{
			this.target = target;
			if (targetIndicator.Enabled)
				targetIndicator.Value.position = new(target.x, targetIndicator.Value.position.y, target.z);
			transform.rotation = GetRotation();
		}

		public Quaternion GetRotation(float speed)
		{
			target.y = 0;
			Vector3 a = transform.position;
			a.y = 0;
			float b = Vector3.Distance(target, a);
			float v = speed;
			float g = Mathf.Abs(Physics.gravity.y);

			float x = Mathf.Asin(Mathf.Clamp01((b * g - (transform.position.y - target.y)) / (v * v))) / 2;

			Vector3 dir = (target - a).normalized;
			Quaternion rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(-Mathf.Rad2Deg * x, 0, 0);
			return rotation;
		}
		public override Quaternion GetRotation() => GetRotation(5); // this shouldn't be used.
	}

}
