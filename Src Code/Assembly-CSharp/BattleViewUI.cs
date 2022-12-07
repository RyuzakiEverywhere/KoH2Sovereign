using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020001E0 RID: 480
public class BattleViewUI : BaseUI
{
	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06001C6A RID: 7274 RVA: 0x0010C0AD File Offset: 0x0010A2AD
	// (set) Token: 0x06001C6B RID: 7275 RVA: 0x0010C0B5 File Offset: 0x0010A2B5
	public BattleViewSelection Selection { get; private set; }

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06001C6C RID: 7276 RVA: 0x0010C0BE File Offset: 0x0010A2BE
	// (set) Token: 0x06001C6D RID: 7277 RVA: 0x0010C0C6 File Offset: 0x0010A2C6
	public int SelectedGroupId { get; private set; }

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06001C6E RID: 7278 RVA: 0x0010C0CF File Offset: 0x0010A2CF
	// (set) Token: 0x06001C6F RID: 7279 RVA: 0x0010C0D7 File Offset: 0x0010A2D7
	public bool dragging { get; private set; }

	// Token: 0x06001C70 RID: 7280 RVA: 0x0010C0E0 File Offset: 0x0010A2E0
	public new static BattleViewUI Get()
	{
		return BaseUI.Get<BattleViewUI>();
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x0010C0E7 File Offset: 0x0010A2E7
	protected override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x0010C0EF File Offset: 0x0010A2EF
	protected override void OnDisable()
	{
		base.OnDisable();
		GameSpeed.SupressSpeedChangesByPlayer = false;
		this.ClearSelection();
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x0010C103 File Offset: 0x0010A303
	private void Awake()
	{
		this.Selection = new BattleViewSelection(this);
		this.SquadActions = new BattleViewSquadActions();
	}

	// Token: 0x06001C74 RID: 7284 RVA: 0x0010C11C File Offset: 0x0010A31C
	protected override void Start()
	{
		base.Start();
		UICommon.FindComponents(this, false);
		base.selection_container = global::Common.FindChildByName(base.gameObject, "id_SelectionUI", true, true);
		this.minimap = global::Common.FindChildComponent<MiniMap>(base.gameObject, "id_MinimapRect");
		this.m_ArmyWindowContainer = global::Common.FindChildComponent<UIBattleViewArmyContainer>(base.gameObject, "id_ArmyWindowContainer");
		this.m_BattleOverview = global::Common.FindChildComponent<UIBattleViewOverview>(base.gameObject, "id_BattleOverview");
		this.m_BattleOverviewCompact = global::Common.FindChildComponent<UIBattleViewOverview>(base.gameObject, "id_BattleOverviewCompact");
		this.m_ShowArmiesButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_ShowArmiesButton");
		this.m_HideArmiesButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_HideArmiesButton");
		this.m_ShowBattleOverviewButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_ShowBattleOverviewButton");
		this.m_HideBattleOverviewButton = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_HideBattleOverviewButton");
		this.m_CapturePoints = global::Common.FindChildComponent<UICapturePointsBar>(base.gameObject, "id_CapturePoints");
		this.m_SquadsTargetPreview = base.GetComponent<SquadsTargetPreview>();
		this.Selection.onSelectionChange.AddListener(new UnityAction<ISelectable[]>(this.HandleSelection));
		this.Selection.Start();
		this.SelectedGroupId = -1;
		BSGButton showArmiesButton = this.m_ShowArmiesButton;
		showArmiesButton.onClick = (BSGButton.OnClick)Delegate.Combine(showArmiesButton.onClick, new BSGButton.OnClick(this.ArmiesVisibilityButton_OnClick));
		BSGButton hideArmiesButton = this.m_HideArmiesButton;
		hideArmiesButton.onClick = (BSGButton.OnClick)Delegate.Combine(hideArmiesButton.onClick, new BSGButton.OnClick(this.ArmiesVisibilityButton_OnClick));
		BSGButton showBattleOverviewButton = this.m_ShowBattleOverviewButton;
		showBattleOverviewButton.onClick = (BSGButton.OnClick)Delegate.Combine(showBattleOverviewButton.onClick, new BSGButton.OnClick(this.BattleOverviewVisibilityButton_OnClick));
		BSGButton hideBattleOverviewButton = this.m_HideBattleOverviewButton;
		hideBattleOverviewButton.onClick = (BSGButton.OnClick)Delegate.Combine(hideBattleOverviewButton.onClick, new BSGButton.OnClick(this.BattleOverviewVisibilityButton_OnClick));
		Mesh sharedMesh = MeshUtils.CreateLinesMesh(new List<Vector3>
		{
			new Vector3(-2f, 0.5f, 1f),
			new Vector3(0f, 0.5f, 2.5f),
			new Vector3(2f, 0.5f, 1f)
		}, 1f, 2, true, false, null, null);
		this.formationArrow = new GameObject();
		this.formationArrow.AddComponent<MeshFilter>().sharedMesh = sharedMesh;
		MeshRenderer meshRenderer = this.formationArrow.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material(Shader.Find("BSG/Instanced/SquadSelectionInstancedOverlay"));
		meshRenderer.sharedMaterial.color = Color.yellow;
		this.formationArrow.SetActive(false);
		Tutorial.Start();
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x0010C3AC File Offset: 0x0010A5AC
	private void HandleSelection(ISelectable[] newSelection)
	{
		if (newSelection != null && newSelection.Length != 0)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < newSelection.Length; i++)
			{
				list.Add(newSelection[i].transform.gameObject);
			}
			this.SelectObjects(list, false, false);
			return;
		}
		this.SelectObjects(null, false, false);
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x0010C3FA File Offset: 0x0010A5FA
	public UIBattleViewArmyContainer GetArmyWindow()
	{
		return this.m_ArmyWindowContainer;
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x0010C404 File Offset: 0x0010A604
	public override void SelectObj(GameObject obj, bool force_refresh = false, bool reload_view = true, bool clicked = true, bool play_sound = true)
	{
		List<GameObject> list = new List<GameObject>();
		if (obj != null && UICommon.GetKey(KeyCode.LeftControl, false))
		{
			global::Squad component = obj.GetComponent<global::Squad>();
			List<BattleSimulation.Squad> squads = BattleMap.battle.simulation.GetSquads(BattleMap.BattleSide);
			if (!(component != null))
			{
				goto IL_D4;
			}
			using (List<BattleSimulation.Squad>.Enumerator enumerator = squads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BattleSimulation.Squad squad = enumerator.Current;
					if (squad.def.name == component.logic.def.name)
					{
						global::Squad squad2 = squad.sub_squads[0].squad.visuals as global::Squad;
						if (squad2 != null)
						{
							list.Add(squad2.gameObject);
						}
					}
				}
				goto IL_D4;
			}
		}
		list.Add(obj);
		IL_D4:
		this.SelectObjects(list, force_refresh, false);
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0010C500 File Offset: 0x0010A700
	public override void SelectObjFromLogic(Logic.Object obj, bool force_refresh = false, bool reload_view = true)
	{
		if (!(obj is Logic.Character))
		{
			return;
		}
		BattleSimulation.Squad squad = this.ExtractLeaderSimulationLogic(obj as Logic.Character);
		bool flag;
		if (squad == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Squad squad2 = squad.squad;
			flag = (((squad2 != null) ? squad2.visuals : null) != null);
		}
		if (!flag)
		{
			return;
		}
		global::Squad squad3 = squad.squad.visuals as global::Squad;
		squad3.Selected = true;
		this.SelectObj(squad3.gameObject, false, true, true, true);
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x0010C568 File Offset: 0x0010A768
	public void BattleViewSquad_OnSquadTypeIconClicked(GameObject obj, bool force_refresh = false, bool reload_view = true, bool clicked = true, bool play_sound = true)
	{
		List<GameObject> list = new List<GameObject>();
		if (obj != null && UICommon.GetKey(KeyCode.LeftControl, false))
		{
			global::Squad component = obj.GetComponent<global::Squad>();
			List<BattleSimulation.Squad> squads = BattleMap.battle.simulation.GetSquads(BattleMap.BattleSide);
			if (!(component != null))
			{
				goto IL_1B9;
			}
			global::Defs.GetDefField(component.def.name, null);
			using (List<BattleSimulation.Squad>.Enumerator enumerator = squads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BattleSimulation.Squad squad = enumerator.Current;
					if (!squad.IsDefeated())
					{
						global::Squad squad2 = null;
						if (component.def.is_siege_eq && component.def.name == squad.def.name)
						{
							squad2 = (squad.sub_squads[0].squad.visuals as global::Squad);
						}
						else if (squad.def.type == Logic.Unit.Type.Noble && component.def.type == squad.def.type)
						{
							squad2 = (squad.sub_squads[0].squad.visuals as global::Squad);
						}
						else if (component.def.type == squad.def.type && component.def.secondary_type == squad.def.secondary_type && component.def.is_heavy == squad.def.is_heavy)
						{
							squad2 = (squad.sub_squads[0].squad.visuals as global::Squad);
						}
						if (squad2 != null)
						{
							list.Add(squad2.gameObject);
						}
					}
				}
				goto IL_1B9;
			}
		}
		list.Add(obj);
		IL_1B9:
		this.SelectObjects(list, force_refresh, false);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x0010C754 File Offset: 0x0010A954
	public void SelectObjects(List<GameObject> newSelection, bool force_refresh = false, bool group_selection = false)
	{
		this.SelectSquads(newSelection, group_selection);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x0010C760 File Offset: 0x0010A960
	public void SelectSquads(List<GameObject> newSelection, bool isGroupSelection = false)
	{
		bool key = UICommon.GetKey(KeyCode.LeftShift, false);
		if (newSelection == null || newSelection.Count == 0)
		{
			if (!key)
			{
				this.ClearGroupSelection();
			}
			this.ClearSelection();
			return;
		}
		bool flag = false;
		RelationUtils.Stance stance = BattleViewUI.<SelectSquads>g__GetSquadsSide|51_0(this.m_SelectedObjects, out flag);
		RelationUtils.Stance stance2 = BattleViewUI.<SelectSquads>g__GetSquadsSide|51_0(newSelection, out flag);
		if ((stance2 & (RelationUtils.Stance.Alliance | RelationUtils.Stance.Own)) == RelationUtils.Stance.None)
		{
			flag = false;
		}
		if (flag)
		{
			if ((stance2 & RelationUtils.Stance.Own) != RelationUtils.Stance.None)
			{
				stance2 = RelationUtils.Stance.Own;
			}
			else if ((stance2 & RelationUtils.Stance.Alliance) != RelationUtils.Stance.None)
			{
				stance2 = RelationUtils.Stance.Alliance;
			}
			BattleViewUI.<SelectSquads>g__RemoveSquadsNotMatchingTheSide|51_1(newSelection, stance2);
		}
		if ((stance2 != stance && stance != RelationUtils.Stance.None) || stance2 == RelationUtils.Stance.None)
		{
			this.ClearSelection();
		}
		if (newSelection != null && newSelection.Count != 0)
		{
			List<GameObject> list = BattleViewUI.<SelectSquads>g__GetObjectsEligableForSelection|51_2(newSelection);
			if (!key)
			{
				this.<SelectSquads>g__ClearCurrentSelection|51_3();
			}
			bool flag2 = false;
			List<GameObject> list2 = new List<GameObject>();
			for (int i = 0; i < list.Count; i++)
			{
				if (key && !isGroupSelection && newSelection.Count == 1 && this.m_SelectedObjects.Contains(list[i]))
				{
					this.m_SelectedObjects.Remove(list[i]);
					list2.Add(list[i]);
					flag2 = true;
				}
				else if (!this.m_SelectedObjects.Contains(list[i]))
				{
					this.m_SelectedObjects.Add(list[i]);
					flag2 = true;
				}
			}
			if (flag2 && !isGroupSelection && (!key || newSelection == null || newSelection.Count == 0 || !flag))
			{
				this.ClearGroupSelection();
			}
			foreach (GameObject gameObject in list2)
			{
				global::Squad component = gameObject.GetComponent<global::Squad>();
				if (component != null)
				{
					component.Selected = false;
				}
			}
			global::Squad squad = null;
			foreach (GameObject gameObject2 in this.m_SelectedObjects)
			{
				global::Squad component2 = gameObject2.GetComponent<global::Squad>();
				if (component2 != null)
				{
					component2.Selected = true;
					if (squad == null)
					{
						squad = component2;
					}
				}
			}
			if (squad != null && squad.logic.IsOwnStance(BaseUI.LogicKingdom()))
			{
				BaseUI.PlaySoundEvent(squad.logic.def.SelectSound, null);
				BaseUI.PlayVoiceEvent(squad.logic.def.select_voice, squad.logic, global::Common.SnapToTerrain(squad.logic.VisualPosition(), 0f, null, -1f, false));
			}
		}
		this.UpdateContexSelection();
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x0010CA0C File Offset: 0x0010AC0C
	public List<Logic.Object> GetSelectedObjects()
	{
		List<Logic.Object> list = new List<Logic.Object>();
		for (int i = 0; i < this.m_SelectedObjects.Count; i++)
		{
			if (!(this.m_SelectedObjects[i] == null))
			{
				global::Squad component = this.m_SelectedObjects[i].GetComponent<global::Squad>();
				if (!(component == null) && component.logic != null)
				{
					list.Add(component.logic);
				}
			}
		}
		return list;
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x0010CA7C File Offset: 0x0010AC7C
	public List<Logic.Squad> GetSelectedSquads()
	{
		List<Logic.Squad> list = new List<Logic.Squad>();
		for (int i = 0; i < this.selected_squads.Count; i++)
		{
			global::Squad squad = this.selected_squads[i];
			if (!(squad == null) && squad.logic != null)
			{
				list.Add(squad.logic);
			}
		}
		return list;
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x0010CAD0 File Offset: 0x0010ACD0
	public void ClearSelection()
	{
		foreach (GameObject gameObject in this.m_SelectedObjects)
		{
			if (!(gameObject == null))
			{
				global::Squad component = gameObject.GetComponent<global::Squad>();
				if (component != null)
				{
					component.Selected = false;
				}
			}
		}
		this.m_SelectedObjects.Clear();
		this.UpdateContexSelection();
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x0010CB50 File Offset: 0x0010AD50
	public void ClearGroupSelection()
	{
		this.SelectedGroupId = -1;
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x0010CB5C File Offset: 0x0010AD5C
	public void UpdateContexSelection()
	{
		List<Logic.Squad> selectedSquads = this.GetSelectedSquads();
		global::Squad selectedSquad;
		if (selectedSquads.Count <= 0)
		{
			selectedSquad = null;
		}
		else
		{
			Logic.Squad squad = selectedSquads[0];
			selectedSquad = (((squad != null) ? squad.visuals : null) as global::Squad);
		}
		this.m_SelectedSquad = selectedSquad;
		this.selected_obj = ((this.m_SelectedSquad == null) ? null : this.m_SelectedSquad.gameObject);
		GameObject selectionPanel = null;
		if (this.selected_obj != null)
		{
			global::Squad selectedSquad2 = this.m_SelectedSquad;
			if (selectedSquad2 != null && selectedSquad2.logic != null)
			{
				this.selected_logic_obj = selectedSquad2.logic;
				if (selectedSquads.Count > 1)
				{
					Vars vars = new Vars();
					vars.Set<string>("window_name", "MultipleSquadsWindow");
					selectionPanel = MultipleObjectWindow.GetPrefab(this.GetSelectedObjects(), vars);
				}
				else
				{
					selectionPanel = ObjectWindow.GetPrefab(selectedSquad2.logic, null);
				}
			}
		}
		else
		{
			this.selected_logic_obj = null;
		}
		this.SquadActions.SetSquads(selectedSquads);
		base.SetSelectionPanel(selectionPanel);
		if (this.m_ArmyWindowContainer != null)
		{
			this.m_ArmyWindowContainer.UpdateSelection();
		}
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x0010CC60 File Offset: 0x0010AE60
	public override void OnMenu()
	{
		base.OnMenu();
		if (this.menu == null)
		{
			return;
		}
		GameSpeed.SupressSpeedChangesByPlayer = this.menu.activeSelf;
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x0010CC88 File Offset: 0x0010AE88
	private void PickSquad(Ray ray)
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		EventSystem current = EventSystem.current;
		if ((current != null) ? current.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI())
		{
			return;
		}
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			global::Squad squad = Troops.squads[i];
			if (!(squad == null) && squad.data != null && squad.logic != null)
			{
				int battle_side = squad.logic.battle_side;
				if (battle_side >= 0)
				{
					float num3 = squad.RayCast(this, ray);
					if (num3 >= 0f && num3 < ((battle_side == 0) ? num : num2))
					{
						this.picked_squads[battle_side] = squad;
						if (battle_side == 0)
						{
							num = num3;
						}
						else
						{
							num2 = num3;
						}
						this.picked_map_object = squad.logic;
					}
				}
			}
		}
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x0010CD44 File Offset: 0x0010AF44
	private void PickFortification(Ray ray)
	{
		if (Troops.fortifications == null)
		{
			return;
		}
		EventSystem current = EventSystem.current;
		if ((current != null) ? current.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI())
		{
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		for (int i = 0; i < Troops.fortifications.Length; i++)
		{
			global::Fortification fortification = Troops.fortifications[i];
			if (!(fortification == null) && fortification.data != null && fortification.logic != null && !fortification.logic.IsDefeated())
			{
				int battle_side = fortification.logic.battle_side;
				if (battle_side >= 0)
				{
					float num3 = fortification.RayCast(this, ray);
					if (num3 >= 0f && num3 < ((battle_side == 0) ? num : num2))
					{
						this.picked_fortifications[battle_side] = fortification;
						if (battle_side == 0)
						{
							num = num3;
						}
						else
						{
							num2 = num3;
						}
						this.picked_map_object = fortification.logic;
					}
				}
			}
		}
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x0010CE18 File Offset: 0x0010B018
	private void PickCapturePoint(Ray ray)
	{
		Logic.Battle battle = BattleMap.battle;
		if (((battle != null) ? battle.capture_points : null) == null)
		{
			return;
		}
		EventSystem current = EventSystem.current;
		if ((current != null) ? current.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI())
		{
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		for (int i = 0; i < BattleMap.battle.capture_points.Count; i++)
		{
			Logic.CapturePoint capturePoint = BattleMap.battle.capture_points[i];
			int battle_side = capturePoint.battle_side;
			if (battle_side >= 0)
			{
				global::CapturePoint capturePoint2 = ((capturePoint != null) ? capturePoint.visuals : null) as global::CapturePoint;
				if (!(capturePoint2 == null))
				{
					float num3 = capturePoint2.RayCast(ray);
					if (num3 >= 0f && num3 < ((battle_side == 0) ? num : num2))
					{
						this.picked_capture_point[battle_side] = capturePoint2;
						if (battle_side == 0)
						{
							num = num3;
						}
						else
						{
							num2 = num3;
						}
						this.picked_map_object = capturePoint;
					}
				}
			}
		}
	}

	// Token: 0x06001C85 RID: 7301 RVA: 0x0010CEF4 File Offset: 0x0010B0F4
	protected override void UpdatePicker()
	{
		if (BattleMap.battle == null || !BattleMap.battle.IsValid() || BattleFieldOverview.InProgress())
		{
			return;
		}
		if (Input.mousePosition != this.ptLastMousePos)
		{
			this.tmLastMouseMove = UnityEngine.Time.unscaledTime;
			this.ptLastMousePos = Input.mousePosition;
		}
		Ray ray = CameraController.MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] array = Physics.RaycastAll(ray);
		this.picked_squads[0] = (this.picked_squads[1] = null);
		this.picked_fortifications[0] = (this.picked_fortifications[1] = null);
		this.picked_capture_point[0] = (this.picked_capture_point[1] = null);
		this.picked_terrain_point = Vector3.zero;
		this.picked_passable_area = 0;
		this.picked_map_object = null;
		Tooltip tooltip = null;
		this.PickSquadFromUI();
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i].transform;
			if (!(transform == null))
			{
				if (tooltip == null)
				{
					tooltip = Tooltip.FindInParents(transform);
				}
				global::Squad componentInParent = transform.GetComponentInParent<global::Squad>();
				if (componentInParent != null && componentInParent.logic != null && componentInParent.logic.battle_side >= 0)
				{
					this.picked_squads[componentInParent.logic.battle_side] = componentInParent;
					this.picked_map_object = componentInParent.logic;
				}
				if (transform.GetComponentInParent<Terrain>() != null)
				{
					this.picked_terrain_point = array[i].point;
				}
				global::Fortification componentInParent2 = transform.GetComponentInParent<global::Fortification>();
				if (componentInParent2 != null && !componentInParent2.logic.IsDefeated())
				{
					this.picked_map_object = componentInParent2.logic;
				}
				global::CapturePoint componentInParent3 = transform.GetComponentInParent<global::CapturePoint>();
				if (componentInParent3 != null)
				{
					this.picked_capture_point[componentInParent3.logic.battle_side] = componentInParent3;
					this.picked_map_object = componentInParent3.logic;
				}
				PassableArea componentInParent4 = transform.GetComponentInParent<PassableArea>();
				if (componentInParent4 != null)
				{
					this.picked_passable_area_pos = array[i].point;
					this.picked_passable_area = componentInParent4.id;
				}
			}
		}
		if (Troops.Initted)
		{
			if (this.picked_squads[0] == null || this.picked_squads[1] == null)
			{
				this.PickSquad(ray);
			}
			if (this.picked_fortifications[0] == null || this.picked_fortifications[1] == null)
			{
				this.PickFortification(ray);
			}
		}
		this.SetCursor(this.picked_passable_area_pos);
		base.UpdateTooltip(tooltip);
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x0010D170 File Offset: 0x0010B370
	private void PickSquadFromUI()
	{
		for (int i = 0; i < Troops.squads.Length; i++)
		{
			global::Squad squad = Troops.squads[i];
			if (!(squad == null) && squad.logic != null && squad.StatusBarIsHovered() && this.picked_squads[squad.logic.battle_side] == null)
			{
				this.picked_squads[squad.logic.battle_side] = squad;
			}
		}
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x0010D1E0 File Offset: 0x0010B3E0
	private bool IsMouseOverWall(Logic.Fortification fortification)
	{
		bool flag = false;
		List<Logic.Squad> selectedSquads = this.GetSelectedSquads();
		for (int i = 0; i < selectedSquads.Count; i++)
		{
			global::Squad squad = selectedSquads[i].visuals as global::Squad;
			if (!(squad == null))
			{
				Logic.Squad logic = squad.logic;
				if (logic != null && !logic.is_inside_walls_or_on_walls && !logic.climbing && (logic.movement.allowed_area_types & PathData.PassableArea.Type.Ladder) > PathData.PassableArea.Type.None)
				{
					flag = true;
					break;
				}
			}
		}
		return flag && ((fortification != null && fortification.def.type == Logic.Fortification.Type.Wall) || this.IsMouseOverWall());
	}

	// Token: 0x06001C88 RID: 7304 RVA: 0x0010D274 File Offset: 0x0010B474
	private bool IsMouseOverWall()
	{
		if (this.picked_passable_area == 0)
		{
			return false;
		}
		PathData.PassableArea pa = BattleMap.battle.batte_view_game.path_finding.data.pointers.GetPA(this.picked_passable_area - 1);
		return pa.type == PathData.PassableArea.Type.Ladder || pa.type == PathData.PassableArea.Type.Wall;
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0010D2C8 File Offset: 0x0010B4C8
	private void UpdateLadderPreview()
	{
		if (this.ladder_previewed == this.ladder_was_previewed && this.ladder_PA == this.ladder_PA_prev)
		{
			return;
		}
		this.ladder_was_previewed = this.ladder_previewed;
		this.ladder_PA_prev = this.ladder_PA;
		if (this.ladder_PA != 0)
		{
			if (BattleMap.battle.batte_view_game.path_finding.data.pointers.GetPA(this.ladder_PA - 1).type != PathData.PassableArea.Type.Ladder)
			{
				this.ladder_previewed = false;
			}
		}
		else
		{
			this.ladder_previewed = false;
		}
		if (this.ladder_previewed && this.ladder_preview_go == null)
		{
			Logic.Ladder.Def @base = GameLogic.Get(true).defs.GetBase<Logic.Ladder.Def>();
			if (@base == null)
			{
				return;
			}
			GameObject gameObject = @base.field.GetRandomValue("preview_prefab", null, true, true, true, '.').Get<GameObject>();
			if (gameObject == null)
			{
				return;
			}
			this.ladder_preview_go = global::Common.Spawn(gameObject, base.transform, false, "");
		}
		if (this.ladder_previewed)
		{
			Vector3 position;
			float num;
			float x;
			float y;
			global::Ladder.CalcPosRot(this.ladder_PA, out position, out num, out x, out y);
			this.ladder_preview_go.transform.position = position;
			this.ladder_preview_go.transform.rotation = Quaternion.Euler(x, y, 0f);
		}
		if (this.ladder_preview_go != null)
		{
			this.ladder_preview_go.SetActive(this.ladder_previewed);
		}
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x0010D428 File Offset: 0x0010B628
	public override void SetCursor(Vector3 pos)
	{
		if (GameLogic.Get(false) == null)
		{
			return;
		}
		DT.Field defField = global::Defs.GetDefField("Cursors", null);
		DT.Field cursor = defField.FindChild("Normal_Cursor", null, true, true, true, '.');
		this.ladder_PA = 0;
		this.ladder_previewed = false;
		EventSystem current = EventSystem.current;
		if (!((current != null) ? current.IsPointerOverGameObject() : IMGUIHandler.IsPointerOverIMGUI()))
		{
			Logic.Squad squad = this.selected_logic_obj as Logic.Squad;
			if (((squad != null) ? squad.def : null) != null)
			{
				Logic.Squad target_squad = this.picked_map_object as Logic.Squad;
				DT.Field field = BattleViewUI.SquadToSquadCursorField(squad, target_squad, defField);
				if (field != null)
				{
					cursor = field;
				}
				global::Fortification fortification = this.picked_fortifications[1 - squad.battle_side];
				Logic.Fortification fortification2 = (fortification != null) ? fortification.logic : null;
				if (fortification2 == null)
				{
					global::Fortification fortification3 = this.picked_fortifications[squad.battle_side];
					fortification2 = ((fortification3 != null) ? fortification3.logic : null);
				}
				if (this.IsMouseOverWall(fortification2))
				{
					cursor = defField.FindChild("Climb_Cursor", null, true, true, true, '.');
					if (this.picked_passable_area != 0)
					{
						this.ladder_PA = this.picked_passable_area;
					}
					else if (((fortification2 != null) ? fortification2.paids : null) != null && fortification2.paids.Count > 0)
					{
						for (int i = 0; i < fortification2.paids.Count; i++)
						{
							int num = fortification2.paids[i];
							if (BattleMap.battle.batte_view_game.path_finding.data.pointers.GetPA(num - 1).type == PathData.PassableArea.Type.Ladder)
							{
								this.ladder_PA = num;
								break;
							}
						}
					}
					if (this.ladder_PA != 0)
					{
						this.ladder_previewed = true;
					}
				}
				else if (squad.CanAttack(fortification2) && fortification2 != null && fortification2.IsEnemy(squad) && (!this.dragging || UnityEngine.Time.unscaledTime - this.btn_down_time <= 0.25f))
				{
					if (!squad.CanShootInTrees())
					{
						cursor = defField.FindChild("SiegeCantAttack_Cursor", null, true, true, true, '.');
					}
					else
					{
						cursor = defField.FindChild("Siege_Cursor", null, true, true, true, '.');
					}
				}
			}
			base.SetCursor(cursor);
			return;
		}
		if (Hotspot.picked != null)
		{
			cursor = Hotspot.picked.GetCursorFieldKey(defField);
			base.SetCursor(cursor);
			return;
		}
		if (BSGButton.picked != null)
		{
			cursor = BSGButton.picked.GetCursorFieldKey(defField);
			base.SetCursor(cursor);
			return;
		}
		if (Hotspot.picked != null || BSGButton.picked != null)
		{
			cursor = defField.FindChild("Normal_Cursor_Highlight", null, true, true, true, '.');
		}
		base.SetCursor(cursor);
	}

	// Token: 0x06001C8B RID: 7307 RVA: 0x0010D6A0 File Offset: 0x0010B8A0
	public static DT.Field SquadToSquadCursorField(Logic.Squad squad, Logic.Squad target_squad, DT.Field field)
	{
		DT.Field result = null;
		if (target_squad != null && target_squad.IsEnemy(squad))
		{
			if (squad.is_fleeing)
			{
				result = field.FindChild("CantAttack_Cursor", null, true, true, true, '.');
			}
			else if (squad.CanShoot(target_squad, false) && !UICommon.GetKey(KeyCode.LeftControl, false) && !squad.is_fighting)
			{
				global::Squad squad2 = squad.visuals as global::Squad;
				float num = 0f;
				float num2;
				float value;
				squad.high_ground_buff.CalcMod(global::Common.GetTerrainHeight(squad.position, null, false), global::Common.GetTerrainHeight(target_squad.position, null, false), out num2, out value);
				if (Math.Abs(value) > squad.high_ground_buff.high_ground_def.min_height_diff)
				{
					num += num2;
				}
				num += squad.trees_buff.GetCTHShootMod();
				if ((squad2 != null && !squad2.CheckSalvoPassability(target_squad.position, target_squad, false)) || !squad.CanShootInTrees())
				{
					result = field.FindChild("Shoot_Cursor_No_LOS", null, true, true, true, '.');
				}
				else if (num < 0f)
				{
					result = field.FindChild("Shoot_Cursor_Disadvantage", null, true, true, true, '.');
				}
				else if (num > 0f)
				{
					result = field.FindChild("Shoot_Cursor_Advantage", null, true, true, true, '.');
				}
				else
				{
					result = field.FindChild("Shoot_Cursor", null, true, true, true, '.');
				}
			}
			else if (squad.def.can_attack_melee)
			{
				result = field.FindChild("Attack_Cursor", null, true, true, true, '.');
			}
		}
		return result;
	}

	// Token: 0x06001C8C RID: 7308 RVA: 0x0010D81C File Offset: 0x0010BA1C
	protected unsafe override void UpdateInput()
	{
		if (BattleFieldOverview.InProgress())
		{
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_stop"))
		{
			this.SquadActions.CallSquadActions("stop");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_shrink_ranks"))
		{
			this.SquadActions.CallSquadActions("shrink_formation");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_regular_ranks"))
		{
			this.SquadActions.CallSquadActions("reset_formation");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_widen_ranks"))
		{
			this.SquadActions.CallSquadActions("expand_formation");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_square_formation"))
		{
			this.SquadActions.CallSquadActions("line");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_triangle_formation"))
		{
			this.SquadActions.CallSquadActions("triangle");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_hold_fire"))
		{
			this.SquadActions.CallSquadActions("hold_fire");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_hold_ground"))
		{
			this.SquadActions.CallSquadActions("hold_ground");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_deploy_trebuchet"))
		{
			this.SquadActions.CallSquadActions("deploy");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_squad_charge"))
		{
			this.SquadActions.CallSquadActions("charge");
			return;
		}
		if (KeyBindings.GetBindUp("toggle_mark_as_target"))
		{
			this.SquadActions.CallSquadActions("mark_as_target");
			return;
		}
		if (KeyBindings.GetBindDown("hold_to_reveal_travel_paths") && this.m_SquadsTargetPreview != null)
		{
			this.m_SquadsTargetPreview.OnPreviewStart();
		}
		if (KeyBindings.GetBind("hold_to_reveal_travel_paths", false) && this.m_SquadsTargetPreview != null)
		{
			this.m_SquadsTargetPreview.SetPreviewPositions();
		}
		if (KeyBindings.GetBindUp("hold_to_reveal_travel_paths") && this.m_SquadsTargetPreview != null)
		{
			this.m_SquadsTargetPreview.OnPreviewEnd();
		}
		if (UICommon.GetKeyUp(KeyCode.PageDown, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			foreach (GameObject gameObject in this.m_SelectedObjects)
			{
				global::Squad component = gameObject.GetComponent<global::Squad>();
				if (!(component == null) && component.data != null && component.data->logic_alive > 0)
				{
					component.data->logic_alive--;
				}
			}
			return;
		}
		if (this.dragging && this.formationArrow.activeInHierarchy && UICommon.GetKeyUp(KeyCode.Escape, UICommon.ModifierKey.None, UICommon.ModifierKey.All))
		{
			this.DisableFormation();
			return;
		}
		base.UpdateInput();
		this.UpdateControlGroupInput();
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x000023FD File Offset: 0x000005FD
	private void SubSquadTestInputs()
	{
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x0010DA9C File Offset: 0x0010BC9C
	protected override void Update()
	{
		base.Update();
		this.UpdateLadderPreview();
		if (this.mouse_btn_down <= 0)
		{
			this.Selection.Update();
		}
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x0010DAC0 File Offset: 0x0010BCC0
	public override void OnMouseDown(Vector2 pts, int btn)
	{
		float unscaledTime = UnityEngine.Time.unscaledTime;
		Vector3 mousePosition = Input.mousePosition;
		this.dblclk = (unscaledTime - this.btn_down_time < this.dblclk_delay && Vector2.Distance(mousePosition, this.mouse_click_last_pos) < this.dblclk_max_dist);
		this.mouse_click_last_pos = mousePosition;
		this.btn_down_time = unscaledTime;
		if (this.m_SelectedObjects != null && this.m_SelectedObjects.Count > 0 && (btn == 1 || (btn == 0 && KeyBindings.GetBind("alternate_drag_formation", false))))
		{
			this.dragging = true;
			if (this.picked_passable_area != 0)
			{
				this.startDragPos = new PPos(this.picked_passable_area_pos, this.picked_passable_area);
				return;
			}
			this.startDragPos = this.picked_terrain_point;
		}
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x0010DB85 File Offset: 0x0010BD85
	public override void OnSecondMouseDown()
	{
		if (this.mouse_btn_down == 1 || this.mouse_btn_down == 0)
		{
			this.DisableFormation();
		}
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x0010DB9E File Offset: 0x0010BD9E
	public override void OnMouseUp(Vector2 pts, int btn)
	{
		if (btn == 1)
		{
			this.OnRightClick();
		}
		else if (btn == 0 && KeyBindings.GetBind("alternate_drag_formation", false))
		{
			this.OnLeftClick();
		}
		this.DisableFormation();
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x0010DBC8 File Offset: 0x0010BDC8
	private bool IgnoreSquadInFormation(global::Squad squad)
	{
		return Game.cheat_level == Game.CheatLevel.None && (squad.logic.GetKingdom() != BaseUI.LogicKingdom() || squad.logic.IsMercenary());
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x0010DBF2 File Offset: 0x0010BDF2
	private void DisableFormation()
	{
		this.dragging = false;
		this.formationArrow.SetActive(false);
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x0010DC08 File Offset: 0x0010BE08
	private unsafe void LineFormation(bool dragIsForward = false)
	{
		if (this.selected_squads == null || this.selected_squads.Count == 0)
		{
			return;
		}
		bool flag = UnityEngine.Time.unscaledTime - this.btn_down_time > 0.25f;
		PPos ppos = this.startDragPos;
		PPos pt = this.picked_terrain_point;
		PathData.PassableArea passableArea = default(PathData.PassableArea);
		if (this.picked_passable_area != 0)
		{
			pt = new PPos(this.picked_passable_area_pos, this.picked_passable_area);
		}
		else if (ppos.paID != 0)
		{
			pt = new PPos(this.picked_terrain_point, ppos.paID);
		}
		if (ppos.paID != 0)
		{
			passableArea = Troops.path_data.GetPA(ppos.paID - 1);
		}
		Point point = pt - ppos;
		Point point2 = point.GetNormalized();
		float num = point.Length();
		bool flag2 = num < 2f;
		bool flag3 = ppos.paID > 0 && !passableArea.IsGround();
		bool flag4 = flag2 || dragIsForward || flag3;
		bool flag5 = BattleMap.Get().IsInsideWall(ppos);
		if (flag4)
		{
			num *= 2f;
		}
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		int num8 = 0;
		List<BattleViewUI.SquadProj> list = new List<BattleViewUI.SquadProj>();
		Point point3 = Point.Zero;
		Point point4 = Point.Zero;
		global::Squad squad = null;
		for (int i = this.selected_squads.Count - 1; i >= 0; i--)
		{
			if (this.selected_squads[i] == null)
			{
				this.selected_squads.RemoveAt(i);
			}
			else
			{
				global::Squad squad2 = this.selected_squads[i];
				if (squad2 == null || squad2.data_formation.count == 0)
				{
					this.selected_squads.RemoveAt(i);
				}
				else if (!this.IgnoreSquadInFormation(squad2))
				{
					if (this.IsPositionInvalidForSquad(squad2, flag, flag2, flag3))
					{
						squad2.previewFormation = null;
						squad2.previewFormationConfirmed = null;
					}
					else
					{
						Formation formation = squad2.previewFormation;
						Formation data_formation = squad2.data_formation;
						if (formation == null)
						{
							formation = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
							FormationPool.Return(ref squad2.previewFormation);
							squad2.previewFormation = formation;
							Formation previewFormationConfirmed = FormationPool.Get(squad2.logic.formation.def, squad2.logic);
							FormationPool.Return(ref squad2.previewFormationConfirmed);
							squad2.previewFormationConfirmed = previewFormationConfirmed;
						}
						if (squad == null)
						{
							squad = squad2;
						}
						Point point5 = squad.data->pos - ppos;
						Point point6 = squad2.data->pos - ppos;
						if (point5.SqrLength() > point6.SqrLength())
						{
							squad = squad2;
						}
						num4 += ((!flag3) ? data_formation.MaxWidth() : ((float)data_formation.MaxCols() * data_formation.min_spacing + num2));
						num6 += ((!flag3) ? data_formation.MinWidth() : ((float)data_formation.MinCols() * data_formation.min_spacing + num2));
						num5 += ((!flag3) ? data_formation.cur_width : ((float)data_formation.cols * data_formation.min_spacing + num2));
						point3 -= point6.GetNormalized() * (point5.SqrLength() / point6.SqrLength());
						point4 += squad2.data->pos;
						formation.CopyParamsFrom(data_formation);
						formation.SetCount(data_formation.count);
						formation.spacing = data_formation.spacing;
						num8++;
					}
				}
			}
		}
		if (this.selected_squads.Count == 0)
		{
			return;
		}
		num4 -= num2;
		num6 -= num2;
		num5 -= num2;
		point3 /= (float)num8;
		point4 /= (float)num8;
		if (flag3)
		{
			Point3 normal = passableArea.normal;
			Point point7 = new Point3(normal.x, 0f, normal.z);
			if (point2.Dot(point7) < 0f)
			{
				point7 *= -1f;
			}
			if (point7.x != 0f || point7.y != 0f)
			{
				this.formationFacing = point7;
				point2 = point7.Right(0f);
			}
			if (flag2)
			{
				num = num5;
			}
			else
			{
				num = num6;
			}
		}
		else if (flag2)
		{
			this.formationFacing = point3.GetNormalized();
			if (float.IsNaN(this.formationFacing.x) || float.IsNaN(this.formationFacing.y))
			{
				global::Squad squad3 = this.selected_squads[0];
				this.formationFacing = squad3.data->dir;
			}
			point2 = this.formationFacing.Right(0f);
			num = num5;
		}
		else if (dragIsForward)
		{
			this.formationFacing = point2;
			point2 = point2.Right(0f);
		}
		else
		{
			this.formationFacing = -point2.Right(0f);
		}
		float num9 = 100f;
		if (dragIsForward && num4 > num9)
		{
			float num10 = 0.3f;
			num /= num9;
			num = Mathf.Clamp01(num - num10);
			num = num6 + num * (num4 - num6);
		}
		num = Mathf.Clamp(num, num6, num4);
		for (int j = this.selected_squads.Count - 1; j >= 0; j--)
		{
			global::Squad squad4 = this.selected_squads[j];
			if (!this.IsPositionInvalidForSquad(squad4, flag, flag2, flag3) && !this.IgnoreSquadInFormation(squad4))
			{
				Formation previewFormation = squad4.previewFormation;
				if (!flag2 || flag3)
				{
					float num11 = (!flag3) ? previewFormation.MaxWidth() : ((float)previewFormation.MaxCols() * previewFormation.min_spacing);
					float num12 = (!flag3) ? previewFormation.MinWidth() : ((float)previewFormation.MinCols() * previewFormation.min_spacing);
					float width = Mathf.Max((num - num6) / (num4 - num6) * (num11 - num12) + num12, num12);
					previewFormation.SetWidth(width);
				}
				if (num7 < previewFormation.cur_height)
				{
					num7 = previewFormation.cur_height;
				}
			}
		}
		float num13 = num / 2f;
		float num14 = num / 2f;
		if (num8 == 1 && !flag3)
		{
			Formation previewFormation2 = squad.previewFormation;
			float num15;
			float num16;
			previewFormation2.GetClearanceRange(ppos, point2, num, out num15, out num16);
			Mathf.Clamp(num15, num6, num4);
			float num17;
			float num18;
			previewFormation2.GetClearanceRange(ppos, -point2.Right(0f), num7 / 2.5f, out num17, out num18);
			PPos position = ppos - point2.Right(0f) * (num17 - 0.1f);
			PPos position2 = ppos + point2.Right(0f) * (num18 - 0.1f);
			float item;
			float item2;
			previewFormation2.GetClearanceRange(position, point2, num, out item, out item2);
			float item3;
			float item4;
			previewFormation2.GetClearanceRange(position2, point2, num, out item3, out item4);
			num13 = num16;
			num14 = num15;
			List<float> list2 = new List<float>
			{
				num16,
				item2,
				item4
			};
			list2.Sort();
			List<float> list3 = new List<float>
			{
				num15,
				item,
				item3
			};
			list3.Sort();
			this.GetMinSidesRange(ref num13, ref num14, list2, list3);
			num = Mathf.Clamp(num14, num6, num4);
		}
		for (int k = this.selected_squads.Count - 1; k >= 0; k--)
		{
			if (this.selected_squads[k] == null)
			{
				this.selected_squads.RemoveAt(k);
			}
			else
			{
				global::Squad squad5 = this.selected_squads[k];
				if (!this.IsPositionInvalidForSquad(squad5, flag, flag2, flag3) && !this.IgnoreSquadInFormation(squad5))
				{
					list.Add(new BattleViewUI.SquadProj
					{
						squad = squad5,
						proj = (-point4 + squad5.data->pos).ProjLen(point2 * num)
					});
				}
			}
		}
		list.Sort((BattleViewUI.SquadProj p1, BattleViewUI.SquadProj p2) => p1.proj.CompareTo(p2.proj));
		float num19 = 0f;
		bool flag6 = false;
		bool flag7 = false;
		for (int l = 0; l < num8; l++)
		{
			global::Squad squad6 = list[l].squad;
			if (!(squad6 == null))
			{
				Formation previewFormation3 = squad6.previewFormation;
				Formation data_formation2 = squad6.data_formation;
				float num20 = data_formation2.cur_width;
				PPos ppos2 = ppos;
				PPos ppos3 = ppos2;
				PPos pt2 = this.formationFacing;
				if (!flag2 || flag3)
				{
					float num21 = (num - num6) / (num4 - num6);
					float num22 = (!flag3) ? data_formation2.MaxWidth() : ((float)data_formation2.MaxCols() * data_formation2.min_spacing);
					float num23 = (!flag3) ? data_formation2.MinWidth() : ((float)data_formation2.MinCols() * data_formation2.min_spacing);
					num20 = Mathf.Max(num21 * (num22 - num23) + num23, num23);
				}
				previewFormation3.SetWidth(num20);
				if (!flag4 || num8 > 1)
				{
					float num24 = (!flag4) ? 0f : (num / 2f);
					ppos3 += point2 * (num3 - num24 + num20 / 2f);
				}
				num3 += num20 + num2;
				PPos ppos4 = ppos3;
				PPos ppos5;
				Troops.path_data.BurstedTrace(ppos, ppos3, out ppos5, false, true, false, squad6.logic.movement.allowed_non_ladder_areas, squad6.logic.battle_side, squad6.logic.is_inside_walls_or_on_walls);
				ppos3.paID = ppos5.paID;
				float num25;
				float num26;
				previewFormation3.GetClearanceRange(ppos3, this.formationFacing, previewFormation3.cur_height / 2f, out num25, out num26);
				bool flag8 = previewFormation3.rows > 1 && num25 + num26 < previewFormation3.spacing.y * 2f;
				BattleMap.Get().IsInsideWall(ppos3);
				if (!Troops.path_data.BurstedTrace(ppos, ppos3, out ppos4, false, true, false, squad6.logic.movement.allowed_non_ladder_areas, squad6.logic.battle_side, squad6.logic.is_inside_walls_or_on_walls) || !Troops.path_data.IsPassable(ppos3) || flag8 || num19 != 0f || (ppos3.paID != 0 && (num13 + num14 < num6 || !passableArea.IsGround())))
				{
					if (ppos4.paID <= 0)
					{
						if (num19 == 0f)
						{
							for (int m = l; m >= 0; m--)
							{
								PathData.DataPointers path_data = Troops.path_data;
								bool flag9 = false;
								float cur_height = previewFormation3.cur_height;
								float y = previewFormation3.spacing.y;
								PPos ppos6 = ppos3;
								PPos ppos7 = ppos3;
								int num27 = 0;
								int num28 = 6;
								int num29 = 20;
								if (l == m)
								{
									ppos6 += this.formationFacing * y * 3f;
								}
								else
								{
									Formation previewFormation4 = list[m].squad.previewFormation;
									ppos7 = (ppos6 = previewFormation4.pos);
									ppos6 -= this.formationFacing * ((float)previewFormation4.lines.Count * previewFormation4.spacing.y / 2f + cur_height / 2f);
								}
								do
								{
									ppos6 -= this.formationFacing * y;
									PPos ppos8 = ppos6;
									for (int n = 0; n < num28; n++)
									{
										if (m != l || ppos7.Dist(ppos6) >= cur_height)
										{
											int num30 = (n % 2 == 0) ? 1 : -1;
											ppos8 = ppos6 + point2 * (float)num30 * (float)n * y;
										}
										bool flag10 = BattleMap.Get().IsInsideWall(ppos8);
										if (path_data.IsPassable(ppos8) && flag5 == flag10)
										{
											float num31;
											float num32;
											previewFormation3.GetClearanceRange(ppos8, this.formationFacing, cur_height / 2f, out num31, out num32);
											if (num32 < cur_height / 2f && num31 < cur_height / 2f)
											{
												ppos8 += this.formationFacing * (num31 - num32);
											}
											else if (ppos8.paID == 0)
											{
												if (num32 < cur_height / 2f)
												{
													ppos8 += this.formationFacing * (cur_height / 2f - (num32 - 0.1f));
												}
												if (num31 < cur_height / 2f)
												{
													ppos8 -= this.formationFacing * (cur_height / 2f - (num31 - 0.1f));
												}
											}
											previewFormation3.GetClearanceRange(ppos8, this.formationFacing, cur_height / 2f, out num31, out num32);
											flag10 = BattleMap.Get().IsInsideWall(ppos8);
											if (num31 >= cur_height / 2f && num32 >= cur_height / 2f && flag5 == flag10)
											{
												bool flag11 = false;
												if (l != m)
												{
													for (int num33 = 0; num33 < l; num33++)
													{
														global::Squad squad7 = list[num33].squad;
														Formation previewFormation5 = squad7.previewFormation;
														PPos pos = squad7.previewFormation.pos;
														if (ppos8.Dist(pos) < y * 3f)
														{
															flag11 = true;
															break;
														}
													}
												}
												if (!flag11)
												{
													ppos6 = ppos8;
													flag9 = true;
													break;
												}
											}
										}
									}
									num27++;
								}
								while (!flag9 && num27 < num29);
								if (flag9)
								{
									ppos4 = ppos6;
									break;
								}
							}
						}
					}
					else if (!passableArea.IsGround() && ppos4.paID > 0 && (l == 0 || num19 != 0f))
					{
						if (l == 0 && num8 > 1)
						{
							num19 = num4 / 2f;
							flag7 = true;
							if (previewFormation3.CalcClearance(ppos4 + point2 * 0.05f, this.formationFacing, previewFormation3.min_spacing * 2f) < 1f)
							{
								flag6 = true;
							}
						}
						if (flag7)
						{
							num19 -= (float)data_formation2.MaxCols() * data_formation2.min_spacing * 0.5f;
							int num34 = 0;
							PPos ppos9 = ppos2;
							bool flag12 = num19 > 0f;
							Point point8 = (flag6 ? (!flag12) : flag12) ? (-point2) : point2;
							PPos pt3 = ppos9;
							PPos ppos10 = ppos9;
							PPos pt4 = ppos10;
							for (int num35 = 0; num35 < num8 * 50; num35++)
							{
								Point pt5 = (num35 % 2 == 0) ? ((num19 > 0f) ? this.formationFacing : (-this.formationFacing)) : point8;
								PPos to = ppos10 + pt5 * previewFormation3.min_spacing;
								Troops.path_data.BurstedTrace(ppos10, to, out pt4, false, true, false, squad6.logic.movement.allowed_non_ladder_areas, squad6.logic.battle_side, squad6.logic.is_inside_walls_or_on_walls);
								ppos10 = pt4 - pt5 * 0.2f;
								if (pt4.Dist(pt3) < 0.05f)
								{
									num34++;
									if (num34 > 5)
									{
										break;
									}
								}
								else
								{
									num34 = 0;
								}
								if (pt4.Dist(ppos2) > Math.Abs(num19))
								{
									break;
								}
								pt3 = ppos10;
							}
							ppos4 = ppos10;
						}
						if (ppos4.paID > 0)
						{
							Point3 normal2 = Troops.path_data.GetPA(ppos4.paID - 1).normal;
							Point point9 = new Point3(normal2.x, 0f, normal2.z);
							if (this.formationFacing.Dot(point9) < 0f)
							{
								point9 *= -1f;
							}
							Point point10 = point9;
							if (point10.x != 0f || point10.y != 0f)
							{
								pt2 = point10;
							}
						}
					}
				}
				ppos3 = ppos4;
				previewFormation3.pos = ppos3;
				previewFormation3.Calc(previewFormation3.pos, pt2, true, false);
				if (flag7)
				{
					num19 -= (float)previewFormation3.MaxCols() * data_formation2.min_spacing * 0.5f + num2;
				}
			}
		}
		float num36 = 0f;
		for (int num37 = 0; num37 < num8; num37++)
		{
			global::Squad squad8 = list[num37].squad;
			if ((!flag2 || flag) && num8 > 0)
			{
				squad8.InDrag = true;
			}
			Formation previewFormation6 = squad8.previewFormation;
			if (!flag2 || flag)
			{
				int num38 = 0;
				int num39 = 0;
				Troops.Troop troop = squad8.data->FirstTroop;
				while (troop <= squad8.data->LastTroop)
				{
					if (troop.HasFlags(Troops.Troop.Flags.Dead) || troop.HasFlags(Troops.Troop.Flags.Destroyed))
					{
						troop.preview_position = 0;
					}
					else
					{
						PPos pt6 = previewFormation6.positions[num39];
						float y2 = pt6.Height(squad8.logic.game, global::Common.GetHeight(pt6, null, -1f, false), 0f);
						troop.preview_position = new float3(pt6.x, y2, pt6.y);
						num39++;
					}
					troop = ++troop;
					num38++;
				}
				float cur_height2 = previewFormation6.cur_height;
				if (num8 % 2 == 0)
				{
					if (cur_height2 > num36)
					{
						num36 = cur_height2;
					}
				}
				else if ((float)num37 == Mathf.Ceil((float)(num8 / 2)))
				{
					num36 = cur_height2;
				}
			}
		}
		if ((!flag2 || flag) && num8 > 0)
		{
			this.formationArrow.SetActive(true);
			Point pt7 = this.formationFacing;
			Vector3 vector = global::Common.SnapToTerrain(ppos + point2 * ((!flag4) ? (num / 2f) : 0f) + pt7 * (num36 / 2f + 5f), 0f, null, -1f, false);
			if (ppos.paID != 0)
			{
				PathData.PassableArea pa = Troops.path_data.GetPA(ppos.paID - 1);
				vector.y = Mathf.Max(vector.y, pa.GetHeight(vector));
			}
			this.formationArrow.transform.position = vector;
			this.formationArrow.transform.rotation = Quaternion.LookRotation(pt7);
			return;
		}
		this.formationArrow.SetActive(false);
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x0010EEC5 File Offset: 0x0010D0C5
	private bool IsPositionInvalidForSquad(global::Squad squad, bool is_long_press, bool smallDrag, bool is_on_wall)
	{
		return (squad.def.is_cavalry || (squad.def.is_siege_eq && (is_long_press || !smallDrag))) && is_on_wall;
	}

	// Token: 0x06001C96 RID: 7318 RVA: 0x0010EEF4 File Offset: 0x0010D0F4
	public override bool OverrideEdgeScroll()
	{
		return this.dragging;
	}

	// Token: 0x06001C97 RID: 7319 RVA: 0x0010EEFC File Offset: 0x0010D0FC
	private void OffsetPosition(ref PPos position, PPos dirNormalized, float width, float left_range, float right_range)
	{
		PPos from = position;
		if (left_range > right_range)
		{
			float f = width * 0.5f - right_range;
			position -= dirNormalized * f;
		}
		else if (left_range < right_range)
		{
			float f = width * 0.5f - left_range;
			position += dirNormalized * f;
		}
		PPos ppos;
		Troops.path_data.BurstedTrace(from, position, out ppos, false, true, false, PathData.PassableArea.Type.All, -1, false);
		position.paID = ppos.paID;
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x0010EF90 File Offset: 0x0010D190
	private void GetMinSidesRange(ref float squad_left, ref float squad_right, List<float> squad_lefts, List<float> squad_rights)
	{
		for (int i = 0; i < squad_lefts.Count; i++)
		{
			if (squad_lefts[i] >= 0f)
			{
				squad_left = squad_lefts[i];
				break;
			}
		}
		for (int j = 0; j < squad_rights.Count; j++)
		{
			if (squad_rights[j] >= 0f)
			{
				squad_right = squad_rights[j];
				return;
			}
		}
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x0010EFF4 File Offset: 0x0010D1F4
	private void DebugDrawRect(PPos rectPosition, PPos dir, float baseWidth, float backwards_range, float forward_range, float left_range, float right_range)
	{
		PPos pt = rectPosition + dir.Right(0f) * backwards_range - dir * left_range;
		PPos pt2 = rectPosition + -dir.Right(0f) * forward_range + dir * right_range;
		Debug.DrawLine(pt, rectPosition, Color.green, 0.1f);
		Debug.DrawRay(rectPosition + dir.Right(0f) * 0.5f, dir * (baseWidth / 2f), new Color(0.8f, 0.8f, 0f), 0.1f);
		Debug.DrawRay(rectPosition + dir.Right(0f) * 0.5f, -dir * (baseWidth / 2f), new Color(0.8f, 0.8f, 0f), 0.1f);
		Debug.DrawRay(pt, -dir.Right(0f) * (backwards_range + forward_range), new Color(0.3f, 1f, 0f), 0.1f);
		Debug.DrawRay(pt2, dir.Right(0f) * (backwards_range + forward_range), new Color(0.3f, 1f, 0f), 0.1f);
		Debug.DrawRay(pt2, -dir * (left_range + right_range), new Color(0.3f, 1f, 0f), 0.1f);
		Debug.DrawRay(pt, dir * (left_range + right_range), new Color(0.3f, 1f, 0f), 0.1f);
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x0010F204 File Offset: 0x0010D404
	private unsafe void MoveFormation()
	{
		if (this.selected_squads == null || this.selected_squads.Count == 0)
		{
			return;
		}
		this.formationArrow.SetActive(false);
		int num = 0;
		Point point = Point.Zero;
		for (int i = this.selected_squads.Count - 1; i >= 0; i--)
		{
			if (this.selected_squads[i] == null)
			{
				this.selected_squads.RemoveAt(i);
			}
			else
			{
				global::Squad squad = this.selected_squads[i];
				if (squad == null || squad.data_formation.count == 0)
				{
					this.selected_squads.RemoveAt(i);
				}
				else if (!this.IgnoreSquadInFormation(squad))
				{
					num++;
					point += squad.logic.VisualPosition();
					Formation data_formation = squad.data_formation;
					Formation formation = squad.previewFormation;
					if (formation == null)
					{
						formation = FormationPool.Get(squad.logic.formation.def, squad.logic);
						FormationPool.Return(ref squad.previewFormation);
						squad.previewFormation = formation;
						Formation previewFormationConfirmed = FormationPool.Get(squad.logic.formation.def, squad.logic);
						FormationPool.Return(ref squad.previewFormationConfirmed);
						squad.previewFormationConfirmed = previewFormationConfirmed;
					}
					formation.CopyParamsFrom(data_formation);
					formation.SetCount(data_formation.count);
				}
			}
		}
		if (num == 0)
		{
			return;
		}
		point /= (float)num;
		Point point2 = this.startDragPos;
		Camera mainCamera = CameraController.MainCamera;
		Vector3 b = mainCamera.WorldToScreenPoint(this.picked_terrain_point);
		float angle = Vector3.SignedAngle(mainCamera.WorldToScreenPoint(global::Common.SnapToTerrain(point2, 0f, null, -1f, false)) - b, -Vector3.right, Vector3.forward);
		for (int j = this.selected_squads.Count - 1; j >= 0; j--)
		{
			global::Squad squad2 = this.selected_squads[j];
			if (!this.IgnoreSquadInFormation(squad2))
			{
				squad2.InDrag = true;
				PPos ppos = squad2.logic.VisualPosition() - point;
				Formation data_formation2 = squad2.data_formation;
				Formation previewFormation = squad2.previewFormation;
				previewFormation.pos = point2 + ppos.GetRotated(angle);
				PPos pos = previewFormation.pos;
				Troops.path_data.BurstedTrace(point2, previewFormation.pos, out pos, false, true, false, squad2.logic.movement.allowed_non_ladder_areas, squad2.logic.battle_side, squad2.logic.is_inside_walls_or_on_walls);
				previewFormation.Calc(pos, data_formation2.dir.GetRotated(angle), true, false);
				int num2 = 0;
				int num3 = 0;
				Troops.Troop troop = squad2.data->FirstTroop;
				while (troop <= squad2.data->LastTroop)
				{
					if (troop.HasFlags(Troops.Troop.Flags.Dead))
					{
						troop.preview_position = 0;
					}
					else
					{
						PPos pt = previewFormation.positions[num3];
						float y = pt.Height(squad2.logic.game, global::Common.GetHeight(pt, null, -1f, false), 0f);
						troop.preview_position = new float3(pt.x, y, pt.y);
						num3++;
					}
					troop = ++troop;
					num2++;
				}
			}
		}
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x0010F585 File Offset: 0x0010D785
	public override void OnMouseMove(Vector2 pts, int btn)
	{
		if (this.dragging)
		{
			if (KeyBindings.GetBind("alternate_drag_formation", false))
			{
				if (btn == 0)
				{
					this.MoveFormation();
				}
				else
				{
					this.LineFormation(true);
				}
			}
			else
			{
				this.LineFormation(false);
			}
		}
		base.OnMouseMove(pts, btn);
	}

	// Token: 0x06001C9C RID: 7324 RVA: 0x0010F5C0 File Offset: 0x0010D7C0
	public void OnLeftClick()
	{
		if (!this.dragging)
		{
			return;
		}
		if (this.m_SelectedObjects != null && this.m_SelectedObjects.Count > 0)
		{
			for (int i = 0; i < this.m_SelectedObjects.Count; i++)
			{
				global::Squad component = this.m_SelectedObjects[i].GetComponent<global::Squad>();
				if (((component != null) ? component.logic : null) != null)
				{
					component.MoveTo(component.previewFormation.pos, component.previewFormation.pos.paID, component.previewFormation.dir, true, false);
				}
			}
		}
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0010F658 File Offset: 0x0010D858
	public override void OnRightClick()
	{
		List<Logic.Squad> selectedSquads = this.GetSelectedSquads();
		if (this.dragging && selectedSquads != null && selectedSquads.Count > 0)
		{
			for (int i = 0; i < selectedSquads.Count; i++)
			{
				Logic.Squad squad = selectedSquads[i];
				global::Squad squad2 = ((squad != null) ? squad.visuals : null) as global::Squad;
				if (((squad2 != null) ? squad2.logic : null) != null)
				{
					if (squad2.kingdom == base.kingdom)
					{
						Logic.Squad logic = squad2.logic;
						bool flag;
						if (logic == null)
						{
							flag = (null != null);
						}
						else
						{
							BattleSimulation.Squad simulation = logic.simulation;
							if (simulation == null)
							{
								flag = (null != null);
							}
							else
							{
								Logic.Unit unit = simulation.unit;
								if (unit == null)
								{
									flag = (null != null);
								}
								else
								{
									Logic.Army army = unit.army;
									flag = (((army != null) ? army.mercenary : null) != null);
								}
							}
						}
						if (!flag)
						{
							goto IL_AC;
						}
					}
					if (!BaseUI.CanControlAI())
					{
						goto IL_10F;
					}
					IL_AC:
					if (squad2.previewFormation != null)
					{
						squad2.logic.formation.CopyParamsFrom(squad2.previewFormation);
						squad2.previewFormationConfirmed.Copy(squad2.previewFormation);
						squad2.MoveTo(squad2.previewFormation.pos, squad2.previewFormation.pos.paID, squad2.previewFormation.dir, true, false);
					}
				}
				IL_10F:;
			}
			base.SpawnClickReticle();
		}
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x0010F78C File Offset: 0x0010D98C
	public void Show(bool shown, bool fadeTween = false)
	{
		BattleViewUI.<>c__DisplayClass87_0 CS$<>8__locals1 = new BattleViewUI.<>c__DisplayClass87_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.shown = shown;
		if (this.tCanvas == null)
		{
			this.tCanvas = base.transform.Find("Canvas");
		}
		if (this.tCanvas == null)
		{
			return;
		}
		if (!fadeTween)
		{
			this.tCanvas.gameObject.SetActive(CS$<>8__locals1.shown);
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(this.tCanvas.gameObject, "id_BattleHud", true, true);
		TweenCanvasGroupAplha tween = base.GetComponent<TweenCanvasGroupAplha>();
		if (tween == null)
		{
			gameObject.AddComponent<TweenCanvasGroupAplha>();
		}
		if (tween != null)
		{
			if (CS$<>8__locals1.shown)
			{
				this.tCanvas.gameObject.SetActive(CS$<>8__locals1.shown);
				tween.ignoreTimeScale = true;
				tween.from = 0f;
				tween.to = 1f;
				tween.duration = 0.2f;
			}
			else
			{
				tween.ignoreTimeScale = true;
				tween.from = 1f;
				tween.to = 0f;
				tween.duration = 0.2f;
				tween.onFinished.AddListener(delegate()
				{
					CS$<>8__locals1.<>4__this.tCanvas.gameObject.SetActive(CS$<>8__locals1.shown);
					tween.onFinished.RemoveAllListeners();
				});
			}
			tween.ResetToBeginning();
			tween.PlayForward();
			return;
		}
		this.tCanvas.gameObject.SetActive(CS$<>8__locals1.shown);
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x0010F944 File Offset: 0x0010DB44
	public override Color GetStanceColor(Logic.Object obj, bool primary = true)
	{
		Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(base.kingdom);
		if (kingdom == null || obj == null)
		{
			if (!primary)
			{
				return this.selectionSettings.secondaryNeutralColor;
			}
			return this.selectionSettings.neutralColor;
		}
		else
		{
			Logic.Kingdom kingdom2 = obj.GetKingdom();
			if (kingdom2 == kingdom)
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryFriendColor;
				}
				return this.selectionSettings.friendColor;
			}
			else
			{
				Logic.Kingdom attacker_kingdom = BattleMap.battle.attacker_kingdom;
				Logic.Army attacker_support = BattleMap.battle.attacker_support;
				Logic.Kingdom kingdom3 = (attacker_support != null) ? attacker_support.GetKingdom() : null;
				int num;
				if (kingdom == attacker_kingdom || kingdom == kingdom3)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
				int num2;
				if (kingdom2 == attacker_kingdom || kingdom2 == kingdom3)
				{
					num2 = 0;
				}
				else
				{
					num2 = 1;
				}
				if (num2 != num)
				{
					if (!primary)
					{
						return this.selectionSettings.secondaryEnemyColor;
					}
					return this.selectionSettings.enemyColor;
				}
				else
				{
					if (!primary)
					{
						return this.selectionSettings.secondaryNeutralColor;
					}
					return this.selectionSettings.neutralColor;
				}
			}
		}
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x0010FA30 File Offset: 0x0010DC30
	public Color GetStanceColorMinimap(Logic.Object obj, bool primary = true)
	{
		Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(base.kingdom);
		if (kingdom == null || obj == null)
		{
			if (!primary)
			{
				return this.selectionSettings.secondaryNeutralColorMinimap;
			}
			return this.selectionSettings.neutralColorMinimap;
		}
		else
		{
			Logic.Kingdom kingdom2 = obj.GetKingdom();
			if (kingdom2 == kingdom)
			{
				if (!primary)
				{
					return this.selectionSettings.secondaryFriendColorMinimap;
				}
				return this.selectionSettings.friendColorMinimap;
			}
			else
			{
				Logic.Kingdom attacker_kingdom = BattleMap.battle.attacker_kingdom;
				Logic.Army attacker_support = BattleMap.battle.attacker_support;
				Logic.Kingdom kingdom3 = (attacker_support != null) ? attacker_support.GetKingdom() : null;
				int num;
				if (kingdom == attacker_kingdom || kingdom == kingdom3)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
				int num2;
				if (kingdom2 == attacker_kingdom || kingdom2 == kingdom3)
				{
					num2 = 0;
				}
				else
				{
					num2 = 1;
				}
				if (num2 != num)
				{
					if (!primary)
					{
						return this.selectionSettings.secondaryEnemyColorMinimap;
					}
					return this.selectionSettings.enemyColorMinimap;
				}
				else
				{
					if (!primary)
					{
						return this.selectionSettings.secondaryNeutralColorMinimap;
					}
					return this.selectionSettings.neutralColorMinimap;
				}
			}
		}
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x0010FB1C File Offset: 0x0010DD1C
	public BattleViewUI.BattleViewStance GetStanceType(Logic.Object obj)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom.IsEnemy(obj))
		{
			Logic.Kingdom kingdom2 = obj.GetKingdom();
			War war = kingdom.FindWarWith(obj.GetKingdom());
			if (war != null && !war.IsLeader(kingdom2))
			{
				return BattleViewUI.BattleViewStance.EnemySupporter;
			}
			return BattleViewUI.BattleViewStance.Enemy;
		}
		else
		{
			if (kingdom.IsOwnStance(obj))
			{
				return BattleViewUI.BattleViewStance.Own;
			}
			if (kingdom.IsAlly(obj))
			{
				return BattleViewUI.BattleViewStance.Supporter;
			}
			return BattleViewUI.BattleViewStance.Neutral;
		}
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x0010FB74 File Offset: 0x0010DD74
	public BattleSimulation.Squad ExtractLeaderSimulationLogic(Logic.Character leader)
	{
		Logic.Army army = (leader != null) ? leader.GetArmy() : null;
		Logic.Battle battle = (army != null) ? army.battle : null;
		if (battle == null)
		{
			return null;
		}
		int battle_side = army.battle_side;
		List<Logic.Squad> list = battle.squads.Get(army.battle_side);
		for (int i = 0; i < list.Count; i++)
		{
			Logic.Squad squad = list[i];
			if (squad != null && !squad.IsDefeated() && squad.def.type == Logic.Unit.Type.Noble && squad.simulation.army == army)
			{
				return squad.simulation;
			}
		}
		return null;
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x0010FC08 File Offset: 0x0010DE08
	public void AddToControlGroup(global::Squad m_obj, int group_index, BaseUI.ControlGroup group = null)
	{
		if (m_obj == null)
		{
			return;
		}
		if (group == null && !this.groups.TryGetValue(group_index, out group))
		{
			return;
		}
		if (m_obj.control_groups.Contains(group_index))
		{
			return;
		}
		m_obj.control_groups.Add(group_index);
		group.squads.Add(m_obj.logic);
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x0010FC60 File Offset: 0x0010DE60
	protected override void UpdateControlGroupInput()
	{
		for (KeyCode keyCode = KeyCode.Alpha1; keyCode <= KeyCode.Alpha9; keyCode++)
		{
			if (UICommon.GetKeyUp(keyCode, UICommon.ModifierKey.None, UICommon.ModifierKey.All))
			{
				int num = keyCode - KeyCode.Alpha0;
				BaseUI.ControlGroup controlGroup;
				if (!this.groups.TryGetValue(num, out controlGroup))
				{
					controlGroup = new BaseUI.ControlGroup();
					this.groups[num] = controlGroup;
				}
				if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
				{
					for (int i = 0; i < Troops.squads.Length; i++)
					{
						global::Squad squad = Troops.squads[i];
						if (squad != null && squad.Selected && squad.logic != null && !BaseUI.LogicKingdom().IsAllyOrOwn(squad.logic.GetKingdom()))
						{
							return;
						}
					}
					for (int j = controlGroup.squads.Count - 1; j >= 0; j--)
					{
						global::Squad squad2 = controlGroup.squads[j].visuals as global::Squad;
						if (squad2 != null)
						{
							squad2.control_groups.Remove(num);
						}
						controlGroup.squads.RemoveAt(j);
					}
					for (int k = 0; k < Troops.squads.Length; k++)
					{
						global::Squad squad3 = Troops.squads[k];
						if (squad3 != null && squad3.Selected)
						{
							this.AddToControlGroup(squad3, num, controlGroup);
						}
					}
					this.SelectedGroupId = num;
					UIBattleViewArmyContainer armyWindow = this.GetArmyWindow();
					if (armyWindow != null)
					{
						armyWindow.Refresh();
					}
					this.UpdateContexSelection();
				}
				else if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
				{
					for (int l = 0; l < Troops.squads.Length; l++)
					{
						global::Squad squad4 = Troops.squads[l];
						if (squad4 != null && squad4.Selected && squad4.logic != null && !BaseUI.LogicKingdom().IsAllyOrOwn(squad4.logic.GetKingdom()))
						{
							return;
						}
					}
					for (int m = 0; m < Troops.squads.Length; m++)
					{
						global::Squad squad5 = Troops.squads[m];
						if (squad5 != null && squad5.Selected)
						{
							this.AddToControlGroup(squad5, num, controlGroup);
						}
					}
					this.SelectedGroupId = num;
					UIBattleViewArmyContainer armyWindow2 = this.GetArmyWindow();
					if (armyWindow2 != null)
					{
						armyWindow2.Refresh();
					}
					this.UpdateContexSelection();
				}
				if (controlGroup.squads.Count > 0)
				{
					this.SelectControlGroup(controlGroup, num);
				}
				this.DoubleTapNumberCheck(keyCode);
				if (this.isDoubleTappedNumber)
				{
					this.CenterCameraOnSelection();
				}
			}
		}
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x0010FED9 File Offset: 0x0010E0D9
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

	// Token: 0x06001CA6 RID: 7334 RVA: 0x0010FF18 File Offset: 0x0010E118
	public void SelectControlGroup(int id)
	{
		if (!this.groups.ContainsKey(id))
		{
			return;
		}
		this.SelectControlGroup(this.groups[id], id);
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x0010FF3C File Offset: 0x0010E13C
	private void SelectControlGroup(BaseUI.ControlGroup group, int group_index)
	{
		if (group.squads.Count > 0)
		{
			this.SelectedGroupId = group_index;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < group.squads.Count; i++)
			{
				Logic.Squad squad = group.squads[i];
				if (((squad != null) ? squad.visuals : null) != null)
				{
					global::Squad squad2 = squad.visuals as global::Squad;
					list.Add(squad2.gameObject);
				}
			}
			this.SelectObjects(list, false, true);
		}
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x0010FFB8 File Offset: 0x0010E1B8
	public void RemoveSquadFromGroup(Logic.Squad squad, int groupIndex)
	{
		BaseUI.ControlGroup controlGroup;
		if (groupIndex != -1 && this.groups.TryGetValue(groupIndex, out controlGroup))
		{
			controlGroup.squads.Remove(squad);
			UIBattleViewArmyContainer armyWindowContainer = this.m_ArmyWindowContainer;
			if (armyWindowContainer == null)
			{
				return;
			}
			armyWindowContainer.UpdateSelection();
		}
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x0010FFF6 File Offset: 0x0010E1F6
	public void CenterCameraOnSelection()
	{
		base.LookAt(BaseUI.SelLO(), false);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x00110004 File Offset: 0x0010E204
	public override void DelSelected()
	{
		if (!Game.CheckCheatLevel(Game.CheatLevel.High, "del", true))
		{
			return;
		}
		if (this.m_SelectedObjects.Count > 0)
		{
			for (int i = this.m_SelectedObjects.Count - 1; i >= 0; i--)
			{
				global::Squad component = this.m_SelectedObjects[i].GetComponent<global::Squad>();
				if (!(component == null))
				{
					component.Selected = false;
					this.m_SelectedObjects.RemoveAt(i);
					component.DeleteObject();
				}
			}
			return;
		}
		base.DelSelected();
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x00110084 File Offset: 0x0010E284
	public void Deselect(global::Squad squad)
	{
		if (squad == null)
		{
			return;
		}
		for (int i = this.m_SelectedObjects.Count - 1; i >= 0; i--)
		{
			if (this.m_SelectedObjects[i] == squad.gameObject)
			{
				this.m_SelectedObjects.RemoveAt(i);
			}
		}
		squad.Selected = false;
		squad.PreSelected = false;
		this.UpdateContexSelection();
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x001100EC File Offset: 0x0010E2EC
	private void ArmiesVisibilityButton_OnClick(BSGButton btn)
	{
		if (this.m_ArmyWindowContainer != null)
		{
			this.m_ArmyWindowContainer.ToggleArmyWindowsVisibility();
		}
		if (this.m_HideArmiesButton != null)
		{
			this.m_HideArmiesButton.gameObject.SetActive(!this.m_HideArmiesButton.gameObject.activeSelf);
		}
		if (this.m_ShowArmiesButton != null)
		{
			this.m_ShowArmiesButton.gameObject.SetActive(!this.m_ShowArmiesButton.gameObject.activeSelf);
		}
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x00110174 File Offset: 0x0010E374
	private void BattleOverviewVisibilityButton_OnClick(BSGButton btn)
	{
		if (this.m_CapturePoints != null)
		{
			this.m_CapturePoints.gameObject.SetActive(!this.m_CapturePoints.gameObject.activeSelf);
		}
		if (this.m_BattleOverviewCompact != null)
		{
			this.m_BattleOverviewCompact.ToggleVisibility();
		}
		if (this.m_BattleOverview != null)
		{
			this.m_BattleOverview.ToggleVisibility();
		}
		if (this.m_ShowBattleOverviewButton != null)
		{
			this.m_ShowBattleOverviewButton.gameObject.SetActive(!this.m_ShowBattleOverviewButton.gameObject.activeSelf);
		}
		if (this.m_HideBattleOverviewButton != null)
		{
			this.m_HideBattleOverviewButton.gameObject.SetActive(!this.m_HideBattleOverviewButton.gameObject.activeSelf);
		}
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x001102AC File Offset: 0x0010E4AC
	[CompilerGenerated]
	internal static RelationUtils.Stance <SelectSquads>g__GetSquadsSide|51_0(List<GameObject> squads, out bool areSidesMixed)
	{
		RelationUtils.Stance stance = RelationUtils.Stance.None;
		bool flag = false;
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		for (int i = 0; i < squads.Count; i++)
		{
			if (!(squads[i] == null))
			{
				global::Squad component = squads[i].GetComponent<global::Squad>();
				if (!(component == null) && component.logic != null)
				{
					RelationUtils.Stance stance2 = RelationUtils.Stance.Alliance;
					if (component.logic.IsOwnStance(obj) && !component.logic.IsMercenary())
					{
						stance2 = RelationUtils.Stance.Own;
					}
					else if (component.logic.IsEnemy(obj))
					{
						stance2 = RelationUtils.Stance.War;
					}
					if ((stance & stance2) == RelationUtils.Stance.None)
					{
						if (stance != RelationUtils.Stance.None)
						{
							flag = true;
						}
						stance |= stance2;
					}
				}
			}
		}
		areSidesMixed = flag;
		return stance;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x00110358 File Offset: 0x0010E558
	[CompilerGenerated]
	internal static void <SelectSquads>g__RemoveSquadsNotMatchingTheSide|51_1(List<GameObject> squads, RelationUtils.Stance allowedStance)
	{
		Logic.Kingdom obj = BaseUI.LogicKingdom();
		for (int i = 0; i < squads.Count; i++)
		{
			if (!(squads[i] == null))
			{
				global::Squad component = squads[i].GetComponent<global::Squad>();
				if (!(component == null) && component.logic != null)
				{
					RelationUtils.Stance stance = RelationUtils.Stance.Alliance;
					if (component.logic.IsOwnStance(obj))
					{
						stance = RelationUtils.Stance.Own;
					}
					else if (component.logic.IsEnemy(obj))
					{
						stance = RelationUtils.Stance.War;
					}
					if (stance != allowedStance)
					{
						component.Selected = false;
						squads.RemoveAt(i);
						i--;
					}
				}
			}
		}
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x001103E8 File Offset: 0x0010E5E8
	[CompilerGenerated]
	internal static List<GameObject> <SelectSquads>g__GetObjectsEligableForSelection|51_2(List<GameObject> squads)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < squads.Count; i++)
		{
			if (!(squads[i] == null))
			{
				global::Squad component = squads[i].GetComponent<global::Squad>();
				if (component != null && ((component != null) ? component.logic : null) != null && component.logic.IsValid() && !component.logic.IsDefeated())
				{
					if (component.GetMainSquadID() != -1)
					{
						global::Squad squad = Troops.GetSquad(component.GetMainSquadID());
						if (squad != null && !list.Contains(squad.gameObject))
						{
							list.Add(squad.gameObject);
						}
					}
					else
					{
						list.Add(squads[i]);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x001104AC File Offset: 0x0010E6AC
	[CompilerGenerated]
	private void <SelectSquads>g__ClearCurrentSelection|51_3()
	{
		foreach (GameObject gameObject in this.m_SelectedObjects)
		{
			if (!(gameObject == null))
			{
				global::Squad component = gameObject.GetComponent<global::Squad>();
				if (component != null)
				{
					component.Selected = false;
				}
			}
		}
		this.m_SelectedObjects.Clear();
	}

	// Token: 0x04001298 RID: 4760
	private Point formationFacing;

	// Token: 0x04001299 RID: 4761
	private GameObject formationArrow;

	// Token: 0x0400129A RID: 4762
	public global::CapturePoint[] picked_capture_point = new global::CapturePoint[2];

	// Token: 0x0400129B RID: 4763
	public global::Squad[] picked_squads = new global::Squad[2];

	// Token: 0x0400129C RID: 4764
	public global::Fortification[] picked_fortifications = new global::Fortification[2];

	// Token: 0x0400129E RID: 4766
	public List<GameObject> m_SelectedObjects = new List<GameObject>();

	// Token: 0x0400129F RID: 4767
	public List<global::Squad> selected_squads = new List<global::Squad>();

	// Token: 0x040012A1 RID: 4769
	public BattleViewSquadActions SquadActions = new BattleViewSquadActions();

	// Token: 0x040012A2 RID: 4770
	private GameObject ladder_preview_go;

	// Token: 0x040012A3 RID: 4771
	private bool ladder_previewed;

	// Token: 0x040012A4 RID: 4772
	private bool ladder_was_previewed;

	// Token: 0x040012A5 RID: 4773
	private int ladder_PA;

	// Token: 0x040012A6 RID: 4774
	private int ladder_PA_prev;

	// Token: 0x040012A7 RID: 4775
	[UIFieldTarget("id_ShowArmiesButton")]
	private BSGButton m_ShowArmiesButton;

	// Token: 0x040012A8 RID: 4776
	[UIFieldTarget("id_HideArmiesButton")]
	private BSGButton m_HideArmiesButton;

	// Token: 0x040012A9 RID: 4777
	[UIFieldTarget("id_ShowBattleOverviewButton")]
	private BSGButton m_ShowBattleOverviewButton;

	// Token: 0x040012AA RID: 4778
	[UIFieldTarget("id_HideBattleOverviewButton")]
	private BSGButton m_HideBattleOverviewButton;

	// Token: 0x040012AB RID: 4779
	[UIFieldTarget("id_BattleOverview")]
	private UIBattleViewOverview m_BattleOverview;

	// Token: 0x040012AC RID: 4780
	[UIFieldTarget("id_BattleOverviewCompact")]
	private UIBattleViewOverview m_BattleOverviewCompact;

	// Token: 0x040012AD RID: 4781
	private SquadsTargetPreview m_SquadsTargetPreview;

	// Token: 0x040012AE RID: 4782
	private global::Squad m_SelectedSquad;

	// Token: 0x040012AF RID: 4783
	private UIBattleViewArmyContainer m_ArmyWindowContainer;

	// Token: 0x040012B0 RID: 4784
	private UICapturePointsBar m_CapturePoints;

	// Token: 0x040012B2 RID: 4786
	private PPos startDragPos;

	// Token: 0x040012B3 RID: 4787
	private KeyCode lastPressedNumber;

	// Token: 0x040012B4 RID: 4788
	private float timeLastPressedNumber = float.PositiveInfinity;

	// Token: 0x040012B5 RID: 4789
	private bool isDoubleTappedNumber;

	// Token: 0x0200071D RID: 1821
	public enum BattleViewStance
	{
		// Token: 0x04003862 RID: 14434
		Own,
		// Token: 0x04003863 RID: 14435
		Supporter,
		// Token: 0x04003864 RID: 14436
		Neutral,
		// Token: 0x04003865 RID: 14437
		EnemySupporter,
		// Token: 0x04003866 RID: 14438
		Enemy
	}

	// Token: 0x0200071E RID: 1822
	private struct SquadProj
	{
		// Token: 0x04003867 RID: 14439
		public float proj;

		// Token: 0x04003868 RID: 14440
		public global::Squad squad;
	}
}
