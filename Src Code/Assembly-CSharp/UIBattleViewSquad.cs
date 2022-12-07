using System;
using Logic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001D5 RID: 469
public class UIBattleViewSquad : WorldToScreenObject, IListener
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06001BB3 RID: 7091 RVA: 0x00107DB0 File Offset: 0x00105FB0
	// (remove) Token: 0x06001BB4 RID: 7092 RVA: 0x00107DE8 File Offset: 0x00105FE8
	public event Action<UIBattleViewSquad, PointerEventData> onClick;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06001BB5 RID: 7093 RVA: 0x00107E20 File Offset: 0x00106020
	// (remove) Token: 0x06001BB6 RID: 7094 RVA: 0x00107E58 File Offset: 0x00106058
	public event Action<UIBattleViewSquad, PointerEventData> onSquadTypeIconClick;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06001BB7 RID: 7095 RVA: 0x00107E90 File Offset: 0x00106090
	// (remove) Token: 0x06001BB8 RID: 7096 RVA: 0x00107EC8 File Offset: 0x001060C8
	public event Action<UIBattleViewSquad> onRemoved;

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x00107EFD File Offset: 0x001060FD
	// (set) Token: 0x06001BBA RID: 7098 RVA: 0x00107F05 File Offset: 0x00106105
	public BattleSimulation.Squad SimulationSquadLogic { get; private set; }

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06001BBB RID: 7099 RVA: 0x00107F0E File Offset: 0x0010610E
	// (set) Token: 0x06001BBC RID: 7100 RVA: 0x00107F16 File Offset: 0x00106116
	public Logic.Squad SquadLogic { get; private set; }

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06001BBD RID: 7101 RVA: 0x00107F1F File Offset: 0x0010611F
	// (set) Token: 0x06001BBE RID: 7102 RVA: 0x00107F27 File Offset: 0x00106127
	public global::Squad Visuals { get; private set; }

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06001BBF RID: 7103 RVA: 0x00107F30 File Offset: 0x00106130
	public static DT.Field icon_settings
	{
		get
		{
			if (UIBattleViewSquad._icon_settings == null)
			{
				UIBattleViewSquad._icon_settings = global::Defs.GetDefField("SquadStatusIconSettings", null);
			}
			return UIBattleViewSquad._icon_settings;
		}
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x00107F4E File Offset: 0x0010614E
	private void Update()
	{
		this.CheckTooltipToggleChanged(false);
		this.Refresh();
		if (base.Clamped)
		{
			this.RotateArrowTowardsSquad();
		}
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00107F6B File Offset: 0x0010616B
	protected override bool AddToGlobalListOnAwake()
	{
		return !this.is_static_ui;
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00107F76 File Offset: 0x00106176
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.Clear();
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x00107F84 File Offset: 0x00106184
	public void SetData(BattleSimulation.Squad squad)
	{
		this.Clear();
		this.SimulationSquadLogic = squad;
		this.Initialize();
		this.Refresh();
		this.CheckTooltipToggleChanged(true);
	}

	// Token: 0x06001BC4 RID: 7108 RVA: 0x00107FA6 File Offset: 0x001061A6
	public void UpdateAsCompact(bool isDisplayedInWorldView)
	{
		this.m_isDisplayedInWorldView = isDisplayedInWorldView;
	}

	// Token: 0x06001BC5 RID: 7109 RVA: 0x00107FAF File Offset: 0x001061AF
	public void LeaderCallback(Sprite sp, Logic.Character c)
	{
		if (((this != null) ? this.m_SquadIcon : null) == null)
		{
			return;
		}
		this.m_SquadIcon.overrideSprite = sp;
	}

	// Token: 0x06001BC6 RID: 7110 RVA: 0x00107FD2 File Offset: 0x001061D2
	public void UpdateSelection()
	{
		if (this.SimulationSquadLogic == null || this.SquadLogic == null || this.SquadLogic.visuals == null)
		{
			return;
		}
		if (this.Visuals != null)
		{
			this.Select(this.Visuals.Selected);
		}
	}

	// Token: 0x06001BC7 RID: 7111 RVA: 0x00108011 File Offset: 0x00106211
	public void Select(bool selected)
	{
		this.m_isSelected = selected;
		this.Refresh();
	}

	// Token: 0x06001BC8 RID: 7112 RVA: 0x00108020 File Offset: 0x00106220
	public void SetInteraction(bool isInteractable)
	{
		this.m_isInteractable = isInteractable;
	}

	// Token: 0x06001BC9 RID: 7113 RVA: 0x0010802C File Offset: 0x0010622C
	public void Clear()
	{
		if (this.SimulationSquadLogic != null)
		{
			this.SquadLogic.DelListener(this);
		}
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.Clear();
		}
		if (this.m_SquadTypeIcon != null)
		{
			this.m_SquadTypeIcon.OnSquadIconClicked -= this.OnClick;
			this.m_SquadTypeIcon.OnSquadIconClicked -= this.Squad_OnSquadTypeIconClicked;
		}
		this.Visuals = null;
		this.SimulationSquadLogic = null;
		this.SquadLogic = null;
		this.onClick = null;
		this.onRemoved = null;
		this.m_isSelected = false;
	}

	// Token: 0x06001BCA RID: 7114 RVA: 0x001080D0 File Offset: 0x001062D0
	public override DT.Field GetCursorFieldKey(DT.Field field)
	{
		Logic.Squad squad = BattleViewUI.Get().selected_logic_obj as Logic.Squad;
		Logic.Squad squadLogic = this.SquadLogic;
		return BattleViewUI.SquadToSquadCursorField(squad, squadLogic, field);
	}

	// Token: 0x06001BCB RID: 7115 RVA: 0x001080FA File Offset: 0x001062FA
	public override bool IsVisible()
	{
		return this.m_isVisible && this.NameplatesEnabled;
	}

	// Token: 0x06001BCC RID: 7116 RVA: 0x0010810C File Offset: 0x0010630C
	public void ForceHideHealthBar(bool isFilterOn)
	{
		UISquadStatusBar squadStatusBar = this.m_SquadStatusBar;
		if (squadStatusBar != null)
		{
			squadStatusBar.UpdateHealthVisibilityFilter(isFilterOn);
		}
		this.UpdateSquadTypeIconPosition();
		this.UpdateSquadSelectionEffects();
	}

	// Token: 0x06001BCD RID: 7117 RVA: 0x0010812C File Offset: 0x0010632C
	public void CheckTooltipToggleChanged(bool force = false)
	{
		if (this.Visuals == null)
		{
			return;
		}
		bool nameplateTooltipFilter = this.Visuals.m_NameplateTooltipFilter;
		bool flag = this.last_tooltip_enabled == 1;
		if (nameplateTooltipFilter == flag && this.last_tooltip_enabled != -1 && !force)
		{
			return;
		}
		this.last_tooltip_enabled = (nameplateTooltipFilter ? 1 : 0);
		this.ToggleTooltip(nameplateTooltipFilter);
	}

	// Token: 0x06001BCE RID: 7118 RVA: 0x00108184 File Offset: 0x00106384
	public void ToggleTooltip(bool isFilterOn)
	{
		if (this.m_isPinnable)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadBVTooltip", new Vars(this.SimulationSquadLogic));
			return;
		}
		if (isFilterOn)
		{
			Tooltip.Get(base.gameObject, true).SetDef("SquadBVTooltipUnPinable", new Vars(this.SimulationSquadLogic));
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, false);
		if (tooltip != null)
		{
			global::Common.DestroyObj(tooltip);
		}
	}

	// Token: 0x06001BCF RID: 7119 RVA: 0x001081FC File Offset: 0x001063FC
	public void ForceHideStaminaBar(bool isFilterOn)
	{
		UISquadStatusBar squadStatusBar = this.m_SquadStatusBar;
		if (squadStatusBar != null)
		{
			squadStatusBar.UpdateStaminaVisibilityFilter(isFilterOn);
		}
		UISquadTypeIcon squadTypeIcon = this.m_SquadTypeIcon;
		if (squadTypeIcon != null)
		{
			squadTypeIcon.UpdateStaminaVisibilityFilter(isFilterOn);
		}
		this.UpdateSquadTypeIconPosition();
		this.UpdateSquadSelectionEffects();
	}

	// Token: 0x06001BD0 RID: 7120 RVA: 0x0010822E File Offset: 0x0010642E
	public void ForceHideMoraleBar(bool isFilterOn)
	{
		UISquadStatusBar squadStatusBar = this.m_SquadStatusBar;
		if (squadStatusBar != null)
		{
			squadStatusBar.gameObject.SetActive(isFilterOn);
		}
		this.UpdateSquadTypeIconPosition();
	}

	// Token: 0x06001BD1 RID: 7121 RVA: 0x0010824D File Offset: 0x0010644D
	public void ForceHideLevelIndicatior(bool isFilterOn)
	{
		UISquadTypeIcon squadTypeIcon = this.m_SquadTypeIcon;
		if (squadTypeIcon == null)
		{
			return;
		}
		squadTypeIcon.UpdateLevelVisibilityFilter(isFilterOn);
	}

	// Token: 0x06001BD2 RID: 7122 RVA: 0x00108260 File Offset: 0x00106460
	public void ForceHideStatuesContainer(bool isFilterOn)
	{
		UISquadStatuses squadStatuses = this.m_SquadStatuses;
		if (squadStatuses == null)
		{
			return;
		}
		squadStatuses.gameObject.SetActive(isFilterOn);
	}

	// Token: 0x06001BD3 RID: 7123 RVA: 0x00108278 File Offset: 0x00106478
	public override void UpdateVisibilityFromView(ViewMode.AllowedFigures allowedFigures)
	{
		bool flag = this.IsVisible();
		this.m_isVisibleFromView = ((allowedFigures & this.allowedType) > ViewMode.AllowedFigures.None);
		this.m_isVisible = this.m_isVisibleFromView;
		base.UpdateVisibility();
		if (this.IsVisible() && !flag)
		{
			this.Refresh();
		}
	}

	// Token: 0x06001BD4 RID: 7124 RVA: 0x001082C0 File Offset: 0x001064C0
	public override void UpdateVisibilityFromObject(bool visible_from_object)
	{
		bool flag = this.IsVisible();
		this.m_isVisibleFromObject = visible_from_object;
		this.m_isVisible = this.m_isVisibleFromView;
		base.UpdateVisibility();
		if (this.IsVisible() && !flag)
		{
			this.Refresh();
		}
	}

	// Token: 0x06001BD5 RID: 7125 RVA: 0x00108300 File Offset: 0x00106500
	protected override void OnClampChange()
	{
		base.OnClampChange();
		if (this.Visuals == null)
		{
			return;
		}
		this.Visuals.RefreshClampedBars();
		if (this.m_OffScreenArrow != null)
		{
			this.m_OffScreenArrow.gameObject.SetActive(base.Clamped && !this.m_isPinnable);
		}
		if (this.m_SquadTypeIcon != null)
		{
			this.m_SquadTypeIcon.OnParentClampedChange(base.Clamped);
		}
		UISquadStanceTint uisquadStanceTint = this.stanceTint;
		if (uisquadStanceTint != null)
		{
			uisquadStanceTint.UpdateImageRecolor(this.SquadLogic, base.gameObject, base.Clamped, false);
		}
		this.UpdateSquadSelectionEffects();
	}

	// Token: 0x06001BD6 RID: 7126 RVA: 0x001083A8 File Offset: 0x001065A8
	private unsafe void RotateArrowTowardsSquad()
	{
		if (this.m_OffScreenArrow != null && this.Visuals != null && this.Visuals.GetID() != -1)
		{
			GameCamera gameCamera = CameraController.GameCamera;
			Camera cam = (gameCamera != null) ? gameCamera.Camera : null;
			Point pt = WorldToScreenObject.WorldToScreen(this.Visuals.data->banner_pos, cam);
			Point point = new Point(base.transform.position.x, base.transform.position.y) - pt;
			this.m_OffScreenArrow.rotation = Quaternion.Euler(0f, 0f, point.Heading() + 90f);
		}
	}

	// Token: 0x06001BD7 RID: 7127 RVA: 0x0010846C File Offset: 0x0010666C
	public void OnMessage(object obj, string message, object param)
	{
		if (this == null)
		{
			return;
		}
		if (base.gameObject == null)
		{
			return;
		}
		if (message == "defeated")
		{
			this.Refresh();
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			if (this.SimulationSquadLogic.spawned_in_bv)
			{
				Action<UIBattleViewSquad> action = this.onRemoved;
				if (action != null)
				{
					action(this);
				}
				this.Clear();
			}
			return;
		}
		if (message == "troop_died")
		{
			this.Refresh();
			return;
		}
		if (message == "started_packing")
		{
			if (this.m_isDisplayedInWorldView)
			{
				this.m_SquadStatusBar.ShowDeploymentBar(true);
				this.UpdateSquadTypeIconPosition();
			}
			return;
		}
		if (!(message == "finished_packing"))
		{
			return;
		}
		if (this.m_isDisplayedInWorldView)
		{
			this.m_SquadStatusBar.ShowDeploymentBar(false);
			this.UpdateSquadTypeIconPosition();
		}
	}

	// Token: 0x06001BD8 RID: 7128 RVA: 0x0010854A File Offset: 0x0010674A
	public override void OnClick(PointerEventData e)
	{
		if (!this.m_isInteractable)
		{
			return;
		}
		base.OnClick(e);
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
		this.Refresh();
	}

	// Token: 0x06001BD9 RID: 7129 RVA: 0x00108577 File Offset: 0x00106777
	public override void OnRightClick(PointerEventData e)
	{
		if (!this.m_isInteractable)
		{
			return;
		}
		base.OnRightClick(e);
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
		this.Refresh();
	}

	// Token: 0x06001BDA RID: 7130 RVA: 0x001085A4 File Offset: 0x001067A4
	public override void OnDoubleClick(PointerEventData e)
	{
		if (!this.m_isInteractable)
		{
			return;
		}
		base.OnClick(e);
		if (this.onClick != null)
		{
			this.onClick(this, e);
		}
	}

	// Token: 0x06001BDB RID: 7131 RVA: 0x001085CC File Offset: 0x001067CC
	private void Initialize()
	{
		UICommon.FindComponents(this, false);
		this.InitializeWindowDef();
		this.ShowAsEmpty(this.SimulationSquadLogic == null || this.SimulationSquadLogic.squad == null);
		this.m_isPinnable = !this.m_isDisplayedInWorldView;
		if (this.stanceTint == null)
		{
			this.stanceTint = new UISquadStanceTint();
			this.stanceTint.is_pinnable = this.m_isPinnable;
		}
		if (this.SimulationSquadLogic == null)
		{
			return;
		}
		this.SquadLogic = this.SimulationSquadLogic.squad;
		if (this.SquadLogic == null)
		{
			return;
		}
		if (this.SquadLogic.visuals != null)
		{
			this.Visuals = (this.SquadLogic.visuals as global::Squad);
			this.clickSound = this.SquadLogic.def.SelectSound;
			this._g = this.Visuals.gameObject;
		}
		this.UpdateSquadIcon();
		this.UpdateKingdomIcon();
		this.UpdateCrown();
		this.RefreshDefField();
		if (this.m_SquadTypeIcon != null)
		{
			this.m_SquadTypeIcon.ForceToShowUnitTypeIcon(!this.m_isDisplayedInWorldView);
			this.m_SquadTypeIcon.SetObject(this.SquadLogic, null);
			this.m_SquadTypeIcon.OnSquadIconClicked += this.Squad_OnSquadTypeIconClicked;
			this.m_squadTypeIconPosition = this.m_SquadTypeIcon.transform.localPosition;
		}
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.SetSquad(this.SquadLogic);
		}
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.SetSquad(this.SquadLogic);
			if (this.m_isDisplayedInWorldView)
			{
				this.m_SquadStatusBar.ShowSalvoBar(false);
			}
			else
			{
				this.m_SquadStatusBar.ShowSalvoBar(this.SquadLogic.def.is_ranged);
			}
			if (this.m_isDisplayedInWorldView)
			{
				this.m_SquadStatusBar.ShowDeploymentBar(false);
			}
		}
		if (this.m_SquadStatuses != null)
		{
			this.m_SquadStatuses.SetSquad(this.SquadLogic);
		}
		if (this.SquadLogic != null)
		{
			this.SquadLogic.AddListener(this);
		}
		if (this.m_Buff != null)
		{
			this.m_Buff.SetDef(null, null);
			if (this.m_Buff.m_StatusBackground != null)
			{
				this.m_StatusBackgroundPosition = this.m_Buff.m_StatusBackground.transform.localPosition;
			}
		}
		this.UpdateSquadTypeIconPosition();
		this.ShowGlow(false);
		if (this.m_OffScreenArrow != null)
		{
			this.m_OffScreenArrow.gameObject.SetActive(false);
		}
		UISquadStanceTint uisquadStanceTint = this.stanceTint;
		if (uisquadStanceTint == null)
		{
			return;
		}
		uisquadStanceTint.UpdateImageRecolor(this.SquadLogic, base.gameObject, base.Clamped, false);
	}

	// Token: 0x06001BDC RID: 7132 RVA: 0x0010886C File Offset: 0x00106A6C
	public override void RefreshDefField()
	{
		if (base.InitDef())
		{
			DT.Field defField = global::Defs.GetDefField(this.DefKey(false), null);
			this.LoadDefs(defField);
			base.RefreshDefField();
		}
	}

	// Token: 0x06001BDD RID: 7133 RVA: 0x0010889C File Offset: 0x00106A9C
	protected override string DefKey(bool refresh = false)
	{
		if (refresh)
		{
			if (this.SquadLogic.def.is_siege_eq)
			{
				this.def_key = "SquadSiegeStatusBar";
			}
			else
			{
				this.def_key = "SquadStatusBar";
			}
		}
		return base.DefKey(refresh);
	}

	// Token: 0x06001BDE RID: 7134 RVA: 0x001088D4 File Offset: 0x00106AD4
	public unsafe override Vector3 GetDesiredPosition(bool is_pv)
	{
		global::Squad visuals = this.Visuals;
		if (visuals == null || this.SimulationSquadLogic == null || visuals.squad_banner == null || WorldToScreenObject.def_params == null || this.SquadLogic == null)
		{
			return Vector3.zero;
		}
		float3 banner_pos = visuals.data->banner_pos;
		Vector3 vector = new Vector3(banner_pos.x, banner_pos.y, banner_pos.z);
		float terrainHeight = global::Common.GetTerrainHeight(vector, null, false);
		vector.y = Mathf.Max(terrainHeight, vector.y);
		Vector3 vector2 = vector - visuals.squad_banner.transform.position;
		WorldToScreenObject.WorldToScreenScaleParams worldToScreenScaleParams = WorldToScreenObject.def_params[this.DefKey(false)];
		Vector3 b = worldToScreenScaleParams.offset_3d;
		if (this.SimulationSquadLogic.def.name.Equals("Trebuchet") && !this.SquadLogic.is_packed)
		{
			b = worldToScreenScaleParams.offset_3d_alternative;
		}
		if (Mathf.Abs(vector2.x) > 10f || Mathf.Abs(vector2.z) > 10f)
		{
			return vector + b;
		}
		return Vector3.Lerp(visuals.squad_banner.transform.position, vector, UnityEngine.Time.deltaTime * 4f) + b;
	}

	// Token: 0x06001BDF RID: 7135 RVA: 0x00108A1A File Offset: 0x00106C1A
	private void InitializeWindowDef()
	{
		if (this.m_windowDef == null)
		{
			this.m_windowDef = global::Defs.GetDefField("UIBattleViewSquad", null);
		}
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x00108A38 File Offset: 0x00106C38
	private void ShowAsEmpty(bool isEmpty)
	{
		if (this.m_SquadBorder != null)
		{
			this.m_SquadBorder.gameObject.SetActive(!isEmpty);
		}
		if (this.m_EmptyBackground != null)
		{
			this.m_EmptyBackground.SetActive(isEmpty);
		}
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.gameObject.SetActive(!isEmpty);
		}
		if (this.m_SquadTypeIcon != null)
		{
			this.m_SquadTypeIcon.gameObject.SetActive(!isEmpty);
		}
		if (this.m_SquadStatusBar != null)
		{
			this.m_SquadStatusBar.gameObject.SetActive(!isEmpty);
			this.m_SquadStatusBar.ShowHealthBar(!isEmpty);
			this.m_SquadStatusBar.ShowStaminaBar(false);
		}
		if (this.m_SquadStatuses != null)
		{
			this.m_SquadStatuses.gameObject.SetActive(!isEmpty);
		}
		if (this.m_Crown != null)
		{
			this.m_Crown.gameObject.SetActive(!isEmpty);
		}
		if (this.m_KingdomIcon != null)
		{
			this.m_KingdomIcon.gameObject.SetActive(!isEmpty);
		}
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x00108B68 File Offset: 0x00106D68
	private void Refresh()
	{
		UIBattleViewSquad.SelectionState selectionState = this.m_selectionState;
		this.UpdateState();
		this.UpdateSquadDamageEffect();
		this.UpdateStateIcons();
		if (this.SquadLogic != null)
		{
			Logic.Kingdom obj = BaseUI.LogicKingdom();
			this.is_dark_icon = (!this.SquadLogic.IsOwnStance(obj) && !this.SquadLogic.IsEnemy(obj));
		}
		if (selectionState != this.m_selectionState)
		{
			this.UpdateStatusBarVisibility();
			this.UpdateSquadSelectionEffects();
		}
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x00108BD8 File Offset: 0x00106DD8
	private void UpdateStateIcons()
	{
		if (this.m_FleeingIcon != null)
		{
			this.m_FleeingIcon.gameObject.SetActive(this.m_selectionState == UIBattleViewSquad.SelectionState.Flee);
		}
		if (this.m_Buff != null)
		{
			SquadBuff squadBuff = null;
			Logic.Squad squadLogic = this.SquadLogic;
			if (((squadLogic != null) ? squadLogic.buffs : null) != null)
			{
				float num = -1f;
				for (int i = 0; i < this.SquadLogic.buffs.Count; i++)
				{
					SquadBuff squadBuff2 = this.SquadLogic.buffs[i];
					if (squadBuff2.enabled)
					{
						float priority = squadBuff2.def.priority;
						if (priority > num)
						{
							squadBuff = squadBuff2;
							num = priority;
						}
					}
				}
			}
			bool flag = !base.Clamped && squadBuff != null;
			this.m_Buff.gameObject.SetActive(flag);
			if (this.last_priority_buff != squadBuff || this.last_show_buff != flag)
			{
				this.last_show_buff = flag;
				this.last_priority_buff = squadBuff;
				if (flag)
				{
					this.m_Buff.SetDef(squadBuff, UIBattleViewSquad.icon_settings.FindChild(squadBuff.field.key, null, true, true, true, '.'));
				}
				this.UpdateSquadSelectionEffects();
			}
		}
		if (this.m_SquadIconDead != null)
		{
			this.m_SquadIconDead.gameObject.SetActive(this.m_selectionState == UIBattleViewSquad.SelectionState.Destroyed);
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x00108D24 File Offset: 0x00106F24
	private void UpdateSquadIcon()
	{
		if (this.m_SquadIcon != null)
		{
			Logic.Army army = this.SimulationSquadLogic.army;
			Logic.Character character = (army != null) ? army.leader : null;
			if (this.SimulationSquadLogic.def.type == Logic.Unit.Type.Noble && character != null && character.IsValid())
			{
				this.m_SquadIcon.overrideSprite = global::Character.GetIcon(character, this.m_SquadIcon.GetComponent<RectTransform>().rect.width);
				return;
			}
			this.m_SquadIcon.overrideSprite = global::Defs.GetObj<Sprite>(this.SimulationSquadLogic.def.field, "icon", null);
		}
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x00108DC8 File Offset: 0x00106FC8
	private void UpdateStatusBarVisibility()
	{
		if (this.m_selectionState == UIBattleViewSquad.SelectionState.Destroyed || this.m_selectionState == UIBattleViewSquad.SelectionState.Flee)
		{
			this.m_SquadStatusBar.ShowHealthBar(false);
			this.m_SquadStatusBar.ShowStaminaBar(false);
			this.m_SquadStatusBar.ShowSalvoBar(false);
			this.UpdateSquadTypeIconPosition();
		}
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x00108E08 File Offset: 0x00107008
	private void UpdateKingdomIcon()
	{
		if (this.m_KingdomIconContainer == null || this.SimulationSquadLogic == null)
		{
			if (this.m_KingdomIcon != null)
			{
				this.m_KingdomIcon.gameObject.SetActive(false);
			}
			return;
		}
		Logic.Object @object = this.SimulationSquadLogic.army;
		if (@object == null)
		{
			Garrison garrison = this.SimulationSquadLogic.garrison;
			@object = ((garrison != null) ? garrison.settlement : null);
		}
		if (this.m_KingdomIcon == null && ((@object != null) ? @object.GetKingdom() : null) != null)
		{
			this.m_KingdomIcon = ObjectIcon.GetIcon(@object.GetKingdom(), null, this.m_KingdomIconContainer).GetComponent<UIKingdomIcon>();
		}
		this.m_KingdomIcon.SetObject(@object, null);
	}

	// Token: 0x06001BE6 RID: 7142 RVA: 0x00108EC0 File Offset: 0x001070C0
	private void UpdateCrown()
	{
		if (this.m_Crown == null)
		{
			return;
		}
		bool flag = false;
		if (this.SimulationSquadLogic != null && this.SimulationSquadLogic.def.type != Logic.Unit.Type.Noble)
		{
			Logic.Army army = this.SimulationSquadLogic.army;
			Logic.Character character = (army != null) ? army.leader : null;
			if (character != null)
			{
				flag = (character.title == "King");
				flag |= (character.title == "Queen");
				flag |= (character.title == "Prince");
				flag |= (character.title == "Prince");
				flag |= (character.title == "Princess");
				flag |= character.IsPope();
				flag |= character.IsPatriarch();
				UICrown crown = this.m_Crown;
				Logic.Army army2 = this.SimulationSquadLogic.army;
				crown.SetData((army2 != null) ? army2.leader : null);
			}
		}
		this.m_Crown.gameObject.SetActive(flag);
	}

	// Token: 0x06001BE7 RID: 7143 RVA: 0x00108FC0 File Offset: 0x001071C0
	private void UpdateSquadSelectionEffects()
	{
		if (this.m_SquadIcon != null)
		{
			this.m_SquadIcon.color = global::Defs.GetColor(this.m_stateDef, this.is_dark_icon ? "icon_color_dark" : "icon_color", null);
		}
		if (this.m_selectionState == UIBattleViewSquad.SelectionState.Destroyed || this.m_selectionState == UIBattleViewSquad.SelectionState.Flee || this.m_selectionState == UIBattleViewSquad.SelectionState.Default)
		{
			this.ShowGlow(false);
			this.ShowHooverBackground(false);
			this.ShowSelectionFrame(false);
			if (this.m_selectionState != UIBattleViewSquad.SelectionState.Default)
			{
				this.HideDamageEffect();
				return;
			}
		}
		else if (this.m_selectionState == UIBattleViewSquad.SelectionState.Over)
		{
			this.ShowGlow(true);
			this.ShowHooverBackground(true);
			if (!this.m_isSelected)
			{
				this.ShowSelectionFrame(false);
				return;
			}
		}
		else if (this.m_selectionState == UIBattleViewSquad.SelectionState.Selected)
		{
			this.ShowGlow(false);
			this.ShowHooverBackground(false);
			this.ShowSelectionFrame(true);
		}
	}

	// Token: 0x06001BE8 RID: 7144 RVA: 0x0010908C File Offset: 0x0010728C
	private void UpdateSquadDamageEffect()
	{
		if (this.Visuals != null && this.Visuals.TroopDied && !this.SimulationSquadLogic.IsDefeated() && this.m_DamageEffect != null && !this.m_DamageEffect.gameObject.activeInHierarchy)
		{
			this.m_DamageEffect.gameObject.SetActive(true);
			this.m_DamageEffect.onFinished.AddListener(new UnityAction(this.DamageEffect_OnFinished));
			this.m_DamageEffect.PlayForward();
		}
	}

	// Token: 0x06001BE9 RID: 7145 RVA: 0x0010911C File Offset: 0x0010731C
	private void UpdateSquadTypeIconPosition()
	{
		if (!this.m_isDisplayedInWorldView || this.m_SquadStatusBar == null || this.m_SquadTypeIcon == null)
		{
			return;
		}
		UISquadStatus buff = this.m_Buff;
		if (((buff != null) ? buff.m_StatusBackground : null) != null)
		{
			if (this.m_SquadStatusBar.HealthVisible())
			{
				this.m_Buff.m_StatusBackground.transform.localPosition = this.m_StatusBackgroundPosition;
			}
			else if (this.m_statusBackgroundPositionSmall != null)
			{
				this.m_Buff.m_StatusBackground.transform.localPosition = this.m_statusBackgroundPositionSmall.localPosition;
			}
		}
		if (!this.m_SquadStatusBar.IsContentVisible())
		{
			this.m_SquadTypeIcon.transform.localPosition = Vector3.zero;
			if (this.m_Glow_Circle_Full != null)
			{
				this.m_Glow_Circle_Full.transform.localPosition = Vector3.zero;
			}
			if (this.m_Glow_Circle_Small != null)
			{
				this.m_Glow_Circle_Small.transform.localPosition = Vector3.zero;
			}
		}
		else
		{
			this.m_SquadTypeIcon.transform.localPosition = this.m_squadTypeIconPosition;
			if (this.m_Glow_Circle_Full != null)
			{
				this.m_Glow_Circle_Full.transform.localPosition = this.m_squadTypeIconPosition;
			}
			if (this.m_Glow_Circle_Small != null)
			{
				this.m_Glow_Circle_Small.transform.localPosition = this.m_squadTypeIconPosition;
			}
		}
		if (this.m_OffScreenArrow != null && base.Clamped && this.m_SquadTypeIcon != null)
		{
			this.m_OffScreenArrow.transform.position = this.m_SquadTypeIcon.transform.position;
		}
	}

	// Token: 0x06001BEA RID: 7146 RVA: 0x001092CC File Offset: 0x001074CC
	private void DamageEffect_OnFinished()
	{
		this.HideDamageEffect();
	}

	// Token: 0x06001BEB RID: 7147 RVA: 0x001092D4 File Offset: 0x001074D4
	private void ShowSelectionFrame(bool show)
	{
		if (this.m_SelectionFrame == null)
		{
			return;
		}
		this.m_SelectionFrame.gameObject.SetActive(show && this.m_isInteractable);
		if (show)
		{
			this.m_SelectionFrame.PlayForward();
			return;
		}
		this.m_SelectionFrame.Stop();
	}

	// Token: 0x06001BEC RID: 7148 RVA: 0x00109328 File Offset: 0x00107528
	private void ShowHooverBackground(bool show)
	{
		if (this.m_SquadBorder == null || !this.m_isInteractable)
		{
			return;
		}
		if (show)
		{
			string key = string.IsNullOrEmpty(this.variant) ? "portretBackgroundHoover" : ("portretBackgroundHoover." + this.variant);
			this.m_SquadBorder.overrideSprite = global::Defs.GetObj<Sprite>(this.m_windowDef, key, null);
			return;
		}
		this.m_SquadBorder.overrideSprite = null;
	}

	// Token: 0x06001BED RID: 7149 RVA: 0x0010939C File Offset: 0x0010759C
	private void ShowGlow(bool show)
	{
		bool flag = this.m_SquadTypeIcon.StaminaVisible() || this.m_SquadTypeIcon.DeploymentVisible();
		if (this.Visuals.Selected)
		{
			show = true;
		}
		if (this.m_Glow_Circle_Full != null)
		{
			this.m_Glow_Circle_Full.gameObject.SetActive(show && this.m_SquadTypeIcon != null && flag);
		}
		if (this.m_Glow_Circle_Small != null)
		{
			this.m_Glow_Circle_Small.gameObject.SetActive(show && (this.m_SquadTypeIcon == null || !flag));
		}
		if (this.m_Glow_Rectangle_Full != null)
		{
			this.m_Glow_Rectangle_Full.gameObject.SetActive(show && this.m_SquadStatusBar != null && this.m_SquadStatusBar.gameObject.activeSelf && !this.SquadLogic.def.is_siege_eq && this.m_SquadStatusBar.HealthVisible());
		}
		if (this.m_Glow_Rectangle_Small != null)
		{
			if (this.SquadLogic.def.is_siege_eq)
			{
				this.m_Glow_Rectangle_Small.gameObject.SetActive(show && this.m_SquadStatusBar != null && this.m_SquadStatusBar.gameObject.activeSelf && this.m_SquadStatusBar.HealthVisible());
			}
			else
			{
				this.m_Glow_Rectangle_Small.gameObject.SetActive(show && this.m_SquadStatusBar != null && this.m_SquadStatusBar.gameObject.activeSelf && !this.m_SquadStatusBar.HealthVisible());
			}
		}
		if (this.m_Glow_Buff != null)
		{
			this.m_Glow_Buff.gameObject.SetActive(show && this.m_Buff.gameObject.activeInHierarchy && this.m_SquadStatusBar != null && this.m_SquadStatusBar.gameObject.activeSelf && this.m_SquadStatusBar.HealthVisible());
		}
		if (this.m_Glow_Buff_Small != null)
		{
			this.m_Glow_Buff_Small.gameObject.SetActive(show && this.m_Buff.gameObject.activeInHierarchy && this.m_SquadStatusBar != null && this.m_SquadStatusBar.gameObject.activeSelf && !this.m_SquadStatusBar.HealthVisible());
		}
		this.UpdateGlowColor();
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x00109618 File Offset: 0x00107818
	private void UpdateGlowColor()
	{
		if (this.m_isPinnable)
		{
			return;
		}
		Color white = Color.white;
		if (!this.Visuals.Selected)
		{
			white = new Color(1f, 1f, 1f, 0.5f);
		}
		if (this.m_Glow_Circle_Full != null)
		{
			this.m_Glow_Circle_Full.color = white;
		}
		if (this.m_Glow_Circle_Small != null)
		{
			this.m_Glow_Circle_Small.color = white;
		}
		if (this.m_Glow_Rectangle_Full != null)
		{
			this.m_Glow_Rectangle_Full.color = white;
		}
		if (this.m_Glow_Rectangle_Small != null)
		{
			this.m_Glow_Rectangle_Small.color = white;
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x001096C4 File Offset: 0x001078C4
	private void HideDamageEffect()
	{
		if (this.m_DamageEffect == null)
		{
			return;
		}
		this.m_DamageEffect.ResetToBeginning();
		this.m_DamageEffect.onFinished.RemoveListener(new UnityAction(this.DamageEffect_OnFinished));
		this.m_DamageEffect.gameObject.SetActive(false);
	}

	// Token: 0x06001BF0 RID: 7152 RVA: 0x00109718 File Offset: 0x00107918
	private void UpdateState()
	{
		UIBattleViewSquad.SelectionState state = this.DecideState();
		this.SetState(state);
	}

	// Token: 0x06001BF1 RID: 7153 RVA: 0x00109734 File Offset: 0x00107934
	private void SetState(UIBattleViewSquad.SelectionState state)
	{
		if (this.m_selectionState == state && this.m_stateDef != null)
		{
			return;
		}
		this.m_selectionState = state;
		this.InitializeWindowDef();
		if (this.m_windowDef != null)
		{
			this.m_stateDef = this.m_windowDef.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.m_stateDef == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.m_stateDef = this.m_windowDef.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.m_stateDef = null;
		}
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x001097D0 File Offset: 0x001079D0
	private UIBattleViewSquad.SelectionState DecideState()
	{
		if (this.SquadLogic == null && this.SimulationSquadLogic == null)
		{
			return UIBattleViewSquad.SelectionState.Empty;
		}
		if (this.SquadLogic != null && this.SimulationSquadLogic == null)
		{
			return UIBattleViewSquad.SelectionState.Destroyed;
		}
		this.Visuals.MouseOvered = false;
		BattleSimulation.Squad simulationSquadLogic = this.SimulationSquadLogic;
		if (simulationSquadLogic == null || simulationSquadLogic.IsDefeated())
		{
			if (this.SimulationSquadLogic.state == BattleSimulation.Squad.State.Fled)
			{
				return UIBattleViewSquad.SelectionState.Flee;
			}
			return UIBattleViewSquad.SelectionState.Destroyed;
		}
		else
		{
			if (this.m_isSelected || this.Visuals.Selected)
			{
				return UIBattleViewSquad.SelectionState.Selected;
			}
			if (this.mouse_in || (this.Visuals != null && this.Visuals.Highlighted))
			{
				this.Visuals.MouseOvered = true;
				return UIBattleViewSquad.SelectionState.Over;
			}
			return UIBattleViewSquad.SelectionState.Default;
		}
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x0010987C File Offset: 0x00107A7C
	private void Squad_OnSquadTypeIconClicked(PointerEventData e)
	{
		Action<UIBattleViewSquad, PointerEventData> action = this.onSquadTypeIconClick;
		if (action == null)
		{
			return;
		}
		action(this, e);
	}

	// Token: 0x04001205 RID: 4613
	public bool NameplatesEnabled = true;

	// Token: 0x04001206 RID: 4614
	public bool is_static_ui = true;

	// Token: 0x04001207 RID: 4615
	[UIFieldTarget("id_Glow_Circle_Full")]
	private Image m_Glow_Circle_Full;

	// Token: 0x04001208 RID: 4616
	[UIFieldTarget("id_Glow_Circle_Small")]
	private Image m_Glow_Circle_Small;

	// Token: 0x04001209 RID: 4617
	[UIFieldTarget("id_Glow_Rectangle_Full")]
	private Image m_Glow_Rectangle_Full;

	// Token: 0x0400120A RID: 4618
	[UIFieldTarget("id_Glow_Rectangle_Small")]
	private Image m_Glow_Rectangle_Small;

	// Token: 0x0400120B RID: 4619
	[UIFieldTarget("id_Glow_Buff")]
	private Image m_Glow_Buff;

	// Token: 0x0400120C RID: 4620
	[UIFieldTarget("id_Glow_Buff_Small")]
	private Image m_Glow_Buff_Small;

	// Token: 0x0400120D RID: 4621
	[UIFieldTarget("id_SquadIcon")]
	private Image m_SquadIcon;

	// Token: 0x0400120E RID: 4622
	[UIFieldTarget("id_Border")]
	private Image m_SquadBorder;

	// Token: 0x0400120F RID: 4623
	[UIFieldTarget("id_SquadIconDead")]
	private Image m_SquadIconDead;

	// Token: 0x04001210 RID: 4624
	[UIFieldTarget("id_Buff")]
	private UISquadStatus m_Buff;

	// Token: 0x04001211 RID: 4625
	[UIFieldTarget("id_StatusBackgroundPositionSmall")]
	private Transform m_statusBackgroundPositionSmall;

	// Token: 0x04001212 RID: 4626
	[UIFieldTarget("id_FleeingIcon")]
	private Image m_FleeingIcon;

	// Token: 0x04001213 RID: 4627
	[UIFieldTarget("id_SquadStatuses")]
	private UISquadStatuses m_SquadStatuses;

	// Token: 0x04001214 RID: 4628
	[UIFieldTarget("id_SquadStatusBar")]
	private UISquadStatusBar m_SquadStatusBar;

	// Token: 0x04001215 RID: 4629
	[UIFieldTarget("id_SquadTypeIcon")]
	private UISquadTypeIcon m_SquadTypeIcon;

	// Token: 0x04001216 RID: 4630
	[UIFieldTarget("id_KingdomIconContainer")]
	private RectTransform m_KingdomIconContainer;

	// Token: 0x04001217 RID: 4631
	[UIFieldTarget("id_KingdomIcon")]
	private UIKingdomIcon m_KingdomIcon;

	// Token: 0x04001218 RID: 4632
	[UIFieldTarget("id_Crown")]
	private UICrown m_Crown;

	// Token: 0x04001219 RID: 4633
	[UIFieldTarget("id_ControlGroup")]
	private TextMeshProUGUI m_ControlGroup;

	// Token: 0x0400121A RID: 4634
	[UIFieldTarget("id_SelectionFrame")]
	private TweenAlpha m_SelectionFrame;

	// Token: 0x0400121B RID: 4635
	[UIFieldTarget("id_DamageEffect")]
	private TweenAlpha m_DamageEffect;

	// Token: 0x0400121C RID: 4636
	[UIFieldTarget("id_EmptyBackground")]
	private GameObject m_EmptyBackground;

	// Token: 0x0400121D RID: 4637
	[UIFieldTarget("id_OffScreenArrow")]
	private RectTransform m_OffScreenArrow;

	// Token: 0x0400121E RID: 4638
	[SerializeField]
	private string variant;

	// Token: 0x0400121F RID: 4639
	private DT.Field m_windowDef;

	// Token: 0x04001220 RID: 4640
	private DT.Field m_stateDef;

	// Token: 0x04001221 RID: 4641
	private UISquadStanceTint stanceTint;

	// Token: 0x04001222 RID: 4642
	private static DT.Field _icon_settings;

	// Token: 0x04001223 RID: 4643
	private UIBattleViewSquad.SelectionState m_selectionState = UIBattleViewSquad.SelectionState.Uninitialized;

	// Token: 0x04001224 RID: 4644
	private bool m_isSelected;

	// Token: 0x04001225 RID: 4645
	private bool m_isDisplayedInWorldView;

	// Token: 0x04001226 RID: 4646
	private bool m_isInteractable = true;

	// Token: 0x04001227 RID: 4647
	private bool m_isVisible;

	// Token: 0x04001228 RID: 4648
	private bool m_isVisibleFromObject;

	// Token: 0x04001229 RID: 4649
	private bool m_isVisibleFromView = true;

	// Token: 0x0400122A RID: 4650
	private bool m_isPinnable;

	// Token: 0x0400122B RID: 4651
	private Vector3 m_squadTypeIconPosition;

	// Token: 0x0400122C RID: 4652
	private Vector3 m_StatusBackgroundPosition;

	// Token: 0x0400122D RID: 4653
	private bool is_dark_icon;

	// Token: 0x0400122E RID: 4654
	private SquadBuff last_priority_buff;

	// Token: 0x0400122F RID: 4655
	private bool last_show_buff;

	// Token: 0x04001230 RID: 4656
	private int last_tooltip_enabled = -1;

	// Token: 0x0200071B RID: 1819
	protected enum SelectionState
	{
		// Token: 0x04003858 RID: 14424
		Default,
		// Token: 0x04003859 RID: 14425
		Destroyed,
		// Token: 0x0400385A RID: 14426
		Selected,
		// Token: 0x0400385B RID: 14427
		Over,
		// Token: 0x0400385C RID: 14428
		Flee,
		// Token: 0x0400385D RID: 14429
		Empty,
		// Token: 0x0400385E RID: 14430
		Uninitialized
	}
}
