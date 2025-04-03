using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace laplahce.Projectiles
{

	[Serializable]
	public struct Optional<T>
	{
		[SerializeField] private bool enabled;
		[SerializeField] private T value;

		public Optional(T init)
		{
			enabled = true;
			value = init;
		}
		public Optional(T init, bool enabled)
		{
			this.enabled = enabled;
			value = init;
		}

		public void Set(T value)
		{
			this.value = value;
		}

		public readonly bool Enabled => enabled;
		public readonly T Value => value;
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(Optional<>))]
	public class OptionalPropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var valueProp = property.FindPropertyRelative("value");
			return EditorGUI.GetPropertyHeight(valueProp);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var valueProp = property.FindPropertyRelative("value");
			var enabledProp = property.FindPropertyRelative("enabled");

			bool isGeneric = valueProp.propertyType == SerializedPropertyType.Generic;

			position.width -= (EditorGUI.indentLevel + 1) * 24 + (isGeneric ? 1 : 0) * 26;
			EditorGUI.BeginDisabledGroup(!enabledProp.boolValue);
			EditorGUI.PropertyField(position, valueProp, label, true);
			EditorGUI.EndDisabledGroup();

			position.x += position.width + (EditorGUI.indentLevel + 1) * 24;
			position.width = position.height = EditorGUI.GetPropertyHeight(enabledProp);
			position.x -= position.width;
			EditorGUI.PropertyField(position, enabledProp, GUIContent.none);
		}
	}
#endif

}