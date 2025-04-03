using System.Collections;
using UnityEngine;

namespace laplahce.Projectiles
{

	public class TrailParticle : MonoBehaviour
	{
		public void Fade()
		{
			StartCoroutine(FadeOut(GetComponentsInChildren<TrailRenderer>()));
		}

		private IEnumerator FadeOut(TrailRenderer[] renderers)
		{
			const float T = 0.15f;

			float t = 0;
			while (t <= T)
			{
				foreach (var r in renderers)
					r.material.SetFloat("_Opacity", 1 - t / T);

				yield return new WaitForEndOfFrame();
				t += Time.deltaTime;
			}
			Destroy(gameObject);
		}
	}

}
