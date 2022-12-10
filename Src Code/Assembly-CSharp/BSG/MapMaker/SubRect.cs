using System;
using Unity.Mathematics;

namespace BSG.MapMaker
{
	// Token: 0x02000372 RID: 882
	[Serializable]
	public class SubRect
	{
		// Token: 0x06003432 RID: 13362 RVA: 0x001A2490 File Offset: 0x001A0690
		public int2 ApplyTo(ref GridData grid)
		{
			int2 resolution = grid.resolution;
			float num = this.center.x;
			if (num > 1f)
			{
				num /= this.divider.x;
			}
			if (num <= 1f)
			{
				num *= (float)resolution.x;
			}
			float num2 = this.center.y;
			if (num2 > 1f)
			{
				num2 /= this.divider.y;
			}
			if (num2 <= 1f)
			{
				num2 *= (float)resolution.y;
			}
			float2 @float = this.size;
			if (@float.x > 1f || @float.y > 1f)
			{
				@float /= this.divider;
			}
			if (@float.x <= 1f || @float.y <= 1f)
			{
				@float *= resolution;
			}
			if (@float.x > (float)resolution.x)
			{
				@float.x = (float)resolution.x;
			}
			if (@float.y > (float)resolution.y)
			{
				@float.y = (float)resolution.y;
			}
			grid.resolution = (int2)@float;
			int2 @int = new int2((int)(num - @float.x * 0.5f), (int)(num2 - @float.y * 0.5f));
			if (@int.x < 0)
			{
				@int.x = 0;
			}
			else if (@int.x + grid.resolution.x > resolution.x)
			{
				@int.x = resolution.x - grid.resolution.x;
			}
			if (@int.y < 0)
			{
				@int.y = 0;
			}
			else if (@int.y + grid.resolution.y > resolution.y)
			{
				@int.y = resolution.y - grid.resolution.y;
			}
			return @int;
		}

		// Token: 0x06003433 RID: 13363 RVA: 0x001A265E File Offset: 0x001A085E
		public override string ToString()
		{
			return string.Format("Center: ({0}, {1}), Size: {2}", this.center.x, this.center.y, this.size);
		}

		// Token: 0x04002323 RID: 8995
		public float2 center = new float2(0.5f, 0.5f);

		// Token: 0x04002324 RID: 8996
		public float2 size = 1f;

		// Token: 0x04002325 RID: 8997
		public float2 divider = 1f;
	}
}
