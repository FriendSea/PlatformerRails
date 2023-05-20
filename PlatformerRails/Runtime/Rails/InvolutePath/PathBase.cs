using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerRails
{
	public abstract class PathBase : RailBehaviour
	{
		public List<Vector3> ControlPoints;

		public bool Loop = false;

		public override float Length { get { return chainRail.Length; } }

		public IEnumerable<IRail> Segments { get { return chainRail.Segments; } }

		protected ChainRail chainRail = new ChainRail();

		protected virtual void Reset()
		{
			ControlPoints = new List<Vector3>
			{
				Vector3.zero,
				Vector3.forward,
				Vector3.forward +Vector3.right
			};
		}

		void Awake()
		{
			SetupPath();
		}

#if UNITY_EDITOR
		void OnValidate()
		{
			SetupPath();
		}
#endif

		//接続点の比率を再帰的に求める
		const float MinRate = 0.1f;
		float GetKnotRate(int index, int depth)
		{
			if (!Loop)
			{
				if (index == 0) return 0;
				if (index == ControlPoints.Count - 2) return 1f;
			}
			if (depth <= 0) return 0.5f;

			int leftindex = index - 1;
			if (leftindex < 0) leftindex += ControlPoints.Count;
			leftindex = leftindex % ControlPoints.Count;

			float leftRate = GetKnotRate(leftindex, depth - 1);
			float rightRate = GetKnotRate((index + 1) % ControlPoints.Count, depth - 1);

			Vector3 leftVec = ControlPoints[index] - ControlPoints[leftindex];
			Vector3 rightvec = ControlPoints[(index + 2) % ControlPoints.Count] - ControlPoints[(index + 1) % ControlPoints.Count];
			leftVec.y = 0;
			rightvec.y = 0;
			float leftlength = leftVec.magnitude * (1 - leftRate);
			float rightlength = rightvec.magnitude * rightRate;

			float value = leftlength / (leftlength + rightlength);
			value = Mathf.Clamp(value, MinRate, 1f - MinRate);
			return value;
		}

		//接続点の位置
		public Vector3 CalcKnot(int index)
		{
			float rate = GetKnotRate(index, 1);
			return Vector3.Lerp(ControlPoints[index], ControlPoints[(index + 1) % ControlPoints.Count], rate);
		}

		public Vector3 KnotTangent(int index)
		{
			return (ControlPoints[(index + 1) % ControlPoints.Count] - ControlPoints[index]).normalized;
		}

		//制御点に従ってレールを配置する
		public abstract void SetupPath();

		public override Vector3 Local2World(Vector3 LocalPosition)
		{
			if (!chainRail.isValid) SetupPath();
			if (Loop)
			{
				if (LocalPosition.z > Length) LocalPosition.z -= Length;
				if (LocalPosition.z < 0) LocalPosition.z += Length;
			}
			return chainRail.Local2World(LocalPosition);
		}

		public override Vector3? World2Local(Vector3 WorldPosition)
		{
			if (!chainRail.isValid) SetupPath();
			return chainRail.World2Local(WorldPosition);
		}

		public override Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail)
		{
			if (!chainRail.isValid) SetupPath();
			return chainRail.World2Local(WorldPosition, out usedSubrail);
		}

		public override Quaternion Rotation(float RailZ)
		{
			if (!chainRail.isValid) SetupPath();
			if (Loop)
			{
				if (RailZ > Length) RailZ -= Length;
				if (RailZ < 0) RailZ += Length;
			}
			return chainRail.Rotation(RailZ);
		}

		public override float Height(float RailZ)
		{
			if (!chainRail.isValid) SetupPath();
			if (Loop)
			{
				if (RailZ > Length) RailZ -= Length;
				if (RailZ < 0) RailZ += Length;
			}
			return chainRail.Height(RailZ);
		}
	}
}
