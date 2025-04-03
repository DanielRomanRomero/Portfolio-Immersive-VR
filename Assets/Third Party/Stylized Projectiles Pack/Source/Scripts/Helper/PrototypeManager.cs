using System.Linq;
using UnityEngine;

namespace laplahce.Projectiles
{

	public class PrototypeManager : MonoBehaviour
	{
		[Tooltip("All prototype parents.")]
		[SerializeField] private GameObject[] prototypes;
		private int index;
		private int Index
		{
			get => index;
			set
			{
				index = value % prototypes.Length;
				Switch();
			}
		}

		private void Awake()
		{
			Index = 0;
		}

		private void Start()
		{
			Cursor.visible = false;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return))
				Next();
		}

		private void Next() => Index++;

		private void Switch()
		{
			// Select prototype.
			var prototype = prototypes[index];
			prototype.SetActive(true);
			// Disable all other prototypes.
			prototypes.Except(new[] { prototype }).ToList().ForEach(c => c.SetActive(false));
		}
	}

}
