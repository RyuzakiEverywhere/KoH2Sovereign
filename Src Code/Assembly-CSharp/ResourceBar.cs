using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000173 RID: 371
public static class ResourceBar
{
	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060012FC RID: 4860 RVA: 0x000C5FB6 File Offset: 0x000C41B6
	public static ResourceBar.Def def
	{
		get
		{
			return ResourceBar.Def.Get();
		}
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x000C5FC0 File Offset: 0x000C41C0
	private static void AddIcons(global::Kingdom k, global::Realm r, ResourceType rt, int count, float scale = 1f, bool separate = true)
	{
		if (count == 0)
		{
			return;
		}
		bool negative;
		if (count < 0)
		{
			negative = true;
			count = -count;
		}
		else
		{
			negative = false;
		}
		if (ResourceBar.tmp_icons.Count == 0)
		{
			separate = false;
		}
		ResourceBar.tmp_icons.Add(new ResourceBar.IconInfo
		{
			rt = ResourceBar.GetResourceIconIndex(r, rt, negative),
			count = count,
			negative = negative,
			scale = scale,
			overlap_perc = ResourceBar.def.CalcOverlapPerc(rt, count),
			separate = separate
		});
		ResourceBar.tmp_count += count;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000C6054 File Offset: 0x000C4254
	private static int GetResourceIconIndex(global::Realm r, ResourceType rt, bool negative = false)
	{
		if (rt == ResourceType.Piety)
		{
			return ResourceBar.GetPietyIndex(r.GetKingdom(), negative);
		}
		if (rt == ResourceType.Trade)
		{
			return ResourceBar.GetTradeIndex(r);
		}
		if (rt == ResourceType.Workers && r.logic != null && r.logic.IsDisorder())
		{
			return 21;
		}
		if (rt == ResourceType.Rebels && r.logic != null && r.logic.IsDisorder())
		{
			return 22;
		}
		if (rt == ResourceType.Levy)
		{
			return ResourceBar.GetLevyIndex(r);
		}
		return (int)rt;
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x000C60C3 File Offset: 0x000C42C3
	private static int GetGuardIndex(global::Realm r)
	{
		if (r.logic == null || !r.logic.IsDisorder())
		{
			return 7;
		}
		return 25;
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x000C60DE File Offset: 0x000C42DE
	private static int GetLevyIndex(global::Realm r)
	{
		if (r.logic == null || !r.logic.IsDisorder())
		{
			return 8;
		}
		return 23;
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x000C60F9 File Offset: 0x000C42F9
	private static int GetTradeIndex(global::Realm r)
	{
		if (r == null || r.logic == null)
		{
			return 6;
		}
		if (r.logic.IsTradeCenter())
		{
			return 20;
		}
		return 6;
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000C611C File Offset: 0x000C431C
	private static int GetPietyIndex(global::Kingdom kingdom, bool negative = false)
	{
		if (kingdom == null || kingdom.logic == null)
		{
			if (!negative)
			{
				return 5;
			}
			return 28;
		}
		else
		{
			Logic.Kingdom logic = kingdom.logic;
			if (logic.is_catholic && !logic.excommunicated)
			{
				if (!negative)
				{
					return 13;
				}
				return 26;
			}
			else if (logic.is_catholic && logic.excommunicated)
			{
				if (!negative)
				{
					return 14;
				}
				return 27;
			}
			else if (logic.is_orthodox && !logic.subordinated)
			{
				if (!negative)
				{
					return 16;
				}
				return 29;
			}
			else if (logic.is_orthodox && (logic.subordinated || logic.is_ecumenical_patriarchate))
			{
				if (!negative)
				{
					return 15;
				}
				return 28;
			}
			else if (logic.is_sunni)
			{
				if (!negative)
				{
					return 17;
				}
				return 30;
			}
			else if (logic.is_shia)
			{
				if (!negative)
				{
					return 18;
				}
				return 31;
			}
			else
			{
				if (!logic.is_pagan)
				{
					return 5;
				}
				if (!negative)
				{
					return 19;
				}
				return 32;
			}
		}
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x000C61E4 File Offset: 0x000C43E4
	private static void CalcIcons(global::Settlement settlement)
	{
		ResourceBar.tmp_icons.Clear();
		ResourceBar.tmp_count = 0;
		if (ResourceBar.hidden >= 2)
		{
			return;
		}
		global::Kingdom kingdom = global::Kingdom.Get(settlement.GetKingdomID());
		global::Realm realm = settlement.GetRealm();
		if (realm == null)
		{
			return;
		}
		if (settlement.IsCastle())
		{
			Castle castle = settlement.logic as Castle;
			int num;
			int num2;
			int num3;
			int num4;
			if (castle == null)
			{
				num = 0;
				num2 = 0;
				num3 = settlement.level / 3;
				num4 = settlement.GetMaxLevel() / 3;
			}
			else
			{
				castle.population.Recalc(false);
				num = castle.population.Count(Population.Type.Rebel, true);
				num2 = castle.population.Slots(Population.Type.Rebel, true);
				num3 = castle.population.Count(Population.Type.Worker, true);
				num4 = castle.population.Slots(Population.Type.Worker, true);
			}
			if (num4 < num3)
			{
				num4 = num3;
			}
			int? num5;
			if (realm == null)
			{
				num5 = null;
			}
			else
			{
				Logic.Realm logic = realm.logic;
				num5 = ((logic != null) ? new int?(logic.CalcVisibleBy(BaseUI.LogicKingdom(), true)) : null);
			}
			if ((num5 ?? 2) > 0)
			{
				Resource r;
				if (realm == null)
				{
					r = null;
				}
				else
				{
					Logic.Realm logic2 = realm.logic;
					r = ((logic2 != null) ? logic2.income : null);
				}
				if (r != null)
				{
					ResourceBar.AddIcons(kingdom, realm, ResourceType.TownGuards, (int)realm.logic.income.Get(ResourceType.TownGuards), 1f, true);
				}
				ResourceBar.AddIcons(kingdom, realm, ResourceType.Rebels, num, 1f, true);
				ResourceBar.AddIcons(kingdom, realm, ResourceType.RebelsSlots, num2 - num, 1f, true);
				ResourceBar.AddIcons(kingdom, realm, ResourceType.Workers, num3, 1f, true);
				ResourceBar.AddIcons(kingdom, realm, ResourceType.WorkerSlots, num4 - num3, 1f, true);
				return;
			}
		}
		else
		{
			if (ResourceBar.hidden >= 1)
			{
				return;
			}
			int num6 = 0;
			if (settlement.logic == null)
			{
				PerLevelValues perLevelValues = PerLevelValues.Parse<Resource>(global::Defs.GetDefField(settlement.setType, "production"), null, false);
				if (perLevelValues != null)
				{
					num6 = settlement.GetLevel();
					perLevelValues.GetResources(num6, false, true);
					Value per_level = perLevelValues.per_level;
				}
			}
			else
			{
				Resource production_flat = settlement.logic.production_flat;
				Resource production_from_buildings = settlement.logic.production_from_buildings;
				Resource production_per_level = settlement.logic.production_per_level;
				num6 = settlement.logic.level;
			}
			if (num6 <= 0)
			{
				return;
			}
			Logic.Settlement logic3 = settlement.logic;
			Resource resource = (logic3 != null) ? logic3.GetResources(false) : null;
			for (ResourceType resourceType = ResourceType.None; resourceType < ResourceType.Workers; resourceType++)
			{
				int count = (resource == null) ? 0 : ((int)resource[resourceType]);
				ResourceBar.AddIcons(kingdom, realm, resourceType, count, 1f, true);
			}
			if (((kingdom != null) ? kingdom.logic : null) != null && settlement.logic.level > 0)
			{
				Logic.Settlement logic4 = settlement.logic;
				if (!logic4.GetRealm().IsOccupied() && logic4.level > 0)
				{
					int num7 = (int)resource.Get(ResourceType.WorkerSlots);
					if (num7 > 0)
					{
						ResourceBar.AddIcons(kingdom, realm, ResourceType.Workers, num7, 1f, true);
					}
				}
			}
		}
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x000C64B8 File Offset: 0x000C46B8
	private static bool IconsDiffer(global::Settlement settlement)
	{
		List<ResourceBar.IconInfo> res_icons = settlement.res_icons;
		if (res_icons == null || res_icons.Count != ResourceBar.tmp_icons.Count)
		{
			return true;
		}
		for (int i = 0; i < res_icons.Count; i++)
		{
			ResourceBar.IconInfo iconInfo = ResourceBar.tmp_icons[i];
			ResourceBar.IconInfo icon = res_icons[i];
			if (iconInfo.Differs(icon))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x000C6515 File Offset: 0x000C4715
	private static void CopyIcons(global::Settlement settlement)
	{
		if (settlement.res_icons == null)
		{
			settlement.res_icons = new List<ResourceBar.IconInfo>(ResourceBar.tmp_icons.Count);
		}
		else
		{
			settlement.res_icons.Clear();
		}
		settlement.res_icons.AddRange(ResourceBar.tmp_icons);
	}

	// Token: 0x06001306 RID: 4870 RVA: 0x000C6551 File Offset: 0x000C4751
	private static bool UpdateIcons(global::Settlement settlement)
	{
		ResourceBar.CalcIcons(settlement);
		if (!ResourceBar.IconsDiffer(settlement))
		{
			return false;
		}
		ResourceBar.CopyIcons(settlement);
		return true;
	}

	// Token: 0x06001307 RID: 4871 RVA: 0x000C656A File Offset: 0x000C476A
	public static bool Refresh(global::Settlement settlement)
	{
		if (!ResourceBar.UpdateIcons(settlement) && settlement.defs_version == global::Defs.Version)
		{
			return false;
		}
		settlement.defs_version = global::Defs.Version;
		settlement.CreateLabel(true, false);
		return true;
	}

	// Token: 0x06001308 RID: 4872 RVA: 0x000C6598 File Offset: 0x000C4798
	private static float AddIcon(int rt, ref float x, float scale, float overlap_perc, int idx, Color color, Vector3[] vertices, Color[] colors, Vector2[] uvs, int[] triangles, bool first_to_front = true)
	{
		ResourceBar.ResInfo resInfo = ResourceBar.res_info[rt];
		float num = (float)ResourceBar.def.tex_size / (float)ResourceBar.def.tex_grid;
		int num2 = triangles.Length - 1;
		int num3 = uvs.Length - 1;
		int num4 = vertices.Length - 1;
		float num5 = (float)resInfo.w * ResourceBar.def.w2u * scale;
		float num6 = ResourceBar.def.cell_height * scale;
		float num7 = ResourceBar.def.baseline * (1f - scale) * num6 / num;
		float z = 0f;
		float num8 = (float)resInfo.cx * num + (num - (float)resInfo.w) / 2f;
		float num9 = (float)resInfo.cy * num;
		float num10 = (float)resInfo.w / (float)ResourceBar.def.tex_size;
		num /= (float)ResourceBar.def.tex_size;
		num8 /= (float)ResourceBar.def.tex_size;
		num9 /= (float)ResourceBar.def.tex_size;
		if (first_to_front)
		{
			vertices[num4 - 4 * idx] = new Vector3(x + 0f, num7 + 0f, z);
			vertices[num4 - (4 * idx + 1)] = new Vector3(x + 0f, num7 + num6, z);
			vertices[num4 - (4 * idx + 2)] = new Vector3(x + num5, num7 + num6, z);
			vertices[num4 - (4 * idx + 3)] = new Vector3(x + num5, num7 + 0f, z);
			colors[num4 - 4 * idx] = color;
			colors[num4 - (4 * idx + 1)] = color;
			colors[num4 - (4 * idx + 2)] = color;
			colors[num4 - (4 * idx + 3)] = color;
			triangles[num2 - 6 * idx] = num4 - 4 * idx;
			triangles[num2 - (6 * idx + 1)] = num4 - (4 * idx + 1);
			triangles[num2 - (6 * idx + 2)] = num4 - (4 * idx + 2);
			triangles[num2 - (6 * idx + 3)] = num4 - 4 * idx;
			triangles[num2 - (6 * idx + 4)] = num4 - (4 * idx + 2);
			triangles[num2 - (6 * idx + 5)] = num4 - (4 * idx + 3);
			uvs[num3 - 4 * idx] = new Vector2(num8, num9);
			uvs[num3 - (4 * idx + 1)] = new Vector2(num8, num9 + num);
			uvs[num3 - (4 * idx + 2)] = new Vector2(num8 + num10, num9 + num);
			uvs[num3 - (4 * idx + 3)] = new Vector2(num8 + num10, num9);
		}
		else
		{
			vertices[4 * idx] = new Vector3(x + 0f, num7 + 0f, z);
			vertices[4 * idx + 1] = new Vector3(x + 0f, num7 + num6, z);
			vertices[4 * idx + 2] = new Vector3(x + num5, num7 + num6, z);
			vertices[4 * idx + 3] = new Vector3(x + num5, num7 + 0f, z);
			colors[4 * idx] = color;
			colors[4 * idx + 1] = color;
			colors[4 * idx + 2] = color;
			colors[4 * idx + 3] = color;
			triangles[6 * idx] = 4 * idx;
			triangles[6 * idx + 1] = 4 * idx + 1;
			triangles[6 * idx + 2] = 4 * idx + 2;
			triangles[6 * idx + 3] = 4 * idx;
			triangles[6 * idx + 4] = 4 * idx + 2;
			triangles[6 * idx + 5] = 4 * idx + 3;
			uvs[4 * idx] = new Vector2(num8, num9);
			uvs[4 * idx + 1] = new Vector2(num8, num9 + num);
			uvs[4 * idx + 2] = new Vector2(num8 + num10, num9 + num);
			uvs[4 * idx + 3] = new Vector2(num8 + num10, num9);
		}
		float result = x + num5;
		x += num5 * (1f - overlap_perc * 0.01f);
		return result;
	}

	// Token: 0x06001309 RID: 4873 RVA: 0x000C69D0 File Offset: 0x000C4BD0
	public static GameObject Create(global::Settlement settlement, string obj_name, Transform parent)
	{
		if (settlement == null)
		{
			return null;
		}
		if (settlement.resourceMaterial == null)
		{
			return null;
		}
		if (settlement.res_icons == null)
		{
			ResourceBar.UpdateIcons(settlement);
		}
		List<ResourceBar.IconInfo> res_icons = settlement.res_icons;
		if (res_icons.Count <= 0)
		{
			return null;
		}
		Vector3[] array = new Vector3[4 * ResourceBar.tmp_count];
		Color[] colors = new Color[4 * ResourceBar.tmp_count];
		Vector2[] array2 = new Vector2[4 * ResourceBar.tmp_count];
		int[] triangles = new int[6 * ResourceBar.tmp_count];
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < res_icons.Count; i++)
		{
			ResourceBar.IconInfo iconInfo = res_icons[i];
			if (iconInfo.separate)
			{
				num += ResourceBar.def.spacing;
			}
			float num3 = 0f;
			for (int j = 0; j < iconInfo.count; j++)
			{
				Color white = Color.white;
				num3 = ResourceBar.AddIcon(iconInfo.rt, ref num, iconInfo.scale, iconInfo.overlap_perc, num2, white, array, colors, array2, triangles, false);
				num2++;
			}
			num = num3;
		}
		for (int k = 0; k < array.Length; k++)
		{
			array[k] += new Vector3(-num / 2f, 0f, 0f);
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.colors = colors;
		mesh.triangles = triangles;
		mesh.uv = array2;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		MeshFilter meshFilter = global::Common.SpawnTemplate<MeshFilter>("ResourceBar", obj_name, parent, true, new Type[]
		{
			typeof(MeshFilter),
			typeof(MeshRenderer)
		});
		meshFilter.mesh = mesh;
		MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
		component.material = settlement.resourceMaterial;
		component.shadowCastingMode = ShadowCastingMode.Off;
		return meshFilter.gameObject;
	}

	// Token: 0x04000CAC RID: 3244
	public static ResourceBar.ResInfo[] res_info = new ResourceBar.ResInfo[]
	{
		new ResourceBar.ResInfo(0, 0, 0),
		new ResourceBar.ResInfo(1, 0, 70),
		new ResourceBar.ResInfo(2, 0, 52),
		new ResourceBar.ResInfo(3, 0, 80),
		new ResourceBar.ResInfo(4, 0, 68),
		new ResourceBar.ResInfo(0, 3, 68),
		new ResourceBar.ResInfo(5, 0, 84),
		new ResourceBar.ResInfo(2, 1, 54),
		new ResourceBar.ResInfo(0, 2, 54),
		new ResourceBar.ResInfo(0, 1, 54),
		new ResourceBar.ResInfo(1, 1, 54),
		new ResourceBar.ResInfo(4, 1, 54),
		new ResourceBar.ResInfo(5, 1, 58),
		new ResourceBar.ResInfo(0, 3, 68),
		new ResourceBar.ResInfo(1, 3, 68),
		new ResourceBar.ResInfo(3, 3, 68),
		new ResourceBar.ResInfo(2, 3, 68),
		new ResourceBar.ResInfo(4, 3, 88),
		new ResourceBar.ResInfo(5, 3, 92),
		new ResourceBar.ResInfo(6, 3, 92),
		new ResourceBar.ResInfo(6, 0, 84),
		new ResourceBar.ResInfo(6, 1, 54),
		new ResourceBar.ResInfo(7, 1, 54),
		new ResourceBar.ResInfo(1, 2, 54),
		new ResourceBar.ResInfo(2, 2, 54),
		new ResourceBar.ResInfo(3, 3, 54),
		new ResourceBar.ResInfo(0, 5, 68),
		new ResourceBar.ResInfo(1, 5, 68),
		new ResourceBar.ResInfo(3, 5, 68),
		new ResourceBar.ResInfo(2, 5, 68),
		new ResourceBar.ResInfo(4, 5, 88),
		new ResourceBar.ResInfo(5, 5, 92),
		new ResourceBar.ResInfo(6, 5, 92)
	};

	// Token: 0x04000CAD RID: 3245
	private static List<ResourceBar.IconInfo> tmp_icons = new List<ResourceBar.IconInfo>(32);

	// Token: 0x04000CAE RID: 3246
	private static int tmp_count;

	// Token: 0x04000CAF RID: 3247
	public static int hidden = 0;

	// Token: 0x0200069C RID: 1692
	public struct ResInfo
	{
		// Token: 0x0600481E RID: 18462 RVA: 0x002169C3 File Offset: 0x00214BC3
		public ResInfo(int cx, int cy, int w)
		{
			this.cx = cx;
			this.cy = cy;
			this.w = w;
		}

		// Token: 0x04003619 RID: 13849
		public int cx;

		// Token: 0x0400361A RID: 13850
		public int cy;

		// Token: 0x0400361B RID: 13851
		public int w;
	}

	// Token: 0x0200069D RID: 1693
	public class Def
	{
		// Token: 0x0600481F RID: 18463 RVA: 0x002169DA File Offset: 0x00214BDA
		public static ResourceBar.Def Get()
		{
			if (ResourceBar.Def.instance != null && ResourceBar.Def.instance.defs_version == global::Defs.Version)
			{
				return ResourceBar.Def.instance;
			}
			if (ResourceBar.Def.instance == null)
			{
				ResourceBar.Def.instance = new ResourceBar.Def();
			}
			ResourceBar.Def.instance.Load();
			return ResourceBar.Def.instance;
		}

		// Token: 0x06004820 RID: 18464 RVA: 0x00216A1C File Offset: 0x00214C1C
		private void Load()
		{
			ResourceBar.Def.<>c__DisplayClass15_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.field = global::Defs.GetDefField("ResourceBarSettings", null);
			if (this.field != null)
			{
				this.cell_width = this.field.GetFloat("cell_width", null, this.cell_width, true, true, true, '.');
				this.cell_height = this.field.GetFloat("cell_height", null, this.cell_height, true, true, true, '.');
				this.spacing = this.field.GetFloat("spacing", null, this.spacing, true, true, true, '.');
			}
			this.w2u = this.cell_width * (float)this.tex_grid / (float)this.tex_size;
			CS$<>8__locals1.def_min_count = 5;
			CS$<>8__locals1.def_min_overlap = 0f;
			CS$<>8__locals1.def_max_count = 10;
			CS$<>8__locals1.def_max_overlap = 90f;
			int num = 13;
			if (this.min_counts == null || this.min_counts.Length != num)
			{
				this.min_counts = new int[num];
			}
			if (this.min_overlaps == null || this.min_overlaps.Length != num)
			{
				this.min_overlaps = new float[num];
			}
			if (this.max_counts == null || this.max_counts.Length != num)
			{
				this.max_counts = new int[num];
			}
			if (this.max_overlaps == null || this.max_overlaps.Length != num)
			{
				this.max_overlaps = new float[num];
			}
			DT.Field field = this.field;
			DT.Field field2 = (field != null) ? field.FindChild("stacks", null, true, true, true, '.') : null;
			this.<Load>g__LoadStacks|15_0(field2, out CS$<>8__locals1.def_min_count, out CS$<>8__locals1.def_min_overlap, out CS$<>8__locals1.def_max_count, out CS$<>8__locals1.def_max_overlap, ref CS$<>8__locals1);
			this.<Load>g__SetStacks|15_1(0, CS$<>8__locals1.def_min_count, CS$<>8__locals1.def_min_overlap, CS$<>8__locals1.def_max_count, CS$<>8__locals1.def_max_overlap, ref CS$<>8__locals1);
			for (int i = 1; i < num; i++)
			{
				ResourceType resourceType = (ResourceType)i;
				DT.Field f = (field2 != null) ? field2.FindChild(resourceType.ToString(), null, true, true, true, '.') : null;
				int min_count;
				float min_overlap;
				int max_count;
				float max_overlap;
				this.<Load>g__LoadStacks|15_0(f, out min_count, out min_overlap, out max_count, out max_overlap, ref CS$<>8__locals1);
				this.<Load>g__SetStacks|15_1(i, min_count, min_overlap, max_count, max_overlap, ref CS$<>8__locals1);
			}
			this.defs_version = global::Defs.Version;
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x00216C38 File Offset: 0x00214E38
		public float CalcOverlapPerc(ResourceType rt, int count)
		{
			if (rt < ResourceType.None || rt >= (ResourceType)this.min_counts.Length)
			{
				return 0f;
			}
			int num = this.min_counts[(int)rt];
			float num2 = this.min_overlaps[(int)rt];
			if (count <= num)
			{
				return num2;
			}
			int num3 = this.max_counts[(int)rt];
			float num4 = this.max_overlaps[(int)rt];
			if (count >= num3)
			{
				return num4;
			}
			return num2 + (float)(count - num) * (num4 - num2) / (float)(num3 - num);
		}

		// Token: 0x06004824 RID: 18468 RVA: 0x00216CDC File Offset: 0x00214EDC
		[CompilerGenerated]
		private void <Load>g__LoadStacks|15_0(DT.Field f, out int min_count, out float min_overlap, out int max_count, out float max_overlap, ref ResourceBar.Def.<>c__DisplayClass15_0 A_6)
		{
			if (f == null)
			{
				min_count = A_6.def_min_count;
				min_overlap = A_6.def_min_overlap;
				max_count = A_6.def_max_count;
				max_overlap = A_6.def_max_overlap;
				return;
			}
			min_count = f.Int(0, null, A_6.def_min_count);
			min_overlap = f.Float(1, null, A_6.def_min_overlap);
			max_count = f.Int(2, null, A_6.def_max_count);
			max_overlap = f.Float(3, null, A_6.def_max_overlap);
		}

		// Token: 0x06004825 RID: 18469 RVA: 0x00216D59 File Offset: 0x00214F59
		[CompilerGenerated]
		private void <Load>g__SetStacks|15_1(int i, int min_count, float min_overlap, int max_count, float max_overlap, ref ResourceBar.Def.<>c__DisplayClass15_0 A_6)
		{
			this.min_counts[i] = min_count;
			this.min_overlaps[i] = min_overlap;
			this.max_counts[i] = max_count;
			this.max_overlaps[i] = max_overlap;
		}

		// Token: 0x0400361C RID: 13852
		public DT.Field field;

		// Token: 0x0400361D RID: 13853
		public float cell_width = 2.5f;

		// Token: 0x0400361E RID: 13854
		public float cell_height = 2.5f;

		// Token: 0x0400361F RID: 13855
		public float spacing;

		// Token: 0x04003620 RID: 13856
		public float baseline = 15f;

		// Token: 0x04003621 RID: 13857
		public int tex_size = 1024;

		// Token: 0x04003622 RID: 13858
		public int tex_grid = 8;

		// Token: 0x04003623 RID: 13859
		public float w2u;

		// Token: 0x04003624 RID: 13860
		public int[] min_counts;

		// Token: 0x04003625 RID: 13861
		public int[] max_counts;

		// Token: 0x04003626 RID: 13862
		public float[] min_overlaps;

		// Token: 0x04003627 RID: 13863
		public float[] max_overlaps;

		// Token: 0x04003628 RID: 13864
		public static ResourceBar.Def instance;

		// Token: 0x04003629 RID: 13865
		public int defs_version;
	}

	// Token: 0x0200069E RID: 1694
	public struct IconInfo
	{
		// Token: 0x06004826 RID: 18470 RVA: 0x00216D84 File Offset: 0x00214F84
		public bool Differs(ResourceBar.IconInfo icon)
		{
			return this.rt != icon.rt || this.count != icon.count || this.scale != icon.scale || this.overlap_perc != icon.overlap_perc || this.separate != icon.separate;
		}

		// Token: 0x0400362A RID: 13866
		public int rt;

		// Token: 0x0400362B RID: 13867
		public int count;

		// Token: 0x0400362C RID: 13868
		public bool negative;

		// Token: 0x0400362D RID: 13869
		public float scale;

		// Token: 0x0400362E RID: 13870
		public float overlap_perc;

		// Token: 0x0400362F RID: 13871
		public bool separate;
	}
}
