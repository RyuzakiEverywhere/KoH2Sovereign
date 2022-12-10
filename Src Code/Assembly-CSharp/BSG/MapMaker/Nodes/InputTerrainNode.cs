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
	// Token: 0x02000391 RID: 913
	[BurstCompile(CompileSynchronously = true)]
	[Node.CreateNodeMenuAttribute("BSG Nodes/Terrain/InputTerrain")]
	[Serializable]
	public class InputTerrainNode : Node
	{
		// Token: 0x060034D0 RID: 13520 RVA: 0x001A695C File Offset: 0x001A4B5C
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetHeight(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTerrainNode.HeightsData ptr = ref *(InputTerrainNode.HeightsData*)data;
			return ptr.grid.GetValue(ptr.heights, 1, wx, wy, 0, ptr.filter_type);
		}

		// Token: 0x060034D1 RID: 13521 RVA: 0x001A6994 File Offset: 0x001A4B94
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetSplat(float wx, float wy, int l, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTerrainNode.SplatsData ptr = ref *(InputTerrainNode.SplatsData*)data;
			if (l >= ptr.remap_count)
			{
				return 0f;
			}
			l = ptr.remap[l];
			if (l < 0)
			{
				return 0f;
			}
			ptr.grid.WorldToLocal(new float2(wx, wy));
			return ptr.grid.GetValue(ptr.alphas, ptr.layers, wx, wy, l, ptr.filter_type);
		}

		// Token: 0x060034D2 RID: 13522 RVA: 0x001A6A08 File Offset: 0x001A4C08
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetSplatTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref InputTerrainNode.SplatsData ptr = ref *(InputTerrainNode.SplatsData*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int2 @int = new int2(@float);
			float2 float2 = @float - @int;
			FilterType filterType = (ptr.filter_type != FilterType.None && math.any(float2)) ? ptr.filter_type : FilterType.None;
			for (int i = 0; i < layers; i++)
			{
				if (i >= ptr.remap_count)
				{
					result[i] = 0f;
				}
				else
				{
					int num = ptr.remap[i];
					if (num < 0)
					{
						result[i] = 0f;
					}
					else
					{
						result[i] = ptr.grid.GetLocal(ptr.alphas, ptr.layers, @int, float2, num, filterType);
					}
				}
			}
		}

		// Token: 0x060034D3 RID: 13523 RVA: 0x001A6AD3 File Offset: 0x001A4CD3
		private unsafe void GetDetailDensities(float2 worldSpacePos, float* output_prototype_densities, int output_prototype_densities_length)
		{
			this.detailsData.GetDetailDensities(worldSpacePos, output_prototype_densities, output_prototype_densities_length);
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x001A6AE4 File Offset: 0x001A4CE4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetAlphaLayerValue(float wx, float wy, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTerrainNode.SplatsData ptr = ref *(InputTerrainNode.SplatsData*)data;
			if (ptr.alphaLayerIndex >= 0)
			{
				return 1f - ptr.grid.GetValue(ptr.alphas, ptr.layers, wx, wy, ptr.alphaLayerIndex, ptr.filter_type);
			}
			return 1f;
		}

		// Token: 0x060034D5 RID: 13525 RVA: 0x001A6B38 File Offset: 0x001A4D38
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static float GetNonAlphaLayersSplat(float wx, float wy, int l, void* data)
		{
			if (data == null)
			{
				return 0f;
			}
			ref InputTerrainNode.SplatsData ptr = ref *(InputTerrainNode.SplatsData*)data;
			if (l >= ptr.remap_count)
			{
				return 0f;
			}
			l = ptr.remap[l];
			if (l < 0)
			{
				return 0f;
			}
			if (l == ptr.alphaLayerIndex)
			{
				return 0f;
			}
			float num = ptr.grid.GetValue(ptr.alphas, ptr.layers, wx, wy, l, ptr.filter_type);
			if (ptr.alphaLayerIndex < 0 && ptr.normalizeNonAlphaLayers)
			{
				float value = ptr.grid.GetValue(ptr.alphas, ptr.layers, wx, wy, ptr.alphaLayerIndex, ptr.filter_type);
				if (value < 1f)
				{
					num /= 1f - value;
				}
				else
				{
					num = 0f;
				}
			}
			return num;
		}

		// Token: 0x060034D6 RID: 13526 RVA: 0x001A6BFC File Offset: 0x001A4DFC
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetNonAlphaLayersTile(float wx, float wy, float* result, int layers, void* data)
		{
			if (data == null)
			{
				return;
			}
			ref InputTerrainNode.SplatsData ptr = ref *(InputTerrainNode.SplatsData*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int2 @int = new int2(@float);
			float2 float2 = @float - @int;
			FilterType filterType = (ptr.filter_type != FilterType.None && math.any(float2)) ? ptr.filter_type : FilterType.None;
			for (int i = 0; i < layers; i++)
			{
				if (i >= ptr.remap_count)
				{
					result[i] = 0f;
				}
				else
				{
					int num = ptr.remap[i];
					if (num < 0)
					{
						result[i] = 0f;
					}
					else if (num == ptr.alphaLayerIndex)
					{
						result[i] = 0f;
					}
					else if (ptr.alphaLayerIndex >= 0 && ptr.normalizeNonAlphaLayers)
					{
						float local = ptr.grid.GetLocal(ptr.alphas, ptr.layers, @int, float2, ptr.alphaLayerIndex, filterType);
						if (local >= 1f)
						{
							result[i] = 0f;
						}
						else
						{
							float local2 = ptr.grid.GetLocal(ptr.alphas, ptr.layers, @int, float2, num, filterType);
							result[i] = local2 / (1f - local);
						}
					}
					else
					{
						result[i] = ptr.grid.GetLocal(ptr.alphas, ptr.layers, @int, float2, num, filterType);
					}
				}
			}
		}

		// Token: 0x060034D7 RID: 13527 RVA: 0x001A6D70 File Offset: 0x001A4F70
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetTrees(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetTreesCB.AddTreeFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref InputTerrainNode.TreesData ptr = ref *(InputTerrainNode.TreesData*)data;
			float2 @float = ptr.grid.WorldToLocal(new float2(wx, wy));
			int cx = (int)@float.x;
			int cy = (int)@float.y;
			float num = (float)((int)(wx * ptr.grid.inv_cell_size.x)) * ptr.grid.cell_size.x;
			float num2 = (float)((int)(wy * ptr.grid.inv_cell_size.y)) * ptr.grid.cell_size.y;
			GrowBuffer<TreeInstance> cell = ptr.GetCell(cx, cy);
			int count = cell.Count;
			for (int i = 0; i < count; i++)
			{
				TreeInstance treeInstance = *cell[i];
				treeInstance.position.x = treeInstance.position.x + num;
				treeInstance.position.z = treeInstance.position.z + num2;
				treeInstance.position.x = treeInstance.position.x * ptr.grid.scale.x;
				treeInstance.position.z = treeInstance.position.z * ptr.grid.scale.y;
				cb.Invoke(ref treeInstance, cb_data);
			}
		}

		// Token: 0x060034D8 RID: 13528 RVA: 0x001A6EA4 File Offset: 0x001A50A4
		[BurstCompile(CompileSynchronously = true)]
		public unsafe static void GetObjects(float wx, float wy, float ww, float wh, void* data, FunctionPointer<GetObjectsCB.AddObjectsFunc> cb, void* cb_data)
		{
			if (data == null)
			{
				return;
			}
			ref InputTerrainNode.ObjectsData ptr = ref *(InputTerrainNode.ObjectsData*)data;
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

		// Token: 0x060034D9 RID: 13529 RVA: 0x001A6F90 File Offset: 0x001A5190
		public unsafe override void Initialize()
		{
			this.arr_heightsData = new NativeArray<InputTerrainNode.HeightsData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pheightsData = (InputTerrainNode.HeightsData*)this.arr_heightsData.GetUnsafeReadOnlyPtr<InputTerrainNode.HeightsData>();
			this.arr_splatsData = new NativeArray<InputTerrainNode.SplatsData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.psplatsData = (InputTerrainNode.SplatsData*)this.arr_splatsData.GetUnsafeReadOnlyPtr<InputTerrainNode.SplatsData>();
			this.arr_treesData = new NativeArray<InputTerrainNode.TreesData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.ptreesData = (InputTerrainNode.TreesData*)this.arr_treesData.GetUnsafeReadOnlyPtr<InputTerrainNode.TreesData>();
			this.arr_objectsData = new NativeArray<InputTerrainNode.ObjectsData>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			this.pobjectsData = (InputTerrainNode.ObjectsData*)this.arr_objectsData.GetUnsafeReadOnlyPtr<InputTerrainNode.ObjectsData>();
			if (InputTerrainNode.get_heights_func == IntPtr.Zero)
			{
				InputTerrainNode.get_heights_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTerrainNode.GetHeight)).Value;
			}
			if (InputTerrainNode.get_splats_tile_func == IntPtr.Zero)
			{
				InputTerrainNode.get_splats_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(InputTerrainNode.GetSplatTile)).Value;
			}
			if (InputTerrainNode.get_alpha_layer_func == IntPtr.Zero)
			{
				InputTerrainNode.get_alpha_layer_func = GetFloat2DCB.GetFuncPtr(new GetFloat2DCB.GetValueFunc(InputTerrainNode.GetAlphaLayerValue)).Value;
			}
			if (InputTerrainNode.get_non_alpha_tile_func == IntPtr.Zero)
			{
				InputTerrainNode.get_non_alpha_tile_func = GetSplatCB.GetFuncPtr(new GetSplatCB.GetTileFunc(InputTerrainNode.GetNonAlphaLayersTile)).Value;
			}
			if (InputTerrainNode.get_trees_func == IntPtr.Zero)
			{
				InputTerrainNode.get_trees_func = GetTreesCB.GetFuncPtr(new GetTreesCB.GetTreesFunc(InputTerrainNode.GetTrees)).Value;
			}
			if (InputTerrainNode.get_objects_func == IntPtr.Zero)
			{
				InputTerrainNode.get_objects_func = GetObjectsCB.GetFuncPtr(new GetObjectsCB.GetObjectsFunc(InputTerrainNode.GetObjects)).Value;
			}
		}

		// Token: 0x060034DA RID: 13530 RVA: 0x001A7134 File Offset: 0x001A5334
		public unsafe override void CleanUp()
		{
			if (this.pheightsData != null)
			{
				this.pheightsData->Dispose();
				this.pheightsData = null;
				this.arr_heightsData.Dispose();
				this.isHeightsDataFilled = false;
			}
			if (this.psplatsData != null)
			{
				this.psplatsData->Dispose();
				this.psplatsData = null;
				this.arr_splatsData.Dispose();
				this.isSplatsDataFilled = false;
			}
			this.isDetailsDataFilled = false;
			if (this.ptreesData != null)
			{
				ref InputTerrainNode.TreesData ptr = ref *this.ptreesData;
				for (int i = 0; i < ptr.grid.resolution.y; i++)
				{
					for (int j = 0; j < ptr.grid.resolution.x; j++)
					{
						ptr.GetCell(j, i).Dispose();
					}
				}
				if (this.treesCells.IsCreated)
				{
					this.treesCells.Dispose();
				}
				this.ptreesData = null;
				this.arr_treesData.Dispose();
				this.isTreesDataFilled = false;
			}
			if (this.pobjectsData != null)
			{
				if (this.pobjectsData == null)
				{
					return;
				}
				ref InputTerrainNode.ObjectsData ptr2 = ref *this.pobjectsData;
				for (int k = 0; k < ptr2.grid.resolution.y; k++)
				{
					for (int l = 0; l < ptr2.grid.resolution.x; l++)
					{
						ptr2.GetCell(l, k).Dispose();
					}
				}
				if (this.objectsCells.IsCreated)
				{
					this.objectsCells.Dispose();
				}
				this.pobjectsData = null;
				this.root_object = null;
				this.game_objects = null;
				this.arr_objectsData.Dispose();
				this.isObjectsDataFilled = false;
			}
		}

		// Token: 0x060034DB RID: 13531 RVA: 0x001A72E4 File Offset: 0x001A54E4
		public unsafe override object GetValue(NodePort port)
		{
			string fieldName = port.fieldName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(fieldName);
			if (num <= 1960560654U)
			{
				if (num <= 462646629U)
				{
					if (num != 405004626U)
					{
						if (num == 462646629U)
						{
							if (fieldName == "heightmapSize")
							{
								return (this.td != null) ? new float2((float)this.td.heightmapResolution, (float)this.td.heightmapResolution) : new float2(0f, 0f);
							}
						}
					}
					else if (fieldName == "trees")
					{
						if (this.ptreesData != null && !this.isTreesDataFilled)
						{
							this.FillTreesData();
						}
						return new GetTreesCB
						{
							get_trees_func = InputTerrainNode.get_trees_func,
							data = (void*)this.ptreesData
						};
					}
				}
				else if (num != 866844096U)
				{
					if (num != 1499984805U)
					{
						if (num == 1960560654U)
						{
							if (fieldName == "terrain")
							{
								if (this.pheightsData != null && !this.isHeightsDataFilled)
								{
									this.FillHeightsData();
								}
								if (this.psplatsData != null && !this.isSplatsDataFilled)
								{
									this.FillSplatsData();
								}
								if (this.ptreesData != null && !this.isTreesDataFilled)
								{
									this.FillTreesData();
								}
								if (this.pobjectsData != null && !this.isObjectsDataFilled)
								{
									this.FillObjectsData();
								}
								return new GetTerrainCB
								{
									heights = new GetFloat2DCB
									{
										get_value_func = InputTerrainNode.get_heights_func,
										data = (void*)this.pheightsData
									},
									splats = new GetSplatCB
									{
										get_tile_func = InputTerrainNode.get_splats_tile_func,
										data = (void*)this.psplatsData
									},
									trees = new GetTreesCB
									{
										get_trees_func = InputTerrainNode.get_trees_func,
										data = (void*)this.ptreesData
									},
									objects = new ObjectsPackage
									{
										objects = new GetObjectsCB
										{
											get_objects_func = InputTerrainNode.get_objects_func,
											data = (void*)this.pobjectsData
										},
										objList = this.game_objects
									}
								};
							}
						}
					}
					else if (fieldName == "details")
					{
						if (!this.isDetailsDataFilled)
						{
							this.FillDetailsData();
						}
						return new GetDetailsCB
						{
							GetDetailDensitiesFunction = new GetDetailsCB.GetDetailDensitiesForCoord(this.GetDetailDensities)
						};
					}
				}
				else if (fieldName == "alphaLayerIndex")
				{
					if (this.psplatsData != null && !this.isSplatsDataFilled)
					{
						this.FillSplatsData();
					}
					return this.alphaLayerIndex;
				}
			}
			else if (num <= 2831556715U)
			{
				if (num != 2225829743U)
				{
					if (num != 2345278562U)
					{
						if (num == 2831556715U)
						{
							if (fieldName == "objects")
							{
								if (this.pobjectsData != null && !this.isObjectsDataFilled)
								{
									this.FillObjectsData();
								}
								return new ObjectsPackage
								{
									objects = new GetObjectsCB
									{
										get_objects_func = InputTerrainNode.get_objects_func,
										data = (void*)this.pobjectsData
									},
									objList = this.game_objects
								};
							}
						}
					}
					else if (fieldName == "nonAlphaLayers")
					{
						if (this.psplatsData != null && !this.isSplatsDataFilled)
						{
							this.FillSplatsData();
						}
						return new GetSplatCB
						{
							get_tile_func = InputTerrainNode.get_non_alpha_tile_func,
							data = (void*)this.psplatsData
						};
					}
				}
				else if (fieldName == "terrainSize")
				{
					return (this.td != null) ? this.td.size : new float3(0f, 0f, 0f);
				}
			}
			else if (num != 2863509027U)
			{
				if (num != 2916510632U)
				{
					if (num == 3667798628U)
					{
						if (fieldName == "alphaLayer")
						{
							if (this.psplatsData != null && !this.isSplatsDataFilled)
							{
								this.FillSplatsData();
							}
							return new GetFloat2DCB
							{
								get_value_func = InputTerrainNode.get_alpha_layer_func,
								data = (void*)this.psplatsData
							};
						}
					}
				}
				else if (fieldName == "splats")
				{
					if (this.psplatsData != null && !this.isSplatsDataFilled)
					{
						this.FillSplatsData();
					}
					return new GetSplatCB
					{
						get_tile_func = InputTerrainNode.get_splats_tile_func,
						data = (void*)this.psplatsData
					};
				}
			}
			else if (fieldName == "heights")
			{
				if (this.pheightsData != null && !this.isHeightsDataFilled)
				{
					this.FillHeightsData();
				}
				return new GetFloat2DCB
				{
					get_value_func = InputTerrainNode.get_heights_func,
					data = (void*)this.pheightsData
				};
			}
			return null;
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x001A7828 File Offset: 0x001A5A28
		private unsafe void FillHeightsData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputTerrainNode.HeightsData ptr = ref *this.pheightsData;
			ptr.grid.resolution = this.td.heightmapResolution - 1;
			ptr.grid.cell_size = new float2(this.td.size.x, this.td.size.z) / ptr.grid.resolution;
			int2 @int = subRect.ApplyTo(ref ptr.grid);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			ptr.filter_type = this.filterType;
			float[,] target = this.td.GetHeights(@int.x, @int.y, ptr.grid.resolution.x, ptr.grid.resolution.y);
			ptr.heights = (float*)AllocationManager.PinGCArrayAndGetDataAddress(target, ref ptr.h_heights);
			this.isHeightsDataFilled = true;
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x001A794C File Offset: 0x001A5B4C
		private unsafe void FillSplatsData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputTerrainNode.SplatsData ptr = ref *this.psplatsData;
			ptr.grid.resolution = this.td.alphamapResolution;
			ptr.grid.cell_size = new float2(this.td.size.x, this.td.size.z) / ptr.grid.resolution;
			int2 @int = subRect.ApplyTo(ref ptr.grid);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			ptr.layers = this.td.alphamapLayers;
			ptr.filter_type = this.filterType;
			float[,,] alphamaps = this.td.GetAlphamaps(@int.x, @int.y, ptr.grid.resolution.x, ptr.grid.resolution.y);
			ptr.remap = (int*)AllocationManager.PinGCArrayAndGetDataAddress(this.splats_remap.ToArray(), ref ptr.h_remap);
			ptr.remap_count = this.splats_remap.Count;
			ptr.alphas = (float*)AllocationManager.PinGCArrayAndGetDataAddress(alphamaps, ref ptr.h_alphas);
			ptr.alphaLayerIndex = -1;
			for (int i = 0; i < this.td.terrainLayers.Length; i++)
			{
				Texture2D diffuseTexture = this.td.terrainLayers[i].diffuseTexture;
				if (diffuseTexture.width == 64 && diffuseTexture.height == 64)
				{
					ptr.alphaLayerIndex = i;
					break;
				}
			}
			this.alphaLayerIndex = (float)ptr.alphaLayerIndex;
			ptr.normalizeNonAlphaLayers = this.normalizeNonAlphaLayers;
			this.isSplatsDataFilled = true;
		}

		// Token: 0x060034DE RID: 13534 RVA: 0x001A7B1C File Offset: 0x001A5D1C
		private void FillDetailsData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			this.detailsData.grid.resolution = this.td.detailResolution;
			this.detailsData.grid.cell_size = new float2(this.td.size.x, this.td.size.z) / this.detailsData.grid.resolution;
			int2 @int = subRect.ApplyTo(ref this.detailsData.grid);
			transformation.ApplyTo(ref this.detailsData.grid, base.mm.tgt_terrain);
			int num = this.td.detailPrototypes.Length;
			this.detailsData.filter_type = this.filterType;
			float[,,] array = new float[this.detailsData.grid.resolution.y, this.detailsData.grid.resolution.x, num];
			int[][,] array2 = new int[num][,];
			for (int i = 0; i < num; i++)
			{
				array2[i] = this.td.GetDetailLayer(@int.x, @int.y, this.detailsData.grid.resolution.x, this.detailsData.grid.resolution.y, i);
			}
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < this.detailsData.grid.resolution.y; k++)
				{
					for (int l = 0; l < this.detailsData.grid.resolution.x; l++)
					{
						array[k, l, j] = (float)array2[j][k, l] / 16f;
					}
				}
			}
			this.detailsData.remap = this.details_remap.ToArray();
			this.detailsData.details = array;
			this.isDetailsDataFilled = true;
		}

		// Token: 0x060034DF RID: 13535 RVA: 0x001A7D44 File Offset: 0x001A5F44
		private unsafe void FillTreesData()
		{
			SubRect subRect = base.GetInputValue<SubRect>("subRect", null) ?? this.subRect;
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputTerrainNode.TreesData ptr = ref *this.ptreesData;
			ptr.grid.cell_size = new float2(32f, 32f);
			ptr.grid.resolution = (int)(this.td.size.x / ptr.grid.cell_size.x);
			subRect.ApplyTo(ref ptr.grid);
			transformation.ApplyTo(ref ptr.grid, base.mm.tgt_terrain);
			TreeInstance[] treeInstances = this.td.treeInstances;
			this.treesCells = new NativeArray<IntPtr>(ptr.grid.resolution.y * ptr.grid.resolution.x, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			ptr.cells = (IntPtr*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<IntPtr>(this.treesCells);
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					GrowBuffer<TreeInstance> gb = new GrowBuffer<TreeInstance>(Allocator.TempJob, 0);
					ptr.SetCell(j, i, gb);
				}
			}
			TreeInstance[] array;
			TreeInstance* arr_trees;
			if ((array = treeInstances) == null || array.Length == 0)
			{
				arr_trees = null;
			}
			else
			{
				arr_trees = &array[0];
			}
			int[] array2;
			int* ptr2;
			if ((array2 = this.trees_remap.ToArray()) == null || array2.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &array2[0];
			}
			InputTerrainNode.FillTreesJob fillTreesJob = default(InputTerrainNode.FillTreesJob);
			fillTreesJob.arr_trees = arr_trees;
			fillTreesJob.trees_remap = ptr2;
			fillTreesJob.pdata = this.ptreesData;
			fillTreesJob.size = this.td.size;
			fillTreesJob.verify = this.verifyTrees;
			fillTreesJob.ofs = ptr.grid.world_pos;
			fillTreesJob.wrap = ptr.grid.wrap;
			if (this.SingleThreaded)
			{
				fillTreesJob.Run(treeInstances.Length);
			}
			else
			{
				fillTreesJob.Schedule(treeInstances.Length, 1, default(JobHandle)).Complete();
			}
			if (fillTreesJob.ignored != 0)
			{
				Debug.LogWarning(string.Format("Ignored {0} trees", fillTreesJob.ignored));
			}
			array2 = null;
			array = null;
			this.isTreesDataFilled = true;
			if (this.verifyTrees && !this.Validate(treeInstances))
			{
				this.CleanUp();
			}
		}

		// Token: 0x060034E0 RID: 13536 RVA: 0x001A7FB8 File Offset: 0x001A61B8
		public unsafe bool Validate(TreeInstance[] arr_trees)
		{
			ref InputTerrainNode.TreesData ptr = ref *this.ptreesData;
			int[] array = new int[arr_trees.Length];
			int num = 0;
			for (int i = 0; i < ptr.grid.resolution.y; i++)
			{
				for (int j = 0; j < ptr.grid.resolution.x; j++)
				{
					GrowBuffer<TreeInstance> cell = ptr.GetCell(j, i);
					int count = cell.Count;
					num += count;
					for (int k = 0; k < count; k++)
					{
						ref TreeInstance ptr2 = ref cell[k];
						int num2 = ptr2.prototypeIndex >> 4;
						ptr2.prototypeIndex &= 15;
						array[num2]++;
					}
				}
			}
			if (num != array.Length)
			{
				Debug.LogError(string.Format("Wrong number of imported trees: {0} instead of {1}", num, array.Length));
				return false;
			}
			for (int l = 0; l < num; l++)
			{
				int num3 = array[l];
				if (num3 != 1)
				{
					Debug.LogError(string.Format("Tree {0} imported {1} times", l, num3));
					return false;
				}
			}
			Debug.Log(string.Format("Successfuly imported {0} trees", num));
			return true;
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x001A80E0 File Offset: 0x001A62E0
		private unsafe void FillObjectsData()
		{
			Transformation transformation = base.GetInputValue<Transformation>("transformation", null) ?? this.transformation;
			ref InputTerrainNode.ObjectsData ptr = ref *this.pobjectsData;
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
				this.game_objects = BSGCloneHelper.GetObjectsToClone(this.root_object, this.ObjectsLayerMask);
			}
			else
			{
				this.game_objects = new List<GameObject>();
			}
			ObjectInstance[] array2 = this.GenerateObjectInstancesArray(this.game_objects);
			this.objectsCells = new NativeArray<IntPtr>(ptr.grid.resolution.y * ptr.grid.resolution.x, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			ptr.cells = (IntPtr*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<IntPtr>(this.objectsCells);
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
			InputTerrainNode.FillObjectsJob fillObjectsJob = default(InputTerrainNode.FillObjectsJob);
			fillObjectsJob.arr_objects = arr_objects;
			fillObjectsJob.objects_remap = ptr2;
			fillObjectsJob.pdata = this.pobjectsData;
			fillObjectsJob.size = this.td.size;
			fillObjectsJob.verify = this.verifyObjects;
			if (this.SingleThreaded)
			{
				fillObjectsJob.Run(array2.Length);
			}
			else
			{
				fillObjectsJob.Schedule(array2.Length, 1, default(JobHandle)).Complete();
			}
			if (fillObjectsJob.ignored != 0)
			{
				Debug.LogWarning(string.Format("Ignored {0} objects", fillObjectsJob.ignored));
			}
			array4 = null;
			array3 = null;
			this.isObjectsDataFilled = true;
			if (this.verifyObjects && !this.ValidateObjects(array2))
			{
				this.CleanUp();
			}
		}

		// Token: 0x060034E2 RID: 13538 RVA: 0x001A8408 File Offset: 0x001A6608
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

		// Token: 0x060034E3 RID: 13539 RVA: 0x001A623E File Offset: 0x001A443E
		private GameObject GetTerrainPrefab(string name)
		{
			return Resources.Load<GameObject>("TerrainPrefabs/" + name);
		}

		// Token: 0x060034E4 RID: 13540 RVA: 0x001A84E0 File Offset: 0x001A66E0
		public unsafe bool ValidateObjects(ObjectInstance[] arr_objects)
		{
			ref InputTerrainNode.ObjectsData ptr = ref *this.pobjectsData;
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

		// Token: 0x040023CE RID: 9166
		public TerrainData td;

		// Token: 0x040023CF RID: 9167
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public SubRect subRect;

		// Token: 0x040023D0 RID: 9168
		[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
		public Transformation transformation;

		// Token: 0x040023D1 RID: 9169
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB heights;

		// Token: 0x040023D2 RID: 9170
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB splats;

		// Token: 0x040023D3 RID: 9171
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTreesCB trees;

		// Token: 0x040023D4 RID: 9172
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetDetailsCB details;

		// Token: 0x040023D5 RID: 9173
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public ObjectsPackage objects;

		// Token: 0x040023D6 RID: 9174
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetTerrainCB terrain;

		// Token: 0x040023D7 RID: 9175
		[Space]
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float3 terrainSize;

		// Token: 0x040023D8 RID: 9176
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float2 heightmapSize;

		// Token: 0x040023D9 RID: 9177
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetSplatCB nonAlphaLayers;

		// Token: 0x040023DA RID: 9178
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public GetFloat2DCB alphaLayer;

		// Token: 0x040023DB RID: 9179
		[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, false)]
		public float alphaLayerIndex;

		// Token: 0x040023DC RID: 9180
		public LayerMask ObjectsLayerMask = -1;

		// Token: 0x040023DD RID: 9181
		public bool normalizeNonAlphaLayers = true;

		// Token: 0x040023DE RID: 9182
		public bool verifyTrees;

		// Token: 0x040023DF RID: 9183
		public bool verifyObjects;

		// Token: 0x040023E0 RID: 9184
		public FilterType filterType = FilterType.Bilinear;

		// Token: 0x040023E1 RID: 9185
		public bool SingleThreaded;

		// Token: 0x040023E2 RID: 9186
		[HideInInspector]
		public List<int> splats_remap = new List<int>();

		// Token: 0x040023E3 RID: 9187
		[HideInInspector]
		public List<int> trees_remap = new List<int>();

		// Token: 0x040023E4 RID: 9188
		[HideInInspector]
		public List<int> details_remap = new List<int>();

		// Token: 0x040023E5 RID: 9189
		[HideInInspector]
		public List<int> objects_remap = new List<int>();

		// Token: 0x040023E6 RID: 9190
		private List<GameObject> game_objects;

		// Token: 0x040023E7 RID: 9191
		private GameObject root_object;

		// Token: 0x040023E8 RID: 9192
		private const int ALPHA_TEXTURE_SIZE = 64;

		// Token: 0x040023E9 RID: 9193
		private bool isHeightsDataFilled;

		// Token: 0x040023EA RID: 9194
		private bool isSplatsDataFilled;

		// Token: 0x040023EB RID: 9195
		private bool isDetailsDataFilled;

		// Token: 0x040023EC RID: 9196
		private bool isTreesDataFilled;

		// Token: 0x040023ED RID: 9197
		private bool isObjectsDataFilled;

		// Token: 0x040023EE RID: 9198
		public NativeArray<IntPtr> treesCells;

		// Token: 0x040023EF RID: 9199
		public NativeArray<IntPtr> objectsCells;

		// Token: 0x040023F0 RID: 9200
		public static IntPtr get_heights_func;

		// Token: 0x040023F1 RID: 9201
		public static IntPtr get_splats_tile_func;

		// Token: 0x040023F2 RID: 9202
		public static IntPtr get_alpha_layer_func;

		// Token: 0x040023F3 RID: 9203
		public static IntPtr get_non_alpha_tile_func;

		// Token: 0x040023F4 RID: 9204
		public static IntPtr get_trees_func;

		// Token: 0x040023F5 RID: 9205
		public static IntPtr get_objects_func;

		// Token: 0x040023F6 RID: 9206
		public NativeArray<InputTerrainNode.HeightsData> arr_heightsData;

		// Token: 0x040023F7 RID: 9207
		public unsafe InputTerrainNode.HeightsData* pheightsData;

		// Token: 0x040023F8 RID: 9208
		public NativeArray<InputTerrainNode.SplatsData> arr_splatsData;

		// Token: 0x040023F9 RID: 9209
		public unsafe InputTerrainNode.SplatsData* psplatsData;

		// Token: 0x040023FA RID: 9210
		public InputTerrainNode.DetailsData detailsData;

		// Token: 0x040023FB RID: 9211
		public NativeArray<InputTerrainNode.TreesData> arr_treesData;

		// Token: 0x040023FC RID: 9212
		public unsafe InputTerrainNode.TreesData* ptreesData;

		// Token: 0x040023FD RID: 9213
		public NativeArray<InputTerrainNode.ObjectsData> arr_objectsData;

		// Token: 0x040023FE RID: 9214
		public unsafe InputTerrainNode.ObjectsData* pobjectsData;

		// Token: 0x020008CB RID: 2251
		public struct HeightsData
		{
			// Token: 0x06005206 RID: 20998 RVA: 0x0023F1B3 File Offset: 0x0023D3B3
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_heights);
			}

			// Token: 0x040040F9 RID: 16633
			public GridData grid;

			// Token: 0x040040FA RID: 16634
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* heights;

			// Token: 0x040040FB RID: 16635
			public ulong h_heights;

			// Token: 0x040040FC RID: 16636
			public FilterType filter_type;
		}

		// Token: 0x020008CC RID: 2252
		public struct SplatsData
		{
			// Token: 0x06005207 RID: 20999 RVA: 0x0023F1C0 File Offset: 0x0023D3C0
			public void Dispose()
			{
				AllocationManager.ReleaseGCObject(ref this.h_remap);
				AllocationManager.ReleaseGCObject(ref this.h_alphas);
			}

			// Token: 0x040040FD RID: 16637
			public GridData grid;

			// Token: 0x040040FE RID: 16638
			public int layers;

			// Token: 0x040040FF RID: 16639
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* remap;

			// Token: 0x04004100 RID: 16640
			public int remap_count;

			// Token: 0x04004101 RID: 16641
			[NativeDisableUnsafePtrRestriction]
			public unsafe float* alphas;

			// Token: 0x04004102 RID: 16642
			public ulong h_remap;

			// Token: 0x04004103 RID: 16643
			public ulong h_alphas;

			// Token: 0x04004104 RID: 16644
			public FilterType filter_type;

			// Token: 0x04004105 RID: 16645
			public bool normalizeNonAlphaLayers;

			// Token: 0x04004106 RID: 16646
			public int alphaLayerIndex;
		}

		// Token: 0x020008CD RID: 2253
		public struct DetailsData
		{
			// Token: 0x1700067C RID: 1660
			// (get) Token: 0x06005208 RID: 21000 RVA: 0x0023F1D8 File Offset: 0x0023D3D8
			public int detail_prototypes_count
			{
				get
				{
					return this.details.GetLength(2);
				}
			}

			// Token: 0x06005209 RID: 21001 RVA: 0x0023F1E8 File Offset: 0x0023D3E8
			public unsafe void GetDetailDensities(float2 worldSpacePos, float* output_prototype_densities, int output_prototype_densities_length)
			{
				float2 v = this.grid.WorldToLocal(worldSpacePos);
				int2 @int = new int2(v);
				@int = math.clamp(@int, int2.zero, this.grid.resolution - new int2(1, 1));
				for (int i = 0; i < output_prototype_densities_length; i++)
				{
					output_prototype_densities[i] = 0f;
				}
				int length = this.details.GetLength(2);
				for (int j = 0; j < length; j++)
				{
					int num = this.remap[j];
					if (j < 0)
					{
						output_prototype_densities[num] = 0f;
					}
					else
					{
						output_prototype_densities[num] = this.details[@int.y, @int.x, j];
					}
				}
			}

			// Token: 0x04004107 RID: 16647
			public GridData grid;

			// Token: 0x04004108 RID: 16648
			public int[] remap;

			// Token: 0x04004109 RID: 16649
			public float[,,] details;

			// Token: 0x0400410A RID: 16650
			public FilterType filter_type;
		}

		// Token: 0x020008CE RID: 2254
		public struct TreesData
		{
			// Token: 0x0600520A RID: 21002 RVA: 0x0023F2A4 File Offset: 0x0023D4A4
			public unsafe GrowBuffer<TreeInstance> GetCell(int cx, int cy)
			{
				return new GrowBuffer<TreeInstance>(this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}

			// Token: 0x0600520B RID: 21003 RVA: 0x0023F2CF File Offset: 0x0023D4CF
			public unsafe void SetCell(int cx, int cy, GrowBuffer<TreeInstance> gb)
			{
				this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = gb.ToIntPtr();
			}

			// Token: 0x0400410B RID: 16651
			public GridData grid;

			// Token: 0x0400410C RID: 16652
			[NativeDisableUnsafePtrRestriction]
			public unsafe IntPtr* cells;
		}

		// Token: 0x020008CF RID: 2255
		public struct ObjectsData
		{
			// Token: 0x0600520C RID: 21004 RVA: 0x0023F2FC File Offset: 0x0023D4FC
			public unsafe GrowBuffer<ObjectInstance> GetCell(int cx, int cy)
			{
				return new GrowBuffer<ObjectInstance>(this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
			}

			// Token: 0x0600520D RID: 21005 RVA: 0x0023F327 File Offset: 0x0023D527
			public unsafe void SetCell(int cx, int cy, GrowBuffer<ObjectInstance> gb)
			{
				this.cells[(IntPtr)(cy * this.grid.resolution.y + cx) * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = gb.ToIntPtr();
			}

			// Token: 0x0400410D RID: 16653
			public GridData grid;

			// Token: 0x0400410E RID: 16654
			[NativeDisableUnsafePtrRestriction]
			public unsafe IntPtr* cells;
		}

		// Token: 0x020008D0 RID: 2256
		[BurstCompile]
		private struct FillTreesJob : IJobParallelFor
		{
			// Token: 0x0600520E RID: 21006 RVA: 0x0023F354 File Offset: 0x0023D554
			public unsafe void Execute(int index)
			{
				ref InputTerrainNode.TreesData ptr = ref *this.pdata;
				TreeInstance treeInstance = this.arr_trees[index];
				treeInstance.prototypeIndex = this.trees_remap[treeInstance.prototypeIndex];
				if (this.verify)
				{
					treeInstance.prototypeIndex |= index << 4;
				}
				treeInstance.position.Scale(this.size);
				treeInstance.position += new Vector3(this.ofs.x, 0f, this.ofs.y);
				if (this.wrap)
				{
					treeInstance.position.x = treeInstance.position.x % this.size.x;
					treeInstance.position.z = treeInstance.position.z % this.size.z;
					if (treeInstance.position.x < 0f)
					{
						treeInstance.position.x = treeInstance.position.x + this.size.x;
					}
					if (treeInstance.position.z < 0f)
					{
						treeInstance.position.z = treeInstance.position.z + this.size.z;
					}
				}
				if (treeInstance.position.x < 0f || treeInstance.position.x >= this.size.x || treeInstance.position.z < 0f || treeInstance.position.z >= this.size.z)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				int num = (int)(treeInstance.position.x * ptr.grid.inv_cell_size.x);
				int num2 = (int)(treeInstance.position.z * ptr.grid.inv_cell_size.y);
				if (num < 0 || num >= ptr.grid.resolution.x || num2 < 0 || num2 >= ptr.grid.resolution.y)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				treeInstance.position.x = treeInstance.position.x - (float)num * ptr.grid.cell_size.x;
				treeInstance.position.z = treeInstance.position.z - (float)num2 * ptr.grid.cell_size.y;
				GrowBuffer<TreeInstance> cell = ptr.GetCell(num, num2);
				if (!cell.IsCreated)
				{
					Interlocked.Increment(ref this.ignored);
					return;
				}
				cell.Add(treeInstance);
			}

			// Token: 0x0400410F RID: 16655
			[NativeDisableUnsafePtrRestriction]
			public unsafe TreeInstance* arr_trees;

			// Token: 0x04004110 RID: 16656
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* trees_remap;

			// Token: 0x04004111 RID: 16657
			[NativeDisableUnsafePtrRestriction]
			public unsafe InputTerrainNode.TreesData* pdata;

			// Token: 0x04004112 RID: 16658
			public Vector3 size;

			// Token: 0x04004113 RID: 16659
			public bool verify;

			// Token: 0x04004114 RID: 16660
			public int ignored;

			// Token: 0x04004115 RID: 16661
			public float2 ofs;

			// Token: 0x04004116 RID: 16662
			public bool wrap;
		}

		// Token: 0x020008D1 RID: 2257
		[BurstCompile]
		private struct FillObjectsJob : IJobParallelFor
		{
			// Token: 0x0600520F RID: 21007 RVA: 0x0023F5D8 File Offset: 0x0023D7D8
			public unsafe void Execute(int index)
			{
				ref InputTerrainNode.ObjectsData ptr = ref *this.pdata;
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

			// Token: 0x04004117 RID: 16663
			[NativeDisableUnsafePtrRestriction]
			public unsafe ObjectInstance* arr_objects;

			// Token: 0x04004118 RID: 16664
			[NativeDisableUnsafePtrRestriction]
			public unsafe int* objects_remap;

			// Token: 0x04004119 RID: 16665
			[NativeDisableUnsafePtrRestriction]
			public unsafe InputTerrainNode.ObjectsData* pdata;

			// Token: 0x0400411A RID: 16666
			public Vector3 size;

			// Token: 0x0400411B RID: 16667
			public bool verify;

			// Token: 0x0400411C RID: 16668
			public int ignored;
		}
	}
}
