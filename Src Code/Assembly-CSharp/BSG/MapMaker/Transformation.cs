using System;
using Unity.Mathematics;
using UnityEngine;

namespace BSG.MapMaker
{
	// Token: 0x02000371 RID: 881
	[Serializable]
	public class Transformation
	{
		// Token: 0x0600342D RID: 13357 RVA: 0x001A22DB File Offset: 0x001A04DB
		public float CalcScale(float val, float grid_size, float tgt_size)
		{
			if (val != 0f)
			{
				return val;
			}
			return tgt_size / grid_size;
		}

		// Token: 0x0600342E RID: 13358 RVA: 0x001A22EC File Offset: 0x001A04EC
		public float2 CalcScale(float2 scale, ref GridData grid, Terrain tgt_terrain)
		{
			if (tgt_terrain == null)
			{
				return scale;
			}
			float2 @float = grid.resolution * grid.cell_size;
			Vector3 size = tgt_terrain.terrainData.size;
			scale.x = this.CalcScale(scale.x, @float.x, size.x);
			scale.y = this.CalcScale(scale.y, @float.y, size.z);
			return scale;
		}

		// Token: 0x0600342F RID: 13359 RVA: 0x001A2368 File Offset: 0x001A0568
		public void ApplyTo(ref GridData grid, Terrain tgt_terrain)
		{
			float2 @float = this.CalcScale(this.Scale, ref grid, tgt_terrain);
			grid.cell_size *= @float;
			grid.inv_cell_size = 1f / grid.cell_size;
			grid.world_pos = -this.Offset;
			grid.rot = this.Rotation;
			grid.wrap = this.Wrap;
			grid.flip_x = this.FlipX;
			grid.flip_y = this.FlipY;
			grid.flip_xy = this.FlipXY;
			grid.scale = @float;
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x001A2408 File Offset: 0x001A0608
		public override string ToString()
		{
			return string.Format("Ofs: ({0}, {1}), Scale: ({2}, {3})", new object[]
			{
				this.Offset.x,
				this.Offset.y,
				this.Scale.x,
				this.Scale.y
			});
		}

		// Token: 0x0400231C RID: 8988
		public float2 Offset;

		// Token: 0x0400231D RID: 8989
		public float2 Scale = 1f;

		// Token: 0x0400231E RID: 8990
		[Range(0f, 3f)]
		public int Rotation;

		// Token: 0x0400231F RID: 8991
		public bool Wrap = true;

		// Token: 0x04002320 RID: 8992
		public bool FlipX;

		// Token: 0x04002321 RID: 8993
		public bool FlipY;

		// Token: 0x04002322 RID: 8994
		public bool FlipXY;
	}
}
