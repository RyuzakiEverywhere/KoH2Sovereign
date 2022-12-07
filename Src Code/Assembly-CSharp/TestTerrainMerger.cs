using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000333 RID: 819
[Serializable]
public class TestTerrainMerger : MonoBehaviour
{
	// Token: 0x06003240 RID: 12864 RVA: 0x00197730 File Offset: 0x00195930
	public void GenerateAll()
	{
		this.ResetOutput();
		List<TreeInstance> list = new List<TreeInstance>(this.mergeOut.td.treeInstances);
		int num = this.mergeBase.td.terrainLayers.Length;
		foreach (TestTerrainMerger.MergeShot mergeShot in this.mergeShots)
		{
			mergeShot.Init();
			if (mergeShot.td.terrainLayers.Length != num)
			{
				Debug.Log(mergeShot.td.name + " - Splat prototypes count mismatch");
			}
			else if (mergeShot.td.treePrototypes.Length != this.mergeBase.td.treePrototypes.Length)
			{
				Debug.Log(mergeShot.td.name + " - Tree prototypes count mismatch");
			}
			else
			{
				BSGTerrainTools.ModifyHeights(this.mergeOut.block, this.mergeOut.bounds, mergeShot.heights_gen, mergeShot.alphas_gen);
				BSGTerrainTools.ModifySplats(this.mergeOut.block, this.mergeOut.bounds, mergeShot.splats_gen, mergeShot.alphas_gen);
				BSGTerrainTools.ExtractTrees(list, this.mergeOut.td.bounds, this.mergeOut.bounds, null, mergeShot.trees_alphas_gen, mergeShot.treesMinAlpha);
				list.AddRange(mergeShot.extractedTrees);
				this.mergeOut.td.treeInstances = list.ToArray();
			}
		}
		this.mergeOut.td.SetHeights((int)this.mergeOut.bounds.min.x, (int)this.mergeOut.bounds.min.z, this.mergeOut.block.heights);
		this.mergeOut.td.SetAlphamaps((int)this.mergeOut.bounds.min.x, (int)this.mergeOut.bounds.min.z, this.mergeOut.block.splats);
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x0019796C File Offset: 0x00195B6C
	public void ResetOutput()
	{
		this.mergeBase.Init();
		this.mergeOut.Init();
		this.ResetOutHeights();
		this.ResetOutSplats();
		this.ResetOutTrees();
	}

	// Token: 0x06003242 RID: 12866 RVA: 0x00197996 File Offset: 0x00195B96
	private void ResetOutHeights()
	{
		this.mergeOut.td.SetHeights(0, 0, this.mergeBase.block.heights);
	}

	// Token: 0x06003243 RID: 12867 RVA: 0x001979BA File Offset: 0x00195BBA
	private void ResetOutSplats()
	{
		this.mergeOut.td.SetAlphamaps(0, 0, this.mergeBase.block.splats);
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x001979DE File Offset: 0x00195BDE
	private void ResetOutTrees()
	{
		this.mergeOut.td.treeInstances = this.mergeBase.block.trees.ToArray();
	}

	// Token: 0x06003245 RID: 12869 RVA: 0x00197A08 File Offset: 0x00195C08
	public BSGTerrainTools.TerrainBlock TakeAlphaSnapshot(TerrainData td, Bounds bounds)
	{
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(td, bounds);
		terrainBlock.Import(true, true, false);
		for (int i = 0; i < terrainBlock.splats.GetLength(1); i++)
		{
			for (int j = 0; j < terrainBlock.splats.GetLength(0); j++)
			{
				terrainBlock.splats[i, j, 0] = 0f;
			}
		}
		return terrainBlock;
	}

	// Token: 0x06003246 RID: 12870 RVA: 0x00197A68 File Offset: 0x00195C68
	public BSGTerrainTools.TerrainBlock LoadBlockSnapshotFromFile(string path, TerrainData sourceTd)
	{
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(sourceTd, sourceTd.bounds);
		terrainBlock.Load(path + ".terrain.bytes", true, true, false);
		return terrainBlock;
	}

	// Token: 0x06003247 RID: 12871 RVA: 0x00197A8C File Offset: 0x00195C8C
	public BSGTerrainTools.TerrainBlock SaveBlockSnapshot(string path, TerrainData td, Bounds bounds)
	{
		BSGTerrainTools.TerrainBlock block = new BSGTerrainTools.TerrainBlock(td, bounds);
		return this.SaveBlockSnapshot(path, block);
	}

	// Token: 0x06003248 RID: 12872 RVA: 0x00197AA9 File Offset: 0x00195CA9
	public BSGTerrainTools.TerrainBlock SaveBlockSnapshot(string path, BSGTerrainTools.TerrainBlock block)
	{
		block.Import(true, true, false);
		block.Save(path + ".terrain.bytes", true, true, false);
		return block;
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x00197A68 File Offset: 0x00195C68
	public BSGTerrainTools.TerrainBlock LoadBlockSnapshot(string path, TerrainData sourceTd)
	{
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(sourceTd, sourceTd.bounds);
		terrainBlock.Load(path + ".terrain.bytes", true, true, false);
		return terrainBlock;
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x00197ACC File Offset: 0x00195CCC
	public void PasteRectSnapshot(BSGTerrainTools.TerrainBlock sourceBlock, TerrainData td, Bounds bounds, float fade = 0f)
	{
		if (sourceBlock == null)
		{
			return;
		}
		if (td == null)
		{
			return;
		}
		BSGTerrainTools.Float2D float2D = new BSGTerrainTools.Float2D(sourceBlock.heights);
		BSGTerrainTools.Float3D float3D = new BSGTerrainTools.Float3D(sourceBlock.splats);
		if (float2D != null)
		{
			Bounds bounds2 = BSGTerrainTools.SnapHeightsBounds(bounds, td);
			float2D.SetWorldRect(bounds2.min.x, bounds2.min.z, bounds2.size.x, bounds2.size.z);
		}
		if (float3D != null)
		{
			Bounds bounds3 = BSGTerrainTools.SnapSplatsBounds(bounds, td);
			float3D.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
		}
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(td, bounds);
		terrainBlock.Import(true, true, false);
		BSGTerrainTools.Gen2D gen2D = new BSGTerrainTools.Gen2D(1f);
		gen2D.func = delegate(BSGTerrainTools.Gen2D gen, float wx, float wy, float src_val, float tgt_val, ref float alpha)
		{
			float num = wx - bounds.min.x;
			float num2 = bounds.max.x - wx;
			float num3 = wy - bounds.min.z;
			float num4 = bounds.max.z - wy;
			float num5 = Mathf.Min(new float[]
			{
				num,
				num2,
				num3,
				num4
			});
			if (num5 <= 0f)
			{
				return 0f;
			}
			if (num5 >= fade)
			{
				return 1f;
			}
			return num5 / fade;
		};
		if (float2D != null)
		{
			BSGTerrainTools.ModifyHeights(terrainBlock, bounds, new BSGTerrainTools.Gen2D(float2D), gen2D);
		}
		if (float3D != null)
		{
			BSGTerrainTools.ModifySplats(terrainBlock, bounds, new BSGTerrainTools.Gen3D(float3D), gen2D);
		}
		terrainBlock.Apply(true, true, true);
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x00197C10 File Offset: 0x00195E10
	public void TakeAndSaveSnapshotToFileAndPNGs(string path, TerrainData td, Bounds bounds)
	{
		Vector2Int resolution = BSGTerrainTools.HeightsResolution(td);
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out vector2Int, out vector2Int2);
		BSGTerrainTools.Float2D float2D = BSGTerrainTools.Float2D.ImportHeights(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
		float2D.SavePNG(path + "_heights.png");
		resolution = BSGTerrainTools.SplatsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out vector2Int, out vector2Int2);
		BSGTerrainTools.Float3D float3D = new BSGTerrainTools.Float3D(td.GetAlphamaps(vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y));
		for (int i = 0; i < float3D.layers.Length; i++)
		{
			float3D.layers[i].SavePNG(path + "_layer" + i.ToString() + ".png");
		}
		List<TreeInstance> all_trees = new List<TreeInstance>(td.treeInstances);
		List<TreeInstance> list = new List<TreeInstance>();
		BSGTerrainTools.ExtractTrees(all_trees, td.bounds, bounds, list, null, 1f);
		for (int j = 0; j < list.Count; j++)
		{
			TreeInstance treeInstance = list[j];
			float num = treeInstance.position.x * td.size.x - bounds.min.x;
			treeInstance.position.x = num / bounds.size.x;
			float num2 = treeInstance.position.z * td.size.z - bounds.min.z;
			treeInstance.position.z = num2 / bounds.size.z;
			list[j] = treeInstance;
		}
		using (FileStream fileStream = new FileStream(path + ".terrain.bytes", FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				float2D.Save(binaryWriter);
				float3D.Save(binaryWriter);
				binaryWriter.Write(list.Count);
				for (int k = 0; k < list.Count; k++)
				{
					BSGTerrainTools.SaveTree(binaryWriter, list[k]);
				}
			}
		}
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x00197E5C File Offset: 0x0019605C
	public void PasteNaturalFormSnapshotFromFile(string path, TerrainData td, Bounds bounds)
	{
		if (td == null)
		{
			return;
		}
		List<TreeInstance> list = new List<TreeInstance>();
		BSGTerrainTools.Float2D float2D;
		BSGTerrainTools.Float3D float3D;
		using (FileStream fileStream = new FileStream(path + ".terrain.bytes", FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				float2D = BSGTerrainTools.Float2D.Load(binaryReader);
				float3D = BSGTerrainTools.Float3D.Load(binaryReader);
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					TreeInstance item = BSGTerrainTools.LoadTree(binaryReader);
					list.Add(item);
				}
			}
		}
		if (float2D == null || float3D == null)
		{
			return;
		}
		if (float2D != null)
		{
			Bounds bounds2 = BSGTerrainTools.SnapHeightsBounds(bounds, td);
			float2D.SetWorldRect(bounds2.min.x, bounds2.min.z, bounds2.size.x, bounds2.size.z);
		}
		if (float3D != null)
		{
			Bounds bounds3 = BSGTerrainTools.SnapSplatsBounds(bounds, td);
			float3D.SetWorldRect(bounds3.min.x, bounds3.min.z, bounds3.size.x, bounds3.size.z);
		}
		BSGTerrainTools.TerrainBlock terrainBlock = new BSGTerrainTools.TerrainBlock(td, bounds);
		terrainBlock.Import(true, true, true);
		for (int j = 0; j < float2D.arr_height; j++)
		{
			for (int k = 0; k < float2D.arr_width; k++)
			{
				float num2 = 1f - float3D.layers[0].arr[j, k];
				if (num2 > 0f)
				{
					float num3 = terrainBlock.heights[j, k];
					float num4 = float2D.arr[j, k];
					float num5 = (1f - num2) * num3 + num2 * num4;
					terrainBlock.heights[j, k] = num5;
					int num6 = 0;
					while (num6 < terrainBlock.splats.GetLength(2) || num6 < float3D.layers.Length)
					{
						num3 = terrainBlock.splats[j, k, num6];
						num4 = float3D.layers[num6].arr[j, k];
						num5 = (1f - num2) * num3 + num2 * num4;
						terrainBlock.splats[j, k, num6] = num5;
						num6++;
					}
					terrainBlock.splats[j, k, 0] = 0f;
					BSGTerrainTools.NormalizeSplatAlphas(terrainBlock.splats, k, j);
				}
			}
		}
		BSGTerrainTools.Float2D float2D2 = new BSGTerrainTools.Float2D(float3D.layers[0].arr);
		float2D2.SetWorldRect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
		BSGTerrainTools.Gen2D gen_alphas = new BSGTerrainTools.Gen2D(float2D2);
		List<TreeInstance> list2 = new List<TreeInstance>(terrainBlock.td.treeInstances);
		BSGTerrainTools.ExtractTrees(list2, terrainBlock.td.bounds, bounds, null, gen_alphas, this.snapshotTreesAlphaTreshold);
		for (int l = 0; l < list.Count; l++)
		{
			TreeInstance treeInstance = list[l];
			float num7 = treeInstance.position.x * bounds.size.x + bounds.min.x;
			treeInstance.position.x = num7 / td.bounds.size.x;
			float num8 = treeInstance.position.z * bounds.size.z + bounds.min.z;
			treeInstance.position.z = num8 / td.bounds.size.z;
			list[l] = treeInstance;
		}
		list2.AddRange(list);
		terrainBlock.trees = list2;
		terrainBlock.Apply(true, true, true);
	}

	// Token: 0x040021CC RID: 8652
	public float snapshotTreesAlphaTreshold = 0.5f;

	// Token: 0x040021CD RID: 8653
	public List<TestTerrainMerger.MergeShot> mergeShots = new List<TestTerrainMerger.MergeShot>();

	// Token: 0x040021CE RID: 8654
	[Header("Base________________________________________")]
	public TestTerrainMerger.MergeShot mergeBase;

	// Token: 0x040021CF RID: 8655
	[Header("Output______________________________________")]
	public TestTerrainMerger.MergeShot mergeOut;

	// Token: 0x02000887 RID: 2183
	[Serializable]
	public class MergeShot
	{
		// Token: 0x06005175 RID: 20853 RVA: 0x0023D8B8 File Offset: 0x0023BAB8
		public void Init()
		{
			this.block = new BSGTerrainTools.TerrainBlock(this.td, this.bounds);
			this.block.Import(true, true, true);
			this.f2dHeights = new BSGTerrainTools.Float2D(this.block.heights);
			this.heights_gen = new BSGTerrainTools.Gen2D(this.f2dHeights);
			BSGTerrainTools.Float3D f3d = new BSGTerrainTools.Float3D(this.block.splats);
			this.splats_gen = new BSGTerrainTools.Gen3D(f3d);
			this.tex = BSGTerrainTools.Float2D.ImportTexture(this.maskTexPath, 0);
			if (this.tex == null)
			{
				this.alphas_gen = new BSGTerrainTools.Gen2D(1f);
				this.trees_alphas_gen = new BSGTerrainTools.Gen2D(1f);
			}
			else
			{
				this.tex.SetWorldRect(this.bounds.min.x, this.bounds.min.z, this.bounds.size.x, this.bounds.size.z);
				this.alphas_gen = new BSGTerrainTools.Gen2D(this.tex);
				this.trees_alphas_gen = new BSGTerrainTools.Gen2D(this.tex);
			}
			this.extractedTrees = new List<TreeInstance>();
			BSGTerrainTools.ExtractTrees(this.block.trees, this.td.bounds, this.bounds, this.extractedTrees, this.trees_alphas_gen, this.treesMinAlpha);
		}

		// Token: 0x04003FDA RID: 16346
		public TerrainData td;

		// Token: 0x04003FDB RID: 16347
		[Tooltip("Leave it empty for no mask.")]
		public string maskTexPath;

		// Token: 0x04003FDC RID: 16348
		public Bounds bounds;

		// Token: 0x04003FDD RID: 16349
		public BSGTerrainTools.Gen2D heights_gen;

		// Token: 0x04003FDE RID: 16350
		public BSGTerrainTools.Gen2D alphas_gen;

		// Token: 0x04003FDF RID: 16351
		public BSGTerrainTools.Gen2D trees_alphas_gen;

		// Token: 0x04003FE0 RID: 16352
		public BSGTerrainTools.Gen3D splats_gen;

		// Token: 0x04003FE1 RID: 16353
		public BSGTerrainTools.TerrainBlock block;

		// Token: 0x04003FE2 RID: 16354
		public List<TreeInstance> extractedTrees = new List<TreeInstance>();

		// Token: 0x04003FE3 RID: 16355
		private BSGTerrainTools.Float2D tex;

		// Token: 0x04003FE4 RID: 16356
		private BSGTerrainTools.Float2D f2dHeights;

		// Token: 0x04003FE5 RID: 16357
		public float treesMinAlpha = 1f;
	}
}
