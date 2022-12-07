using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200033A RID: 826
public class Traveler
{
	// Token: 0x0600326D RID: 12909 RVA: 0x001994E4 File Offset: 0x001976E4
	public static void LoadPrefabs(TextureBaker texture_baker)
	{
		global::Defs defs = global::Defs.Get(false);
		if (defs == null)
		{
			return;
		}
		DT.Def def = defs.dt.FindDef("Traveler");
		if (def == null || def.defs == null || def.defs.Count < 1)
		{
			return;
		}
		Traveler.prefabs.Clear();
		int layer = LayerMask.NameToLayer("Animals");
		for (int i = 0; i < def.defs.Count; i++)
		{
			DT.Field field = def.defs[i].field;
			if (!string.IsNullOrEmpty(field.base_path))
			{
				Traveler.Def def2 = new Traveler.Def();
				if (def2 != null)
				{
					def2.Load(field);
					Traveler.prefabs.Add(def2);
					AnimatorInstancer.LoadBakedSkinningData(texture_baker, field, null, def2.render_scale, layer);
					if (AnimatorInstancer.LoadBakedSkinningData(texture_baker, field, "_addon", def2.render_scale, layer) != null)
					{
						def2.has_addon = true;
					}
				}
			}
		}
	}

	// Token: 0x0600326E RID: 12910 RVA: 0x001995D0 File Offset: 0x001977D0
	[ConsoleMethod("travellers_per_road")]
	public static void TravellersPerRoad(int num = 4)
	{
		for (int i = 0; i < Traveler.prefabs.Count; i++)
		{
			Traveler.prefabs[i].total_travellers_per_road = num;
		}
		AutoRoads autoRoads = UnityEngine.Object.FindObjectOfType<AutoRoads>();
		if (autoRoads == null)
		{
			return;
		}
		for (int j = 0; j < autoRoads.instanced_roads.Count; j++)
		{
			autoRoads.instanced_roads[j].ClearTravellers();
		}
	}

	// Token: 0x040021E9 RID: 8681
	public static List<Traveler.Def> prefabs = new List<Traveler.Def>();

	// Token: 0x0200088E RID: 2190
	public class Def
	{
		// Token: 0x06005180 RID: 20864 RVA: 0x0023DB78 File Offset: 0x0023BD78
		public void Load(DT.Field f)
		{
			this.field = f;
			this.model = f.GetValue("model", null, true, true, true, '.').Get<GameObject>();
			this.model_addon = f.GetValue("model_addon", null, true, true, true, '.').Get<GameObject>();
			this.movement_speed = f.GetFloat("movement_speed", null, 0f, true, true, true, '.');
			this.finish_path_offset = f.GetFloat("finish_path_offset", null, this.finish_path_offset, true, true, true, '.');
			this.anim_movement_speed = f.GetFloat("anim_movement_speed", null, 0f, true, true, true, '.');
			this.total_travellers_per_road = f.GetInt("total_travellers_per_road", null, 0, true, true, true, '.');
			this.travellers_per_road = f.GetInt("travellers_per_road", null, 0, true, true, true, '.');
			this.min_spawn_interval = f.GetFloat("min_spawn_interval", null, 0f, true, true, true, '.');
			this.max_spawn_interval = f.GetFloat("max_spawn_interval", null, 0f, true, true, true, '.');
			this.look_ahead = f.GetFloat("look_ahead", null, this.look_ahead, true, true, true, '.');
			this.pivot_offset = new Vector3(f.GetFloat("pivot_offset_x", null, this.pivot_offset.x, true, true, true, '.'), f.GetFloat("pivot_offset_y", null, this.pivot_offset.y, true, true, true, '.'), f.GetFloat("pivot_offset_z", null, this.pivot_offset.z, true, true, true, '.'));
			this.addon_offset = f.GetFloat("addon_offset", null, this.addon_offset, true, true, true, '.');
			this.min_dist = f.GetFloat("min_dist", null, this.min_dist, true, true, true, '.');
			this.render_scale = f.GetFloat("render_scale", null, this.render_scale, true, true, true, '.');
			DT.Field field = f.FindChild("culture", null, true, true, true, '.');
			if (field == null || field.children == null)
			{
				return;
			}
			for (int i = 0; i < field.children.Count; i++)
			{
				this.culture.Add(field.children[i].key);
			}
		}

		// Token: 0x0400400A RID: 16394
		public DT.Field field;

		// Token: 0x0400400B RID: 16395
		public GameObject model;

		// Token: 0x0400400C RID: 16396
		public GameObject model_addon;

		// Token: 0x0400400D RID: 16397
		public float movement_speed;

		// Token: 0x0400400E RID: 16398
		public float finish_path_offset;

		// Token: 0x0400400F RID: 16399
		public float anim_movement_speed;

		// Token: 0x04004010 RID: 16400
		public int total_travellers_per_road;

		// Token: 0x04004011 RID: 16401
		public int travellers_per_road;

		// Token: 0x04004012 RID: 16402
		public float min_spawn_interval = 120f;

		// Token: 0x04004013 RID: 16403
		public float max_spawn_interval = 360f;

		// Token: 0x04004014 RID: 16404
		public float look_ahead = 0.75f;

		// Token: 0x04004015 RID: 16405
		public List<string> culture = new List<string>();

		// Token: 0x04004016 RID: 16406
		public Vector3 pivot_offset;

		// Token: 0x04004017 RID: 16407
		public float addon_offset;

		// Token: 0x04004018 RID: 16408
		public float min_dist;

		// Token: 0x04004019 RID: 16409
		public bool has_addon;

		// Token: 0x0400401A RID: 16410
		public float render_scale = 1f;
	}
}
