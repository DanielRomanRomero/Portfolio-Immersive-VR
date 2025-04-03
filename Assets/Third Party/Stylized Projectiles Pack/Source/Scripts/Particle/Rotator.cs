using UnityEngine;

namespace laplahce.Projectiles
{

	public class Rotator : MonoBehaviour
	{
		[Tooltip("Speed at which the projectile should move in.")]
		[SerializeField] private float speed;
		[Tooltip("What is up/its normal for the rotator?")]
		[SerializeField] private Vector3 up = Vector3.forward;

		private void Update()
		{
			transform.Rotate(up * (speed * Time.deltaTime));
		}
	}

}
