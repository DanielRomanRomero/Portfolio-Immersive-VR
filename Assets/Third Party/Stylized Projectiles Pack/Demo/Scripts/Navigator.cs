using System;
using UnityEngine;
using UnityEngine.UI;

namespace laplahce.Projectiles.Demo
{

	public class Navigator : Singleton<Navigator>
	{
		[SerializeField] private Text tabName;

		[SerializeField] private AProjectile[] projectiles;

		private string TabName => projectiles[Index].gameObject.name;
		private AProjectile Current => projectiles[Index];
		public static AProjectile GetCurrent()
		{
			if (!IsAvailable(out var instance))
				throw new Exception("Canvas does not have a navigator on it. Please re-download the demo scene from the package manager.");
			return instance.Current;
		}

		private int index;
		private int Index
		{
			get => index;
			set
			{
				index = ((value % projectiles.Length) + projectiles.Length) % projectiles.Length;
				IndexChanged();
			}
		}


		public void ChangeIndex(bool up)
		{
			Index += up ? 1 : -1;
		}


		protected override void Awake()
		{
			base.Awake();
			Index = 0;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				Index--;
			else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
				Index++;
		}


		private void IndexChanged()
		{
			tabName.text = TabName.ToUpper();
		}
	}

}
