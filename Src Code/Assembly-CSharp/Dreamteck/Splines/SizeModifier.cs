using System;
using System.Collections.Generic;

namespace Dreamteck.Splines
{
	// Token: 0x020004BB RID: 1211
	[Serializable]
	public class SizeModifier : SplineSampleModifier
	{
		// Token: 0x06003F9F RID: 16287 RVA: 0x001E6235 File Offset: 0x001E4435
		public SizeModifier()
		{
			this.keys = new List<SizeModifier.SizeKey>();
		}

		// Token: 0x06003FA0 RID: 16288 RVA: 0x001E6254 File Offset: 0x001E4454
		public override List<SplineSampleModifier.Key> GetKeys()
		{
			List<SplineSampleModifier.Key> list = new List<SplineSampleModifier.Key>();
			for (int i = 0; i < this.keys.Count; i++)
			{
				list.Add(this.keys[i]);
			}
			return list;
		}

		// Token: 0x06003FA1 RID: 16289 RVA: 0x001E6290 File Offset: 0x001E4490
		public override void SetKeys(List<SplineSampleModifier.Key> input)
		{
			this.keys = new List<SizeModifier.SizeKey>();
			for (int i = 0; i < input.Count; i++)
			{
				input[i].modifier = this;
				this.keys.Add((SizeModifier.SizeKey)input[i]);
			}
		}

		// Token: 0x06003FA2 RID: 16290 RVA: 0x001E62DD File Offset: 0x001E44DD
		public void AddKey(double f, double t)
		{
			this.keys.Add(new SizeModifier.SizeKey(f, t, this));
		}

		// Token: 0x06003FA3 RID: 16291 RVA: 0x001E62F4 File Offset: 0x001E44F4
		public override void Apply(SplineSample result)
		{
			if (this.keys.Count == 0)
			{
				return;
			}
			base.Apply(result);
			for (int i = 0; i < this.keys.Count; i++)
			{
				result.size += this.keys[i].Evaluate(result.percent) * this.keys[i].size;
			}
		}

		// Token: 0x04002CE8 RID: 11496
		public List<SizeModifier.SizeKey> keys = new List<SizeModifier.SizeKey>();

		// Token: 0x02000997 RID: 2455
		[Serializable]
		public class SizeKey : SplineSampleModifier.Key
		{
			// Token: 0x06005440 RID: 21568 RVA: 0x0024610B File Offset: 0x0024430B
			public SizeKey(double f, double t, SizeModifier modifier) : base(f, t, modifier)
			{
			}

			// Token: 0x040044A0 RID: 17568
			public float size;
		}
	}
}
