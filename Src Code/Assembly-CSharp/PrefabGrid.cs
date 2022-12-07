using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class PrefabGrid : MonoBehaviour
{
	// Token: 0x060010CE RID: 4302 RVA: 0x000B33B1 File Offset: 0x000B15B1
	public static void UseBattleView(bool bv)
	{
		if (PrefabGrid.use_battle_view == bv)
		{
			return;
		}
		PrefabGrid.use_battle_view = bv;
		PrefabGrid.Info.Clear();
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x000B33C7 File Offset: 0x000B15C7
	public static bool IsBattleView()
	{
		return (BattleMap.battle != null && BattleMap.battle.IsValid() && BattleMap.battle.battle_map_only) || PrefabGrid.use_battle_view;
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x000B33EF File Offset: 0x000B15EF
	public static string CurMapType()
	{
		if (!PrefabGrid.IsBattleView())
		{
			return PrefabGrid.WV_Dir;
		}
		return PrefabGrid.BV_Dir;
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x060010D1 RID: 4305 RVA: 0x000B3403 File Offset: 0x000B1603
	public static string EditorDir
	{
		get
		{
			return "Assets/" + PrefabGrid.CurMapType() + "/Architectures";
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x060010D2 RID: 4306 RVA: 0x000B3419 File Offset: 0x000B1619
	public static string PGDefsDir
	{
		get
		{
			return "PGDefs/" + PrefabGrid.CurMapType();
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x000B342C File Offset: 0x000B162C
	private static int[] layers_to_ignore
	{
		get
		{
			if (PrefabGrid._layers_to_ignore == null)
			{
				PrefabGrid._layers_to_ignore = new int[PrefabGrid.layers_to_ignore_names.Length];
				for (int i = 0; i < PrefabGrid._layers_to_ignore.Length; i++)
				{
					PrefabGrid._layers_to_ignore[i] = LayerMask.NameToLayer(PrefabGrid.layers_to_ignore_names[i]);
				}
			}
			return PrefabGrid._layers_to_ignore;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060010D4 RID: 4308 RVA: 0x000B347B File Offset: 0x000B167B
	public UnityEngine.Component parent
	{
		get
		{
			if (this._parent == null)
			{
				this.UpdateParent();
			}
			return this._parent;
		}
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x000B3497 File Offset: 0x000B1697
	public static string GetDir(string type, string architecture)
	{
		return string.Concat(new string[]
		{
			PrefabGrid.EditorDir,
			"/",
			type,
			"/",
			architecture
		});
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x000B34C4 File Offset: 0x000B16C4
	public string GetDir()
	{
		return PrefabGrid.GetDir(this.cur_type, this.cur_architecture);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x000B34D7 File Offset: 0x000B16D7
	public static bool IsAuto(string s)
	{
		return string.IsNullOrEmpty(s) || s == "Auto";
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x000B34F0 File Offset: 0x000B16F0
	public static bool ArchitectureExists(string type)
	{
		Assets.Init(false);
		Assets.DirInfo dirInfo = Assets.GetDir(PrefabGrid.EditorDir, false);
		if (dirInfo == null)
		{
			return false;
		}
		int num = type.IndexOf('/');
		if (num < 0)
		{
			dirInfo = dirInfo.GetSubDir(type, false);
		}
		else
		{
			Assets.DirInfo subDir = dirInfo.GetSubDir(type.Substring(0, num), false);
			dirInfo = ((subDir != null) ? subDir.GetSubDir(type.Substring(num + 1), false) : null);
		}
		return dirInfo != null;
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x000B355C File Offset: 0x000B175C
	public static bool ArchitectureExists(string type, string architecture)
	{
		Profile.BeginSection("ArchitectureExists");
		Assets.Init(false);
		Assets.DirInfo dirInfo = Assets.GetDir(PrefabGrid.EditorDir, false);
		if (dirInfo == null)
		{
			Profile.EndSection("ArchitectureExists");
			return false;
		}
		int num = type.IndexOf('/');
		if (num < 0)
		{
			dirInfo = dirInfo.GetSubDir(type, false);
		}
		else
		{
			Assets.DirInfo subDir = dirInfo.GetSubDir(type.Substring(0, num), false);
			dirInfo = ((subDir != null) ? subDir.GetSubDir(type.Substring(num + 1), false) : null);
		}
		if (dirInfo == null)
		{
			Profile.EndSection("ArchitectureExists");
			return false;
		}
		dirInfo = dirInfo.GetSubDir(architecture, false);
		Profile.EndSection("ArchitectureExists");
		return dirInfo != null;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x000B35FC File Offset: 0x000B17FC
	public static string ResolveArchitecture(string type, string architecture)
	{
		DT.Field defField = global::Defs.GetDefField(PrefabGrid.IsBattleView() ? "ArchitecturesBattleview" : "Architectures", null);
		if (defField == null)
		{
			return null;
		}
		DT.Field field = defField.FindChild("disabled", null, true, true, true, '.');
		bool flag = false;
		if (((field != null) ? field.children : null) != null && field.children.Count > 0)
		{
			for (int i = 0; i < field.children.Count; i++)
			{
				if (field.children[i].key == architecture)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag && PrefabGrid.ArchitectureExists(type, architecture))
		{
			return architecture;
		}
		do
		{
			DT.Field field2 = defField.FindChild(architecture, null, true, true, true, '.');
			if (field2 == null)
			{
				goto IL_13F;
			}
			DT.Field field3 = field2.FindChild(type, null, true, true, true, '.');
			if (field3 == null)
			{
				field3 = field2;
			}
			architecture = field3.String(null, "");
			if (string.IsNullOrEmpty(architecture))
			{
				goto IL_13F;
			}
			if (((field != null) ? field.children : null) != null && field.children.Count > 0)
			{
				flag = false;
				for (int j = 0; j < field.children.Count; j++)
				{
					if (field.children[j].key == architecture)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
		}
		while (!PrefabGrid.ArchitectureExists(type, architecture));
		return architecture;
		IL_13F:
		return null;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x000B374C File Offset: 0x000B194C
	public static List<string> EnumTypes(bool include_props = true)
	{
		Assets.Init(false);
		List<string> list = new List<string>();
		Assets.DirInfo dir = Assets.GetDir(PrefabGrid.EditorDir, false);
		if (dir == null)
		{
			return list;
		}
		List<Assets.DirInfo> subdirs = dir.GetSubdirs();
		int i = 0;
		while (i < subdirs.Count)
		{
			Assets.DirInfo dirInfo = subdirs[i];
			if (!string.Equals(dirInfo.name, "Props", StringComparison.OrdinalIgnoreCase))
			{
				goto IL_BA;
			}
			if (include_props)
			{
				string str = "Props/";
				List<Assets.DirInfo> subdirs2 = dirInfo.GetSubdirs();
				for (int j = 0; j < subdirs2.Count; j++)
				{
					Assets.DirInfo dirInfo2 = subdirs2[j];
					list.Add(str + char.ToUpperInvariant(dirInfo2.name[0]).ToString() + dirInfo2.name.Substring(1).ToLowerInvariant());
				}
				goto IL_BA;
			}
			IL_F2:
			i++;
			continue;
			IL_BA:
			list.Add(char.ToUpperInvariant(dirInfo.name[0]).ToString() + dirInfo.name.Substring(1).ToLowerInvariant());
			goto IL_F2;
		}
		return list;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x000B385C File Offset: 0x000B1A5C
	public static List<string> EnumArchitectures(string type, bool add_auto = false, bool add_all = false)
	{
		Assets.Init(false);
		List<string> list = new List<string>();
		if (add_auto)
		{
			list.Add("Auto");
		}
		if (add_all)
		{
			DT.Field defField = global::Defs.GetDefField("Architectures", null);
			if (defField != null && defField.children != null)
			{
				for (int i = 0; i < defField.children.Count; i++)
				{
					DT.Field field = defField.children[i];
					if (!string.IsNullOrEmpty(field.key))
					{
						list.Add(field.key);
					}
				}
			}
		}
		if (string.IsNullOrEmpty(type))
		{
			return list;
		}
		Assets.DirInfo dir = Assets.GetDir(PrefabGrid.EditorDir + "/" + type, false);
		if (dir == null)
		{
			return list;
		}
		List<Assets.DirInfo> subdirs = dir.GetSubdirs();
		for (int j = 0; j < subdirs.Count; j++)
		{
			Assets.DirInfo dirInfo = subdirs[j];
			string item = char.ToUpperInvariant(dirInfo.name[0]).ToString() + dirInfo.name.Substring(1).ToLowerInvariant();
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x000B3978 File Offset: 0x000B1B78
	public static PrefabGrid ApplyToTransform(string type, string architecture, bool force_type, bool force_architecture, Transform t, bool refresh = true)
	{
		if (t == null)
		{
			return null;
		}
		InstanceHolder component = t.GetComponent<InstanceHolder>();
		if (component != null)
		{
			component.Despawn();
			UnityEngine.Object.DestroyImmediate(component);
		}
		SettlementPart component2 = t.GetComponent<SettlementPart>();
		if (component2 != null)
		{
			UnityEngine.Object.DestroyImmediate(component2);
		}
		PrefabGrid prefabGrid = t.GetComponent<PrefabGrid>();
		if (prefabGrid != null && !string.IsNullOrEmpty(prefabGrid.type))
		{
			if (!force_type)
			{
				type = prefabGrid.type;
			}
			if (!force_architecture)
			{
				architecture = prefabGrid.architecture;
			}
		}
		if (prefabGrid == null)
		{
			prefabGrid = t.gameObject.AddComponent<PrefabGrid>();
		}
		prefabGrid.type = type;
		prefabGrid.architecture = architecture;
		prefabGrid.expanded = false;
		if (refresh)
		{
			prefabGrid.Refresh(true, true);
		}
		return prefabGrid;
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x000B3A34 File Offset: 0x000B1C34
	public static void ApplyToChildren(string type, string architecture, bool force, Transform parent)
	{
		if (parent == null)
		{
			return;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			PrefabGrid.ApplyToTransform(type, architecture, false, force, child, true);
		}
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x000B3A70 File Offset: 0x000B1C70
	private static bool Match(string name, ref int idx, char c)
	{
		if (idx >= name.Length)
		{
			return false;
		}
		if (char.ToLowerInvariant(name[idx]) != char.ToLowerInvariant(c))
		{
			return false;
		}
		idx++;
		return true;
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x000B3A9C File Offset: 0x000B1C9C
	private static bool Match(string name, ref int idx, string s)
	{
		if (idx + s.Length > name.Length)
		{
			return false;
		}
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			char c2 = name[idx + i];
			if (char.ToLowerInvariant(c) != char.ToLowerInvariant(c2))
			{
				return false;
			}
		}
		idx += s.Length;
		return true;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x000B3AFC File Offset: 0x000B1CFC
	private static int ParseNumber(string name, ref int idx)
	{
		int num = 0;
		while (idx < name.Length)
		{
			char c = name[idx];
			if (!char.IsDigit(c))
			{
				break;
			}
			num = 10 * num + (int)c - 48;
			idx++;
		}
		return num;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x000B3B3C File Offset: 0x000B1D3C
	public static bool ParsePrefabName(string name, string suffix, out int variant, out int level)
	{
		variant = 0;
		level = 0;
		int num = 0;
		if (!PrefabGrid.Match(name, ref num, 'v'))
		{
			return false;
		}
		variant = PrefabGrid.ParseNumber(name, ref num);
		if (variant < 1)
		{
			return false;
		}
		if (!PrefabGrid.Match(name, ref num, 'l'))
		{
			return false;
		}
		level = PrefabGrid.ParseNumber(name, ref num);
		return level >= 1 && PrefabGrid.Match(name, ref num, suffix) && num == name.Length;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x000B3BA8 File Offset: 0x000B1DA8
	public PrefabGrid.Cell FindCell(GameObject root)
	{
		if (this.cells == null)
		{
			return null;
		}
		int variant;
		int level;
		if (!PrefabGrid.ParsePrefabName(root.name, "", out variant, out level))
		{
			return null;
		}
		return this.GetCell(variant, level, false);
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x000B3BE0 File Offset: 0x000B1DE0
	public void DecideVariant()
	{
		this.cur_variant = PrefabGrid.ChooserContext.DecideVariant(this);
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x000B3BF0 File Offset: 0x000B1DF0
	public void DecideLevel()
	{
		if (this.set_level > 0)
		{
			this.cur_level = this.set_level;
			return;
		}
		PrefabGrid prefabGrid = this.parent as PrefabGrid;
		if (prefabGrid != null && prefabGrid != this)
		{
			if (prefabGrid.expanded)
			{
				this.cur_level = prefabGrid.WorldToGrid(base.transform.position).y;
			}
			else
			{
				prefabGrid.DecideLevel();
				this.cur_level = prefabGrid.cur_level;
			}
			int maxLevel = this.GetMaxLevel();
			int maxLevel2 = prefabGrid.GetMaxLevel();
			this.cur_level = global::Common.map(this.cur_level, 1, maxLevel2, 1, maxLevel, true);
			return;
		}
		if (this.cur_level <= 0)
		{
			this.cur_level = Random.Range(1, this.GetMaxLevel() + 1);
		}
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x000B3CAE File Offset: 0x000B1EAE
	public void DecideType()
	{
		PrefabGrid.IsAuto(this.type);
		this.cur_type = this.type;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x000B3CC8 File Offset: 0x000B1EC8
	public string DecideAutoArchitecture()
	{
		Profile.BeginSection("PG.DecideAutoArchitecture");
		Transform transform = base.transform;
		Transform parent = base.transform.parent;
		while (parent != null)
		{
			parent.GetComponents<UnityEngine.Component>(PrefabGrid.s_components);
			int i = 0;
			while (i < PrefabGrid.s_components.Count)
			{
				UnityEngine.Component component = PrefabGrid.s_components[i];
				PrefabGrid prefabGrid;
				global::Settlement settlement;
				SettlementBV settlementBV;
				if ((prefabGrid = (component as PrefabGrid)) != null)
				{
					PrefabGrid.s_components.Clear();
					if (PrefabGrid.IsAuto(prefabGrid.architecture))
					{
						string text = prefabGrid.DecideAutoArchitecture();
						if (text != null)
						{
							Profile.EndSection("PG.DecideAutoArchitecture");
							return text;
						}
						break;
					}
					else
					{
						if (PrefabGrid.ResolveArchitecture(this.cur_type, prefabGrid.architecture) != null)
						{
							Profile.EndSection("PG.DecideAutoArchitecture");
							return prefabGrid.architecture;
						}
						break;
					}
				}
				else if ((settlement = (component as global::Settlement)) != null)
				{
					PrefabGrid.s_components.Clear();
					string text2;
					if (transform.name == "Citadel")
					{
						text2 = settlement.citadel_architecture;
					}
					else
					{
						text2 = settlement.houses_architecture;
					}
					if (!PrefabGrid.IsAuto(text2))
					{
						Profile.EndSection("PG.DecideAutoArchitecture");
						return text2;
					}
					break;
				}
				else if ((settlementBV = (component as SettlementBV)) != null)
				{
					PrefabGrid.s_components.Clear();
					string text3;
					if (transform.name == "Citadel")
					{
						text3 = settlementBV.citadel_architecture;
					}
					else
					{
						text3 = settlementBV.houses_architecture;
					}
					if (!PrefabGrid.IsAuto(text3))
					{
						Profile.EndSection("PG.DecideAutoArchitecture");
						return text3;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			PrefabGrid.s_components.Clear();
			transform = parent;
			parent = parent.parent;
		}
		global::Realm realm = global::Realm.At(base.transform.position);
		if (realm == null)
		{
			Profile.EndSection("PG.DecideAutoArchitecture");
			return PrefabGrid.default_architecture;
		}
		Logic.Realm logic = realm.logic;
		Castle castle = (logic != null) ? logic.castle : null;
		if (castle != null && castle.field != null)
		{
			string @string = castle.field.GetString("houses", null, "", true, true, true, '.');
			if (!PrefabGrid.IsAuto(@string))
			{
				Profile.EndSection("PG.DecideAutoArchitecture");
				return @string;
			}
			string string2 = castle.field.GetString("citadel.architecture", null, "", true, true, true, '.');
			if (!PrefabGrid.IsAuto(string2))
			{
				Profile.EndSection("PG.DecideAutoArchitecture");
				return string2;
			}
		}
		Profile.EndSection("PG.DecideAutoArchitecture");
		return PrefabGrid.default_architecture;
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x000B3F14 File Offset: 0x000B2114
	public void DecideArchitecture()
	{
		if (this.expanded)
		{
			this.cur_architecture = this.architecture;
			return;
		}
		if (PrefabGrid.IsAuto(this.architecture))
		{
			this.cur_architecture = this.DecideAutoArchitecture();
		}
		else
		{
			this.cur_architecture = this.architecture;
		}
		Profile.BeginSection("PG.DecideArchitecture.ResolveArchitecture");
		this.cur_architecture = PrefabGrid.ResolveArchitecture(this.cur_type, this.cur_architecture);
		Profile.EndSection("PG.DecideArchitecture.ResolveArchitecture");
		if (this.cur_architecture == null)
		{
			this.cur_architecture = this.architecture;
		}
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x000B3F9C File Offset: 0x000B219C
	public void DecideTypeAndArchitecture()
	{
		this.DecideType();
		this.DecideArchitecture();
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x000B3FAC File Offset: 0x000B21AC
	public void DecideVariantAndLevel()
	{
		if (this.expanded)
		{
			this.cur_variant = (this.cur_level = 0);
			return;
		}
		this.DecideVariant();
		this.DecideLevel();
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x000B3FE0 File Offset: 0x000B21E0
	public bool RefreshRealmTags()
	{
		bool result = false;
		if (!this.expanded)
		{
			int num = this.cur_variant;
			this.DecideVariant();
			if (num != this.cur_variant)
			{
				result = true;
				this.Despawn(true);
				this.Spawn();
				if (!(this.parent is Wall) && Application.isPlaying)
				{
					global::Settlement componentInParent = base.GetComponentInParent<global::Settlement>();
					if (componentInParent != null)
					{
						componentInParent.MarkFroBatch();
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x000B4048 File Offset: 0x000B2248
	public int GetMaxLevel()
	{
		if (this.expanded)
		{
			return this.grid_size.y;
		}
		this.UpdateInfo(false);
		if (this.info == null)
		{
			return this.grid_size.y;
		}
		return this.info.grid_size.y;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x000B4094 File Offset: 0x000B2294
	public PrefabGrid.Cell GetCell(int variant, int level, bool create = false)
	{
		if (variant < 1 || level < 1)
		{
			return null;
		}
		if (this.cells == null)
		{
			if (!create)
			{
				return null;
			}
			this.cells = new List<List<PrefabGrid.Cell>>(variant);
		}
		if (variant > this.cells.Count)
		{
			if (!create)
			{
				return null;
			}
			while (this.cells.Count < variant)
			{
				this.cells.Add(null);
			}
		}
		List<PrefabGrid.Cell> list = this.cells[variant - 1];
		if (list == null)
		{
			if (!create)
			{
				return null;
			}
			list = new List<PrefabGrid.Cell>(level);
			this.cells[variant - 1] = list;
		}
		if (level > list.Count)
		{
			if (!create)
			{
				return null;
			}
			while (list.Count < level)
			{
				list.Add(null);
			}
		}
		PrefabGrid.Cell cell = list[level - 1];
		if (cell == null)
		{
			if (!create)
			{
				return null;
			}
			cell = new PrefabGrid.Cell();
			list[level - 1] = cell;
		}
		return cell;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x000B4160 File Offset: 0x000B2360
	public void Load(DT.Field field, string default_type, string default_architecture, int default_variant, int default_level, bool position, bool rotation)
	{
		this.type = field.GetString("type", null, default_type, true, true, true, '.');
		this.architecture = field.GetString("architecture", null, default_architecture, true, true, true, '.');
		if (position)
		{
			if (field.value.obj_val is Point)
			{
				base.transform.localPosition = (Point)field.value.obj_val;
			}
			else
			{
				base.transform.localPosition = Vector3.zero;
			}
		}
		if (rotation)
		{
			float @float = field.GetFloat("rotation", null, 0f, true, true, true, '.');
			if (@float != 0f)
			{
				base.transform.localEulerAngles = new Vector3(0f, @float, 0f);
			}
		}
		this.set_variant = field.GetInt("variant", null, default_variant, true, true, true, '.');
		this.cur_variant = 0;
		this.set_level = field.GetInt("level", null, default_level, true, true, true, '.');
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x000B425C File Offset: 0x000B245C
	public static PrefabGrid Load(DT.Field field, Transform parent, string default_type, string default_architecture, int default_variant, int default_level, bool position, bool rotation)
	{
		if (field == null)
		{
			return null;
		}
		PrefabGrid prefabGrid = global::Common.SpawnTemplate<PrefabGrid>("PrefabGrid", field.key, parent, false, new Type[]
		{
			typeof(PrefabGrid)
		});
		prefabGrid.expanded = false;
		prefabGrid.Load(field, default_type, default_architecture, default_variant, default_level, position, rotation);
		prefabGrid.gameObject.SetActive(true);
		return prefabGrid;
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x000023FD File Offset: 0x000005FD
	private void MarkExpandedPGParentModified(PrefabGrid pg_parent)
	{
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x000B42B8 File Offset: 0x000B24B8
	public void SetParent(UnityEngine.Component parent, bool destroying = false)
	{
		if (parent == this._parent && parent != null)
		{
			return;
		}
		if (parent == this)
		{
			Debug.LogError("PG parent recursion");
			parent = null;
		}
		PrefabGrid prefabGrid = this._parent as PrefabGrid;
		this._parent = parent;
		if (this._parent == null)
		{
			if (!destroying && base.gameObject.activeInHierarchy)
			{
				RootPG rootPG = base.GetComponent<RootPG>();
				if (rootPG == null)
				{
					rootPG = base.gameObject.AddComponent<RootPG>();
					rootPG.hideFlags = HideFlags.DontSave;
				}
				rootPG.pg = this;
				this._parent = rootPG;
				PrefabGrid.root_pgs.Add(this);
			}
		}
		else
		{
			bool flag = this._parent is ArchitectureSet;
			if (this.expanded != flag)
			{
				this.expanded = flag;
				this.Refresh(true, false);
			}
			RootPG component = base.GetComponent<RootPG>();
			if (component != null)
			{
				component.pg = null;
				UnityEngine.Object.DestroyImmediate(component);
			}
			if (this.expanded && !Application.isPlaying)
			{
				PrefabGrid.root_pgs.Add(this);
			}
			else
			{
				PrefabGrid.root_pgs.Remove(this);
			}
		}
		PrefabGrid prefabGrid2 = this._parent as PrefabGrid;
		if (prefabGrid2 != prefabGrid)
		{
			if (prefabGrid != null)
			{
				List<PrefabGrid> list = prefabGrid.child_pgs;
				if (list != null)
				{
					list.Remove(this);
				}
			}
			if (prefabGrid2 != null)
			{
				if (prefabGrid2.child_pgs == null)
				{
					prefabGrid2.child_pgs = new List<PrefabGrid>();
				}
				prefabGrid2.child_pgs.Add(this);
			}
		}
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x000B4434 File Offset: 0x000B2634
	private void UpdateParent()
	{
		UnityEngine.Component parent;
		if (PrefabGrid.currently_spawning != null)
		{
			parent = PrefabGrid.currently_spawning;
		}
		else
		{
			parent = null;
			Transform parent2 = base.transform.parent;
			while (parent2 != null)
			{
				parent2.GetComponents<UnityEngine.Component>(PrefabGrid.s_components);
				for (int i = 0; i < PrefabGrid.s_components.Count; i++)
				{
					UnityEngine.Component component = PrefabGrid.s_components[i];
					if (component is global::Settlement || component is Wall || component is PrefabGrid || component is ArchitectureSet)
					{
						parent = component;
						PrefabGrid.s_components.Clear();
						break;
					}
					PrefabGrid.s_components.Clear();
				}
				parent2 = parent2.parent;
			}
		}
		this.SetParent(parent, false);
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x000B44E8 File Offset: 0x000B26E8
	private void FindRoots()
	{
		if (!this.expanded)
		{
			this.root = global::Common.FindChildByName(base.gameObject, string.Concat(new object[]
			{
				"V",
				this.cur_variant,
				"L",
				this.cur_level
			}), false, false);
			return;
		}
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x000B4548 File Offset: 0x000B2748
	public void UpdateInfo(bool force_reload)
	{
		this.DecideTypeAndArchitecture();
		if (!force_reload && this.info != null && this.info.valid && this.info.type == this.cur_type && this.info.architecture == this.cur_architecture)
		{
			this.hex_size = this.info.hex_size;
			this.rect_width = this.info.rect_width;
			this.rect_height = this.info.rect_height;
			this.use_rect_for_splats = this.info.use_rect_for_splats;
			this.tile_size = this.info.tile_size;
			this.grid_size = this.info.grid_size;
			this.capture_point = this.info.capture_point;
			return;
		}
		Profile.BeginSection("PrefabGrid.UpdateInfo");
		this.info = PrefabGrid.Info.Get(this.cur_type, this.cur_architecture, force_reload);
		if (this.hex_size <= 0f)
		{
			this.hex_size = this.info.hex_size;
		}
		if (this.rect_width <= 0f)
		{
			this.rect_width = this.info.rect_width;
		}
		if (this.rect_height <= 0f)
		{
			this.rect_height = this.info.rect_height;
		}
		if (this.grid_size.x <= 0 || force_reload)
		{
			this.grid_size.x = this.info.grid_size.x;
		}
		if (this.grid_size.y <= 0 || force_reload)
		{
			this.grid_size.y = this.info.grid_size.y;
		}
		if (this.tile_size <= 0f || force_reload)
		{
			this.tile_size = this.info.tile_size;
		}
		this.cells = null;
		this.root = null;
		this.modified = false;
		this.FindRoots();
		Profile.EndSection("PrefabGrid.UpdateInfo");
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x000B4750 File Offset: 0x000B2950
	public void Load()
	{
		this.hex_size = 0f;
		this.rect_width = 0f;
		this.rect_height = 0f;
		this.tile_size = 0f;
		this.use_rect_for_splats = false;
		this.grid_size = Vector2Int.zero;
		this.settings_changed = false;
		this.Refresh(true, true);
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x000B47AC File Offset: 0x000B29AC
	public void Refresh(bool full = false, bool instantly = false)
	{
		string name = full ? "PrefabGrid.Refresh full" : "PrefabGrid.Refresh partial";
		Profile.BeginSection(name);
		if (this._parent == null)
		{
			this.UpdateParent();
		}
		this.UpdateInfo(full);
		this.DecideVariantAndLevel();
		this.Despawn(full);
		this.Spawn();
		Profile.EndSection(name);
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x000B4801 File Offset: 0x000B2A01
	public void Despawn(bool full)
	{
		global::Common.DestroyChildren(base.gameObject);
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000B4810 File Offset: 0x000B2A10
	private void Snap(GameObject instance, GameObject org, GameObject root, GameObject prefab, Terrain terrain, float water_level)
	{
		Renderer x = null;
		instance.GetComponents<UnityEngine.Component>(PrefabGrid.s_components);
		for (int i = 0; i < PrefabGrid.s_components.Count; i++)
		{
			UnityEngine.Component component = PrefabGrid.s_components[i];
			if (component is PrefabGrid || component is WanderGroup)
			{
				return;
			}
			Renderer renderer;
			if ((renderer = (component as Renderer)) != null)
			{
				x = renderer;
			}
		}
		PrefabGrid.s_components.Clear();
		if (x != null || instance.tag == "Rigid")
		{
			float ofs = org.transform.position.y - prefab.transform.position.y;
			global::Common.SnapToTerrain(instance, ofs, terrain, water_level);
			return;
		}
		int childCount = instance.transform.childCount;
		int childCount2 = org.transform.childCount;
		if (childCount != childCount2)
		{
			Debug.LogError("Prefab instanse messed up during snap");
			return;
		}
		for (int j = 0; j < childCount; j++)
		{
			GameObject gameObject = instance.transform.GetChild(j).gameObject;
			GameObject gameObject2 = org.transform.GetChild(j).gameObject;
			this.Snap(gameObject, gameObject2, root, prefab, terrain, water_level);
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x000B4934 File Offset: 0x000B2B34
	private void Snap(GameObject root, GameObject prefab)
	{
		Terrain terrain = Terrain.activeTerrain;
		float waterLevel = MapData.GetWaterLevel();
		if (terrain == null || (Application.isPlaying && !PrefabGrid.use_battle_view))
		{
			Terrain terrain2 = WorldMap.GetTerrain();
			if (terrain2 != null)
			{
				terrain = terrain2;
				WorldMap worldMap = WorldMap.Get();
				if (worldMap != null)
				{
					waterLevel = worldMap.TerrainHeights.WaterLevel;
				}
			}
		}
		if (terrain == null)
		{
			return;
		}
		Profile.BeginSection("PrefabGrid.Snap");
		this.Snap(root, prefab, root, prefab, terrain, waterLevel);
		Profile.EndSection("PrefabGrid.Snap");
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x000B49BC File Offset: 0x000B2BBC
	public void Snap()
	{
		if (this.expanded || this.root == null)
		{
			return;
		}
		GameObject prefab = this.info.GetPrefab(this.cur_variant, this.cur_level);
		if (prefab == null)
		{
			return;
		}
		this.Snap(this.root, prefab);
		if (this.child_pgs != null)
		{
			for (int i = 0; i < this.child_pgs.Count; i++)
			{
				this.child_pgs[i].Snap();
			}
		}
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x000B4A40 File Offset: 0x000B2C40
	public GameObject Spawn(GameObject root, int variant, int level)
	{
		GameObject prefab = this.info.GetPrefab(variant, level);
		if (prefab == null)
		{
			if (root != null)
			{
				UnityEngine.Object.DestroyImmediate(root);
			}
			return null;
		}
		if (root == null)
		{
			Profile.BeginSection("PrefabGrid.Spawn.Spawn root");
			PrefabGrid prefabGrid = PrefabGrid.currently_spawning;
			PrefabGrid.currently_spawning = this;
			TerrainSnap.do_not_spawn = (!this.expanded || Application.isPlaying);
			root = global::Common.Spawn(prefab, true, false);
			TerrainSnap.do_not_spawn = false;
			root.name = string.Concat(new object[]
			{
				"V",
				variant,
				"L",
				level
			});
			root.transform.SetParent(base.transform, false);
			if (this.expanded)
			{
				root.transform.localPosition = this.GridToLocal(variant, level);
			}
			else
			{
				root.transform.localPosition = Vector3.zero;
			}
			root.transform.localRotation = Quaternion.identity;
			root.tag = "AutoGenerated";
			Profile.EndSection("PrefabGrid.Spawn.Spawn root");
			if (this.child_pgs != null)
			{
				Profile.BeginSection("PrefabGrid.Spawn.Enable child PGs");
				for (int i = 0; i < this.child_pgs.Count; i++)
				{
					PrefabGrid prefabGrid2 = this.child_pgs[i];
					if (!(prefabGrid2 == null))
					{
						PrefabGrid.currently_spawning = prefabGrid2;
						prefabGrid2.OnEnable();
					}
				}
				Profile.EndSection("PrefabGrid.Spawn.Enable child PGs");
			}
			PrefabGrid.currently_spawning = prefabGrid;
		}
		else if (this.expanded)
		{
			root.transform.localPosition = this.GridToLocal(variant, level);
		}
		else
		{
			root.transform.localPosition = Vector3.zero;
		}
		root.SetActive(true);
		if (!this.expanded && !(this.parent is Wall))
		{
			this.Snap(root, prefab);
		}
		return root;
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x000B4C04 File Offset: 0x000B2E04
	public void Spawn()
	{
		TerrainSnap component = base.GetComponent<TerrainSnap>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		global::Common.SnapToTerrain(base.gameObject, 0f, null, -1f);
		if (this.info == null || !this.info.IsValid())
		{
			return;
		}
		if (!this.expanded)
		{
			Profile.BeginSection("PrefabGrid.Spawn collapsed");
			int maxLevel = this.info.GetMaxLevel(this.cur_variant);
			if (maxLevel > this.cur_level)
			{
				maxLevel = this.cur_level;
			}
			this.root = this.Spawn(this.root, this.cur_variant, maxLevel);
			Profile.EndSection("PrefabGrid.Spawn collapsed");
			return;
		}
		Profile.BeginSection("PrefabGrid.Spawn expanded");
		for (int i = 1; i <= this.info.max_variant; i++)
		{
			int maxLevel2 = this.info.GetMaxLevel(i);
			for (int j = 1; j <= maxLevel2; j++)
			{
				PrefabGrid.Cell cell = this.GetCell(i, j, true);
				if (cell != null)
				{
					cell.root = this.Spawn(cell.root, i, j);
				}
			}
		}
		Profile.EndSection("PrefabGrid.Spawn expanded");
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x000B4D1C File Offset: 0x000B2F1C
	public Vector3[] GetRectCornersInWorldSpace()
	{
		float num = this.rect_width * 0.5f;
		float num2 = this.rect_height * 0.5f;
		return new Vector3[]
		{
			base.transform.TransformPoint(new Vector3(num, 0f, num2)),
			base.transform.TransformPoint(new Vector3(num, 0f, -num2)),
			base.transform.TransformPoint(new Vector3(-num, 0f, -num2)),
			base.transform.TransformPoint(new Vector3(-num, 0f, num2))
		};
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x000B4DC8 File Offset: 0x000B2FC8
	public Vector3 GridToLocal(int variant, int level)
	{
		return new Vector3(((float)(-(float)this.grid_size.x) / 2f + (float)variant - 0.5f) * this.tile_size, 0f, ((float)(-(float)this.grid_size.y) / 2f + (float)level - 0.5f) * this.tile_size);
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x000B4E25 File Offset: 0x000B3025
	public Vector2Int WorldToGrid(Vector3 pt)
	{
		pt = base.transform.InverseTransformPoint(pt);
		return this.LocalToGrid(pt);
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x000B4E3C File Offset: 0x000B303C
	public Vector2Int LocalToGrid(Vector3 pt)
	{
		float num = pt.x / this.tile_size;
		float num2 = pt.z / this.tile_size;
		float f = num + (float)this.grid_size.x * 0.5f;
		num2 += (float)this.grid_size.y * 0.5f;
		return new Vector2Int(Mathf.FloorToInt(f) + 1, Mathf.FloorToInt(num2) + 1);
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x000B4EA0 File Offset: 0x000B30A0
	public float ScaledRadius()
	{
		return (float)Math.Sqrt(Math.Pow((double)this.ScaledWidth(), 2.0) + Math.Pow((double)this.ScaledHeight(), 2.0));
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x000B4ED4 File Offset: 0x000B30D4
	public float ScaledWidth()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		return (lossyScale.x + lossyScale.y + lossyScale.z) / 3f / 2f * this.rect_width;
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x000B4F14 File Offset: 0x000B3114
	public float ScaledHeight()
	{
		Vector3 lossyScale = base.transform.lossyScale;
		return (lossyScale.x + lossyScale.y + lossyScale.z) / 3f / 2f * this.rect_height;
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x000B4F54 File Offset: 0x000B3154
	public float ScaledRadiusInner()
	{
		return this.ScaledRadius() * 0.866f;
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x000B4F64 File Offset: 0x000B3164
	public Bounds CalcBounds()
	{
		Vector3 position = base.transform.position;
		float x;
		float z;
		if (this.info == null)
		{
			z = (x = 10f);
		}
		else
		{
			z = (x = this.info.tile_size);
		}
		return new Bounds(position, new Vector3(x, 100f, z));
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x000B4FAF File Offset: 0x000B31AF
	private void OnDestroy()
	{
		PrefabGrid.ChooserContext.OnDestroy(this);
		this.SetParent(null, true);
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x000B4FC0 File Offset: 0x000B31C0
	private void OnEnable()
	{
		if (this.expanded)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		if (this._parent == null)
		{
			this.UpdateParent();
		}
		this.ResolveNestedPG();
		if (PrefabGrid.currently_spawning != null && PrefabGrid.currently_spawning != this)
		{
			return;
		}
		if (BattleMap.Get() == null)
		{
			if (Application.isPlaying && !(this.parent is RootPG) && !(this.parent is PrefabGrid))
			{
				return;
			}
			if (!this.expanded && !this.preview && (this.parent is global::Settlement || this.parent is Wall))
			{
				return;
			}
		}
		this.Refresh(false, PrefabGrid.currently_spawning != null);
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x000B5084 File Offset: 0x000B3284
	public static void SaveAllNestedPGs()
	{
		string str = PrefabGrid.IsBattleView() ? "_bv" : "";
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (string text in PrefabGrid.EnumTypes(true))
		{
			foreach (string text2 in PrefabGrid.EnumArchitectures(text, false, false))
			{
				PrefabGrid.AddNestedPGs(text, text2, dictionary);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("def nested_pgs" + str + "\n{\n");
		foreach (KeyValuePair<string, string> keyValuePair in dictionary)
		{
			string key = keyValuePair.Key;
			string value = keyValuePair.Value;
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"\t",
				key,
				" = \"",
				value,
				"\""
			}));
		}
		stringBuilder.Append("}");
		File.WriteAllText("assets/defs/netsted_pgs" + str + ".def", stringBuilder.ToString());
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x000B51F8 File Offset: 0x000B33F8
	public static void AddNestedPGs(string type, string architecture, Dictionary<string, string> nested_pgs)
	{
		PrefabGrid.Info info = PrefabGrid.Info.Get(type, architecture, true);
		for (int i = 1; i <= info.max_variant; i++)
		{
			int maxLevel = info.GetMaxLevel(i);
			for (int j = 1; j <= maxLevel; j++)
			{
				GameObject prefab = info.GetPrefab(i, j);
				if (!(prefab == null))
				{
					PrefabGrid.AddNestedPGs(string.Concat(new string[]
					{
						type,
						"/",
						architecture,
						"/",
						prefab.name
					}), prefab.transform, nested_pgs);
				}
			}
		}
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x000B5284 File Offset: 0x000B3484
	private static void AddNestedPGs(string path, Transform t, Dictionary<string, string> nested_pgs)
	{
		PrefabGrid component = t.GetComponent<PrefabGrid>();
		if (component != null)
		{
			nested_pgs.Add(path, component.type + "/" + component.architecture);
			return;
		}
		int childCount = t.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			PrefabGrid.AddNestedPGs(path + "/" + i, child, nested_pgs);
		}
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x000B52F4 File Offset: 0x000B34F4
	public static void LoadNestedPGs(bool force_reload = false)
	{
		if (!force_reload && PrefabGrid.nested_pgs != null)
		{
			return;
		}
		string str = PrefabGrid.IsBattleView() ? "_bv" : "";
		PrefabGrid.nested_pgs = new Dictionary<string, string>();
		DT.Field defField = global::Defs.GetDefField("nested_pgs" + str, null);
		if (defField == null || defField.children == null)
		{
			return;
		}
		for (int i = 0; i < defField.children.Count; i++)
		{
			DT.Field field = defField.children[i];
			string key = field.key;
			if (!string.IsNullOrEmpty(key))
			{
				string value = field.String(null, "");
				if (!string.IsNullOrEmpty(value))
				{
					PrefabGrid.nested_pgs.Add(key, value);
				}
			}
		}
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x000B53A4 File Offset: 0x000B35A4
	private void ResolveNestedPG()
	{
		if (!string.IsNullOrEmpty(this.type))
		{
			return;
		}
		string text = base.transform.GetSiblingIndex().ToString();
		Transform transform = base.transform.parent;
		while (transform != null)
		{
			Transform parent = transform.parent;
			if (parent == null)
			{
				return;
			}
			PrefabGrid component = parent.GetComponent<PrefabGrid>();
			if (!(component == null))
			{
				text = string.Concat(new string[]
				{
					component.cur_type,
					"/",
					component.cur_architecture,
					"/",
					transform.name,
					"/",
					text
				});
				break;
			}
			text = transform.GetSiblingIndex() + "/" + text;
			transform = parent;
		}
		PrefabGrid.LoadNestedPGs(false);
		string text2;
		if (!PrefabGrid.nested_pgs.TryGetValue(text, out text2))
		{
			Debug.LogError(text + ": Nested PG info not found");
			return;
		}
		int num = text2.LastIndexOf("/");
		if (num < 0)
		{
			Debug.LogError(text + ": Nested PG info is invalid");
			return;
		}
		this.type = text2.Substring(0, num);
		this.architecture = text2.Substring(num + 1);
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x000B54DE File Offset: 0x000B36DE
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			this.type,
			"/",
			this.architecture,
			": ",
			global::Common.ObjPath(base.gameObject)
		});
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600110E RID: 4366 RVA: 0x000B551C File Offset: 0x000B371C
	public List<BSGTerrainTools.PerTerrainInfo> terrain_infos
	{
		get
		{
			if (this._terrain_infos != null)
			{
				return this._terrain_infos;
			}
			GameObject gameObject = GameObject.Find("Terrains");
			if (gameObject == null)
			{
				return this._terrain_infos;
			}
			this._terrain_infos = new List<BSGTerrainTools.PerTerrainInfo>();
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Terrain component = gameObject.transform.GetChild(i).GetComponent<Terrain>();
				if (!(component == null))
				{
					BSGTerrainTools.PerTerrainInfo perTerrainInfo = new BSGTerrainTools.PerTerrainInfo();
					perTerrainInfo.td = component.terrainData;
					this._terrain_infos.Add(perTerrainInfo);
				}
			}
			return this._terrain_infos;
		}
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x000B55B3 File Offset: 0x000B37B3
	public static BSGTerrainTools.PerTerrainInfo GetTerrainInfo(TerrainData terrain)
	{
		return new BSGTerrainTools.PerTerrainInfo
		{
			td = terrain
		};
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x000B55C4 File Offset: 0x000B37C4
	public Bounds GetBounds(Point center, int x, int y)
	{
		Vector3 vector = new Vector3(this.tile_size, 0f, this.tile_size);
		if (x == -1 || y == -1)
		{
			return new Bounds(center, vector);
		}
		return new Bounds(new Vector3(center.x + vector.x * ((float)x - (float)(this.grid_size.x - 1) / 2f), 0f, center.y + vector.z * ((float)y - (float)(this.grid_size.y - 1) / 2f)), vector);
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x000B5658 File Offset: 0x000B3858
	public static Bounds GetBounds(Point center, int x, int y, Vector2Int grid_size, float tile_size_x, float tile_size_y)
	{
		Vector3 vector = new Vector3(tile_size_x, 0f, tile_size_y);
		if (x == -1 || y == -1)
		{
			return new Bounds(center, vector);
		}
		return new Bounds(new Vector3(center.x + vector.x * ((float)x - (float)(grid_size.x - 1) / 2f), 0f, center.y + vector.z * ((float)y - (float)(grid_size.y - 1) / 2f)), vector);
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x000B56DC File Offset: 0x000B38DC
	public void Clear()
	{
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			this.terrain_infos[i].cells = null;
		}
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x000B5714 File Offset: 0x000B3914
	private void CalcSplatmapBounds(TerrainData td, Point center, int x, int y, out Vector2Int tile, out Vector2Int size)
	{
		PrefabGrid.CalcSplatmapBounds(td, center, x, y, this.grid_size, this.tile_size, this.tile_size, out tile, out size);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x000B5744 File Offset: 0x000B3944
	private static void CalcSplatmapBounds(TerrainData td, Point center, int x, int y, Vector2Int grid_size, float tile_size_x, float tile_size_y, out Vector2Int tile, out Vector2Int size)
	{
		Bounds bounds = PrefabGrid.GetBounds(center, x, y, grid_size, tile_size_x, tile_size_y);
		Vector2Int resolution = BSGTerrainTools.SplatsResolution(td);
		BSGTerrainTools.CalcGridBounds(bounds, td.bounds, resolution, out tile, out size);
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x000B5776 File Offset: 0x000B3976
	private BSGTerrainTools.Float3D CopySplats(TerrainData td, Point center, int x, int y)
	{
		return PrefabGrid.CopySplats(td, center, x, y, this.grid_size, this.tile_size, this.tile_size);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x000B5794 File Offset: 0x000B3994
	private static BSGTerrainTools.Float3D CopySplats(TerrainData td, Point center, int x, int y, Vector2Int grid_size, float tile_size_x, float tile_size_y)
	{
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		PrefabGrid.CalcSplatmapBounds(td, center, x, y, grid_size, tile_size_x, tile_size_y, out vector2Int, out vector2Int2);
		return BSGTerrainTools.Float3D.ImportSplats(td, vector2Int.x, vector2Int.y, vector2Int2.x, vector2Int2.y);
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x000B57D6 File Offset: 0x000B39D6
	private List<TreeInstance> CopyTrees(TerrainData td, Point center, int x, int y)
	{
		return PrefabGrid.CopyTrees(td, center, x, y, this.grid_size, this.tile_size, this.tile_size);
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x000B57F4 File Offset: 0x000B39F4
	private static List<TreeInstance> CopyTrees(TerrainData td, Point center, int x, int y, Vector2Int grid_size, float tile_size_x, float tile_size_y)
	{
		List<TreeInstance> list = new List<TreeInstance>();
		Bounds bounds = PrefabGrid.GetBounds(center, x, y, grid_size, tile_size_x, tile_size_y);
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		for (int i = 0; i < td.treeInstanceCount; i++)
		{
			TreeInstance treeInstance = td.GetTreeInstance(i);
			Vector3 vector = Vector3.Scale(treeInstance.position, td.size);
			if (vector.x > min.x && vector.x < max.x && vector.z > min.z && vector.z < max.z)
			{
				vector = new Vector3(vector.x - bounds.center.x, vector.y, vector.z - bounds.center.z);
				vector = new Vector3(vector.x / td.size.x, vector.y / td.size.y, vector.z / td.size.z);
				treeInstance.position = vector;
				list.Add(treeInstance);
			}
		}
		return list;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x000B5930 File Offset: 0x000B3B30
	private void CreateTerrainCells()
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			BSGTerrainTools.PerTerrainInfo.PerCellInfo[,] array = perTerrainInfo.cells;
			int num = 0;
			int num2 = 0;
			if (array != null)
			{
				num = array.GetLength(0);
				num2 = array.GetLength(1);
			}
			if (array == null || num != this.grid_size.x || num2 != this.grid_size.y)
			{
				perTerrainInfo.cells = new BSGTerrainTools.PerTerrainInfo.PerCellInfo[this.grid_size.x, this.grid_size.y];
				for (int j = 0; j < this.grid_size.x; j++)
				{
					for (int k = 0; k < this.grid_size.y; k++)
					{
						BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo;
						if (array != null && num > j && num2 > k)
						{
							perCellInfo = array[j, k];
						}
						else
						{
							perCellInfo = new BSGTerrainTools.PerTerrainInfo.PerCellInfo();
						}
						perTerrainInfo.cells[j, k] = perCellInfo;
					}
				}
			}
		}
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x000B5A40 File Offset: 0x000B3C40
	public void Copy(List<BSGTerrainTools.PerTerrainInfo> terrain_infos, Point center)
	{
		if (terrain_infos == null)
		{
			terrain_infos = this.terrain_infos;
		}
		if (terrain_infos == null)
		{
			return;
		}
		this.CreateTerrainCells();
		for (int i = 0; i < terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = terrain_infos[i];
			for (int j = 0; j < this.grid_size.x; j++)
			{
				for (int k = 0; k < this.grid_size.y; k++)
				{
					BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = perTerrainInfo.cells[j, k];
					perCellInfo.splats = this.CopySplats(perTerrainInfo.td, center, j, k);
					perCellInfo.trees = this.CopyTrees(perTerrainInfo.td, center, j, k);
				}
			}
		}
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x000B5AE0 File Offset: 0x000B3CE0
	public static BSGTerrainTools.PerTerrainInfo.PerCellInfo CopySingle(PrefabGrid.Info pg_info, BSGTerrainTools.PerTerrainInfo terrain_info, Point center, int picked_x = -1, int picked_y = -1)
	{
		if (terrain_info == null || center == Point.Zero)
		{
			return null;
		}
		BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = new BSGTerrainTools.PerTerrainInfo.PerCellInfo();
		if (picked_x < 0)
		{
			picked_x = 0;
		}
		if (picked_y < 0)
		{
			picked_y = 0;
		}
		float tile_size_x;
		float tile_size_y;
		pg_info.GetSplatSize(out tile_size_x, out tile_size_y);
		perCellInfo.splats = PrefabGrid.CopySplats(terrain_info.td, center, picked_x, picked_y, pg_info.grid_size, tile_size_x, tile_size_y);
		perCellInfo.trees = PrefabGrid.CopyTrees(terrain_info.td, center, picked_x, picked_y, pg_info.grid_size, tile_size_x, tile_size_y);
		return perCellInfo;
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x000B5B58 File Offset: 0x000B3D58
	public BSGTerrainTools.PerTerrainInfo.PerCellInfo CopySingle(BSGTerrainTools.PerTerrainInfo terrain_info, Point center, int picked_x = -1, int picked_y = -1)
	{
		if (terrain_info == null || center == Point.Zero)
		{
			return null;
		}
		BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = new BSGTerrainTools.PerTerrainInfo.PerCellInfo();
		if (picked_x < 0)
		{
			picked_x = 0;
		}
		if (picked_y < 0)
		{
			picked_y = 0;
		}
		perCellInfo.splats = this.CopySplats(terrain_info.td, center, picked_x, picked_y);
		perCellInfo.trees = this.CopyTrees(terrain_info.td, center, picked_x, picked_y);
		return perCellInfo;
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x000B5BB8 File Offset: 0x000B3DB8
	public static BSGTerrainTools.PerTerrainInfo.PerCellInfo CopySingle(BSGTerrainTools.PerTerrainInfo terrain_info, Point center, Vector2Int grid_size, float tile_size_x, float tile_size_y, int picked_x = -1, int picked_y = -1)
	{
		if (terrain_info == null || center == Point.Zero)
		{
			return null;
		}
		BSGTerrainTools.PerTerrainInfo.PerCellInfo perCellInfo = new BSGTerrainTools.PerTerrainInfo.PerCellInfo();
		if (picked_x < 0)
		{
			picked_x = 0;
		}
		if (picked_y < 0)
		{
			picked_y = 0;
		}
		perCellInfo.splats = PrefabGrid.CopySplats(terrain_info.td, center, picked_x, picked_y, grid_size, tile_size_x, tile_size_y);
		perCellInfo.trees = PrefabGrid.CopyTrees(terrain_info.td, center, picked_x, picked_y, grid_size, tile_size_x, tile_size_y);
		return perCellInfo;
	}

	// Token: 0x0600111E RID: 4382 RVA: 0x000B5C20 File Offset: 0x000B3E20
	public void Paste(List<BSGTerrainTools.PerTerrainInfo> terrain_infos, Point center, bool transparent = false)
	{
		if (terrain_infos == null)
		{
			terrain_infos = this.terrain_infos;
		}
		if (terrain_infos == null)
		{
			return;
		}
		this.CreateTerrainCells();
		if (!transparent)
		{
			this.Erase(center);
		}
		for (int i = 0; i < terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = terrain_infos[i];
			BSGTerrainTools.ApplySplatMaps(perTerrainInfo.cells, this, perTerrainInfo.td, perTerrainInfo.td);
		}
	}

	// Token: 0x0600111F RID: 4383 RVA: 0x000B5C80 File Offset: 0x000B3E80
	private float[,,] EmptySplat(BSGTerrainTools.PerTerrainInfo t, Point center)
	{
		int alphamapLayers = t.td.alphamapLayers;
		Vector2Int vector2Int;
		Vector2Int vector2Int2;
		this.CalcSplatmapBounds(t.td, center, -1, -1, out vector2Int, out vector2Int2);
		float[,,] array = new float[vector2Int2.y, vector2Int2.x, alphamapLayers];
		for (int i = 0; i < vector2Int2.x; i++)
		{
			for (int j = 0; j < vector2Int2.y; j++)
			{
				array[j, i, 0] = 1f;
			}
		}
		for (int k = 1; k < alphamapLayers; k++)
		{
			for (int l = 0; l < vector2Int2.x; l++)
			{
				for (int m = 0; m < vector2Int2.y; m++)
				{
					array[m, l, k] = 0f;
				}
			}
		}
		return array;
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x000B5D4C File Offset: 0x000B3F4C
	public void Erase(Point center)
	{
		if (this.terrain_infos == null)
		{
			return;
		}
		this.CreateTerrainCells();
		for (int i = 0; i < this.terrain_infos.Count; i++)
		{
			BSGTerrainTools.PerTerrainInfo perTerrainInfo = this.terrain_infos[i];
			int alphamapLayers = this.terrain_infos[i].td.alphamapLayers;
			float[,,] arr = this.EmptySplat(perTerrainInfo, center);
			for (int j = 0; j < this.grid_size.x; j++)
			{
				for (int k = 0; k < this.grid_size.y; k++)
				{
					perTerrainInfo.cells[j, k].bg_splats = new BSGTerrainTools.Float3D(arr);
				}
			}
			BSGTerrainTools.EraseSplatMaps(perTerrainInfo.cells, this, perTerrainInfo.td, perTerrainInfo.td);
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x000B5E12 File Offset: 0x000B4012
	public void Cut()
	{
		this.Copy(null, base.transform.position);
		this.Erase(base.transform.position);
	}

	// Token: 0x04000B2A RID: 2858
	private static bool use_battle_view = false;

	// Token: 0x04000B2B RID: 2859
	public static string BV_Dir = "BattleView";

	// Token: 0x04000B2C RID: 2860
	public static string WV_Dir = "Europe";

	// Token: 0x04000B2D RID: 2861
	public const string BuildPGDefsDir = "PGDefs";

	// Token: 0x04000B2E RID: 2862
	public const string RootDir = "Assets/";

	// Token: 0x04000B2F RID: 2863
	public const string PropsDir = "Props";

	// Token: 0x04000B30 RID: 2864
	public string type = "";

	// Token: 0x04000B31 RID: 2865
	public string architecture = "";

	// Token: 0x04000B32 RID: 2866
	public string cur_type = "";

	// Token: 0x04000B33 RID: 2867
	public string cur_architecture = "";

	// Token: 0x04000B34 RID: 2868
	public static string default_architecture = "Roman";

	// Token: 0x04000B35 RID: 2869
	private static readonly string[] layers_to_ignore_names = new string[]
	{
		"Vegetation",
		"PassableSettlement",
		"ImpassableSettlement"
	};

	// Token: 0x04000B36 RID: 2870
	private static int[] _layers_to_ignore;

	// Token: 0x04000B37 RID: 2871
	public PrefabGrid.Info info;

	// Token: 0x04000B38 RID: 2872
	private List<List<PrefabGrid.Cell>> cells;

	// Token: 0x04000B39 RID: 2873
	private GameObject root;

	// Token: 0x04000B3A RID: 2874
	public int set_variant;

	// Token: 0x04000B3B RID: 2875
	public int set_level;

	// Token: 0x04000B3C RID: 2876
	public int cur_variant = 1;

	// Token: 0x04000B3D RID: 2877
	public int cur_level = 1;

	// Token: 0x04000B3E RID: 2878
	public bool expanded;

	// Token: 0x04000B3F RID: 2879
	public float hex_size;

	// Token: 0x04000B40 RID: 2880
	public float rect_width;

	// Token: 0x04000B41 RID: 2881
	public float rect_height;

	// Token: 0x04000B42 RID: 2882
	public bool use_rect_for_splats;

	// Token: 0x04000B43 RID: 2883
	public Vector2Int grid_size = Vector2Int.zero;

	// Token: 0x04000B44 RID: 2884
	public float tile_size;

	// Token: 0x04000B45 RID: 2885
	public PrefabGrid.Info.CapturePoint capture_point;

	// Token: 0x04000B46 RID: 2886
	public bool preview;

	// Token: 0x04000B47 RID: 2887
	[NonSerialized]
	public bool settings_changed;

	// Token: 0x04000B48 RID: 2888
	[NonSerialized]
	public bool modified;

	// Token: 0x04000B49 RID: 2889
	[NonSerialized]
	public List<PrefabGrid> child_pgs;

	// Token: 0x04000B4A RID: 2890
	public static HashSet<PrefabGrid> root_pgs = new HashSet<PrefabGrid>();

	// Token: 0x04000B4B RID: 2891
	private UnityEngine.Component _parent;

	// Token: 0x04000B4C RID: 2892
	[NonSerialized]
	public int chooser_version;

	// Token: 0x04000B4D RID: 2893
	private static List<UnityEngine.Component> s_components = new List<UnityEngine.Component>(64);

	// Token: 0x04000B4E RID: 2894
	private static PrefabGrid currently_spawning = null;

	// Token: 0x04000B4F RID: 2895
	private static Dictionary<string, string> nested_pgs = null;

	// Token: 0x04000B50 RID: 2896
	public float fade = 8f;

	// Token: 0x04000B51 RID: 2897
	public List<BSGTerrainTools.PerTerrainInfo> _terrain_infos;

	// Token: 0x02000660 RID: 1632
	public class Info
	{
		// Token: 0x06004789 RID: 18313 RVA: 0x00214154 File Offset: 0x00212354
		public static void Clear()
		{
			foreach (KeyValuePair<string, PrefabGrid.Info> keyValuePair in PrefabGrid.Info.registry)
			{
				keyValuePair.Value.valid = false;
			}
			PrefabGrid.nested_pgs = null;
			PrefabGrid.Info.registry.Clear();
		}

		// Token: 0x0600478A RID: 18314 RVA: 0x002141BC File Offset: 0x002123BC
		public static Dictionary<string, PrefabGrid.Info> GetRegistry()
		{
			return PrefabGrid.Info.registry;
		}

		// Token: 0x0600478B RID: 18315 RVA: 0x002141C4 File Offset: 0x002123C4
		public static PrefabGrid.Info Get(string type, string architecture, bool force_reload = false)
		{
			string key = type + "/" + architecture;
			PrefabGrid.Info info = null;
			if (PrefabGrid.Info.registry.TryGetValue(key, out info))
			{
				if (force_reload)
				{
					info.LoadSettings();
					info.LoadPrefabs();
				}
				return info;
			}
			info = new PrefabGrid.Info();
			info.type = type;
			info.architecture = architecture;
			info.valid = true;
			info.LoadSettings();
			info.LoadPrefabs();
			PrefabGrid.Info.registry.Add(key, info);
			return info;
		}

		// Token: 0x0600478C RID: 18316 RVA: 0x00214236 File Offset: 0x00212436
		public bool IsValid()
		{
			return this.max_variant > 0 && this.max_level > 0;
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x0021424C File Offset: 0x0021244C
		public GameObject GetPrefab(int variant, int level)
		{
			if (this.prefabs == null || variant <= 0 || variant > this.prefabs.Count || level <= 0 || level > this.max_level)
			{
				return null;
			}
			List<GameObject> list = this.prefabs[variant - 1];
			if (list == null || level > list.Count)
			{
				return null;
			}
			return list[level - 1];
		}

		// Token: 0x0600478E RID: 18318 RVA: 0x002142A8 File Offset: 0x002124A8
		public int GetMaxLevel(int variant)
		{
			if (this.prefabs == null || variant <= 0 || variant > this.prefabs.Count)
			{
				return 0;
			}
			List<GameObject> list = this.prefabs[variant - 1];
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		// Token: 0x0600478F RID: 18319 RVA: 0x002142EB File Offset: 0x002124EB
		public string GetTag(int variant)
		{
			if (this.tags == null || variant <= 0)
			{
				return null;
			}
			if (variant > this.tags.Count)
			{
				return null;
			}
			return this.tags[variant - 1];
		}

		// Token: 0x06004790 RID: 18320 RVA: 0x0021431C File Offset: 0x0021251C
		public void LoadPrefabs()
		{
			this.max_variant = (this.max_level = 0);
			this.prefabs = null;
			Assets.Init(false);
			Assets.DirInfo dir = Assets.GetDir(PrefabGrid.GetDir(this.type, this.architecture), false);
			if (dir == null)
			{
				return;
			}
			this.prefabs = new List<List<GameObject>>();
			List<Assets.AssetInfo> assets = dir.GetAssets();
			for (int i = 0; i < assets.Count; i++)
			{
				Assets.AssetInfo assetInfo = assets[i];
				GameObject gameObject = assetInfo.GetAsset() as GameObject;
				int num;
				int num2;
				if (!(gameObject == null) && PrefabGrid.ParsePrefabName(assetInfo.name, ".prefab", out num, out num2))
				{
					if (num > this.max_variant)
					{
						this.max_variant = num;
					}
					if (num2 > this.max_level)
					{
						this.max_level = num2;
					}
					while (this.prefabs.Count < num)
					{
						this.prefabs.Add(null);
					}
					List<GameObject> list = this.prefabs[num - 1];
					if (list == null)
					{
						list = new List<GameObject>(num2);
						this.prefabs[num - 1] = list;
					}
					while (list.Count < num2)
					{
						list.Add(null);
					}
					list[num2 - 1] = gameObject;
				}
			}
			if (this.grid_size.x <= 0)
			{
				this.grid_size.x = this.max_variant;
			}
			if (this.grid_size.y <= 0)
			{
				this.grid_size.y = this.max_level;
			}
		}

		// Token: 0x06004791 RID: 18321 RVA: 0x0021449C File Offset: 0x0021269C
		public bool LoadSettings()
		{
			if (string.IsNullOrEmpty(this.type) || string.IsNullOrEmpty(this.architecture))
			{
				return false;
			}
			string path = string.Concat(new string[]
			{
				PrefabGrid.PGDefsDir,
				"/",
				this.type,
				"/",
				this.architecture,
				"/pg.def"
			});
			List<DT.Field> list = null;
			try
			{
				list = DT.Parser.ReadFile(null, path, null);
			}
			catch
			{
			}
			if (list == null || list.Count < 1)
			{
				this.loaded = false;
				return false;
			}
			this.loaded = true;
			DT.Field field = list[0];
			this.show_hexes = field.GetBool("show_hexes", null, this.show_hexes, true, true, true, '.');
			this.hex_size = field.GetFloat("hex_size", null, this.hex_size, true, true, true, '.');
			this.show_rect = field.GetBool("show_rect", null, this.show_rect, true, true, true, '.');
			this.rect_width = field.GetFloat("rect_width", null, this.rect_width, true, true, true, '.');
			this.rect_height = field.GetFloat("rect_height", null, this.rect_height, true, true, true, '.');
			this.use_rect_for_splats = field.GetBool("use_rect_for_splats", null, this.use_rect_for_splats, true, true, true, '.');
			this.show_grid = field.GetBool("show_grid", null, this.show_grid, true, true, true, '.');
			this.grid_size.x = field.GetInt("variants", null, this.grid_size.x, true, true, true, '.');
			this.grid_size.y = field.GetInt("levels", null, this.grid_size.y, true, true, true, '.');
			this.tile_size = field.GetFloat("tile_size", null, this.tile_size, true, true, true, '.');
			this.capture_point = (PrefabGrid.Info.CapturePoint)field.GetInt("control_point", null, (int)this.capture_point, true, true, true, '.');
			if (this.capture_point == PrefabGrid.Info.CapturePoint.Error)
			{
				Game.Log(string.Format("Missing capture points in {0}", this), Game.LogType.Error);
			}
			this.architectures_position = field.GetPoint("architectures_position", null, true, true, true, '.');
			DT.Field field2 = field.FindChild("tags", null, true, true, true, '.');
			if (field2 != null && field2.children != null)
			{
				for (int i = 0; i < field2.children.Count; i++)
				{
					DT.Field field3 = field2.children[i];
					int variant;
					if (!(field3.key == "") && field3.value.is_string && int.TryParse(field3.key, out variant))
					{
						string tag = field3.value.String(null);
						this.SetTag(variant, tag);
					}
				}
			}
			return true;
		}

		// Token: 0x06004792 RID: 18322 RVA: 0x0021475C File Offset: 0x0021295C
		private void SetTag(int variant, string tag)
		{
			if (variant <= 0)
			{
				return;
			}
			if (this.tags == null)
			{
				this.tags = new List<string>(variant);
			}
			while (this.tags.Count < variant)
			{
				this.tags.Add(null);
			}
			this.tags[variant - 1] = tag;
		}

		// Token: 0x06004793 RID: 18323 RVA: 0x002147AD File Offset: 0x002129AD
		public void GetSplatSize(out float tile_x, out float tile_y)
		{
			if (this.use_rect_for_splats)
			{
				tile_x = this.rect_width;
				tile_y = this.rect_height;
				return;
			}
			tile_x = this.tile_size;
			tile_y = this.tile_size;
		}

		// Token: 0x0400353E RID: 13630
		public string type = "";

		// Token: 0x0400353F RID: 13631
		public string architecture = "";

		// Token: 0x04003540 RID: 13632
		public int max_variant;

		// Token: 0x04003541 RID: 13633
		public int max_level;

		// Token: 0x04003542 RID: 13634
		private List<List<GameObject>> prefabs;

		// Token: 0x04003543 RID: 13635
		public bool show_grid = true;

		// Token: 0x04003544 RID: 13636
		public float tile_size = 10f;

		// Token: 0x04003545 RID: 13637
		public Vector2Int grid_size = Vector2Int.zero;

		// Token: 0x04003546 RID: 13638
		public bool show_hexes = true;

		// Token: 0x04003547 RID: 13639
		public float hex_size = 5f;

		// Token: 0x04003548 RID: 13640
		public bool show_rect;

		// Token: 0x04003549 RID: 13641
		public float rect_width = 5f;

		// Token: 0x0400354A RID: 13642
		public float rect_height = 5f;

		// Token: 0x0400354B RID: 13643
		public bool use_rect_for_splats;

		// Token: 0x0400354C RID: 13644
		public Point architectures_position;

		// Token: 0x0400354D RID: 13645
		public PrefabGrid.Info.CapturePoint capture_point = PrefabGrid.Info.CapturePoint.No;

		// Token: 0x0400354E RID: 13646
		public List<string> tags;

		// Token: 0x0400354F RID: 13647
		public bool valid;

		// Token: 0x04003550 RID: 13648
		public bool loaded;

		// Token: 0x04003551 RID: 13649
		private static Dictionary<string, PrefabGrid.Info> registry = new Dictionary<string, PrefabGrid.Info>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x02000A02 RID: 2562
		public enum CapturePoint
		{
			// Token: 0x04004623 RID: 17955
			Yes,
			// Token: 0x04004624 RID: 17956
			No,
			// Token: 0x04004625 RID: 17957
			Error
		}
	}

	// Token: 0x02000661 RID: 1633
	public class Cell
	{
		// Token: 0x04003552 RID: 13650
		public GameObject root;

		// Token: 0x04003553 RID: 13651
		public bool modified;
	}

	// Token: 0x02000662 RID: 1634
	public class ChooserContext
	{
		// Token: 0x06004797 RID: 18327 RVA: 0x00214860 File Offset: 0x00212A60
		public static PrefabGrid.ChooserContext Get(PrefabGrid pg)
		{
			global::Realm realm = (pg == null) ? null : global::Realm.At(pg.transform.position);
			PrefabGrid.ChooserContext chooserContext = (realm != null) ? realm.pg_chooser : null;
			if (chooserContext == null)
			{
				chooserContext = new PrefabGrid.ChooserContext();
				chooserContext.realm = realm;
				chooserContext.Reset(false);
				if (realm != null)
				{
					realm.pg_chooser = chooserContext;
				}
			}
			return chooserContext;
		}

		// Token: 0x06004798 RID: 18328 RVA: 0x002148BC File Offset: 0x00212ABC
		public void Reset(bool full = false)
		{
			int seed = 238746;
			if (this.realm != null)
			{
				seed = this.realm.id * 1000 + this.realm.id;
			}
			this.rnd = new Random(seed);
			if (full)
			{
				this.assigned.Clear();
				this.version++;
			}
		}

		// Token: 0x06004799 RID: 18329 RVA: 0x00214920 File Offset: 0x00212B20
		public void RefreshRealmTags()
		{
			this.Reset(false);
			this.refresh_idx = 0;
			while (this.refresh_idx < this.tagged_pgs.Count)
			{
				this.tagged_pgs[this.refresh_idx].RefreshRealmTags();
				this.refresh_idx++;
			}
			this.refresh_idx = -1;
		}

		// Token: 0x0600479A RID: 18330 RVA: 0x0021497C File Offset: 0x00212B7C
		public static void RefreshRealmTags(Castle castle)
		{
			if (castle == null)
			{
				return;
			}
			PrefabGrid.ChooserContext.RefreshRealmTags(castle.GetRealm());
		}

		// Token: 0x0600479B RID: 18331 RVA: 0x00214990 File Offset: 0x00212B90
		public static void RefreshRealmTags(Logic.Realm lr)
		{
			if (lr == null)
			{
				return;
			}
			global::Realm realm = lr.visuals as global::Realm;
			if (realm == null || realm.pg_chooser == null)
			{
				return;
			}
			realm.pg_chooser.RefreshRealmTags();
		}

		// Token: 0x0600479C RID: 18332 RVA: 0x002149C4 File Offset: 0x00212BC4
		public bool HasTag(string tag)
		{
			return this.realm != null && this.realm.HasTag(tag);
		}

		// Token: 0x0600479D RID: 18333 RVA: 0x002149DC File Offset: 0x00212BDC
		public int GetTag(string tag)
		{
			if (this.realm != null)
			{
				return this.realm.GetTag(tag);
			}
			return 0;
		}

		// Token: 0x0600479E RID: 18334 RVA: 0x002149F4 File Offset: 0x00212BF4
		public int GetAssigned(string tag)
		{
			int result = 0;
			this.assigned.TryGetValue(tag, out result);
			return result;
		}

		// Token: 0x0600479F RID: 18335 RVA: 0x00214A13 File Offset: 0x00212C13
		public void IncAssigned(string tag)
		{
			this.assigned[tag] = this.GetAssigned(tag) + 1;
		}

		// Token: 0x060047A0 RID: 18336 RVA: 0x00214A2A File Offset: 0x00212C2A
		public void DecAssigned(string tag)
		{
			this.assigned[tag] = this.GetAssigned(tag) - 1;
		}

		// Token: 0x060047A1 RID: 18337 RVA: 0x00214A44 File Offset: 0x00212C44
		public static void OnDestroy(PrefabGrid pg)
		{
			if (pg.set_variant <= 0)
			{
				PrefabGrid.Info info = pg.info;
				if (((info != null) ? info.tags : null) != null)
				{
					PrefabGrid.ChooserContext chooserContext = PrefabGrid.ChooserContext.Get(pg);
					if (chooserContext == null)
					{
						return;
					}
					int num = chooserContext.tagged_pgs.IndexOf(pg);
					if (num >= 0)
					{
						chooserContext.tagged_pgs.RemoveAt(num);
						if (num < chooserContext.refresh_idx)
						{
							chooserContext.refresh_idx--;
						}
					}
					string tag = pg.info.GetTag(pg.cur_variant);
					if (tag == null)
					{
						return;
					}
					if (pg.chooser_version == chooserContext.version)
					{
						chooserContext.DecAssigned(tag);
					}
					pg.chooser_version = 0;
					return;
				}
			}
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x00214AE0 File Offset: 0x00212CE0
		private int ChooseVariant(PrefabGrid pg)
		{
			this.untagged_variants.Clear();
			this.tagged_variants.Clear();
			this.tagged_counts.Clear();
			int num = int.MaxValue;
			int num2 = int.MinValue;
			int num3 = 0;
			for (int i = 1; i <= pg.info.grid_size.x; i++)
			{
				string tag = pg.info.GetTag(i);
				if (tag == null)
				{
					this.untagged_variants.Add(i);
				}
				else
				{
					int tag2 = this.GetTag(tag);
					if (tag2 > 0)
					{
						int num4 = this.GetAssigned(tag);
						if (num4 >= tag2)
						{
							if (num3 == 0)
							{
								num3 = i;
							}
						}
						else
						{
							if (num4 < num)
							{
								num = num4;
							}
							if (num4 > num2)
							{
								num2 = num4;
							}
							this.tagged_variants.Add(i);
							this.tagged_counts.Add(num4);
						}
					}
				}
			}
			if (this.tagged_variants.Count <= 0)
			{
				if (this.untagged_variants.Count == 0)
				{
					return num3;
				}
				if (this.untagged_variants.Count == 1)
				{
					return this.untagged_variants[0];
				}
				return this.untagged_variants[this.rnd.Next(0, this.untagged_variants.Count)];
			}
			else
			{
				if (num2 > num)
				{
					int num5 = 0;
					for (int j = 0; j < this.tagged_counts.Count; j++)
					{
						if (this.tagged_counts[j] <= num)
						{
							num5++;
						}
						else
						{
							this.tagged_variants.RemoveAt(num5);
						}
					}
				}
				if (this.tagged_variants.Count <= 1)
				{
					return this.tagged_variants[0];
				}
				return this.tagged_variants[this.rnd.Next(0, this.tagged_variants.Count)];
			}
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x00214C88 File Offset: 0x00212E88
		public static int DecideVariant(PrefabGrid pg)
		{
			if (pg.set_variant > 0 && pg.set_variant <= pg.info.grid_size.x)
			{
				return pg.set_variant;
			}
			if (pg.info.grid_size.x <= 0)
			{
				return 0;
			}
			if (pg.info.tags != null)
			{
				PrefabGrid.ChooserContext chooserContext = PrefabGrid.ChooserContext.Get(pg);
				if (!chooserContext.tagged_pgs.Contains(pg))
				{
					chooserContext.tagged_pgs.Add(pg);
				}
				string tag = pg.info.GetTag(pg.cur_variant);
				if (tag != null && pg.chooser_version == chooserContext.version)
				{
					chooserContext.DecAssigned(tag);
				}
				pg.chooser_version = chooserContext.version;
				int num = chooserContext.ChooseVariant(pg);
				string tag2 = pg.info.GetTag(num);
				if (tag2 != null)
				{
					chooserContext.IncAssigned(tag2);
				}
				return num;
			}
			if (pg.cur_variant > 0 && pg.cur_variant <= pg.info.grid_size.x)
			{
				return pg.cur_variant;
			}
			return Random.Range(1, pg.info.grid_size.x + 1);
		}

		// Token: 0x04003554 RID: 13652
		public global::Realm realm;

		// Token: 0x04003555 RID: 13653
		public List<PrefabGrid> tagged_pgs = new List<PrefabGrid>();

		// Token: 0x04003556 RID: 13654
		public int refresh_idx = -1;

		// Token: 0x04003557 RID: 13655
		public Dictionary<string, int> assigned = new Dictionary<string, int>();

		// Token: 0x04003558 RID: 13656
		public Random rnd;

		// Token: 0x04003559 RID: 13657
		public int version = 1;

		// Token: 0x0400355A RID: 13658
		private List<int> untagged_variants = new List<int>();

		// Token: 0x0400355B RID: 13659
		private List<int> tagged_variants = new List<int>();

		// Token: 0x0400355C RID: 13660
		private List<int> tagged_counts = new List<int>();
	}
}
