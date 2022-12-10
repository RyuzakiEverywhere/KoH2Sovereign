using System;
using System.Collections.Generic;
using System.Diagnostics;
using BSG.MapMaker.Nodes;
using UnityEngine;
using XNode;

namespace BSG.MapMaker
{
	// Token: 0x0200037B RID: 891
	[CreateAssetMenu(fileName = "New Map Maker", menuName = "Map Maker")]
	[Serializable]
	public class MapMakerGraph : NodeGraph
	{
		// Token: 0x06003446 RID: 13382 RVA: 0x001A29CC File Offset: 0x001A0BCC
		private int FindOrAddPrototype(List<TerrainLayer> protos, TerrainLayer proto)
		{
			for (int i = 0; i < protos.Count; i++)
			{
				if (protos[i] == proto)
				{
					return i;
				}
			}
			protos.Add(proto);
			return protos.Count - 1;
		}

		// Token: 0x06003447 RID: 13383 RVA: 0x001A2A0C File Offset: 0x001A0C0C
		private int FindOrAddPrototype(List<TreePrototype> protos, TreePrototype proto)
		{
			for (int i = 0; i < protos.Count; i++)
			{
				if (protos[i].Equals(proto))
				{
					return i;
				}
			}
			protos.Add(proto);
			return protos.Count - 1;
		}

		// Token: 0x06003448 RID: 13384 RVA: 0x001A2A4C File Offset: 0x001A0C4C
		private int FindOrAddPrototype(List<DetailPrototype> protos, DetailPrototype proto)
		{
			for (int i = 0; i < protos.Count; i++)
			{
				if (proto == protos[i])
				{
					return i;
				}
			}
			protos.Add(proto);
			return protos.Count - 1;
		}

		// Token: 0x06003449 RID: 13385 RVA: 0x001A2A85 File Offset: 0x001A0C85
		private void SetIdxAt(List<int> lst, int idx, int val)
		{
			while (lst.Count <= idx)
			{
				lst.Add(-1);
			}
			lst[idx] = val;
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x001A2AA4 File Offset: 0x001A0CA4
		private void AddSplatPrototypes(InputTerrainNode itn, List<TerrainLayer> splat_prototypes)
		{
			itn.splats_remap.Clear();
			TerrainLayer[] terrainLayers = itn.td.terrainLayers;
			int num = terrainLayers.Length;
			for (int i = 0; i < num; i++)
			{
				TerrainLayer proto = terrainLayers[i];
				int idx = this.FindOrAddPrototype(splat_prototypes, proto);
				this.SetIdxAt(itn.splats_remap, idx, i);
			}
		}

		// Token: 0x0600344B RID: 13387 RVA: 0x001A2AF8 File Offset: 0x001A0CF8
		private void AddSplatPrototypes(InputSplatsNode isn, List<TerrainLayer> splat_prototypes)
		{
			isn.splats_remap.Clear();
			TerrainLayer[] terrainLayers = isn.td.terrainLayers;
			int num = terrainLayers.Length;
			for (int i = 0; i < num; i++)
			{
				TerrainLayer proto = terrainLayers[i];
				int idx = this.FindOrAddPrototype(splat_prototypes, proto);
				this.SetIdxAt(isn.splats_remap, idx, i);
			}
		}

		// Token: 0x0600344C RID: 13388 RVA: 0x001A2B4C File Offset: 0x001A0D4C
		private void AddTreePrototypes(InputTerrainNode itn, List<TreePrototype> tree_prototypes)
		{
			itn.trees_remap.Clear();
			TreePrototype[] treePrototypes = itn.td.treePrototypes;
			int num = treePrototypes.Length;
			for (int i = 0; i < num; i++)
			{
				TreePrototype proto = treePrototypes[i];
				int item = this.FindOrAddPrototype(tree_prototypes, proto);
				itn.trees_remap.Add(item);
			}
		}

		// Token: 0x0600344D RID: 13389 RVA: 0x001A2B9C File Offset: 0x001A0D9C
		private void AddDetailPrototypes(InputTerrainNode itn, List<DetailPrototype> output_detail_prototypes)
		{
			itn.details_remap.Clear();
			for (int i = 0; i < itn.td.detailPrototypes.Length; i++)
			{
				DetailPrototype item = itn.td.detailPrototypes[i];
				output_detail_prototypes.Add(item);
				itn.details_remap.Add(output_detail_prototypes.Count - 1);
			}
		}

		// Token: 0x0600344E RID: 13390 RVA: 0x001A2BF4 File Offset: 0x001A0DF4
		private void AddTreePrototypes(InputTreesNode itn, List<TreePrototype> tree_prototypes)
		{
			itn.trees_remap.Clear();
			TreePrototype[] treePrototypes = itn.td.treePrototypes;
			int num = treePrototypes.Length;
			for (int i = 0; i < num; i++)
			{
				TreePrototype proto = treePrototypes[i];
				int item = this.FindOrAddPrototype(tree_prototypes, proto);
				itn.trees_remap.Add(item);
			}
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x001A2C43 File Offset: 0x001A0E43
		private void AddObjectPrototypes(InputObjectsNode ion, List<GameObject> object_prototypes)
		{
			ion.objects_remap.Clear();
		}

		// Token: 0x06003450 RID: 13392 RVA: 0x001A2C50 File Offset: 0x001A0E50
		public Node FindNode(string name)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				Node node = this.nodes[i];
				if (node.name == name)
				{
					return node;
				}
			}
			return null;
		}

