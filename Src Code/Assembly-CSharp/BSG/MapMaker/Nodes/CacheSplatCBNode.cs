using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000380 RID: 896
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Cache/CacheSplatsCB")]
	[Serializable]
	public class CacheSplatCBNode : Node
	{
		// Token: 0x06003472 RID: 13426 RVA: 0x001A3C28 File Offset: 0x001A1E28
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref CacheSplatCBNode.Data ptr = ref *(CacheSplatCBNode.Data*)data;
			if (ptr.alphas == null)
			{
				float* ptr2 = stackalloc float[checked(unchecked((UIntPtr)layers) * 4)];
				ptr.in1.GetTile(wx, wy, ptr2, layers);
				for (int i = 0; i < layers; i++)
				{
					float num = ptr2[i];
					result[i] = num;
				}
				return;
			}
			int num2 = (int)(wy * ptr.inv_cell_size.y);
			int num3 = (int)(wx * ptr.inv_cell_size.x);
			int num4 = ptr.resolution.x * ptr.resolution.y * layers;
			int num5 = num2 * ptr.resolution.y * layers + num3 * layers;
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num5 >= num4)
			{
				num5 = num4 - 1;
			}
			float* ptr3 = ptr.alphas + num5;
			for (int j = 0; j < layers; j++)
			{
				float num6 = ptr3[j];
				result[j] = num6;
			}
		}

		// Token: 0x06003473 RID: 13427 RVA: 0x001A3D14 File Offset: 0x001A1F14
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<CacheSplatCBNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (CacheSplatCBNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<CacheSplatCBNode.Data>();
			if (CacheSplatCBNode.get_tile_func == IntPtr.Zero)
			{
				CacheSplatCBNode.get_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(CacheSplatCBNode.GetTile)).Value;
			}
		}

		// Token: 0x06003474 RID: 13428 RVA: 0x001A3D6F File Offset: 0x001A1F6F
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
			this.cachedInput = null;
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x001A3D94 File Offset: 0x001A1F94
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetSplatCB
			{
				get_tile_func = CacheSplatCBNode.get_tile_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x001A3DE0 File Offset: 0x001A1FE0
		private unsafe void FillData()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			ref CacheSplatCBNode.Data ptr = ref *this.pdata;
			ptr.in1 = base.GetInputValue<GetSplatCB>("in1", default(GetSplatCB));
			ptr.resolution = terrainData.alphamapResolution;
			float2 rhs = new float2(terrainData.size.x, terrainData.size.z) / ptr.resolution;
			ptr.inv_cell_size = 1f / rhs;
			if (this.cachedInput == null)
			{
				this.CacheSplats();
			}
			if (this.cachedInput != null)
			{
				ptr.alphas = (float*)AllocationManager.PinGCArrayAndGetDataAddress(this.cachedInput, ref ptr.h_alphas);
			}
			ptr.layers = terrainData.alphamapLayers;
			this.isDataFilled = true;
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x001A3EC0 File Offset: 0x001A20C0
		private void CacheSplats()
		{
			if (NodeHelper.GetConnectionCount(base.GetOutputPort("res")) > 1)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData == null)
				{
					return;
				}
				GetSplatCB inputValue = base.GetInputValue<GetSplatCB>("in1", default(GetSplatCB));
				if (inputValue.data == null)
				{
					return;
				}
				int alphamapResolution = terrainData.alphamapResolution;
				int alphamapLayers = terrainData.alphamapLayers;
				float2 tileSize = new float2(terrainData.size.x, terrainData.size.z) / (float)alphamapResolution;
				this.cachedInput = NodeHelper.SampleSplatInput(inputValue, alphamapResolution, tileSize, alphamapLayers);
			}
		}

		// Token: 0x0400235A RID: 9050
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB in1;

		// Token: 0x0400235B RID: 9051
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB res;

		// Token: 0x0400235C RID: 9052
		private float[,,] cachedInput;

		// Token: 0x0400235D RID: 9053
		private bool isDataFilled;

		// Token: 0x0400235E RID: 9054
		public static IntPtr get_tile_func;

		// Token: 0x0400235F RID: 9055
		public NativeArray<CacheSplatCBNode.Data> arr_data;

		// Token: 0x04002360 RID: 9056
		public unsafe CacheSplatCBNode.Data* pdata;

		// Token: 0x020008BE RID: 2238
		public struct Data
		{
			// Token: 0x060051F9 RID: 20985 RVA: 0x0023EDBE File Offset: 0x0023CFBE
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_alphas);
			}

			// Token: 0x040040B6 RID: 16566
			public GetSplatCB in1;

			// Token: 0x040040B7 RID: 16567
			public int2 resolution;

			// Token: 0x040040B8 RID: 16568
			public float2 inv_cell_size;

			// Token: 0x040040B9 RID: 16569
			public int layers;

			// Token: 0x040040BA RID: 16570
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* alphas;

			// Token: 0x040040BB RID: 16571
			public ulong h_alphas;
		}
	}
}
