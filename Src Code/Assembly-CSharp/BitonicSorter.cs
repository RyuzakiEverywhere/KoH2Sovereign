using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class BitonicSorter
{
	// Token: 0x06001096 RID: 4246 RVA: 0x000B0DC0 File Offset: 0x000AEFC0
	private static float CompareVal(LinesBatching.LineShaderData a)
	{
		return a.pos.x + a.pos.y + a.right.x + a.right.y + a.uv.x + a.uv.y + a.pos_prev.x + a.pos_prev.y + a.right_prev.x + a.right_prev.y;
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x000B0E44 File Offset: 0x000AF044
	private static bool Compare(LinesBatching.LineShaderData a, LinesBatching.LineShaderData b)
	{
		return BitonicSorter.CompareVal(a) > BitonicSorter.CompareVal(b);
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x000B0E54 File Offset: 0x000AF054
	private void SortGPU(int p, int q)
	{
		this.shader.SetInt("p", p);
		this.shader.SetInt("q", q);
		this.shader.Dispatch(this.kernelSort, Mathf.Max(this.N / 512, 1), 1, 1);
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x000B0EA8 File Offset: 0x000AF0A8
	private void SortCPU(int p, int q, LinesBatching.LineShaderData[] Data)
	{
		int num = 1 << p - q;
		for (int i = 0; i < Data.Length; i++)
		{
			bool flag = (i >> p & 2) != 0;
			if ((i & num) == 0 && (i | num) < Data.Length)
			{
				LinesBatching.LineShaderData lineShaderData = Data[i];
				LinesBatching.LineShaderData lineShaderData2 = Data[i | num];
				if (BitonicSorter.Compare(lineShaderData, lineShaderData2) == flag)
				{
					Data[i] = lineShaderData2;
					Data[i | num] = lineShaderData;
				}
			}
		}
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x000B0F18 File Offset: 0x000AF118
	public void bitonicSortCPU(ComputeBuffer buffer, int N)
	{
		this.N = BitonicSorter.upper_power_of_two(N);
		LinesBatching.LineShaderData[] data = new LinesBatching.LineShaderData[this.N];
		buffer.GetData(data, 0, 0, N);
		int num = (int)Mathf.Log((float)this.N, 2f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				this.SortCPU(i, j, data);
			}
		}
		buffer.SetData(data, 0, 0, N);
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x000B0F88 File Offset: 0x000AF188
	public void bitonicSortGPU(ComputeBuffer buffer, int N)
	{
		this.N = BitonicSorter.upper_power_of_two(N);
		this.shader.SetInt("arraySize", this.N);
		this.shader.SetBuffer(this.kernelSort, "Data", buffer);
		int num = (int)Mathf.Log((float)this.N, 2f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				this.SortGPU(i, j);
			}
		}
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x000B1001 File Offset: 0x000AF201
	private static int upper_power_of_two(int v)
	{
		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		v++;
		return v;
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x000B1032 File Offset: 0x000AF232
	public void Init(ComputeShader shader)
	{
		this.shader = shader;
		this.kernelSort = shader.FindKernel("Sort");
	}

	// Token: 0x04000B02 RID: 2818
	private ComputeShader shader;

	// Token: 0x04000B03 RID: 2819
	private int kernelSort;

	// Token: 0x04000B04 RID: 2820
	private const int BITONIC_BLOCK_SIZE = 512;

	// Token: 0x04000B05 RID: 2821
	private int N;
}
