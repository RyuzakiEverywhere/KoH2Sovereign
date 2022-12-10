using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x020003A1 RID: 929
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Objects/PaintObject")]
	[Serializable]
	public class PaintObjectNode : Node
	{
		// Token: 0x0600353F RID: 13631 RVA: 0x001AB6C4 File Offset: 0x001A98C4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetObjects(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref PaintObjectNode.Data ptr = ref *(PaintObjectNode.Data*)data;
			if (ptr.objects.data == null)
			{
				return;
			}
			PaintObjectNode.AddData addData = default(PaintObjectNode.AddData);
			addData.data = (PaintObjectNode.Data*)data;
			addData.cb_func = cb.Value;
			addData.cb_data = cb_data;
			FunctionPointer<GetObjectsCB.AddObjectsFunc> cb2 = new FunctionPointer<GetObjectsCB.AddObjectsFunc>(ptr.add_object_func);
			ptr.objects.GetObjects(wx, wy, ww, wh, cb2, (void*)(&addData));
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x001AB734 File Offset: 0x001A9934
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddObject(ref ObjectInstance obj, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref PaintObjectNode.AddData ptr = ref *(PaintObjectNode.AddData*)data;
			ref PaintObjectNode.Data data2 = ref *ptr.data;
			float num;
			if (data2.mask.data != null)
			{
				num = data2.mask.GetValue(obj.position.x, obj.position.z);
			}
			else
			{
				num = 1f;
			}
			if (num < data2.cutout.x || num > data2.cutout.y)
			{
				return;
			}
			ptr.Add(ref obj);
		}

		// Token: 0x06003541 RID: 13633 RVA: 0x001AB7B0 File Offset: 0x001A99B0
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<PaintObjectNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (PaintObjectNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<PaintObjectNode.Data>();
			if (PaintObjectNode.get_objects_func == IntPtr.Zero)
			{
				PaintObjectNode.get_objects_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.GetObjectsFunc(PaintObjectNode.GetObjects)).Value;
			}
			if (PaintObjectNode.add_object_func == IntPtr.Zero)
			{
				PaintObjectNode.add_object_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(PaintObjectNode.AddObject)).Value;
			}
		}

		// Token: 0x06003542 RID: 13634 RVA: 0x001AB83A File Offset: 0x001A9A3A
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x06003543 RID: 13635 RVA: 0x001AB858 File Offset: 0x001A9A58
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			List<GameObject> objList = base.GetInputValue<ObjectsPackage>("objects", default(ObjectsPackage)).objList;
			return new ObjectsPackage
			{
				objects = new GetObjectsCB
				{
					get_objects_func = PaintObjectNode.get_objects_func,
					data = (void*)this.pdata
				},
				objList = objList
			};
		}

		// Token: 0x06003544 RID: 13636 RVA: 0x001AB8D8 File Offset: 0x001A9AD8
		private unsafe void FillData()
		{
			PaintObjectNode.Data* ptr = this.pdata;
			ptr->objects = base.GetInputValue<ObjectsPackage>("objects", default(ObjectsPackage)).objects;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.cutout;
			ptr->add_object_func = PaintObjectNode.add_object_func;
			this.isDataFilled = true;
		}

		// Token: 0x04002472 RID: 9330
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage objects;

		// Token: 0x04002473 RID: 9331
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x04002474 RID: 9332
		public float2 cutout = new float2(0f, 1f);

		// Token: 0x04002475 RID: 9333
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage result;

		// Token: 0x04002476 RID: 9334
		private bool isDataFilled;

		// Token: 0x04002477 RID: 9335
		public static IntPtr get_objects_func;

		// Token: 0x04002478 RID: 9336
		public static IntPtr add_object_func;

		// Token: 0x04002479 RID: 9337
		public NativeArray<PaintObjectNode.Data> arr_data;

		// Token: 0x0400247A RID: 9338
		public unsafe PaintObjectNode.Data* pdata;

		// Token: 0x020008EA RID: 2282
		public struct Data
		{
			// Token: 0x04004188 RID: 16776
			public GetObjectsCB objects;

			// Token: 0x04004189 RID: 16777
			public GetFloat2DCB mask;

			// Token: 0x0400418A RID: 16778
			public float2 cutout;

			// Token: 0x0400418B RID: 16779
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_object_func;
		}

		// Token: 0x020008EB RID: 2283
		public struct AddData
		{
			// Token: 0x06005226 RID: 21030 RVA: 0x0024024C File Offset: 0x0023E44C
			public void Add(ref ObjectInstance obj)
			{
				if (this.cb_func == IntPtr.Zero)
				{
					return;
				}
				FunctionPointer<GetObjectsCB.AddObjectsFunc> functionPointer = new FunctionPointer<GetObjectsCB.AddObjectsFunc>(this.cb_func);
				functionPointer.Invoke(ref obj, this.cb_data);
			}

			// Token: 0x0400418C RID: 16780
			[NativeDisableUnsafePtrRestriction]
			public unsafe PaintObjectNode.Data* data;

			// Token: 0x0400418D RID: 16781
			[NativeDisableUnsafePtrRestriction]
			public IntPtr cb_func;

			// Token: 0x0400418E RID: 16782
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* cb_data;
		}
	}
}
