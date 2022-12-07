using System;
using Logic;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class Ladder : GameLogic.Behaviour
{
	// Token: 0x060008E0 RID: 2272 RVA: 0x00060948 File Offset: 0x0005EB48
	public static void CreateVisuals(Logic.Object obj)
	{
		Logic.Ladder ladder = obj as Logic.Ladder;
		if (global::Ladder.prefab == null)
		{
			global::Ladder.prefab = ladder.def.field.GetRandomValue("prefab", null, true, true, true, '.').Get<GameObject>();
		}
		if (global::Ladder.preview_prefab == null)
		{
			global::Ladder.preview_prefab = ladder.def.field.GetRandomValue("preview_prefab", null, true, true, true, '.').Get<GameObject>();
		}
		GameObject gameObject = global::Common.SpawnTemplate("Ladder", "Ladder " + ladder.paID, null, true, new Type[]
		{
			typeof(global::Ladder)
		});
		global::Common.SetObjectParent(gameObject, GameLogic.instance.transform, "Ladders");
		global::Ladder component = gameObject.GetComponent<global::Ladder>();
		component.logic = ladder;
		Vector3 position;
		float x;
		float x2;
		float y;
		global::Ladder.CalcPosRot(ladder.paID, out position, out x, out x2, out y);
		component.start_x = x;
		component.final_x = x2;
		component.rot_y = y;
		GameObject gameObject2 = global::Common.Spawn(global::Ladder.prefab, gameObject.transform, false, "");
		gameObject2.transform.position = position;
		gameObject2.transform.rotation = Quaternion.Euler(x, y, 0f);
		gameObject2.transform.localScale = Vector3.one;
		component.ladder_obj = gameObject2;
		gameObject2 = global::Common.Spawn(global::Ladder.preview_prefab, gameObject.transform, false, "");
		gameObject2.transform.position = position;
		gameObject2.transform.rotation = Quaternion.Euler(x2, y, 0f);
		gameObject2.transform.localScale = Vector3.one;
		component.ladder_preview = gameObject2;
		ladder.visuals = component;
		component.UpdateVisibility();
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00060B08 File Offset: 0x0005ED08
	public static void CalcPosRot(int paID, out Vector3 start, out float start_x, out float final_x, out float rot_y)
	{
		Logic.PathFinding path_finding = BattleMap.battle.batte_view_game.path_finding;
		start = Vector3.zero;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			PathData.PassableAreaNode paNode = path_finding.data.GetPaNode((paID - 1) * PathData.PassableArea.numNodes + i);
			if (paNode.type != PathData.PassableAreaNode.Type.Unlinked)
			{
				if (!(start == Vector3.zero))
				{
					vector = global::Common.Point3D(paNode.pos, path_finding.game);
					break;
				}
				start = global::Common.Point3D(paNode.pos, path_finding.game);
			}
		}
		if (vector.y < start.y)
		{
			Vector3 vector2 = vector;
			vector = start;
			start = vector2;
		}
		Vector3 normalized = (vector - start).normalized;
		float num = Vector3.SignedAngle(Vector3.forward, new Vector3(normalized.x, 0f, normalized.z), Vector3.up);
		float num2 = Vector3.SignedAngle(new Vector3(normalized.x, 0f, normalized.z), normalized, new Vector3(normalized.z, 0f, -normalized.x));
		Vector3 terrainNormal = global::Common.GetTerrainNormal(start, null);
		float num3 = -Vector3.Angle(new Vector3(terrainNormal.x, 0f, terrainNormal.z), terrainNormal);
		final_x = num2;
		start_x = -180f + (90f + num3);
		rot_y = num;
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00060C83 File Offset: 0x0005EE83
	public void Destroy()
	{
		if (this.ladder_obj != null)
		{
			global::Common.DestroyObj(this.ladder_obj);
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00060CA0 File Offset: 0x0005EEA0
	public void RotateLadder()
	{
		float num = this.logic.ladder_rot_progress.Get();
		if (num >= 1f)
		{
			this.logic.rotating = false;
			this.ladder_obj.transform.rotation = Quaternion.Euler(Mathf.Lerp(this.start_x, this.final_x, 1f), this.rot_y, 0f);
			return;
		}
		this.ladder_obj.transform.rotation = Quaternion.Euler(Mathf.Lerp(this.start_x, this.final_x, num), this.rot_y, 0f);
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00060D3B File Offset: 0x0005EF3B
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00060D43 File Offset: 0x0005EF43
	public override void OnMessage(object obj, string message, object param)
	{
		if (message == "destroying" || message == "finishing")
		{
			global::Common.DestroyObj(base.gameObject);
			return;
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00060D6C File Offset: 0x0005EF6C
	private void UpdateVisibility()
	{
		Logic.Ladder ladder = this.logic;
		if (((ladder != null) ? ladder.ladder_rot_progress : null) != null)
		{
			Logic.Ladder ladder2 = this.logic;
			if (((ladder2 != null) ? ladder2.squads : null) != null)
			{
				bool flag = this.logic.ladder_rot_progress.Get() >= 1f;
				if (!flag)
				{
					float num = this.logic.def.max_t_appear * this.logic.def.max_t_appear;
					for (int i = 0; i < this.logic.squads.Count; i++)
					{
						Logic.Squad squad = this.logic.squads[i];
						if (squad.movement.path != null && squad.position.SqrDist(this.logic.position) < num)
						{
							flag = true;
							break;
						}
					}
				}
				this.logic.visible = flag;
				this.ladder_obj.SetActive(flag);
				bool flag2 = !flag;
				if (flag2)
				{
					RelationUtils.Stance stance = RelationUtils.Stance.None;
					Logic.Kingdom k = BaseUI.LogicKingdom();
					for (int j = 0; j < this.logic.squads.Count; j++)
					{
						Logic.Squad squad2 = this.logic.squads[j];
						stance |= squad2.GetStance(k);
					}
					if (stance == RelationUtils.Stance.War)
					{
						flag2 = false;
					}
				}
				this.ladder_preview.SetActive(flag2);
				return;
			}
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00060EBA File Offset: 0x0005F0BA
	private void Update()
	{
		if (this.logic.rotating)
		{
			this.RotateLadder();
		}
		this.UpdateVisibility();
	}

	// Token: 0x04000703 RID: 1795
	public Logic.Ladder logic;

	// Token: 0x04000704 RID: 1796
	public GameObject ladder_obj;

	// Token: 0x04000705 RID: 1797
	public GameObject ladder_preview;

	// Token: 0x04000706 RID: 1798
	private float rot_y;

	// Token: 0x04000707 RID: 1799
	private float start_x;

	// Token: 0x04000708 RID: 1800
	private float final_x;

	// Token: 0x04000709 RID: 1801
	public static GameObject prefab;

	// Token: 0x0400070A RID: 1802
	public static GameObject preview_prefab;
}
