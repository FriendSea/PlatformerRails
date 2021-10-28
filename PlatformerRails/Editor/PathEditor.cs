using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PlatformerRails
{
	public class PathEditor : Editor
	{
		[DrawGizmo(GizmoType.InSelectionHierarchy)]
		static void DrawControlPoints(PathBase Path, GizmoType type)
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
		static void DrawLowResPath(PathBase Path, GizmoType type)
		{
			DrawPath(Path, 20);
		}

		const float DrawWidth = 0.25f;
		static void DrawPath(PathBase Path, int resolution)
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
