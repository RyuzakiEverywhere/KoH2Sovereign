using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000394 RID: 916
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Levels")]
	[Serializable]
	public class LevelsNode : Node
	{
		// Token: 0x060034F6 RID: 13558 RVA: 0x001A90F0 File Offset: 0x001A72F0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float x, float y, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref LevelsNode.Data ptr = ref *(LevelsNode.Data*)data;
			return Levels.Calc(ptr.input.GetValue(x, y), ptr.in_min, ptr.in_gamma, ptr.in_max, ptr.out_min, ptr.out_max);
		}

		// Token: 0x060034F7 RID: 13559 RVA: 0x001A913C File Offset: 0x001A733C
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<LevelsNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (LevelsNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<LevelsNode.Data>();
			if (LevelsNode.get_value_func == IntPtr.Zero)
			{
				LevelsNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(LevelsNode.GetValue)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x060034F8 RID: 13560 RVA: 0x001A91A2 File Offset: 0x001A73A2
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.res_cache.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x060034F9 RID: 13561 RVA: 0x001A91CC File Offset: 0x001A73CC
		private unsafe void FillData()
		{
			LevelsNode.Data* ptr = this.pdata;
			ptr->input = base.GetInputValue<GetFloat2DCB>("input", default(GetFloat2DCB));
			ptr->in_min = this.in_min;
			ptr->in_gamma = this.in_gamma;
			ptr->in_max = this.in_max;
			ptr->out_min = this.out_min;
			ptr->out_max = this.out_max;
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					NodePort outputPort = base.GetOutputPort("res");
					GetFloat2DCB callback = this.GetCallback();
					this.res_cache.CacheData(outputPort, callback, terrainData.heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x060034FA RID: 13562 RVA: 0x001A92AC File Offset: 0x001A74AC
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

		// Token: 0x060034FB RID: 13563 RVA: 0x001A92FC File Offset: 0x001A74FC
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = LevelsNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0400241A RID: 9242
		[Range(0f, 1f)]
		public float in_min;

		// Token: 0x0400241B RID: 9243
		[Range(-1f, 1f)]
		public float in_gamma;

		// Token: 0x0400241C RID: 9244
		[Range(0f, 1f)]
		public float in_max = 1f;

		// Token: 0x0400241D RID: 9245
		[Range(0f, 1f)]
		public float out_min;

		// Token: 0x0400241E RID: 9246
		[Range(0f, 1f)]
		public float out_max = 1f;

		// Token: 0x0400241F RID: 9247
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB input;

		// Token: 0x04002420 RID: 9248
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x04002421 RID: 9249
		public bool cache = true;

		// Token: 0x04002422 RID: 9250
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x04002423 RID: 9251
		private bool isDataFilled;

		// Token: 0x04002424 RID: 9252
		public static IntPtr get_value_func;

		// Token: 0x04002425 RID: 9253
		public NativeArray<LevelsNode.Data> arr_data;

		// Token: 0x04002426 RID: 9254
		public unsafe LevelsNode.Data* pdata;

		// Token: 0x020008D6 RID: 2262
		public struct Data
		{
			// Token: 0x0400412E RID: 16686
			public GetFloat2DCB input;

			// Token: 0x0400412F RID: 16687
			public float in_min;

			// Token: 0x04004130 RID: 16688
			public float in_gamma;

			// Token: 0x04004131 RID: 16689
			public float in_max;

			// Token: 0x04004132 RID: 16690
			public float out_min;

			// Token: 0x04004133 RID: 16691
			public float out_max;
		}
	}
}