		// Token: 0x06003451 RID: 13393 RVA: 0x001A2C94 File Offset: 0x001A0E94
		public void InitPrototypes()
		{
			List<TerrainLayer> list = new List<TerrainLayer>();
			List<TreePrototype> list2 = new List<TreePrototype>();
			List<DetailPrototype> list3 = new List<DetailPrototype>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				Node node = this.nodes[i];
				if (node != null)
				{
					InputTerrainNode inputTerrainNode;
					if ((inputTerrainNode = (node as InputTerrainNode)) == null)
					{
						InputSplatsNode inputSplatsNode;
						if ((inputSplatsNode = (node as InputSplatsNode)) == null)
						{
							InputTreesNode inputTreesNode;
							if ((inputTreesNode = (node as InputTreesNode)) == null)
							{
								if (!(node is OutputTerrainNode))
								{
									if (!(node is OutputSplatsNode))
									{
										if (node is OutputTreesNode)
										{
											flag4 = true;
										}
									}
									else
									{
										flag2 = true;
									}
								}
								else
								{
									flag2 = true;
									flag4 = true;
									flag6 = true;
								}
							}
							else
							{
								InputTreesNode itn = inputTreesNode;
								flag3 = true;
								this.AddTreePrototypes(itn, list2);
							}
						}
						else
						{
							InputSplatsNode isn = inputSplatsNode;
							flag = true;
							this.AddSplatPrototypes(isn, list);
						}
					}
					else
					{
						InputTerrainNode itn2 = inputTerrainNode;
						flag = true;
						flag3 = true;
						flag5 = true;
						this.AddSplatPrototypes(itn2, list);
						this.AddTreePrototypes(itn2, list2);
						this.AddDetailPrototypes(itn2, list3);
					}
				}
			}
			if (flag && flag2)
			{
				this.tgt_terrain.terrainData.terrainLayers = list.ToArray();
			}
			if (flag3 && flag4)
			{
				this.tgt_terrain.terrainData.treeInstances = new TreeInstance[0];
				this.tgt_terrain.terrainData.treePrototypes = list2.ToArray();
			}
			if (flag5 && flag6)
			{
				DetailPrototype[] detailPrototypes = this.tgt_terrain.terrainData.detailPrototypes;
				int[,] details = new int[this.tgt_terrain.terrainData.detailResolution, this.tgt_terrain.terrainData.detailResolution];
				for (int j = 0; j < detailPrototypes.Length; j++)
				{
					this.tgt_terrain.terrainData.SetDetailLayer(0, 0, j, details);
				}
				this.tgt_terrain.terrainData.detailPrototypes = list3.ToArray();
			}
		}

		// Token: 0x06003452 RID: 13394 RVA: 0x001A2E74 File Offset: 0x001A1074
		public void Run(Terrain t)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.tgt_terrain = t;
			this.InitPrototypes();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			for (int i = 0; i < this.nodes.Count; i++)
			{
				Node node = this.nodes[i] as Node;
				if (node != null)
				{
					node.Initialize();
				}
			}
			long elapsedMilliseconds2 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			for (int j = 0; j < this.nodes.Count; j++)
			{
				Node node2 = this.nodes[j] as Node;
				if (node2 != null)
				{
					node2.PreCache();
				}
			}
			long elapsedMilliseconds3 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			for (int k = 0; k < this.nodes.Count; k++)
			{
				Node node3 = this.nodes[k] as Node;
				if (node3 != null)
				{
					node3.Build();
				}
			}
			long elapsedMilliseconds4 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			for (int l = 0; l < this.nodes.Count; l++)
			{
				Node node4 = this.nodes[l] as Node;
				if (node4 != null)
				{
					node4.CleanUp();
				}
			}
			long elapsedMilliseconds5 = stopwatch.ElapsedMilliseconds;
			stopwatch.Restart();
			long num = elapsedMilliseconds + elapsedMilliseconds2 + elapsedMilliseconds3 + elapsedMilliseconds4 + elapsedMilliseconds5;
			Debug.Log(string.Format("Map build in {0}ms", num));
			if (MapMakerGraph.Debug_Mode)
			{
				Debug.Log(string.Format("Build debug details: Init prototypes:{0}ms, Init Nodes:{1}ms, Precache Nodes:{2}ms, Build Nodes:{3}ms, CleanUp Nodes:{4}ms", new object[]
				{
					elapsedMilliseconds,
					elapsedMilliseconds2,
					elapsedMilliseconds3,
					elapsedMilliseconds4,
					elapsedMilliseconds5
				}));
			}
		}

		// Token: 0x04002335 RID: 9013
		public static bool Debug_Mode = false;

		// Token: 0x04002336 RID: 9014
		public static bool Auto_Cache = true;

		// Token: 0x04002337 RID: 9015
		public Terrain tgt_terrain;

		// Token: 0x04002338 RID: 9016
		public int trees_cell_size = 32;

		// Token: 0x04002339 RID: 9017
		public Dictionary<string, Texture2D> generated_textures;

		// Token: 0x0400233A RID: 9018
		public Dictionary<string, float> generated_floats;
	}
}
