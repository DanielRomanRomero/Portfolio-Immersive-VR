using UnityEngine;

namespace laplahce.Projectiles
{

	public abstract class ARotator : MonoBehaviour
	{
		[SerializeField] private Camera cam;
		[Tooltip("The maximum length to shoot the ray at. Outside of this, it will just fire at Max Length distance from camera.")]
		[SerializeField] private float maxLength;


		private void Update()
		{
			if (cam == null)
				throw new UnassignedReferenceException("Camera must be assigned.");

			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			var pos = ray.GetPoint(maxLength);
			if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, maxLength))
				pos = hit.point;

			RotateTowards(pos);
		}


		protected abstract void RotateTowards(Vector3 target);
		public abstract Quaternion GetRotation();
	}

}