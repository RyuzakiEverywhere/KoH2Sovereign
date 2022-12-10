using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using XNode;

namespace BSG.MapMaker.Nodes
{
	// Token: 0x0200039E RID: 926
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Terrain/OutputTerrain")]
	[Serializable]
	public class OutputTerrainNode : Node, IOutputNode
	{
		// Token: 0x0600352A RID: 13610 RVA: 0x000448AF File Offset: 0x00042AAF
		public override object GetValue(NodePort port)
		{
			return null;
		}

		// Token: 0x0600352B RID: 13611 RVA: 0x001AA550 File Offset: 0x001A8750
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddTree(ref TreeInstance tree, void* data)
		{
			if (data == null)
			{
				return;
			}
			((OutputTerrainNode.AddTreeData*)data)->Add(tree);
		}

		// Token: 0x0600352C RID: 13612 RVA: 0x001AA564 File Offset: 0x001A8764
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void AddObject(ref ObjectInstance obj, void* data)
		{
			if (data == null)
			{
				return;
			}
			((OutputTerrainNode.AddObjectData*)data)->Add(obj);
		}

		// Token: 0x0600352D RID: 13613 RVA: 0x001AA578 File Offset: 0x001A8778
		public override void Build()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			this.terrain = base.GetInputValue<GetTerrainCB>("terrain", default(GetTerrainCB));
			if (this.terrain.heights.data != null && this.terrain.heights.data != null)
			{
				this.heights = this.terrain.heights;
				this.splats = this.terrain.splats;
				this.trees = this.terrain.trees;
				this.objects = this.terrain.objects;
			}
			else
			{
				this.heights = base.GetInputValue<GetFloat2DCB>("heights", default(GetFloat2DCB));
				this.splats = base.GetInputValue<GetSplatCB>("splats", default(GetSplatCB));
				this.details = base.GetInputValue<GetDetailsCB>("details", default(GetDetailsCB));
				this.trees = base.GetInputValue<GetTreesCB>("trees", default(GetTreesCB));
				this.objects = base.GetInputValue<ObjectsPackage>("objects", default(ObjectsPackage));
			}
			this.BuildHeights(terrainData, this.heights);
			this.BuildSplats(terrainData, this.splats);
			this.BuildDetails(terrainData, this.details);
			this.BuildTrees(terrainData, this.trees);
			this.BuildObjects(terrainData, this.objects);
		}

