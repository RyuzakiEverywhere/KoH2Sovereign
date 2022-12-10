using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003AB RID: 939
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Texture/SDF Node")]
	[Serializable]
	public class SDFNode : Node
	{
		// Token: 0x0600355E RID: 13662 RVA: 0x001ABF38 File Offset: 0x001AA138
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetR(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SDFNode.Data ptr = ref *(SDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x0600355F RID: 13663 RVA: 0x001ABF70 File Offset: 0x001AA170
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetG(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SDFNode.Data ptr = ref *(SDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 1, ptr.filter_type);
		}

		// Token: 0x06003560 RID: 13664 RVA: 0x001ABFA8 File Offset: 0x001AA1A8
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetB(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SDFNode.Data ptr = ref *(SDFNode.Data*)data;
			return ptr.grid.GetValue(ptr.rgba, 4, wx, wy, 2, ptr.filter_type);
		}

		// Token: 0x06003561 RID: 13665 RVA: 0x001ABFE0 File Offset: 0x001AA1E0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SDFNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SDFNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SDFNode.Data>();
			if (SDFNode.get_r_func == IntPtr.Zero)
			{
				SDFNode.get_r_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SDFNode.GetR)).Value;
			}
			if (SDFNode.get_g_func == IntPtr.Zero)
			{
				SDFNode.get_g_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SDFNode.GetG)).Value;
			}
			if (SDFNode.get_b_func == IntPtr.Zero)
			{
				SDFNode.get_b_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SDFNode.GetB)).Value;
			}
			this.isGenerated = false;
		}

		// Token: 0x06003562 RID: 13666 RVA: 0x001AC0A0 File Offset: 0x001AA2A0
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
		}

		// Token: 0x06003563 RID: 13667 RVA: 0x001AC0C0 File Offset: 0x001AA2C0
		public unsafe override object GetValue(NodePort port)
		{
			this.PreCache();
			string fieldName = port.fieldName;
			if (fieldName == "dist_combined")
			{
				return new GetFloat2DCB
				{
					get_value_func = SDFNode.get_r_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "dist_above")
			{
				return new GetFloat2DCB
				{
					get_value_func = SDFNode.get_g_func,
					data = (void*)this.pdata
				};
			}
			if (fieldName == "dist_below")
			{
				return new GetFloat2DCB
				{
					get_value_func = SDFNode.get_b_func,
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

		// Token: 0x06003564 RID: 13668 RVA: 0x001AC1B4 File Offset: 0x001AA3B4
		public unsafe override void PreCache()
		{
			if (this.isGenerated)
			{
				return;
			}
			float3 @float = base.mm.tgt_terrain.terrainData.size;
			int num = base.GetInputValue<int>("input_resolution", this.input_resolution);
			if (num == 0)
			{
				num = base.mm.tgt_terrain.terrainData.heightmapResolution;
			}
			this.heights = base.GetInputValue<GetFloat2DCB>("heights", default(GetFloat2DCB));
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			int num2 = num;
			float[,] array = NodeHelper.SampleInput(this.heights, num2, new float2(@float.x / (float)num2, @float.z / (float)num2));
			SDFGenerator sdfgenerator = new SDFGenerator(array, this.sdf_resolution, this.threshold, 1f, this.max_circle);
			this.texture = sdfgenerator.GenerateGPU();
			ref SDFNode.Data ptr = ref *this.pdata;
			ptr.grid.resolution = this.texture.width;
			ptr.grid.cell_size = 1f;
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			float[,,] array2 = new float[ptr.grid.resolution.y, ptr.grid.resolution.x, 4];
			Stopwatch stopwatch = Stopwatch.StartNew();
			Color[] pixels = this.texture.GetPixels();
			fixed (float* ptr2 = &array2[0, 0, 0])
			{
				float* prgba = ptr2;
				fixed (float* ptr3 = &array[0, 0])
				{
					float* pheights = ptr3;
					fixed (Color* ptr4 = ref pixels[0])
					{
						Color* ppixels = ptr4;
						SDFNode.BuildTextureDataJob jobData = default(SDFNode.BuildTextureDataJob);
						jobData.gridResolution = ptr.grid.resolution;
						jobData.textureResolution = this.texture.width;
						jobData.heightResolution = num2;
						jobData.pheights = pheights;
						jobData.threshold = this.threshold;
						jobData.prgba = prgba;
						jobData.ppixels = ppixels;
						if (this.SingleThreaded)
						{
							jobData.Run(ptr.grid.resolution.y);
						}
						else
						{
							jobData.Schedule(ptr.grid.resolution.y, 16, default(JobHandle)).Complete();
						}
					}
				}
			}
			Debug.Log("Creating texture data time " + stopwatch.ElapsedMilliseconds);
			ptr.rgba = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array2, ref ptr.h_rgba);
			ptr.filter_type = this.filterType;
			this.isGenerated = true;
		}

		// Token: 0x04002494 RID: 9364
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB heights;

		// Token: 0x04002495 RID: 9365
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x04002496 RID: 9366
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public int input_resolution;

		// Token: 0x04002497 RID: 9367
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public Texture2D texture;

		// Token: 0x04002498 RID: 9368
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_combined;

		// Token: 0x04002499 RID: 9369
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_above;

		// Token: 0x0400249A RID: 9370
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB dist_below;

		// Token: 0x0400249B RID: 9371
		[Node.OutputAttribute(Node.ShowBackingValue.Always, Node.ConnectionType.Multiple, false)]
		public float max_circle = 30f;

		// Token: 0x0400249C RID: 9372
		public float threshold = 0.25f;

		// Token: 0x0400249D RID: 9373
		public int sdf_resolution = 512;

		// Token: 0x0400249E RID: 9374
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x0400249F RID: 9375
		public bool SingleThreaded = true;

		// Token: 0x040024A0 RID: 9376
		private bool isGenerated;

		// Token: 0x040024A1 RID: 9377
		public static IntPtr get_r_func;

		// Token: 0x040024A2 RID: 9378
		public static IntPtr get_g_func;

		// Token: 0x040024A3 RID: 9379
		public static IntPtr get_b_func;

		// Token: 0x040024A4 RID: 9380
		public NativeArray<SDFNode.Data> arr_data;

		// Token: 0x040024A5 RID: 9381
		public unsafe SDFNode.Data* pdata;

		// Token: 0x020008F0 RID: 2288
		public struct Data
		{
			// Token: 0x06005228 RID: 21032 RVA: 0x002402CC File Offset: 0x0023E4CC
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_rgba);
			}

			// Token: 0x0400419A RID: 16794
			public GridData grid;

			// Token: 0x0400419B RID: 16795
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* rgba;

			// Token: 0x0400419C RID: 16796
			public ulong h_rgba;

			// Token: 0x0400419D RID: 16797
			public FilterType filter_type;
		}

		// Token: 0x020008F1 RID: 2289
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildTextureDataJob : IJobParallelFor
		{
			// Token: 0x06005229 RID: 21033 RVA: 0x002402DC File Offset: 0x0023E4DC
			public unsafe void Execute(int y)
			{
				int num = this.textureResolution * this.textureResolution;
				int num2 = this.heightResolution * this.heightResolution;
				int num3 = this.gridResolution.y * this.gridResolution.x * 4;
				for (int i = 0; i < this.gridResolution.x; i++)
				{
					int num4 = y * this.textureResolution + i;
					if (num4 >= 0 && num4 < num)
					{
						float r = this.ppixels[num4].r;
						int num5 = (int)((float)y / (float)this.gridResolution.y * (float)this.heightResolution);
						int num6 = (int)((float)i / (float)this.gridResolution.x * (float)this.heightResolution);
						int num7 = num5 * this.heightResolution + num6;
						if (num7 >= 0 && num7 < num2)
						{
							float num8 = this.pheights[num7];
							int num9 = y * this.gridResolution.y * 4 + i * 4;
							if (num9 >= 0 && num9 < num3)
							{
								this.prgba[num9] = r;
								if (num8 > this.threshold)
								{
									this.prgba[num9 + 1] = r;
									this.prgba[num9 + 2] = 0f;
								}
								else
								{
									this.prgba[num9 + 1] = 0f;
									this.prgba[num9 + 2] = r;
								}
								this.prgba[num9 + 3] = 1f;
							}
						}
					}
				}
			}

			// Token: 0x0400419E RID: 16798
			public int heightResolution;

			// Token: 0x0400419F RID: 16799
			public int textureResolution;

			// Token: 0x040041A0 RID: 16800
			public int2 gridResolution;

			// Token: 0x040041A1 RID: 16801
			public float threshold;

			// Token: 0x040041A2 RID: 16802
			[NativeDisableUnsafePtrRestriction]
			public unsafe Color* ppixels;

			// Token: 0x040041A3 RID: 16803
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pheights;

			// Token: 0x040041A4 RID: 16804
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* prgba;
		}
	}
}
