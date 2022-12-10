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
	// Token: 0x02000398 RID: 920
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Objects/ObjectsMixer")]
	[Serializable]
	public class ObjectsMixerNode : Node
	{
		// Token: 0x0600350D RID: 13581 RVA: 0x001A9810 File Offset: 0x001A7A10
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetObjects(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref ObjectsMixerNode.Data ptr = ref *(ObjectsMixerNode.Data*)data;
			if (ptr.in1.data == null)
			{
				return;
			}
			ObjectsMixerNode.AddData addData = default(ObjectsMixerNode.AddData);
			addData.data = (ObjectsMixerNode.Data*)data;
			addData.cb_func = cb.Value;
			addData.cb_data = cb_data;
			FunctionPointer<GetObjectsCB.AddObjectsFunc> cb2 = new FunctionPointer<GetObjectsCB.AddObjectsFunc>(ptr.add_object_func);
			FunctionPointer<GetObjectsCB.AddObjectsFunc> cb3 = new FunctionPointer<GetObjectsCB.AddObjectsFunc>(ptr.add_object_with_mask_func);
			ptr.in1.GetObjects(wx, wy, ww, wh, cb2, (void*)(&addData));
			ptr.in2.GetObjects(wx, wy, ww, wh, cb3, (void*)(&addData));
		}

		// Token: 0x0600350E RID: 13582 RVA: 0x001A989F File Offset: 0x001A7A9F
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddObject(ref ObjectInstance obj, void* data)
		{
			if (data == null)
			{
				return;
			}
			ObjectsMixerNode.Data data2 = *((ObjectsMixerNode.AddData*)data)->data;
			((ObjectsMixerNode.AddData*)data)->Add(ref obj);
		}

		// Token: 0x0600350F RID: 13583 RVA: 0x001A98BC File Offset: 0x001A7ABC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddObjectWithMask(ref ObjectInstance obj, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref ObjectsMixerNode.AddData ptr = ref *(ObjectsMixerNode.AddData*)data;
			ref ObjectsMixerNode.Data data2 = ref *ptr.data;
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
			obj.objectIndex += data2.indexOffset;
			ptr.Add(ref obj);
		}

		// Token: 0x06003510 RID: 13584 RVA: 0x001A9948 File Offset: 0x001A7B48
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<ObjectsMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (ObjectsMixerNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<ObjectsMixerNode.Data>();
			if (ObjectsMixerNode.get_objects_func == IntPtr.Zero)
			{
				ObjectsMixerNode.get_objects_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.GetObjectsFunc(ObjectsMixerNode.GetObjects)).Value;
			}
			if (ObjectsMixerNode.add_object_func == IntPtr.Zero)
			{
				ObjectsMixerNode.add_object_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(ObjectsMixerNode.AddObject)).Value;
			}
			if (ObjectsMixerNode.add_object_with_mask_func == IntPtr.Zero)
			{
				ObjectsMixerNode.add_object_with_mask_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(ObjectsMixerNode.AddObjectWithMask)).Value;
			}
		}

		// Token: 0x06003511 RID: 13585 RVA: 0x001A9A01 File Offset: 0x001A7C01
		public override void CleanUp()
		{
			this.pdata = null;
			this.arr_data.Dispose();
			this.game_objects = null;
			this.isDataFilled = false;
		}

		// Token: 0x06003512 RID: 13586 RVA: 0x001A9A24 File Offset: 0x001A7C24
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			return new ObjectsPackage
			{
				objects = new GetObjectsCB
				{
					get_objects_func = ObjectsMixerNode.get_objects_func,
					data = (void*)this.pdata
				},
				objList = this.game_objects
			};
		}

		// Token: 0x06003513 RID: 13587 RVA: 0x001A9A90 File Offset: 0x001A7C90
		private unsafe void FillData()
		{
			this.in1 = base.GetInputValue<ObjectsPackage>("in1", default(ObjectsPackage));
			this.in2 = base.GetInputValue<ObjectsPackage>("in2", default(ObjectsPackage));
			ObjectsMixerNode.Data* ptr = this.pdata;
			ptr->in1 = this.in1.objects;
			ptr->in2 = this.in2.objects;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.cutout;
			ptr->add_object_func = ObjectsMixerNode.add_object_func;
			ptr->add_object_with_mask_func = ObjectsMixerNode.add_object_with_mask_func;
			ptr->indexOffset = this.in1.objList.Count;
			this.game_objects = new List<GameObject>(this.in1.objList);
			this.game_objects.AddRange(this.in2.objList);
			this.isDataFilled = true;
		}

		// Token: 0x04002439 RID: 9273
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage in1;

		// Token: 0x0400243A RID: 9274
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage in2;

		// Token: 0x0400243B RID: 9275
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x0400243C RID: 9276
		public float2 cutout = new float2(0f, 1f);

		// Token: 0x0400243D RID: 9277
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage objects;

		// Token: 0x0400243E RID: 9278
		private List<GameObject> game_objects;

		// Token: 0x0400243F RID: 9279
		private bool isDataFilled;

		// Token: 0x04002440 RID: 9280
		public static IntPtr get_objects_func;

		// Token: 0x04002441 RID: 9281
		public static IntPtr add_object_func;

		// Token: 0x04002442 RID: 9282
		public static IntPtr add_object_with_mask_func;

		// Token: 0x04002443 RID: 9283
		public NativeArray<ObjectsMixerNode.Data> arr_data;

		// Token: 0x04002444 RID: 9284
		public unsafe ObjectsMixerNode.Data* pdata;

		// Token: 0x020008D8 RID: 2264
		public struct Data
		{
			// Token: 0x04004137 RID: 16695
			public GetObjectsCB in1;

			// Token: 0x04004138 RID: 16696
			public GetObjectsCB in2;

			// Token: 0x04004139 RID: 16697
			public GetFloat2DCB mask;

			// Token: 0x0400413A RID: 16698
			public float2 cutout;

			// Token: 0x0400413B RID: 16699
			public int indexOffset;

			// Token: 0x0400413C RID: 16700
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_object_func;

			// Token: 0x0400413D RID: 16701
			[NativeDisableUnsafePtrRestriction]
			public IntPtr add_object_with_mask_func;
		}

		// Token: 0x020008D9 RID: 2265
		public struct AddData
		{
			// Token: 0x06005215 RID: 21013 RVA: 0x0023F9F8 File Offset: 0x0023DBF8
			public void Add(ref ObjectInstance obj)
			{
				if (this.cb_func == IntPtr.Zero)
				{
					return;
				}
				FunctionPointer<GetObjectsCB.AddObjectsFunc> functionPointer = new FunctionPointer<GetObjectsCB.AddObjectsFunc>(this.cb_func);
				functionPointer.Invoke(ref obj, this.cb_data);
			}

			// Token: 0x0400413E RID: 16702
			[NativeDisableUnsafePtrRestriction]
			public unsafe ObjectsMixerNode.Data* data;

			// Token: 0x0400413F RID: 16703
			[NativeDisableUnsafePtrRestriction]
			public IntPtr cb_func;

			// Token: 0x04004140 RID: 16704
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* cb_data;
		}
	}
}
