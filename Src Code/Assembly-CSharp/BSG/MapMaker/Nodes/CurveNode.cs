using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000383 RID: 899
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Curve")]
	public class CurveNode : Node
	{
		// Token: 0x06003488 RID: 13448 RVA: 0x001A4980 File Offset: 0x001A2B80
		public unsafe static float Evaluate(float x, float resolution, float* values)
		{
			if (x > 1f)
			{
				x = 1f;
			}
			else if (x < 0f)
			{
				x = 0f;
			}
			int num = Mathf.RoundToInt(x * resolution);
			return values[num];
		}

		// Token: 0x06003489 RID: 13449 RVA: 0x001A49C0 File Offset: 0x001A2BC0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref CurveNode.Data ptr = ref *(CurveNode.Data*)data;
			return CurveNode.Evaluate(ptr.input.GetValue(wx, wy), ptr.resolution, ptr.values);
		}

		// Token: 0x0600348A RID: 13450 RVA: 0x001A49F8 File Offset: 0x001A2BF8
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<CurveNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (CurveNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<CurveNode.Data>();
			if (CurveNode.get_value_func == IntPtr.Zero)
			{
				CurveNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CurveNode.GetValue)).Value;
			}
			this.res_cache.Initialize();
		}

		// Token: 0x0600348B RID: 13451 RVA: 0x001A4A5E File Offset: 0x001A2C5E
		public unsafe override void CleanUp()
		{
			this.pdata->Dispose();
			this.pdata = null;
			this.arr_data.Dispose();
			this.res_cache.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x0600348C RID: 13452 RVA: 0x001A4A90 File Offset: 0x001A2C90
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

		// Token: 0x0600348D RID: 13453 RVA: 0x001A4AE0 File Offset: 0x001A2CE0
		private unsafe GetFloat2DCB GetCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = CurveNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x001A4B10 File Offset: 0x001A2D10
		private unsafe void FillData()
		{
			ref CurveNode.Data ptr = ref *this.pdata;
			ptr.resolution = (float)this.resolution;
			float[] array = new float[this.resolution];
			for (int i = 0; i < this.resolution; i++)
			{
				array[i] = this.curve.Evaluate((float)i / (float)this.resolution);
			}
			ptr.values = (float*)AllocationManager.PinGCArrayAndGetDataAddress(array, ref ptr.h_values);
			ptr.input = base.GetInputValue<GetFloat2DCB>("input", default(GetFloat2DCB));
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

		// Token: 0x04002379 RID: 9081
		public AnimationCurve curve;

		// Token: 0x0400237A RID: 9082
		public int resolution = 64000;

		// Token: 0x0400237B RID: 9083
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB input;

		// Token: 0x0400237C RID: 9084
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x0400237D RID: 9085
		public bool cache = true;

		// Token: 0x0400237E RID: 9086
		private CacheFloat2D res_cache = new CacheFloat2D();

		// Token: 0x0400237F RID: 9087
		private bool isDataFilled;

		// Token: 0x04002380 RID: 9088
		public static IntPtr get_value_func;

		// Token: 0x04002381 RID: 9089
		public NativeArray<CurveNode.Data> arr_data;

		// Token: 0x04002382 RID: 9090
		public unsafe CurveNode.Data* pdata;

		// Token: 0x020008C2 RID: 2242
		public struct Data
		{
			// Token: 0x060051FC RID: 20988 RVA: 0x0023EE8D File Offset: 0x0023D08D
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_values);
			}

			// Token: 0x040040C9 RID: 16585
			public GetFloat2DCB input;

			// Token: 0x040040CA RID: 16586
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* values;

			// Token: 0x040040CB RID: 16587
			public ulong h_values;

			// Token: 0x040040CC RID: 16588
			public float resolution;
		}
	}
}
