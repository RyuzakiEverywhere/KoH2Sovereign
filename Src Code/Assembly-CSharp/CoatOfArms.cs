using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class CoatOfArms
{
	// Token: 0x06000C03 RID: 3075 RVA: 0x000875A6 File Offset: 0x000857A6
	private CoatOfArms()
	{
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000C04 RID: 3076 RVA: 0x000875DA File Offset: 0x000857DA
	public static CoatOfArms Instance
	{
		get
		{
			if (CoatOfArms._instance != null)
			{
				return CoatOfArms._instance;
			}
			return CoatOfArms._instance = new CoatOfArms();
		}
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x000875F4 File Offset: 0x000857F4
	public void Load(string map)
	{
		this.WHITETEXTURE = new Texture2D(2, 2);
		this.WHITETEXTURE.SetPixels(new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		});
		this.WHITETEXTURE.Apply();
		global::Defs defs = global::Defs.Get(true);
		this.divisions = CoatOfArms.GetTextures("Assets/CoatOfArms/Divisions", false);
		this.patterns = CoatOfArms.GetTextures("Assets/CoatOfArms/Patterns", false);
		this.ordinaries = CoatOfArms.GetTextures("Assets/CoatOfArms/Ordinaries", false);
		this.charges = CoatOfArms.GetTextures("Assets/CoatOfArms/Charges", false);
		this.map_name = map;
		this.LoadDefs(defs.dt, map);
		this.init = true;
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x000876C0 File Offset: 0x000858C0
	public static Texture2D[] GetTextures(string path, bool recursive = true)
	{
		return CoatOfArmsUtility.ReadInAllTextures(Directory.GetFiles(path, "*.png", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x000876D9 File Offset: 0x000858D9
	public void LoadDefs(DT dt, string map_name)
	{
		this.LoadKingdoms(dt, map_name);
		this.LoadFactionKingdoms(dt, map_name);
		this.LoadPaletteColors();
		this.LoadCrestModes();
		this.LoadChargeTemplates();
		this.LoadCoatOfArms(dt, map_name);
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00087708 File Offset: 0x00085908
	private void LoadPaletteColors()
	{
		this.paletteColors = new Dictionary<string, Color>();
		DT.Def def = global::Defs.Get(true).dt.FindDef("CoatOfArmsPaletteColors");
		if (def == null)
		{
			return;
		}
		for (int i = 0; i < def.field.children.Count; i++)
		{
			DT.Field field = def.field.children[i];
			string key = field.key;
			Color value = global::Defs.ColorFromString(field.value_str, Color.white);
			this.paletteColors.Add(key, value);
		}
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0008778C File Offset: 0x0008598C
	private void LoadCrestModes()
	{
		this.modes = new List<CrestMode>();
		DT.Def def = global::Defs.Get(true).dt.FindDef("CoatOfArmsModes");
		if (def == null)
		{
			return;
		}
		for (int i = 0; i < def.field.children.Count; i++)
		{
			DT.Field field = def.field.children[i];
			Material obj = global::Defs.GetObj<Material>(field, "mat", null);
			CrestMode item = new CrestMode
			{
				atlas_width = field.GetInt("atlas_width", null, 0, true, true, true, '.'),
				atlas_height = field.GetInt("atlas_height", null, 0, true, true, true, '.'),
				width = field.GetInt("width", null, 0, true, true, true, '.'),
				height = field.GetInt("height", null, 0, true, true, true, '.'),
				dir = field.GetString("dir", null, "", true, true, true, '.'),
				mat = obj
			};
			this.modes.Add(item);
		}
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x00087895 File Offset: 0x00085A95
	private void LoadChargeTemplates()
	{
		global::Defs.Get(true).dt.FindDef("CoATemplates");
		this.chargeTemplates = new Dictionary<string, List<CoatOfArmsTexture>>();
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x000878B8 File Offset: 0x00085AB8
	private DT.Field CreateNewCrestDef(string map_name)
	{
		DT.Field field = new DT.Field(null);
		Directory.CreateDirectory("Assets/Maps/" + map_name + "/");
		field.flags = DT.Field.Flags.StartsAtSameLine;
		field.type = "extend";
		field.key = "CoatOfArmsCrests";
		field.AddChild(map_name);
		global::Defs defs = global::Defs.Get(true);
		if (defs != null)
		{
			defs.Reload();
			return defs.dt.Find("CoatOfArmsCrests." + map_name, null);
		}
		return null;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00087934 File Offset: 0x00085B34
	public CoatOfArmsCrest GetCrestByKey(string key, out int crest_id)
	{
		crest_id = 0;
		this.kingdomMapping.mapping.TryGetValue(key, out crest_id);
		if (crest_id == 0)
		{
			return this.CreateNewCrestForKey(key, out crest_id, null);
		}
		CoatOfArmsCrest coatOfArmsCrest;
		this.createdCrests.TryGetValue(crest_id, out coatOfArmsCrest);
		if (coatOfArmsCrest == null)
		{
			coatOfArmsCrest = new CoatOfArmsCrest();
			this.createdCrests.Add(crest_id, coatOfArmsCrest);
		}
		return coatOfArmsCrest;
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x00087990 File Offset: 0x00085B90
	private CoatOfArmsCrest CreateNewCrestForKey(string key, out int crest_id, CoatOfArmsCrest c = null)
	{
		int num = 1;
		while (this.createdCrests.ContainsKey(num))
		{
			num++;
		}
		if (c == null)
		{
			c = new CoatOfArmsCrest();
		}
		this.createdCrests.Add(num, c);
		crest_id = num;
		this.kingdomMapping.Set(key, num);
		Debug.Log(string.Concat(new object[]
		{
			"Created new crestId = ",
			num,
			" for ",
			key
		}));
		return c;
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x00087A08 File Offset: 0x00085C08
	public void SetCrestForKey(CoatOfArmsCrest c, string key, out int crest_id)
	{
		crest_id = 0;
		if (c == null)
		{
			Debug.LogWarning("Can't set crest for kingdom " + key + ". Def field is null.");
			return;
		}
		this.kingdomMapping.mapping.TryGetValue(key, out crest_id);
		if (crest_id == 0)
		{
			this.CreateNewCrestForKey(key, out crest_id, c);
			return;
		}
		if (this.createdCrests.ContainsKey(crest_id))
		{
			this.createdCrests[crest_id] = c;
			return;
		}
		this.createdCrests.Add(crest_id, c);
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x00087A80 File Offset: 0x00085C80
	private void LoadCoatOfArms(DT dt, string map_name)
	{
		if (this.kingdomMapping == null)
		{
			return;
		}
		DT.Field field = dt.Find("CoatOfArmsCrests." + map_name, null);
		this.coatOfArmsDef = field;
		if (this.coatOfArmsDef == null || this.coatOfArmsDef.children == null)
		{
			this.coatOfArmsDef = this.CreateNewCrestDef(map_name);
		}
		this.ReloadCreatedCrests();
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x00087AD8 File Offset: 0x00085CD8
	public void ReloadCreatedCrests()
	{
		if (this.coatOfArmsDef != null && this.coatOfArmsDef.children != null)
		{
			this.createdCrests = new Dictionary<int, CoatOfArmsCrest>();
			for (int i = 0; i < this.coatOfArmsDef.children.Count; i++)
			{
				DT.Field field = this.coatOfArmsDef.children[i];
				int key = int.Parse(field.key);
				this.createdCrests.Add(key, new CoatOfArmsCrest(field));
			}
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00087B50 File Offset: 0x00085D50
	public void LoadCoADef(string map_name)
	{
		global::Defs defs = global::Defs.Get(true);
		this.coatOfArmsDef = defs.dt.Find("CoatOfArmsCrests." + map_name, null);
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00087B81 File Offset: 0x00085D81
	private void LoadKingdoms(DT dt, string map_name)
	{
		this.kingdomMapping = new CoAMapping();
		this.kingdomMapping.Load(map_name);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00087B9C File Offset: 0x00085D9C
	private void LoadFactionKingdoms(DT dt, string map_name)
	{
		DT.Def def = dt.FindDef("FactionKingdom");
		if (def == null)
		{
			return;
		}
		if (def.defs == null || def.defs.Count == 0)
		{
			return;
		}
		List<DT.Def> defs = def.defs;
		for (int i = 0; i < defs.Count; i++)
		{
			DT.Def def2 = defs[i];
			if (!this.kingdomMapping.mapping.ContainsKey(def2.field.key))
			{
				this.kingdomMapping.Set(def2.field.key, 0);
			}
		}
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x00087C24 File Offset: 0x00085E24
	public void ClearCrest(string key)
	{
		int num = this.kingdomMapping.Get(key);
		if (num == 0)
		{
			return;
		}
		if (this.createdCrests.ContainsKey(num))
		{
			this.createdCrests[num] = new CoatOfArmsCrest();
		}
	}

	// Token: 0x04000963 RID: 2403
	public Texture2D WHITETEXTURE;

	// Token: 0x04000964 RID: 2404
	public bool init;

	// Token: 0x04000965 RID: 2405
	public string[] kingdomNames;

	// Token: 0x04000966 RID: 2406
	public CoAMapping kingdomMapping;

	// Token: 0x04000967 RID: 2407
	public Dictionary<string, DT.Field> kingdomDef;

	// Token: 0x04000968 RID: 2408
	public DT.Field coatOfArmsDef;

	// Token: 0x04000969 RID: 2409
	public Dictionary<string, List<CoatOfArmsTexture>> chargeTemplates = new Dictionary<string, List<CoatOfArmsTexture>>();

	// Token: 0x0400096A RID: 2410
	public Dictionary<string, Color> paletteColors = new Dictionary<string, Color>();

	// Token: 0x0400096B RID: 2411
	public string map_name = "";

	// Token: 0x0400096C RID: 2412
	public Texture2D[] divisions;

	// Token: 0x0400096D RID: 2413
	public Texture2D[] patterns;

	// Token: 0x0400096E RID: 2414
	public Texture2D[] ordinaries;

	// Token: 0x0400096F RID: 2415
	public Texture2D[] charges;

	// Token: 0x04000970 RID: 2416
	private static CoatOfArms _instance;

	// Token: 0x04000971 RID: 2417
	public List<CrestMode> modes = new List<CrestMode>();

	// Token: 0x04000972 RID: 2418
	public Dictionary<int, CoatOfArmsCrest> createdCrests;
}
