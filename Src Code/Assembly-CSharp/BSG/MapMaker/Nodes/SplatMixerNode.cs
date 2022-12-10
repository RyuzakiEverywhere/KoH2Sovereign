using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003B0 RID: 944
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Splats/SplatMixer")]
	[Serializable]
	public class SplatMixerNode : Node
	{
		// Token: 0x06003587 RID: 13703 RVA: 0x001AD4C8 File Offset: 0x001AB6C8
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref SplatMixerNode.Data ptr = ref *(SplatMixerNode.Data*)data;
			float num = ptr.strength;
			if (num <= 0f)
			{
				ptr.in1.GetTile(wx, wy, result, layers);
				return;
			}
			if (ptr.mask.data != null)
			{
				num *= ptr.mask.GetValue(wx, wy);
				if (num <= 0f)
				{
					ptr.in1.GetTile(wx, wy, result, layers);
					return;
				}
			}
			if (num >= 1f)
			{
				ptr.in2.GetTile(wx, wy, result, layers);
				return;
			}
			float* ptr2;
			float* ptr3;
			float num2;
			checked
			{
				ptr2 = stackalloc float[unchecked((UIntPtr)layers) * 4];
				ptr.in1.GetTile(wx, wy, ptr2, layers);
				ptr3 = stackalloc float[unchecked((UIntPtr)layers) * 4];
				ptr.in2.GetTile(wx, wy, ptr3, layers);
				num2 = 0f;
			}
			for (int i = 0; i < layers; i++)
			{
				float num3 = ptr2[i];
				float num4 = ptr3[i];
				result[i] = (1f - num) * num3 + num * num4;
				num2 += result[i];
			}
			float num5 = 1f / num2;
			for (int j = 0; j < layers; j++)
			{
				result[j] *= num5;
			}
		}

		// Token: 0x06003588 RID: 13704 RVA: 0x001AD5F0 File Offset: 0x001AB7F0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SplatMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SplatMixerNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SplatMixerNode.Data>();
			if (SplatMixerNode.get_tile_func == IntPtr.Zero)
			{
				SplatMixerNode.get_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(SplatMixerNode.GetTile)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x06003589 RID: 13705 RVA: 0x001AD656 File Offset: 0x001AB856
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.res_cache.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x0600358A RID: 13706 RVA: 0x001AD680 File Offset: 0x001AB880
		public override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			if (this.res_cache.IsCached())
			{
				return this.res_cache.GetCallback();
			}
			return this.GetCallback();
		}

		// Token: 0x0600358B RID: 13707 RVA: 0x001AD6D0 File Offset: 0x001AB8D0
		private unsafe GetSplatCB GetCallback()
		{
			return new GetSplatCB
			{
				get_tile_func = SplatMixerNode.get_tile_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600358C RID: 13708 RVA: 0x001AD700 File Offset: 0x001AB900
		private unsafe void FillData()
		{
			SplatMixerNode.Data* ptr = this.pdata;
			ptr->in1 = base.GetInputValue<GetSplatCB>("in1", default(GetSplatCB));
			ptr->in2 = base.GetInputValue<GetSplatCB>("in2", default(GetSplatCB));
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->strength = this.strength;
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					NodePort outputPort = base.GetOutputPort("res");
					GetSplatCB callback = this.GetCallback();
					this.res_cache.CacheData(outputPort, callback, terrainData.alphamapResolution, new float2(terrainData.size.x, terrainData.size.z), terrainData.alphamapLayers);
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x040024DC RID: 9436
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB in1;

		// Token: 0x040024DD RID: 9437
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB in2;

		// Token: 0x040024DE RID: 9438
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x040024DF RID: 9439
		public float strength = 1f;

		// Token: 0x040024E0 RID: 9440
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB res;

		// Token: 0x040024E1 RID: 9441
		public bool cache = true;

		// Token: 0x040024E2 RID: 9442
		private CacheSplatsCB res_cache = new CacheSplatsCB();

		// Token: 0x040024E3 RID: 9443
		private bool isDataFilled;

		// Token: 0x040024E4 RID: 9444
		public static IntPtr get_tile_func;

		// Token: 0x040024E5 RID: 9445
		public NativeArray<SplatMixerNode.Data> arr_data;

		// Token: 0x040024E6 RID: 9446
		public unsafe SplatMixerNode.Data* pdata;

		// Token: 0x020008F8 RID: 2296
		public struct Data
		{
			// Token: 0x040041C8 RID: 16840
			public GetSplatCB in1;

			// Token: 0x040041C9 RID: 16841
			public GetSplatCB in2;

			// Token: 0x040041CA RID: 16842
			public GetFloat2DCB mask;

			// Token: 0x040041CB RID: 16843
			public float strength;
		}
	}
}
