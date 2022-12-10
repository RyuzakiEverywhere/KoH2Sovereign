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
	// Token: 0x020003B2 RID: 946
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Terrain/TerrainCombiner")]
	[Serializable]
	public class TerrainCombinerNode : Node
	{
		// Token: 0x06003590 RID: 13712 RVA: 0x001AD81C File Offset: 0x001ABA1C
		public unsafe override void Initialize()
		{
			this.arr_heightsData = new NativeArray<CombinerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pheightsData = (CombinerNode.Data*)this.arr_heightsData.GetUnsafeReadOnlyPtr<CombinerNode.Data>();
			this.arr_splatsData = new NativeArray<SplatMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.psplatsData = (SplatMixerNode.Data*)this.arr_splatsData.GetUnsafeReadOnlyPtr<SplatMixerNode.Data>();
			this.arr_treesData = new NativeArray<VegetationMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.ptreesData = (VegetationMixerNode.Data*)this.arr_treesData.GetUnsafeReadOnlyPtr<VegetationMixerNode.Data>();
			this.arr_objectsData = new NativeArray<ObjectsMixerNode.Data>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pobjectsData = (ObjectsMixerNode.Data*)this.arr_objectsData.GetUnsafeReadOnlyPtr<ObjectsMixerNode.Data>();
			if (TerrainCombinerNode.get_height_value_funcs == null)
			{
				TerrainCombinerNode.get_height_value_funcs = new IntPtr[8];
				TerrainCombinerNode.get_height_value_funcs[0] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.BlendFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[1] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.AddFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[2] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MinFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[3] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MaxFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[4] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.AverageFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[5] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.SubtractFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[6] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.MultiplyFunc)).Value;
				TerrainCombinerNode.get_height_value_funcs[7] = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(CombinerNode.ModulateFunc)).Value;
			}
			if (TerrainCombinerNode.get_tile_func == IntPtr.Zero)
			{
				TerrainCombinerNode.get_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(SplatMixerNode.GetTile)).Value;
			}
			if (TerrainCombinerNode.get_trees_func == IntPtr.Zero)
			{
				TerrainCombinerNode.get_trees_func = GetTreesCB.GetFuncPtr(new GetTreesCB.GetTreesFunc(VegetationMixerNode.GetTrees)).Value;
			}
			if (TerrainCombinerNode.add_tree_func == IntPtr.Zero)
			{
				TerrainCombinerNode.add_tree_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(VegetationMixerNode.AddTree)).Value;
			}
			if (TerrainCombinerNode.add_tree_with_mask_func == IntPtr.Zero)
			{
				TerrainCombinerNode.add_tree_with_mask_func = GetTreesCB.GetFuncPtr(new GetTreesCB.AddTreeFunc(VegetationMixerNode.AddTreeWithMask)).Value;
			}
			if (TerrainCombinerNode.get_objects_func == IntPtr.Zero)
			{
				TerrainCombinerNode.get_objects_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.GetObjectsFunc(ObjectsMixerNode.GetObjects)).Value;
			}
			if (TerrainCombinerNode.add_object_func == IntPtr.Zero)
			{
				TerrainCombinerNode.add_object_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(ObjectsMixerNode.AddObject)).Value;
			}
			if (TerrainCombinerNode.add_object_with_mask_func == IntPtr.Zero)
			{
				TerrainCombinerNode.add_object_with_mask_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.AddObjectsFunc(ObjectsMixerNode.AddObjectWithMask)).Value;
			}
		}

		// Token: 0x06003591 RID: 13713 RVA: 0x001ADB08 File Offset: 0x001ABD08
		public override void CleanUp()
		{
			if (this.pheightsData != null)
			{
				this.pheightsData = null;
				this.arr_heightsData.Dispose();
				this.isHeightsDataFilled = false;
			}
			if (this.psplatsData != null)
			{
				this.psplatsData = null;
				this.arr_splatsData.Dispose();
				this.isSplatsDataFilled = false;
			}
			if (this.ptreesData != null)
			{
				this.ptreesData = null;
				this.arr_treesData.Dispose();
				this.isTreesDataFilled = false;
			}
			if (this.pobjectsData != null)
			{
				this.pobjectsData = null;
				this.arr_objectsData.Dispose();
				this.game_objects = null;
				this.isObjectsDataFilled = false;
			}
		}

		// Token: 0x06003592 RID: 13714 RVA: 0x001ADBAC File Offset: 0x001ABDAC
		public override object GetValue(NodePort port)
		{
			GetTerrainCB inputValue = base.GetInputValue<GetTerrainCB>("in1", default(GetTerrainCB));
			GetTerrainCB inputValue2 = base.GetInputValue<GetTerrainCB>("in2", default(GetTerrainCB));
			string fieldName = port.fieldName;
			if (fieldName == "terrain")
			{
				GetFloat2DCB getFloat2DCB = this.GetHeights(inputValue.heights, inputValue2.heights);
				GetSplatCB getSplatCB = this.GetSplats(inputValue.splats, inputValue2.splats);
				GetTreesCB getTreesCB = this.GetTrees(inputValue.trees, inputValue2.trees);
				ObjectsPackage objectsPackage = this.GetObjects(inputValue.objects, inputValue2.objects);
				return new GetTerrainCB
				{
					heights = getFloat2DCB,
					splats = getSplatCB,
					trees = getTreesCB,
					objects = objectsPackage
				};
			}
			if (fieldName == "heights")
			{
				return this.GetHeights(inputValue.heights, inputValue2.heights);
			}
			if (fieldName == "splats")
			{
				return this.GetSplats(inputValue.splats, inputValue2.splats);
			}
			if (fieldName == "trees")
			{
				return this.GetTrees(inputValue.trees, inputValue2.trees);
			}
			if (!(fieldName == "objects"))
			{
				return null;
			}
			return this.GetObjects(inputValue.objects, inputValue2.objects);
		}

		// Token: 0x06003593 RID: 13715 RVA: 0x001ADD20 File Offset: 0x001ABF20
		private unsafe GetFloat2DCB GetHeights(GetFloat2DCB heights1, GetFloat2DCB heights2)
		{
			GetFloat2DCB result;
			if (this.combineHeights)
			{
				if (this.pheightsData != null && !this.isHeightsDataFilled)
				{
					this.FillHeightsCombinerData(heights1, heights2);
				}
				result = new GetFloat2DCB
				{
					get_value_func = TerrainCombinerNode.get_height_value_funcs[(int)this.heightOperation],
					data = (void*)this.pheightsData
				};
			}
			else
			{
				result = heights1;
			}
			return result;
		}

		// Token: 0x06003594 RID: 13716 RVA: 0x001ADD80 File Offset: 0x001ABF80
		private unsafe GetSplatCB GetSplats(GetSplatCB splats1, GetSplatCB splats2)
		{
			GetSplatCB result;
			if (this.combineSplats)
			{
				if (this.psplatsData != null && !this.isSplatsDataFilled)
				{
					this.FillSplatsCombinerData(splats1, splats2);
				}
				result = new GetSplatCB
				{
					get_tile_func = TerrainCombinerNode.get_tile_func,
					data = (void*)this.psplatsData
				};
			}
			else
			{
				result = splats1;
			}
			return result;
		}

		// Token: 0x06003595 RID: 13717 RVA: 0x001ADDD8 File Offset: 0x001ABFD8
		private unsafe GetTreesCB GetTrees(GetTreesCB trees1, GetTreesCB trees2)
		{
			GetTreesCB result;
			if (this.combineTrees)
			{
				if (this.ptreesData != null && !this.isTreesDataFilled)
				{
					this.FillTreesCombinerData(trees1, trees2);
				}
				result = new GetTreesCB
				{
					get_trees_func = TerrainCombinerNode.get_trees_func,
					data = (void*)this.ptreesData
				};
			}
			else
			{
				result = trees1;
			}
			return result;
		}

		// Token: 0x06003596 RID: 13718 RVA: 0x001ADE30 File Offset: 0x001AC030
		private unsafe ObjectsPackage GetObjects(ObjectsPackage objects1, ObjectsPackage objects2)
		{
			ObjectsPackage result;
			if (this.combineObjects)
			{
				if (this.pobjectsData != null && !this.isObjectsDataFilled)
				{
					this.FillObjectsCombinerData(objects1, objects2);
				}
				result = new ObjectsPackage
				{
					objects = new GetObjectsCB
					{
						get_objects_func = TerrainCombinerNode.get_objects_func,
						data = (void*)this.pobjectsData
					},
					objList = this.game_objects
				};
			}
			else
			{
				result = objects1;
			}
			return result;
		}

		// Token: 0x06003597 RID: 13719 RVA: 0x001ADEA4 File Offset: 0x001AC0A4
		private unsafe void FillHeightsCombinerData(GetFloat2DCB heights1, GetFloat2DCB heights2)
		{
			CombinerNode.Data* ptr = this.pheightsData;
			ptr->in1 = heights1;
			ptr->in2 = heights2;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->strength = this.strength;
			ptr->range = new float2(this.min, this.max);
			this.isHeightsDataFilled = true;
		}

		// Token: 0x06003598 RID: 13720 RVA: 0x001ADF08 File Offset: 0x001AC108
		private unsafe void FillSplatsCombinerData(GetSplatCB splats1, GetSplatCB splats2)
		{
			SplatMixerNode.Data* ptr = this.psplatsData;
			ptr->in1 = splats1;
			ptr->in2 = splats2;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->strength = this.strength;
			this.isSplatsDataFilled = true;
		}

		// Token: 0x06003599 RID: 13721 RVA: 0x001ADF58 File Offset: 0x001AC158
		private unsafe void FillTreesCombinerData(GetTreesCB trees1, GetTreesCB trees2)
		{
			VegetationMixerNode.Data* ptr = this.ptreesData;
			ptr->in1 = trees1;
			ptr->in2 = trees2;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.treesCutout;
			ptr->add_tree_func = TerrainCombinerNode.add_tree_func;
			ptr->add_tree_with_mask_func = TerrainCombinerNode.add_tree_with_mask_func;
			this.isTreesDataFilled = true;
		}

		// Token: 0x0600359A RID: 13722 RVA: 0x001ADFBC File Offset: 0x001AC1BC
		private unsafe void FillObjectsCombinerData(ObjectsPackage objects1, ObjectsPackage objects2)
		{
			ObjectsMixerNode.Data* ptr = this.pobjectsData;
			ptr->in1 = objects1.objects;
			ptr->in2 = objects2.objects;
			ptr->mask = base.GetInputValue<GetFloat2DCB>("mask", default(GetFloat2DCB));
			ptr->cutout = this.objectsCutout;
			ptr->add_object_func = TerrainCombinerNode.add_object_func;
			ptr->add_object_with_mask_func = TerrainCombinerNode.add_object_with_mask_func;
			ptr->indexOffset = objects1.objList.Count;
			this.game_objects = new List<GameObject>(objects1.objList);
			this.game_objects.AddRange(objects2.objList);
			this.isObjectsDataFilled = true;
		}

		// Token: 0x0600359B RID: 13723 RVA: 0x001AE05C File Offset: 0x001AC25C
		public override void PreCache()
		{
			if (this.heightOperation == CombineOperationType.Modulate)
			{
				this.CalculateHeightRangeFloat2DCB();
			}
		}

		// Token: 0x0600359C RID: 13724 RVA: 0x001AE070 File Offset: 0x001AC270
		private void CalculateHeightRangeFloat2DCB()
		{
			Terrain tgt_terrain = base.mm.tgt_terrain;
			TerrainData terrainData = (tgt_terrain != null) ? tgt_terrain.terrainData : null;
			if (terrainData == null)
			{
				return;
			}
			int heightmapResolution = terrainData.heightmapResolution;
			GetFloat2DCB getFloat2DCB = base.GetInputValue<GetTerrainCB>("in2", default(GetTerrainCB)).heights;
			if (getFloat2DCB.data == null)
			{
				return;
			}
			using (NativeQueue<float> mins = new NativeQueue<float>(Allocator.TempJob))
			{
				using (NativeQueue<float> maxs = new NativeQueue<float>(Allocator.TempJob))
				{
					using (NativeArray<float> nativeArray = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
					{
						using (NativeArray<float> nativeArray2 = new NativeArray<float>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory))
						{
							CombinerNode.RangeJob jobData = default(CombinerNode.RangeJob);
							jobData.resolution = heightmapResolution;
							jobData.Min = mins.AsParallelWriter();
							jobData.Max = maxs.AsParallelWriter();
							jobData.input = getFloat2DCB;
							if (this.SingleThreaded)
							{
								jobData.Run(heightmapResolution);
							}
							else
							{
								jobData.Schedule(heightmapResolution, 16, default(JobHandle)).Complete();
							}
							CombinerNode.RangeCombineJob jobData2 = new CombinerNode.RangeCombineJob
							{
								Mins = mins,
								Maxs = maxs,
								Min = nativeArray,
								Max = nativeArray2
							};
							jobData2.Schedule(default(JobHandle)).Complete();
							this.min = jobData2.Min[0];
							this.max = jobData2.Max[0];
						}
					}
				}
			}
		}

		// Token: 0x040024E9 RID: 9449
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTerrainCB in1;

		// Token: 0x040024EA RID: 9450
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetTerrainCB in2;

		// Token: 0x040024EB RID: 9451
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public GetFloat2DCB mask;

		// Token: 0x040024EC RID: 9452
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTerrainCB terrain;

		// Token: 0x040024ED RID: 9453
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB heights;

		// Token: 0x040024EE RID: 9454
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB splats;

		// Token: 0x040024EF RID: 9455
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB trees;

		// Token: 0x040024F0 RID: 9456
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage objects;

		// Token: 0x040024F1 RID: 9457
		public bool combineHeights;

		// Token: 0x040024F2 RID: 9458
		public bool combineSplats;

		// Token: 0x040024F3 RID: 9459
		public bool combineTrees;

		// Token: 0x040024F4 RID: 9460
		public bool combineObjects;

		// Token: 0x040024F5 RID: 9461
		public float strength;

		// Token: 0x040024F6 RID: 9462
		public CombineOperationType heightOperation;

		// Token: 0x040024F7 RID: 9463
		public float2 treesCutout;

		// Token: 0x040024F8 RID: 9464
		public float2 objectsCutout;

		// Token: 0x040024F9 RID: 9465
		public bool SingleThreaded;

		// Token: 0x040024FA RID: 9466
		private float min = float.MaxValue;

		// Token: 0x040024FB RID: 9467
		private float max = float.MinValue;

		// Token: 0x040024FC RID: 9468
		private List<GameObject> game_objects;

		// Token: 0x040024FD RID: 9469
		private bool isHeightsDataFilled;

		// Token: 0x040024FE RID: 9470
		private bool isSplatsDataFilled;

		// Token: 0x040024FF RID: 9471
		private bool isTreesDataFilled;

		// Token: 0x04002500 RID: 9472
		private bool isObjectsDataFilled;

		// Token: 0x04002501 RID: 9473
		public static IntPtr[] get_height_value_funcs;

		// Token: 0x04002502 RID: 9474
		public NativeArray<CombinerNode.Data> arr_heightsData;

		// Token: 0x04002503 RID: 9475
		public unsafe CombinerNode.Data* pheightsData;

		// Token: 0x04002504 RID: 9476
		public static IntPtr get_tile_func;

		// Token: 0x04002505 RID: 9477
		public NativeArray<SplatMixerNode.Data> arr_splatsData;

		// Token: 0x04002506 RID: 9478
		public unsafe SplatMixerNode.Data* psplatsData;

		// Token: 0x04002507 RID: 9479
		public static IntPtr get_trees_func;

		// Token: 0x04002508 RID: 9480
		public static IntPtr add_tree_func;

		// Token: 0x04002509 RID: 9481
		public static IntPtr add_tree_with_mask_func;

		// Token: 0x0400250A RID: 9482
		public NativeArray<VegetationMixerNode.Data> arr_treesData;

		// Token: 0x0400250B RID: 9483
		public unsafe VegetationMixerNode.Data* ptreesData;

		// Token: 0x0400250C RID: 9484
		public static IntPtr get_objects_func;

		// Token: 0x0400250D RID: 9485
		public static IntPtr add_object_func;

		// Token: 0x0400250E RID: 9486
		public static IntPtr add_object_with_mask_func;

		// Token: 0x0400250F RID: 9487
		public NativeArray<ObjectsMixerNode.Data> arr_objectsData;

		// Token: 0x04002510 RID: 9488
		public unsafe ObjectsMixerNode.Data* pobjectsData;

		// Token: 0x020008F9 RID: 2297
		public struct Data
		{
			// Token: 0x040041CC RID: 16844
			public GetTerrainCB in1;

			// Token: 0x040041CD RID: 16845
			public GetTerrainCB in2;

			// Token: 0x040041CE RID: 16846
			public GetFloat2DCB mask;

			// Token: 0x040041CF RID: 16847
			public float strength;
		}
	}
}
