using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B9 RID: 1209
	[Serializable]
	public class OffsetModifier : SplineSampleModifier
	{
		// Token: 0x06003F94 RID: 16276 RVA: 0x001E5E65 File Offset: 0x001E4065
		public OffsetModifier()
		{
			this.keys = new List<OffsetModifier.OffsetKey>();
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x001E5E84 File Offset: 0x001E4084
		public override List<SplineSampleModifier.Key> GetKeys()
		{
			List<SplineSampleModifier.Key> list = new List<SplineSampleModifier.Key>();
			for (int i = 0; i < this.keys.Count; i++)
			{
				list.Add(this.keys[i]);
			}
			return list;
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x001E5EC0 File Offset: 0x001E40C0
		public override void SetKeys(List<SplineSampleModifier.Key> input)
		{
			this.keys = new List<OffsetModifier.OffsetKey>();
			for (int i = 0; i < input.Count; i++)
			{
				this.keys.Add((OffsetModifier.OffsetKey)input[i]);
			}
			base.SetKeys(input);
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x001E5F07 File Offset: 0x001E4107
		public void AddKey(Vector2 offset, double f, double t)
		{
			this.keys.Add(new OffsetModifier.OffsetKey(offset, f, t, this));
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x001E5F20 File Offset: 0x001E4120
		public override void Apply(SplineSample result)
		{
			if (this.keys.Count == 0)
			{
				return;
			}
			base.Apply(result);
			Vector2 vector = this.Evaluate(result.percent);
			result.position += result.right * vector.x + result.up * vector.y;
		}

		// Token: 0x06003F99 RID: 16281 RVA: 0x001E5F88 File Offset: 0x001E4188
		private Vector2 Evaluate(double time)
		{
			if (this.keys.Count == 0)
			{
				return Vector2.zero;
			}
			Vector2 a = Vector2.zero;
			for (int i = 0; i < this.keys.Count; i++)
			{
				a += this.keys[i].offset * this.keys[i].Evaluate(time);
			}
			return a * this.blend;
		}

		// Token: 0x04002CE6 RID: 11494
		public List<OffsetModifier.OffsetKey> keys = new List<OffsetModifier.OffsetKey>();

		// Token: 0x02000995 RID: 2453
		[Serializable]
		public class OffsetKey : SplineSampleModifier.Key
		{
			// Token: 0x0600543E RID: 21566 RVA: 0x002460CF File Offset: 0x002442CF
			public OffsetKey(Vector2 o, double f, double t, OffsetModifier modifier) : base(f, t, modifier)
			{
				this.offset = o;
			}

			// Token: 0x0400449C RID: 17564
			public Vector2 offset = Vector2.zero;
		}
	}
}
