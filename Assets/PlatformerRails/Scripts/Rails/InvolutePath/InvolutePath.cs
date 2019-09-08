using System.Collections.Generic;
using UnityEngine;

namespace PlatformerRails
{
    public class InvolutePath : PathBase
    {
        //制御点に従ってレールを配置する
        public override void SetupPath()
        {
            chainRail.Clear();
            for (int i = 0; i < ControlPoints.Count - (Loop ? 0 : 2); i++)
            {
                Vector3 startpos = CalcKnot(i);
                float startHeight = startpos.y;
                startpos.y = 0;
                startpos = transform.TransformPoint(startpos);
                Vector3 starttangent = KnotTangent(i);
                float startgradient = starttangent.y / new Vector2(starttangent.x, starttangent.z).magnitude;
                starttangent.y = 0;
                Quaternion startrot = transform.rotation * Quaternion.LookRotation(starttangent, Vector3.up);

                Vector3 endpos = CalcKnot((i + 1) % ControlPoints.Count);
                float endHeight = endpos.y;
                endpos.y = 0;
                endpos = transform.TransformPoint(endpos);
                Vector3 endtangent = KnotTangent((i + 1) % ControlPoints.Count);
                float endgradient = endtangent.y / new Vector2(endtangent.x, endtangent.z).magnitude;
                endtangent.y = 0;
                Quaternion endrot = transform.rotation * Quaternion.LookRotation(endtangent, Vector3.up);

                if (Quaternion.Angle(startrot, endrot) < 1f)
                {
                    var newrail = new LerpRail(startpos, endpos, transform.up);
                    var func = new HermiteFunc(startHeight, startgradient * newrail.Length, endHeight, endgradient * newrail.Length);
                    newrail.HeightFunc = (z) => func.Hermite(z / newrail.Length);
                    chainRail.AddRail(newrail);
                }
                else
                {
                    var newrail = new InvoluteRail();
                    newrail.Interpolate(startpos, startrot, endpos, endrot);
                    var func = new HermiteFunc(startHeight, startgradient * newrail.Length, endHeight, endgradient * newrail.Length);
                    newrail.HeightFunc = (z) => func.Hermite(z / newrail.Length);
                    chainRail.AddRail(newrail);
                }
            }
        }

        struct HermiteFunc
        {
            float a, b, c, d;
            public float Hermite(float t)
            {
                return ((a * t + b) * t + c) * t + d;
            }

            public HermiteFunc(float v0, float t0, float v1, float t1)
            {
                a = 2 * (v0 - v1) + t0 + t1;
                b = -3 * (v0 - v1) - 2 * t0 - t1;
                c = t0;
                d = v0;
            }
        }
    }
}
