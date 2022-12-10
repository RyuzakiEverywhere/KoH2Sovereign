using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200038E RID: 910
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Objects/InputObjects")]
	[Serializable]
	public class InputObjectsNode : Node
	{
		// Token: 0x060034BB RID: 13499 RVA: 0x001A5BE0 File Offset: 0x001A3DE0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetObjects(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref InputObjectsNode.Data ptr = ref *(InputObjectsNode.Data*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int cx = (int)@float.x;
			int cy = (int)@float.y;
			float num = (float)((int)(wx * ptr.grid.inv_cell_size.x)) * ptr.grid.cell_size.x;
			float num2 = (float)((int)(wy * ptr.grid.inv_cell_size.y)) * ptr.grid.cell_size.y;
			GrowBuffer<ObjectInstance> cell = ptr.GetCell(cx, cy);
			int count = cell.Count;
			for (int i = 0; i < count; i++)
			{
				ObjectInstance objectInstance = *cell[i];
				objectInstance.position.x = objectInstance.position.x + num;
				objectInstance.position.z = objectInstance.position.z + num2;
				cb.Invoke(ref objectInstance, cb_data);
			}
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x001A5CCC File Offset: 0x001A3ECC
		public unsafe override void Initialize()
		{
			this.arr_data = new NativeArray<InputObjectsNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pdata = (InputObjectsNode.Data*)this.arr_data.GetUnsafeReadOnlyPtr<InputObjectsNode.Data>();
			if (InputObjectsNode.get_objects_func == IntPtr.Zero)
			{
				InputObjectsNode.get_objects_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.GetObjectsFunc(InputObjectsNode.GetObjects)).Value;
			}
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x001A5D28 File Offset: 0x001A3F28
		public unsafe override void CleanUp()
		{
			if (this.pdata == null)
			{
				return;
			}
			ref InputObjectsNode.Data ptr = ref *this.pdata;
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					ptr.GetCell(j, i).Dispose();
				}
			}
			this.cells.Dispose();
			this.pdata = null;
			this.root_object = null;
			this.game_objects = null;
			this.arr_data.Dispose();
			this.isDataFilled = false;
		}

		// Token: 0x060034BE RID: 13502 RVA: 0x001A5DC0 File Offset: 0x001A3FC0
		public unsafe override object GetValue(NodePort port)
		{
			if (this.pdata != null && !this.isDataFilled)
			{
				this.FillData();
			}
			string fieldName = port.fieldName;
			if (fieldName == "objects")
			{
				return new ObjectsPackage
				{
					objects = new GetObjectsCB
					{
						get_objects_func = InputObjectsNode.get_objects_func,
						data = (void*)this.pdata
					},
					objList = this.game_objects
				};
			}
			return null;
		}

		// Token: 0x060034BF RID: 13503 RVA: 0x001A5E40 File Offset: 0x001A4040
		private unsafe void FillData()
		{
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputObjectsNode.Data ptr = ref *this.pdata;
			ptr.grid.cell_size = new float2(32f / transformation.Scale.x, 32f / transformation.Scale.y);
			ptr.grid.resolution = (int)(this.td.size.x / ptr.grid.cell_size.x);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			Terrain[] array = Object.FindObjectsOfType<Terrain>();
			this.root_object = null;
			if (array != null && array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].terrainData == this.td)
					{
						Terrain terrain = array[i];
						this.root_object = terrain.gameObject;
						break;
					}
				}
			}
			if (this.root_object == null)
			{
				this.root_object = this.GetTerrainPrefab(this.td.name);
			}
			if (this.root_object != null)
			{
				this.game_objects = BSGCloneHelper.GetObjectsToClone(this.root_object, this.LayerMask);
			}
			else
			{
				this.game_objects = new List<GameObject>();
			}
			ObjectInstance[] array2 = this.GenerateObjectInstancesArray(this.game_objects);
			this.cells = new NativeArray<IntPtr>(ptr.grid.resolution.y * ptr.grid.resolution.x, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			ptr.cells = (IntPtr*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<IntPtr>(this.cells);
			for (int j = 0; j < ptr.grid.resolution.y; j++)
			{
				for (int k = 0; k < ptr.grid.resolution.x; k++)
				{
					GrowBuffer<ObjectInstance> gb = new GrowBuffer<ObjectInstance>(Allocator.TempJob, 0);
					ptr.SetCell(k, j, gb);
				}
			}
			this.objects_remap.Clear();
			for (int l = 0; l < array2.Length; l++)
			{
				this.objects_remap.Add(array2[l].objectIndex);
			}
			ObjectInstance[] array3;
			ObjectInstance* arr_objects;
			if ((array3 = array2) == null || array3.Length == 0)
			{
				arr_objects = null;
			}
			else
			{
				arr_objects = &array3[0];
			}
			int[] array4;
			int* ptr2;
			if ((array4 = this.objects_remap.ToArray()) == null || array4.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array4[0];
			}
			InputObjectsNode.FillJob fillJob = default(InputObjectsNode.FillJob);
			fillJob.arr_objects = arr_objects;
			fillJob.objects_remap = ptr2;
			fillJob.pdata = this.pdata;
			fillJob.size = this.td.size;
			fillJob.verify = this.Verify;
			if (this.SingleThreaded)
			{
				fillJob.Run(array2.Length);
			}
			else
			{
				fillJob.Schedule(array2.Length, 1, default(JobHandle)).Complete();
			}
			if (fillJob.ignored != 0)
			{
				Debug.LogWarning(string.Format("Ignored {0} objects", fillJob.ignored));
			}
			array4 = null;
			array3 = null;
			if (this.Verify && !this.Validate(array2))
			{
				this.CleanUp();
			}
			this.isDataFilled = true;
		}

		// Token: 0x060034C0 RID: 13504 RVA: 0x001A6168 File Offset: 0x001A4368
		private ObjectInstance[] GenerateObjectInstancesArray(List<GameObject> game_objects)
		{
			ObjectInstance[] array = new ObjectInstance[game_objects.Count];
			for (int i = 0; i < game_objects.Count; i++)
			{
				Transform transform = game_objects[i].transform;
				float offsetFromGround = transform.localPosition.y - this.td.GetInterpolatedHeight(transform.localPosition.x / this.td.size.x, transform.localPosition.z / this.td.size.z);
				array[i] = new ObjectInstance
				{
					position = transform.localPosition,
					offsetFromGround = offsetFromGround,
					rotation = transform.localRotation,
					scale = transform.localScale,
					objectIndex = i
				};
			}
			return array;
		}

		// Token: 0x060034C1 RID: 13505 RVA: 0x001A623E File Offset: 0x001A443E
		private GameObject GetTerrainPrefab(string name)
		{
			return Resources.Load<GameObject>("TerrainPrefabs/" + name);
		}

		// Token: 0x060034C2 RID: 13506 RVA: 0x001A6250 File Offset: 0x001A4450
		public unsafe bool Validate(ObjectInstance[] arr_objects)
		{
			ref InputObjectsNode.Data ptr = ref *this.pdata;
			int[] array = new int[arr_objects.Length];
			int num = 0;
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					GrowBuffer<ObjectInstance> cell = ptr.GetCell(j, i);
					int count = cell.Count;
					num += count;
					for (int k = 0; k < count; k++)
					{
						ref ObjectInstance ptr2 = ref cell[k];
						int num2 = ptr2.objectIndex >> 4;
						ptr2.objectIndex &= 15;
						array[num2]++;
					}
				}
			}
			if (num != array.Length)
			{
				Debug.LogError(string.Format("Wrong number of imported objects: {0} instead of {1}", num, array.Length));
				return false;
			}
			for (int l = 0; l < num; l++)
			{
				int num3 = array[l];
				if (num3 != 1)
				{
					Debug.LogError(string.Format("Objects {0} imported {1} times", l, num3));
					return false;
				}
			}
			Debug.Log(string.Format("Successfuly imported {0} objects", num));
			return true;
		}

		// Token: 0x040023AC RID: 9132
		public TerrainData td;

		// Token: 0x040023AD RID: 9133
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x040023AE RID: 9134
		public LayerMask LayerMask = -1;

		// Token: 0x040023AF RID: 9135
		public bool SingleThreaded;

		// Token: 0x040023B0 RID: 9136
		public bool Verify;

		// Token: 0x040023B1 RID: 9137
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage objects;

		// Token: 0x040023B2 RID: 9138
		[HideInInspector]
		public List<int> objects_remap = new List<int>();

		// Token: 0x040023B3 RID: 9139
		private List<GameObject> game_objects;

		// Token: 0x040023B4 RID: 9140
		private GameObject root_object;

		// Token: 0x040023B5 RID: 9141
		private bool isDataFilled;

		// Token: 0x040023B6 RID: 9142
		public NativeArray<IntPtr> cells;

		// Token: 0x040023B7 RID: 9143
		public static IntPtr get_objects_func;

		// Token: 0x040023B8 RID: 9144
		public NativeArray<InputObjectsNode.Data> arr_data;

		// Token: 0x040023B9 RID: 9145
		public unsafe InputObjectsNode.Data* pdata;

		// Token: 0x020008C6 RID: 2246
		public struct Data
		{
			// Token: 0x06005200 RID: 20992 RVA: 0x0023EF37 File Offset: 0x0023D137
			public unsafe GrowBuffer<ObjectInstance> GetCell(int cx, int cy)
			{
				return new GrowBuffer<ObjectInstance>(this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}

			// Token: 0x06005201 RID: 20993 RVA: 0x0023EF62 File Offset: 0x0023D162
			public unsafe void SetCell(int cx, int cy, GrowBuffer<ObjectInstance> gb)
			{
				this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = gb.ToIntPtr();
			}

			// Token: 0x040040E0 RID: 16608
			public GridData grid;

			// Token: 0x040040E1 RID: 16609
			[NativeDisableUnsafePtrRestriction]
			public unsafe IntPtr* cells;
		}

		// Token: 0x020008C7 RID: 2247
		[BurstCompile]
		private struct FillJob : IJobParallelFor
		{
			// Token: 0x06005202 RID: 20994 RVA: 0x0023EF90 File Offset: 0x0023D190
			public unsafe void Execute(int index)
			{
				ref InputObjectsNode.Data ptr = ref *this.pdata;
				ObjectInstance objectInstance = this.arr_objects[index];
				objectInstance.objectIndex = this.objects_remap[objectInstance.objectIndex];
				if (this.verify)
				{
					objectInstance.objectIndex |= index << 4;
				}
				objectInstance.position.Scale(new Vector3(ptr.grid.scale.x, 0f, ptr.grid.scale.y));
				int num = (int)(objectInstance.position.x * ptr.grid.inv_cell_size.x);
				int num2 = (int)(objectInstance.position.z * ptr.grid.inv_cell_size.y);
				if (num < 0 || num >= ptr.grid.resolution.x || num2 < 0 || num2 >= ptr.grid.resolution.y)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				objectInstance.position.x = objectInstance.position.x - (float)num * ptr.grid.cell_size.x;
				objectInstance.position.z = objectInstance.position.z - (float)num2 * ptr.grid.cell_size.y;
				GrowBuffer<ObjectInstance> cell = ptr.GetCell(num, num2);
				if (!cell.IsCreated)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				cell.Add(objectInstance);
			}

			// Token: 0x040040E2 RID: 16610
			[NativeDisableUnsafePtrRestriction]
			public unsafe ObjectInstance* arr_objects;

			// Token: 0x040040E3 RID: 16611
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* objects_remap;

			// Token: 0x040040E4 RID: 16612
			[NativeDisableUnsafePtrRestriction]
			public unsafe InputObjectsNode.Data* pdata;

			// Token: 0x040040E5 RID: 16613
			public Vector3 size;

			// Token: 0x040040E6 RID: 16614
			public bool verify;

			// Token: 0x040040E7 RID: 16615
			public int ignored;
		}
	}
}
