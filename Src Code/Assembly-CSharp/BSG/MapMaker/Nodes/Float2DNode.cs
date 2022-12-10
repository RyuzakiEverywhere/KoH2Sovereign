using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x02000385 RID: 901
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Const/Float2D")]
	[Serializable]
	public class Float2DNode : Node
	{
		// Token: 0x06003496 RID: 13462 RVA: 0x001A4E0C File Offset: 0x001A300C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			return *(float*)data;
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x001A4E1C File Offset: 0x001A301C
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (float*)this.arr_data.GetUnsafeReadOnlyPtr<float>();
			if (Float2DNode.get_value_func == IntPtr.Zero)
			{
				Float2DNode.get_value_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(Float2DNode.GetValue)).Value;
			}
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x001A4E77 File Offset: 0x001A3077
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
		}

		// Token: 0x06003499 RID: 13465 RVA: 0x001A4E8C File Offset: 0x001A308C
		public unsafe override object GetValue(NodePort port)
		{
			*this.pdata = this.value;
			return new GetFloat2DCB
			{
				get_value_func = Float2DNode.get_value_func,
				data = (void*)this.pdata
			};
		}

		// Token: 0x04002389 RID: 9097
		public float value;

		// Token: 0x0400238A RID: 9098
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB res;

		// Token: 0x0400238B RID: 9099
		public static IntPtr get_value_func;

		// Token: 0x0400238C RID: 9100
		public NativeArray<float> arr_data;

		// Token: 0x0400238D RID: 9101
		public unsafe float* pdata;
	}
}
