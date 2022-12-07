using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class CapturePoint : GameLogic.Behaviour
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000891 RID: 2193 RVA: 0x0005CF8B File Offset: 0x0005B18B
	// (set) Token: 0x06000892 RID: 2194 RVA: 0x0005CF93 File Offset: 0x0005B193
	public bool MouseOvered
	{
		get
		{
			return this.m_MouseOvered;
		}
		set
		{
			this.m_MouseOvered = value;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000893 RID: 2195 RVA: 0x0005CF9C File Offset: 0x0005B19C
	public bool Highlighted
	{
		get
		{
			BattleViewUI battleViewUI = BattleViewUI.Get();
			return this.MouseOvered || (battleViewUI != null && battleViewUI.picked_capture_point != null && (battleViewUI.picked_capture_point[0] == this || battleViewUI.picked_capture_point[1] == this));
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0005CFEC File Offset: 0x0005B1EC
	public virtual void UpdateSelection()
	{
		if (this.selection != null)
		{
			this.selection.SetActive(this.Highlighted);
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0005D010 File Offset: 0x0005B210
	public virtual float RayCast(Ray ray)
	{
		if (this.logic == null)
		{
			return -1f;
		}
		float num = Vector3.Dot(base.transform.position - ray.origin, ray.direction);
		if (num < 0f)
		{
			return -1f;
		}
		Vector3 b = ray.origin + ray.direction * num;
		Vector3 position = base.transform.position;
		if (this.logic.position.paID > 0)
		{
			position.y = this.logic.position.Height(this.logic.game, position.y, 0f);
		}
		BattleViewUI battleViewUI = BattleViewUI.Get();
		if (battleViewUI.picked_passable_area != 0)
		{
			b = battleViewUI.picked_passable_area_pos;
		}
		float sqrMagnitude = (position - b).sqrMagnitude;
		float num2 = this.radius * 1.75f;
		float num3 = num2 * num2;
		if (sqrMagnitude > num3)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0005D107 File Offset: 0x0005B307
	public int GetBattleSide()
	{
		if (this.logic == null)
		{
			return this.battle_side;
		}
		return this.logic.battle_side;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0005D123 File Offset: 0x0005B323
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0005D12C File Offset: 0x0005B32C
	public override void OnMessage(object obj, string message, object param)
	{
		if (message == "battle_side_changed")
		{
			this.UpdateSelectionCircle();
			this.PlaySideChangedVoice();
			return;
		}
		if (message == "started_capture")
		{
			this.PlayStartCaptureVoice();
			return;
		}
		if (!(message == "destroying"))
		{
			return;
		}
		this.UnregisterMinimapIcon();
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0005D17C File Offset: 0x0005B37C
	public void PlaySideChangedVoice()
	{
		if (this.logic.battle.stage != Logic.Battle.Stage.Ongoing || this.logic.battle.battle_map_finished)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		int num;
		if (this.logic.battle.attacker_kingdom != kingdom)
		{
			Logic.Army attacker_support = this.logic.battle.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom() : null) != kingdom)
			{
				if (this.logic.battle.defender_kingdom != kingdom)
				{
					Logic.Army defender_support = this.logic.battle.defender_support;
					if (((defender_support != null) ? defender_support.GetKingdom() : null) != kingdom)
					{
						return;
					}
				}
				num = 1;
				goto IL_99;
			}
		}
		num = 0;
		IL_99:
		bool flag = num == this.logic.battle_side;
		bool flag2 = num == this.logic.original_battle_side;
		if (flag && flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.OurCapturePointTakenByUs, null);
		}
		else if (!flag && flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.OurCapturePointTakenByEnemy, null);
		}
		else if (flag && !flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.EnemyCapturePointTakenByUs, null);
		}
		else if (!flag && !flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.EnemyCapturePointTakenByEnemy, null);
		}
		Logic.Army army = this.logic.battle.GetArmy(num);
		Logic.Character character = (army != null) ? army.leader : null;
		if (flag)
		{
			BaseUI.PlayCharacterlessVoiceEvent(character, "battle_" + this.logic.def.field.key + "_taken_by_us");
			return;
		}
		BaseUI.PlayCharacterlessVoiceEvent(character, "battle_" + this.logic.def.field.key + "_taken_by_enemy");
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0005D330 File Offset: 0x0005B530
	public void PlayStartCaptureVoice()
	{
		if (this.logic.battle.stage != Logic.Battle.Stage.Ongoing || this.logic.battle.battle_map_finished)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		int num;
		if (this.logic.battle.attacker_kingdom != kingdom)
		{
			Logic.Army attacker_support = this.logic.battle.attacker_support;
			if (((attacker_support != null) ? attacker_support.GetKingdom() : null) != kingdom)
			{
				if (this.logic.battle.defender_kingdom != kingdom)
				{
					Logic.Army defender_support = this.logic.battle.defender_support;
					if (((defender_support != null) ? defender_support.GetKingdom() : null) != kingdom)
					{
						return;
					}
				}
				num = 1;
				goto IL_99;
			}
		}
		num = 0;
		IL_99:
		bool flag = num == this.logic.battle_side;
		bool flag2 = num == this.logic.original_battle_side;
		if (flag && flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.EnemyAttemptsTakeOurCapturePoint, null);
			return;
		}
		if (!flag && flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.WeAttemptTakeOurCapturePoint, null);
			return;
		}
		if (flag && !flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.EnemyAttemptTakeEnemyCapturePoint, null);
			return;
		}
		if (!flag && !flag2)
		{
			BaseUI.PlayVoiceEvent(this.logic.def.WeAttemptTakeEnemyCapturePoint, null);
		}
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0005D468 File Offset: 0x0005B668
	public void SetAreaPaids()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		PassableArea[] componentsInChildren = base.transform.parent.GetComponentsInChildren<PassableArea>();
		this.logic.paids.Clear();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.logic.paids.Add(componentsInChildren[i].id);
		}
		PassableAreaManager paManager = BattleMap.Get().paManager;
		paManager.onFinishPathfinding = (PassableAreaManager.OnFinishPathfinding)Delegate.Remove(paManager.onFinishPathfinding, new PassableAreaManager.OnFinishPathfinding(this.SetAreaPaids));
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0005D4FC File Offset: 0x0005B6FC
	private void CreateLogic()
	{
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Remove(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.CreateLogic));
		if (this == null)
		{
			return;
		}
		PrefabGrid prefabGrid = base.GetComponent<PrefabGrid>();
		if (prefabGrid == null)
		{
			prefabGrid = global::Common.GetParentComponent<PrefabGrid>(base.gameObject);
		}
		this.CreateLogic((prefabGrid != null) ? prefabGrid.type : null);
		if (this.logic.def.count_victory)
		{
			if (this.logic.battle_side == 0)
			{
				BattleMap.battle.simulation.attacker_totals.starting_capture_point_count++;
			}
			else
			{
				BattleMap.battle.simulation.defender_totals.starting_capture_point_count++;
			}
		}
		Collider collider = global::Common.FindChildComponent<Collider>(base.gameObject, "SelectionCollider");
		if (collider != null)
		{
			collider.gameObject.SetLayer(0, true);
		}
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0005D5E4 File Offset: 0x0005B7E4
	public void CreateLogic(string type = null)
	{
		if (this.logic != null && this.logic.IsValid())
		{
			return;
		}
		this.logic = new Logic.CapturePoint(BattleMap.battle, base.transform.position, this.radius, BattleMap.Get().IsInsideWall(base.transform.position), type, this.battle_side);
		this.logic.visuals = this;
		this.radius = this.logic.radius;
		this.CreateSelection();
		this.UpdateSelectionCircle();
		this.flag_block = new MaterialPropertyBlock();
		if (base.transform.parent != null && base.GetComponent<RootPG>() == null)
		{
			this.FindFlags(base.transform.parent);
		}
		else
		{
			this.FindFlags(base.transform);
		}
		this.FindFortification();
		this.RegisterMinimapIcon();
		this.friendly_flag_color = global::Defs.GetColor(this.logic.def.field, "friendly_flag_color", null);
		this.enemy_flag_color = global::Defs.GetColor(this.logic.def.field, "enemy_flag_color", null);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0005D70C File Offset: 0x0005B90C
	private void FindFlags(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Transform child = t.GetChild(i);
			if (child.name == "WavingFlag" || child.name == "FlagHolder")
			{
				child.transform.rotation = Quaternion.identity;
			}
			if (child.name.Contains("WavingFlag"))
			{
				MeshRenderer component = child.GetComponent<MeshRenderer>();
				if (component != null)
				{
					this.flag_renderers.Add(component);
				}
			}
			this.FindFlags(child);
		}
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x0005D79C File Offset: 0x0005B99C
	private void FindFortification()
	{
		global::Fortification parentComponent = global::Common.GetParentComponent<global::Fortification>(base.gameObject);
		if (((parentComponent != null) ? parentComponent.logic : null) != null)
		{
			parentComponent.logic.capture_point = this.logic;
			if (!this.logic.fortifications.Contains(parentComponent.logic))
			{
				this.logic.fortifications.Add(parentComponent.logic);
			}
			foreach (global::Fortification fortification in parentComponent.GetComponentsInChildren<global::Fortification>())
			{
				if (fortification.logic != null && !this.logic.fortifications.Contains(fortification.logic))
				{
					this.logic.fortifications.Add(fortification.logic);
				}
			}
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0005D854 File Offset: 0x0005BA54
	private void CreateSelection()
	{
		Mesh sharedMesh = MeshUtils.CreateGridMesh(new Vector3(-this.radius, 0.5f, -this.radius), (int)this.radius * 2, (int)this.radius * 2, Vector3.forward, Vector3.right, false, false);
		this.selection = new GameObject("_selection");
		this.selection.transform.SetParent(base.transform);
		this.selection.transform.position = base.transform.position;
		this.selection.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
		this.selection.AddComponent<MeshFilter>().sharedMesh = sharedMesh;
		this.selection_renderer = this.selection.AddComponent<MeshRenderer>();
		if (global::CapturePoint.mat == null)
		{
			global::CapturePoint.mat = this.logic.def.field.GetRandomValue("material", null, true, true, true, '.').Get<Material>();
		}
		this.selection_renderer.sharedMaterial = global::CapturePoint.mat;
		this.UpdateSelection();
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0005D974 File Offset: 0x0005BB74
	private void UpdateSelectionCircle()
	{
		if (this.selection_renderer == null)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		int num = global::Battle.PlayerBattleSide();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		BattleMap battleMap = BattleMap.Get();
		if (battleMap != null)
		{
			if (this.GetBattleSide() == num)
			{
				materialPropertyBlock.SetColor("_Color", battleMap.capture_point_friendly);
			}
			else
			{
				materialPropertyBlock.SetColor("_Color", battleMap.capture_point_enemy);
			}
		}
		if (this.logic.IsInsideWall)
		{
			materialPropertyBlock.SetColor("_wall_sdf_mask_col", new Color(0f, 1f, 0f, 1f));
		}
		else
		{
			materialPropertyBlock.SetColor("_wall_sdf_mask_col", new Color(1f, 0f, 0f, 1f));
		}
		this.selection_renderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0005DA44 File Offset: 0x0005BC44
	private void UnregisterMinimapIcon()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return;
		}
		battleViewUI.minimap.DelObj(this.logic);
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0005DA80 File Offset: 0x0005BC80
	private void RegisterMinimapIcon()
	{
		BattleViewUI battleViewUI = BaseUI.Get<BattleViewUI>();
		if (battleViewUI == null || battleViewUI.minimap == null)
		{
			return;
		}
		if (this.logic.def.count_victory)
		{
			battleViewUI.minimap.AddObj(this.logic);
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0005DAD0 File Offset: 0x0005BCD0
	public void UpdateFlags()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Battle battle = BattleMap.battle;
		if (battle != null && battle.IsValid())
		{
			Logic.CapturePoint capturePoint = this.logic;
			if (((capturePoint != null) ? capturePoint.game : null) != null && this.logic.game.IsValid())
			{
				int num = -1;
				if (battle.attacker_kingdom != kingdom)
				{
					Logic.Army attacker_support = battle.attacker_support;
					if (((attacker_support != null) ? attacker_support.GetKingdom() : null) != kingdom)
					{
						if (battle.defender_kingdom != kingdom)
						{
							Logic.Army defender_support = battle.defender_support;
							if (((defender_support != null) ? defender_support.GetKingdom() : null) != kingdom)
							{
								goto IL_82;
							}
						}
						num = 1;
						goto IL_82;
					}
				}
				num = 0;
				IL_82:
				if (num == -1)
				{
					return;
				}
				this.flag_block.SetFloat("_CaptureLerp", this.logic.capture_progress.Get());
				if (num == this.GetBattleSide())
				{
					this.flag_block.SetColor("_Color", this.friendly_flag_color);
					this.flag_block.SetColor("_LightColor", this.enemy_flag_color);
				}
				else
				{
					this.flag_block.SetColor("_Color", this.enemy_flag_color);
					this.flag_block.SetColor("_LightColor", this.friendly_flag_color);
				}
				for (int i = 0; i < this.flag_renderers.Count; i++)
				{
					this.flag_renderers[i].SetPropertyBlock(this.flag_block);
				}
				return;
			}
		}
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0005DC14 File Offset: 0x0005BE14
	private void OnEnable()
	{
		SettlementBV.OnGenerationComplete = (SettlementBV.OnGenerationEvent)Delegate.Combine(SettlementBV.OnGenerationComplete, new SettlementBV.OnGenerationEvent(this.CreateLogic));
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0005DC38 File Offset: 0x0005BE38
	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (((battle != null) ? battle.game : null) == null)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		bool highlighted = this.Highlighted;
		if (highlighted != this.was_highlighted)
		{
			this.UpdateSelection();
			this.was_highlighted = highlighted;
		}
		Logic.CapturePoint capturePoint = this.logic;
		if (((capturePoint != null) ? capturePoint.fortification : null) != null && this.logic.fortification.def.type == Logic.Fortification.Type.Gate)
		{
			return;
		}
		this.UpdateFlags();
	}

	// Token: 0x040006C8 RID: 1736
	public Logic.CapturePoint logic;

	// Token: 0x040006C9 RID: 1737
	private GameObject selection;

	// Token: 0x040006CA RID: 1738
	private MeshRenderer selection_renderer;

	// Token: 0x040006CB RID: 1739
	public float radius;

	// Token: 0x040006CC RID: 1740
	private static Material mat;

	// Token: 0x040006CD RID: 1741
	private List<MeshRenderer> flag_renderers = new List<MeshRenderer>();

	// Token: 0x040006CE RID: 1742
	private MaterialPropertyBlock flag_block;

	// Token: 0x040006CF RID: 1743
	public int battle_side = 1;

	// Token: 0x040006D0 RID: 1744
	private float next_control_check_time;

	// Token: 0x040006D1 RID: 1745
	private Color friendly_flag_color;

	// Token: 0x040006D2 RID: 1746
	private Color enemy_flag_color;

	// Token: 0x040006D3 RID: 1747
	protected bool m_MouseOvered;

	// Token: 0x040006D4 RID: 1748
	private bool was_highlighted;
}
