using System.Collections.Generic;
using UnityEngine;

namespace laplahce.Projectiles
{

	[RequireComponent(typeof(Collider), typeof(Rigidbody))]
	public abstract class AProjectile : MonoBehaviour
	{
		[Tooltip("The particles to instantiate.")]
		[SerializeField] private Optional<Particle> muzzleParticle, hitParticle;

		[Tooltip("Whether to destroy after a set delay or not.")]
		[SerializeField] private Optional<float> destroyAfterDelay;

		[Tooltip("Speed at which the projectile should move.")]
		[SerializeField] protected float speed;
		public float Speed
		{
			get => speed;
		}
		public float Mass
		{
			get => rb.mass;
		}

		protected Rigidbody rb { get; private set; }

		protected bool HasHit { get; private set; } = false;

		private List<Particle> aliveParticles;


		protected virtual void Awake()
		{
			aliveParticles = new();
			HasHit = false;
			rb = GetComponent<Rigidbody>();
		}

		protected virtual void Start()
		{
			if (!muzzleParticle.Enabled) return;

			var particle = Instantiate(muzzleParticle.Value, transform.position, Quaternion.identity);
			particle.transform.forward = transform.forward;

			if (destroyAfterDelay.Enabled)
				Destroy(gameObject, destroyAfterDelay.Value);
		}

		protected virtual void FixedUpdate()
		{
			if (HasHit) return;
			Move(Time.fixedDeltaTime);
		}

		/// <summary>
		/// The actual move action.
		/// </summary>
		/// <param name="dt">is the delta time, so in most cases just Time.fixedDeltaTime</param>
		protected abstract void Move(float dt);
		/// <summary>
		/// What should happen when projectile hits a wall. Standard implementation is simply instantiating Hit particle if that is assigned.
		/// No need to implement this function.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="rot"></param>
		protected virtual void Hit(Vector3 pos, Quaternion rot)
		{
			if (HasHit) return;
			HasHit = true;

			rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			rb.isKinematic = true;
			foreach (var c in GetComponents<Collider>())
				c.enabled = false;

			foreach (var p in GetComponentsInChildren<Particle>())
			{
				aliveParticles.Add(p);
				p.enabled = true;
			}
			foreach (var t in GetComponentsInChildren<TrailParticle>())
				t.Fade();

			if (!hitParticle.Enabled) return;

			var particle = Instantiate(hitParticle.Value, pos, rot);
		}

		/// <summary>
		/// Rotates the projectile towards the velocity it's moving in.
		/// </summary>
		protected void Rotate()
		{
			// transform.forward = (transform.position - last).normalized;
			transform.forward = rb.linearVelocity.normalized;
		}


		public void RegisterParticleDestroyed(Particle particle)
		{
			aliveParticles.Remove(particle);
			if (aliveParticles.Count == 0)
				Destroy(gameObject);
		}


		private void OnCollisionEnter(Collision other)
		{
			if (HasHit) return;
			speed = 0;

			var contact = other.contacts[0];
			var rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
			var pos = contact.point;

			Hit(pos, rot);
		}
	}

}
