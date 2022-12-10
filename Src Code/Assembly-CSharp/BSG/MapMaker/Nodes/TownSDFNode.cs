using System;
using System.Diagnostics;
using Logic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B6 RID: 950
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/Town SDF Node")]
	[Serializable]
	public class TownSDFNode : Node
	{
		// Token: 0x060035A7 RID: 13735 RVA: 0x001AE88C File Offset: 0x001ACA8C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetR(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref TownSDFNode.Data ptr = ref *(TownSDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x060035A8 RID: 13736 RVA: 0x001AE8C4 File Offset: 0x001ACAC4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetG(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref TownSDFNode.Data ptr = ref *(TownSDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 1, ptr.filter_type);
		}

		// Token: 0x060035A9 RID: 13737 RVA: 0x001AE8FC File Offset: 0x001ACAFC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetB(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref TownSDFNode.Data ptr = ref *(TownSDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 2, ptr.filter_type);
		}

		// Token: 0x060035AA RID: 13738 RVA: 0x001AE934 File Offset: 0x001ACB34
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<TownSDFNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (TownSDFNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<TownSDFNode.Data>();
			if (TownSDFNode.get_r_func == IntPtr.Zero)
			{
				TownSDFNode.get_r_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(TownSDFNode.GetR)).Value;
			}
			if (TownSDFNode.get_g_func == IntPtr.Zero)
			{
				TownSDFNode.get_g_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(TownSDFNode.GetG)).Value;
			}
			if (TownSDFNode.get_b_func == IntPtr.Zero)
			{
				TownSDFNode.get_b_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(TownSDFNode.GetB)).Value;
			}
			this.isGenerated = false;
		}

		// Token: 0x060035AB RID: 13739 RVA: 0x001AE9F4 File Offset: 0x001ACBF4
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
		}

		// Token: 0x060035AC RID: 13740 RVA: 0x001AEA14 File Offset: 0x001ACC14
		public unsafe override object GetValue(NodePort port)
		{
			this.PreCache();
			string fieldName = port.fieldName;
			if (fieldName == "dist_combined")
			{
				return new GetFloat2DCB
				{
					get_value_func = TownSDFNode.get_r_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "dist_above")
			{
				return new GetFloat2DCB
				{
					get_value_func = TownSDFNode.get_g_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "dist_below")
			{
				return new GetFloat2DCB
				{
					get_value_func = TownSDFNode.get_b_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "texture")
			{
				return this.texture;
			}
			if (!(fieldName == "max_circle"))
			{
				return null;
			}
			return this.max_circle;
		}

		// Token: 0x060035AD RID: 13741 RVA: 0x001AEB08 File Offset: 0x001ACD08
		public unsafe override void PreCache()
		{
			if (this.isGenerated)
			{
				return;
			}
			float3 @float = base.mm.tgt_terrain.terrainData.size;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			if (this.corner_points.IsCreated && this.corner_points.Length > 0)
			{
				TownSDFGenerator townSDFGenerator = new TownSDFGenerator(this.sdf_resolution, this.max_circle, this.corner_points, @float.xz);
				this.texture = townSDFGenerator.GenerateGPU();
			}
			else
			{
				this.texture = null;
			}
			ref TownSDFNode.Data ptr = ref *this.pdata;
			ptr.grid.resolution = this.sdf_resolution;
			ptr.grid.cell_size = 1f;
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			float[,,] array = new float[ptr.grid.resolution.y, ptr.grid.resolution.x, 4];
			Stopwatch stopwatch = Stopwatch.StartNew();
			Color[] array2;
			float2* corners;
			int corner_count;
			if (this.texture != null)
			{
				array2 = this.texture.GetPixels();
				corners = (float2*)this.corner_points.GetUnsafePtr<float2>();
				corner_count = this.corner_points.Length;
			}
			else
			{
				array2 = new Color[this.sdf_resolution * this.sdf_resolution];
				corners = null;
				corner_count = 0;
			}
			fixed (float* ptr2 = &array[0, 0, 0])
			{
				float* prgba = ptr2;
				fixed (Color* ptr3 = &array2[0])
				{
					Color* ppixels = ptr3;
					TownSDFNode.BuildTextureDataJob buildTextureDataJob = default(TownSDFNode.BuildTextureDataJob);
					buildTextureDataJob.gridResolution = ptr.grid.resolution;
					buildTextureDataJob.textureResolution = this.sdf_resolution;
					buildTextureDataJob.prgba = prgba;
					buildTextureDataJob.ppixels = ppixels;
					buildTextureDataJob.corners = corners;
					buildTextureDataJob.corner_count = corner_count;
					buildTextureDataJob.cell_size = new float2((float)this.sdf_resolution / @float.x, (float)this.sdf_resolution / @float.z);
					buildTextureDataJob.bounds = new float4(this.bounds.x * buildTextureDataJob.cell_size.x, this.bounds.y * buildTextureDataJob.cell_size.x, this.bounds.z * buildTextureDataJob.cell_size.y, this.bounds.w * buildTextureDataJob.cell_size.y);
					if (this.SingleThreaded)
					{
						buildTextureDataJob.Run(ptr.grid.resolution.y);
					}
					else
					{
						buildTextureDataJob.Schedule(ptr.grid.resolution.y, 16, default(JobHandle)).Complete();
					}
				}
			}
			Debug.Log("Creating texture data time " + stopwatch.ElapsedMilliseconds);
			ptr.rgba = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref ptr.h_rgba);
			ptr.filter_type = this.filterType;
			this.isGenerated = true;
		}

		// Token: 0x04002527 RID: 9511
		[HideInInspector]
		public NativeArray<float2> corner_points;

		// Token: 0x04002528 RID: 9512
		[HideInInspector]
		public float rotation;

		// Token: 0x04002529 RID: 9513
		[HideInInspector]
		public float2 wall_center;

		// Token: 0x0400252A RID: 9514
		[HideInInspector]
		public float4 bounds;

		// Token: 0x0400252B RID: 9515
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation = new Transformation
		{
			Scale = new float2(0f, 0f)
		};

		// Token: 0x0400252C RID: 9516
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public Texture2D texture;

		// Token: 0x0400252D RID: 9517
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_combined;

		// Token: 0x0400252E RID: 9518
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_above;

		// Token: 0x0400252F RID: 9519
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_below;

		// Token: 0x04002530 RID: 9520
		[Node.OutputAttribute(Node.ShowBackingValue.Always, Node.ConnectionType.Multiple, false)]
		public float max_circle = 30f;

		// Token: 0x04002531 RID: 9521
		public int sdf_resolution = 512;

		// Token: 0x04002532 RID: 9522
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x04002533 RID: 9523
		public bool SingleThreaded = true;

		// Token: 0x04002534 RID: 9524
		private bool isGenerated;

		// Token: 0x04002535 RID: 9525
		public static IntPtr get_r_func;

		// Token: 0x04002536 RID: 9526
		public static IntPtr get_g_func;

		// Token: 0x04002537 RID: 9527
		public static IntPtr get_b_func;

		// Token: 0x04002538 RID: 9528
		public NativeArray<TownSDFNode.Data> arr_data;

		// Token: 0x04002539 RID: 9529
		public unsafe TownSDFNode.Data* pdata;

		// Token: 0x020008FB RID: 2299
		public struct Data
		{
			// Token: 0x0600522D RID: 21037 RVA: 0x002406DC File Offset: 0x0023E8DC
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_rgba);
			}

			// Token: 0x040041DD RID: 16861
			public GridData grid;

			// Token: 0x040041DE RID: 16862
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* rgba;

			// Token: 0x040041DF RID: 16863
			public ulong h_rgba;

			// Token: 0x040041E0 RID: 16864
			public FilterType filter_type;
		}

		// Token: 0x020008FC RID: 2300
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildTextureDataJob : IJobParallelFor
		{
			// Token: 0x0600522E RID: 21038 RVA: 0x002406EC File Offset: 0x0023E8EC
			public unsafe void Execute(int y)
			{
				int num = this.textureResolution * this.textureResolution;
				int num2 = this.gridResolution.y * this.gridResolution.x * 4;
				Point center = new Point((float)(this.textureResolution / 2) / this.cell_size.x, (float)(this.textureResolution / 2) / this.cell_size.y);
				for (int i = 0; i < this.gridResolution.x; i++)
				{
					int num3 = y * this.textureResolution + i;
					if (num3 >= 0 && num3 < num)
					{
						float num4 = this.ppixels[num3].r;
						if (this.corner_count == 0)
						{
							num4 = 1f;
						}
						int num5 = y * this.gridResolution.y * 4 + i * 4;
						if (num5 >= 0 && num5 < num2)
						{
							this.prgba[num5] = num4;
							this.prgba[num5 + 3] = 1f;
							if ((float)i < this.bounds.x || (float)i > this.bounds.y || (float)y < this.bounds.z || (float)y > this.bounds.w || this.IsOutsidePolygon(new Point((float)i / this.cell_size.x, (float)y / this.cell_size.y), center))
							{
								this.prgba[num5 + 1] = num4;
								this.prgba[num5 + 2] = 0f;
							}
							else
							{
								this.prgba[num5 + 1] = 0f;
								this.prgba[num5 + 2] = num4;
							}
						}
					}
				}
			}

			// Token: 0x0600522F RID: 21039 RVA: 0x002408B0 File Offset: 0x0023EAB0
			public unsafe bool IsOutsidePolygon(Point pt, Point center)
			{
				for (int i = 0; i < this.corner_count; i++)
				{
					Point point = this.corners[i];
					Point point2 = this.corners[(i + 1) % this.corner_count];
					Point point3;
					if (global::Common.Intersect(point, point2, pt, center, out point3) && global::Common.CheckIfPointIsInRectangle(point3, point, point2, 0.0) && global::Common.CheckIfPointIsInRectangle(point3, pt, center, 0.03))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x040041E1 RID: 16865
			public int textureResolution;

			// Token: 0x040041E2 RID: 16866
			public int2 gridResolution;

			// Token: 0x040041E3 RID: 16867
			public float2 cell_size;

			// Token: 0x040041E4 RID: 16868
			public int corner_count;

			// Token: 0x040041E5 RID: 16869
			public float4 bounds;

			// Token: 0x040041E6 RID: 16870
			[NativeDisableUnsafePtrRestriction]
			public unsafe float2* corners;

			// Token: 0x040041E7 RID: 16871
			[NativeDisableUnsafePtrRestriction]
			public unsafe Color* ppixels;

			// Token: 0x040041E8 RID: 16872
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* prgba;
		}
	}
}