		// Token: 0x0600352E RID: 13614 RVA: 0x001AA6F8 File Offset: 0x001A88F8
		private unsafe void BuildHeights(TerrainData td, GetFloat2DCB heights)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (heights.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			int heightmapResolution = td.heightmapResolution;
			float[,] array = new float[heightmapResolution, heightmapResolution];
			fixed (float* ptr = &array[0, 0])
			{
				float* pheights = ptr;
				OutputTerrainNode.BuildHeightsJob jobData = default(OutputTerrainNode.BuildHeightsJob);
				jobData.resolution = heightmapResolution;
				jobData.tile_size = new float2(td.size.x, td.size.z) / (float)(heightmapResolution - 1);
				jobData.pheights = pheights;
				jobData.cb = heights;
				if (this.SingleThreaded)
				{
					jobData.Run(heightmapResolution);
				}
				else
				{
					jobData.Schedule(heightmapResolution, 16, default(JobHandle)).Complete();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			td.SetHeights(0, 0, array);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Heights initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x0600352F RID: 13615 RVA: 0x001AA804 File Offset: 0x001A8A04
		private unsafe void BuildSplats(TerrainData td, GetSplatCB splats)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (splats.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			int alphamapResolution = td.alphamapResolution;
			int alphamapLayers = td.alphamapLayers;
			float[,,] array = new float[alphamapResolution, alphamapResolution, alphamapLayers];
			fixed (float* ptr = &array[0, 0, 0])
			{
				float* psplats = ptr;
				OutputTerrainNode.BuildSplatsJob jobData = default(OutputTerrainNode.BuildSplatsJob);
				jobData.resolution = alphamapResolution;
				jobData.tile_size = new float2(td.size.x, td.size.z) / (float)alphamapResolution;
				jobData.layers = alphamapLayers;
				jobData.psplats = psplats;
				jobData.cb = splats;
				if (this.SingleThreaded)
				{
					jobData.Run(alphamapResolution);
				}
				else
				{
					jobData.Schedule(alphamapResolution, 16, default(JobHandle)).Complete();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			td.SetAlphamaps(0, 0, array);
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Splats initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x06003530 RID: 13616 RVA: 0x001AA920 File Offset: 0x001A8B20
		private unsafe void BuildDetails(TerrainData td, GetDetailsCB details)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (this.splats.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			int resolution = td.detailResolution;
			int detail_prototype_count = td.detailPrototypes.Length;
			float[,,] detail_densities_normalized = new float[resolution, resolution, detail_prototype_count];
			float2 tile_size = new float2(td.size.x, td.size.z) / (float)resolution;
			Parallel.For(0, resolution, delegate(int y)
			{
				float[] array2 = new float[detail_prototype_count];
				float y2 = (float)y * tile_size.y;
				for (int l = 0; l < resolution; l++)
				{
					float x = (float)l * tile_size.x;
					float[] array3;
					float* ptr;
					if ((array3 = array2) == null || array3.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array3[0];
					}
					details.GetDetailDensitiesFunction(new float2(x, y2), ptr, array2.Length);
					for (int m = 0; m < detail_prototype_count; m++)
					{
						detail_densities_normalized[y, l, m] = ptr[m];
					}
					array3 = null;
				}
			});
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			for (int i = 0; i < detail_prototype_count; i++)
			{
				int[,] array = new int[resolution, resolution];
				for (int j = 0; j < resolution; j++)
				{
					for (int k = 0; k < resolution; k++)
					{
						array[j, k] = Mathf.FloorToInt(detail_densities_normalized[j, k, i] * 16f);
					}
				}
				td.SetDetailLayer(0, 0, i, array);
			}
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Details initted for {0}ms, calculated for {1}ms, applied for {2}ms", elapsedMilliseconds, elapsedMilliseconds2, elapsedMilliseconds3));
		}

		// Token: 0x06003531 RID: 13617 RVA: 0x001AAA98 File Offset: 0x001A8C98
		private unsafe void BuildTrees(TerrainData td, GetTreesCB trees)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (trees.data == null)
			{
				return;
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			if (!OutputTerrainNode.add_tree_func.IsCreated)
			{
				OutputTerrainNode.add_tree_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(OutputTerrainNode.AddTree));
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			GrowBuffer<TreeInstance> growBuffer = new GrowBuffer<TreeInstance>(Allocator.TempJob, 65536);
			OutputTerrainNode.BuildTreesJob buildTreesJob = default(OutputTerrainNode.BuildTreesJob);
			buildTreesJob.tile_size = new float2(32f, 32f);
			buildTreesJob.resolution = (int)(td.size.x / buildTreesJob.tile_size.x);
			buildTreesJob.cb = trees;
			buildTreesJob.add_func = OutputTerrainNode.add_tree_func;
			OutputTerrainNode.AddTreeData addTreeData = default(OutputTerrainNode.AddTreeData);
			addTreeData.buf_data = growBuffer.data;
			addTreeData.inv_size_x = 1f / td.size.x;
			addTreeData.inv_size_z = 1f / td.size.z;
			buildTreesJob.add_func_data = (void*)(&addTreeData);
			if (this.SingleThreaded)
			{
				buildTreesJob.Run(buildTreesJob.resolution);
			}
			else
			{
				buildTreesJob.Schedule(buildTreesJob.resolution, 1, default(JobHandle)).Complete();
			}
			TreeInstance[] array = growBuffer.ToArray();
			growBuffer.Dispose();
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			TreesBatching component = base.mm.tgt_terrain.GetComponent<TreesBatching>();
			if (component != null && component.isActiveAndEnabled && component.is_enabled)
			{
				component.do_rebuild = true;
			}
			td.SetTreeInstances(array, true);
			long elapsedMilliseconds4 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Trees: {0}, initted for {1}ms, calculated for {2}ms, applied for {3}ms", new object[]
			{
				array.Length,
				elapsedMilliseconds,
				elapsedMilliseconds3,
				elapsedMilliseconds4
			}));
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x001AAC78 File Offset: 0x001A8E78
		public unsafe void BuildObjects(TerrainData td, ObjectsPackage objects)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			if (objects.objects.data == null)
			{
				return;
			}
			if (!OutputTerrainNode.add_object_func.IsCreated)
			{
				OutputTerrainNode.add_object_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(OutputTerrainNode.AddObject));
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			GrowBuffer<ObjectInstance> growBuffer = new GrowBuffer<ObjectInstance>(Allocator.TempJob, 65536);
			OutputTerrainNode.BuildObjectsJob buildObjectsJob = default(OutputTerrainNode.BuildObjectsJob);
			buildObjectsJob.tile_size = new float2(32f, 32f);
			buildObjectsJob.resolution = (int)(td.size.x / buildObjectsJob.tile_size.x);
			buildObjectsJob.cb = objects.objects;
			buildObjectsJob.add_func = OutputTerrainNode.add_object_func;
			OutputTerrainNode.AddObjectData addObjectData = default(OutputTerrainNode.AddObjectData);
			addObjectData.buf_data = growBuffer.data;
			buildObjectsJob.add_func_data = (void*)(&addObjectData);
			if (this.SingleThreaded)
			{
				buildObjectsJob.Run(buildObjectsJob.resolution);
			}
			else
			{
				buildObjectsJob.Schedule(buildObjectsJob.resolution, 1, default(JobHandle)).Complete();
			}
			ObjectInstance[] array = growBuffer.ToArray();
			growBuffer.Dispose();
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
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
			List<GameObject> objList = objects.objList;
			foreach (ObjectInstance objectInstance in array)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(objList[objectInstance.objectIndex], objectInstance.position, objectInstance.rotation, gameObject.transform);
				gameObject2.transform.localScale = objectInstance.scale;
				float y = td.GetInterpolatedHeight(gameObject2.transform.localPosition.x / td.size.x, gameObject2.transform.localPosition.z / td.size.z) + objectInstance.offsetFromGround;
				gameObject2.transform.localPosition = new Vector3(gameObject2.transform.localPosition.x, y, gameObject2.transform.localPosition.z);
			}
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			Debug.Log(string.Format("Objects: {0}, initted for {1}ms, calculated for {2}ms, applied for {3}ms", new object[]
			{
				array.Length,
				elapsedMilliseconds,
				elapsedMilliseconds2,
				elapsedMilliseconds3
			}));
		}

		// Token: 0x06003533 RID: 13619 RVA: 0x001AAFDC File Offset: 0x001A91DC
		public int2 GetResolution(NodePort port)
		{
			int2 result = 0;
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData != null)
			{
				string fieldName = port.fieldName;
				if (fieldName == "terrain" || fieldName == "heights")
				{
					return terrainData.heightmapResolution;
				}
				if (fieldName == "splats")
				{
					return terrainData.alphamapResolution;
				}
				if (fieldName == "trees" || fieldName == "objects")
				{
					float2 @float = new float2(32f, 32f);
					return (int)(terrainData.size.x / @float.x);
				}
			}
			return result;
		}

		// Token: 0x04002458 RID: 9304
		private const string ROOT_OBJECT_NAME = "Root Object";

		// Token: 0x04002459 RID: 9305
		private const int TILE_SIZE = 32;

		// Token: 0x0400245A RID: 9306
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB heights;

		// Token: 0x0400245B RID: 9307
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetSplatCB splats;

		// Token: 0x0400245C RID: 9308
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetDetailsCB details;

		// Token: 0x0400245D RID: 9309
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTreesCB trees;

		// Token: 0x0400245E RID: 9310
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public ObjectsPackage objects;

		// Token: 0x0400245F RID: 9311
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTerrainCB terrain;

		// Token: 0x04002460 RID: 9312
		public bool SingleThreaded;

		// Token: 0x04002461 RID: 9313
		public static FunctionPointer<GetTreesCB.AddTreeFunc> add_tree_func;

		// Token: 0x04002462 RID: 9314
		public static FunctionPointer<GetObjectsCB.AddObjectsFunc> add_object_func;

		// Token: 0x020008E0 RID: 2272
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildHeightsJob : IJobParallelFor
		{
			// Token: 0x0600521B RID: 21019 RVA: 0x0023FC80 File Offset: 0x0023DE80
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					float value = this.cb.GetValue(wx, wy);
					this.pheights[y * this.resolution + i] = value;
				}
			}

			// Token: 0x04004157 RID: 16727
			public int resolution;

			// Token: 0x04004158 RID: 16728
			public float2 tile_size;

			// Token: 0x04004159 RID: 16729
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* pheights;

			// Token: 0x0400415A RID: 16730
			public GetFloat2DCB cb;
		}

