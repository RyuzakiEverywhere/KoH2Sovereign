using System;
using System.Collections.Generic;
using System.IO;
using Logic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200016B RID: 363
[ExecuteInEditMode]
[RequireComponent(typeof(global::HexGrid))]
public class HexGroups : MonoBehaviour
{
	// Token: 0x06001276 RID: 4726 RVA: 0x000C127A File Offset: 0x000BF47A
	public static global::HexGroups Get()
	{
		return global::HexGroups.instance;
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x000C1284 File Offset: 0x000BF484
	private void OnEnable()
	{
		global::HexGroups.instance = this;
		if (this.groupsAsset == null)
		{
			string scenePath = global::HexGroups.GetScenePath();
			this.groupsAsset = Assets.Get<TextAsset>(scenePath + ".groups.bytes");
		}
		if (this.groupsAsset != null)
		{
			this.groups = global::HexGroups.Load(this.groupsAsset, false);
		}
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x000C12E1 File Offset: 0x000BF4E1
	private void Start()
	{
		this.LoadDefs();
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x000C12E9 File Offset: 0x000BF4E9
	private void OnDisable()
	{
		if (global::HexGroups.instance == this)
		{
			global::HexGroups.instance = null;
		}
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x000C12FE File Offset: 0x000BF4FE
	public static Logic.HexGrid Grid()
	{
		if (global::HexGroups.instance == null || global::HexGrid.Grid() == null)
		{
			return null;
		}
		return global::HexGrid.Grid();
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x000C131C File Offset: 0x000BF51C
	public void LoadDefs()
	{
		DT.Field defField = global::Defs.GetDefField("Hex", null);
		if (defField == null)
		{
			Debug.LogWarning("No def");
			return;
		}
		DT.Field field = defField.FindChild("hex_shapes", null, true, true, true, '.');
		if (field == null)
		{
			return;
		}
		List<DT.Field> list = field.Children();
		if (list.Count == 0)
		{
			Debug.LogWarning("Invalid def");
			return;
		}
		string[] array = field.Keys(true, true).ToArray();
		for (int i = 0; i < list.Count; i++)
		{
			string[] array2 = list[i].value_str.Replace('"', ' ').Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			this.shapes[array[i]] = new List<Logic.HexGrid.Coord>();
			for (int j = 0; j < array2.Length; j++)
			{
				string[] array3 = array2[j].Split(new char[]
				{
					','
				});
				this.shapes[array[i]].Add(new Logic.HexGrid.Coord(int.Parse(array3[0]), int.Parse(array3[1])));
			}
		}
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x000C1434 File Offset: 0x000BF634
	public void CreateGroup(List<Logic.HexGrid.Coord> selectedNodes, string originalShape, int rotation = 0, bool locked = false, string nodeType = "")
	{
		for (int i = 0; i < selectedNodes.Count; i++)
		{
			Logic.HexGrid.Coord c = selectedNodes[i];
			if (this.groups.Contains(c))
			{
				this.groups.Remove(this.groups.GetGroup(c));
			}
		}
		HexGroup grp = new HexGroup(selectedNodes, originalShape, rotation, locked, nodeType);
		this.groups.Add(grp);
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x000C1498 File Offset: 0x000BF698
	public static void Save(BinaryWriter writer)
	{
		if (global::HexGroups.instance == null)
		{
			writer.Write(0);
			return;
		}
		writer.Write(global::HexGroups.instance.groups.Count);
		for (int i = 0; i < global::HexGroups.instance.groups.Count; i++)
		{
			HexGroup hexGroup = global::HexGroups.instance.groups[i];
			writer.Write(hexGroup.Count);
			for (int j = 0; j < hexGroup.Count; j++)
			{
				writer.Write(hexGroup.hexes[j].x);
				writer.Write(hexGroup.hexes[j].y);
			}
			writer.Write(hexGroup.rotation);
			writer.Write(hexGroup.locked);
			writer.Write(hexGroup.nodeType);
			writer.Write(hexGroup.originalShape);
		}
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x000C157C File Offset: 0x000BF77C
	private Logic.HexGroups LoadGroups(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		if (num > 1000)
		{
			Debug.LogError("Attempting to load " + num + " hex groups, data is corrupted!");
			num = 0;
		}
		List<HexGroup> list = new List<HexGroup>(num);
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt32();
			if (num2 > 1000)
			{
				Debug.LogError("Attempting to load " + num2 + " hex group tiles, data is corrupted!");
				break;
			}
			HexGroup hexGroup = new HexGroup(new List<Logic.HexGrid.Coord>(num2), "", 0, false, "");
			for (int j = 0; j < num2; j++)
			{
				Logic.HexGrid.Coord item = default(Logic.HexGrid.Coord);
				item.x = reader.ReadInt32();
				item.y = reader.ReadInt32();
				hexGroup.hexes.Add(item);
			}
			hexGroup.rotation = reader.ReadInt32();
			hexGroup.locked = reader.ReadBoolean();
			hexGroup.nodeType = reader.ReadString();
			hexGroup.originalShape = reader.ReadString();
			list.Add(hexGroup);
		}
		return new Logic.HexGroups(list);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x000C1698 File Offset: 0x000BF898
	public static Logic.HexGroups Load(TextAsset asset, bool set = false)
	{
		if (global::HexGroups.instance == null)
		{
			return null;
		}
		if (asset == null)
		{
			Logic.HexGroups result = new Logic.HexGroups();
			if (set)
			{
				global::HexGroups.instance.groups = result;
				global::HexGrid.Get().GenerateTexture(false);
			}
			return result;
		}
		Logic.HexGroups result2;
		using (MemoryStream memoryStream = new MemoryStream(asset.bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				Logic.HexGroups hexGroups = global::HexGroups.instance.LoadGroups(binaryReader);
				if (set)
				{
					global::HexGroups.instance.groups = hexGroups;
					global::HexGrid.Get().GenerateTexture(false);
				}
				result2 = hexGroups;
			}
		}
		return result2;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x000C1750 File Offset: 0x000BF950
	public void Reload()
	{
		if (this.groupsAsset != null)
		{
			global::HexGroups.Load(this.groupsAsset, true);
		}
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x000C1770 File Offset: 0x000BF970
	public void SaveGroups(string path)
	{
		using (FileStream fileStream = new FileStream(path + ".groups.bytes", FileMode.Create))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
			{
				global::HexGroups.Save(binaryWriter);
			}
		}
		if (this.groupsAsset == null)
		{
			this.groupsAsset = Assets.Get<TextAsset>(path + ".groups.bytes");
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x000C17F4 File Offset: 0x000BF9F4
	public static string GetScenePath()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		return Path.GetDirectoryName(activeScene.path) + "/" + Path.GetFileNameWithoutExtension(activeScene.path);
	}

	// Token: 0x04000C69 RID: 3177
	private static global::HexGroups instance;

	// Token: 0x04000C6A RID: 3178
	public TextAsset groupsAsset;

	// Token: 0x04000C6B RID: 3179
	public Logic.HexGroups groups = new Logic.HexGroups();

	// Token: 0x04000C6C RID: 3180
	public Dictionary<string, List<Logic.HexGrid.Coord>> shapes = new Dictionary<string, List<Logic.HexGrid.Coord>>();
}
