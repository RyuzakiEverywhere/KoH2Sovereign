using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class ResourcesView : PoliticalView
{
	// Token: 0x06000BD5 RID: 3029 RVA: 0x00084E9C File Offset: 0x0008309C
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.zeroChance = field.GetInt("zeroChance", null, 0, true, true, true, '.');
		this.numResources = field.GetInt("numResources", null, 0, true, true, true, '.');
		this.randomExtraNumMin = field.GetInt("randomExtraNumMin", null, 0, true, true, true, '.');
		this.randomExtraNumMax = field.GetInt("randonExtraNumMax", null, 0, true, true, true, '.');
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00084F10 File Offset: 0x00083110
	public void RandomizeColors()
	{
		this.LoadDef(global::Defs.GetDefField(this.def_id, null));
		string text = "Realm;Resource;NumResources;numNeighbors";
		WorldMap worldMap = WorldMap.Get();
		if (worldMap == null)
		{
			return;
		}
		List<global::Realm> realms = worldMap.Realms;
		this.colors = new List<Color>();
		this.resources = new List<int>();
		this.unResourcesNums = new List<int>();
		int num = 0;
		while (num < realms.Count && !realms[num].IsSeaRealm())
		{
			if (Random.Range(1, 100) < this.zeroChance)
			{
				this.resources.Add(0);
			}
			else
			{
				this.resources.Add(Random.Range(1, this.numResources));
			}
			num++;
		}
		int num2 = int.MaxValue;
		int num3 = 0;
		for (int i = 0; i < realms.Count; i++)
		{
			if (!realms[i].IsSeaRealm())
			{
				int num4 = 0;
				HashSet<int> hashSet = new HashSet<int>();
				hashSet.Add(this.resources[i]);
				if (this.resources[i] != 0)
				{
					num4++;
					hashSet.Add(0);
				}
				int num5 = 0;
				bool flag = false;
				foreach (Logic.Realm realm in realms[i].logic.neighbors)
				{
					if (realm.IsSeaRealm())
					{
						flag = true;
					}
					else
					{
						num5++;
						if (!hashSet.Contains(this.resources[realm.id - 1]))
						{
							num4++;
							hashSet.Add(this.resources[realm.id - 1]);
						}
					}
				}
				int num6 = num4 + (flag ? Random.Range(this.randomExtraNumMin, this.randomExtraNumMax) : 0);
				this.unResourcesNums.Add(num6);
				text = string.Concat(new object[]
				{
					text,
					"\n",
					realms[i].Name,
					";",
					this.resources[i],
					";",
					num6,
					";",
					num5
				});
				num2 = Mathf.Min(num2, num4);
				num3 = Mathf.Max(num3, num4);
			}
		}
		for (int j = 0; j < realms.Count; j++)
		{
			if (!realms[j].IsSeaRealm())
			{
				float num7 = (float)(this.unResourcesNums[j] - num2) / (float)(num3 - num2);
				float num8 = num7;
				float num9 = 1f - num7;
				float num10 = Mathf.Min(Mathf.Min(1f, 1f - num8), 1f - num9);
				this.colors.Add(new Color(num9 + num10, num8 + num10, 0f, 1f));
			}
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x0008521C File Offset: 0x0008341C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		if (this.colors == null || this.colors.Count == 0)
		{
			this.RandomizeColors();
		}
		if (this.resources != null && this.unResourcesNums != null)
		{
			LabelUpdater.Get(true).UpdateResourceLabels(this.resources, this.unResourcesNums);
		}
		WorldUI worldUI = WorldUI.Get();
		global::Settlement settlement = (worldUI != null && worldUI.selected_obj != null) ? worldUI.selected_obj.GetComponent<global::Settlement>() : null;
		for (int i = 1; i <= this.realms.Count; i++)
		{
			global::Realm realm = this.realms[i - 1];
			Color newColor = (settlement != null && settlement.IsCastle() && settlement.GetRealmID() == realm.id) ? Color.blue : (this.realms[i - 1].IsSeaRealm() ? Color.black : this.colors[i - 1]);
			this.SetRealmColor(i, newColor);
		}
	}

	// Token: 0x0400093D RID: 2365
	public List<Color> colors;

	// Token: 0x0400093E RID: 2366
	public List<int> resources;

	// Token: 0x0400093F RID: 2367
	public List<int> unResourcesNums;

	// Token: 0x04000940 RID: 2368
	private int zeroChance;

	// Token: 0x04000941 RID: 2369
	private int numResources;

	// Token: 0x04000942 RID: 2370
	private int randomExtraNumMin;

	// Token: 0x04000943 RID: 2371
	private int randomExtraNumMax;
}
