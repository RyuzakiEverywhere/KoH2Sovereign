using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004CF RID: 1231
	[Serializable]
	public class SplineSample
	{
		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x0600417F RID: 16767 RVA: 0x001F28B8 File Offset: 0x001F0AB8
		public Quaternion rotation
		{
			get
			{
				if (!(this.up == this.forward))
				{
					return Quaternion.LookRotation(this.forward, this.up);
				}
				if (this.up == Vector3.up)
				{
					return Quaternion.LookRotation(Vector3.up, Vector3.back);
				}
				return Quaternion.LookRotation(this.forward, Vector3.up);
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06004180 RID: 16768 RVA: 0x001F291C File Offset: 0x001F0B1C
		public Vector3 right
		{
			get
			{
				if (!(this.up == this.forward))
				{
					return Vector3.Cross(this.up, this.forward).normalized;
				}
				if (this.up == Vector3.up)
				{
					return Vector3.right;
				}
				return Vector3.Cross(Vector3.up, this.forward).normalized;
			}
		}

		// Token: 0x06004181 RID: 16769 RVA: 0x001F2988 File Offset: 0x001F0B88
		public static SplineSample Lerp(SplineSample a, SplineSample b, float t)
		{
			SplineSample splineSample = new SplineSample();
			SplineSample.Lerp(a, b, t, splineSample);
			return splineSample;
		}

		// Token: 0x06004182 RID: 16770 RVA: 0x001F29A8 File Offset: 0x001F0BA8
		public static SplineSample Lerp(SplineSample a, SplineSample b, double t)
		{
			SplineSample splineSample = new SplineSample();
			SplineSample.Lerp(a, b, t, splineSample);
			return splineSample;
		}

		// Token: 0x06004183 RID: 16771 RVA: 0x001F29C8 File Offset: 0x001F0BC8
		public static void Lerp(SplineSample a, SplineSample b, double t, SplineSample target)
		{
			float t2 = (float)t;
			target.position = DMath.LerpVector3(a.position, b.position, t);
			target.forward = Vector3.Slerp(a.forward, b.forward, t2);
			target.up = Vector3.Slerp(a.up, b.up, t2);
			target.color = Color.Lerp(a.color, b.color, t2);
			target.size = Mathf.Lerp(a.size, b.size, t2);
			target.percent = DMath.Lerp(a.percent, b.percent, t);
		}

		// Token: 0x06004184 RID: 16772 RVA: 0x001F2A68 File Offset: 0x001F0C68
		public static void Lerp(SplineSample a, SplineSample b, float t, SplineSample target)
		{
			target.position = DMath.LerpVector3(a.position, b.position, (double)t);
			target.forward = Vector3.Slerp(a.forward, b.forward, t);
			target.up = Vector3.Slerp(a.up, b.up, t);
			target.color = Color.Lerp(a.color, b.color, t);
			target.size = Mathf.Lerp(a.size, b.size, t);
			target.percent = DMath.Lerp(a.percent, b.percent, (double)t);
		}

		// Token: 0x06004185 RID: 16773 RVA: 0x001F2B07 File Offset: 0x001F0D07
		public void Lerp(SplineSample b, double t)
		{
			SplineSample.Lerp(this, b, t, this);
		}

		// Token: 0x06004186 RID: 16774 RVA: 0x001F2B12 File Offset: 0x001F0D12
		public void Lerp(SplineSample b, float t)
		{
			SplineSample.Lerp(this, b, t, this);
		}

		// Token: 0x06004187 RID: 16775 RVA: 0x001F2B20 File Offset: 0x001F0D20
		public void CopyFrom(SplineSample input)
		{
			this.position = input.position;
			this.forward = input.forward;
			this.up = input.up;
			this.color = input.color;
			this.size = input.size;
			this.percent = input.percent;
		}

		// Token: 0x06004188 RID: 16776 RVA: 0x001F2B75 File Offset: 0x001F0D75
		public SplineSample()
		{
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x001F2BB4 File Offset: 0x001F0DB4
		public SplineSample(Vector3 position, Vector3 normal, Vector3 direction, Color color, float size, double percent)
		{
			this.position = position;
			this.up = normal;
			this.forward = direction;
			this.color = color;
			this.size = size;
			this.percent = percent;
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x001F2C2C File Offset: 0x001F0E2C
		public SplineSample(SplineSample input)
		{
			this.position = input.position;
			this.up = input.up;
			this.forward = input.forward;
			this.color = input.color;
			this.size = input.size;
			this.percent = input.percent;
		}

		// Token: 0x04002D9C RID: 11676
		public Vector3 position = Vector3.zero;

		// Token: 0x04002D9D RID: 11677
		public Vector3 up = Vector3.up;

		// Token: 0x04002D9E RID: 11678
		public Vector3 forward = Vector3.forward;

		// Token: 0x04002D9F RID: 11679
		public Color color = Color.white;

		// Token: 0x04002DA0 RID: 11680
		public float size = 1f;

		// Token: 0x04002DA1 RID: 11681
		public double percent;
	}
}
