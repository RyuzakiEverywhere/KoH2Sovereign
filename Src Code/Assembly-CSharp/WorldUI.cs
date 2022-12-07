using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class WorldUI : BaseUI
{
	// Token: 0x17000263 RID: 611
	// (get) Token: 0x060030FE RID: 12542 RVA: 0x0018BA65 File Offset: 0x00189C65
	// (set) Token: 0x060030FF RID: 12543 RVA: 0x0018BA6D File Offset: 0x00189C6D
	public bool m_InvalidateUpdateKingdom { get; private set; }

	// Token: 0x06003100 RID: 12544 RVA: 0x0018BA78 File Offset: 0x00189C78
	public static void FillWorstRealmVars(Vars worst_realm_vars)
	{
		if (worst_realm_vars == null)
		{
			worst_realm_vars = new Vars();
		}
		for (int i = 0; i < 4; i++)
		{
			worst_realm_vars.Del("worst_realm_" + i);
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null && kingdom.realms.Count > 0)
		{
			List<Logic.Realm> list = new List<Logic.Realm>(kingdom.realms);
			list.Sort((Logic.Realm x, Logic.Realm y) => x.GetTotalRebellionRisk().CompareTo(y.GetTotalRebellionRisk()));
			for (int j = 0; j < Mathf.Min(list.Count, 3); j++)
			{
				Logic.Realm realm = list[j];
				if (realm.GetTotalRebellionRisk() <= kingdom.stability.def.stability_tooltip_threshold)
				{
					worst_realm_vars.Set<Logic.Realm>("worst_realm_" + j, realm);
				}
			}
		}
	}

	// Token: 0x06003101 RID: 12545 RVA: 0x0018BB50 File Offset: 0x00189D50
	protected override void OnEnable()
	{
		base.OnEnable();
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			Shader.SetGlobalVector("_WVBattlePos", Vector4.zero);
			worldMap.SetHighlighedRealm(null);
			worldMap.SetSelectedRealm(0);
		}
		AttributeConsoleManager instance = AttributeConsoleManager.instance;
		if (instance != null && instance.GetComponent<DevCheats>() == null)
		{
			instance.gameObject.AddComponent<DevCheats>();
		}
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x0018BBB8 File Offset: 0x00189DB8
	protected override void OnDisable()
	{
		base.OnDisable();
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.SetHighlighedRealm(null);
			worldMap.SetSelectedRealm(0);
		}
		GameSpeed.SupressSpeedChangesByPlayer = false;
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x0018BBEE File Offset: 0x00189DEE
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x0018BBF6 File Offset: 0x00189DF6
	public new static WorldUI Get()
	{
		return BaseUI.Get<WorldUI>();
	}

	// Token: 0x06003105 RID: 12549 RVA: 0x0018BC00 File Offset: 0x00189E00
	protected override void Start()
	{
		base.Start();
		this.minimap = global::Common.FindChildComponent<MiniMap>(base.gameObject, "id_Minimap");
		this.minimapObject = global::Common.FindChildByName(base.gameObject, "id_Minimap", true, true);
		this.menu = global::Common.FindChildByName(this.tCanvas.gameObject, "id_Menu", false, false);
		this.royalCourt = base.GetComponentInChildren<UIRoyalCourt>();
		this.minimapOverlay = base.gameObject.GetComponentInChildren<UIMinimapOverlay>();
		this.logger = global::Common.FindChildComponent<UILogger>(base.gameObject, "id_EventLogger");
		this.messageWindow = global::Common.FindChildByName(base.gameObject, "id_MessageIcons", true, true);
		this.playerRemidner = base.gameObject.AddComponent<UIPlayerReminder>();
		this.started = true;
		Tutorial.Start();
		if (this.load_from != null)
		{
			this.Load(this.load_from);
			this.load_from = null;
			return;
		}
		this.UpdateKingdom(false);
	}

	// Token: 0x06003106 RID: 12550 RVA: 0x0018BCEC File Offset: 0x00189EEC
	public IconsBar GetMessageIcons()
	{
		if (this.m_MessageIcons == null)
		{
			this.m_MessageIcons = global::Common.FindChildComponent<IconsBar>(base.gameObject, "id_MessageIcons");
			if (this.m_MessageIcons != null)
			{
				this.m_MessageIcons.stack_same_frame_messages = true;
			}
		}
		return this.m_MessageIcons;
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x0018BD3D File Offset: 0x00189F3D
	public IconsBar GetOngoingIcons()
	{
		if (this.m_OngoingIcons == null)
		{
			this.m_OngoingIcons = global::Common.FindChildComponent<IconsBar>(base.gameObject, "id_OngoingIcons");
		}
		return this.m_OngoingIcons;
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x0018BD69 File Offset: 0x00189F69
	public IconsBar GetIOIcons()
	{
		if (this.m_IOIcons == null)
		{
			this.m_IOIcons = global::Common.FindChildComponent<IconsBar>(base.gameObject, "id_IOIcons");
		}
		return this.m_IOIcons;
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x0018BD95 File Offset: 0x00189F95
	public UILogger GetEventLogger()
	{
		if (this.m_EventLogger == null)
		{
			this.m_EventLogger = global::Common.FindChildComponent<UILogger>(base.gameObject, "id_EventLogger");
		}
		return this.m_EventLogger;
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x0018BDC4 File Offset: 0x00189FC4
	public void Save(DT.Field f)
	{
		f.SetValue("kingdom", BaseUI.LogicKingdom().ToDTValue());
		f.SetValue("cam_pos", base.ptLookAt.ToString(), null);
		Logic.Object @object = this.selected_logic_obj;
		if (@object != null && @object.visuals == null)
		{
			GameLogic.Behaviour component = this.selected_obj.GetComponent<GameLogic.Behaviour>();
			if (component != null)
			{
				@object = component.GetLogic();
			}
		}
		f.SetValue("selected", @object.ToDTValue());
		this.minimap.Save(f);
		Tutorial.Save(f);
		Voices.Save(f);
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x0018BE78 File Offset: 0x0018A078
	public void Load(DT.Field f)
	{
		if (!this.started)
		{
			this.load_from = f;
			return;
		}
		Game game = GameLogic.Get(true);
		WorldMap.Get();
		this.ClearSelection();
		Point point = f.GetPoint("cam_pos", null, true, true, true, '.');
		NID nid = NID.FromDTValue(f.GetValue("selected", null, true, true, true, '.'));
		ViewMode.WorldView.Apply(true);
		this.selected_obj = null;
		this.selected_logic_obj = nid.GetObj(game);
		if (point != Point.Invalid)
		{
			base.LookAt(point, false);
			WorldUI.cam_pos_loaded = true;
		}
		this.minimap.Load(f);
		Tutorial.Load(f);
		this.Refresh();
		this.InvalidateKingdom();
		LabelUpdater.Get(true).GenerateLabels(false);
		Voices.Load(f);
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x0018BF41 File Offset: 0x0018A141
	public void OnJoinGame()
	{
		this.ClearSelection();
		this.InvalidateKingdom();
		this.Refresh();
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x0018BF58 File Offset: 0x0018A158
	public void LookAtCapital(bool all_cameras = false)
	{
		global::Kingdom kingdom = global::Kingdom.Get(base.kingdom);
		if (kingdom != null && kingdom.logic != null)
		{
			Logic.Realm capital = kingdom.logic.GetCapital();
			if (capital != null && capital.castle != null)
			{
				base.LookAt(capital.castle.position, all_cameras);
			}
		}
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x0018BFB0 File Offset: 0x0018A1B0
	private void ClearSelection()
	{
		this.select_target = null;
		this.selected_orig = null;
		this.selected_obj = null;
		this.selected_objects.Clear();
		this.selected_logic_obj = null;
		this.selected_logic_objects.Clear();
		this.selected_kingdom = null;
		this.selected_court_member = null;
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x0018C000 File Offset: 0x0018A200
	public void SelectKingdom(int kid, bool reload_view = true)
	{
		global::Kingdom kingdom = global::Kingdom.Get(kid);
		if (kingdom != null && kingdom.logic != null && kingdom.logic.type != Logic.Kingdom.Type.Regular)
		{
			return;
		}
		this.SelectObj(null, false, false, true, true);
		this.selected_kingdom = kingdom;
		if (this.selected_kingdom == null)
		{
			this.selected_logic_obj = null;
			if (reload_view)
			{
				MapData.UpdateSrcKingdom(true);
			}
			return;
		}
		this.selected_logic_obj = this.selected_kingdom.logic;
		if (reload_view)
		{
			MapData.UpdateSrcKingdom(true);
		}
		Vars vars = new Vars();
		if (this.selected_kingdom.logic.IsDefeated())
		{
			vars.Set<string>("variant", "defeated");
		}
		else if (this.selected_kingdom.logic == BaseUI.LogicKingdom())
		{
			vars.Set<string>("variant", "own");
		}
		base.SetSelectionPanel(ObjectWindow.GetPrefab(this.selected_kingdom.logic, vars));
		Logic.Kingdom kingdom2 = BaseUI.LogicKingdom();
		if (((kingdom2 != null) ? kingdom2.inAudienceWith : null) != null && this.selected_kingdom.logic != kingdom2 && kingdom2.inAudienceWith != this.selected_kingdom.logic)
		{
			AudienceWindow.Create(this.selected_kingdom, "Main", null);
		}
		string text;
		if (!ViewMode.IsPoliticalView())
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			text = ((soundsDef != null) ? soundsDef.GetString("select_kingdom", null, "", true, true, true, '.') : null);
		}
		else
		{
			DT.Field soundsDef2 = BaseUI.soundsDef;
			text = ((soundsDef2 != null) ? soundsDef2.GetString("select_kingdom_pv", null, "", true, true, true, '.') : null);
		}
		string text2 = text;
		if (!string.IsNullOrEmpty(text2))
		{
			BaseUI.PlaySoundEvent(text2, null);
		}
		string key = "SelKingdomTrigger";
		Logic.Kingdom logic = this.selected_kingdom.logic;
		string religion;
		if (logic == null)
		{
			religion = null;
		}
		else
		{
			Religion religion2 = logic.religion;
			religion = ((religion2 != null) ? religion2.name : null);
		}
		BackgroundMusic.OnTrigger(key, religion);
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x0018C1A4 File Offset: 0x0018A3A4
	public void UpdateCourtSelection()
	{
		Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(base.kingdom);
		if (kingdom == null)
		{
			return;
		}
		this.tmp_CharList.Clear();
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character courtOrSpecialCourtMember = kingdom.GetCourtOrSpecialCourtMember(i);
			if (courtOrSpecialCourtMember != null)
			{
				global::Character character = courtOrSpecialCourtMember.visuals as global::Character;
				if (character != null && character.Obj != null && base.IsSelected(character.Obj))
				{
					this.tmp_CharList.Add(character);
				}
			}
		}
		if (this.tmp_CharList.Count == 1)
		{
			this.selected_court_member = this.tmp_CharList[0];
		}
		else if (this.tmp_CharList.Count > 1)
		{
			global::Character character2 = this.tmp_CharList[0];
			for (int j = 0; j < this.tmp_CharList.Count; j++)
			{
				if (this.tmp_CharList[j] == this.selected_court_member)
				{
					global::Character character3 = this.tmp_CharList[j];
					break;
				}
			}
		}
		if (this.royalCourt != null)
		{
			this.royalCourt.SelectCourtMember((this.selected_court_member != null) ? this.selected_court_member.logic : null, false);
		}
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x0018C2DC File Offset: 0x0018A4DC
	public bool SelectCourtMember(Logic.Character character)
	{
		global::Character character2 = ((character != null) ? character.visuals : null) as global::Character;
		if (character2 == null)
		{
			return false;
		}
		if (character2.Obj != null)
		{
			this.SelectObj(character2.Obj, false, true, true, true);
		}
		else
		{
			global::Character selected_court_member = this.selected_court_member;
			if (((selected_court_member != null) ? selected_court_member.Obj : null) != null)
			{
				this.SelectObj(null, false, true, true, true);
			}
		}
		this.selected_court_member = character2;
		this.UpdateCourtSelection();
		if (!character.IsOnMap())
		{
			BaseUI.PlayVoiceEvent(character.GetVoiceLine("select_character_voice_line"), character);
			BaseUI.PlaySoundEvent(character.GetSoundEffect("select_character_sound_effect"), null);
		}
		return true;
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x0018C380 File Offset: 0x0018A580
	public override GameObject GetSelectionObj(GameObject obj)
	{
		if (obj == null)
		{
			return null;
		}
		global::Battle battle = obj.GetComponent<global::Battle>();
		if (battle != null)
		{
			if (battle.CanBeSelected())
			{
				return obj;
			}
			if (battle.logic.settlement != null)
			{
				return (battle.logic.settlement.visuals as global::Settlement).gameObject;
			}
		}
		global::Settlement component = obj.GetComponent<global::Settlement>();
		if (component != null)
		{
			if (!component.logic.def.is_active_settlement)
			{
				return null;
			}
			battle = component.GetBattle();
			if (!(battle != null))
			{
				return obj;
			}
			global::Battle battle2 = component.GetBattle();
			if (battle2 != null && battle2.CanBeSelected())
			{
				return battle.gameObject;
			}
			return component.gameObject;
		}
		else
		{
			global::Army army = null;
			if (army == null)
			{
				army = obj.GetComponent<global::Army>();
			}
			if (!(army != null))
			{
				return obj;
			}
			bool flag = army.CanBeSelected();
			if (!flag)
			{
				return null;
			}
			battle = army.GetBattle();
			if (battle != null)
			{
				if (battle.CanBeSelected())
				{
					return battle.gameObject;
				}
				Logic.Battle logic = battle.logic;
				object obj2;
				if (logic == null)
				{
					obj2 = null;
				}
				else
				{
					Logic.Settlement settlement = logic.settlement;
					obj2 = ((settlement != null) ? settlement.visuals : null);
				}
				global::Settlement settlement2 = obj2 as global::Settlement;
				if (settlement2 == null)
				{
					return null;
				}
				return settlement2.gameObject;
			}
			else
			{
				global::Settlement castle = army.GetCastle();
				if (castle != null)
				{
					return castle.gameObject;
				}
				if (flag)
				{
					return obj;
				}
				return null;
			}
		}
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x0018C4D0 File Offset: 0x0018A6D0
	private void SetSelectionObjects(GameObject obj)
	{
		GameObject selectionObj = this.GetSelectionObj(obj);
		this.selected_objects.Clear();
		if (selectionObj == null)
		{
			return;
		}
		global::Settlement component = selectionObj.GetComponent<global::Settlement>();
		if (component != null && component.logic != null)
		{
			Logic.Realm realm = component.logic.GetRealm();
			if (realm.settlements != null && realm.settlements.Count > 1)
			{
				for (int i = 0; i < realm.settlements.Count; i++)
				{
					Logic.Settlement settlement = realm.settlements[i];
					if (settlement.IsActiveSettlement() && settlement != component.logic && settlement.visuals is global::Settlement)
					{
						this.selected_objects.Add((realm.settlements[i].visuals as global::Settlement).gameObject);
					}
				}
			}
		}
		global::Army component2 = selectionObj.GetComponent<global::Army>();
		if (component2 != null)
		{
			Logic.Army logic = component2.logic;
			if (((logic != null) ? logic.mercenary : null) != null)
			{
				Mercenary mercenary = component2.logic.mercenary;
				for (int j = 0; j < mercenary.buyers.Count; j++)
				{
					this.selected_objects.Add((mercenary.buyers[j].visuals as global::Army).gameObject);
				}
				return;
			}
			if (component2.logic != null)
			{
				Mercenary mercenary2;
				component2.HasTransferTarget(out mercenary2);
				if (mercenary2 != null && !component2.logic.movement.IsMoving(true) && !mercenary2.army.movement.IsMoving(true))
				{
					this.selected_objects.Add((mercenary2.army.visuals as global::Army).gameObject);
				}
				Logic.Army logic2 = component2.logic;
				if (logic2.interact_target != null)
				{
					this.selected_objects.Add((logic2.interact_target.visuals as global::Army).gameObject);
					return;
				}
				if (logic2.interactor != null)
				{
					this.selected_objects.Add((logic2.interactor.visuals as global::Army).gameObject);
				}
			}
		}
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x0018C6E0 File Offset: 0x0018A8E0
	public override void SelectObj(GameObject obj, bool force_refresh = false, bool reload_view = true, bool clicked = true, bool play_sound = true)
	{
		base.SetSelectTarget(obj);
		GameObject selectionObj = this.GetSelectionObj(obj);
		this.selected_orig = obj;
		if (this.selected_obj != null)
		{
			global::Army component = this.selected_obj.GetComponent<global::Army>();
			if (component != null)
			{
				component.SetSelected(false, true);
			}
			global::Settlement component2 = this.selected_obj.GetComponent<global::Settlement>();
			if (component2 != null)
			{
				component2.SetSelected(false, true);
			}
			global::Battle component3 = this.selected_obj.GetComponent<global::Battle>();
			if (component3 != null)
			{
				component3.SetSelected(false, true);
			}
		}
		if (this.selected_objects.Count > 0)
		{
			int i = 0;
			int count = this.selected_objects.Count;
			while (i < count)
			{
				GameObject gameObject = this.selected_objects[i];
				if (!(gameObject == null))
				{
					global::Army component4 = gameObject.GetComponent<global::Army>();
					if (component4 != null)
					{
						component4.SetSelected(false, true);
					}
					global::Settlement component5 = gameObject.GetComponent<global::Settlement>();
					if (component5 != null)
					{
						component5.SetSelected(false, true);
					}
					global::Battle component6 = gameObject.GetComponent<global::Battle>();
					if (component6 != null)
					{
						component6.SetSelected(false, true);
					}
				}
				i++;
			}
		}
		this.SetSelectionObjects(obj);
		this.selected_obj = selectionObj;
		this.selected_logic_obj = null;
		this.selected_kingdom = null;
		if (this.selected_court_member != null && selectionObj == null)
		{
			this.selected_court_member = null;
			this.UpdateCourtSelection();
		}
		else if (this.selected_court_member != null && selectionObj != null && !force_refresh)
		{
			GameLogic.Behaviour component7 = selectionObj.GetComponent<GameLogic.Behaviour>();
			if (component7 != null)
			{
				Logic.Object logic = component7.GetLogic();
				if (logic != null)
				{
					Logic.Kingdom kingdom = logic.GetKingdom();
					if (kingdom != null && kingdom.id == base.GetCurrentKingdomId())
					{
						this.selected_court_member = null;
						this.UpdateCourtSelection();
					}
				}
			}
		}
		GameObject gameObject2 = null;
		int selectedRealm = 0;
		if (this.selected_obj != null)
		{
			global::Army component8 = this.selected_obj.GetComponent<global::Army>();
			if (component8 != null && component8.logic != null)
			{
				component8.SetSelected(true, true);
				if (component8.logic.rebel != null)
				{
					this.selected_logic_obj = component8.logic.rebel;
					gameObject2 = ObjectWindow.GetPrefab(component8.logic.rebel, null);
				}
				else if (component8.logic.mercenary != null)
				{
					this.selected_logic_obj = component8.logic.mercenary;
					Vars vars = new Vars();
					gameObject2 = ObjectWindow.GetPrefab(component8.logic.mercenary, vars);
				}
				else
				{
					this.selected_logic_obj = component8.logic;
					bool flag = component8.logic.kingdom_id == base.GetCurrentKingdomId();
					Mercenary mercenary;
					bool flag2 = component8.HasTransferTarget(out mercenary);
					Vars vars2 = new Vars();
					if (flag && flag2 && mercenary == null)
					{
						vars2.Set<string>("variant", "unit_transfer");
					}
					else
					{
						vars2.Set<string>("variant", flag ? "compact" : "compact_other");
					}
					if (flag && mercenary != null && !component8.logic.movement.IsMoving(true) && !mercenary.army.movement.IsMoving(true))
					{
						gameObject2 = ObjectWindow.GetPrefab(mercenary, vars2);
					}
					else
					{
						gameObject2 = ObjectWindow.GetPrefab(component8.logic, vars2);
					}
				}
			}
			if (gameObject2 == null)
			{
				global::Settlement component9 = this.selected_obj.GetComponent<global::Settlement>();
				if (component9 != null && component9.logic != null)
				{
					this.selected_logic_obj = component9.logic;
					component9.SetSelected(true, true);
					selectedRealm = component9.GetRealmID();
					Vars vars3 = new Vars();
					if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat inspect castle", true))
					{
						vars3.Set<string>("variant", "own");
					}
					else
					{
						Vars vars4 = vars3;
						string key = "variant";
						string val;
						if (!component9.logic.GetRealm().IsOccupied())
						{
							Logic.Object controller = component9.logic.GetRealm().controller;
							int? num;
							if (controller == null)
							{
								num = null;
							}
							else
							{
								Logic.Kingdom kingdom2 = controller.GetKingdom();
								num = ((kingdom2 != null) ? new int?(kingdom2.id) : null);
							}
							if ((num ?? 0) == base.GetCurrentKingdomId())
							{
								val = "own";
								goto IL_439;
							}
						}
						val = "other";
						IL_439:
						vars4.Set<string>(key, val);
					}
					gameObject2 = ObjectWindow.GetPrefab(component9.logic, vars3);
				}
			}
			if (gameObject2 == null)
			{
				global::Battle component10 = this.selected_obj.GetComponent<global::Battle>();
				if (component10 != null && component10.logic != null)
				{
					this.selected_logic_obj = component10.logic;
					component10.SetSelected(true, true);
					selectedRealm = component10.GetRealmID();
					new Vars().Set<string>("variant", component10.logic.type.ToString());
					gameObject2 = ObjectWindow.GetPrefab(component10.logic, null);
				}
			}
		}
		if (this.selected_objects.Count > 0)
		{
			for (int j = 0; j < this.selected_objects.Count; j++)
			{
				GameObject gameObject3 = this.selected_objects[j];
				global::Army component11 = gameObject3.GetComponent<global::Army>();
				if (component11 != null)
				{
					component11.SetSelected(true, false);
				}
				else
				{
					global::Settlement component12 = gameObject3.GetComponent<global::Settlement>();
					if (component12 != null)
					{
						component12.SetSelected(true, this.selected_objects[j] == this.selected_orig);
					}
					else
					{
						global::Battle component13 = gameObject3.GetComponent<global::Battle>();
						if (component13 != null)
						{
							component13.SetSelected(true, false);
						}
					}
				}
			}
		}
		WorldMap worldMap = WorldMap.Get();
		if (worldMap != null)
		{
			worldMap.SetSelectedRealm(selectedRealm);
		}
		this.UpdateCourtSelection();
		if (reload_view && worldMap != null)
		{
			worldMap.SetSrcKingdom(this.selected_kingdom, true);
		}
		base.SetSelectionPanel(gameObject2);
		if (!force_refresh && play_sound)
		{
			BaseUI.PlaySelectionSound(this.selected_orig, clicked);
		}
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x0018CCAC File Offset: 0x0018AEAC
	public override void SelectObjFromLogic(Logic.Object obj, bool force_refresh = false, bool reload_view = true)
	{
		Logic.Character character = obj as Logic.Character;
		if (character != null)
		{
			MapObject location = character.GetLocation();
			if (location != null && location.visuals != null)
			{
				this.SelectObj((location.visuals as GameLogic.Behaviour).gameObject, force_refresh, reload_view, true, true);
				return;
			}
			if (character.IsInCourt())
			{
				this.SelectCourtMember(character);
				return;
			}
			return;
		}
		else
		{
			Logic.Kingdom kingdom = obj as Logic.Kingdom;
			if (kingdom != null)
			{
				this.SelectKingdom(kingdom.id, reload_view);
				return;
			}
			Logic.Realm realm = obj as Logic.Realm;
			if (realm != null && realm.castle != null)
			{
				obj = realm.castle;
			}
			base.SelectObjFromLogic(obj, force_refresh, reload_view);
			return;
		}
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x0018CD40 File Offset: 0x0018AF40
	private void OpenCharacterInfoWindow(Logic.Character character)
	{
		if (character == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		GameObject prefab = UICommon.GetPrefab("CharacterInfo", null);
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		if (gameObject != null)
		{
			UICommon.DeleteChildren(gameObject.transform, typeof(UICharacter));
			UICharacter.Create(character, prefab, gameObject.transform as RectTransform, null);
		}
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x0018CDBD File Offset: 0x0018AFBD
	public override void OnMenu()
	{
		base.OnMenu();
		GameSpeed.SupressSpeedChangesByPlayer = this.menu.activeSelf;
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x0018CDD5 File Offset: 0x0018AFD5
	public void InvalidateKingdom()
	{
		this.m_InvalidateUpdateKingdom = true;
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x0018CDE0 File Offset: 0x0018AFE0
	protected void UpdateKingdom(bool force = false)
	{
		Game game = GameLogic.Get(false);
		if (game == null || game.kingdoms == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.curt_logic_kingdom == kingdom && !force)
		{
			return;
		}
		this.curt_logic_kingdom = kingdom;
		GameObject gameObject = global::Common.FindChildByName(base.gameObject, "Kingdom Header", true, true);
		if (gameObject != null)
		{
			UIKingdomIcon[] componentsInChildren = gameObject.GetComponentsInChildren<UIKingdomIcon>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetObject(kingdom, null);
			}
			UIText.SetText(gameObject.GetComponentInChildren<TextMeshProUGUI>(), (kingdom == null) ? "" : global::Defs.Localize(kingdom.GetNameKey(null, ""), null, null, true, true));
		}
		this.RefreshCourt();
		if (this.selected_logic_obj != null)
		{
			this.SelectObjFromLogic(this.selected_logic_obj, false, true);
		}
		else
		{
			this.SelectObj(this.selected_obj, true, true, true, true);
		}
		if (kingdom != null && this.royalCourt != null)
		{
			this.royalCourt.SetData(kingdom);
		}
		if (!this.curt_logic_kingdom.IsDefeated())
		{
			UIEndGameWindow.Close();
		}
		MessageIcon.DeleteAll();
		MessageIcon.RecreateAll();
		this.RefreshMultiplayerUI();
		UIPlayerReminder uiplayerReminder = this.playerRemidner;
		if (uiplayerReminder != null)
		{
			uiplayerReminder.SetKingdom(kingdom);
		}
		if (kingdom != null)
		{
			kingdom.NotifyListeners("new_ui_kingdom_set", null);
		}
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x0018CF14 File Offset: 0x0018B114
	public void RefreshCourt()
	{
		if (this.royalCourt != null)
		{
			this.royalCourt.Refresh();
		}
		UIHireWidnow componentInChildren = base.GetComponentInChildren<UIHireWidnow>();
		if (componentInChildren != null)
		{
			componentInChildren.Refresh();
		}
		this.UpdateCourtSelection();
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x0018CF56 File Offset: 0x0018B156
	public void CreateGameStartedMessageIcons()
	{
		base.Invoke("CreateGameStartedWarsIcon", 1f);
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x0018CF68 File Offset: 0x0018B168
	public void CreateGameStartedWarsIcon()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null || kingdom.wars.Count < 1)
		{
			return;
		}
		Vars vars = new Vars();
		vars.Set<Logic.Kingdom>("kingdom", kingdom);
		vars.Set<int>("wars_count", kingdom.wars.Count);
		vars.Set<Value>("wars", new Value(kingdom.wars));
		MessageIcon.Create("GameStartedWarsMessage", vars, true, null);
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x0018CFD9 File Offset: 0x0018B1D9
	private void RefreshMultiplayerUI()
	{
		UIMultiplayerStats.Rebuild();
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x0018CFE0 File Offset: 0x0018B1E0
	protected override void UpdatePicker()
	{
		base.UpdatePicker();
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x0018CFE8 File Offset: 0x0018B1E8
	protected override void UpdateInput()
	{
		if (UIEndGameWindow.EndGameShown())
		{
			return;
		}
		if (UICommon.GetKeyUp(KeyCode.Escape, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && !this.IsMainMenuOpen() && ViewMode.IsPoliticalView() && !this.window_dispatcher.HasFocusWindow())
		{
			ViewMode.WorldView.Apply();
			return;
		}
		if (UICommon.GetKeyUp(KeyCode.F5, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && !UICommon.GetKey(KeyCode.LeftShift, false))
		{
			string text = "Quicksave";
			Game game = GameLogic.Get(false);
			Campaign campaign = (game != null) ? game.campaign : null;
			SaveGame.Save((campaign != null) ? campaign.Dir(text) : null, text, -1, -1, null);
			return;
		}
		if (UICommon.GetKeyUp(KeyCode.F9, UICommon.ModifierKey.None, UICommon.ModifierKey.None) && !UICommon.GetKey(KeyCode.LeftShift, false))
		{
			SaveGame.QuickLoad();
			return;
		}
		if (KeyBindings.GetBindDown("province_overview"))
		{
			UIProvinceSelectorWindow.ToggleOpen(BaseUI.LogicKingdom());
			return;
		}
		if (KeyBindings.GetBindDown("great_powers_and_rankings"))
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			GreatPowers great_powers;
			if (kingdom == null)
			{
				great_powers = null;
			}
			else
			{
				Game game2 = kingdom.game;
				great_powers = ((game2 != null) ? game2.great_powers : null);
			}
			UIGreatPowersWindow.ToggleOpen(great_powers);
			return;
		}
		if (KeyBindings.GetBindDown("royal_family_and_traditions"))
		{
			UIRoyalFamily.ToggleOpen(BaseUI.LogicKingdom());
			return;
		}
		if (KeyBindings.GetBindDown("royal_library") || UICommon.GetKeyDown(KeyCode.F1, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			UIWikiWindow.ToggleOpen("", null);
			return;
		}
		if (KeyBindings.GetBindDown("kingdom_advantages"))
		{
			UIKingdomAdvantagesWindow.ToggleOpen(BaseUI.LogicKingdom());
			return;
		}
		if (KeyBindings.GetBindDown("friends_threats_and_wars"))
		{
			UIWarsOverviewWindow.ToggleOpen(BaseUI.LogicKingdom(), null, null);
			return;
		}
		base.UpdateInput();
		this.UpdateControlGroupInput();
		UIMinimapOverlay uiminimapOverlay = this.minimapOverlay;
		if (uiminimapOverlay == null)
		{
			return;
		}
		uiminimapOverlay.UpdateMinimapPVInputButtons();
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x0018D16A File Offset: 0x0018B36A
	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (this.m_InvalidateUpdateKingdom)
		{
			this.UpdateKingdom(true);
			this.m_InvalidateUpdateKingdom = false;
		}
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x0018D188 File Offset: 0x0018B388
	protected override void Update()
	{
		base.Update();
		if (!UIEndGameWindow.EndGameShown())
		{
			if (KeyBindings.GetBindDown("cylce_selection"))
			{
				this.CycleSelection(false);
			}
			if (KeyBindings.GetBindDown("cylce_selection_backward"))
			{
				this.CycleSelection(true);
			}
		}
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x0018D1C0 File Offset: 0x0018B3C0
	private void CycleSelection(bool backwards = false)
	{
		global::Kingdom kingdom = global::Kingdom.Get(base.kingdom);
		GameObject gameObject = null;
		if (this.selected_obj == null)
		{
			global::Realm realm = kingdom.realms[0];
			if (realm.logic != null && realm.logic.castle != null && realm.logic.castle.visuals != null)
			{
				this.selected_obj = (realm.logic.castle.visuals as global::Settlement).gameObject;
				gameObject = this.GetNextOfKin(this.selected_obj, kingdom, backwards);
			}
		}
		else
		{
			gameObject = this.GetNextOfKin(this.selected_obj, kingdom, backwards);
		}
		if (gameObject != null)
		{
			this.SelectObj(gameObject, false, true, false, true);
		}
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x0018D278 File Offset: 0x0018B478
	private GameObject GetNextOfKin(GameObject targetObj, global::Kingdom kingdom, bool backwards = true)
	{
		if (targetObj == null)
		{
			return null;
		}
		if (kingdom == null)
		{
			return null;
		}
		if (kingdom.logic == null)
		{
			return null;
		}
		global::Battle component = targetObj.GetComponent<global::Battle>();
		if (component != null && component.logic != null)
		{
			if (component.logic.settlement != null && component.logic.settlement.type == "Castle" && component.logic.settlement.kingdom_id == kingdom.id)
			{
				targetObj = (component.logic.settlement.visuals as global::Settlement).gameObject;
			}
			else
			{
				int num;
				if (component.logic.attacker.GetKingdom().id != kingdom.id)
				{
					Logic.Army attacker_support = component.logic.attacker_support;
					if (((attacker_support != null) ? attacker_support.GetKingdom().id : 0) != kingdom.id)
					{
						num = 1;
						goto IL_DF;
					}
				}
				num = 0;
				IL_DF:
				int side = num;
				List<Logic.Army> armies = component.logic.GetArmies(side);
				bool flag = false;
				for (int i = 0; i < armies.Count; i++)
				{
					Logic.Army army = armies[i];
					if (army != null && army.visuals != null && army.kingdom_id == kingdom.id)
					{
						targetObj = (army.visuals as global::Army).gameObject;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return null;
				}
			}
		}
		global::Settlement component2 = targetObj.GetComponent<global::Settlement>();
		if (component2 != null && component2.logic != null)
		{
			if (component2.logic.type != "Castle")
			{
				return (component2.logic.GetRealm().castle.visuals as global::Settlement).gameObject;
			}
			if (kingdom.realms != null && kingdom.realms.Count > 0)
			{
				global::Realm item = component2.logic.GetRealm().visuals as global::Realm;
				int count = kingdom.realms.Count;
				int num2 = kingdom.realms.IndexOf(item);
				if (backwards)
				{
					num2--;
				}
				else
				{
					num2++;
				}
				if (num2 >= count)
				{
					num2 = 0;
				}
				else if (num2 < 0)
				{
					num2 = count - 1;
				}
				return (kingdom.realms[num2].logic.castle.visuals as global::Settlement).gameObject;
			}
		}
		global::Army component3 = targetObj.GetComponent<global::Army>();
		if (component3 != null && component3.logic != null && component3.logic.kingdom_id == kingdom.id)
		{
			int count2 = kingdom.logic.armies.Count;
			int num3 = kingdom.logic.armies.IndexOf(component3.logic);
			if (backwards)
			{
				num3--;
			}
			else
			{
				num3++;
			}
			if (num3 >= count2)
			{
				num3 = 0;
			}
			else if (num3 < 0)
			{
				num3 = count2 - 1;
			}
			return (kingdom.logic.armies[num3].visuals as global::Army).gameObject;
		}
		return null;
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x0018D568 File Offset: 0x0018B768
	public override void Select()
	{
		if (!ViewMode.IsPoliticalView())
		{
			base.Select();
			return;
		}
		global::Realm realm = global::Realm.At(this.picked_terrain_point);
		global::Kingdom kingdom = (realm == null) ? null : realm.GetKingdom();
		ViewMode current = ViewMode.current;
		if (realm != null && realm.IsSeaRealm())
		{
			this.SelectObj(null, false, true, true, true);
			return;
		}
		ViewMode.SelectionRules.Rule selectionRule = current.GetSelectionRule();
		string a = (selectionRule != null) ? selectionRule.pick_target : null;
		if (realm != null && a == "realm")
		{
			if (realm.logic != null && realm.logic.castle != null && realm.logic.castle.visuals != null)
			{
				GameObject gameObject = (realm.logic.castle.visuals as global::Settlement).gameObject;
				if (current is CulturesView && this.select_target == gameObject)
				{
					this.SelectObj(null, false, true, true, true);
				}
				else
				{
					this.SelectObj(gameObject, false, true, true, true);
				}
				DT.Field soundsDef = BaseUI.soundsDef;
				BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("select_kingdom_pv", null, "", true, true, true, '.') : null, null);
				return;
			}
		}
		else if (a == "kingdom")
		{
			if (kingdom != null)
			{
				this.SelectKingdom(kingdom.id, true);
				if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
				{
					AudienceWindow.Create(kingdom, "Main", BaseUI.LogicKingdom().visuals as global::Kingdom);
				}
			}
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null && realm != null)
			{
				worldMap.SetSelectedRealm(realm.id);
				return;
			}
		}
		else if (kingdom != null)
		{
			this.SelectKingdom(kingdom.id, true);
		}
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x0018D70D File Offset: 0x0018B90D
	protected override void OnParentSelect()
	{
		if (this.logger != null)
		{
			this.logger.Show(this.SelectionPanel == null);
		}
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x0018D731 File Offset: 0x0018B931
	public override void OnMouseDown(Vector2 pts, int btn)
	{
		base.OnMouseDown(pts, btn);
		UIMinimapOverlay uiminimapOverlay = this.minimapOverlay;
		if (uiminimapOverlay == null)
		{
			return;
		}
		uiminimapOverlay.Deselect();
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x0018D74C File Offset: 0x0018B94C
	public override void OnMouseUp(Vector2 pts, int btn)
	{
		base.OnMouseUp(pts, btn);
		if (btn == 0 && UnityEngine.Time.unscaledTime - this.btn_down_time <= 0.25f)
		{
			this.Select();
			if (this.dblclk)
			{
				bool flag = ViewMode.IsPoliticalView();
				if (flag)
				{
					ViewMode.WorldView.Apply();
				}
				if (this.selected_obj != null)
				{
					if (this.picked_settlement != null)
					{
						base.LookAt(this.picked_settlement.transform.position, false);
					}
					else
					{
						base.LookAt(this.selected_obj.transform.position, false);
					}
				}
				else if (flag && this.picked_terrain_point != Vector3.zero)
				{
					base.LookAt(this.picked_terrain_point, false);
				}
			}
			if (UICommon.CheckModifierKeys(UICommon.ModifierKey.Shift, UICommon.ModifierKey.None))
			{
				base.HandleChatLink(this.selected_logic_obj);
			}
		}
		if (ViewMode.IsPoliticalView())
		{
			UIMinimapOverlay uiminimapOverlay = this.minimapOverlay;
			if (uiminimapOverlay == null)
			{
				return;
			}
			uiminimapOverlay.DeselectPaletteSelection();
		}
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x0018D840 File Offset: 0x0018BA40
	public override void OnMouseMove(Vector2 pts, int btn)
	{
		base.OnMouseMove(pts, btn);
		global::Realm highlighedRealm = global::Realm.At(this.picked_terrain_point);
		WorldMap.Get().SetHighlighedRealm(highlighedRealm);
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x0018D86C File Offset: 0x0018BA6C
	public override void OnRightClick()
	{
		if (ViewMode.IsPoliticalView())
		{
			this.SelectObj(null, true, true, true, true);
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				worldMap.SetSrcKingdom(null, false);
				worldMap.UpdateSelectedBorders();
			}
			if (ViewMode.current != null)
			{
				ViewMode.current.Apply();
			}
			return;
		}
		global::Army army = null;
		if (this.select_target != null)
		{
			army = this.select_target.GetComponent<global::Army>();
			if (army == null)
			{
				global::Settlement component = this.select_target.GetComponent<global::Settlement>();
				if (component != null)
				{
					army = component.GetArmy();
				}
			}
		}
		if (army == null && this.selected_obj != null)
		{
			army = this.selected_obj.GetComponent<global::Army>();
			if (army == null)
			{
				global::Settlement component2 = this.selected_obj.GetComponent<global::Settlement>();
				if (component2 != null)
				{
					army = component2.GetArmy();
				}
			}
			if (army == null)
			{
				global::Battle component3 = this.selected_obj.GetComponent<global::Battle>();
				if (component3 != null)
				{
					if (base.kingdom.id != component3.logic.attacker.GetKingdom().id)
					{
						int id = base.kingdom.id;
						Logic.Army attacker_support = component3.logic.attacker_support;
						if (id != ((attacker_support != null) ? attacker_support.GetKingdom().id : 0))
						{
							if (base.kingdom.id != component3.logic.defender.GetKingdom().id)
							{
								int id2 = base.kingdom.id;
								Logic.Army defender_support = component3.logic.defender_support;
								if (id2 != ((defender_support != null) ? defender_support.GetKingdom().id : 0))
								{
									goto IL_1FD;
								}
							}
							army = ((component3.logic.defenders.Count > 0) ? (component3.logic.defenders[0].visuals as global::Army) : null);
							goto IL_1FD;
						}
					}
					army = ((component3.logic.attackers.Count > 0) ? (component3.logic.attackers[0].visuals as global::Army) : null);
				}
			}
		}
		IL_1FD:
		if (!(army != null))
		{
			return;
		}
		if (this.picked_army != null)
		{
			army.MoveTo(this.picked_army, true, true);
			return;
		}
		if (this.picked_settlement != null)
		{
			army.MoveTo(this.picked_settlement, true, true);
			return;
		}
		if (this.picked_passable_area != 0)
		{
			army.MoveTo(this.picked_passable_area_pos, this.picked_passable_area, true, true);
			return;
		}
		if (this.picked_terrain_point != Vector3.zero)
		{
			army.MoveTo(this.picked_terrain_point, 0, true, true);
		}
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x0018DAF8 File Offset: 0x0018BCF8
	public override void ViewModeChanged()
	{
		bool flag = ViewMode.IsPoliticalView();
		if (this.minimapObject != null)
		{
			this.minimapObject.SetActive(!flag);
		}
		if (this.minimapOverlay != null)
		{
			this.minimapOverlay.UpdateViewMode();
		}
		if (flag && this.SelectionPanel == null)
		{
			WorldMap worldMap = WorldMap.Get();
			if (worldMap != null)
			{
				this.SelectKingdom(worldMap.SrcKingdom, false);
			}
		}
		if (ViewMode.previousMode is PoliticalView && !(ViewMode.current is PoliticalView))
		{
			DT.Field soundsDef = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString("exit_political_view", null, "", true, true, true, '.') : null, null);
			return;
		}
		if (!(ViewMode.previousMode is PoliticalView) && ViewMode.current is PoliticalView)
		{
			DT.Field soundsDef2 = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef2 != null) ? soundsDef2.GetString("enter_political_view", null, "", true, true, true, '.') : null, null);
			return;
		}
		if (ViewMode.previousMode is PoliticalView && ViewMode.current is PoliticalView)
		{
			DT.Field soundsDef3 = BaseUI.soundsDef;
			BaseUI.PlaySoundEvent((soundsDef3 != null) ? soundsDef3.GetString("switch_political_view", null, "", true, true, true, '.') : null, null);
		}
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x0018DC2C File Offset: 0x0018BE2C
	public void OnKeyBindsChange()
	{
		UIMinimapOverlay uiminimapOverlay = this.minimapOverlay;
		if (uiminimapOverlay == null)
		{
			return;
		}
		uiminimapOverlay.RefreshMinimapPVButtons();
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x0018DC3E File Offset: 0x0018BE3E
	private void DoubleTapNumberCheck(KeyCode _key)
	{
		this.isDoubleTappedNumber = false;
		if (this.lastPressedNumber == _key && UnityEngine.Time.unscaledTime - this.timeLastPressedNumber <= this.dbtap_delay)
		{
			this.isDoubleTappedNumber = true;
		}
		this.lastPressedNumber = _key;
		this.timeLastPressedNumber = UnityEngine.Time.unscaledTime;
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x0018DC80 File Offset: 0x0018BE80
	protected override void UpdateControlGroupInput()
	{
		for (KeyCode keyCode = KeyCode.Alpha1; keyCode <= KeyCode.Alpha9; keyCode++)
		{
			if (UICommon.GetKeyUp(keyCode, UICommon.ModifierKey.None, UICommon.ModifierKey.All))
			{
				int index = this.courtButtonsDesiredIndexes[keyCode - KeyCode.Alpha1];
				this.DoubleTapNumberCheck(keyCode);
				this.SelectionSystem(index, this.isDoubleTappedNumber);
			}
		}
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x0018DCCC File Offset: 0x0018BECC
	public void SelectionSystem(int index, bool doubleTap)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom.court[index] == null)
		{
			this.royalCourt.SelectSlot(index);
			return;
		}
		global::Character character = kingdom.court[index].visuals as global::Character;
		if (character != null)
		{
			if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
			{
				this.royalCourt.ClearCurrentSelection();
				this.royalCourt.ClearHireSelection();
				Logic.Settlement settlement = character.logic.GetGovernedCastle();
				if (settlement == null)
				{
					settlement = character.logic.GetPreparingToGovernCastle();
				}
				if (settlement != null)
				{
					this.SelectObjFromLogic(settlement, false, true);
					if (doubleTap)
					{
						base.LookAt(settlement.position, false);
						return;
					}
				}
			}
			else
			{
				this.royalCourt.SelectSlot(index);
				if (doubleTap)
				{
					this.royalCourt.FocusSlot(index);
				}
			}
		}
	}

	// Token: 0x040020CC RID: 8396
	private GameObject minimapObject;

	// Token: 0x040020CD RID: 8397
	private UIMinimapOverlay minimapOverlay;

	// Token: 0x040020CE RID: 8398
	private UIRoyalCourt royalCourt;

	// Token: 0x040020CF RID: 8399
	private UILogger logger;

	// Token: 0x040020D0 RID: 8400
	private GameObject messageWindow;

	// Token: 0x040020D1 RID: 8401
	private Logic.Kingdom curt_logic_kingdom;

	// Token: 0x040020D2 RID: 8402
	[NonSerialized]
	public UIPlayerReminder playerRemidner;

	// Token: 0x040020D3 RID: 8403
	private IconsBar m_MessageIcons;

	// Token: 0x040020D4 RID: 8404
	private IconsBar m_OngoingIcons;

	// Token: 0x040020D5 RID: 8405
	private IconsBar m_IOIcons;

	// Token: 0x040020D6 RID: 8406
	private UILogger m_EventLogger;

	// Token: 0x040020D8 RID: 8408
	private bool started;

	// Token: 0x040020D9 RID: 8409
	private DT.Field load_from;

	// Token: 0x040020DA RID: 8410
	public static bool cam_pos_loaded;

	// Token: 0x040020DB RID: 8411
	private List<global::Character> tmp_CharList = new List<global::Character>();

	// Token: 0x040020DC RID: 8412
	private KeyCode lastPressedNumber;

	// Token: 0x040020DD RID: 8413
	private float timeLastPressedNumber = float.PositiveInfinity;

	// Token: 0x040020DE RID: 8414
	private bool isDoubleTappedNumber;

	// Token: 0x040020DF RID: 8415
	private int[] courtButtonsDesiredIndexes = new int[]
	{
		1,
		2,
		3,
		4,
		0,
		5,
		6,
		7,
		8,
		9
	};
}
