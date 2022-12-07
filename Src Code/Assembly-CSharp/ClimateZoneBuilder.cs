using System;
using System.IO;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200018B RID: 395
public class ClimateZoneBuilder
{
	// Token: 0x060015B8 RID: 5560 RVA: 0x000DD895 File Offset: 0x000DBA95
	public static ClimateZoneBuilder Build()
	{
		ClimateZoneBuilder climateZoneBuilder = new ClimateZoneBuilder();
		climateZoneBuilder.BuildAll();
		return climateZoneBuilder;
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x000DD8A4 File Offset: 0x000DBAA4
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

	// Token: 0x060015BA RID: 5562 RVA: 0x000DD90C File Offset: 0x000DBB0C
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
		this.LoadDefs();
		this.LoadSourceTexure();
		Vector3 size = this.terrain.terrainData.size;
		this.width = (int)(size.x / this.tile_size);
		this.height = (int)(size.z / this.tile_size);
		this.nodes = new ClimateZoneBuilder.Node[this.width, this.height];
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				this.nodes[j, i] = new ClimateZoneBuilder.Node();
			}
		}
		Scene activeScene = SceneManager.GetActiveScene();
		this.path = Game.maps_path + Path.GetFileNameWithoutExtension(activeScene.path) + "/";
		return true;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000DDA08 File Offset: 0x000DBC08
	private void LoadDefs()
	{
		DT.Field defField = global::Defs.GetDefField("ClimateZones", null);
		if (defField == null)
		{
			return;
		}
		this.tile_size = defField.GetFloat("tile_size", null, this.tile_size, true, true, true, '.');
		DT.Field field = defField.FindChild("colors", null, true, true, true, '.');
		if (field != null)
		{
			for (ClimateZoneType climateZoneType = ClimateZoneType.Tropical; climateZoneType < ClimateZoneType.COUNT; climateZoneType++)
			{
				int num = (int)climateZoneType;
				string key = climateZoneType.ToString();
				this.colors[num] = global::Defs.GetColor(field, key, this.colors[num], null);
			}
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x000DDAA0 File Offset: 0x000DBCA0
	private void LoadSourceTexure()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		string text = Path.GetDirectoryName(activeScene.path).Replace('\\', '/') + "/" + activeScene.name + "_climatezones_source.png";
		this.sourceTexture = Assets.Get<Texture2D>(text);
		if (this.sourceTexture == null)
		{
			Debug.LogWarning(string.Format("Missing Climate Zone source data at path: <b><color=red>{0}</color></b>. All nodes will be exported as {1}", text, ClimateZoneType.Temperate));
		}
		if (this.sourceTexture != null && !this.sourceTexture.isReadable)
		{
			Debug.LogWarning("Climate Zone source data at path: <b><color=red>" + text + "</color></b>. Is not readable.Pleace, make the texture readable in the Texture Import Settings. ");
		}
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x000DDB40 File Offset: 0x000DBD40
	private bool Calc()
	{
		if (!global::Common.EditorProgress("Build Climate Zones", "Calculating thresholds", 0f, true))
		{
			return false;
		}
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				ClimateZoneBuilder.Node node = this.nodes[j, i];
				if (this.sourceTexture != null && this.sourceTexture.isReadable)
				{
					Vector2 vector = new Vector2((float)j / (float)this.width, (float)i / (float)this.height);
					node.Set(this.ColorToZone(this.sourceTexture.GetPixelBilinear(vector.x, vector.y)));
				}
				else
				{
					node.Set(ClimateZoneType.Temperate);
				}
			}
		}
		return true;
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x000DDC00 File Offset: 0x000DBE00
	private ClimateZoneType ColorToZone(Color c)
	{
		for (int i = 0; i < this.colors.Length; i++)
		{
			if (c == this.colors[i])
			{
				return (ClimateZoneType)i;
			}
		}
		return ClimateZoneType.Temperate;
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000023FD File Offset: 0x000005FD
	private void CleanTextures()
	{
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x000DDC3C File Offset: 0x000DBE3C
	private void Cleanup()
	{
		this.nodes = null;
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x000DDC48 File Offset: 0x000DBE48
	private void SaveBin()
	{
		using (FileStream fileStream = File.OpenWrite(this.path + "climate_zones.bin"))
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

	// Token: 0x060015C2 RID: 5570 RVA: 0x000DDCFC File Offset: 0x000DBEFC
	private void SaveTex()
	{
		Color[] array = new Color[this.width * this.height];
		int num = 0;
		for (int i = 0; i < this.height; i++)
		{
			for (int j = 0; j < this.width; j++)
			{
				ClimateZoneBuilder.Node node = this.nodes[j, i];
				Color color = this.colors[(int)node.type];
				array[num++] = color;
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height);
		texture2D.SetPixels(array);
		File.WriteAllBytes(this.path + "climate_zones.png", texture2D.EncodeToPNG());
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x000DDDAE File Offset: 0x000DBFAE
	private void Save()
	{
		global::Common.EditorProgress("Build Climate Zomes", "Saving", 0f, false);
		this.SaveBin();
	}

	// Token: 0x04000E18 RID: 3608
	private float tile_size = 5f;

	// Token: 0x04000E19 RID: 3609
	private Color32[] colors = new Color32[]
	{
		new Color32(38, 69, 16, byte.MaxValue),
		new Color32(253, 198, 137, byte.MaxValue),
		new Color32(149, 207, 107, byte.MaxValue),
		new Color32(198, 156, 109, byte.MaxValue),
		new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue)
	};

	// Token: 0x04000E1A RID: 3610
	private int width;

	// Token: 0x04000E1B RID: 3611
	private int height;

	// Token: 0x04000E1C RID: 3612
	private ClimateZoneBuilder.Node[,] nodes;

	// Token: 0x04000E1D RID: 3613
	private Terrain terrain;

	// Token: 0x04000E1E RID: 3614
	private string path;

	// Token: 0x04000E1F RID: 3615
	private Texture2D sourceTexture;

	// Token: 0x020006C4 RID: 1732
	private class Node
	{
		// Token: 0x0600488B RID: 18571 RVA: 0x00217FF8 File Offset: 0x002161F8
		public void Set(ClimateZoneType type)
		{
			this.type = type;
		}

		// Token: 0x040036E7 RID: 14055
		public ClimateZoneType type = ClimateZoneType.Temperate;
	}
}
