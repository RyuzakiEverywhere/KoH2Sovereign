using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000189 RID: 393
public class TerrainTypesBuilder
{
	// Token: 0x06001597 RID: 5527 RVA: 0x000DBFAD File Offset: 0x000DA1AD
	public static TerrainTypesBuilder Build()
	{
		TerrainTypesBuilder terrainTypesBuilder = new TerrainTypesBuilder();
		terrainTypesBuilder.BuildAll();
		return terrainTypesBuilder;
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x000DBFBA File Offset: 0x000DA1BA
	public static TerrainTypesBuilder AnalyzeOnlyTrees()
	{
		TerrainTypesBuilder terrainTypesBuilder = new TerrainTypesBuilder();
		terrainTypesBuilder.BuildTreesData();
		return terrainTypesBuilder;
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000DBFC8 File Offset: 0x000DA1C8
	private void BuildAll()
	{
		bool flag = false;
		try
		{
			if (!flag)
			{
				flag = !this.Init();
			}
			if (!flag)
			{
				flag = !this.AnalyzeHeights();
			}
			if (!flag)
			{
				flag = !this.AnalyzeTrees();
			}
			if (!flag)
			{
				flag = !this.Render();
			}
			if (!flag)
			{
				flag = !this.Calc();
			}
			if (!flag)
			{
				this.Save();
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

	// Token: 0x0600159A RID: 5530 RVA: 0x000DC058 File Offset: 0x000DA258
	private void BuildTreesData()
	{
		bool flag = false;
		try
		{
			if (!flag)
			{
				flag = !this.Init();
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

	// Token: 0x0600159B RID: 5531 RVA: 0x000DC0B8 File Offset: 0x000DA2B8
	private bool Init()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Initializing", 0f, true))
		{
			return false;
		}
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
		this.width = (int)(size.x / this.tile_size);
		this.height = (int)(size.z / this.tile_size);
		this.nodes = new TerrainTypesBuilder.Node[this.width, this.height];
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				this.nodes[j, i] = new TerrainTypesBuilder.Node();
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

	// Token: 0x0600159C RID: 5532 RVA: 0x000DC25A File Offset: 0x000DA45A
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

	// Token: 0x0600159D RID: 5533 RVA: 0x000DC296 File Offset: 0x000DA496
	private void Cleanup()
	{
		this.nodes = null;
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x000DC2A0 File Offset: 0x000DA4A0
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
		DT.Field field2 = defField.FindChild("thresholds_min", null, true, true, true, '.');
		if (field2 != null)
		{
			for (TerrainType terrainType2 = TerrainType.Plains; terrainType2 < TerrainType.COUNT; terrainType2++)
			{
				int num2 = (int)terrainType2;
				string key2 = terrainType2.ToString();
				this.thresholds_min[num2] = field2.GetFloat(key2, null, this.thresholds_min[num2], true, true, true, '.');
			}
		}
		DT.Field field3 = defField.FindChild("colors", null, true, true, true, '.');
		if (field3 != null)
		{
			for (TerrainType terrainType3 = TerrainType.Plains; terrainType3 < TerrainType.COUNT; terrainType3++)
			{
				int num3 = (int)terrainType3;
				string key3 = terrainType3.ToString();
				this.colors[num3] = global::Defs.GetColor(field3, key3, this.colors[num3], null);
			}
		}
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x000DC3E8 File Offset: 0x000DA5E8
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
				TerrainTypesBuilder.Node node = this.nodes[num2, num];
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

	// Token: 0x060015A0 RID: 5536 RVA: 0x000DC514 File Offset: 0x000DA714
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

	// Token: 0x060015A1 RID: 5537 RVA: 0x000DC698 File Offset: 0x000DA898
	private void Render(string layer, Color clr, bool over_terrain_only = true, bool clear = false)
	{
		Shader.SetGlobalInt("_ARPass", over_terrain_only ? 0 : 3);
		Shader.SetGlobalColor("_ARColor", clr);
		this.cam.clearFlags = (clear ? CameraClearFlags.Color : CameraClearFlags.Nothing);
		this.cam.cullingMask = 1 << LayerMask.NameToLayer(layer);
		this.cam.RenderWithShader(this.shader, "");
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x000DC700 File Offset: 0x000DA900
	private bool Render()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Rendering masks", 0f, true))
		{
			return false;
		}
		this.Render("Lakes", new Color(0f, 0f, 1f, 0f), true, true);
		this.Render("Mountains", new Color(1f, 0f, 0f, 0f), true, false);
		this.Render("Rivers", new Color(0f, 1f, 0f, 0f), false, false);
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
				TerrainTypesBuilder.Node node = this.nodes[num3, num2];
				node.Add(TerrainType.Mountains, color.r > 0f);
				node.Add(TerrainType.River, color.g > 0f);
				node.Add(TerrainType.Lake, color.b > 0f);
			}
		}
		return true;
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x000DC924 File Offset: 0x000DAB24
	private bool Calc()
	{
		if (!global::Common.EditorProgress("Build Terrain Types", "Calculating thresholds", 0f, true))
		{
			return false;
		}
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				TerrainTypesBuilder.Node node = this.nodes[j, i];
				for (int k = 7; k > 0; k--)
				{
					int num = node.counts[k];
					if (num > 0)
					{
						int num2 = node.totals[k];
						if ((num2 <= 0 || (float)num / (float)num2 >= this.thresholds[k]) && (float)num >= this.thresholds_min[k])
						{
							node.type = (TerrainType)k;
							break;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x000DC9CC File Offset: 0x000DABCC
	private void SaveTex()
	{
		Color[] array = new Color[this.width * this.height];
		int num = 0;
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				TerrainTypesBuilder.Node node = this.nodes[j, i];
				Color color = this.colors[(int)node.type];
				array[num++] = color;
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height);
		texture2D.SetPixels(array);
		File.WriteAllBytes(this.path + "terrain_types.png", texture2D.EncodeToPNG());
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x000DCA7C File Offset: 0x000DAC7C
	private void SaveBin()
	{
		using (FileStream fileStream = File.OpenWrite(this.path + "terrain_types.bin"))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				binaryWriter.Write(this.width);
				binaryWriter.Write(this.height);
				for (int i = 0; i < this.height; i++)
				{
					for (int j = 0; j < this.width; j++)
					{
						byte value = (byte)this.nodes[j, i].type;
						binaryWriter.Write(value);
					}
				}
			}
		}
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x000DCB30 File Offset: 0x000DAD30
	private void Save()
	{
		global::Common.EditorProgress("Build Terrain Types", "Saving", 0f, false);
		this.SaveBin();
		this.SaveTex();
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x000DCB54 File Offset: 0x000DAD54
	public bool HasType(Point pos, TerrainType type)
	{
		TerrainData terrainData = this.terrain.terrainData;
		int num = (int)(pos.x * (float)this.width / terrainData.size.x);
		int num2 = (int)(pos.y * (float)this.height / terrainData.size.z);
		return num >= 0 && num2 >= 0 && num < this.width && num2 < this.width && this.nodes[num, num2].totals[(int)type] > 0;
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x000DCBD8 File Offset: 0x000DADD8
	public TerrainTypesBuilder()
	{
		float[] array = new float[8];
		array[2] = 4f;
		this.thresholds_min = array;
		this.colors = new Color[]
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
		base..ctor();
	}

	// Token: 0x04000DF7 RID: 3575
	private float tile_size = 5f;

	// Token: 0x04000DF8 RID: 3576
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

	// Token: 0x04000DF9 RID: 3577
	private float[] thresholds_min;

	// Token: 0x04000DFA RID: 3578
	private Color[] colors;

	// Token: 0x04000DFB RID: 3579
	private int width;

	// Token: 0x04000DFC RID: 3580
	private int height;

	// Token: 0x04000DFD RID: 3581
	private TerrainTypesBuilder.Node[,] nodes;

	// Token: 0x04000DFE RID: 3582
	private Terrain terrain;

	// Token: 0x04000DFF RID: 3583
	private float water_level;

	// Token: 0x04000E00 RID: 3584
	private float hills_delta_min;

	// Token: 0x04000E01 RID: 3585
	private float hills_delta_max;

	// Token: 0x04000E02 RID: 3586
	private Camera cam;

	// Token: 0x04000E03 RID: 3587
	private RenderTexture rt;

	// Token: 0x04000E04 RID: 3588
	private const int rt_res = 16384;

	// Token: 0x04000E05 RID: 3589
	private const int tex_res = 4096;

	// Token: 0x04000E06 RID: 3590
	private Shader shader;

	// Token: 0x04000E07 RID: 3591
	private string path;

	// Token: 0x020006C2 RID: 1730
	private class Node
	{
		// Token: 0x06004887 RID: 18567 RVA: 0x00217F6A File Offset: 0x0021616A
		public void Add(TerrainType type, bool present)
		{
			this.totals[(int)type]++;
			if (present)
			{
				this.counts[(int)type]++;
			}
		}

		// Token: 0x040036E1 RID: 14049
		public TerrainType type;

		// Token: 0x040036E2 RID: 14050
		public int[] counts = new int[8];

		// Token: 0x040036E3 RID: 14051
		public int[] totals = new int[8];
	}
}
