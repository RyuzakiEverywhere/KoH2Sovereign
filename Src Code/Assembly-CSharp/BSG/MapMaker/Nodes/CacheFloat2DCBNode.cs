using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200037F RID: 895
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Cache/CacheFloat2DCB")]
	[Serializable]
	public class CacheFloat2DCBNode : Node
	{
		// Token: 0x0600346B RID: 13419 RVA: 0x001A3920 File Offset: 0x001A1B20
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetCachedValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CacheFloat2DCBNode.Data ptr = ref *(CacheFloat2DCBNode.Data*)data;
			if (ptr.heights == null)
			{
				return ptr.in1.GetValue(wx, wy);
			}
			int num = (int)(wy * ptr.inv_cell_size.y);
			int num2 = (int)(wx * ptr.inv_cell_size.x);
			int num3 = ptr.resolution.x * ptr.resolution.y;
			int num4 = num * ptr.resolution.y + num2;
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 >= num3)
			{
				num4 = num3 - 1;
			}
			return ptr.heights[num4];
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x001A39B0 File Offset: 0x001A1BB0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<CacheFloat2DCBNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (CacheFloat2DCBNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<CacheFloat2DCBNode.Data>();
			if (CacheFloat2DCBNode.get_value_func == IntPtr.Zero)
			{
				CacheFloat2DCBNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CacheFloat2DCBNode.GetCachedValue)).Value;
			}
		}

		// Token: 0x0600346D RID: 13421 RVA: 0x001A3A0B File Offset: 0x001A1C0B
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
			this.cachedInput = null;
		}

		// Token: 0x0600346E RID: 13422 RVA: 0x001A3A30 File Offset: 0x001A1C30
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetFloat2DCB
			{
				get_value_func = CacheFloat2DCBNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x001A3A7C File Offset: 0x001A1C7C
		private unsafe void FillData()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			ref CacheFloat2DCBNode.Data ptr = ref *this.pdata;
			ptr.in1 = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
			int heightmapResolution = terrainData.heightmapResolution;
			if (this.cachedInput == null)
			{
				this.CacheFloat2D(ref heightmapResolution);
			}
			if (this.cachedInput != null)
			{
				ptr.heights = (float*)AllocationManager.PinGCArrayAndGetDataAddress(this.cachedInput, ref ptr.h_heights);
			}
			ptr.resolution = heightmapResolution;
			float2 rhs = new float2(terrainData.size.x, terrainData.size.z) / (ptr.resolution - 1);
			ptr.inv_cell_size = 1f / rhs;
			this.isDataFilled = true;
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x001A3B5C File Offset: 0x001A1D5C
		private void CacheFloat2D(ref int resolution)
		{
			NodePort outputPort = base.GetOutputPort("res");
			if (NodeHelper.GetConnectionCount(outputPort) > 1)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData == null)
				{
					return;
				}
				GetFloat2DCB inputValue = base.GetInputValue<GetFloat2DCB>("in1", default(GetFloat2DCB));
				if (inputValue.data == null)
				{
					return;
				}
				int2 @int = NodeHelper.GetOutputResolution(outputPort).x;
				if (@int.x > 0)
				{
					resolution = @int.x;
				}
				float2 tileSize = new float2(terrainData.size.x, terrainData.size.z) / (float)(resolution - 1);
				this.cachedInput = NodeHelper.SampleInput(inputValue, resolution, tileSize);
			}
		}

		// Token: 0x04002353 RID: 9043
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB in1;

		// Token: 0x04002354 RID: 9044
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x04002355 RID: 9045
		private float[,] cachedInput;

		// Token: 0x04002356 RID: 9046
		private bool isDataFilled;

		// Token: 0x04002357 RID: 9047
		public static IntPtr get_value_func;

		// Token: 0x04002358 RID: 9048
		public NativeArray<CacheFloat2DCBNode.Data> arr_data;

		// Token: 0x04002359 RID: 9049
		public unsafe CacheFloat2DCBNode.Data* pdata;

		// Token: 0x020008BD RID: 2237
		public struct Data
		{
			// Token: 0x060051F8 RID: 20984 RVA: 0x0023EDB1 File Offset: 0x0023CFB1
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_heights);
			}

			// Token: 0x040040B1 RID: 16561
			public GetFloat2DCB in1;

			// Token: 0x040040B2 RID: 16562
			public int2 resolution;

			// Token: 0x040040B3 RID: 16563
			public float2 inv_cell_size;

			// Token: 0x040040B4 RID: 16564
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* heights;

			// Token: 0x040040B5 RID: 16565
			public ulong h_heights;
		}
	}
}
