using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003AD RID: 941
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Noise/SimplePerlinNoise")]
	[Serializable]
	public class SimplePerlinNoiseNode : Node
	{
		// Token: 0x0600356D RID: 13677 RVA: 0x001AC6FC File Offset: 0x001AA8FC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref SimplePerlinNoiseNode.Data ptr = ref *(SimplePerlinNoiseNode.Data*)data;
			wx *= ptr.freqScale;
			wy *= ptr.freqScale;
			return Mathf.PerlinNoise(wx, wy) * ptr.valueScale;
		}

		// Token: 0x0600356E RID: 13678 RVA: 0x001AC738 File Offset: 0x001AA938
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<SimplePerlinNoiseNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (SimplePerlinNoiseNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<SimplePerlinNoiseNode.Data>();
			if (SimplePerlinNoiseNode.get_value_func == IntPtr.Zero)
			{
				SimplePerlinNoiseNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(SimplePerlinNoiseNode.GetValue)).Value;
			}
		}

		// Token: 0x0600356F RID: 13679 RVA: 0x001AC793 File Offset: 0x001AA993
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x06003570 RID: 13680 RVA: 0x001AC7AF File Offset: 0x001AA9AF
		private unsafe void FillData()
		{
			SimplePerlinNoiseNode.Data* ptr = this.pdata;
			ptr->freqScale = base.GetInputValue<float>("freqScale", this.freqScale);
			ptr->valueScale = base.GetInputValue<float>("valueScale", this.valueScale);
			this.isDataFilled = true;
		}

		// Token: 0x06003571 RID: 13681 RVA: 0x001AC7EC File Offset: 0x001AA9EC
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new GetFloat2DCB
			{
				get_value_func = SimplePerlinNoiseNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x040024B2 RID: 9394
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float freqScale = 0.01f;

		// Token: 0x040024B3 RID: 9395
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public float valueScale = 1f;

		// Token: 0x040024B4 RID: 9396
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x040024B5 RID: 9397
		private bool isDataFilled;

		// Token: 0x040024B6 RID: 9398
		public static IntPtr get_value_func;

		// Token: 0x040024B7 RID: 9399
		public NativeArray<SimplePerlinNoiseNode.Data> arr_data;

		// Token: 0x040024B8 RID: 9400
		public unsafe SimplePerlinNoiseNode.Data* pdata;

		// Token: 0x020008F3 RID: 2291
		public struct Data
		{
			// Token: 0x040041AA RID: 16810
			public float freqScale;

			// Token: 0x040041AB RID: 16811
			public float valueScale;
		}
	}
}
