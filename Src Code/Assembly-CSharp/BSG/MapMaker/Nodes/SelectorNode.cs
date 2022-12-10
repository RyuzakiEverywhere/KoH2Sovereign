using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003AC RID: 940
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Selector")]
	[Serializable]
	public class SelectorNode : Node
	{
		// Token: 0x06003566 RID: 13670 RVA: 0x001AC480 File Offset: 0x001AA680
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SelectorNode.Data ptr = ref *(SelectorNode.Data*)data;
			return BSGTerrainMask.CalcFadeInOut(ptr.input.GetValue(wx, wy), ptr.fade_in, ptr.min_val, ptr.max_val, ptr.fade_out);
		}

		// Token: 0x06003567 RID: 13671 RVA: 0x001AC4C4 File Offset: 0x001AA6C4
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SelectorNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SelectorNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SelectorNode.Data>();
			if (SelectorNode.get_value_func == IntPtr.Zero)
			{
				SelectorNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SelectorNode.GetValue)).Value;
			}
			this.mask_cache.Initialize();
		}

		// Token: 0x06003568 RID: 13672 RVA: 0x001AC52A File Offset: 0x001AA72A
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
			this.mask_cache.CleanUp();
		}

		// Token: 0x06003569 RID: 13673 RVA: 0x001AC554 File Offset: 0x001AA754
		public override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			if (this.mask_cache.IsCached())
			{
				return this.mask_cache.GetCallback();
			}
			return this.GetCallback();
		}

		// Token: 0x0600356A RID: 13674 RVA: 0x001AC5A4 File Offset: 0x001AA7A4
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = SelectorNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600356B RID: 13675 RVA: 0x001AC5D4 File Offset: 0x001AA7D4
		private unsafe void FillData()
		{
			SelectorNode.Data* ptr = this.pdata;
			ptr->input = base.GetInputValue<GetFloat2DCB>("input", default(GetFloat2DCB));
			ptr->fade_in = this.fade_in;
			ptr->min_val = this.min_val;
			ptr->max_val = this.max_val;
			ptr->fade_out = this.fade_out;
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					NodePort outputPort = base.GetOutputPort("mask");
					GetFloat2DCB callback = this.GetCallback();
					this.mask_cache.CacheData(outputPort, callback, terrainData.heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x040024A6 RID: 9382
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB input;

		// Token: 0x040024A7 RID: 9383
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB mask;

		// Token: 0x040024A8 RID: 9384
		public float fade_in = 0.1f;

		// Token: 0x040024A9 RID: 9385
		public float min_val = 0.33f;

		// Token: 0x040024AA RID: 9386
		public float max_val = 0.66f;

		// Token: 0x040024AB RID: 9387
		public float fade_out = 0.1f;

		// Token: 0x040024AC RID: 9388
		public bool cache = true;

		// Token: 0x040024AD RID: 9389
		private CacheFloat2D mask_cache = new CacheFloat2D();

		// Token: 0x040024AE RID: 9390
		private bool isDataFilled;

		// Token: 0x040024AF RID: 9391
		public static IntPtr get_value_func;

		// Token: 0x040024B0 RID: 9392
		public NativeArray<SelectorNode.Data> arr_data;

		// Token: 0x040024B1 RID: 9393
		public unsafe SelectorNode.Data* pdata;

		// Token: 0x020008F2 RID: 2290
		public struct Data
		{
			// Token: 0x040041A5 RID: 16805
			public GetFloat2DCB input;

			// Token: 0x040041A6 RID: 16806
			public float fade_in;

			// Token: 0x040041A7 RID: 16807
			public float min_val;

			// Token: 0x040041A8 RID: 16808
			public float max_val;

			// Token: 0x040041A9 RID: 16809
			public float fade_out;
		}
	}
}
