using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
[CreateAssetMenu(menuName = "Benchmarking/GPU Benchmark Assets", fileName = "GPUBenchmarkAssets")]
public class GPUBenchmarkAssets : ScriptableObject
{
	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060001D8 RID: 472 RVA: 0x0001E2FA File Offset: 0x0001C4FA
	public Mesh Mesh
	{
		get
		{
			return this.mesh;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060001D9 RID: 473 RVA: 0x0001E302 File Offset: 0x0001C502
	public Material RenderMaterial
	{
		get
		{
			return this.render_material;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060001DA RID: 474 RVA: 0x0001E30A File Offset: 0x0001C50A
	public Material SecondRenderMaterial
	{
		get
		{
			return this.render_material2;
		}
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060001DB RID: 475 RVA: 0x0001E312 File Offset: 0x0001C512
	public Material TextureGenerationmaterial
	{
		get
		{
			return this.texture_generation_material;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001DC RID: 476 RVA: 0x0001E31A File Offset: 0x0001C51A
	public AnimationCurve GPURatingCurve
	{
		get
		{
			return this.rating_curve;
		}
	}

	// Token: 0x040002D7 RID: 727
	[SerializeField]
	private Mesh mesh;

	// Token: 0x040002D8 RID: 728
	[SerializeField]
	private Material render_material;

	// Token: 0x040002D9 RID: 729
	[SerializeField]
	private Material render_material2;

	// Token: 0x040002DA RID: 730
	[SerializeField]
	private Material texture_generation_material;

	// Token: 0x040002DB RID: 731
	[SerializeField]
	private AnimationCurve rating_curve;
}