		// Token: 0x020008E1 RID: 2273
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildSplatsJob : IJobParallelFor
		{
			// Token: 0x0600521C RID: 21020 RVA: 0x0023FCE0 File Offset: 0x0023DEE0
			public unsafe void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					float* ptr = this.psplats + y * this.resolution * this.layers + i * this.layers;
					float num = 0f;
					this.cb.GetTile(wx, wy, ptr, this.layers);
					for (int j = 0; j < this.layers; j++)
					{
						float num2 = ptr[j];
						num += num2;
					}
					if (num != 1f)
					{
						if (num == 0f)
						{
							*ptr = 1f;
						}
						else
						{
							float num3 = 1f / num;
							for (int k = 0; k < this.layers; k++)
							{
								ptr[k] *= num3;
							}
						}
					}
				}
			}

			// Token: 0x0400415B RID: 16731
			public int resolution;

			// Token: 0x0400415C RID: 16732
			public float2 tile_size;

			// Token: 0x0400415D RID: 16733
			public int layers;

			// Token: 0x0400415E RID: 16734
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* psplats;

			// Token: 0x0400415F RID: 16735
			public GetSplatCB cb;
		}

		// Token: 0x020008E2 RID: 2274
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildTreesJob : IJobParallelFor
		{
			// Token: 0x0600521D RID: 21021 RVA: 0x0023FDD0 File Offset: 0x0023DFD0
			public void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					this.cb.GetTrees(wx, wy, this.tile_size.x, this.tile_size.y, this.add_func, this.add_func_data);
				}
			}

			// Token: 0x04004160 RID: 16736
			public int resolution;

			// Token: 0x04004161 RID: 16737
			public float2 tile_size;

			// Token: 0x04004162 RID: 16738
			public GetTreesCB cb;

			// Token: 0x04004163 RID: 16739
			public FunctionPointer<GetTreesCB.AddTreeFunc> add_func;

			// Token: 0x04004164 RID: 16740
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* add_func_data;
		}

		// Token: 0x020008E3 RID: 2275
		public struct AddTreeData
		{
			// Token: 0x0600521E RID: 21022 RVA: 0x0023FE3C File Offset: 0x0023E03C
			public unsafe void Add(TreeInstance tree)
			{
				tree.position.x = tree.position.x * this.inv_size_x;
				tree.position.z = tree.position.z * this.inv_size_z;
				if (tree.position.x > 1f || tree.position.x < 0f || tree.position.z > 1f || tree.position.z < 0f)
				{
					return;
				}
				GrowBuffer<TreeInstance> growBuffer = new GrowBuffer<TreeInstance>((void*)this.buf_data);
				growBuffer.Add(ref tree);
			}

			// Token: 0x04004165 RID: 16741
			[NativeDisableUnsafePtrRestriction]
			public unsafe GrowBufferData* buf_data;

			// Token: 0x04004166 RID: 16742
			public float inv_size_x;

			// Token: 0x04004167 RID: 16743
			public float inv_size_z;
		}

		// Token: 0x020008E4 RID: 2276
		[BurstCompile(CompileSynchronously = true)]
		private struct BuildObjectsJob : IJobParallelFor
		{
			// Token: 0x0600521F RID: 21023 RVA: 0x0023FED4 File Offset: 0x0023E0D4
			public void Execute(int y)
			{
				float wy = (float)y * this.tile_size.y;
				for (int i = 0; i < this.resolution; i++)
				{
					float wx = (float)i * this.tile_size.x;
					this.cb.GetObjects(wx, wy, this.tile_size.x, this.tile_size.y, this.add_func, this.add_func_data);
				}
			}

			// Token: 0x04004168 RID: 16744
			public int resolution;

			// Token: 0x04004169 RID: 16745
			public float2 tile_size;

			// Token: 0x0400416A RID: 16746
			public GetObjectsCB cb;

			// Token: 0x0400416B RID: 16747
			public FunctionPointer<GetObjectsCB.AddObjectsFunc> add_func;

			// Token: 0x0400416C RID: 16748
			[NativeDisableUnsafePtrRestriction]
			public unsafe void* add_func_data;
		}

		// Token: 0x020008E5 RID: 2277
		public struct AddObjectData
		{
			// Token: 0x06005220 RID: 21024 RVA: 0x0023FF40 File Offset: 0x0023E140
			public unsafe void Add(ObjectInstance obj)
			{
				GrowBuffer<ObjectInstance> growBuffer = new GrowBuffer<ObjectInstance>((void*)this.buf_data);
				growBuffer.Add(ref obj);
			}

			// Token: 0x0400416D RID: 16749
			[NativeDisableUnsafePtrRestriction]
			public unsafe GrowBufferData* buf_data;
		}
	}
}
