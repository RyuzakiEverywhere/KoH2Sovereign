using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200018A RID: 394
public class TerrainTypesBuilderAlternative
{
	// Token: 0x060015A9 RID: 5545 RVA: 0x000DCD05 File Offset: 0x000DAF05
	public static TerrainTypesBuilderAlternative Build()
	{
		TerrainTypesBuilderAlternative terrainTypesBuilderAlternative = new TerrainTypesBuilderAlternative();
		terrainTypesBuilderAlternative.BuildAll();
		return terrainTypesBuilderAlternative;
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x000DCD12 File Offset: 0x000DAF12
	public static TerrainTypesBuilderAlternative AnalyzeOnlyTrees()
	{
		TerrainTypesBuilderAlternative terrainTypesBuilderAlternative = new TerrainTypesBuilderAlternative();
		terrainTypesBuilderAlternative.BuildTreesData();
		return terrainTypesBuilderAlternative;
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x000DCD20 File Offset: 0x000DAF20
	private void BuildAll()
	{
		bool flag = false;
		try
		{
			if (!flag)
			{
				flag = !this.InitMaxRes();
			}
			if (!flag)
			{
				flag = !this.AnalyzeTrees();
			}
			if (!flag)
			{
				flag = !this.AnalyzeHeights();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			flag = true;
		}
		global::Common.EditorProgress(null, null, 0f, false);
		BuildTools.cancelled = flag;
		this.Cleanup();
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x000DCD8C File Offset: 0x000DAF8C
	private void BuildTreesData()
	{
		bool flag = false;
		try
		{
			if (!flag)
			{
				flag = !this.InitMaxRes();
			}
			if (!flag)
			{
				flag = !this.AnalyzeTrees();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			flag = true;
		}
		global::Common.EditorProgress(null, null, 0f, false);
		BuildTools.cancelled = flag;
		this.CleanTextures();
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x000DCDEC File Offset: 0x000DAFEC
	private bool InitMaxRes()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Initializing", 0f, true))
		{
			return false;
		}
		global::Settlement.SpawnAll();
		this.terrain = Terrain.activeTerrain;
		if (this.terrain == null)
		{
			return false;
		}
		this.shader = Shader.Find("Hidden/BSG/AudioRenderer");
		if (this.shader == null)
		{
			return false;
		}
		this.cam = global::Common.CreateTerrainCam(this.terrain);
		if (this.cam == null)
		{
			return false;
		}
		this.LoadDefs();
		this.rt = RenderTexture.GetTemporary(16384, 16384, 0);
		this.cam.targetTexture = this.rt;
		Vector3 size = this.terrain.terrainData.size;
		this.width = 512;
		this.height = 512;
		this.nodes = new TerrainTypesBuilderAlternative.Node[this.width, this.height];
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				this.nodes[j, i] = new TerrainTypesBuilderAlternative.Node();
			}
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			this.water_level = worldMap.TerrainHeights.WaterLevel;
			this.hills_delta_min = worldMap.TerrainHeights.HillsDeltaMin;
			this.hills_delta_max = worldMap.TerrainHeights.HillsDeltaMax;
		}
		Scene activeScene = SceneManager.GetActiveScene();
		this.path = Game.maps_path + Path.GetFileNameWithoutExtension(activeScene.path) + "/";
		return true;
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x000DCF7C File Offset: 0x000DB17C
	private void CleanTextures()
	{
		if (this.cam != null)
		{
			this.cam.targetTexture = null;
		}
		if (this.rt != null)
		{
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = null;
		}
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x000DCFB8 File Offset: 0x000DB1B8
	private void Cleanup()
	{
		this.CleanTextures();
		this.nodes = null;
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x000DCFC8 File Offset: 0x000DB1C8
	private void LoadDefs()
	{
		DT.Field defField = global::Defs.GetDefField("TerrainTypes", null);
		if (defField == null)
		{
			return;
		}
		this.tile_size = defField.GetFloat("tile_size", null, this.tile_size, true, true, true, '.');
		DT.Field field = defField.FindChild("thresholds", null, true, true, true, '.');
		if (field != null)
		{
			for (TerrainType terrainType = TerrainType.Plains; terrainType < TerrainType.COUNT; terrainType++)
			{
				int num = (int)terrainType;
				string key = terrainType.ToString();
				this.thresholds[num] = field.GetFloat(key, null, this.thresholds[num], true, true, true, '.');
			}
		}
		DT.Field field2 = defField.FindChild("colors", null, true, true, true, '.');
		if (field2 != null)
		{
			for (TerrainType terrainType2 = TerrainType.Plains; terrainType2 < TerrainType.COUNT; terrainType2++)
			{
				int num2 = (int)terrainType2;
				string key2 = terrainType2.ToString();
				this.colors[num2] = global::Defs.GetColor(field2, key2, this.colors[num2], null);
			}
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x000DD0B0 File Offset: 0x000DB2B0
	private bool AnalyzeHeights()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Processing heights", 0f, true))
		{
			return false;
		}
		TerrainData terrainData = this.terrain.terrainData;
		int heightmapResolution = terrainData.heightmapResolution;
		for (int i = 0; i < heightmapResolution; i++)
		{
			int num = i * this.height / heightmapResolution;
			for (int j = 0; j < heightmapResolution; j++)
			{
				int num2 = j * this.width / heightmapResolution;
				float num3 = terrainData.GetHeight(j, i);
				if (num3 < 0.01f)
				{
					num3 = 0.01f;
				}
				TerrainTypesBuilderAlternative.Node node = this.nodes[num2, num];
				node.Add(TerrainType.Ocean, num3 <= this.water_level);
				for (int k = i - 1; k <= i + 1; k++)
				{
					for (int l = j - 1; l <= j + 1; l++)
					{
						if (l != j || k != i)
						{
							float num4 = terrainData.GetHeight(l, k);
							float num5 = num3 - num4;
							if (num5 > this.hills_delta_min && num5 < this.hills_delta_max && node.counts[3] == 0)
							{
								node.Add(TerrainType.Hills, true);
							}
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x000DD1DC File Offset: 0x000DB3DC
	private bool AnalyzeTrees()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Processing Trees", 0f, true))
		{
			return false;
		}
		TreeInstance[] treeInstances = this.terrain.terrainData.treeInstances;
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		for (int i = 0; i < this.terrain.terrainData.treePrototypes.Length; i++)
		{
			TreePrototype treePrototype = this.terrain.terrainData.treePrototypes[i];
			MeshRenderer[] componentsInChildren = treePrototype.prefab.GetComponentsInChildren<MeshRenderer>();
			string text = treePrototype.prefab.name.ToLowerInvariant();
			bool flag = text.Contains("grass") || text.Contains("bush");
			if (!flag)
			{
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j].sharedMaterial.shader.name.ToLowerInvariant().Contains("grass"))
					{
						flag = true;
						break;
					}
				}
			}
			dictionary[i] = !flag;
		}
		foreach (TreeInstance treeInstance in treeInstances)
		{
			if (dictionary[treeInstance.prototypeIndex])
			{
				Vector3 position = treeInstance.position;
				int num = (int)(position.x * (float)this.width);
				int num2 = (int)(position.z * (float)this.height);
				if (num >= 0 && num2 >= 0 && num < this.width && num2 < this.width)
				{
					this.nodes[num, num2].Add(TerrainType.Forest, true);
				}
			}
		}
		return true;
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x000DD360 File Offset: 0x000DB560
	private void Render(string layer, Color clr, bool over_terrain_only = true, bool clear = false)
	{
		Shader.SetGlobalInt("_ARPass", over_terrain_only ? 0 : 3);
		Shader.SetGlobalColor("_ARColor", clr);
		this.cam.clearFlags = (clear ? CameraClearFlags.Color : CameraClearFlags.Nothing);
		this.cam.cullingMask = 1 << LayerMask.NameToLayer(layer);
		this.cam.RenderWithShader(this.shader, "");
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x000DD3C8 File Offset: 0x000DB5C8
	[Obsolete]
	private bool RenderBig()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Rendering masks", 0f, true))
		{
			return false;
		}
		this.Render("Mountains", new Color(1f, 0f, 0f), true, false);
		this.Render("Rivers", new Color(0f, 1f, 0f), false, false);
		this.Render("Settlements", new Color(0f, 0f, 1f), false, false);
		int i = 16384;
		while (i > 4096)
		{
			i /= 2;
			RenderTexture temporary = RenderTexture.GetTemporary(i, i, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			Graphics.Blit(this.rt, temporary);
			RenderTexture.ReleaseTemporary(this.rt);
			this.rt = temporary;
		}
		if (!global::Common.EditorProgress("Build Terrain Types", "Downloading masks texture", 0f, true))
		{
			return false;
		}
		Texture2D texture2D = new Texture2D(4096, 4096, TextureFormat.RGB24, false);
		RenderTexture.active = this.rt;
		texture2D.ReadPixels(new Rect(0f, 0f, 4096f, 4096f), 0, 0);
		RenderTexture.active = null;
		if (!global::Common.EditorProgress("Build Terrain Types", "Analying masks", 0f, true))
		{
			return false;
		}
		Vector3 size = this.terrain.terrainData.size;
		int num = 0;
		Color[] pixels = texture2D.GetPixels();
		for (int j = num; j < 4096 - num; j++)
		{
			int num2 = (j - num) * this.height / (4096 - 2 * num);
			for (int k = 0; k < 4096; k++)
			{
				Color color = pixels[j * 4096 + k];
				int num3 = k * this.width / 4096;
				TerrainTypesBuilderAlternative.Node node = this.nodes[num3, num2];
				node.Add(TerrainType.Mountains, color.r > 0f);
				node.Add(TerrainType.River, color.g > 0f);
				node.Add(TerrainType.Town, color.b > 0f);
			}
		}
		return true;
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x000DD5DC File Offset: 0x000DB7DC
	[Obsolete]
	private void SaveTexBig()
	{
		Color[] array = new Color[this.width * this.height];
		int num = 0;
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				TerrainTypesBuilderAlternative.Node node = this.nodes[j, i];
				Color color = new Color(0f, 0f, 0f, 0f);
				if (node.counts[1] > 0)
				{
					color.r = 1f;
				}
				if (node.counts[7] > 0)
				{
					color.g = 1f;
				}
				if (node.counts[2] > 0)
				{
					color.b = 1f;
				}
				if (node.counts[4] > 0)
				{
					color.a = 1f;
				}
				array[num++] = color;
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false);
		texture2D.SetPixels(array);
		File.WriteAllBytes(this.path + "terrain_types_big.png", texture2D.EncodeToPNG());
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x000DD6F8 File Offset: 0x000DB8F8
	public bool HasType(Point pos, TerrainType type)
	{
		TerrainData terrainData = this.terrain.terrainData;
		int num = (int)(pos.x * (float)this.width / terrainData.size.x);
		int num2 = (int)(pos.y * (float)this.height / terrainData.size.z);
		return num >= 0 && num2 >= 0 && num < this.width && num2 < this.width && this.nodes[num, num2].totals[(int)type] > 0;
	}

	// Token: 0x04000E08 RID: 3592
	private float tile_size = 5f;

	// Token: 0x04000E09 RID: 3593
	private float[] thresholds = new float[]
	{
		0f,
		0.5f,
		0f,
		0.25f,
		0.5f,
		0.75f,
		0.75f,
		0.75f
	};

	// Token: 0x04000E0A RID: 3594
	private Color[] colors = new Color[]
	{
		new Color(0f, 0.5f, 0f),
		new Color(0.5f, 0.5f, 0f),
		new Color(0.5f, 0.3f, 0.1f),
		new Color(0f, 1f, 1f),
		new Color(1f, 0f, 1f),
		new Color(0f, 0.5f, 0.5f),
		new Color(0f, 0f, 1f),
		new Color(1f, 1f, 1f)
	};

	// Token: 0x04000E0B RID: 3595
	private int width;

	// Token: 0x04000E0C RID: 3596
	private int height;

	// Token: 0x04000E0D RID: 3597
	private TerrainTypesBuilderAlternative.Node[,] nodes;

	// Token: 0x04000E0E RID: 3598
	private Terrain terrain;

	// Token: 0x04000E0F RID: 3599
	private float water_level;

	// Token: 0x04000E10 RID: 3600
	private float hills_delta_min;

	// Token: 0x04000E11 RID: 3601
	private float hills_delta_max;

	// Token: 0x04000E12 RID: 3602
	private Camera cam;

	// Token: 0x04000E13 RID: 3603
	private RenderTexture rt;

	// Token: 0x04000E14 RID: 3604
	private const int rt_res = 16384;

	// Token: 0x04000E15 RID: 3605
	private const int tex_res = 4096;

	// Token: 0x04000E16 RID: 3606
	private Shader shader;

	// Token: 0x04000E17 RID: 3607
	private string path;

	// Token: 0x020006C3 RID: 1731
	private class Node
	{
		// Token: 0x06004889 RID: 18569 RVA: 0x00217FB1 File Offset: 0x002161B1
		public void Add(TerrainType type, bool present)
		{
			this.totals[(int)type]++;
			if (present)
			{
				this.counts[(int)type]++;
			}
		}

		// Token: 0x040036E4 RID: 14052
		public TerrainType type;

		// Token: 0x040036E5 RID: 14053
		public int[] counts = new int[8];

		// Token: 0x040036E6 RID: 14054
		public int[] totals = new int[8];
	}
}
