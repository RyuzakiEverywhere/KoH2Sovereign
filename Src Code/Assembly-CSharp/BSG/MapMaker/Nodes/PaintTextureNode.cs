using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A2 RID: 930
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Splats/PaintTexture")]
	[Serializable]
	public class PaintTextureNode : Node
	{
		// Token: 0x06003546 RID: 13638 RVA: 0x001AB960 File Offset: 0x001A9B60
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref PaintTextureNode.Data ptr = ref *(PaintTextureNode.Data*)data;
			float num = ptr.strength;
			float num2 = 1f;
			if (ptr.mask.data != null)
			{
				num2 = ptr.mask.GetValue(wx, wy);
			}
			result[ptr.splatIndex] = num * num2;
		}

		// Token: 0x06003547 RID: 13639 RVA: 0x001AB9B0 File Offset: 0x001A9BB0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<PaintTextureNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (PaintTextureNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<PaintTextureNode.Data>();
			if (PaintTextureNode.get_tile_func == IntPtr.Zero)
			{
				PaintTextureNode.get_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(PaintTextureNode.GetTile)).Value;
			}
		}

		// Token: 0x06003548 RID: 13640 RVA: 0x001ABA0B File Offset: 0x001A9C0B
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x06003549 RID: 13641 RVA: 0x001ABA28 File Offset: 0x001A9C28
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetSplatCB
			{
				get_tile_func = PaintTextureNode.get_tile_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600354A RID: 13642 RVA: 0x001ABA74 File Offset: 0x001A9C74
		private unsafe void FillData()
		{
			ref PaintTextureNode.Data ptr = ref *this.pdata;
			ptr.input = base.GetInputValue<GetSplatCB>("input", default(GetSplatCB));
			ptr.mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr.strength = this.strength;
			ptr.splatIndex = -1;
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				TerrainLayer[] terrainLayers = terrainData.terrainLayers;
				for (int i = 0; i < terrainLayers.Length; i++)
				{
					if (terrainLayers[i] == this.selectedTerrainLayer)
					{
						ptr.splatIndex = i;
						break;
					}
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x0400247B RID: 9339
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB input;

		// Token: 0x0400247C RID: 9340
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x0400247D RID: 9341
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB output;

		// Token: 0x0400247E RID: 9342
		[HideInInspector]
		public TerrainLayer selectedTerrainLayer;

		// Token: 0x0400247F RID: 9343
		public float strength = 1f;

		// Token: 0x04002480 RID: 9344
		private bool isDataFilled;

		// Token: 0x04002481 RID: 9345
		public static IntPtr get_tile_func;

		// Token: 0x04002482 RID: 9346
		public NativeArray<PaintTextureNode.Data> arr_data;

		// Token: 0x04002483 RID: 9347
		public unsafe PaintTextureNode.Data* pdata;

		// Token: 0x020008EC RID: 2284
		public struct Data
		{
			// Token: 0x0400418F RID: 16783
			public GetSplatCB input;

			// Token: 0x04004190 RID: 16784
			public GetFloat2DCB mask;

			// Token: 0x04004191 RID: 16785
			public float strength;

			// Token: 0x04004192 RID: 16786
			public int splatIndex;
		}
	}
}
