using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace laplahce.Projectiles
{

	public class Particle : Creator
	{
#pragma warning disable CS0414
		[Header("Editor Playback")]
		[Tooltip("Speed of the playback.")]
		[SerializeField, Range(0, 2)] private float playbackSpeed = 1;
		[Tooltip("Pause the playback.")]
		[SerializeField] private bool pause = false;
#pragma warning restore

		//[Header("On Init")]//comentado en mi juego
		//[SerializeField] private EShake cameraShake = EShake.None;//comentado en mi juego

        [Header("Play")]
		[SerializeField] private VisualEffect[] particleVisuals;
		public enum ClearBehavior
		{
			None,
			Disable,
			Destroy
		}
		[SerializeField] private bool isHit;

		[Header("On Finished")]
		[SerializeField] private ClearBehavior clearBehavior = ClearBehavior.Destroy;
		[SerializeField] private UnityEvent callbacks;
		[SerializeField] private Optional<float> lifetime;

        //private float age;

    

     
        private void OnEnable()
        {
			if (isHit)
			{
            //    Show();
                StartCoroutine(DeactivateObject());
            }
			
        }
  //      private void Start()
		//{
		//	//CameraController.Shake(cameraShake);//comentado en mi juego

  //          age = lifetime.Enabled ? lifetime.Value : 1f;
		//}


		private IEnumerator DeactivateObject()
		{
			yield return new WaitForSeconds(2f);
			print("se debería desactivar");
			gameObject.SetActive(false);

		}

        public void PlayEffect()
        {
            foreach (var particles in particleVisuals)
            {
				particles.Play();
    
            }
        }

        //	private void Update()//comentado en mi juego
        //{
        //        if (age < 0)
        //        {
        //            if (lifetime.Enabled)
        //            {
        //                Dispose();
        //                return;
        //            }
        //            foreach (var fx in Effects)
        //            {
        //                if (fx.aliveParticleCount > 0)
        //                    return;
        //            }
        //            Dispose();

        //            return;
        //        }
        //        age -= Time.deltaTime;
        //    }
        

        private void Dispose()
		{
			callbacks?.Invoke();
			switch (clearBehavior)
			{
				case ClearBehavior.None:
					break;
				case ClearBehavior.Disable:
					gameObject.SetActive(false);
					break;
				case ClearBehavior.Destroy:
					Destroy(gameObject);
					break;
			}
		}


#if UNITY_EDITOR
		internal override void Refresh()
		{
			if (Application.isPlaying) return;

			ApplyToAll(fx =>
			{
				fx.pause = pause;
				fx.playRate = playbackSpeed;
			});
		}
#endif

#if UNITY_EDITOR
		public override void Show()
		{
			if (Application.isPlaying) return;

			pause = false;

			Refresh();
			ApplyToAll(fx =>
			{
				fx.Reinit();
			});

		}
#endif
	}

	



#if UNITY_EDITOR
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Particle))]
	public class ParticleEditor : CreatorEditor<Particle>
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
#endif

}