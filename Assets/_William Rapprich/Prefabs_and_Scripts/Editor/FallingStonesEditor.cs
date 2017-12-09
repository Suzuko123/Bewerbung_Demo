using UnityEditor;
using UnityEngine;

//Author: William Rapprich
//Last edited: 6.12.2017 by: William
[CustomEditor(typeof(FallingStones))]
[CanEditMultipleObjects]
public class FallingStonesEditor : Editor
{
	SerializedProperty widthProp, heightProp, depthProp, 
		colorProp, stoneProp, spawnProp, activeTimeProp,
		radiusProp, varProp, damageProp;
	float standardWidth;

	void OnEnable()
	{
		standardWidth = EditorGUIUtility.labelWidth;
		widthProp = serializedObject.FindProperty("width");
		heightProp = serializedObject.FindProperty("height");
		depthProp = serializedObject.FindProperty("depth");
		colorProp = serializedObject.FindProperty("boxColor");
		spawnProp = serializedObject.FindProperty("spawnInterval");
		activeTimeProp = serializedObject.FindProperty("dangerZoneActiveTime");
		radiusProp = serializedObject.FindProperty("dangerZoneRadius");
		damageProp = serializedObject.FindProperty("damage");
		stoneProp = serializedObject.FindProperty("stonePrefab");
		varProp = serializedObject.FindProperty("radialVariety");
	}

	override public void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Area:");

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Dimensions", GUILayout.MinWidth(65));
		EditorGUIUtility.labelWidth = 12;
		EditorGUILayout.PropertyField(widthProp, new GUIContent("W"), GUILayout.MinWidth(50));
		EditorGUILayout.PropertyField(heightProp, new GUIContent("H"), GUILayout.MinWidth(50));
		EditorGUILayout.PropertyField(depthProp, new GUIContent("D"), GUILayout.MinWidth(50));

		EditorGUILayout.EndHorizontal();

		EditorGUIUtility.labelWidth = standardWidth;
		EditorGUILayout.PropertyField(colorProp);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Effect:");

		EditorGUILayout.PropertyField(stoneProp);

		EditorGUILayout.BeginHorizontal();
		EditorGUIUtility.labelWidth = standardWidth;
		EditorGUILayout.LabelField("Danger Zone", GUILayout.MinWidth(70));
		EditorGUIUtility.labelWidth = 40;
		EditorGUILayout.PropertyField(radiusProp, new GUIContent("radius"), GUILayout.MinWidth(50));
		EditorGUILayout.PropertyField(varProp, new GUIContent("variety"), GUILayout.MinWidth(50));
		EditorGUILayout.EndHorizontal();

		EditorGUIUtility.labelWidth = standardWidth;
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Time", GUILayout.MinWidth(40));
		EditorGUIUtility.labelWidth = 40;
		EditorGUILayout.PropertyField(spawnProp, new GUIContent("spawn"), GUILayout.MinWidth(50));
		EditorGUILayout.PropertyField(activeTimeProp, new GUIContent("active"), GUILayout.MinWidth(50));

		EditorGUILayout.EndHorizontal();

		EditorGUIUtility.labelWidth = standardWidth;
		EditorGUILayout.PropertyField(damageProp);

		if (spawnProp.floatValue < 0f)
		{
			spawnProp.floatValue = 0f;
		}
		if (activeTimeProp.floatValue < 0f)
		{
			activeTimeProp.floatValue = 0f;
		}
		if(radiusProp.floatValue < 0f)
		{
			radiusProp.floatValue = 0f;
		}
		if(damageProp.intValue < 0)
		{
			damageProp.intValue = 0;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
