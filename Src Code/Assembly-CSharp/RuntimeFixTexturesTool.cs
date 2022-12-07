using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class RuntimeFixTexturesTool
{
	// Token: 0x0600088A RID: 2186 RVA: 0x0005CCA8 File Offset: 0x0005AEA8
	public RuntimeFixTexturesTool(Terrain t)
	{
		this.picked_terrain = t;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0005CCCC File Offset: 0x0005AECC
	public void Run()
	{
		this.GetSplats();
		for (int i = 0; i < this.grid_height; i++)
		{
			for (int j = 0; j < this.grid_width; j++)
			{
				this.ApplySplat(j, i);
			}
		}
		this.picked_terrain.terrainData.SetAlphamaps(0, 0, this.splats);
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0005CD24 File Offset: 0x0005AF24
	private void GetSplats()
	{
		if (this.picked_terrain == null)
		{
			this.splats = null;
			this.layers = 0;
			return;
		}
		this.splats = this.picked_terrain.terrainData.GetAlphamaps(0, 0, this.picked_terrain.terrainData.alphamapResolution, this.picked_terrain.terrainData.alphamapResolution);
		this.grid_height = this.splats.GetLength(0);
		this.grid_width = this.splats.GetLength(1);
		this.layers = this.splats.GetLength(2);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0005CDBC File Offset: 0x0005AFBC
	private bool IsRedundant(int x, int y, int idx)
	{
		int num = 0;
		float num2 = this.splats[y, x, idx];
		if (num2 <= 0f)
		{
			return false;
		}
		for (int i = 0; i < this.layers; i++)
		{
			float num3 = this.splats[y, x, i];
			if (num3 >= num2 && (num3 != num2 || i < idx) && ++num >= this.max_textures)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0005CE20 File Offset: 0x0005B020
	public void ApplySplat(int x, int y)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < this.layers; i++)
		{
			float num2 = this.splats[y, x, i];
			if (num2 > 0f)
			{
				if (num2 < this.min_weight)
				{
					this.splats[y, x, i] = 0f;
					flag = true;
				}
				else
				{
					num++;
				}
			}
		}
		if (num > this.max_textures)
		{
			for (int j = 0; j < this.layers; j++)
			{
				if (this.IsRedundant(x, y, j))
				{
					this.splats[y, x, j] = 0f;
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		RuntimeFixTexturesTool.NormalizeSplatAlphas(this.splats, x, y, -1);
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0005CED0 File Offset: 0x0005B0D0
	public static void NormalizeSplatAlphas(float[,,] splats, int x, int y, int fixed_index = -1)
	{
		int length = splats.GetLength(2);
		float num = 0f;
		for (int i = 0; i < length; i++)
		{
			num += splats[y, x, i];
		}
		if (num == 1f)
		{
			return;
		}
		float num2 = 1f;
		if (fixed_index >= 0 && fixed_index < length)
		{
			float num3 = splats[y, x, fixed_index];
			num -= num3;
			num2 = 1f - num3;
		}
		float num4 = (num == 0f) ? 0f : (num2 / num);
		for (int j = 0; j < length; j++)
		{
			if (j != fixed_index)
			{
				splats[y, x, j] *= num4;
			}
		}
		if (num == 0f)
		{
			splats[y, x, 0] = ((fixed_index == 0) ? 1f : num2);
		}
	}

	// Token: 0x040006C1 RID: 1729
	private Terrain picked_terrain;

	// Token: 0x040006C2 RID: 1730
	public int max_textures = 4;

	// Token: 0x040006C3 RID: 1731
	public float min_weight = 0.05f;

	// Token: 0x040006C4 RID: 1732
	private float[,,] splats;

	// Token: 0x040006C5 RID: 1733
	private int layers;

	// Token: 0x040006C6 RID: 1734
	private int grid_width;

	// Token: 0x040006C7 RID: 1735
	private int grid_height;
}
