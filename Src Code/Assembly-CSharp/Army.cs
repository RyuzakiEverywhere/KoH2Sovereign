using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Logic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class Army : GameLogic.Behaviour, VisibilityDetector.IVisibilityChanged
{
	// Token: 0x06000715 RID: 1813 RVA: 0x00049C9A File Offset: 0x00047E9A
	public global::Army.Formation GetFormation()
	{
		return this.cur_formation;
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000716 RID: 1814 RVA: 0x00049CA2 File Offset: 0x00047EA2
	public int MaxUnits
	{
		get
		{
			if (this.logic == null)
			{
				return 10;
			}
			return this.logic.MaxUnits() + 1;
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00049CBC File Offset: 0x00047EBC
	public bool IsVisible()
	{
		return this.logicVisible;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00049CC4 File Offset: 0x00047EC4
	public override Logic.Object GetLogic()
	{
		return this.logic;
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00049CCC File Offset: 0x00047ECC
	public void CreateGameLogic(Game game)
	{
		if (GameLogic.in_create_visuals)
		{
			return;
		}
		this.logic = new Logic.Army(game, base.transform.position, this.kingdom);
		this.logic.visuals = this;
		this.logic.SetNid(this.nid, false);
		Logic.Character c = CharacterFactory.CreateCourtCandidate(game, this.logic.kingdom_id, "Marshal");
		this.logic.SetLeader(c, true);
		this.ResetFormation(true);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00049D51 File Offset: 0x00047F51
	private void Awake()
	{
		if (Application.isPlaying)
		{
			this.CreateGameLogic(GameLogic.Get(true));
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00049D66 File Offset: 0x00047F66
	public static GameObject Prefab()
	{
		return global::Defs.GetObj<GameObject>("Army", "prefab", null);
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x00049D78 File Offset: 0x00047F78
	public static GameObject PVFigurePrefab(global::Army army)
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("PoliticalView", "pv_figures.Army"), "figure_prefab", null);
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x00049D94 File Offset: 0x00047F94
	public static GameObject UIPVFigurePrefab(global::Army army)
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("PoliticalView", "pv_figures.Army"), "ui_figure_prefab", null);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00049DB0 File Offset: 0x00047FB0
	public static GameObject StatusBarPrefab()
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("ArmyStatusBar", null), "window_prefab", null);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00049DC8 File Offset: 0x00047FC8
	public static GameObject HealParticlePrefabs()
	{
		return global::Defs.GetObj<GameObject>(global::Defs.GetDefField("Army", null), "heal_particles", null);
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00049DE0 File Offset: 0x00047FE0
	public static GameObject BannerPrefab(global::Army army)
	{
		if (army == null || army.logic == null)
		{
			return global::Defs.GetObj<GameObject>("Army", "banner", null);
		}
		if (army.ship != null && army.ship.gameObject.activeSelf)
		{
			return global::Defs.GetObj<GameObject>("Army", "ship_banner", null);
		}
		if (army.logic.IsFleeing())
		{
			return global::Defs.GetObj<GameObject>("Army", "banner_fleeing", null);
		}
		bool flag;
		if (army == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Army army2 = army.logic;
			if (army2 == null)
			{
				flag = (null != null);
			}
			else
			{
				Logic.Rebel rebel = army2.rebel;
				flag = (((rebel != null) ? rebel.def : null) != null);
			}
		}
		if (flag)
		{
			return global::Defs.GetObj<GameObject>(army.logic.rebel.def.field, "banner", null);
		}
		if (army.logic.mercenary == null)
		{
			return global::Defs.GetObj<GameObject>("Army", "banner", null);
		}
		if (army.logic.mercenary.former_owner_id != 0)
		{
			return global::Defs.GetObj<GameObject>(army.logic.mercenary.def.field, "banner_hired", null);
		}
		if (army.logic.IsHiredMercenary())
		{
			return global::Defs.GetObj<GameObject>(army.logic.mercenary.def.field, "banner_hired", null);
		}
		return global::Defs.GetObj<GameObject>(army.logic.mercenary.def.field, "banner", null);
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00049F48 File Offset: 0x00048148
	private void UpdateHealParticles()
	{
		bool flag = false;
		if (this.logic.mercenary != null)
		{
			flag = (!this.logic.mercenary.IsBusy() && Timer.Find(this.logic.mercenary, "heal_timer") != null && this.IsVisible() && this.inCameraView);
		}
		else if (this.logic.rebel != null)
		{
			flag = (!this.logic.rebel.IsBusy() && Timer.Find(this.logic.rebel, "heal_timer") != null && this.IsVisible() && this.inCameraView);
		}
		if (flag)
		{
			if (this.heal_particles == null)
			{
				GameObject gameObject = global::Common.Spawn(global::Army.HealParticlePrefabs(), base.transform, false, "");
				if (gameObject == null)
				{
					return;
				}
				gameObject.transform.position = base.transform.position;
				this.heal_particles = gameObject.GetComponentsInChildren<ParticleSystem>();
				if (this.heal_particles == null)
				{
					return;
				}
			}
			for (int i = 0; i < this.heal_particles.Length; i++)
			{
				ParticleSystem particleSystem = this.heal_particles[i];
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
			return;
		}
		if (this.heal_particles != null)
		{
			for (int j = 0; j < this.heal_particles.Length; j++)
			{
				ParticleSystem particleSystem2 = this.heal_particles[j];
				if (particleSystem2.isPlaying)
				{
					particleSystem2.Stop();
				}
			}
		}
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0004A0A8 File Offset: 0x000482A8
	public static GameObject UnitFlagPrefab(global::Army army)
	{
		bool? flag;
		if (army == null)
		{
			flag = null;
		}
		else
		{
			Logic.Army army2 = army.logic;
			if (army2 == null)
			{
				flag = null;
			}
			else
			{
				Logic.Rebel rebel = army2.rebel;
				if (rebel == null)
				{
					flag = null;
				}
				else
				{
					Rebellion rebellion = rebel.rebellion;
					flag = ((rebellion != null) ? new bool?(rebellion.IsFamous()) : null);
				}
			}
		}
		if (flag ?? false)
		{
			return global::Defs.GetObj<GameObject>(army.logic.rebel.def.field, "flag_famous_rebellion", null);
		}
		if (army != null && army.logic.IsCrusadeArmy())
		{
			return global::Defs.GetObj<GameObject>(army.logic.def.field, "flag_crusader_unit", null);
		}
		return null;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0004A174 File Offset: 0x00048374
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Army army = logic_obj as Logic.Army;
		if (army == null)
		{
			return;
		}
		global::Army component;
		using (Game.Profile("Spawn Army prefab", false, 0f, null))
		{
			GameObject gameObject = global::Army.Prefab();
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2;
			if (GameLogic.instance != null)
			{
				gameObject2 = global::Common.Spawn(gameObject, GameLogic.instance.transform, false, "Armies");
			}
			else
			{
				gameObject2 = global::Common.Spawn(gameObject, false, false);
			}
			if (gameObject2 == null)
			{
				return;
			}
			component = gameObject2.GetComponent<global::Army>();
			if (component == null)
			{
				return;
			}
		}
		component.logic = army;
		army.visuals = component;
		component.nid = army.GetNid(true);
		component.kingdom.id = army.kingdom_id;
		component.realm_in = army.realm_in;
		using (Game.Profile("Create Army.SnapToTerrain", false, 0f, null))
		{
			component.transform.position = global::Common.SnapToTerrain(army.position, 0f, WorldMap.GetTerrain(), -1f, false);
		}
		component.UpdateBanner(true);
		component.MatchLogicUnits();
		component.ResetFormation(true);
		component.CheckLeaderSelect();
		component.UpdatePVFigure();
		component.UpdateStatusBar();
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0004A2E0 File Offset: 0x000484E0
	private void UpdatePVFigure()
	{
		using (Game.Profile("Army.UpdatePVFigure", false, 0f, null))
		{
			if (this.ui_pvFigure == null)
			{
				GameObject prefab = global::Army.UIPVFigurePrefab(this);
				WorldUI worldUI = WorldUI.Get();
				GameObject gameObject = global::Common.Spawn(prefab, (worldUI != null) ? worldUI.m_statusBar : null, false, "");
				this.ui_pvFigure = ((gameObject != null) ? gameObject.GetComponent<UIPVFigureArmy>() : null);
				UIPVFigureArmy uipvfigureArmy = this.ui_pvFigure;
				if (uipvfigureArmy != null)
				{
					uipvfigureArmy.SetArmy(this);
				}
			}
			else
			{
				UIPVFigureArmy uipvfigureArmy2 = this.ui_pvFigure;
				if (uipvfigureArmy2 != null)
				{
					uipvfigureArmy2.UpdateArmy();
				}
			}
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0004A388 File Offset: 0x00048588
	private void UpdateBanner(bool force = false)
	{
		using (Game.Profile("Army.UpdateBanner", false, 0f, null))
		{
			GameObject gameObject = global::Army.BannerPrefab(this);
			if (!(gameObject == null))
			{
				Billboard billboard = this.banner;
				GameObject gameObject2 = (billboard != null) ? billboard.gameObject : null;
				if (force || !(gameObject2 != null) || !(gameObject2.name.Replace("(Clone)", "") == gameObject.name))
				{
					if (this.banner != null)
					{
						global::Common.DestroyObj(this.banner.gameObject);
					}
					gameObject2 = global::Common.SpawnPooled(gameObject, base.transform, false, "");
					if (!(gameObject2 == null))
					{
						this.banner = gameObject2.GetComponent<Billboard>();
						if (!(this.banner == null))
						{
							float size = WV_Scale.GetSize(WV_Scale.Object_Type.Unit);
							gameObject2.transform.localPosition = gameObject.transform.localPosition;
							gameObject2.transform.localScale = new Vector3(1f, 1f, 1f) * size;
							Vector3 localPosition = gameObject2.transform.localPosition;
							localPosition.y *= size;
							gameObject2.transform.localPosition = localPosition;
							global::Common.SetRendererLayer(gameObject2, base.gameObject.layer);
							CrestObject[] componentsInChildren = this.banner.GetComponentsInChildren<CrestObject>();
							if (componentsInChildren != null && componentsInChildren.Length != 0)
							{
								for (int i = 0; i < componentsInChildren.Length; i++)
								{
									componentsInChildren[i].RefreshCrest();
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0004A538 File Offset: 0x00048738
	private void UpdateUnitFlagPosRot()
	{
		for (int i = 0; i < global::Army.unit_flag_ids.Length; i++)
		{
			int unit_idx = global::Army.unit_flag_ids[i];
			this.UpdateUnitFlagPosRot(unit_idx);
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0004A568 File Offset: 0x00048768
	private void UpdateUnitFlagPosRot(int unit_idx)
	{
		if (this.units.Count <= unit_idx)
		{
			if (this.unit_flags[unit_idx] != null)
			{
				global::Common.DestroyObj(this.unit_flags[unit_idx]);
			}
			return;
		}
		global::Unit unit = this.units[unit_idx];
		if (unit == null)
		{
			return;
		}
		GameObject gameObject = this.unit_flags[unit_idx];
		if (gameObject != null)
		{
			gameObject.SetActive(unit.enabled);
			gameObject.transform.position = unit.instancer.Position;
			gameObject.transform.rotation = unit.instancer.Rotation;
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0004A604 File Offset: 0x00048804
	private void UpdateUnitFlag(int unit_idx, GameObject prefab, bool force = false)
	{
		if (prefab == null || this.units.Count <= unit_idx)
		{
			if (this.unit_flags[unit_idx] != null)
			{
				UnityEngine.Object.Destroy(this.unit_flags[unit_idx]);
			}
			return;
		}
		global::Unit unit = this.units[unit_idx];
		if (unit == null)
		{
			return;
		}
		if (!force && this.unit_flags[unit_idx] != null && this.unit_flags[unit_idx].name.Replace("(Clone)", "") == prefab.name)
		{
			return;
		}
		if (this.unit_flags[unit_idx] != null)
		{
			UnityEngine.Object.Destroy(this.unit_flags[unit_idx]);
		}
		this.unit_flags[unit_idx] = UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform, false);
		if (this.unit_flags[unit_idx] == null)
		{
			return;
		}
		if (this.unit_flags[unit_idx] == null)
		{
			return;
		}
		Vector3 localPosition = this.unit_flags[unit_idx].transform.localPosition;
		float scale = unit.scale;
		localPosition.y *= scale;
		this.unit_flags[unit_idx].transform.localScale *= scale;
		this.UpdateUnitFlagPosRot(unit_idx);
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0004A734 File Offset: 0x00048934
	private void UpdateUnitFlags(bool force = false)
	{
		using (Game.Profile("Army.UpdateUnitFlags", false, 0f, null))
		{
			GameObject prefab = global::Army.UnitFlagPrefab(this);
			for (int i = 0; i < global::Army.unit_flag_ids.Length; i++)
			{
				int unit_idx = global::Army.unit_flag_ids[i];
				this.UpdateUnitFlag(unit_idx, prefab, force);
			}
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0004A7A0 File Offset: 0x000489A0
	private void UpdateTent()
	{
		if (this.tent != null)
		{
			UnityEngine.Object.Destroy(this.tent.gameObject);
			this.tent = null;
		}
		this.SpawnTent(true);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0004A7CE File Offset: 0x000489CE
	private void UpdateShip()
	{
		if (this.ship != null)
		{
			UnityEngine.Object.Destroy(this.ship.gameObject);
			this.ship = null;
		}
		this.SpawnShip(true);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0004A7FC File Offset: 0x000489FC
	private void UpdateShipCannons()
	{
		if (this.ship_cannons != null)
		{
			UnityEngine.Object.Destroy(this.ship_cannons.gameObject);
			this.ship_cannons = null;
		}
		this.SpawnShipCannons(true);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0004A82C File Offset: 0x00048A2C
	public void MatchLogicUnits()
	{
		if (this.logic == null)
		{
			return;
		}
		using (Game.Profile("Army.MatchLogicUnits", false, 0f, null))
		{
			List<global::Unit> list = this.units;
			this.units = new List<global::Unit>();
			for (int i = 0; i < list.Count; i++)
			{
				list[i].logic = null;
			}
			for (int j = 0; j < this.logic.units.Count; j++)
			{
				Logic.Unit unit = this.logic.units[j];
				bool flag = false;
				for (int k = 0; k < list.Count; k++)
				{
					global::Unit unit2 = list[k];
					if (unit2.logic == null && !(unit2.type != unit.def.id))
					{
						unit2.SetLogic(unit);
						unit2.can_move = true;
						this.units.Add(unit2);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.AddUnit(unit, false, false);
				}
			}
			this.ResetFormation(true);
			this.RecalcFormation();
		}
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0004A95C File Offset: 0x00048B5C
	public void RefreshKingdomColors()
	{
		if (this.banner != null)
		{
			CrestObject[] componentsInChildren = this.banner.GetComponentsInChildren<CrestObject>();
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].RefreshCrest();
				}
			}
		}
		if (this.units != null && this.units.Count > 0)
		{
			for (int j = 0; j < this.units.Count; j++)
			{
				global::Unit unit = this.units[j];
				if (unit != null)
				{
					unit.UpdateColors();
				}
			}
		}
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0004A9E4 File Offset: 0x00048BE4
	private string GetVoiceLine(string key)
	{
		if (this.logic.leader != null)
		{
			return this.logic.leader.GetVoiceLine(key);
		}
		CharacterClass.Def @base = GameLogic.Get(true).defs.GetBase<CharacterClass.Def>();
		if (@base == null)
		{
			return null;
		}
		string result = null;
		@base.voice_lines.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0004AA38 File Offset: 0x00048C38
	private void FaceBattle()
	{
		Logic.Battle battle = this.logic.battle;
		if (battle == null)
		{
			return;
		}
		float y = base.transform.position.y;
		Vector3 vector = global::Common.SnapToTerrain(battle.position, 0f, null, -1f, false);
		if (battle.is_plunder && battle.attackers != null && battle.attackers.Count > 0 && battle.attackers[0] != this.logic && battle.attackers.Contains(this.logic))
		{
			vector = battle.attackers[0].position;
		}
		vector.y = y;
		if (vector != base.transform.position)
		{
			base.transform.LookAt(vector);
			if (!this.logic.currently_on_land)
			{
				base.transform.Rotate(base.transform.up, 90f);
			}
		}
		this.UpdateFormation();
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0004AB34 File Offset: 0x00048D34
	public void UpdateRealmFOW()
	{
		if (this.logic == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.ui != null && kingdom != null && kingdom.IsValid() && (this.logic.IsOwnOrCrusader(kingdom) || this.logic.IsAllyOrTeammate(kingdom)))
		{
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				worldMap.ReloadView();
			}
			Game game = GameLogic.Get(true);
			if (game != null && game.fow && this.realm_in != null)
			{
				Logic.Realm realm = this.realm_in;
				global::Realm realm2 = ((realm != null) ? realm.visuals : null) as global::Realm;
				if (realm2 == null)
				{
					return;
				}
				realm2.UpdateFow(false, true);
			}
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0004ABD8 File Offset: 0x00048DD8
	public static void RefreshAllArmyVisibility(Game game)
	{
		List<Logic.Realm> realms = game.realms;
		for (int i = 0; i < realms.Count; i++)
		{
			Logic.Realm realm = realms[i];
			for (int j = 0; j < realm.armies.Count; j++)
			{
				Logic.Army army = realm.armies[j];
				if (army.battle != null)
				{
					global::Battle battle = army.battle.visuals as global::Battle;
					if (battle != null)
					{
						battle.UpdateVisibility();
					}
				}
				global::Army army2 = army.visuals as global::Army;
				if (army2 != null)
				{
					army2.UpdateVisibility(false);
				}
			}
		}
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0004AC7C File Offset: 0x00048E7C
	public override void OnMessage(object obj, string message, object param)
	{
		if (message == "realm_crossed")
		{
			this.UpdateRealmFOW();
			if (this.logic != null)
			{
				if (this.logic.leader != null)
				{
					this.logic.leader.NotifyListeners("status_changed", null);
				}
				Logic.Realm realm = this.realm_in;
				this.realm_in = this.logic.realm_in;
				Game game = GameLogic.Get(true);
				if (game != null && game.fow)
				{
					if (this.ui != null && (this.logic.IsOwnOrCrusader(BaseUI.LogicKingdom()) || this.logic.IsAllyOrTeammate(BaseUI.LogicKingdom())) && this.realm_in != null)
					{
						(this.realm_in.visuals as global::Realm).UpdateFow(false, true);
					}
					else
					{
						this.UpdateVisibility(false);
					}
				}
				if (base.gameObject.activeSelf && realm != null && this.logic.realm_in != null && realm != this.logic.realm_in)
				{
					global::Kingdom kingdom = global::Kingdom.Get(realm.kingdom_id);
					global::Kingdom kingdom2 = global::Kingdom.Get(this.logic.realm_in.kingdom_id);
					bool flag = this.logic.IsEnemy(realm);
					bool flag2 = this.kingdom == realm.kingdom_id;
					bool flag3 = !flag && !flag2 && kingdom != null;
					bool flag4 = this.logic.IsEnemy(this.logic.realm_in);
					bool flag5 = this.kingdom == this.logic.realm_in.kingdom_id;
					bool flag6 = !flag4 && !flag5 && kingdom2 != null;
					Vars vars = new Vars(this.logic);
					vars.Set<Logic.Object>("realm", this.logic.realm_in);
					vars.Set<Logic.Object>("prev_realm", realm);
					Logic.Kingdom kingdom3 = this.logic.realm_in.GetKingdom();
					if (!flag5 && this.ui != null && this.kingdom == this.ui.kingdom)
					{
						string text;
						if (kingdom3 == null)
						{
							text = null;
						}
						else
						{
							Religion religion = kingdom3.religion;
							text = ((religion != null) ? religion.name : null);
						}
						string religion2 = text;
						BackgroundMusic.OnTrigger("ArmyEnterProvinceTrigger", religion2);
					}
					Logic.Kingdom kingdom4 = BaseUI.LogicKingdom();
					Logic.Kingdom kingdom5 = this.logic.GetKingdom();
					if (kingdom3 == kingdom4 && kingdom5.IsEnemy(kingdom4) && (kingdom5.type == Logic.Kingdom.Type.Crusade || kingdom5.type == Logic.Kingdom.Type.Regular))
					{
						BackgroundMusic.OnTrigger("TensionTrigger", this.logic.realm_in.religion.name);
					}
					if (flag2 && flag4)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_own_to_enemy", vars, false);
						return;
					}
					if (flag && flag5)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_enemy_to_own", vars, false);
						return;
					}
					if (flag && flag4)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_enemy_to_enemy", vars, false);
						return;
					}
					if (flag3 && flag6)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_neutral_to_neutral", vars, false);
						return;
					}
					if (flag && flag6)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_enemy_to_neutral", vars, false);
						return;
					}
					if (flag3 && flag4)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_neutral_to_enemy", vars, false);
						return;
					}
					if (flag3 && flag5)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_neutral_to_own", vars, false);
						return;
					}
					if (flag2 && flag6)
					{
						FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "realm_own_to_neutral", vars, false);
					}
				}
			}
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		if (message == "merc_action_changed")
		{
			if (base.gameObject.activeSelf)
			{
				switch (this.logic.mercenary.current_action)
				{
				case Mercenary.Action.Attack:
					FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "merc_attack", null, false);
					break;
				case Mercenary.Action.Rest:
					FloatingText.Create(base.gameObject, "FloatingTexts.Normal", "merc_rest", null, false);
					break;
				}
			}
			this.UpdateHealParticles();
			return;
		}
		if (message == "moved")
		{
			this.UpdateSoundLoop(false);
			this.Moved();
			UIPVFigureArmy uipvfigureArmy = this.ui_pvFigure;
			if (uipvfigureArmy == null)
			{
				return;
			}
			uipvfigureArmy.Refresh();
			return;
		}
		else
		{
			if (message == "path_changed")
			{
				if (this.logic != null && this.logic.leader != null)
				{
					this.logic.leader.NotifyListeners("status_changed", null);
				}
				this.UpdateVisibility(false);
				this.UpdatePVFigure();
				if (this.selected)
				{
					this.CreatePathArrows();
				}
				this.UpdateSoundLoop(this.logic.water_crossing.running);
				this.UpdateBanner(false);
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "enter_castle")
			{
				this.UpdateVisibility(false);
				this.UpdateSoundLoop(false);
				this.RecreateSelectionUI();
				if (this.logic.GetKingdom().is_local_player && this.logic.castle != null)
				{
					Vars vars2 = new Vars();
					vars2.Set<string>("province", this.logic.realm_in.name);
					vars2.Set<string>("armyID", this.logic.GetNid(true).ToString());
					if (this.logic.leader != null)
					{
						vars2.Set<string>("marshalName", this.logic.leader.Name);
					}
					vars2.Set<string>("targetProvince", this.logic.realm_in.name);
					vars2.Set<string>("targetType", this.logic.castle.GetType().ToString());
					vars2.Set<string>("targetRelation", this.logic.castle.GetStance(this.logic).ToString());
					string val = this.logic.realm_in.GetStance(this.logic).ToString();
					vars2.Set<string>("previousProvinceRelation", val);
					vars2.Set<string>("nextProvinceRelation", val);
					this.logic.GetKingdom().NotifyListeners("analytics_army_entered_castle", vars2);
				}
				return;
			}
			if (message == "leave_castle")
			{
				this.ResetFormation(true);
				this.UpdateVisibility(false);
				this.UpdateSoundLoop(false);
				if (this.ui != null && param != null)
				{
					global::Settlement settlement = (param as Castle).visuals as global::Settlement;
					bool flag7 = settlement.GetControllerKingdomID() == BaseUI.LogicKingdom().id;
					if (this.ui.IsSelected(settlement.gameObject) && (this.exitingCasle || !flag7))
					{
						this.ui.SelectObj(base.gameObject, false, true, true, false);
					}
				}
				this.exitingCasle = false;
				return;
			}
			if (message == "units_changed")
			{
				this.MatchLogicUnits();
				this.UpdateSoundLoop(false);
				return;
			}
			if (message == "battle_changed")
			{
				if (this.logic != null && this.logic.leader != null)
				{
					this.logic.leader.NotifyListeners("status_changed", null);
				}
				Logic.Battle battle = this.logic.battle;
				if (battle != null)
				{
					if (this.logicVisible && battle.GetArmy(0) == this.logic)
					{
						Logic.Character leader = this.logic.leader;
						this.PlayVoiceLine((leader != null) ? leader.GetVoiceLine("Attack") : null, this.soundEffects.Attack, null);
					}
					this.FaceBattle();
					this.ResetFormation(true);
				}
				else
				{
					this.RecalcFormation();
				}
				if (this.realm_in != ((battle != null) ? battle.GetRealm() : null))
				{
					this.UpdateRealmFOW();
				}
				this.RecreateSelectionUI();
				this.UpdatePVFigure();
				this.UpdateVisibility(true);
				this.UpdateSoundLoop(false);
				return;
			}
			if (message == "battle_stage_changed")
			{
				Logic.Battle battle2 = this.logic.battle;
				if (battle2 != null)
				{
					float y = base.transform.position.y;
					Vector3 vector = battle2.position;
					vector.y = y;
					if (vector == base.transform.position && battle2.attackers.Count > 0 && battle2.attackers[0] != null)
					{
						vector = battle2.attackers[0].position;
						vector.y = y;
					}
					if (vector != base.transform.position)
					{
						base.transform.LookAt(vector);
						if (this.logic.is_in_water)
						{
							base.transform.Rotate(base.transform.up, 90f);
						}
					}
				}
				if (!this.logic.battle.IsFinishing())
				{
					this.ResetFormation(true);
				}
				this.UpdateChildrenVisibility(true, false);
				this.UpdateBanner(false);
				this.UpdateUnitFlags(false);
				this.UpdateSoundLoop(false);
				return;
			}
			if (message == "unit_killed")
			{
				global::Unit unit = this.GetUnit(param as Logic.Unit);
				if (unit != null)
				{
					this.DelUnit(unit);
				}
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "camp_setup")
			{
				this.UpdateChildrenVisibility(true, false);
				this.UpdateBanner(false);
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "camp_setoff")
			{
				this.UpdateChildrenVisibility(true, false);
				this.UpdateBanner(false);
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "kingdom_changed")
			{
				this.kingdom.id = this.logic.kingdom_id;
				this.UpdateTent();
				this.UpdateChildrenVisibility(true, false);
				this.UpdateBanner(true);
				this.UpdateUnitFlags(true);
				this.UpdatePVFigure();
				this.UpdateStatusBar();
				this.RecreateSelectionUI();
				return;
			}
			if (message == "moved_in_water")
			{
				this.marching_sound_emitter.Stop();
				this.ResetFormation(true);
				this.UpdateChildrenVisibility(true, true);
				this.UpdateSoundLoop(this.logic.water_crossing.running);
				this.UpdateBanner(false);
				this.UpdateUnitFlags(false);
				this.UpdatePVFigure();
				this.UpdateStatusBar();
				return;
			}
			if (message == "started")
			{
				this.ResetFormation(false);
				if (this.ui != null && this.kingdom == this.ui.kingdom)
				{
					WorldMap worldMap = WorldMap.Get();
					if (worldMap != null)
					{
						worldMap.ReloadView();
					}
					Game game2 = GameLogic.Get(true);
					if (game2 != null && game2.fow && this.logic != null)
					{
						Logic.Realm realm2 = this.logic.realm_in;
						global::Realm realm3 = ((realm2 != null) ? realm2.visuals : null) as global::Realm;
						if (realm3 == null)
						{
							return;
						}
						realm3.UpdateFow(false, true);
					}
				}
				return;
			}
			if (message == "leader_changed")
			{
				this.CheckLeaderSelect();
				return;
			}
			if (message == "reset_formation")
			{
				this.ResetFormation(true);
				return;
			}
			if (message == "inventory_changed")
			{
				return;
			}
			if (message == "buyers_changed")
			{
				Logic.Army army = param as Logic.Army;
				if (army != null && army.kingdom_id == this.ui.GetCurrentKingdomId())
				{
					global::Army army2 = army.visuals as global::Army;
					if (army2 != null && this.ui.selected_obj == army2.gameObject && this.logic.mercenary != null && base.gameObject != null)
					{
						this.ui.SelectObj(base.gameObject, false, true, true, true);
					}
				}
			}
			if (message == "inetractors_changed" && this.ui != null && this.logic.kingdom_id == this.ui.GetCurrentKingdomId() && this.ui.selected_obj == base.gameObject)
			{
				this.ui.SelectObj(base.gameObject, false, true, false, false);
			}
			if (message == "flee")
			{
				if (!GameLogic.Get(false).GetKingdom(this.kingdom).is_player && BaseUI.CanControlAI())
				{
					this.Retreat();
					return;
				}
				if (BaseUI.LogicKingdom().id != this.kingdom)
				{
					return;
				}
				MessageWnd messageWnd = MessageWnd.Create("RetreatMessage", new Vars(obj), null, new MessageWnd.OnButton(this.OnRetreatMessage));
				if (messageWnd != null)
				{
					messageWnd.on_update = delegate(MessageWnd w)
					{
						if (this == null || this.logic == null || this.logic.battle == null)
						{
							w.Close(false);
						}
					};
				}
			}
			if (message == "retreat")
			{
				if (base.gameObject.activeSelf)
				{
					FloatingText.Create(base.gameObject.transform.position, "FloatingTexts.Normal", "retreat", null, true);
					this.PlayVoiceLine(this.GetVoiceLine("Retreat"), this.soundEffects.Retreat, null);
				}
				return;
			}
			if (message == "became_rebel" || message == "became_mercenary" || message == "became_regular" || message == "mercenary_changed")
			{
				BaseUI baseUI = BaseUI.Get();
				this.RecreateSelectionUI();
				if (this.logic != null && baseUI.selected_obj == base.gameObject)
				{
					baseUI.SelectObjFromLogic(this.logic, false, true);
				}
				this.UpdateBanner(true);
				this.UpdateUnitFlags(true);
				this.UpdatePVFigure();
				this.UpdateStatusBar();
				this.UpdateTent();
				this.UpdateChildrenVisibility(true, false);
				if (this.logic.mercenary != null)
				{
					this.logic.mercenary.AddListener(this);
					UIImportantEvents.UpdateCategory("mercenary");
					UIHiredMercenaries.UpdateMercList();
				}
				return;
			}
			if (message == "rebellion_changed" || message == "crusader_status_changed")
			{
				if (this.ui_pvFigure != null)
				{
					this.ui_pvFigure.UpdateArmy();
				}
				if (this.statusBar != null)
				{
					this.statusBar.Refresh();
				}
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "rebel_def_update" || message == "rebel_army_update")
			{
				this.UpdateVisibility(false);
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "rebellion_famous_state_changed")
			{
				this.UpdateUnitFlags(false);
				return;
			}
			if (message == "destroying" || message == "finishing")
			{
				if (this.logic.mercenary != null || obj is Mercenary)
				{
					UIImportantEvents.UpdateCategory("mercenary");
					UIHiredMercenaries.UpdateMercList();
				}
				if (obj == this.logic)
				{
					this.UnregisterToMinimap();
					this.UpdateRealmFOW();
					this.logic.DelListener(this);
					this.logic = null;
					if (this.selected)
					{
						this.ui.SelectObj(null, false, true, true, true);
					}
					UnityEngine.Object.DestroyImmediate(base.gameObject);
					return;
				}
			}
			Logic.Army army3 = this.logic;
			if (obj != ((army3 != null) ? army3.mercenary : null) || (!(message == "destroying") && !(message == "finishing")))
			{
				if (message == "realm_crossed_analytics")
				{
					Movement movement = this.logic.movement;
					if (((movement != null) ? movement.path : null) == null)
					{
						return;
					}
					Path path = this.logic.movement.path;
					WorldMap worldMap2 = WorldMap.Get();
					if (worldMap2 == null)
					{
						return;
					}
					global::Realm realm4 = worldMap2.RealmAt(path.dst_pt.x, path.dst_pt.y);
					Logic.Realm realm5 = (realm4 != null) ? realm4.logic : null;
					if (realm5 == null)
					{
						return;
					}
					string val2 = param as string;
					Vars vars3 = new Vars();
					vars3.Set<string>("province", this.logic.realm_in.name);
					vars3.Set<string>("armyID", this.logic.GetNid(true).ToString());
					if (this.logic.leader != null)
					{
						vars3.Set<string>("marshalName", this.logic.leader.Name);
					}
					vars3.Set<string>("targetProvince", realm5.name);
					vars3.Set<string>("targetType", (path.dst_obj != null) ? path.dst_obj.GetType().ToString() : "position");
					vars3.Set<string>("targetRelation", (path.dst_obj != null) ? path.dst_obj.GetStance(this.logic).ToString() : realm5.GetStance(this.logic).ToString());
					vars3.Set<string>("previousProvinceRelation", val2);
					vars3.Set<string>("nextProvinceRelation", this.logic.realm_in.GetStance(this.logic).ToString());
					this.logic.GetKingdom().NotifyListeners("analytics_army_crossed_realm", vars3);
				}
				if (message == "mercenary_unit_hired")
				{
					if (BaseUI.LogicKingdom() != this.logic.GetKingdom())
					{
						return;
					}
					BaseUI.PlayVoiceEvent(this.logic.leader.GetVoiceLine("HireMercenaryUnit"), this.logic.leader);
				}
				if (message == "mercenary_army_hired")
				{
					if (BaseUI.LogicKingdom() != this.logic.GetKingdom())
					{
						return;
					}
					BaseUI.PlayVoiceEvent(this.logic.leader.GetVoiceLine("HireMercenaryArmy"), this.logic.leader);
				}
				return;
			}
			Logic.Army army4 = this.logic;
			if (army4 == null)
			{
				return;
			}
			Mercenary mercenary = army4.mercenary;
			if (mercenary == null)
			{
				return;
			}
			mercenary.DelListener(this);
			return;
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0004BDE4 File Offset: 0x00049FE4
	private void Retreat()
	{
		if (this.logic == null || this.logic.battle == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null && this.logic != null)
		{
			if (kingdom.id == this.logic.battle.attacker_kingdom.id)
			{
				this.logic.battle.DoAction("retreat", 0, "");
				return;
			}
			if (kingdom.id == this.logic.battle.defender_kingdom.id)
			{
				this.logic.battle.DoAction("retreat", 1, "");
				return;
			}
			if (this.logic == this.logic.battle.attacker_support)
			{
				this.logic.battle.DoAction("retreat_supporters", 0, "");
				return;
			}
			if (this.logic == this.logic.battle.defender_support)
			{
				this.logic.battle.DoAction("retreat_supporters", 1, "");
			}
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0004BEF7 File Offset: 0x0004A0F7
	public bool OnRetreatMessage(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.Retreat();
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0004BF14 File Offset: 0x0004A114
	public static global::Army Get(Logic.Army logic)
	{
		if (logic == null)
		{
			return null;
		}
		return logic.visuals as global::Army;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0004BF26 File Offset: 0x0004A126
	public static global::Army Get(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		return go.GetComponent<global::Army>();
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0004BF39 File Offset: 0x0004A139
	public static global::Army Get(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		return t.GetComponent<global::Army>();
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x0004BF4C File Offset: 0x0004A14C
	public global::Settlement GetCastle()
	{
		if (this.logic == null || this.logic.castle == null)
		{
			return null;
		}
		return this.logic.castle.visuals as global::Settlement;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0004BF7A File Offset: 0x0004A17A
	public global::Battle GetBattle()
	{
		if (this.logic == null || this.logic.battle == null)
		{
			return null;
		}
		return this.logic.battle.visuals as global::Battle;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0004BFA8 File Offset: 0x0004A1A8
	public void RecreateSelectionUI()
	{
		if (this.ui != null)
		{
			this.ui.RefreshSelection(base.gameObject);
		}
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0004BFCC File Offset: 0x0004A1CC
	private void CreateRelationMarker()
	{
		if (this.relation != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.relation = MeshUtils.CreateRelectionDisc(base.gameObject, 4.2f, baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, true));
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0004C040 File Offset: 0x0004A240
	private void DestroyRelationMarker()
	{
		if (this.relation != null)
		{
			UnityEngine.Object.Destroy(this.relation.sharedMaterial);
			MeshFilter component = this.relation.GetComponent<MeshFilter>();
			UnityEngine.Object.Destroy((component != null) ? component.sharedMesh : null);
			UnityEngine.Object.Destroy(this.relation.gameObject);
			this.relation = null;
		}
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0004C0A0 File Offset: 0x0004A2A0
	public void CreateSelection()
	{
		if (this.selection != null)
		{
			return;
		}
		if (this.logic == null || !this.logic.started)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null || !baseUI.SelectionShown())
		{
			return;
		}
		this.selection = MeshUtils.CreateSelectionCircle(base.gameObject, 4f, baseUI.GetSelectionMaterial(base.gameObject, null, this.primarySelection, false), 0.25f);
		MeshUtils.SnapSelectionToTerrain(this.selection, WorldMap.GetTerrain());
		this.CreateRelationMarker();
		this.CreatePathArrows();
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0004C138 File Offset: 0x0004A338
	public void DestroySelection()
	{
		this.DestroyPathArrows();
		this.DestroyRelationMarker();
		if (this.selection == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.selection.sharedMaterial);
		MeshFilter component = this.selection.GetComponent<MeshFilter>();
		UnityEngine.Object.Destroy((component != null) ? component.sharedMesh : null);
		UnityEngine.Object.Destroy(this.selection.gameObject);
		this.selection = null;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0004C1A4 File Offset: 0x0004A3A4
	public void UpdateSelection()
	{
		if (this.selection != null)
		{
			this.selection.material = this.ui.GetSelectionMaterial(base.gameObject, this.selection.material, this.primarySelection, false);
			MeshUtils.SnapSelectionToTerrain(this.selection, WorldMap.GetTerrain());
		}
		if (this.relation != null)
		{
			this.relation.material = this.ui.GetSelectionMaterial(base.gameObject, this.relation.material, this.primarySelection, false);
		}
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0004C239 File Offset: 0x0004A439
	private void DestroyPathArrows()
	{
		if (this.path_arrows == null)
		{
			return;
		}
		global::Common.DestroyObj(this.path_arrows.gameObject);
		this.path_arrows = null;
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0004C264 File Offset: 0x0004A464
	private void CreatePathArrows()
	{
		this.DestroyPathArrows();
		if (!this.ui.PathArrowsShown())
		{
			return;
		}
		Logic.Kingdom kingdom = this.logic.GetKingdom();
		if (kingdom == null)
		{
			return;
		}
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		bool dark = false;
		if (!this.logic.IsOwnOrCrusader(obj))
		{
			if (kingdom.IsAllyOrTeammate(obj))
			{
				dark = true;
			}
			else if (!BaseUI.CanControlAI())
			{
				return;
			}
		}
		Path path = this.logic.movement.path;
		if (path == null || path.IsDone())
		{
			return;
		}
		this.path_arrows = PathArrows.Create(this.logic.movement, 3, dark);
		this.path_arrows.gameObject.layer = base.gameObject.layer;
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0004C310 File Offset: 0x0004A510
	public void SetSelected(bool bSelected, bool bPrimaryselection = true)
	{
		this.selected = bSelected;
		this.primarySelection = bPrimaryselection;
		if (this.selected)
		{
			this.CreateSelection();
		}
		else
		{
			this.DestroySelection();
		}
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateIcon();
		}
		if (this.selected)
		{
			Logic.Kingdom kingdom = this.logic.GetKingdom();
			if (((kingdom != null) ? kingdom.religion : null) == null)
			{
				return;
			}
			BackgroundMusic.OnTrigger("SelArmyTrigger", kingdom.religion.name);
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0004C392 File Offset: 0x0004A592
	public bool IsSelected()
	{
		return this.selected;
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0004C39A File Offset: 0x0004A59A
	public void Stop()
	{
		this.logic.MoveTo(this.logic.position, 0f, false);
		this.DestroyPathArrows();
		this.UpdateFormation();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0004C3C4 File Offset: 0x0004A5C4
	public void MoveTo(global::Army a, bool confirmation = true, bool validate = true)
	{
		if (!this.MoveAllowed())
		{
			return;
		}
		if (a == this)
		{
			this.Stop();
			return;
		}
		if (validate && !this.ValidateAction(delegate
		{
			this.MoveTo(a, confirmation, false);
		}))
		{
			return;
		}
		if (confirmation && this.logic.battle == null)
		{
			if (a.logic.battle == null)
			{
				if (this.logic.IsEnemy(a.logic))
				{
					if (a.logic.rebel != null)
					{
						this.PlayVoiceLine(this.GetVoiceLine("AttackRebels"), this.soundEffects.Attack, null);
					}
					else
					{
						this.PlayVoiceLine(this.GetVoiceLine("Attack"), this.soundEffects.Attack, null);
					}
					FloatingText.Create(a.transform.position, "FloatingTexts.Attacking", "attack", null, true);
				}
				else
				{
					this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
					string text = (!this.logic.currently_on_land) ? "disembark" : "move";
					FloatingText.Create(a.transform.position, "FloatingTexts.Normal", text, null, true);
				}
			}
			else if (a.logic.battle.CanJoin(this.logic))
			{
				if (this.logic.leader != null)
				{
					Logic.Settlement settlement = a.logic.battle.settlement;
					if (((settlement != null) ? settlement.keep_effects : null) != null && this.logic.IsEnemy(a.logic.battle.attacker))
					{
						Vars vars = new Vars(this.logic.leader);
						vars.Set<Logic.Battle>("battle", a.logic.battle);
						this.PlayVoiceLine(this.GetVoiceLine("BreakSiegeFromOutside"), this.soundEffects.Attack, vars);
					}
					else if (a.logic.rebel != null && this.logic.IsEnemy(a.logic))
					{
						this.PlayVoiceLine(this.GetVoiceLine("AttackRebels"), this.soundEffects.Attack, null);
					}
					else
					{
						this.PlayVoiceLine(this.GetVoiceLine("Attack"), this.soundEffects.Attack, null);
					}
				}
				FloatingText.Create(a.transform.position, "FloatingTexts.Attacking", "attack", null, true);
			}
			else
			{
				this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
				string text2 = (!this.logic.currently_on_land) ? "disembark" : "move";
				FloatingText.Create(a.transform.position, "FloatingTexts.Normal", text2, null, true);
			}
		}
		this.exitingCasle = (this.logic.castle != null);
		this.logic.MoveTo(a.logic, -1f, true);
		BaseUI.Get().SpawnClickReticle();
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x0004C74C File Offset: 0x0004A94C
	public void MoveTo(global::Settlement s, bool confirmation = true, bool validate = true)
	{
		if (!this.MoveAllowed())
		{
			return;
		}
		Logic.Settlement castle = this.logic.castle;
		global::Settlement s2 = s;
		if (castle == ((s2 != null) ? s2.logic : null))
		{
			return;
		}
		if (validate && !this.ValidateAction(delegate
		{
			this.MoveTo(s, confirmation, false);
		}))
		{
			return;
		}
		if (confirmation && this.logic.battle == null)
		{
			if (this.logic.IsEnemy(s.logic) && s.logic.battle == null && !s.logic.razed)
			{
				if (s.IsCastle() && Logic.Battle.CanSiege(this.logic))
				{
					FloatingText.Create(s.transform.position, "FloatingTexts.Attacking", "assault", null, true);
					this.PlayVoiceLine(this.GetVoiceLine("Siege"), this.soundEffects.Attack, null);
				}
				else if (Logic.Battle.CanPillage(this.logic, s.logic))
				{
					FloatingText.Create(s.transform.position, "FloatingTexts.Attacking", "plunder", null, true);
					this.PlayVoiceLine(this.GetVoiceLine("Pillage"), this.soundEffects.Attack, null);
				}
				else
				{
					string text = (!this.logic.currently_on_land) ? "disembark" : "move";
					FloatingText.Create(s.transform.position, "FloatingTexts.Normal", text, null, true);
					this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
				}
			}
			else if (s.logic.battle != null)
			{
				if (s.logic.battle.CanJoin(this.logic))
				{
					FloatingText.Create(s.transform.position, "FloatingTexts.Attacking", "attack", null, true);
					if (s.logic.keep_effects != null && this.logic.IsEnemy(s.logic.battle.attacker))
					{
						if (this.logic.leader != null)
						{
							Vars vars = new Vars(this.logic.leader);
							vars.Set<Logic.Battle>("battle", s.logic.battle);
							this.PlayVoiceLine(this.GetVoiceLine("BreakSiegeFromOutside"), this.soundEffects.Attack, vars);
						}
					}
					else
					{
						this.PlayVoiceLine(this.GetVoiceLine("Attack"), this.soundEffects.Attack, null);
					}
				}
				else
				{
					string text2 = (!this.logic.currently_on_land) ? "disembark" : "move";
					FloatingText.Create(s.transform.position, "FloatingTexts.Normal", text2, null, true);
					this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
				}
			}
			else if (s.IsCastle() && s.GetKingdomID() == this.kingdom.id)
			{
				FloatingText.Create(s.transform.position, "FloatingTexts.Normal", "deploy", null, true);
				this.PlayVoiceLine(this.GetVoiceLine("Deploy"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
			}
			else
			{
				string text3 = (!this.logic.currently_on_land) ? "disembark" : "move";
				FloatingText.Create(s.transform.position, "FloatingTexts.Normal", text3, null, true);
				this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
			}
		}
		this.exitingCasle = (this.logic.castle != null);
		this.logic.MoveTo(s.logic, -1f, true);
		BaseUI.Get().SpawnClickReticle();
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0004CBF8 File Offset: 0x0004ADF8
	private bool ValidateAction(Action onValidate)
	{
		global::Army.<>c__DisplayClass95_0 CS$<>8__locals1 = new global::Army.<>c__DisplayClass95_0();
		CS$<>8__locals1.onValidate = onValidate;
		CS$<>8__locals1.<>4__this = this;
		Logic.Army army = this.logic;
		bool flag;
		if (army == null)
		{
			flag = false;
		}
		else
		{
			Logic.Character leader = army.leader;
			Action.State? state;
			if (leader == null)
			{
				state = null;
			}
			else
			{
				Action cur_action = leader.cur_action;
				state = ((cur_action != null) ? new Action.State?(cur_action.state) : null);
			}
			Action.State? state2 = state;
			Action.State state3 = Action.State.Inactive;
			flag = (state2.GetValueOrDefault() > state3 & state2 != null);
		}
		if (!flag)
		{
			return true;
		}
		if (this.logic.leader.cur_action.def.secondary)
		{
			return true;
		}
		MessageWnd.OnButton on_button = delegate(MessageWnd wnd, string btn_id)
		{
			if (btn_id == "ok")
			{
				CS$<>8__locals1.onValidate();
			}
			wnd.CloseAndDismiss(true);
			return true;
		};
		Vars vars = new Vars(this.logic.leader);
		vars.Set<Action>("action", this.logic.leader.cur_action);
		MessageWnd messageWnd = MessageWnd.Create("CancelCurrentActionMessage", new Vars(vars), null, on_button);
		if (messageWnd != null)
		{
			messageWnd.on_update = delegate(MessageWnd w)
			{
				object obj = vars.Get("action", true).Object(true);
				if (CS$<>8__locals1.<>4__this.logic.leader.cur_action != obj)
				{
					w.Close(false);
				}
			};
		}
		return false;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x0004CD34 File Offset: 0x0004AF34
	private bool MoveAllowed()
	{
		if (this.logic.battle != null)
		{
			return false;
		}
		bool flag = BaseUI.CanControlAI();
		if (this.logic.IsMercenary() && !flag)
		{
			return false;
		}
		int num = this.kingdom;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		int? num2 = (kingdom != null) ? new int?(kingdom.id) : null;
		bool flag2 = num == num2.GetValueOrDefault() & num2 != null;
		Logic.Army army = this.logic;
		bool flag3 = army == null || army.IsHeadless();
		if (flag2 && !flag3)
		{
			return !this.logic.IsFleeing() || this.logic.def.can_interrupt_flee;
		}
		return flag;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0004CDE4 File Offset: 0x0004AFE4
	public void MoveTo(Vector3 pt, int passableArea = 0, bool confirmation = true, bool validate = true)
	{
		if (!this.MoveAllowed())
		{
			return;
		}
		if (validate && !this.ValidateAction(delegate
		{
			this.MoveTo(pt, passableArea, confirmation, false);
		}))
		{
			return;
		}
		bool key = UICommon.GetKey(KeyCode.LeftShift, false);
		bool key2 = UICommon.GetKey(KeyCode.LeftControl, false);
		bool flag = key2 && key && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat teleport army", true);
		bool flag2 = key2 && !key && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat flee army", true);
		Logic.Realm realm = this.logic.game.GetRealm(pt);
		if (confirmation && this.logic.battle == null)
		{
			string text = "move";
			if (flag2)
			{
				this.PlayVoiceLine(this.GetVoiceLine("Retreat"), this.soundEffects.Retreat, null);
			}
			else if (realm == null)
			{
				this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
			}
			else if (this.logic.is_in_water && realm.IsSeaRealm())
			{
				this.PlayVoiceLine(this.GetVoiceLine("Move"), this.soundEffects.MoveWater, null);
				text = "sail";
			}
			else if (this.logic.is_in_water && realm.id >= 0)
			{
				this.PlayVoiceLine(this.GetVoiceLine("Land"), this.soundEffects.MoveWater, null);
				text = "disembark";
			}
			else if (!this.logic.is_in_water && realm.IsSeaRealm())
			{
				this.PlayVoiceLine(this.GetVoiceLine("Board"), this.soundEffects.MoveLand, null);
				text = "embark";
			}
			else
			{
				this.PlayVoiceLine(this.GetVoiceLine("Move"), this.logic.is_in_water ? this.soundEffects.MoveWater : this.soundEffects.MoveLand, null);
			}
			FloatingText.Create(pt, "FloatingTexts.Normal", text, null, true);
		}
		PPos pt2 = new PPos(pt, passableArea);
		if (flag)
		{
			this.logic.LeaveCastle(pt2, true);
			this.logic.Stop(true);
			this.logic.Teleport(pt2);
			base.transform.position = pt;
			if (this.visibility_index >= 0)
			{
				VisibilityDetector.Move(this.visibility_index, pt, -1f);
			}
			this.UpdateSelection();
			this.ResetFormation(true);
			return;
		}
		if (flag2)
		{
			this.logic.FleeFrom(pt2, this.logic.def.flee_distance);
			return;
		}
		this.exitingCasle = (this.logic.castle != null);
		this.logic.MoveTo(pt2, 0f, true);
		BaseUI.Get().SpawnClickReticle();
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x0004D0F0 File Offset: 0x0004B2F0
	private void SetFormation(global::Army.Formation fm, bool update)
	{
		if (fm == global::Army.Formation.Auto)
		{
			fm = this.DecideFormation();
		}
		if (fm == this.cur_formation)
		{
			return;
		}
		this.InitFormation(fm);
		if (update)
		{
			this.UpdateFormation();
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x0004D118 File Offset: 0x0004B318
	private global::Army.Formation DecideFormation()
	{
		global::Army.Formation formationMode = this.FormationMode;
		if (formationMode != global::Army.Formation.Auto)
		{
			return formationMode;
		}
		if (this.logic == null)
		{
			return global::Army.Formation.Compact;
		}
		if (this.logic.battle != null)
		{
			return global::Army.Formation.Compact;
		}
		Path path = this.logic.movement.path;
		if (path == null)
		{
			return global::Army.Formation.Compact;
		}
		float num = this.thread_length;
		Logic.Army army = this.logic;
		float num2 = (((army != null) ? army.def : null) == null) ? 10f : this.logic.def.min_marching_dist;
		Logic.Army army2 = this.logic;
		float num3 = (((army2 != null) ? army2.def : null) == null) ? 2f : this.logic.def.max_unit_speed_mul;
		float num4 = (this.thread_length + 4f) / (num3 - 1f);
		if (path.path_len < num + num2 + num4)
		{
			return global::Army.Formation.Compact;
		}
		if (path.t + num4 >= path.path_len)
		{
			return global::Army.Formation.Compact;
		}
		return global::Army.Formation.Thread;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x0004D1FC File Offset: 0x0004B3FC
	public void CheckLeaderSelect()
	{
		using (Game.Profile("Army.CheckLeaderSelect", false, 0f, null))
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (kingdom != null && kingdom.id == this.kingdom.id && this.logic.leader != null && this.logic.leader.select_army_on_spawn)
			{
				this.logic.leader.select_army_on_spawn = !WorldUI.Get().SelectCourtMember(this.logic.leader);
			}
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0004D2A0 File Offset: 0x0004B4A0
	public void ResetFormation(bool force_compact = true)
	{
		this._needs_reset_formation = true;
		this._force_compact = force_compact;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0004D2B0 File Offset: 0x0004B4B0
	public void ApplyResetFormation(bool force_compact = true)
	{
		this._needs_reset_formation = false;
		using (Game.Profile("Army.ResetFormation", false, 0f, null))
		{
			if (!force_compact)
			{
				bool flag = this.cur_formation == global::Army.Formation.Thread;
			}
			this.InitFormation(global::Army.Formation.Compact);
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.units.Count; i++)
			{
				global::Unit unit = this.units[i];
				unit.SetPosition(position + this.UnitOffsets[i]);
				unit.MoveTowards(unit.instancer.Position, 0f, false);
				unit.UpdateFinalFacing();
			}
			if (!force_compact)
			{
				this.InitFormation(this.FormationMode);
				if (this.cur_formation == global::Army.Formation.Thread)
				{
					this.ResetThreadFormation();
				}
			}
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0004D394 File Offset: 0x0004B594
	public void RecalcFormation()
	{
		using (Game.Profile("Army.RecalcFormation", false, 0f, null))
		{
			this.InitFormation(this.FormationMode);
			this.UpdateFormation();
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0004D3E8 File Offset: 0x0004B5E8
	private void InitFormation(global::Army.Formation fm)
	{
		int num = this.MaxUnits;
		if (this.units.Count > num)
		{
			num = this.units.Count;
		}
		if (this.UnitOffsets == null || this.UnitOffsets.Length < num)
		{
			this.UnitOffsets = new Vector3[num];
		}
		if (this.UnitPositions == null || this.UnitPositions.Length < num)
		{
			this.UnitPositions = new Vector3[num];
			this.UnitRotations = new Quaternion[num];
		}
		if (fm == global::Army.Formation.Auto)
		{
			fm = this.DecideFormation();
		}
		this.cur_formation = fm;
		if (this.cur_formation == global::Army.Formation.Compact)
		{
			this.InitCompactFormation();
			return;
		}
		if (this.cur_formation == global::Army.Formation.Thread)
		{
			this.InitThreadFormation();
			return;
		}
		if (this.cur_formation == global::Army.Formation.Battle)
		{
			this.InitBattleFormation();
			return;
		}
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0004D4A2 File Offset: 0x0004B6A2
	private void UpdateFormation()
	{
		if (this.logic == null)
		{
			return;
		}
		this.UpdateFormation((this.logic.movement.path == null) ? 0f : this.logic.movement.path.t);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0004D4E4 File Offset: 0x0004B6E4
	private void UpdateFormation(float path_t)
	{
		global::Army.Formation formation = this.DecideFormation();
		this.SetFormation(formation, false);
		this.RotateShip(this.logic.movement.path, path_t);
		if (formation == global::Army.Formation.Compact)
		{
			this.UpdateCompactFormation(path_t);
			return;
		}
		if (formation == global::Army.Formation.Thread)
		{
			this.UpdateThreadFormation(path_t, false);
			return;
		}
		if (formation == global::Army.Formation.Battle)
		{
			this.UpdateBattleFormation(path_t);
			return;
		}
		UnityEngine.Debug.Log("Unknown Army FormationMode");
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0004D548 File Offset: 0x0004B748
	private void InitCompactFormation()
	{
		this.UnitOffsets[0] = Vector3.zero;
		if (this.units.Count <= 1)
		{
			return;
		}
		float num = 1f;
		Logic.Army army = this.logic;
		int num2 = (((army != null) ? army.def : null) == null) ? 8 : this.logic.def.marshal_max_units;
		float num3 = 6.2831855f / (float)num2;
		float num4 = (float)num2 * num / 3.1415927f;
		bool flag = true;
		int num5 = 1;
		for (;;)
		{
			for (float num6 = flag ? 0f : (num3 * 0.5f); num6 <= 3.1415927f; num6 += num3)
			{
				Vector3 vector = new Vector3(-Mathf.Sin(num6) * num4, 0f, -Mathf.Cos(num6) * num4);
				this.UnitOffsets[num5++] = vector;
				if (num5 >= this.units.Count)
				{
					return;
				}
				if (num6 > 0f && num6 < 3.1415927f)
				{
					vector.x = -vector.x;
					this.UnitOffsets[num5++] = vector;
					if (num5 >= this.units.Count)
					{
						return;
					}
				}
			}
			num4 = Mathf.Sqrt(num4 * num4 - num * num) + num * 1.732f;
			flag = !flag;
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0004D694 File Offset: 0x0004B894
	private void UpdateCompactFormation(float path_t)
	{
		Logic.Army army = this.logic;
		float num = (((army != null) ? army.def : null) == null) ? 2f : this.logic.def.max_unit_speed_mul;
		Logic.Army army2 = this.logic;
		float num2 = ((((army2 != null) ? army2.movement : null) == null) ? 1f : this.logic.movement.speed) * num;
		Path path = this.logic.movement.path;
		if (path != null && path.flee)
		{
			num2 *= 1.5f;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			Vector3 dst = base.transform.position + this.UnitOffsets[i];
			unit.MoveTowards(dst, num2, true);
		}
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0004D768 File Offset: 0x0004B968
	private void InitBattleFormation()
	{
		if (this.logic == null)
		{
			return;
		}
		this.logic.CalcBattleFormation(false);
		float num = 0f;
		for (int i = 0; i < this.units.Count; i++)
		{
			float collisionRadius = this.units[i].GetCollisionRadius();
			if (collisionRadius > num)
			{
				num = collisionRadius;
			}
		}
		Vector3 vector = base.transform.forward * (2f * num);
		Vector3 rightVector = global::Common.GetRightVector(vector, 0f);
		for (int j = 0; j < this.units.Count; j++)
		{
			global::Unit unit = this.units[j];
			Vector3 vector2 = rightVector * (float)(unit.logic.battle_col - Logic.Army.battle_cols / 2) - vector * (float)unit.logic.battle_row;
			this.UnitOffsets[j] = vector2;
		}
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0004D852 File Offset: 0x0004BA52
	private void UpdateBattleFormation(float path_t)
	{
		if (!this.logic.battle_formation_valid)
		{
			this.InitBattleFormation();
		}
		this.UpdateCompactFormation(path_t);
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x0004D870 File Offset: 0x0004BA70
	private void InitThreadFormation()
	{
		if (this.units.Count < 1)
		{
			return;
		}
		Logic.Army army = this.logic;
		this.thread_columns = ((((army != null) ? army.def : null) == null) ? 2 : this.logic.def.thread_columns);
		if (this.thread_columns < 1)
		{
			this.thread_columns = 1;
		}
		Logic.Army army2 = this.logic;
		int num = (((army2 != null) ? army2.def : null) == null) ? 8 : this.logic.def.max_thread_rows;
		for (int i = (this.units.Count - 1) / this.thread_columns; i > num; i = (this.units.Count - 1) / this.thread_columns)
		{
			this.thread_columns++;
		}
		Logic.Army army3 = this.logic;
		float num2 = (((army3 != null) ? army3.def : null) == null) ? 0f : this.logic.def.thread_spacing;
		this.UnitOffsets[0] = Vector3.zero;
		this.thread_length = this.units[0].GetCollisionRadius() + num2;
		int num3 = 1;
		for (;;)
		{
			int num4 = Mathf.Min(this.thread_columns, this.units.Count - num3);
			if (num4 <= 0)
			{
				break;
			}
			float num5 = 0f;
			float num6 = 0f;
			for (int j = 0; j < num4; j++)
			{
				float collisionRadius = this.units[num3 + j].GetCollisionRadius();
				if (collisionRadius > num6)
				{
					num6 = collisionRadius;
				}
				num5 += 2f * collisionRadius;
				if (j > 0)
				{
					num5 += num2;
				}
			}
			float num7 = -num5 / 2f;
			for (int k = 0; k < num4; k++)
			{
				float collisionRadius2 = this.units[num3 + k].GetCollisionRadius();
				num7 += collisionRadius2;
				this.UnitOffsets[num3 + k] = new Vector3(num7, 0f, this.thread_length + num6);
				num7 += collisionRadius2 + num2;
			}
			this.thread_length += 2f * num6 + num2;
			num3 += num4;
		}
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x0004DA88 File Offset: 0x0004BC88
	private void ResetThreadFormation()
	{
		Logic.Army army = this.logic;
		Path path;
		if (army == null)
		{
			path = null;
		}
		else
		{
			Movement movement = army.movement;
			path = ((movement != null) ? movement.path : null);
		}
		Path path2 = path;
		if (path2 == null)
		{
			return;
		}
		this.UpdateThreadFormation(path2.t, true);
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0004DAC8 File Offset: 0x0004BCC8
	private void UpdateThreadFormation(float path_t, bool teleport = false)
	{
		Logic.Army army = this.logic;
		Path path;
		if (army == null)
		{
			path = null;
		}
		else
		{
			Movement movement = army.movement;
			path = ((movement != null) ? movement.path : null);
		}
		Path path2 = path;
		if (path2 == null)
		{
			return;
		}
		if (this.units.Count <= 0)
		{
			return;
		}
		float num = this.logic.movement.speed;
		if (path2.flee)
		{
			num *= 1.5f;
		}
		float vmin = num * this.logic.def.min_unit_speed_mul;
		float num2 = num * this.logic.def.max_unit_speed_mul;
		if (teleport)
		{
			num2 = (vmin = float.PositiveInfinity);
		}
		if (teleport)
		{
			PPos ppos;
			PPos pt;
			path2.GetPathPoint(path_t, out ppos, out pt, false, 0f);
			this.units[0].LookAt(pt, true);
		}
		this.units[0].MoveTowards(base.transform.position, num2, !teleport);
		float num3 = 1f;
		float num4 = num * num3;
		float num5 = path2.path_len - path_t;
		if (num5 < num4)
		{
			num4 = num5;
			num3 = num4 / num;
		}
		int i = 1;
		while (i < this.units.Count)
		{
			global::Unit unit = this.units[i];
			float3 position = unit.instancer.Position;
			Vector3 vector = this.UnitOffsets[i];
			float num6 = path_t - vector.z;
			float3 float2;
			if (num6 < 0f)
			{
				int num7 = (i - 1) / this.thread_columns;
				int num8 = (i - 1) % this.thread_columns;
				int num9 = (num7 == 0) ? 0 : (1 + (num7 - 1) * this.thread_columns + num8);
				float magnitude = (this.UnitOffsets[num9] - vector).magnitude;
				float3 @float = this.units[num9].dest_pos - position;
				float num10 = math.length(@float);
				if (num10 <= magnitude)
				{
					unit.MoveTowards(position, 0f, false);
				}
				else
				{
					float2 = position + @float * (num10 - magnitude) / num10;
					if (teleport)
					{
						unit.LookAt(float2, true);
						goto IL_26D;
					}
					goto IL_26D;
				}
			}
			else
			{
				num6 += num4;
				PPos pt2;
				PPos ppos2;
				path2.GetPathPoint(num6, out pt2, out ppos2, false, vector.x);
				float2 = pt2;
				if (teleport)
				{
					float3 lhs = pt2;
					path2.GetPathPoint(num6 - num4, out pt2, out ppos2, false, vector.x);
					float2 = pt2;
					float3 v = lhs - float2;
					unit.SetFacing(v, true);
					goto IL_26D;
				}
				goto IL_26D;
			}
			IL_2C5:
			i++;
			continue;
			IL_26D:
			float2.y = position.y;
			float num11 = (num3 <= 0f) ? num2 : (math.length(float2 - position) / num3);
			if (teleport)
			{
				num11 = float.PositiveInfinity;
			}
			else
			{
				num11 = Game.clamp(num11, vmin, num2);
			}
			unit.MoveTowards(float2, num11, !teleport);
			goto IL_2C5;
		}
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0004DDB4 File Offset: 0x0004BFB4
	private void RotateShip(Path path, float t)
	{
		if (path == null || this.ship == null || !this.logic.is_in_water)
		{
			return;
		}
		PPos pt;
		PPos ppos;
		path.GetPathPoint(t + 0.1f, out pt, out ppos, false, 0f);
		Vector3 vector = pt - this.ship.transform.position;
		vector.y = 0f;
		if (vector.magnitude < 0.001f)
		{
			return;
		}
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		num = 90f - num;
		if (num < 0f)
		{
			num += 360f;
		}
		Vector3 eulerAngles = this.ship.transform.eulerAngles;
		float num2 = eulerAngles.y - num;
		if (num2 < -180f)
		{
			num2 += 360f;
		}
		else if (num2 > 180f)
		{
			num2 -= 360f;
		}
		if (num2 < -120f || num2 > 120f)
		{
			eulerAngles.y = num;
		}
		else
		{
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, num, this.logic.def.ship_rotation_speed * UnityEngine.Time.deltaTime);
		}
		this.ship.transform.eulerAngles = eulerAngles;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x0004DEFC File Offset: 0x0004C0FC
	public void PlayVoiceLine(string voiceLinePath, string soundEffectPath = null, IVars vars = null)
	{
		if (BattleMap.battle != null)
		{
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (string.IsNullOrEmpty(voiceLinePath))
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null || this.kingdom.id != kingdom.id)
		{
			return;
		}
		if (vars == null)
		{
			Logic.Army army = this.logic;
			vars = ((army != null) ? army.leader : null);
		}
		if (vars == null)
		{
			return;
		}
		BaseUI.PlayVoiceEvent(voiceLinePath, vars, base.transform.position);
		if (string.IsNullOrEmpty(soundEffectPath))
		{
			return;
		}
		EventInstance eventInstance = FMODWrapper.CreateInstance(soundEffectPath, true);
		ATTRIBUTES_3D attributes = base.transform.position.To3DAttributes();
		eventInstance.set3DAttributes(attributes);
		eventInstance.start();
		eventInstance.release();
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x0004DFB0 File Offset: 0x0004C1B0
	private void UpdateSoundLoop(bool isCamp = false)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.logic == null)
		{
			return;
		}
		if (this.logic.battle != null)
		{
			this.marching_sound_emitter.Stop();
			this.marching_sound_emitter.Event = null;
		}
		else if (this.logic.movement.path != null && !this.logic.movement.path.IsDone() && !isCamp)
		{
			if (!this.marching_sound_emitter.IsPlaying())
			{
				this.marching_sound_emitter.Event = (this.logic.is_in_water ? this.soundEffects.MarchingLoopWater : this.soundEffects.MarchingLoopLand);
				this.marching_sound_emitter.Play();
			}
		}
		else
		{
			this.marching_sound_emitter.Stop();
			this.marching_sound_emitter.Event = null;
		}
		this.SendParameters();
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x0004E08C File Offset: 0x0004C28C
	private void SendParameters()
	{
		if (this.marching_sound_emitter != null)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.units.Count; i++)
			{
				if (this.units[i].logic.def.type == Logic.Unit.Type.Cavalry || this.units[i].logic.def.type == Logic.Unit.Type.Noble || this.units[i].logic.def.secondary_type == Logic.Unit.Type.Cavalry)
				{
					num2++;
				}
				else if (this.units[i].logic.def.type == Logic.Unit.Type.Infantry || this.units[i].logic.def.type == Logic.Unit.Type.Militia || this.units[i].logic.def.type == Logic.Unit.Type.Defense || this.units[i].logic.def.type == Logic.Unit.Type.Ranged)
				{
					num++;
				}
			}
			if (num2 > 1)
			{
				num = 0;
			}
			this.marching_sound_emitter.EventInstance.setParameterByName("CavalryCount", (float)num2, false);
			this.marching_sound_emitter.EventInstance.setParameterByName("InfantryCount", (float)num, false);
		}
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x0004E1E4 File Offset: 0x0004C3E4
	private void OnEnable()
	{
		if (this.ui == null)
		{
			this.ui = WorldUI.Get();
		}
		if (this.UnitPositions == null)
		{
			this.UnitPositions = new Vector3[this.MaxUnits];
		}
		if (this.UnitRotations == null)
		{
			this.UnitRotations = new Quaternion[this.MaxUnits];
		}
		if (this.marching_sound_emitter == null)
		{
			this.marching_sound_emitter = base.gameObject.AddComponent<StudioEventEmitter>();
			this.marching_sound_emitter.StopEvent = EmitterGameEvent.ObjectDisable;
		}
		global::Common.SnapToTerrain(base.gameObject, 0f, null, -1f);
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0004E280 File Offset: 0x0004C480
	private void Start()
	{
		SphereCollider sphereCollider = base.GetComponent<SphereCollider>();
		if (sphereCollider == null)
		{
			sphereCollider = base.gameObject.AddComponent<SphereCollider>();
		}
		sphereCollider.center = Vector3.zero;
		sphereCollider.radius = 4f;
		this.visibility_index = VisibilityDetector.Add(base.transform.position, 20f, null, this, base.gameObject.layer);
		global::Common.SetRendererLayer(base.gameObject, base.gameObject.layer);
		if (this.logic.mercenary != null)
		{
			this.logic.mercenary.AddListener(this);
		}
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x0004E31C File Offset: 0x0004C51C
	private void OnDestroy()
	{
		this.marching_sound_emitter.Stop();
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			if (unit != null)
			{
				unit.SetLogic(null);
			}
		}
		this.units.Clear();
		this.DestroySelection();
		if (this.ui_pvFigure != null)
		{
			global::Common.DestroyObj(this.ui_pvFigure.gameObject);
		}
		if (this.statusBar != null)
		{
			this.statusBar.DelArmy(this);
			global::Common.DestroyObj(this.statusBar.gameObject);
		}
		if (this.banner != null)
		{
			global::Common.DestroyObj(this.banner.gameObject);
		}
		if (this.visibility_index >= 0)
		{
			VisibilityDetector.Del(this.visibility_index);
			this.visibility_index = -1;
		}
		if (this.logic != null)
		{
			Logic.Object @object = this.logic;
			this.logic = null;
			@object.Destroy(false);
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0004E410 File Offset: 0x0004C610
	public void SpawnTent(bool refresh = false)
	{
		if (!refresh && this.tent != null)
		{
			this.SnapTent();
			return;
		}
		if (refresh)
		{
			UnityEngine.Object.Destroy(this.tent);
		}
		this.tent = global::Common.FindChildByName(base.gameObject, "_tent", false, true);
		if (refresh || this.tent == null)
		{
			GameObject gameObject = null;
			Logic.Rebel rebel = this.logic.rebel;
			if (((rebel != null) ? rebel.def : null) != null)
			{
				gameObject = global::Defs.GetObj<GameObject>(this.logic.rebel.def.field, "tents", null);
			}
			else
			{
				Mercenary mercenary = this.logic.mercenary;
				if (((mercenary != null) ? mercenary.def : null) != null)
				{
					gameObject = global::Defs.GetObj<GameObject>(this.logic.mercenary.def.field, "tents", null);
				}
			}
			if (gameObject == null)
			{
				gameObject = global::Defs.GetObj<GameObject>("Army", "tents", null);
			}
			if (gameObject == null)
			{
				return;
			}
			this.tent = global::Common.Spawn(gameObject, false, false);
			this.tent.name = "_tent";
			this.tent.transform.SetParent(base.transform, false);
			this.tent.transform.localPosition = Vector3.zero;
			global::Common.SetRendererLayer(this.tent, base.gameObject.layer);
			this.SnapTent();
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0004E574 File Offset: 0x0004C774
	private void SnapTent()
	{
		if (this.tent == null)
		{
			return;
		}
		for (int i = 0; i < this.tent.transform.childCount; i++)
		{
			global::Common.SnapToTerrain(this.tent.transform.GetChild(i), 0f, null, -1f);
		}
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0004E5CC File Offset: 0x0004C7CC
	public int CalcShipLevel()
	{
		if (this.ship_field == null)
		{
			return -1;
		}
		string key = this.ship_field.key;
		if (key == "rebel" || key == "mercenary" || key == "crusader")
		{
			return UnityEngine.Random.Range(0, this.ship_field.parent.GetInt("max_ship_level", null, 0, true, true, true, '.'));
		}
		int a = (int)this.logic.GetKingdom().GetStat(Stats.ks_ships_level, true);
		int b = 0;
		if (this.logic.leader != null)
		{
			b = (int)this.logic.leader.GetStat(Stats.cs_ships_level, true);
		}
		return Mathf.Max(a, b);
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0004E680 File Offset: 0x0004C880
	public void GetShipField()
	{
		DT.Field defField = global::Defs.GetDefField("culture_models", "ship_models");
		if (defField == null)
		{
			return;
		}
		string text = "";
		if (this.logic.rebel != null)
		{
			text = "rebel";
		}
		else if (this.logic.mercenary != null)
		{
			text = "mercenary";
		}
		else if (this.logic.leader != null && this.logic.leader.IsCrusader())
		{
			text = "crusader";
		}
		else
		{
			DT.Field defField2 = global::Defs.GetDefField("culture_models", "models_per_culture");
			Logic.Kingdom kingdom = this.logic.GetKingdom();
			if (defField2 != null)
			{
				text = defField2.GetString(kingdom.culture, null, "", true, true, true, '.');
				if (string.IsNullOrEmpty(text))
				{
					text = defField2.GetString(kingdom.game.cultures.GetGroup(kingdom.culture) ?? "", null, "", true, true, true, '.');
				}
			}
		}
		this.ship_field = defField.FindChild(text, null, true, true, true, '.');
		if (this.ship_field == null)
		{
			this.ship_field = defField.FindChild("european", null, true, true, true, '.');
		}
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x0004E7A0 File Offset: 0x0004C9A0
	public void SpawnShip(bool refresh = false)
	{
		int num = this.CalcShipLevel();
		if (num != this.ship_level)
		{
			this.ship_level = num;
			refresh = true;
		}
		if (!refresh && this.ship != null)
		{
			return;
		}
		if (refresh)
		{
			Animator animator = this.ship;
			UnityEngine.Object.Destroy((animator != null) ? animator.gameObject : null);
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "_ship", false, true);
		this.ship = ((gameObject != null) ? gameObject.GetComponent<Animator>() : null);
		if (refresh || this.ship == null)
		{
			this.GetShipField();
			if (num == -1)
			{
				num = this.CalcShipLevel();
			}
			DT.Field field = this.ship_field;
			GameObject obj = global::Defs.GetObj<GameObject>((field != null) ? field.FindChild((num + 1).ToString(), null, true, true, true, '.') : null, null);
			if (obj == null)
			{
				return;
			}
			GameObject gameObject2 = global::Common.Spawn(obj, false, false);
			this.ship = ((gameObject2 != null) ? gameObject2.GetComponent<Animator>() : null);
			MeshRenderer componentInChildren = this.ship.GetComponentInChildren<MeshRenderer>();
			this.ship.gameObject.SetLayer(LayerMask.NameToLayer("Ships"), true);
			if (componentInChildren != null)
			{
				Color value = Color.white;
				Color value2 = Color.white;
				global::Kingdom kingdom = global::Kingdom.Get(this.kingdom.id);
				if (kingdom != null)
				{
					value = kingdom.PrimaryArmyColor;
					value2 = kingdom.SecondaryArmyColor;
				}
				componentInChildren.material.SetColor("_Color1", value);
				componentInChildren.material.SetColor("_Color2", value2);
			}
			this.ship.name = "_ship";
			this.ship.transform.SetParent(base.transform, false);
			this.ship.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0004E950 File Offset: 0x0004CB50
	public void SpawnShipCannons(bool refresh = false)
	{
		if (!refresh && this.ship_cannons != null)
		{
			return;
		}
		this.SpawnShip(false);
		if (this.ship == null)
		{
			return;
		}
		if (refresh)
		{
			UnityEngine.Object.Destroy(this.ship_cannons);
		}
		this.ship_cannons = global::Common.FindChildByName(base.gameObject, "_ship_cannons", false, true);
		if (refresh || this.ship_cannons == null)
		{
			DT.Field defField = global::Defs.GetDefField("culture_models", "cannon_models");
			GameObject gameObject = null;
			if (this.logic.rebel != null)
			{
				gameObject = global::Defs.GetObj<GameObject>(defField, "rebel", null);
			}
			else if (this.logic.mercenary != null)
			{
				gameObject = global::Defs.GetObj<GameObject>(defField, "mercenary", null);
			}
			if (gameObject == null)
			{
				gameObject = global::Defs.GetObj<GameObject>(defField, "army", null);
			}
			if (gameObject == null)
			{
				return;
			}
			this.ship_cannons = global::Common.Spawn(gameObject, false, false);
			this.ship_cannons.name = "_ship_cannons";
			this.ship_cannons.transform.SetParent(base.transform, false);
			this.ship_cannons.transform.localPosition = Vector3.zero;
			global::Common.SetRendererLayer(this.ship.gameObject, base.gameObject.layer);
		}
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0004EA8C File Offset: 0x0004CC8C
	public void SpawnTrebuchet()
	{
		if (this.tebuchet == null)
		{
			this.tebuchet = global::Common.FindChildByName(base.gameObject, "_trebuchet", false, true);
			if (this.tebuchet == null)
			{
				GameObject obj = global::Defs.GetObj<GameObject>("Army", "trebuchet", null);
				if (obj == null)
				{
					return;
				}
				this.tebuchet = global::Common.Spawn(obj, false, false);
				this.tebuchet.name = "_trebuchet";
				this.tebuchet.transform.SetParent(base.transform, false);
				global::Common.SetRendererLayer(this.tent, base.gameObject.layer);
			}
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0004EB36 File Offset: 0x0004CD36
	public void UpdatePVFigureVisiblity(bool visible)
	{
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateVisibilityFromObject(visible);
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x0004EB54 File Offset: 0x0004CD54
	public void UpdateStatusBar()
	{
		using (Game.Profile("Army.UpdateStatusBar", false, 0f, null))
		{
			if (this.logic != null)
			{
				if (this.logic.IsValid())
				{
					if (this.statusBar == null)
					{
						GameObject prefab = global::Army.StatusBarPrefab();
						WorldUI worldUI = WorldUI.Get();
						GameObject gameObject = global::Common.Spawn(prefab, (worldUI != null) ? worldUI.m_statusBar : null, false, "");
						this.statusBar = ((gameObject != null) ? gameObject.GetComponent<UIArmyStatusBar>() : null);
						if (this.statusBar != null)
						{
							this.statusBar.AddArmy(this);
							this.statusBar.SetBattle(null);
							this.UpdateStatusBarVisiblity(this.logicVisible && this.logic.battle == null);
							this.statusBar.Refresh();
							this.statusBar.transform.position = new Vector3(-1000f, 0f, -1000f);
						}
					}
					else
					{
						this.statusBar.RefreshListeners();
					}
				}
			}
		}
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x0004EC78 File Offset: 0x0004CE78
	public void RegisterToMinimap()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		MinimapArmyDisplay minimap_army_display = baseUI.minimap_army_display;
		if (minimap_army_display == null)
		{
			return;
		}
		minimap_army_display.RegisterArmy(this);
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x0004EC94 File Offset: 0x0004CE94
	public void UnregisterToMinimap()
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI == null)
		{
			return;
		}
		MinimapArmyDisplay minimap_army_display = baseUI.minimap_army_display;
		if (minimap_army_display == null)
		{
			return;
		}
		minimap_army_display.UnregisterArmy(this);
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x0004ECB0 File Offset: 0x0004CEB0
	public void UpdateStatusBarVisiblity()
	{
		this.UpdateStatusBarVisiblity(this.logicVisible && this.logic.battle == null);
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0004ECD4 File Offset: 0x0004CED4
	public void UpdateStatusBarVisiblity(bool visible)
	{
		if (this.statusBar == null)
		{
			return;
		}
		this.statusBar.UpdateVisibilityFromObject(visible);
		if (ViewMode.IsPoliticalView())
		{
			this.statusBar.UpdateVisibilityFromView(ViewMode.current.allowedFigures);
			return;
		}
		this.statusBar.UpdateVisibilityFromView((ViewMode.AllowedFigures)(-1));
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x0004ED25 File Offset: 0x0004CF25
	public void UpdatePVFigureVisiblity(ViewMode.AllowedFigures allowedFigures)
	{
		UIPVFigureArmy uipvfigureArmy = this.ui_pvFigure;
		if (uipvfigureArmy == null)
		{
			return;
		}
		uipvfigureArmy.UpdateVisibilityFromView(allowedFigures);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0004ED38 File Offset: 0x0004CF38
	private void UpdateMinimapIconVisibility()
	{
		bool flag = false;
		if (this.logic == null)
		{
			this.UnregisterToMinimap();
			return;
		}
		if (this.ui == null)
		{
			this.UnregisterToMinimap();
			return;
		}
		Castle castle = this.logic.castle;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		Logic.Army army = this.logic;
		if (army != null)
		{
			bool fow = army.game.fow;
		}
		if (castle != null)
		{
			flag = (castle.GetKingdom() == kingdom && this.logic.kingdom_id == kingdom.id);
		}
		else if (this.realm_in != null)
		{
			flag = (this.realm_in.CalcVisibleBy(kingdom, true) > 0);
		}
		if (this.logic.battle != null)
		{
			flag = false;
		}
		if (flag)
		{
			this.RegisterToMinimap();
			return;
		}
		this.UnregisterToMinimap();
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x0004EDF4 File Offset: 0x0004CFF4
	public bool CanBeSelected()
	{
		Logic.Army army = this.logic;
		if (((army != null) ? army.game : null) == null)
		{
			return false;
		}
		Logic.Kingdom k = BaseUI.LogicKingdom();
		Logic.Realm realm = this.logic.realm_in;
		return realm == null || realm.CalcVisibleBy(k, true) > 0;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x0004EE3C File Offset: 0x0004D03C
	public void UpdateVisibility(bool force_children_refresh = false)
	{
		bool flag = this.logicVisible;
		this.logicVisible = true;
		if (this.logic != null)
		{
			if (this.logic.castle != null)
			{
				this.logicVisible = false;
			}
			else if (this.ui != null && this.logic.game != null && this.logic.game.fow)
			{
				Logic.Kingdom k = BaseUI.LogicKingdom();
				if (this.realm_in != null && this.realm_in.CalcVisibleBy(k, true) <= 0)
				{
					this.logicVisible = false;
				}
			}
		}
		Logic.Object selected_logic_obj = BaseUI.Get().selected_logic_obj;
		if (selected_logic_obj != null && (selected_logic_obj == this.logic || selected_logic_obj == this.logic.mercenary || selected_logic_obj == this.logic.rebel))
		{
			GameObject selectionObj = BaseUI.Get().GetSelectionObj(base.gameObject);
			if (selectionObj != BaseUI.Get().selected_obj)
			{
				BaseUI.Get().SelectObj(selectionObj, false, true, true, true);
			}
		}
		this.UpdateMinimapIconVisibility();
		if (this.ui_pvFigure != null)
		{
			this.ui_pvFigure.UpdateVisibilityFilter();
		}
		this.UpdatePVFigureVisiblity(this.logicVisible);
		this.UpdateStatusBarVisiblity(this.logicVisible && (this.inCameraView || ViewMode.IsPoliticalView()) && this.logic.battle == null);
		this.logicVisible &= this.inCameraView;
		if (this.logicVisible)
		{
			this.UpdateSelection();
			if (!flag)
			{
				if (this.logic.battle != null && global::Army.move_units_in_battle)
				{
					this.MoveUnitsBattle(true);
				}
				else
				{
					this.ResetFormation(false);
				}
			}
		}
		if (base.gameObject.activeSelf != this.logicVisible)
		{
			base.gameObject.SetActive(this.logicVisible);
		}
		this.UpdateChildrenVisibility(flag, force_children_refresh);
		this.UpdateBanner(false);
		this.UpdateUnitFlags(false);
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0004F007 File Offset: 0x0004D207
	public void UpdateChildrenVisibility(bool was_visible = true, bool reset_units = false)
	{
		this._needs_update_children_visibility = true;
		this._was_visible = was_visible;
		this._reset_units = reset_units;
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x0004F020 File Offset: 0x0004D220
	public static bool ShipsVisible(Logic.Army army)
	{
		bool result = !army.currently_on_land;
		if (army.water_crossing.running)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x0004F048 File Offset: 0x0004D248
	public void ApplyUpdateChildrenVisibility(bool was_visible = true, bool reset_units = false)
	{
		this._needs_update_children_visibility = false;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = this.logic.castle == null;
		bool flag4 = global::Army.ShipsVisible(this.logic);
		bool flag5 = false;
		if (this.logic.isCamping)
		{
			flag = true;
		}
		if (this.logic != null && this.logic.battle != null)
		{
			flag3 = false;
			flag = global::Battle.TentsVisible(this.logic.battle);
			if (!flag4)
			{
				if (this.logic.battle.type == Logic.Battle.Type.Siege && this.logic.battle.attackers.Contains(this.logic))
				{
					flag2 = true;
				}
			}
			else
			{
				flag5 = true;
			}
		}
		if (this.logic.water_crossing.running)
		{
			flag = (!this.logic.water_crossing.is_fast || this.logic.water_crossing.teleport);
			flag4 = false;
		}
		else if (flag4)
		{
			flag = false;
		}
		if (flag)
		{
			this.SpawnTent(false);
		}
		if (flag2)
		{
			this.SpawnTrebuchet();
		}
		if (flag4)
		{
			this.SpawnShip(false);
		}
		if (flag5)
		{
			this.SpawnShipCannons(false);
		}
		bool flag6 = !flag && !flag4;
		if (this.logic != null && this.logic.castle != null)
		{
			flag6 = false;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			unit.Enable(this.logicVisible && flag6, false);
			unit.UpdateAnimation(was_visible);
		}
		if (this.tent != null)
		{
			this.tent.gameObject.SetActive(this.logicVisible && flag && !flag2);
		}
		if (this.tebuchet != null)
		{
			this.tebuchet.gameObject.SetActive(this.logicVisible && flag2);
		}
		if (this.banner != null)
		{
			this.banner.gameObject.SetActive(this.logicVisible && flag3);
		}
		if (this.relation != null)
		{
			this.relation.gameObject.SetActive(this.logicVisible && this.logic.castle == null);
		}
		if (this.ship != null)
		{
			this.ship.gameObject.SetActive(this.logicVisible && flag4);
		}
		if (this.ship_cannons != null)
		{
			this.ship_cannons.gameObject.SetActive(this.logicVisible && flag5);
		}
		this.UpdateHealParticles();
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x0004F2BE File Offset: 0x0004D4BE
	public void VisibilityChanged(bool visible)
	{
		this.inCameraView = visible;
		this.UpdateVisibility(false);
		this.UpdateSoundLoop(false);
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000023FD File Offset: 0x000005FD
	private void UpdateMinimapIconVisibility(bool visible)
	{
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0004F2D5 File Offset: 0x0004D4D5
	public void AddUnit(string def_id, bool recalc_formation = true, bool send_state = false)
	{
		this.logic.AddUnit(def_id, -1, false, true);
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0004F2E8 File Offset: 0x0004D4E8
	public void AddUnit(Logic.Unit lu = null, bool recalc_formation = true, bool send_state = false)
	{
		using (Game.Profile("Army.AddUnit(Logic.Unit)", false, 0f, null))
		{
			WorldMap worldMap = WorldMap.Get();
			global::Unit u = new global::Unit((worldMap != null) ? worldMap.texture_baker : null, GameLogic.instance.transform);
			this.AddUnit(u, lu, send_state);
			if (recalc_formation)
			{
				this.RecalcFormation();
			}
			this.UpdateChildrenVisibility(true, false);
			this.UpdateBanner(false);
			this.UpdateUnitFlags(false);
		}
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0004F370 File Offset: 0x0004D570
	public void AddUnit(global::Unit u, Logic.Unit lu = null, bool send_state = false)
	{
		using (Game.Profile("Army.AddUnit(Unit)", false, 0f, null))
		{
			u.kingdom = this.kingdom;
			bool flag = false;
			if (lu == null && this.logic != null)
			{
				flag = true;
				lu = new Logic.Unit();
				lu.def = this.logic.game.defs.Get<Logic.Unit.Def>(u.type);
				if (!lu.def.valid)
				{
					UnityEngine.Debug.LogError("Unknown unit type: " + u.type);
				}
			}
			if (lu != null)
			{
				u.SetLogic(lu);
				u.can_move = true;
				if (flag)
				{
					this.logic.AddUnit(lu, -1, send_state);
				}
			}
			this.units.Add(u);
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0004F444 File Offset: 0x0004D644
	public void DelUnit(global::Unit u)
	{
		if (!this.units.Remove(u))
		{
			return;
		}
		if (this.logic != null && u.logic != null)
		{
			this.logic.DelUnit(u.logic, true);
		}
		u.SetLogic(null);
		this.RecalcFormation();
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x0004F484 File Offset: 0x0004D684
	public void ClearUnits()
	{
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			if (this.logic != null && unit.logic != null)
			{
				this.logic.DelUnit(unit.logic, true);
			}
			unit.logic = null;
		}
		this.units.Clear();
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0004F4E8 File Offset: 0x0004D6E8
	public global::Unit GetUnit(Logic.Unit lu)
	{
		if (lu == null)
		{
			return null;
		}
		foreach (global::Unit unit in this.units)
		{
			if (unit.logic == lu)
			{
				return unit;
			}
		}
		return null;
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x0004F54C File Offset: 0x0004D74C
	public global::Unit GetUnitAtBattlePos(int row, int col)
	{
		foreach (global::Unit unit in this.units)
		{
			if (unit.logic != null && unit.logic.battle_row == row && unit.logic.battle_col == col)
			{
				return unit;
			}
		}
		return null;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0004F5C4 File Offset: 0x0004D7C4
	private void OnValidate()
	{
		if (this.units == null || this.units.Count == 0)
		{
			return;
		}
		if (Application.isPlaying)
		{
			this.RecalcFormation();
			return;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			this.units[i].UpdateColors();
		}
		this.ResetFormation(true);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0004F624 File Offset: 0x0004D824
	private void Moved()
	{
		if (this.logic == null)
		{
			return;
		}
		this.Moved(this.logic.position, (this.logic.movement.path == null) ? 0f : this.logic.movement.path.t);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0004F67C File Offset: 0x0004D87C
	private void Moved(PPos pt, float path_t)
	{
		Vector3 vector = global::Common.SnapToTerrain(pt, 0f, WorldMap.GetTerrain(), -1f, false);
		base.transform.position = vector;
		if (this.visibility_index >= 0)
		{
			VisibilityDetector.Move(this.visibility_index, vector, -1f);
		}
		this.FaceBattle();
		if (this.logicVisible && !this.logic.is_in_water)
		{
			if (this.logic.movement.path != null && !this.logic.water_crossing.running)
			{
				Vector3 vector2 = (this.logic.movement.path.dst_obj != null) ? this.logic.movement.path.dst_obj.position : this.logic.movement.path.dst_pt;
				vector2.y = vector.y;
				if (vector2 != vector)
				{
					base.transform.LookAt(vector2);
				}
			}
			this.UpdateSelection();
		}
		if (this.logicVisible)
		{
			this.UpdateFormation(path_t);
		}
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x0004F798 File Offset: 0x0004D998
	private void Update()
	{
		if (this._needs_reset_formation)
		{
			this.ApplyResetFormation(this._force_compact);
		}
		if (this._needs_update_children_visibility)
		{
			this.ApplyUpdateChildrenVisibility(this._was_visible, this._reset_units);
		}
		this.UpdateUnits();
		this.UpdateUnitFlagPosRot();
		if (UnityEngine.Time.deltaTime == 0f)
		{
			return;
		}
		if (this.logic.movement.path != null)
		{
			PPos pt;
			float path_t;
			this.logic.movement.CalcPosition(out pt, out path_t);
			this.Moved(pt, path_t);
			if (this.ship != null)
			{
				this.ship.SetBool("moving", true);
			}
		}
		else if (this.ship != null)
		{
			this.ship.SetBool("moving", false);
		}
		if (this.logic.battle != null)
		{
			this.MoveUnitsBattle(false);
		}
		this.SendParameters();
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x0004F874 File Offset: 0x0004DA74
	private void MoveUnitsBattle(bool instant = false)
	{
		if (!global::Army.move_units_in_battle)
		{
			return;
		}
		for (int i = 0; i < this.units.Count; i++)
		{
			global::Unit unit = this.units[i];
			if (unit.logic.simulation != null)
			{
				if (unit.logic.simulation.state == BattleSimulation.Squad.State.Idle || unit.logic.simulation.state == BattleSimulation.Squad.State.Dead || unit.logic.simulation.state == BattleSimulation.Squad.State.Fled || instant)
				{
					unit.dest_pos = Vector3.zero;
					unit.dest_speed = 0f;
					if (instant || unit.logic.simulation.state == BattleSimulation.Squad.State.Idle)
					{
						unit.SetPosition(unit.logic.simulation.world_pos);
					}
					else
					{
						unit.SetPosition(unit.instancer.Position);
					}
				}
				else
				{
					unit.MoveTowards(unit.logic.simulation.world_tgt_pos, unit.logic.def.move_speed * unit.logic.simulation.mod_move_speed, true);
				}
			}
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x0004F9A4 File Offset: 0x0004DBA4
	private void UpdateUnits()
	{
		for (int i = 0; i < this.units.Count; i++)
		{
			this.units[i].Update();
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0004F9D8 File Offset: 0x0004DBD8
	public bool HasTransferTarget(out Mercenary merc)
	{
		merc = null;
		if (this.logic == null)
		{
			return false;
		}
		if (this.logic.interactor != null)
		{
			return true;
		}
		if (this.logic.interact_target != null)
		{
			return true;
		}
		Logic.Kingdom factionKingdom = FactionUtils.GetFactionKingdom(this.logic.game, "MercenaryFaction");
		for (int i = 0; i < factionKingdom.armies.Count; i++)
		{
			Mercenary mercenary = factionKingdom.armies[i].mercenary;
			if (mercenary != null && mercenary.IsValid() && mercenary.buyers.Contains(this.logic))
			{
				merc = mercenary;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x0004FA74 File Offset: 0x0004DC74
	private void OnDrawGizmos()
	{
		if (this.logic == null)
		{
			return;
		}
		Vector3 center = global::Common.SnapToTerrain(this.logic.position, 0.25f, WorldMap.GetTerrain(), -1f, false);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(center, 0.5f);
		Vector3 b = 0.1f * Vector3.up;
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position + b, 0.25f);
		Path path = this.logic.movement.path;
		if (this.logic.battle != null && !this.logic.battle.is_siege)
		{
			Gizmos.color = ((this.logic.battle_side % 2 == 0) ? Color.blue : Color.red);
			for (int i = 0; i < this.units.Count; i++)
			{
				global::Unit unit = this.units[i];
				BattleSimulation.Squad squad;
				if (unit == null)
				{
					squad = null;
				}
				else
				{
					Logic.Unit unit2 = unit.logic;
					squad = ((unit2 != null) ? unit2.simulation : null);
				}
				BattleSimulation.Squad squad2 = squad;
				if (squad2 != null)
				{
					Gizmos.DrawLine(global::Common.SnapToTerrain(squad2.world_pos, 0f, null, -1f, false), global::Common.SnapToTerrain(squad2.world_tgt_pos, 0f, null, -1f, false));
				}
			}
		}
		if (path == null || path.segments == null)
		{
			return;
		}
		int num = 0;
		while (num + 1 < path.segments.Count)
		{
			Gizmos.color = ((num % 2 == 0) ? Color.blue : Color.red);
			Vector3 vector = path.segments[num].pt;
			vector.y = global::Common.GetTerrainHeight(vector, null, false);
			Vector3 vector2 = path.segments[num + 1].pt;
			vector2.y = global::Common.GetTerrainHeight(vector2, null, false);
			Gizmos.DrawLine(vector + b, vector2 + b);
			num++;
		}
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x0004FC77 File Offset: 0x0004DE77
	public int GetKingdomID()
	{
		if (this.logic == null)
		{
			return this.kingdom;
		}
		return this.logic.kingdom_id;
	}

	// Token: 0x0400060A RID: 1546
	public global::Kingdom.ID kingdom = 0;

	// Token: 0x0400060B RID: 1547
	public global::Army.Formation FormationMode;

	// Token: 0x0400060C RID: 1548
	private global::Army.Formation cur_formation;

	// Token: 0x0400060D RID: 1549
	public const float Radius = 4f;

	// Token: 0x0400060E RID: 1550
	public const float PickingRadius = 4f;

	// Token: 0x0400060F RID: 1551
	private int thread_columns;

	// Token: 0x04000610 RID: 1552
	private float thread_length;

	// Token: 0x04000611 RID: 1553
	private WorldUI ui;

	// Token: 0x04000612 RID: 1554
	[NonSerialized]
	public List<global::Unit> units = new List<global::Unit>();

	// Token: 0x04000613 RID: 1555
	public Billboard banner;

	// Token: 0x04000614 RID: 1556
	public GameObject[] unit_flags = new GameObject[9];

	// Token: 0x04000615 RID: 1557
	public static readonly int[] unit_flag_ids = new int[]
	{
		1,
		3,
		5
	};

	// Token: 0x04000616 RID: 1558
	public UIPVFigureArmy ui_pvFigure;

	// Token: 0x04000617 RID: 1559
	public UIArmyStatusBar statusBar;

	// Token: 0x04000618 RID: 1560
	private GameObject tent;

	// Token: 0x04000619 RID: 1561
	private GameObject tebuchet;

	// Token: 0x0400061A RID: 1562
	private ParticleSystem[] heal_particles;

	// Token: 0x0400061B RID: 1563
	public Animator ship;

	// Token: 0x0400061C RID: 1564
	private GameObject ship_cannons;

	// Token: 0x0400061D RID: 1565
	private DT.Field ship_field;

	// Token: 0x0400061E RID: 1566
	private int ship_level = -1;

	// Token: 0x0400061F RID: 1567
	private Vector3[] UnitOffsets;

	// Token: 0x04000620 RID: 1568
	private Vector3[] UnitPositions;

	// Token: 0x04000621 RID: 1569
	private Quaternion[] UnitRotations;

	// Token: 0x04000622 RID: 1570
	private bool selected;

	// Token: 0x04000623 RID: 1571
	private bool primarySelection;

	// Token: 0x04000624 RID: 1572
	private MeshRenderer selection;

	// Token: 0x04000625 RID: 1573
	private MeshRenderer relation;

	// Token: 0x04000626 RID: 1574
	private int visibility_index = -1;

	// Token: 0x04000627 RID: 1575
	private bool logicVisible = true;

	// Token: 0x04000628 RID: 1576
	private bool inCameraView = true;

	// Token: 0x04000629 RID: 1577
	private bool exitingCasle;

	// Token: 0x0400062A RID: 1578
	private bool _needs_reset_formation;

	// Token: 0x0400062B RID: 1579
	private bool _force_compact;

	// Token: 0x0400062C RID: 1580
	private bool _needs_update_children_visibility;

	// Token: 0x0400062D RID: 1581
	private bool _was_visible;

	// Token: 0x0400062E RID: 1582
	private bool _reset_units;

	// Token: 0x0400062F RID: 1583
	private PathArrows path_arrows;

	// Token: 0x04000630 RID: 1584
	public global::Army.SoundEffects soundEffects;

	// Token: 0x04000631 RID: 1585
	public Logic.Army logic;

	// Token: 0x04000632 RID: 1586
	public Logic.Realm realm_in;

	// Token: 0x04000633 RID: 1587
	private StudioEventEmitter marching_sound_emitter;

	// Token: 0x04000634 RID: 1588
	public static bool move_units_in_battle = true;

	// Token: 0x02000595 RID: 1429
	public enum Formation
	{
		// Token: 0x040030E5 RID: 12517
		Auto,
		// Token: 0x040030E6 RID: 12518
		Compact,
		// Token: 0x040030E7 RID: 12519
		Thread,
		// Token: 0x040030E8 RID: 12520
		Battle
	}

	// Token: 0x02000596 RID: 1430
	[Serializable]
	public struct SoundEffects
	{
		// Token: 0x040030E9 RID: 12521
		[EventRef]
		public string MarchingLoopLand;

		// Token: 0x040030EA RID: 12522
		[EventRef]
		public string MarchingLoopWater;

		// Token: 0x040030EB RID: 12523
		[EventRef]
		public string Attack;

		// Token: 0x040030EC RID: 12524
		[EventRef]
		public string MoveLand;

		// Token: 0x040030ED RID: 12525
		[EventRef]
		public string MoveWater;

		// Token: 0x040030EE RID: 12526
		[EventRef]
		public string Retreat;
	}
}
