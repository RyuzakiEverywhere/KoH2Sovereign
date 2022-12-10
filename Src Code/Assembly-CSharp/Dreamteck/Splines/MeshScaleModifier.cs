using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B8 RID: 1208
	[Serializable]
	public class MeshScaleModifier : SplineSampleModifier
	{
		// Token: 0x06003F8E RID: 16270 RVA: 0x001E5CB8 File Offset: 0x001E3EB8
		public MeshScaleModifier()
		{
			this.keys = new List<MeshScaleModifier.ScaleKey>();
		}

		// Token: 0x06003F8F RID: 16271 RVA: 0x001E5CD8 File Offset: 0x001E3ED8
		public override List<SplineSampleModifier.Key> GetKeys()
		{
			List<SplineSampleModifier.Key> list = new List<SplineSampleModifier.Key>();
			for (int i = 0; i < this.keys.Count; i++)
			{
				list.Add(this.keys[i]);
			}
			return list;
		}

		// Token: 0x06003F90 RID: 16272 RVA: 0x001E5D14 File Offset: 0x001E3F14
		public override void SetKeys(List<SplineSampleModifier.Key> input)
		{
			this.keys = new List<MeshScaleModifier.ScaleKey>();
			for (int i = 0; i < input.Count; i++)
			{
				input[i].modifier = this;
				this.keys.Add((MeshScaleModifier.ScaleKey)input[i]);
			}
		}

		// Token: 0x06003F91 RID: 16273 RVA: 0x001E5D61 File Offset: 0x001E3F61
		public void AddKey(double f, double t)
		{
			this.keys.Add(new MeshScaleModifier.ScaleKey(f, t, this));
		}

		// Token: 0x06003F92 RID: 16274 RVA: 0x001E5D78 File Offset: 0x001E3F78
		public override void Apply(SplineSample result)
		{
			if (this.keys.Count == 0)
			{
				return;
			}
			for (int i = 0; i < this.keys.Count; i++)
			{
				result.size += this.keys[i].Evaluate(result.percent) * this.keys[i].scale.magnitude;
			}
		}

		// Token: 0x06003F93 RID: 16275 RVA: 0x001E5DE4 File Offset: 0x001E3FE4
		public Vector2 GetScale(SplineSample sample)
		{
			Vector2 one = Vector2.one;
			for (int i = 0; i < this.keys.Count; i++)
			{
				float t = this.keys[i].Evaluate(sample.percent);
				Vector2 vector = Vector2.Lerp(Vector2.one, this.keys[i].scale, t);
				one.x *= vector.x;
				one.y *= vector.y;
			}
			return one;
		}

		// Token: 0x04002CE5 RID: 11493
		public List<MeshScaleModifier.ScaleKey> keys = new List<MeshScaleModifier.ScaleKey>();

		// Token: 0x02000994 RID: 2452
		[Serializable]
		public class ScaleKey : SplineSampleModifier.Key
		{
			// Token: 0x0600543D RID: 21565 RVA: 0x002460B9 File Offset: 0x002442B9
			public ScaleKey(double f, double t, MeshScaleModifier modifier) : base(f, t, modifier)
			{
			}

			// Token: 0x0400449B RID: 17563
			public Vector2 scale = Vector2.one;
		}
	}
}
