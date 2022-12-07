using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class SDFGenerator
{
	// Token: 0x06000553 RID: 1363 RVA: 0x0003B6F4 File Offset: 0x000398F4
	public SDFGenerator(float[,] heights, int resolution, float water_level, float terrain_height, float max_circle)
	{
		this.heights_dim_x = heights.GetLength(1);
		this.heights_dim_y = heights.GetLength(0);
		this.tile_size_x = (float)resolution / (float)this.heights_dim_x;
		this.tile_size_y = (float)resolution / (float)this.heights_dim_y;
		this.water_level = water_level;
		this.resolution = resolution;
		this.heights = heights;
		this.terrain_height = terrain_height;
		this.max_circle = max_circle;
		if (SDFGenerator.cs == null)
		{
			SDFGenerator.cs = (Resources.Load("SDFGenerator") as ComputeShader);
			this.cs_kernel = SDFGenerator.cs.FindKernel("GenerateSDF");
		}
		this.render_tex = new RenderTexture(resolution, resolution, 0);
		this.render_tex.enableRandomWrite = true;
		this.render_tex.Create();
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0003B7D0 File Offset: 0x000399D0
	public Texture2D GenerateGPU()
	{
		if (SDFGenerator.cs == null)
		{
			return null;
		}
		uint num;
		uint num2;
		uint num3;
		SDFGenerator.cs.GetKernelThreadGroupSizes(this.cs_kernel, out num, out num2, out num3);
		this.heights_buffer = new ComputeBuffer(this.heights.Length, 4);
		this.heights_buffer.SetData(this.heights, 0, 0, this.heights.Length);
		SDFGenerator.cs.SetBuffer(this.cs_kernel, "heights", this.heights_buffer);
		SDFGenerator.cs.SetFloat("water_level", this.water_level);
		SDFGenerator.cs.SetInt("resolution", this.resolution);
		SDFGenerator.cs.SetFloat("terrain_height", this.terrain_height);
		SDFGenerator.cs.SetFloat("tile_size_x", this.tile_size_x);
		SDFGenerator.cs.SetFloat("tile_size_y", this.tile_size_y);
		SDFGenerator.cs.SetFloat("heights_resolution", (float)this.heights_dim_x);
		SDFGenerator.cs.SetFloat("max_circle", this.max_circle);
		SDFGenerator.cs.SetTexture(this.cs_kernel, "Result", this.render_tex);
		SDFGenerator.cs.Dispatch(this.cs_kernel, Mathf.CeilToInt((float)this.resolution / num), Mathf.CeilToInt((float)this.resolution / num2), 1);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = this.render_tex;
		Texture2D texture2D = new Texture2D(this.resolution, this.resolution);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)this.render_tex.width, (float)this.render_tex.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		this.Dispose();
		return texture2D;
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0003B998 File Offset: 0x00039B98
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

	// Token: 0x06000556 RID: 1366 RVA: 0x0003BA0A File Offset: 0x00039C0A
	public void Dispose()
	{
		if (this.heights_buffer != null)
		{
			this.heights_buffer.Dispose();
		}
		if (this.render_tex != null)
		{
			this.render_tex.Release();
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0003BA38 File Offset: 0x00039C38
	private float GetHeight(int x, int y)
	{
		return this.heights[math.clamp(y, 0, this.heights_dim_x - 1), math.clamp(x, 0, this.heights_dim_x - 1)];
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0003BA64 File Offset: 0x00039C64
	private float GetHeighInterpolated(int x, int y)
	{
		float2 @float = new float2((float)x / this.tile_size_x, (float)y / this.tile_size_y);
		float2 float2 = math.frac(@float);
		int2 @int = (int2)(@float - float2);
		float2 float3 = math.sign(new float2(float2.x - 0.5f, float2.y - 0.5f));
		float2 float4 = math.abs(float2);
		float height = this.GetHeight(@int.x, @int.y);
		float height2 = this.GetHeight((int)((float)@int.x + float3.x), @int.y);
		float height3 = this.GetHeight(@int.x, (int)((float)@int.y + float3.y));
		float height4 = this.GetHeight((int)((float)@int.x + float3.x), (int)((float)@int.y + float3.y));
		float num = (1f - float4.x) * (1f - float4.y);
		float num2 = float4.x * (1f - float4.y);
		float num3 = (1f - float4.x) * float4.y;
		float num4 = float4.x * float4.y;
		return (height * num + height2 * num2 + height3 * num3 + height4 * num4) * this.terrain_height;
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0003BBA8 File Offset: 0x00039DA8
	public void Kernel(int xc, int yc)
	{
		int num = xc + yc * this.resolution;
		bool flag = this.GetHeighInterpolated(xc, yc) < this.water_level;
		float2 @float = new float2(-1f, -1f);
		float num2 = 10000f;
		float2 rhs = new float2((float)xc, (float)yc);
		Color32 color = Color.white;
		int num3 = (int)math.clamp((float)xc - this.max_circle, 0f, (float)this.resolution);
		int num4 = (int)math.clamp((float)yc - this.max_circle, 0f, (float)this.resolution);
		int num5 = (int)math.clamp((float)xc + this.max_circle, 0f, (float)this.resolution);
		int num6 = (int)math.clamp((float)yc + this.max_circle, 0f, (float)this.resolution);
		for (int i = num3; i <= num5; i++)
		{
			for (int j = num4; j <= num6; j++)
			{
				float2 float2 = new float2((float)i, (float)j);
				bool flag2 = this.GetHeighInterpolated(i, j) < this.water_level;
				bool flag3 = flag != flag2;
				float2 = (flag3 ? float2 : @float);
				float2 float3 = flag3 ? (float2 - rhs) : new float2(-100000f, -100000f);
				float num7 = math.length(float3);
				float3 /= num7;
				float num8 = math.clamp(num7, 0f, this.max_circle);
				float2 = ((num8 < num2) ? float2 : @float);
				color = ((num8 < num2 && float2.x != -1f) ? new Color32((byte)(255f * (num8 / this.max_circle)), (byte)(255f * (float3.x / 2f + 0.5f)), (byte)(255f * (float3.y / 2f + 0.5f)), byte.MaxValue) : color);
				num8 = math.min(num8, num2);
				@float = float2;
				num2 = num8;
			}
		}
		this.clrs[num] = color;
	}

	// Token: 0x040004E9 RID: 1257
	private int resolution;

	// Token: 0x040004EA RID: 1258
	private float tile_size_x;

	// Token: 0x040004EB RID: 1259
	private float tile_size_y;

	// Token: 0x040004EC RID: 1260
	private Color[] clrs;

	// Token: 0x040004ED RID: 1261
	private float[,] heights;

	// Token: 0x040004EE RID: 1262
	private float water_level;

	// Token: 0x040004EF RID: 1263
	private float max_circle = 10f;

	// Token: 0x040004F0 RID: 1264
	private float terrain_height;

	// Token: 0x040004F1 RID: 1265
	private int heights_dim_x;

	// Token: 0x040004F2 RID: 1266
	private int heights_dim_y;

	// Token: 0x040004F3 RID: 1267
	private RenderTexture render_tex;

	// Token: 0x040004F4 RID: 1268
	private ComputeBuffer heights_buffer;

	// Token: 0x040004F5 RID: 1269
	private static ComputeShader cs;

	// Token: 0x040004F6 RID: 1270
	private int cs_kernel;
}
