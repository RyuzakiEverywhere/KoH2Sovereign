using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class Attractor : MonoBehaviour
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600080C RID: 2060 RVA: 0x00055574 File Offset: 0x00053774
	// (set) Token: 0x0600080B RID: 2059 RVA: 0x0005556B File Offset: 0x0005376B
	public bool is_inside_wall { get; private set; }

	// Token: 0x0600080D RID: 2061 RVA: 0x0005557C File Offset: 0x0005377C
	public bool HasFlag(Logic.WanderPeasant.AttractorFlags flag)
	{
		return (this.flags & flag) > Logic.WanderPeasant.AttractorFlags.None;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x00055589 File Offset: 0x00053789
	public bool HasPickupFlag(Logic.WanderPeasant.AttractorFlags flag)
	{
		return (this.pickup_flags & flag) > Logic.WanderPeasant.AttractorFlags.None;
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00055596 File Offset: 0x00053796
	public bool HasLeaveFlag(Logic.WanderPeasant.AttractorFlags flag)
	{
		return flag == Logic.WanderPeasant.AttractorFlags.Move || (this.leave_flags & flag) > Logic.WanderPeasant.AttractorFlags.None;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x000555A9 File Offset: 0x000537A9
	private void OnEnable()
	{
		if (BattleMap.Get() == null)
		{
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		this.SnapHeight();
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x000555C8 File Offset: 0x000537C8
	private void Update()
	{
		if (!this.checked_impassable && !this.CheckImpassable() && this.checked_impassable)
		{
			BattleMap battleMap = BattleMap.Get();
			this.is_inside_wall = battleMap.IsInsideWall(base.transform.position);
			if (battleMap.attractors == null)
			{
				battleMap.attractors = new List<Attractor>();
			}
			battleMap.attractors.Add(this);
			if ((this.flags & Logic.WanderPeasant.AttractorFlags.Spawn) != Logic.WanderPeasant.AttractorFlags.None)
			{
				battleMap.spawn_attractors++;
			}
			Attractor.LoadDef();
		}
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x00055648 File Offset: 0x00053848
	private void OnDisable()
	{
		BattleMap battleMap = BattleMap.Get();
		if (((battleMap != null) ? battleMap.attractors : null) == null)
		{
			return;
		}
		battleMap.attractors.Remove(this);
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x00055678 File Offset: 0x00053878
	public bool CheckImpassable()
	{
		global::PathFinding pathFinding = global::PathFinding.Get(false);
		Logic.PathFinding logic = pathFinding.logic;
		if (((logic != null) ? logic.data : null) == null)
		{
			return false;
		}
		this.checked_impassable = true;
		this.invalid = !pathFinding.logic.data.IsPassable(base.transform.position, 0f);
		return this.invalid;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000556DD File Offset: 0x000538DD
	public void SnapHeight()
	{
		if (this.snap_to_terrain)
		{
			global::Common.SnapToTerrain(base.gameObject, 0f, null, -1f);
		}
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00055700 File Offset: 0x00053900
	public static void LoadDef()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Game game = GameLogic.Get(true);
		if (((game != null) ? game.game : null) == null)
		{
			return;
		}
		if (Attractor.defs.Count > 0)
		{
			return;
		}
		global::Defs defs = global::Defs.Get(false);
		DT dt = (defs != null) ? defs.dt : null;
		if (dt == null)
		{
			return;
		}
		DT.Field field = dt.Find("AttractorSettings", null);
		if (field == null)
		{
			return;
		}
		Attractor.animation_settings.Clear();
		Attractor.max_peasants = field.GetInt("max_peasants", null, Attractor.max_peasants, true, true, true, '.');
		Attractor.max_wander_distance = field.GetFloat("max_wander_distance", null, Attractor.max_wander_distance, true, true, true, '.');
		Attractor.time_between_spawns = field.GetFloat("time_between_spawns", null, Attractor.time_between_spawns, true, true, true, '.');
		DT.Field field2 = field.FindChild("animation_settings", null, true, true, true, '.');
		if (((field2 != null) ? field2.children : null) != null)
		{
			for (int i = 0; i < field2.children.Count; i++)
			{
				DT.Field field3 = field2.children[i];
				string key = field3.key;
				if (!string.IsNullOrEmpty(key))
				{
					Attractor.AnimationSettings animationSettings = new Attractor.AnimationSettings
					{
						use_radius = field3.GetBool("use_radius", null, false, true, true, true, '.')
					};
					if (animationSettings.use_radius)
					{
						DT.Field field4 = field3.FindChild("radius_idle_duration", null, true, true, true, '.');
						if (field4 != null)
						{
							if (field4.NumValues() == 2)
							{
								animationSettings.radius_idle_duration_min = field4.Float(0, null, 0f);
								animationSettings.radius_idle_duration_max = field4.Float(1, null, 0f);
							}
							else
							{
								animationSettings.radius_idle_duration_min = (animationSettings.radius_idle_duration_max = field4.Float(null, 0f));
							}
						}
					}
					DT.Field field5 = field3.FindChild("idle_duration", null, true, true, true, '.');
					if (field5 != null)
					{
						if (field5.NumValues() == 2)
						{
							animationSettings.idle_duration_min = field5.Float(0, null, 0f);
							animationSettings.idle_duration_max = field5.Float(1, null, 0f);
						}
						else
						{
							animationSettings.idle_duration_min = (animationSettings.idle_duration_max = field5.Float(null, 0f));
						}
					}
					Attractor.animation_settings[key.ToLowerInvariant()] = animationSettings;
				}
			}
		}
		DT.Field field6 = dt.Find("WanderPeasant", null);
		if (field6 == null)
		{
			return;
		}
		Attractor.AddPeasantDef(field6);
		DT.Def def = field6.def;
		if (((def != null) ? def.defs : null) == null || field6.def.defs.Count == 0)
		{
			return;
		}
		for (int j = 0; j < field6.def.defs.Count; j++)
		{
			Attractor.AddPeasantDef(field6.def.defs[j].field);
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000559B8 File Offset: 0x00053BB8
	private static void AddPeasantDef(DT.Field field)
	{
		Logic.WanderPeasant.Def item = GameLogic.Get(true).game.defs.Find<Logic.WanderPeasant.Def>(field.key);
		Attractor.defs.Add(item);
	}

	// Token: 0x04000667 RID: 1639
	public static int max_peasants = 100;

	// Token: 0x04000668 RID: 1640
	public static float max_wander_distance = 40f;

	// Token: 0x04000669 RID: 1641
	public static List<Logic.WanderPeasant.Def> defs = new List<Logic.WanderPeasant.Def>();

	// Token: 0x0400066A RID: 1642
	public static Dictionary<string, Attractor.AnimationSettings> animation_settings = new Dictionary<string, Attractor.AnimationSettings>();

	// Token: 0x0400066B RID: 1643
	public static float time_between_spawns = 10f;

	// Token: 0x0400066C RID: 1644
	public Logic.WanderPeasant.AttractorFlags flags;

	// Token: 0x0400066D RID: 1645
	public Logic.WanderPeasant.AttractorFlags pickup_flags;

	// Token: 0x0400066E RID: 1646
	public Logic.WanderPeasant.AttractorFlags leave_flags;

	// Token: 0x0400066F RID: 1647
	public bool snap_to_terrain = true;

	// Token: 0x04000670 RID: 1648
	public Logic.WanderPeasant cur_peasant;

	// Token: 0x04000671 RID: 1649
	public Transform control_transform;

	// Token: 0x04000672 RID: 1650
	private bool checked_impassable;

	// Token: 0x04000673 RID: 1651
	private bool invalid;

	// Token: 0x04000674 RID: 1652
	public float radius;

	// Token: 0x020005A1 RID: 1441
	public struct AnimationSettings
	{
		// Token: 0x04003117 RID: 12567
		public bool use_radius;

		// Token: 0x04003118 RID: 12568
		public float radius_idle_duration_min;

		// Token: 0x04003119 RID: 12569
		public float radius_idle_duration_max;

		// Token: 0x0400311A RID: 12570
		public float idle_duration_min;

		// Token: 0x0400311B RID: 12571
		public float idle_duration_max;
	}
}
