using System;
using System.Runtime.CompilerServices;
using Logic;
using UnityEngine;

// Token: 0x02000166 RID: 358
[ExecuteInEditMode]
public class PathFinding : GameLogic.Behaviour
{
	// Token: 0x06001224 RID: 4644 RVA: 0x000BE955 File Offset: 0x000BCB55
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x000023FD File Offset: 0x000005FD
	public override void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x000BE95D File Offset: 0x000BCB5D
	private void OnEnable()
	{
		global::PathFinding.instance = this;
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x000BE965 File Offset: 0x000BCB65
	private void OnDestroy()
	{
		if (global::PathFinding.instance == this)
		{
			global::PathFinding.instance = null;
		}
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x000BE97C File Offset: 0x000BCB7C
	public static global::PathFinding Get(bool create_if_needed = false)
	{
		if (global::PathFinding.instance != null)
		{
			return global::PathFinding.instance;
		}
		MapData mapData = MapData.Get();
		if (mapData != null)
		{
			global::PathFinding.instance = mapData.GetComponent<global::PathFinding>();
			if (global::PathFinding.instance != null)
			{
				return global::PathFinding.instance;
			}
		}
		if (!create_if_needed)
		{
			return null;
		}
		global::PathFinding.instance = new GameObject("PathFinding").AddComponent<global::PathFinding>();
		return global::PathFinding.instance;
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x000BE9E8 File Offset: 0x000BCBE8
	private void OnGUI()
	{
		if (!Logic.PathFinding.debugging)
		{
			return;
		}
		if (this.logic == null)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Game game = GameLogic.Get(true);
			this.logic = ((game != null) ? game.path_finding : null);
			if (this.logic == null)
			{
				return;
			}
		}
		if (this.logic.settings.multithreaded)
		{
			return;
		}
		global::PathFinding.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.left = 10f;
		float y = 150f;
		CS$<>8__locals1.right = CS$<>8__locals1.left + 650f;
		CS$<>8__locals1.line_height = 20f;
		CS$<>8__locals1.pt = new Vector2(CS$<>8__locals1.left, y);
		CS$<>8__locals1.label_style = GUI.skin.GetStyle("Label");
		TextAnchor alignment = CS$<>8__locals1.label_style.alignment;
		GUI.Box(new Rect(CS$<>8__locals1.left - 5f, CS$<>8__locals1.pt.y, CS$<>8__locals1.right - CS$<>8__locals1.left + 10f, 8f * CS$<>8__locals1.line_height), "");
		global::PathFinding.<OnGUI>g__PrintLN|8_8(string.Format("State: {0}, High Steps: {1}, Low Steps: {2}", this.logic.state, Logic.PathFinding.total_high_steps, Logic.PathFinding.total_low_steps), -1f, false, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStatRow|8_10(0, "Section", "Calls", "Total", "Avg", "Min", "Max", ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStatRow|8_10(0, "-------", "-----", "-----", "---", "---", "---", ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStat|8_9(0, "Process", Logic.PathFinding.pfs_Process, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStat|8_9(1, "High Level Path", Logic.PathFinding.pfs_HighLevelPath, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStat|8_9(2, "Modify High Grid", Logic.PathFinding.pfs_ModifyHighGrid, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStat|8_9(0, "Low Level Steps", Logic.PathFinding.pfs_LowLevelSteps, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__PrintStat|8_9(0, "Clear Closed", Logic.PathFinding.pfs_ClearClosed, ref CS$<>8__locals1);
		CS$<>8__locals1.pt.y = CS$<>8__locals1.pt.y + 5f;
		global::PathFinding.<OnGUI>g__Button|8_11("Stop", delegate
		{
			global::PathFinding.StopDebugging();
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__Button|8_11("1 Step", delegate
		{
			global::PathFinding.DebugProcess(1);
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__Button|8_11("10 Steps", delegate
		{
			global::PathFinding.DebugProcess(10);
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__Button|8_11("100 Steps", delegate
		{
			global::PathFinding.DebugProcess(100);
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__Button|8_11("1000 Steps", delegate
		{
			global::PathFinding.DebugProcess(1000);
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__Button|8_11("Repeat", delegate
		{
			global::PathFinding.BeginDebugging(Logic.PathFinding.last_src_pt, Logic.PathFinding.last_dst_pt, Logic.PathFinding.last_low_level_only, Logic.PathFinding.last_src_obj, Logic.PathFinding.last_range, Logic.PathFinding.last_flee);
		}, ref CS$<>8__locals1);
		global::PathFinding.<OnGUI>g__NewLine|8_7(ref CS$<>8__locals1);
		CS$<>8__locals1.label_style.alignment = alignment;
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x000BECF8 File Offset: 0x000BCEF8
	public static void BeginDebugging(bool low_level_only)
	{
		Logic.Battle battle = BattleMap.battle;
		Logic.PathFinding pathFinding;
		if (battle == null)
		{
			pathFinding = null;
		}
		else
		{
			Game batte_view_game = battle.batte_view_game;
			pathFinding = ((batte_view_game != null) ? batte_view_game.path_finding : null);
		}
		Logic.PathFinding pathFinding2 = pathFinding;
		if (pathFinding2 == null)
		{
			Game game = GameLogic.Get(true);
			pathFinding2 = ((game != null) ? game.path_finding : null);
			if (pathFinding2 == null)
			{
				return;
			}
		}
		pathFinding2.FlushPending(false);
		if (!Logic.PathFinding.debugging)
		{
			Logic.PathFinding.debugging = true;
			global::PathFinding.org_pf_multithreaded = pathFinding2.settings.multithreaded;
			global::PathFinding.org_pf_max_steps = pathFinding2.settings.max_steps;
			pathFinding2.settings.multithreaded = false;
			pathFinding2.settings.max_steps = -1;
		}
		Logic.PathFinding.ClearProfileStats();
		global::PathFinding.debug_pf_low_level_only = low_level_only;
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x000BED94 File Offset: 0x000BCF94
	public static void BeginDebugging(PPos pt_from, PPos pt_to, bool low_level_only, MapObject src_obj = null, float range = 1f, bool flee = false)
	{
		Logic.Battle battle = BattleMap.battle;
		Logic.PathFinding pathFinding;
		if (battle == null)
		{
			pathFinding = null;
		}
		else
		{
			Game batte_view_game = battle.batte_view_game;
			pathFinding = ((batte_view_game != null) ? batte_view_game.path_finding : null);
		}
		Logic.PathFinding pathFinding2 = pathFinding;
		if (pathFinding2 == null)
		{
			Game game = GameLogic.Get(true);
			pathFinding2 = ((game != null) ? game.path_finding : null);
			if (pathFinding2 == null)
			{
				return;
			}
		}
		global::PathFinding.BeginDebugging(low_level_only);
		Path path = new Path(pathFinding2.game, pt_from, PathData.PassableArea.Type.All, false);
		path.src_obj = src_obj;
		Logic.Squad squad = src_obj as Logic.Squad;
		if (squad != null)
		{
			path.battle_side = squad.battle_side;
		}
		path.range = range;
		path.flee = flee;
		path.can_reserve = true;
		path.Find(pt_to, range, false);
		Logic.PathFinding.path_dbg = path;
		Logic.PathFinding.last_src_pt = pt_from;
		Logic.PathFinding.last_dst_pt = pt_to;
		Logic.PathFinding.last_low_level_only = low_level_only;
		Logic.PathFinding.last_src_obj = src_obj;
		Logic.PathFinding.last_flee = flee;
		Logic.PathFinding.last_range = range;
		global::PathFinding.DebugProcess(1);
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x000BEE60 File Offset: 0x000BD060
	public static void DebugProcess(int count)
	{
		if (!Logic.PathFinding.debugging)
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		Logic.PathFinding pathFinding;
		if (battle == null)
		{
			pathFinding = null;
		}
		else
		{
			Game batte_view_game = battle.batte_view_game;
			pathFinding = ((batte_view_game != null) ? batte_view_game.path_finding : null);
		}
		Logic.PathFinding pathFinding2 = pathFinding;
		if (pathFinding2 == null)
		{
			Game game = GameLogic.Get(true);
			pathFinding2 = ((game != null) ? game.path_finding : null);
			if (pathFinding2 == null)
			{
				return;
			}
		}
		for (int i = 0; i < count; i++)
		{
			if (!pathFinding2.Process(true, global::PathFinding.debug_pf_low_level_only) || pathFinding2.state == Logic.PathFinding.State.Finished)
			{
				Debug.Log("Finished");
				return;
			}
		}
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x000BEEDC File Offset: 0x000BD0DC
	public static void StopDebugging()
	{
		if (!Logic.PathFinding.debugging)
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		Logic.PathFinding pathFinding;
		if (battle == null)
		{
			pathFinding = null;
		}
		else
		{
			Game batte_view_game = battle.batte_view_game;
			pathFinding = ((batte_view_game != null) ? batte_view_game.path_finding : null);
		}
		Logic.PathFinding pathFinding2 = pathFinding;
		if (pathFinding2 == null)
		{
			Game game = GameLogic.Get(true);
			pathFinding2 = ((game != null) ? game.path_finding : null);
			if (pathFinding2 == null)
			{
				return;
			}
		}
		Logic.PathFinding.debugging = false;
		pathFinding2.FlushPending(false);
		pathFinding2.settings.multithreaded = global::PathFinding.org_pf_multithreaded;
		pathFinding2.settings.max_steps = global::PathFinding.org_pf_max_steps;
		Game.CopyToClipboard(Logic.PathFinding.StatsText("\n", "  "));
		Debug.Log("Debug path finding ended");
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x000BEF74 File Offset: 0x000BD174
	private void LateUpdate()
	{
		Game game = GameLogic.Get(false);
		if (this.logic == null && game != null)
		{
			this.logic = game.path_finding;
		}
		if (game != null && game.IsPaused() && this.logic != null && this.logic.IsRegisteredForUpdate())
		{
			this.logic.OnUpdate();
		}
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x000BEFFC File Offset: 0x000BD1FC
	[CompilerGenerated]
	internal static void <OnGUI>g__Print|8_6(string text, float width, bool right_aligned, ref global::PathFinding.<>c__DisplayClass8_0 A_3)
	{
		if (width < 0f)
		{
			width = A_3.right - A_3.pt.x;
		}
		A_3.label_style.alignment = (right_aligned ? TextAnchor.UpperRight : TextAnchor.UpperLeft);
		GUI.Label(new Rect(A_3.pt.x, A_3.pt.y, width, A_3.line_height), text);
		A_3.pt.x = A_3.pt.x + width;
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x000BF06F File Offset: 0x000BD26F
	[CompilerGenerated]
	internal static void <OnGUI>g__NewLine|8_7(ref global::PathFinding.<>c__DisplayClass8_0 A_0)
	{
		A_0.pt.x = A_0.left;
		A_0.pt.y = A_0.pt.y + A_0.line_height;
	}

	// Token: 0x06001233 RID: 4659 RVA: 0x000BF097 File Offset: 0x000BD297
	[CompilerGenerated]
	internal static void <OnGUI>g__PrintLN|8_8(string text, float width, bool right_aligned, ref global::PathFinding.<>c__DisplayClass8_0 A_3)
	{
		global::PathFinding.<OnGUI>g__Print|8_6(text, width, right_aligned, ref A_3);
		global::PathFinding.<OnGUI>g__NewLine|8_7(ref A_3);
	}

	// Token: 0x06001234 RID: 4660 RVA: 0x000BF0A8 File Offset: 0x000BD2A8
	[CompilerGenerated]
	internal static void <OnGUI>g__PrintStat|8_9(int ident, string name, Game.ProfileScope.Stats stats, ref global::PathFinding.<>c__DisplayClass8_0 A_3)
	{
		global::PathFinding.<OnGUI>g__PrintStatRow|8_10(ident, name, stats.num_calls.ToString(), stats.total_ms.ToString("F3"), stats.avg_ms.ToString("F3"), stats.min_ms.ToString("F3"), stats.max_ms.ToString("F3"), ref A_3);
	}

	// Token: 0x06001235 RID: 4661 RVA: 0x000BF114 File Offset: 0x000BD314
	[CompilerGenerated]
	internal static void <OnGUI>g__PrintStatRow|8_10(int ident, string name, string calls, string total, string avg, string min, string max, ref global::PathFinding.<>c__DisplayClass8_0 A_7)
	{
		float num = (float)(ident * 20);
		A_7.pt.x = A_7.pt.x + num;
		global::PathFinding.<OnGUI>g__Print|8_6(name, 150f - num, false, ref A_7);
		global::PathFinding.<OnGUI>g__Print|8_6(calls, 100f, true, ref A_7);
		global::PathFinding.<OnGUI>g__Print|8_6(total, 100f, true, ref A_7);
		global::PathFinding.<OnGUI>g__Print|8_6(avg, 100f, true, ref A_7);
		global::PathFinding.<OnGUI>g__Print|8_6(min, 100f, true, ref A_7);
		global::PathFinding.<OnGUI>g__Print|8_6(max, 100f, true, ref A_7);
		global::PathFinding.<OnGUI>g__NewLine|8_7(ref A_7);
	}

	// Token: 0x06001236 RID: 4662 RVA: 0x000BF198 File Offset: 0x000BD398
	[CompilerGenerated]
	internal static void <OnGUI>g__Button|8_11(string name, Action action, ref global::PathFinding.<>c__DisplayClass8_0 A_2)
	{
		if (GUI.Button(new Rect(A_2.pt.x, A_2.pt.y, 100f, A_2.line_height), name))
		{
			action();
		}
		A_2.pt.x = A_2.pt.x + 120f;
	}

	// Token: 0x04000C3D RID: 3133
	public Logic.PathFinding.Settings settings = new Logic.PathFinding.Settings();

	// Token: 0x04000C3E RID: 3134
	public Logic.PathFinding logic;

	// Token: 0x04000C3F RID: 3135
	private static global::PathFinding instance = null;

	// Token: 0x04000C40 RID: 3136
	private static bool org_pf_multithreaded = true;

	// Token: 0x04000C41 RID: 3137
	private static int org_pf_max_steps = 100000;

	// Token: 0x04000C42 RID: 3138
	private static bool debug_pf_low_level_only = false;
}
