using System;
using System.Collections.Generic;
using System.Diagnostics;
using BSG;
using UnityEngine;

// Token: 0x02000338 RID: 824
public class MapMixer : MonoBehaviour
{
	// Token: 0x06003257 RID: 12887 RVA: 0x0019839C File Offset: 0x0019659C
	public void Refresh()
	{
		this.main_terr = base.GetComponent<Terrain>();
		if (this.main_terr == null)
		{
			Debug.LogError("Terrain object is NULL!");
			base.enabled = false;
			return;
		}
		for (int i = 0; i < this.meta.Count; i++)
		{
			if (this.meta[i] == null || this.meta[i].terr == null)
			{
				Debug.LogError("Meta Terrain info needs to have no NULL terrains, etc..");
			}
			else if (this.meta[i].terr == this.main_terr)
			{
				Debug.LogError("Meta Terrain can't be part of meta terrains!");
			}
			else
			{
				this.meta[i].width = this.meta[i].terr.terrainData.alphamapWidth;
				this.meta[i].height = this.meta[i].terr.terrainData.alphamapHeight;
			}
		}
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x001984AC File Offset: 0x001966AC
	public void InitLayersFromMain()
	{
		TerrainLayer[] terrainLayers = this.main_terr.terrainData.terrainLayers;
		if (terrainLayers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < terrainLayers.Length; i++)
		{
			for (int j = 0; j < this.meta.Count; j++)
			{
				this.AddLayerToTerrain(this.meta[j].terr, terrainLayers[i]);
			}
		}
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x00198510 File Offset: 0x00196710
	public int GetLayerIndex(Terrain terr, TerrainLayer layer)
	{
		TerrainLayer[] terrainLayers = terr.terrainData.terrainLayers;
		for (int i = 0; i < terrainLayers.Length; i++)
		{
			if (terrainLayers[i] == layer)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x00198548 File Offset: 0x00196748
	public int GetLayerIndex(List<TerrainLayer> layers, TerrainLayer layer)
	{
		if (layers == null || layers.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < layers.Count; i++)
		{
			if (layers[i] == layer)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600325B RID: 12891 RVA: 0x00198588 File Offset: 0x00196788
	public int GetTreeIndex(List<TreePrototype> trees, TreePrototype tree)
	{
		if (trees == null || trees.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < trees.Count; i++)
		{
			if (trees[i].Equals(tree))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x001985C8 File Offset: 0x001967C8
	public int AddLayerToTerrain(Terrain terr, TerrainLayer layer)
	{
		int layerIndex = this.GetLayerIndex(terr, layer);
		if (layerIndex == -1)
		{
			TerrainLayer[] terrainLayers = terr.terrainData.terrainLayers;
			TerrainLayer[] array = new TerrainLayer[terrainLayers.Length + 1];
			Array.Copy(terrainLayers, 0, array, 0, terrainLayers.Length);
			array[terrainLayers.Length] = layer;
			terr.terrainData.terrainLayers = array;
			return array.Length - 1;
		}
		return layerIndex;
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x00198620 File Offset: 0x00196820
	private void AddMetaLayersToPalette(List<TerrainLayer> palette, MapMixer.MetaTerrain mt)
	{
		mt.layer_map = new int[mt.terr.terrainData.terrainLayers.Length];
		TerrainLayer[] terrainLayers = mt.terr.terrainData.terrainLayers;
		for (int i = 0; i < mt.layer_map.Length; i++)
		{
			int num = this.GetLayerIndex(palette, terrainLayers[i]);
			if (num == -1)
			{
				palette.Add(terrainLayers[i]);
				num = palette.Count - 1;
			}
			mt.layer_map[i] = num;
		}
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x00198698 File Offset: 0x00196898
	private void AddMetaTreesToPrototypes(List<TreePrototype> trees, MapMixer.MetaTerrain mt)
	{
		mt.tree_map = new int[mt.terr.terrainData.treePrototypes.Length];
		TreePrototype[] treePrototypes = mt.terr.terrainData.treePrototypes;
		for (int i = 0; i < mt.tree_map.Length; i++)
		{
			int num = this.GetTreeIndex(trees, treePrototypes[i]);
			if (num == -1)
			{
				trees.Add(treePrototypes[i]);
				num = trees.Count - 1;
			}
			mt.tree_map[i] = num;
		}
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x00198710 File Offset: 0x00196910
	public void BuildMainPalette()
	{
		List<TerrainLayer> list = new List<TerrainLayer>();
		list.Add(this.main_terr.terrainData.terrainLayers[0]);
		for (int i = 0; i < this.meta.Count; i++)
		{
			this.AddMetaLayersToPalette(list, this.meta[i]);
		}
		this.main_terr.terrainData.terrainLayers = list.ToArray();
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x0019877C File Offset: 0x0019697C
	public void BuildMainVegetation()
	{
		List<TreePrototype> list = new List<TreePrototype>();
		for (int i = 0; i < this.meta.Count; i++)
		{
			this.AddMetaTreesToPrototypes(list, this.meta[i]);
		}
		this.main_terr.terrainData.treePrototypes = list.ToArray();
		this.main_terr.terrainData.RefreshPrototypes();
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x001987E0 File Offset: 0x001969E0
	public float GetNoise(int x, int y, int w, int h, float offs, float scale)
	{
		float x2 = (float)x / (float)w * scale + offs;
		float y2 = (float)y / (float)h * scale + offs;
		return Mathf.Clamp01(Mathf.PerlinNoise(x2, y2));
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x00198810 File Offset: 0x00196A10
	public void NormalizeAlphas(float[,,] map, int w, int h, int count)
	{
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				float num = 0f;
				for (int k = 0; k < count; k++)
				{
					num += map[i, j, k];
				}
				for (int l = 0; l < count; l++)
				{
					map[i, j, l] /= num;
				}
			}
		}
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x0019887C File Offset: 0x00196A7C
	public void ResetMainZeroLayer(float value = 0f)
	{
		float[,,] alphamaps = this.main_terr.terrainData.GetAlphamaps(0, 0, this.main_terr.terrainData.alphamapWidth, this.main_terr.terrainData.alphamapHeight);
		int alphamapWidth = this.main_terr.terrainData.alphamapWidth;
		int alphamapHeight = this.main_terr.terrainData.alphamapHeight;
		int alphamapLayers = this.main_terr.terrainData.alphamapLayers;
		for (int i = 0; i < alphamapWidth; i++)
		{
			for (int j = 0; j < alphamapHeight; j++)
			{
				for (int k = 0; k < alphamapLayers; k++)
				{
					alphamaps[i, j, k] = ((k == 0) ? value : 0f);
				}
			}
		}
		this.main_terr.terrainData.SetAlphamaps(0, 0, alphamaps);
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x0019894C File Offset: 0x00196B4C
	public void ApplyMeta(int index)
	{
		float noiseOffs = this.meta[index].noiseOffs;
		int alphamapWidth = this.main_terr.terrainData.alphamapWidth;
		int alphamapHeight = this.main_terr.terrainData.alphamapHeight;
		Vector3 size = this.main_terr.terrainData.size;
		int width = this.meta[index].width;
		int height = this.meta[index].height;
		float[,,] alphamaps = this.main_terr.terrainData.GetAlphamaps(0, 0, this.main_terr.terrainData.alphamapWidth, this.main_terr.terrainData.alphamapHeight);
		float[,,] alphamaps2 = this.meta[index].terr.terrainData.GetAlphamaps(0, 0, this.meta[index].terr.terrainData.alphamapWidth, this.meta[index].terr.terrainData.alphamapHeight);
		float[,] heights = this.main_terr.terrainData.GetHeights(0, 0, alphamapHeight + 1, alphamapWidth + 1);
		float[,] heights2 = this.meta[index].terr.terrainData.GetHeights(0, 0, height + 1, width + 1);
		int alphamapLayers = this.main_terr.terrainData.alphamapLayers;
		bool useNoise = this.meta[index].useNoise;
		float[,] array;
		if (useNoise)
		{
			array = new float[alphamapWidth + 1, alphamapHeight + 1];
			for (int i = 0; i < alphamapWidth + 1; i++)
			{
				for (int j = 0; j < alphamapHeight + 1; j++)
				{
					float noise = this.GetNoise(i, j, alphamapWidth, alphamapHeight, noiseOffs, this.meta[index].noiseScale);
					array[i, j] = Levels.Calc(noise, this.meta[index].inMin, this.meta[index].noiseGamma, this.meta[index].inMax, this.meta[index].outMin, this.meta[index].outMax);
				}
			}
		}
		else
		{
			array = new float[1, 1];
			array[0, 0] = 1f;
		}
		for (int k = 0; k < alphamapWidth; k++)
		{
			for (int l = 0; l < alphamapHeight; l++)
			{
				int num = k % width;
				int num2 = l % height;
				if (!useNoise)
				{
					for (int m = 0; m < alphamapLayers; m++)
					{
						alphamaps[k, l, m] = 0f;
					}
				}
				for (int n = 0; n < this.meta[index].layer_map.Length; n++)
				{
					int num3 = this.meta[index].layer_map[n];
					float a = alphamaps[k, l, num3];
					float b = alphamaps2[num, num2, n];
					float t = useNoise ? array[k, l] : array[0, 0];
					alphamaps[k, l, num3] = Mathf.Lerp(a, b, t);
				}
			}
		}
		this.NormalizeAlphas(alphamaps, alphamapWidth, alphamapHeight, alphamapLayers);
		this.main_terr.terrainData.SetAlphamaps(0, 0, alphamaps);
		int heightmapResolution = this.main_terr.terrainData.heightmapResolution;
		if (this.meta[index].blendHeights)
		{
			for (int num4 = 0; num4 < alphamapWidth + 1; num4++)
			{
				for (int num5 = 0; num5 < alphamapHeight + 1; num5++)
				{
					int num6 = num4 % (width + 1);
					int num7 = num5 % (height + 1);
					float b2 = Mathf.Clamp01(heights2[num7, num6] * this.meta[index].heightMul - this.meta[index].heightSub);
					float a2 = heights[num5, num4];
					float t2 = useNoise ? array[num5, num4] : array[0, 0];
					heights[num5, num4] = Mathf.Lerp(a2, b2, t2);
				}
			}
			this.main_terr.terrainData.SetHeightsDelayLOD(0, 0, heights);
		}
		if (this.meta[index].type == MapMixer.MetaTerrainType.Forest)
		{
			List<TreeInstance> list = new List<TreeInstance>();
			int treeInstanceCount = this.meta[index].terr.terrainData.treeInstanceCount;
			int num8 = alphamapWidth / width;
			int num9 = alphamapHeight / height;
			for (int num10 = 0; num10 < treeInstanceCount; num10++)
			{
				TreeInstance treeInstance = this.meta[index].terr.terrainData.GetTreeInstance(num10);
				float num11 = treeInstance.position.x * (float)width;
				float num12 = treeInstance.position.z * (float)height;
				for (int num13 = 0; num13 < num8; num13++)
				{
					for (int num14 = 0; num14 < num9; num14++)
					{
						float num15 = num11 + (float)num13 * (float)width;
						float num16 = num12 + (float)num14 * (float)height;
						if ((useNoise ? array[(int)num16, (int)num15] : array[0, 0]) > 0.75f)
						{
							treeInstance.position.x = num15 / (float)alphamapWidth;
							treeInstance.position.z = num16 / (float)alphamapHeight;
							if (this.main_terr.terrainData.GetInterpolatedHeight(treeInstance.position.x, treeInstance.position.z) > this.WV_OceanLevel && this.main_terr.terrainData.GetSteepness(treeInstance.position.x, treeInstance.position.z) <= this.meta[index].maxSlope)
							{
								treeInstance.prototypeIndex = this.meta[index].tree_map[treeInstance.prototypeIndex];
								list.Add(treeInstance);
							}
						}
					}
				}
			}
			this.main_terr.terrainData.SetTreeInstances(list.ToArray(), true);
		}
		this.main_terr.terrainData.SyncHeightmap();
	}

	// Token: 0x06003265 RID: 12901 RVA: 0x00198F7C File Offset: 0x0019717C
	public void Generate()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		this.BuildMainPalette();
		this.BuildMainVegetation();
		this.ResetMainZeroLayer(0f);
		this.ApplyMeta(0);
		this.ApplyMeta(1);
		this.ApplyMeta(2);
		if (this.trees_batching != null)
		{
			this.trees_batching.Rebuild();
		}
		float num = (float)stopwatch.ElapsedMilliseconds / 1000f;
		Debug.Log("Map generated in " + num);
	}

	// Token: 0x06003266 RID: 12902 RVA: 0x00198FF5 File Offset: 0x001971F5
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x040021DD RID: 8669
	public SettlementBV town;

	// Token: 0x040021DE RID: 8670
	public TreesBatching trees_batching;

	// Token: 0x040021DF RID: 8671
	public Terrain main_terr;

	// Token: 0x040021E0 RID: 8672
	[SerializeField]
	public List<MapMixer.MetaTerrain> meta;

	// Token: 0x040021E1 RID: 8673
	public Texture2D eu_height_tex;

	// Token: 0x040021E2 RID: 8674
	public Texture2D eu_sdf_tex;

	// Token: 0x040021E3 RID: 8675
	public float WV_OceanLevel = 10f;

	// Token: 0x040021E4 RID: 8676
	public int wv_chunk_size = 128;

	// Token: 0x040021E5 RID: 8677
	private float[,] eu_heights;

	// Token: 0x040021E6 RID: 8678
	public bool randomize = true;

	// Token: 0x040021E7 RID: 8679
	public int seed = 777;

	// Token: 0x0200088B RID: 2187
	[Serializable]
	public class MetaTerrain
	{
		// Token: 0x04003FEF RID: 16367
		public Terrain terr;

		// Token: 0x04003FF0 RID: 16368
		public int[] layer_map;

		// Token: 0x04003FF1 RID: 16369
		public int[] tree_map;

		// Token: 0x04003FF2 RID: 16370
		public int width;

		// Token: 0x04003FF3 RID: 16371
		public int height;

		// Token: 0x04003FF4 RID: 16372
		public MapMixer.MetaTerrainType type;

		// Token: 0x04003FF5 RID: 16373
		public int grid_size = 32;

		// Token: 0x04003FF6 RID: 16374
		public bool useNoise;

		// Token: 0x04003FF7 RID: 16375
		[Range(0.1f, 10000.5f)]
		public float noiseOffs = 0.5f;

		// Token: 0x04003FF8 RID: 16376
		[Range(0.1f, 100f)]
		public float noiseScale = 0.5f;

		// Token: 0x04003FF9 RID: 16377
		[Range(0f, 1f)]
		public float inMin;

		// Token: 0x04003FFA RID: 16378
		[Range(-1f, 1f)]
		public float noiseGamma;

		// Token: 0x04003FFB RID: 16379
		[Range(0f, 1f)]
		public float inMax = 1f;

		// Token: 0x04003FFC RID: 16380
		[Range(0f, 1f)]
		public float outMin;

		// Token: 0x04003FFD RID: 16381
		[Range(0f, 1f)]
		public float outMax = 1f;

		// Token: 0x04003FFE RID: 16382
		public bool blendHeights;

		// Token: 0x04003FFF RID: 16383
		[Range(0f, 2f)]
		public float heightMul = 1f;

		// Token: 0x04004000 RID: 16384
		[Range(0f, 1f)]
		public float heightSub;

		// Token: 0x04004001 RID: 16385
		public bool applyForest;

		// Token: 0x04004002 RID: 16386
		[Range(0f, 90f)]
		public float maxSlope = 25f;
	}

	// Token: 0x0200088C RID: 2188
	public enum MetaTerrainType
	{
		// Token: 0x04004004 RID: 16388
		Forest,
		// Token: 0x04004005 RID: 16389
		Heights,
		// Token: 0x04004006 RID: 16390
		Shore,
		// Token: 0x04004007 RID: 16391
		Locations
	}
}
