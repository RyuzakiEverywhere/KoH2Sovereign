using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003AE RID: 942
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Misc/Slope")]
	[Serializable]
	public class SlopeNode : Node
	{
		// Token: 0x06003573 RID: 13683 RVA: 0x001AC858 File Offset: 0x001AAA58
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetMaskValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SlopeNode.Data ptr = ref *(SlopeNode.Data*)data;
			return BSGTerrainMask.CalcFadeInOut(SlopeNode.CalculateSteepness(wx, wy, ptr.input, ptr.terrainSize, ptr.heightmapSize), ptr.fade_in, ptr.min_val, ptr.max_val, ptr.fade_out);
		}

		// Token: 0x06003574 RID: 13684 RVA: 0x001AC8A8 File Offset: 0x001AAAA8
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetSlopeValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SlopeNode.Data ptr = ref *(SlopeNode.Data*)data;
			return SlopeNode.CalculateSteepness(wx, wy, ptr.input, ptr.terrainSize, ptr.heightmapSize);
		}

		// Token: 0x06003575 RID: 13685 RVA: 0x001AC8DC File Offset: 0x001AAADC
		public static float CalculateSteepness(float x, float y, GetFloat2DCB input, float3 terrainSize, float2 heightmapSize)
		{
			float wx = x - 1f;
			float wy = y - 1f;
			float wx2 = x + 1f;
			float wy2 = y + 1f;
			float num = input.GetValue(wx, wy) * terrainSize.y;
			float num2 = input.GetValue(wx, y) * terrainSize.y;
			float num3 = input.GetValue(wx, wy2) * terrainSize.y;
			float num4 = input.GetValue(x, wy2) * terrainSize.y;
			float num5 = input.GetValue(wx2, wy2) * terrainSize.y;
			float num6 = input.GetValue(wx2, y) * terrainSize.y;
			float num7 = input.GetValue(wx2, wy) * terrainSize.y;
			float num8 = input.GetValue(x, wy) * terrainSize.y;
			float num9 = (num3 + 2f * num2 + num - (num5 + 2f * num6 + num7)) * 0.125f;
			float num10 = (num7 + 2f * num8 + num - (num5 + 2f * num4 + num3)) * 0.125f;
			return Mathf.Acos(Vector3.Dot(new Vector3(-num9 * (heightmapSize.x - 1f) / terrainSize.x, 1f, -num10 * (heightmapSize.y - 1f) / terrainSize.z).normalized, Vector3.up)) * 57.29578f / 90f;
		}

		// Token: 0x06003576 RID: 13686 RVA: 0x001ACA40 File Offset: 0x001AAC40
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SlopeNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SlopeNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SlopeNode.Data>();
			if (SlopeNode.get_slope_value_func == IntPtr.Zero)
			{
				SlopeNode.get_slope_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SlopeNode.GetSlopeValue)).Value;
			}
			if (SlopeNode.get_mask_value_func == IntPtr.Zero)
			{
				SlopeNode.get_mask_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SlopeNode.GetMaskValue)).Value;
			}
			this.slope_mask_cache.Initialize();
			this.slope_cache.Initialize();
		}

		// Token: 0x06003577 RID: 13687 RVA: 0x001ACAE0 File Offset: 0x001AACE0
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.slope_mask_cache.CleanUp();
			this.slope_cache.CleanUp();
			this.isDataFilled = false;
		}

		// Token: 0x06003578 RID: 13688 RVA: 0x001ACB14 File Offset: 0x001AAD14
		public override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			string fieldName = port.fieldName;
			if (fieldName == "slope")
			{
				return this.slope_cache.IsCached() ? this.slope_cache.GetCallback() : this.GetSlopeCallback();
			}
			if (!(fieldName == "slopeMask"))
			{
				return null;
			}
			return this.slope_mask_cache.IsCached() ? this.slope_mask_cache.GetCallback() : this.GetSlopeMaskCallback();
		}

		// Token: 0x06003579 RID: 13689 RVA: 0x001ACBAC File Offset: 0x001AADAC
		private unsafe GetFloat2DCB GetSlopeMaskCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = SlopeNode.get_mask_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600357A RID: 13690 RVA: 0x001ACBDC File Offset: 0x001AADDC
		private unsafe GetFloat2DCB GetSlopeCallback()
		{
			return new GetFloat2DCB
			{
				get_value_func = SlopeNode.get_slope_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x0600357B RID: 13691 RVA: 0x001ACC0C File Offset: 0x001AAE0C
		private unsafe void FillData()
		{
			SlopeNode.Data* ptr = this.pdata;
			ptr->input = base.GetInputValue<GetFloat2DCB>("input", default(GetFloat2DCB));
			ptr->terrainSize = base.GetInputValue<float3>("terrainSize", this.terrainSize);
			ptr->heightmapSize = base.GetInputValue<float2>("heightmapSize", this.heightmapSize);
			ptr->fade_in = this.fade_in / 90f;
			ptr->min_val = this.min_val / 90f;
			ptr->max_val = this.max_val / 90f;
			ptr->fade_out = this.fade_out / 90f;
			if (this.cache && MapMakerGraph.Auto_Cache)
			{
				Terrain tgt_terrain = base.mm.tgt_terrain;
				TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
				if (terrainData != null)
				{
					NodePort outputPort = base.GetOutputPort("slope");
					GetFloat2DCB slopeCallback = this.GetSlopeCallback();
					this.slope_cache.CacheData(outputPort, slopeCallback, terrainData.heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
					NodePort outputPort2 = base.GetOutputPort("slopeMask");
					GetFloat2DCB slopeMaskCallback = this.GetSlopeMaskCallback();
					this.slope_mask_cache.CacheData(outputPort2, slopeMaskCallback, terrainData.heightmapResolution, new float2(terrainData.size.x, terrainData.size.z));
				}
			}
			this.isDataFilled = true;
		}

		// Token: 0x040024B9 RID: 9401
		private const float max_steepness = 90f;

		// Token: 0x040024BA RID: 9402
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB input;

		// Token: 0x040024BB RID: 9403
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float3 terrainSize;

		// Token: 0x040024BC RID: 9404
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float2 heightmapSize;

		// Token: 0x040024BD RID: 9405
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB slope;

		// Token: 0x040024BE RID: 9406
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB slopeMask;

		// Token: 0x040024BF RID: 9407
		[Range(0f, 90f)]
		public float fade_in = 20f;

		// Token: 0x040024C0 RID: 9408
		[Range(0f, 90f)]
		public float min_val = 40f;

		// Token: 0x040024C1 RID: 9409
		[Range(0f, 90f)]
		public float max_val = 90f;

		// Token: 0x040024C2 RID: 9410
		[Range(0f, 90f)]
		public float fade_out = 20f;

		// Token: 0x040024C3 RID: 9411
		public bool cache = true;

		// Token: 0x040024C4 RID: 9412
		private CacheFloat2D slope_cache = new CacheFloat2D();

		// Token: 0x040024C5 RID: 9413
		private CacheFloat2D slope_mask_cache = new CacheFloat2D();

		// Token: 0x040024C6 RID: 9414
		private bool isDataFilled;

		// Token: 0x040024C7 RID: 9415
		public static IntPtr get_slope_value_func;

		// Token: 0x040024C8 RID: 9416
		public static IntPtr get_mask_value_func;

		// Token: 0x040024C9 RID: 9417
		public NativeArray<SlopeNode.Data> arr_data;

		// Token: 0x040024CA RID: 9418
		public unsafe SlopeNode.Data* pdata;

		// Token: 0x020008F4 RID: 2292
		public struct Data
		{
			// Token: 0x040041AC RID: 16812
			public GetFloat2DCB input;

			// Token: 0x040041AD RID: 16813
			public float3 terrainSize;

			// Token: 0x040041AE RID: 16814
			public float2 heightmapSize;

			// Token: 0x040041AF RID: 16815
			public float fade_in;

			// Token: 0x040041B0 RID: 16816
			public float min_val;

			// Token: 0x040041B1 RID: 16817
			public float max_val;

			// Token: 0x040041B2 RID: 16818
			public float fade_out;
		}
	}
}
