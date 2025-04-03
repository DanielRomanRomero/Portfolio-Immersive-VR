using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace laplahce.Projectiles
{

	public struct Displacement
	{
		public Vector2 position;
		public float rotation;

		public Displacement(Vector2 position, float rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}
		public Displacement(Displacement displacement)
		{
			position = displacement.position;
			rotation = displacement.rotation;
		}

		public static Displacement zero
		{
			get
			{
				return new Displacement(Vector2.zero, 0);
			}
		}

		public static Displacement operator +(Displacement a, Displacement b)
		{
			return new Displacement(a.position + b.position,
				b.rotation + a.rotation);
		}

		public static Displacement operator -(Displacement a, Displacement b)
		{
			return new Displacement(a.position - b.position,
				b.rotation - a.rotation);
		}

		public static Displacement operator -(Displacement disp)
		{
			return new Displacement(-disp.position, -disp.rotation);
		}

		public static Displacement operator *(Displacement coords, float number)
		{
			return new Displacement(coords.position * number,
				coords.rotation * number);
		}

		public static Displacement operator *(float number, Displacement coords)
		{
			return coords * number;
		}

		public static Displacement operator /(Displacement coords, float number)
		{
			return new Displacement(coords.position / number,
				coords.rotation / number);
		}

		public static Displacement Scale(Displacement a, Displacement b)
		{
			return new Displacement(Vector2.Scale(a.position, b.position),
				b.rotation * a.rotation);
		}

		public static Displacement Lerp(Displacement a, Displacement b, float t)
		{
			return new Displacement(Vector3.Lerp(a.position, b.position, t),
				Mathf.Lerp(a.rotation, b.rotation, t));
		}

		public Displacement ScaledBy(float posScale, float rotScale)
			=> new Displacement(position * posScale, rotation * rotScale);
		public Displacement ScaledBy(Vector3 posScale, float rotScale)
		{
			return new Displacement(
				Vector3.Scale(position, posScale),
				rotation * rotScale);
		}

		public Displacement Normalized
		{
			get => new Displacement(position.normalized, rotation);
		}

		public static Displacement InsideUnitCircle()
			=> new(UnityEngine.Random.insideUnitCircle, UnityEngine.Random.Range(-1f, 1f));

		public override string ToString()
			=> $"Displacement: Position: {position} | Rotation: {rotation}";
	}


	public enum EShake
	{
		None,
		Normal,
		Small,
		Large
	}

	public class CameraController : Singleton<CameraController>
	{
		[Header("Shake")]
		[SerializeField] private bool canShake = true;
		[SerializeField] private Slider shakeSlider;
		[SerializeField, Range(0, 1)] private float shakeAmount = 0.5f;

		public void SetShake()
		{
			if (shakeSlider == null) return;
			shakeAmount = shakeSlider.value;
		}

		[Serializable]
		public class CameraShake
		{
			public EShake type;

			[Tooltip("Strength of the shake (x-y-shake).")]
			public float strength = 1;
			[Tooltip("Strength of the shake for rotation axis.")]
			public float rotationStrength = 2;
			[Tooltip("How long it takes to move forward.")]
			public float attackTime = 0.05f;
			public AnimationCurve attackCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
			[Tooltip("How long it takes to move back.")]
			public float releaseTime = 0.2f;
			public AnimationCurve releaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

			[Tooltip("Frequency of shaking.")]
			public float freq = 25;
			[Tooltip("Number of vibrations before stop.")]
			public int numBounces = 5;
			[Range(0, 1)]
			[Tooltip("Randomness of motion.")]
			public float randomness = 0.5f;
		}
		[SerializeField] private CameraShake[] shakes;

		private Vector3 position;
		private Quaternion rotation;


		protected override void Awake()
		{
			base.Awake();
			position = transform.position;
			rotation = transform.rotation;
		}

		public static void Shake(EShake type = EShake.Normal)
		{
			if (type == EShake.None) return;

			if (!IsAvailable(out var instance)) return;
			if (!instance.canShake) return;

			instance.StopAllCoroutines();
			instance.transform.SetPositionAndRotation(instance.position, instance.rotation);

			instance.StartCoroutine(instance.DoShake(Get(instance.shakes, type)));
		}

		private static CameraShake Get(CameraShake[] list, EShake type)
		{
			var asset = list.FirstOrDefault(e => e.type.Equals(type))
				?? throw new Exception($"Asset of type {type} not found in object list.");
			return asset;
		}

		private IEnumerator DoShake(CameraShake shake)
		{
			Displacement direction = Displacement.InsideUnitCircle();
			Displacement random = new(Vector2.one * shake.strength, shake.rotationStrength);
			Displacement waypoint = Displacement.Scale(direction, random);

			Displacement prevWaypoint = Displacement.zero;

			bool release = false;

			int bounces = 0;

			float T = shake.attackTime;
			while (true)
			{
				if (T < 0)
				{
					if (release)
						break;

					prevWaypoint = waypoint;

					bounces++;
					if (bounces > shake.numBounces)
					{
						release = true;
						waypoint = Displacement.zero;
						T = shake.releaseTime;
					}
					else
					{
						random = new(Vector2.one * shake.strength, shake.rotationStrength);
						direction = -direction + shake.randomness * random;
						direction = direction.Normalized;

						float decayValue = 1 - (float)bounces / (float)shake.numBounces;
						waypoint = decayValue * decayValue
							* direction.ScaledBy(shake.strength, shake.rotationStrength);

						T = shake.attackTime;
					}
				}

				float dt = Time.deltaTime;

				float duration = release ? shake.releaseTime : shake.attackTime;
				AnimationCurve curve = release ? shake.releaseCurve : shake.attackCurve;

				float t = 1 - T / duration;

				Displacement displacement = Displacement.Lerp(prevWaypoint, waypoint, curve.Evaluate(t));

				Vector3 offset = rotation * displacement.position;
				transform.position = position + offset * shakeAmount;
				transform.rotation = rotation * Quaternion.AngleAxis(displacement.rotation * shakeAmount, rotation * Vector3.forward);

				yield return new WaitForEndOfFrame();
				T -= dt;
			}
		}
	}

}
