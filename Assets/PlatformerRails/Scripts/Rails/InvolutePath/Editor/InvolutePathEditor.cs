using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;

namespace PlatformerRails
{
    [CustomEditor(typeof(InvolutePath))]
    public class InvolutePathEditor : Editor
    {

        ReorderableList ControlPoints;

        void OnEnable()
        {
            var Path = target as InvolutePath;

            ControlPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"));

            ControlPoints.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                Vector3 pos = EditorGUI.Vector3Field(rect, GUIContent.none, Path.ControlPoints[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Path, "Change Control Point");
                    EditorUtility.SetDirty(Path);
                    Path.ControlPoints[index] = pos;
                }
            };

            ControlPoints.onChangedCallback = (list) => SceneView.RepaintAll();

            ControlPoints.onSelectCallback = (list) =>
            {
                selected = ControlPoints.index;
                SceneView.RepaintAll();
            };
        }

        public override void OnInspectorGUI()
        {
            var Path = target as InvolutePath;

            serializedObject.Update();
            ControlPoints.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginChangeCheck();
            bool loop = GUILayout.Toggle(Path.Loop, "Looping", "button");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Path, "Change Looping");
                Path.Loop = loop;
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("InvertX"))
            {
                Undo.RecordObject(Path, "Invert X");
                Path.ControlPoints = Path.ControlPoints.Select(v => {
                    v.x *= -1f;
                    return v;
                }).ToList();
                SceneView.RepaintAll();
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


        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void DrawControlPoints(InvolutePath Path, GizmoType type)
        {
            Gizmos.matrix = Path.transform.localToWorldMatrix;

            if (Path.Loop)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(Path.ControlPoints[0], Path.ControlPoints[Path.ControlPoints.Count - 1]);
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < Path.ControlPoints.Count; i++)
            {
                Vector3 pos = Path.CalcKnot(i);
                Gizmos.DrawSphere(pos, HandleUtility.GetHandleSize(Path.transform.TransformPoint(pos)) * 0.05f);
            }

            Gizmos.matrix = Matrix4x4.identity;

            DrawPath(Path, 100);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        static void DrawLowResPath(InvolutePath Path, GizmoType type)
        {
            DrawPath(Path, 20);
        }

        const float DrawWidth = 0.25f;
        static void DrawPath(InvolutePath Path, int resolution)
        {
            foreach (var rail in Path.Segments)
            {
                Gizmos.color = Color.green;
                if (rail is InvoluteRail)
                    Gizmos.color = ((InvoluteRail)rail).isValid ? Color.green : Color.magenta;
                Vector3 beforepos = rail.Local2World(Vector3.zero) + Path.transform.up * rail.Height(0);
                for (int i = 0; i <= resolution; i++)
                {
                    float z = (float)i / resolution * rail.Length;
                    Vector3 pos = rail.Local2World(Vector3.forward * z) + Path.transform.up * rail.Height(z);
                    Gizmos.DrawLine(beforepos, pos);
                    beforepos = pos;
                }
                beforepos = rail.Local2World(Vector3.right * DrawWidth) + Path.transform.up * rail.Height(0);
                for (int i = 0; i <= resolution; i++)
                {
                    float z = (float)i / resolution * rail.Length;
                    Vector3 pos = rail.Local2World(Vector3.right * DrawWidth + Vector3.forward * z) + Path.transform.up * rail.Height(z);
                    Gizmos.DrawLine(beforepos, pos);
                    beforepos = pos;
                }
                beforepos = rail.Local2World(-Vector3.right * DrawWidth) + Path.transform.up * rail.Height(0);
                for (int i = 0; i <= resolution; i++)
                {
                    float z = (float)i / resolution * rail.Length;
                    Vector3 pos = rail.Local2World(-Vector3.right * DrawWidth + Vector3.forward * z) + Path.transform.up * rail.Height(z);
                    Gizmos.DrawLine(beforepos, pos);
                    beforepos = pos;
                }
            }
        }
    }
}
