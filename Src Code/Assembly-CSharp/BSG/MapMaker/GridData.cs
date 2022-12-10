using System;
using Unity.Mathematics;

namespace BSG.MapMaker
{
	// Token: 0x02000370 RID: 880
	public struct GridData
	{
		// Token: 0x06003428 RID: 13352 RVA: 0x001A2074 File Offset: 0x001A0274
		public float2 WorldToLocal(float2 pos)
		{
			pos -= this.world_pos;
			pos *= this.inv_cell_size;
			if (this.wrap)
			{
				pos %= this.resolution;
				if (pos.x < 0f)
				{
					pos.x += (float)this.resolution.x;
				}
				if (pos.y < 0f)
				{
					pos.y += (float)this.resolution.y;
				}
			}
			else
			{
				if (pos.x < 0f)
				{
					pos.x = 0f;
				}
				else if (pos.x >= (float)this.resolution.x)
				{
					pos.x = (float)(this.resolution.x - 1);
				}
				if (pos.y < 0f)
				{
					pos.y = 0f;
				}
				else if (pos.y >= (float)this.resolution.y)
				{
					pos.y = (float)(this.resolution.y - 1);
				}
			}
			return pos;
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x001A2194 File Offset: 0x001A0394
		public int2 CalcNextCell(int2 cell)
		{
			int2 @int = cell + 1;
			if (@int.x >= this.resolution.x)
			{
				@int.x = (this.wrap ? 0 : cell.x);
			}
			if (@int.y >= this.resolution.y)
			{
				@int.y = (this.wrap ? 0 : cell.y);
			}
			return @int;
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x001A2200 File Offset: 0x001A0400
		public unsafe float GetValue(float* arr, int layers, float wx, float wy, int layer, FilterType filterType)
		{
			float2 @float = this.WorldToLocal(new float2(wx, wy));
			int2 @int = new int2(@float);
			float2 frac = @float - @int;
			return this.GetLocal(arr, layers, @int, frac, layer, filterType);
		}

		// Token: 0x0600342B RID: 13355 RVA: 0x001A2240 File Offset: 0x001A0440
		public unsafe float GetLocal(float* arr, int layers, int2 cell, float2 frac, int layer, FilterType filterType)
		{
			float num = arr[cell.y * this.resolution.y * layers + cell.x * layers + layer];
			if (filterType == FilterType.None || !math.any(frac))
			{
				return num;
			}
			float result = num;
			if (filterType != FilterType.Bilinear)
			{
				if (filterType == FilterType.Bicubic)
				{
					result = Filters.BicubicFilter(arr, layers, cell, frac, layer, this.wrap, this.resolution);
				}
			}
			else
			{
				result = Filters.BilinearFilter(arr, layers, cell, frac, layer, this.wrap, this.resolution);
			}
			return result;
		}

		// Token: 0x0600342C RID: 13356 RVA: 0x001A22C6 File Offset: 0x001A04C6
		public float GetLocalSafe(float[,,] arr, int2 cell, int layer)
		{
			return arr[cell.y, cell.x, layer];
		}

		// Token: 0x04002312 RID: 8978
		public int2 resolution;

		// Token: 0x04002313 RID: 8979
		public float2 cell_size;

		// Token: 0x04002314 RID: 8980
		public float2 inv_cell_size;

		// Token: 0x04002315 RID: 8981
		public float2 world_pos;

		// Token: 0x04002316 RID: 8982
		public int rot;

		// Token: 0x04002317 RID: 8983
		public bool wrap;

		// Token: 0x04002318 RID: 8984
		public bool flip_x;

		// Token: 0x04002319 RID: 8985
		public bool flip_y;

		// Token: 0x0400231A RID: 8986
		public bool flip_xy;

		// Token: 0x0400231B RID: 8987
		public float2 scale;
	}
}
