using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200039B RID: 923
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Objects/OutputObjects")]
	[Serializable]
	public class OutputObjectsNode : Node, IOutputNode
	{
		// Token: 0x0600351D RID: 13597 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x001A9DD0 File Offset: 0x001A7FD0
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddObject(ref ObjectInstance obj, void* data)
		{
			if (data == null)
			{
				return;
			}
			((OutputObjectsNode.AddData*)data)->Add(obj);
		}

		// Token: 0x0600351F RID: 13599 RVA: 0x001A9DE4 File Offset: 0x001A7FE4
		public unsafe override void Build()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			this.objects = base.GetInputValue<ObjectsPackage>("objects", default(ObjectsPackage));
			if (this.objects.objects.data == null)
			{
				return;
			}
			if (!OutputObjectsNode.add_object_func.IsCreated)
			{
				OutputObjectsNode.add_object_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(OutputObjectsNode.AddObject));
			}
			GrowBuffer<ObjectInstance> growBuffer = new GrowBuffer<ObjectInstance>(Allocator.TempJob, 65536);
			OutputObjectsNode.BuildJob buildJob = default(OutputObjectsNode.BuildJob);
			buildJob.tile_size = new float2(32f, 32f);
			buildJob.resolution = (int)(terrainData.size.x / buildJob.tile_size.x);
			buildJob.cb = this.objects.objects;
			buildJob.add_func = OutputObjectsNode.add_object_func;
			OutputObjectsNode.AddData addData = default(OutputObjectsNode.AddData);
			addData.buf_data = growBuffer.data;
			buildJob.add_func_data = (void*)(&addData);
			if (this.SingleThreaded)
			{
				buildJob.Run(buildJob.resolution);
			}
			else
			{
				buildJob.Schedule(buildJob.resolution, 1, default(JobHandle)).Complete();
			}
			ObjectInstance[] array = growBuffer.ToArray();
			growBuffer.Dispose();
			GameObject gameObject = null;
			foreach (Transform transform in base.mm.tgt_terrain.gameObject.GetComponentsInChildren<Transform>())
			{
				if (transform.name == "Root Object")
				{
					gameObject = transform.gameObject;
					Transform[] componentsInChildren2 = transform.GetComponentsInChildren<Transform>();
					for (int j = componentsInChildren2.Length - 1; j >= 0; j--)
					{
						if (j > componentsInChildren2.Length - 1)
						{
							j--;
						}
						else if (componentsInChildren2[j] != transform)
						{
							Object.DestroyImmediate(componentsInChildren2[j].gameObject);
						}
					}
					break;
				}
			}
			if (!gameObject)
			{
				gameObject = new GameObject("Root Object");
				gameObject.transform.parent = base.mm.tgt_terrain.gameObject.transform;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
			}
			List<GameObject> objList = this.objects.objList;
			foreach (ObjectInstance objectInstance in array)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(objList[objectInstance.objectIndex], objectInstance.position, objectInstance.rotation, gameObject.transform);
				gameObject2.transform.localScale = objectInstance.scale;
				float y = terrainData.GetInterpolatedHeight(gameObject2.transform.localPosition.x / terrainData.size.x, gameObject2.transform.localPosition.z / terrainData.size.z) + objectInstance.offsetFromGround;
				gameObject2.transform.localPosition = new Vector3(gameObject2.transform.localPosition.x, y, gameObject2.transform.localPosition.z);
			}
		}

		// Token: 0x06003520 RID: 13600 RVA: 0x001AA12C File Offset: 0x001A832C
		public int2 GetResolution(NodePort port)
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				float2 @float = new float2(32f, 32f);
				(int)(terrainData.size.x / @float.x);
			}
			return 0;
		}

		// Token: 0x0400244D RID: 9293
		private const string ROOT_OBJECT_NAME = "Root Object";

		// Token: 0x0400244E RID: 9294
		private const int TILE_SIZE = 32;

		// Token: 0x0400244F RID: 9295
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage objects;

		// Token: 0x04002450 RID: 9296
		public bool SingleThreaded;

		// Token: 0x04002451 RID: 9297
		public static FunctionPointer<GetObjectsCB.AddObjectsFunc> add_object_func;

		// Token: 0x020008DC RID: 2268
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildJob : IJobParallelFor
		{
			// Token: 0x06005217 RID: 21015 RVA: 0x0023FA98 File Offset: 0x0023DC98
			public void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					this.cb.GetObjects(wx, wy, this.tile_size.x, this.tile_size.y, this.add_func, this.add_func_data);
				}
			}

			// Token: 0x04004148 RID: 16712
			public int resolution;

			// Token: 0x04004149 RID: 16713
			public float2 tile_size;

			// Token: 0x0400414A RID: 16714
			public GetObjectsCB cb;

			// Token: 0x0400414B RID: 16715
			public FunctionPointer<GetObjectsCB.AddObjectsFunc> add_func;

			// Token: 0x0400414C RID: 16716
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* add_func_data;
		}

		// Token: 0x020008DD RID: 2269
		public struct AddData
		{
			// Token: 0x06005218 RID: 21016 RVA: 0x0023FB04 File Offset: 0x0023DD04
			public unsafe void Add(ObjectInstance obj)
			{
				GrowBuffer<ObjectInstance> growBuffer = new GrowBuffer<ObjectInstance>((void*)this.buf_data);
				growBuffer.Add(ref obj);
			}

			// Token: 0x0400414D RID: 16717
			[NativeDisableUnsafePtrRestriction]
			public unsafe GrowBufferData* buf_data;
		}
	}
}
