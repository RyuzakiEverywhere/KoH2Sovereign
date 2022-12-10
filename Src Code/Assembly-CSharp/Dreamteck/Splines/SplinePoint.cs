using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamteck.Splines
{
	// Token: 0x020004CE RID: 1230
	[Serializable]
	public struct SplinePoint
	{
		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x0600416E RID: 16750 RVA: 0x001F22EB File Offset: 0x001F04EB
		// (set) Token: 0x0600416F RID: 16751 RVA: 0x001F22F3 File Offset: 0x001F04F3
		public SplinePoint.Type type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
				if (value == SplinePoint.Type.SmoothMirrored)
				{
					this.SmoothMirrorTangent2();
				}
			}
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x001F2308 File Offset: 0x001F0508
		public static SplinePoint Lerp(SplinePoint a, SplinePoint b, float t)
		{
			SplinePoint result = a;
			if (a.type == SplinePoint.Type.Broken || b.type == SplinePoint.Type.Broken)
			{
				result.type = SplinePoint.Type.Broken;
			}
			else if (a.type == SplinePoint.Type.SmoothFree || b.type == SplinePoint.Type.SmoothFree)
			{
				result.type = SplinePoint.Type.SmoothFree;
			}
			else
			{
				result.type = SplinePoint.Type.SmoothMirrored;
			}
			result.position = Vector3.Lerp(a.position, b.position, t);
			SplinePoint.GetInterpolatedTangents(a, b, t, out result.tangent, out result.tangent2);
			result.color = Color.Lerp(a.color, b.color, t);
			result.size = Mathf.Lerp(a.size, b.size, t);
			result.normal = Vector3.Slerp(a.normal, b.normal, t);
			return result;
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x001F23D8 File Offset: 0x001F05D8
		private static void GetInterpolatedTangents(SplinePoint a, SplinePoint b, float t, out Vector3 t1, out Vector3 t2)
		{
			Vector3 a2 = (1f - t) * a.position + t * a.tangent2;
			Vector3 a3 = (1f - t) * a.tangent2 + t * b.tangent;
			Vector3 a4 = (1f - t) * b.tangent + t * b.position;
			Vector3 vector = (1f - t) * a2 + t * a3;
			Vector3 vector2 = (1f - t) * a3 + t * a4;
			t1 = vector;
			t2 = vector2;
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x001F2498 File Offset: 0x001F0698
		public static bool AreDifferent(ref SplinePoint a, ref SplinePoint b)
		{
			return a.position != b.position || a.tangent != b.tangent || a.tangent2 != b.tangent2 || a.normal != b.normal || a.color != b.color || a.size != b.size || a.type != b.type;
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x001F2530 File Offset: 0x001F0730
		public void SetPosition(Vector3 pos)
		{
			this.tangent -= this.position - pos;
			this.tangent2 -= this.position - pos;
			this.position = pos;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x001F2580 File Offset: 0x001F0780
		public void SetTangentPosition(Vector3 pos)
		{
			this.tangent = pos;
			SplinePoint.Type type = this._type;
			if (type == SplinePoint.Type.SmoothMirrored)
			{
				this.SmoothMirrorTangent2();
				return;
			}
			if (type != SplinePoint.Type.SmoothFree)
			{
				return;
			}
			this.SmoothFreeTangent2();
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x001F25B0 File Offset: 0x001F07B0
		public void SetTangent2Position(Vector3 pos)
		{
			this.tangent2 = pos;
			SplinePoint.Type type = this._type;
			if (type == SplinePoint.Type.SmoothMirrored)
			{
				this.SmoothMirrorTangent();
				return;
			}
			if (type != SplinePoint.Type.SmoothFree)
			{
				return;
			}
			this.SmoothFreeTangent();
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x001F25E0 File Offset: 0x001F07E0
		public SplinePoint(Vector3 p)
		{
			this.position = p;
			this.tangent = p;
			this.tangent2 = p;
			this.color = Color.white;
			this.normal = Vector3.up;
			this.size = 1f;
			this._type = SplinePoint.Type.SmoothMirrored;
			this.SmoothMirrorTangent2();
		}

		// Token: 0x06004177 RID: 16759 RVA: 0x001F2630 File Offset: 0x001F0830
		public SplinePoint(Vector3 p, Vector3 t)
		{
			this.position = p;
			this.tangent = t;
			this.tangent2 = p + (p - t);
			this.color = Color.white;
			this.normal = Vector3.up;
			this.size = 1f;
			this._type = SplinePoint.Type.SmoothMirrored;
			this.SmoothMirrorTangent2();
		}

		// Token: 0x06004178 RID: 16760 RVA: 0x001F268C File Offset: 0x001F088C
		public SplinePoint(Vector3 pos, Vector3 tan, Vector3 nor, float s, Color col)
		{
			this.position = pos;
			this.tangent = tan;
			this.tangent2 = pos + (pos - tan);
			this.normal = nor;
			this.size = s;
			this.color = col;
			this._type = SplinePoint.Type.SmoothMirrored;
			this.SmoothMirrorTangent2();
		}

		// Token: 0x06004179 RID: 16761 RVA: 0x001F26E0 File Offset: 0x001F08E0
		public SplinePoint(Vector3 pos, Vector3 tan, Vector3 tan2, Vector3 nor, float s, Color col)
		{
			this.position = pos;
			this.tangent = tan;
			this.tangent2 = tan2;
			this.normal = nor;
			this.size = s;
			this.color = col;
			this._type = SplinePoint.Type.Broken;
			SplinePoint.Type type = this._type;
			if (type == SplinePoint.Type.SmoothMirrored)
			{
				this.SmoothMirrorTangent2();
				return;
			}
			if (type != SplinePoint.Type.SmoothFree)
			{
				return;
			}
			this.SmoothFreeTangent2();
		}

		// Token: 0x0600417A RID: 16762 RVA: 0x001F2740 File Offset: 0x001F0940
		public SplinePoint(SplinePoint source)
		{
			this.position = source.position;
			this.tangent = source.tangent;
			this.tangent2 = source.tangent2;
			this.color = source.color;
			this.normal = source.normal;
			this.size = source.size;
			this._type = source.type;
			SplinePoint.Type type = this._type;
			if (type == SplinePoint.Type.SmoothMirrored)
			{
				this.SmoothMirrorTangent2();
				return;
			}
			if (type != SplinePoint.Type.SmoothFree)
			{
				return;
			}
			this.SmoothFreeTangent2();
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x001F27BE File Offset: 0x001F09BE
		private void SmoothMirrorTangent2()
		{
			this.tangent2 = this.position + (this.position - this.tangent);
		}

		// Token: 0x0600417C RID: 16764 RVA: 0x001F27E2 File Offset: 0x001F09E2
		private void SmoothMirrorTangent()
		{
			this.tangent = this.position + (this.position - this.tangent2);
		}

		// Token: 0x0600417D RID: 16765 RVA: 0x001F2808 File Offset: 0x001F0A08
		private void SmoothFreeTangent2()
		{
			this.tangent2 = this.position + (this.position - this.tangent).normalized * (this.tangent2 - this.position).magnitude;
		}

		// Token: 0x0600417E RID: 16766 RVA: 0x001F2860 File Offset: 0x001F0A60
		private void SmoothFreeTangent()
		{
			this.tangent = this.position + (this.position - this.tangent2).normalized * (this.tangent - this.position).magnitude;
		}

		// Token: 0x04002D95 RID: 11669
		[FormerlySerializedAs("type")]
		[SerializeField]
		[HideInInspector]
		private SplinePoint.Type _type;

		// Token: 0x04002D96 RID: 11670
		public Vector3 position;

		// Token: 0x04002D97 RID: 11671
		public Color color;

		// Token: 0x04002D98 RID: 11672
		public Vector3 normal;

		// Token: 0x04002D99 RID: 11673
		public float size;

		// Token: 0x04002D9A RID: 11674
		public Vector3 tangent;

		// Token: 0x04002D9B RID: 11675
		public Vector3 tangent2;

		// Token: 0x020009B1 RID: 2481
		public enum Type
		{
			// Token: 0x04004523 RID: 17699
			SmoothMirrored,
			// Token: 0x04004524 RID: 17700
			Broken,
			// Token: 0x04004525 RID: 17701
			SmoothFree
		}
	}
}
