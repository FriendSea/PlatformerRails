using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace PlatformerRails
{
	[CustomEditor(typeof(InvolutePath))]
	public class InvolutePathEditor : PathEditor
	{
		ReorderableList reorderableList;		

		void OnEnable()
		{
			reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"));
			reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
			{
				var property = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(rect, property, GUIContent.none);
			};
			reorderableList.drawHeaderCallback += (rect) => {
				EditorGUI.LabelField(rect, reorderableList.serializedProperty.name);
			};
			reorderableList.onSelectCallback += (list) => {
				selected = list.index;
				SceneView.RepaintAll();
			};
		}

		public override void OnInspectorGUI()
		{
			var Path = target as InvolutePath;

			serializedObject.Update();
			reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();

			GUILayout.Label("Length: " + Path.Length);
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
					SceneView.RepaintAll();
				}
			}
		}

		int selected = -1;
		void OnSceneGUI()
		{
			var Path = target as InvolutePath;

			Handles.matrix = Path.transform.localToWorldMatrix;

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
