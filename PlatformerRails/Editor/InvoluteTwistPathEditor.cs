using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PlatformerRails
{
	[CustomEditor(typeof(InvoluteTwistPath))]
	public class InvoluteTwistPathEditor : PathEditor
	{
		ReorderableList reorderableList;

		void OnEnable()
		{
			reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"));
			var UpWards = serializedObject.FindProperty("UpWards");
			var loop = serializedObject.FindProperty("Loop");

			reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3;
			reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
			{
				var property = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				var upward = UpWards.GetArrayElementAtIndex(index);

				EditorGUI.HelpBox(rect, "", MessageType.None);

				rect.y += EditorGUIUtility.standardVerticalSpacing;
				rect.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(rect, property, new GUIContent("Position"));

				rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				using (new EditorGUI.DisabledScope(!loop.boolValue && index == UpWards.arraySize - 1))
					EditorGUI.PropertyField(rect, upward, new GUIContent("UpWard"));
			};
			reorderableList.onAddCallback += (list) =>
			{
				UpWards.InsertArrayElementAtIndex(UpWards.arraySize);
				reorderableList.serializedProperty.InsertArrayElementAtIndex(reorderableList.serializedProperty.arraySize);
			};
			reorderableList.onRemoveCallback += (list) =>
			{
				UpWards.DeleteArrayElementAtIndex(reorderableList.index);
				reorderableList.serializedProperty.DeleteArrayElementAtIndex(reorderableList.index);
			};
			reorderableList.drawHeaderCallback += (rect) =>
			{
				EditorGUI.LabelField(rect, reorderableList.serializedProperty.name);
			};
			reorderableList.onSelectCallback += (list) =>
			{
				selected = list.index;
				SceneView.RepaintAll();
			};
		}

		public override void OnInspectorGUI()
		{
			var Path = target as InvoluteTwistPath;

			serializedObject.Update();
			reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();

			EditorGUI.BeginChangeCheck();
			bool loop = GUILayout.Toggle(Path.Loop, "Looping", "button");
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(Path, "Change Looping");
				Path.Loop = loop;
				SceneView.RepaintAll();
			}
			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("InvertX"))
				{
					Undo.RecordObject(Path, "Invert X");
					Path.ControlPoints = Path.ControlPoints.Select(v =>
					{
						v.x *= -1f;
						return v;
					}).ToList();
					Path.UpWards = Path.UpWards.Select(v =>
					{
						v.x *= -1f;
						return v;
					}).ToList();
					SceneView.RepaintAll();
				}
				if (GUILayout.Button("InvertY"))
				{
					Undo.RecordObject(Path, "Invert Y");
					Path.ControlPoints = Path.ControlPoints.Select(v =>
					{
						v.y *= -1f;
						return v;
					}).ToList();
					Path.UpWards = Path.UpWards.Select(v =>
					{
						v.y *= -1f;
						return v;
					}).ToList();
					SceneView.RepaintAll();
				}
				if (GUILayout.Button("InvertZ"))
				{
					Undo.RecordObject(Path, "Invert Z");
					Path.ControlPoints = Path.ControlPoints.Select(v =>
					{
						v.z *= -1f;
						return v;
					}).ToList();
					Path.UpWards = Path.UpWards.Select(v =>
					{
						v.z *= -1f;
						return v;
					}).ToList();
					SceneView.RepaintAll();
				}
			}
		}

		int selected = -1;
		void OnSceneGUI()
		{
			var Path = target as InvoluteTwistPath;

			Handles.matrix = Path.transform.localToWorldMatrix;
			Handles.color = Color.white;

			for (int i = 0; i < Path.UpWards.Count - (Path.Loop ? 0 : 1); i++)
			{
				var pos = Path.CalcKnot(i);
				Handles.color = Color.green;
				Handles.DrawLine(pos, pos + Path.UpWards[i]);
				Handles.color = Color.white;
				EditorGUI.BeginChangeCheck();
				var tangent = Path.KnotTangent(i);
				Quaternion rot = Handles.Disc(Quaternion.LookRotation(tangent, Path.UpWards[i]), pos, tangent, 1, false, 10);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(Path, "Change Upward");
					EditorUtility.SetDirty(Path);
					Path.UpWards[i] = rot * Vector3.up;
				}
			}
			Handles.color = Color.white;

			for (int i = 0; i < Path.ControlPoints.Count; i++)
			{
				float size = HandleUtility.GetHandleSize(Path.transform.TransformPoint(Path.ControlPoints[i]));
				if (Handles.Button(Path.ControlPoints[i], Path.transform.rotation, size * 0.05f, size * 0.05f, Handles.DotHandleCap))
					selected = i;
				if (selected == i)
				{
					EditorGUI.BeginChangeCheck();
					Vector3 pos = Handles.PositionHandle(Path.ControlPoints[i], Quaternion.identity);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(Path, "Move Control Point");
						EditorUtility.SetDirty(Path);
						Path.ControlPoints[i] = pos;
					}
				}
			}

			Handles.DrawAAPolyLine(5f, Path.ControlPoints.ToArray());

			Handles.matrix = Matrix4x4.identity;

			Path.SetupPath();
		}
	}
}
