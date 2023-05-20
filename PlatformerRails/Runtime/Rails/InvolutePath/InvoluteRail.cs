using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlatformerRails
{
    public class InvoluteRail : IRail
    {
        Vector3 StartPoint;
        Quaternion StartRotation;
		Quaternion EndRotation;
        float a, b;
        float EndRadian;

        Vector3 UnitU;
        Vector3 UnitV;
        Vector3 UnitW;

        public System.Func<float, float> HeightFunc { private get; set; }
        public float Height(float RailZ)
        {
            return HeightFunc(RailZ);
        }

        public float Length { get { return Radian2Length(EndRadian); } }

        public bool isValid { get { return a * (a + b * EndRadian) >= 0; } }

        public Vector3 Local2World(Vector3 LocalPosition)
        {
            float clampedZ = Mathf.Clamp(LocalPosition.z, 0, Length);
            float rad = Length2Radian(clampedZ);
            LocalPosition.z -= clampedZ;
            return Position(rad) + RotationByRadian(rad) * LocalPosition;
        }

        public Quaternion Rotation(float RailZ)
        {
            return RotationByRadian(Length2Radian(RailZ));
        }

        public Vector3? World2Local(Vector3 WorldPosition)
        {
            float rad = NearestRadian(WorldPosition);
            if (float.IsNaN(rad)) return null;
            return Quaternion.Inverse(RotationByRadian(rad)) * (WorldPosition - Position(rad)) + Vector3.forward * Radian2Length(rad);
        }

        public Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail)
        {
            usedSubrail = this;
            return World2Local(WorldPosition);
        }

            float Radian2Length(float radian)
        {
            return a * radian + b * radian * radian / 2f;
        }

        float Length2Radian(float length)
        {
            if (Mathf.Abs(b) < 0.01f)
                return length / a;
            double rad = (-a + Mathf.Sqrt(a * a + 2 * b * length)) / (double)b;
            return (float)rad;
        }

        float NearestRadian(Vector3 point)
        {
            Vector3 C = point - StartPoint;
            Vector2 PlanerVec = new Vector2(Vector3.Dot(C, UnitU), Vector3.Dot(C, UnitV));
            float length = Mathf.Sqrt((PlanerVec.y - a) * (PlanerVec.y - a) + (PlanerVec.x + b) * (PlanerVec.x + b));
            float radian;
            float beta = Mathf.Atan((PlanerVec.y - a) / (PlanerVec.x + b));
            radian = Mathf.Acos(b / length) + beta;
            if (radian > Mathf.PI) radian -= Mathf.PI;
            if (radian < 0) radian += Mathf.PI;
            return radian;
        }

        Quaternion RotationByRadian(float radian)
        {
			return Quaternion.AngleAxis(radian * Mathf.Rad2Deg, UnitW) * Quaternion.Lerp(StartRotation, EndRotation, radian / EndRadian);
        }

        Vector3 Position(float radian)
        {
            float y = b * Mathf.Sin(radian) - Mathf.Cos(radian) * (a + b * radian) + a;
            float x = b * Mathf.Cos(radian) + Mathf.Sin(radian) * (a + b * radian) - b;
            return x * UnitU + y * UnitV + StartPoint;
        }

        static List<float> SolveEquation(List<List<float>> mat)
        {
            for (int i = 0; i < mat.Count; i++)
            {
                float p = mat[i][i];
                for (int j = 0; j < mat[i].Count; j++)
                {
                    mat[i][j] = mat[i][j] / p;
                }

                for (int j = 0; j < mat.Count; j++)
                {
                    if (j == i) continue;
                    float d = mat[j][i];
                    for (int k = 0; k < mat[j].Count; k++)
                    {
                        mat[j][k] = mat[j][k] - mat[i][k] * d;
                    }
                }
            }
            return mat.Select(l => l.Last()).ToList();
        }

        void CalcParamater(Vector3 endpoint, Quaternion endrotation)
        {
            UnitU = StartRotation * Vector3.forward;
            UnitW = Vector3.Cross(UnitU, endpoint - StartPoint).normalized;
            UnitV = Vector3.Cross(UnitW, UnitU);
            Vector3 EndTangent = (endrotation * Vector3.forward - Vector3.Dot(endrotation * Vector3.forward, UnitW) * UnitW).normalized;
            EndRadian = Vector3.Angle(UnitU, EndTangent) * Mathf.Deg2Rad;

            float a11 = Mathf.Sin(EndRadian);
            float a12 = EndRadian * Mathf.Sin(EndRadian) + Mathf.Cos(EndRadian) - 1;
            float a21 = 1 - Mathf.Cos(EndRadian);
            float a22 = Mathf.Sin(EndRadian) - EndRadian * Mathf.Cos(EndRadian);
            var mat = new List<List<float>>{
            new List<float>{a11, a12, Vector3.Dot(UnitU, endpoint-StartPoint)},
            new List<float>{a21, a22, Vector3.Dot(UnitV, endpoint-StartPoint)},
            };
            var result = SolveEquation(mat);
            a = result[0];
            b = result[1];
        }

        public void Interpolate(Vector3 startPoint, Quaternion startRotation, Vector3 endPoint, Quaternion endRotation)
        {
            StartPoint = startPoint;
            StartRotation = startRotation;
            CalcParamater(endPoint, endRotation);
			EndRotation = Quaternion.AngleAxis(-EndRadian * Mathf.Rad2Deg, UnitW) * endRotation;
			//EndRotation = StartRotation;

		}
    }
}
