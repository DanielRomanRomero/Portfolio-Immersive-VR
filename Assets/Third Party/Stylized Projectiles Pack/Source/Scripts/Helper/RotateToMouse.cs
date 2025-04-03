using UnityEngine;

namespace laplahce.Projectiles
{

	public class RotateToMouse : ARotator
	{
		[Tooltip("Object that should be placed at the target location.")]
		[SerializeField] private Optional<Transform> targetIndicator;

		protected override void RotateTowards(Vector3 target)
		{
			var dir = target - transform.position;
			var rot = Quaternion.LookRotation(dir);
			transform.localRotation = Quaternion.Lerp(transform.rotation, rot, 1);
			if (targetIndicator.Enabled)
				targetIndicator.Value.position = target;
		}

		public override Quaternion GetRotation()
		{
			return transform.localRotation;
		}
	}

}
