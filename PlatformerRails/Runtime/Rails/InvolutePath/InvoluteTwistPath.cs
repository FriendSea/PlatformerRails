using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerRails
{
	public class InvoluteTwistPath : PathBase
	{
		public List<Vector3> UpWards;

		protected override void Reset()
		{
			base.Reset();
			UpWards = new List<Vector3>
			{
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
		}

		public override void SetupPath()
		{
			chainRail.Clear();
			for (int i = 0; i < ControlPoints.Count - (Loop ? 0 : 2); i++)
			{
				Vector3 startpos = CalcKnot(i);
				startpos = transform.TransformPoint(startpos);
				Vector3 starttangent = KnotTangent(i);
				Quaternion startrot = transform.rotation * Quaternion.LookRotation(starttangent, UpWards[i]);

				Vector3 endpos = CalcKnot((i + 1) % ControlPoints.Count);
				endpos = transform.TransformPoint(endpos);
				Vector3 endtangent = KnotTangent((i + 1) % ControlPoints.Count);
				Quaternion endrot = transform.rotation * Quaternion.LookRotation(endtangent, UpWards[(i + 1) % UpWards.Count]);

				if (Quaternion.Angle(startrot, endrot) < 1f)
				{
					var newrail = new LerpRail(startpos, endpos, transform.up);
					newrail.HeightFunc = (z) => 0;
					chainRail.AddRail(newrail);
				}
				else
				{
					var newrail = new InvoluteRail();
					newrail.Interpolate(startpos, startrot, endpos, endrot);
					newrail.HeightFunc = (z) => 0;
					chainRail.AddRail(newrail);
				}
			}
		}
	}
}
