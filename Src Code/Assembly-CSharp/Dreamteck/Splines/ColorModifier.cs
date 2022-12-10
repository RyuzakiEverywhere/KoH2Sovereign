using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B7 RID: 1207
	[Serializable]
	public class ColorModifier : SplineSampleModifier
	{
		// Token: 0x06003F89 RID: 16265 RVA: 0x001E5B94 File Offset: 0x001E3D94
		public ColorModifier()
		{
			this.keys = new List<ColorModifier.ColorKey>();
		}

		// Token: 0x06003F8A RID: 16266 RVA: 0x001E5BB4 File Offset: 0x001E3DB4
		public override List<SplineSampleModifier.Key> GetKeys()
		{
			List<SplineSampleModifier.Key> list = new List<SplineSampleModifier.Key>();
			for (int i = 0; i < this.keys.Count; i++)
			{
				list.Add(this.keys[i]);
			}
			return list;
		}

		// Token: 0x06003F8B RID: 16267 RVA: 0x001E5BF0 File Offset: 0x001E3DF0
		public override void SetKeys(List<SplineSampleModifier.Key> input)
		{
			this.keys = new List<ColorModifier.ColorKey>();
			for (int i = 0; i < input.Count; i++)
			{
				this.keys.Add((ColorModifier.ColorKey)input[i]);
			}
			base.SetKeys(input);
		}

		// Token: 0x06003F8C RID: 16268 RVA: 0x001E5C37 File Offset: 0x001E3E37
		public void AddKey(double f, double t)
		{
			this.keys.Add(new ColorModifier.ColorKey(f, t, this));
		}

		// Token: 0x06003F8D RID: 16269 RVA: 0x001E5C4C File Offset: 0x001E3E4C
		public override void Apply(SplineSample result)
		{
			if (this.keys.Count == 0)
			{
				return;
			}
			base.Apply(result);
			for (int i = 0; i < this.keys.Count; i++)
			{
				result.color = this.keys[i].Blend(result.color, this.keys[i].Evaluate(result.percent));
			}
		}

		// Token: 0x04002CE4 RID: 11492
		public List<ColorModifier.ColorKey> keys = new List<ColorModifier.ColorKey>();

		// Token: 0x02000993 RID: 2451
		[Serializable]
		public class ColorKey : SplineSampleModifier.Key
		{
			// Token: 0x0600543B RID: 21563 RVA: 0x00246009 File Offset: 0x00244209
			public ColorKey(double f, double t, ColorModifier modifier) : base(f, t, modifier)
			{
			}

			// Token: 0x0600543C RID: 21564 RVA: 0x00246020 File Offset: 0x00244220
			public Color Blend(Color input, float percent)
			{
				switch (this.blendMode)
				{
				case ColorModifier.ColorKey.BlendMode.Lerp:
					return Color.Lerp(input, this.color, this.blend * percent);
				case ColorModifier.ColorKey.BlendMode.Multiply:
					return Color.Lerp(input, input * this.color, this.blend * percent);
				case ColorModifier.ColorKey.BlendMode.Add:
					return input + this.color * this.blend * percent;
				case ColorModifier.ColorKey.BlendMode.Subtract:
					return input - this.color * this.blend * percent;
				default:
					return input;
				}
			}

			// Token: 0x04004499 RID: 17561
			public Color color = Color.white;

			// Token: 0x0400449A RID: 17562
			public ColorModifier.ColorKey.BlendMode blendMode;

			// Token: 0x02000A42 RID: 2626
			public enum BlendMode
			{
				// Token: 0x0400471D RID: 18205
				Lerp,
				// Token: 0x0400471E RID: 18206
				Multiply,
				// Token: 0x0400471F RID: 18207
				Add,
				// Token: 0x04004720 RID: 18208
				Subtract
			}
		}
	}
}
