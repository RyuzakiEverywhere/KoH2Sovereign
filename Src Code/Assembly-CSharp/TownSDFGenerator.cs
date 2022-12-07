using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class TownSDFGenerator
{
	// Token: 0x0600055A RID: 1370 RVA: 0x0003BDB0 File Offset: 0x00039FB0
	public TownSDFGenerator(int resolution, float max_circle, NativeArray<float2> wall_points, float2 terrain_size)
	{
		this.resolution = resolution;
		this.max_circle = max_circle;
		this.tile_size_x = (float)resolution / terrain_size.x;
		this.tile_size_y = (float)resolution / terrain_size.y;
		if (TownSDFGenerator.cs == null)
		{
			TownSDFGenerator.cs = (Resources.Load("TownSDFGenerator") as ComputeShader);
			if (TownSDFGenerator.cs != null)
			{
				this.cs_kernel = TownSDFGenerator.cs.FindKernel("GenerateSDF");
			}
		}
		this.render_tex = new RenderTexture(resolution, resolution, 0);
		this.render_tex.enableRandomWrite = true;
		this.render_tex.Create();
		this.wall_points_arr = wall_points;
		this.wall_points = new ComputeBuffer(wall_points.Length, sizeof(float2));
		this.wall_points.SetData<float2>(wall_points);
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0003BE90 File Offset: 0x0003A090
	public Texture2D GenerateGPU()
	{
		if (TownSDFGenerator.cs == null)
		{
			return null;
		}
		uint num;
		uint num2;
		uint num3;
		TownSDFGenerator.cs.GetKernelThreadGroupSizes(this.cs_kernel, out num, out num2, out num3);
		TownSDFGenerator.cs.SetInt("resolution", this.resolution);
		TownSDFGenerator.cs.SetFloat("tile_size_x", this.tile_size_x);
		TownSDFGenerator.cs.SetFloat("tile_size_y", this.tile_size_y);
		TownSDFGenerator.cs.SetFloat("max_circle", this.max_circle);
		TownSDFGenerator.cs.SetTexture(this.cs_kernel, "Result", this.render_tex);
		TownSDFGenerator.cs.SetBuffer(this.cs_kernel, "wall_points", this.wall_points);
		TownSDFGenerator.cs.Dispatch(this.cs_kernel, Mathf.CeilToInt((float)this.resolution / num), Mathf.CeilToInt((float)this.resolution / num2), 1);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.render_tex;
		Texture2D texture2D = new Texture2D(this.resolution, this.resolution);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.render_tex.width, (float)this.render_tex.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		this.Dispose();
		return texture2D;
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0003BFE0 File Offset: 0x0003A1E0
	public Texture2D GenerateCPU()
	{
		this.clrs = new Color[this.resolution * this.resolution];
		for (int i = 0; i < this.resolution; i++)
		{
			for (int j = 0; j < this.resolution; j++)
			{
				this.Kernel(i, j);
			}
		}
		Texture2D texture2D = new Texture2D(this.resolution, this.resolution);
		texture2D.SetPixels(this.clrs);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0003C052 File Offset: 0x0003A252
	public void Dispose()
	{
		if (this.render_tex != null)
		{
			this.render_tex.Release();
		}
		if (this.wall_points != null)
		{
			this.wall_points.Dispose();
		}
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0003C080 File Offset: 0x0003A280
	private float dist_to_segment(float2 a, float2 b, float2 p, out float2 vec)
	{
		float2 @float = a - b;
		float num = @float.x * @float.x + @float.y * @float.y;
		if ((double)num == 0.0)
		{
			vec = 0;
			return math.distance(p, a);
		}
		float lhs = math.max(0f, math.min(1f, math.dot(p - a, b - a) / num));
		float2 float2 = a + lhs * (b - a);
		vec = float2 - p;
		return math.distance(p, float2);
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0003C128 File Offset: 0x0003A328
	public void Kernel(int xc, int yc)
	{
		int num = xc + yc * this.resolution;
		float2 @float = new float2(-1f, -1f);
		float num2 = this.max_circle;
		float2 float2 = new float2((float)xc, (float)yc);
		Color32 color = Color.white;
		for (int i = 0; i < this.wall_points_arr.Length; i++)
		{
			float2 float3 = this.wall_points_arr[i];
			float2 b = this.wall_points_arr[(i + 1) % this.wall_points_arr.Length];
			float3.x *= this.tile_size_x;
			float3.y *= this.tile_size_y;
			b.x *= this.tile_size_x;
			b.y *= this.tile_size_y;
			float2 x;
			float num3 = this.dist_to_segment(float3, b, float2, out x);
			float2 float4 = math.normalize(x);
			float2 float5 = float2 + float4;
			float2 float6 = (num3 < num2) ? float3 : @float;
			color = ((num3 < num2 && float6.x != -1f) ? new Color32((byte)(255f * num3 / this.max_circle), (byte)(255f * (float4.x / 2f + 0.5f)), (byte)(255f * (float4.y / 2f + 0.5f)), byte.MaxValue) : color);
			@float = float5;
			num2 = ((num3 < num2) ? num3 : num2);
		}
		this.clrs[num] = color;
	}

	// Token: 0x040004F7 RID: 1271
	private int resolution;

	// Token: 0x040004F8 RID: 1272
	private float tile_size_x;

	// Token: 0x040004F9 RID: 1273
	private float tile_size_y;

	// Token: 0x040004FA RID: 1274
	private Color[] clrs;

	// Token: 0x040004FB RID: 1275
	private float max_circle = 10f;

	// Token: 0x040004FC RID: 1276
	private NativeArray<float2> wall_points_arr;

	// Token: 0x040004FD RID: 1277
	private RenderTexture render_tex;

	// Token: 0x040004FE RID: 1278
	private ComputeBuffer wall_points;

	// Token: 0x040004FF RID: 1279
	private static ComputeShader cs;

	// Token: 0x04000500 RID: 1280
	private int cs_kernel;
}
