using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000396 RID: 918
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Math")]
	[Serializable]
	public class MathNode : Node
	{
		// Token: 0x060034FD RID: 13565 RVA: 0x001A935C File Offset: 0x001A755C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float MulAndAddFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref MathNode.Data ptr = ref *(MathNode.Data*)data;
			return ptr.input.GetValue(wx, wy) * ptr.param1 + ptr.param2;
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x001A9394 File Offset: 0x001A7594
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float PowerFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref MathNode.Data ptr = ref *(MathNode.Data*)data;
			return Mathf.Pow(ptr.input.GetValue(wx, wy), ptr.param1);
		}

		// Token: 0x060034FF RID: 13567 RVA: 0x001A93C8 File Offset: 0x001A75C8
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float ClampAndNormalizeFunc(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref MathNode.Data ptr = ref *(MathNode.Data*)data;
			float num = ptr.input.GetValue(wx, wy);
			if (num <= ptr.param1)
			{
				num = 0f;
			}
			else if (num > ptr.param2)
			{
				num = 1f;
			}
			else
			{
				num = (num - ptr.param1) / (ptr.param2 - ptr.param1);
			}
			return num;
		}

		// Token: 0x06003500 RID: 13568 RVA: 0x001A942C File Offset: 0x001A762C
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<MathNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (MathNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<MathNode.Data>();
			if (MathNode.get_value_funcs == null)
			{
				MathNode.get_value_funcs = new IntPtr[3];
				MathNode.get_value_funcs[0] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(MathNode.MulAndAddFunc)).Value;
				MathNode.get_value_funcs[1] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(MathNode.PowerFunc)).Value;
				MathNode.get_value_funcs[2] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(MathNode.ClampAndNormalizeFunc)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x06003501 RID: 13569 RVA: 0x001A94D5 File Offset: 0x001A76D5
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.res_cache.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x06003502 RID: 13570 RVA: 0x001A94FC File Offset: 0x001A76FC
		private unsafe void FillData()
		{
			MathNode.Data* ptr = this.pdata;
			ptr->input = base.GetInputValue<GetFloat2DCB>("input", default(GetFloat2DCB));
			ptr->param1 = this.param1;
			ptr->param2 = this.param2;
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

		// Token: 0x06003503 RID: 13571 RVA: 0x001A95B8 File Offset: 0x001A77B8
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

		// Token: 0x06003504 RID: 13572 RVA: 0x001A9608 File Offset: 0x001A7808
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = MathNode.get_value_funcs[(int)this.operation],
				data = (void*)this.pdata
			};
		}

		// Token: 0x0400242B RID: 9259
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB input;

		// Token: 0x0400242C RID: 9260
		public OperationType operation;

		// Token: 0x0400242D RID: 9261
		public float param1;

		// Token: 0x0400242E RID: 9262
		public float param2;

		// Token: 0x0400242F RID: 9263
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x04002430 RID: 9264
		public bool cache = true;

		// Token: 0x04002431 RID: 9265
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x04002432 RID: 9266
		private bool isDataFilled;

		// Token: 0x04002433 RID: 9267
		public static IntPtr[] get_value_funcs;

		// Token: 0x04002434 RID: 9268
		public NativeArray<MathNode.Data> arr_data;

		// Token: 0x04002435 RID: 9269
		public unsafe MathNode.Data* pdata;

		// Token: 0x020008D7 RID: 2263
		public struct Data
		{
			// Token: 0x04004134 RID: 16692
			public GetFloat2DCB input;

			// Token: 0x04004135 RID: 16693
			public float param1;

			// Token: 0x04004136 RID: 16694
			public float param2;
		}
	}
}
