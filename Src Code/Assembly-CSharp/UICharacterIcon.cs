using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001E5 RID: 485
public class UICharacterIcon : ObjectIcon, IListener
{
	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06001D07 RID: 7431 RVA: 0x00112184 File Offset: 0x00110384
	// (set) Token: 0x06001D08 RID: 7432 RVA: 0x0011218C File Offset: 0x0011038C
	public Logic.Character Data { get; private set; }

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06001D09 RID: 7433 RVA: 0x00112195 File Offset: 0x00110395
	// (set) Token: 0x06001D0A RID: 7434 RVA: 0x0011219D File Offset: 0x0011039D
	public Vars Vars { get; private set; }

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x06001D0B RID: 7435 RVA: 0x001121A8 File Offset: 0x001103A8
	// (remove) Token: 0x06001D0C RID: 7436 RVA: 0x001121E0 File Offset: 0x001103E0
	public event Action<UICharacterIcon> OnSelect;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06001D0D RID: 7437 RVA: 0x00112218 File Offset: 0x00110418
	// (remove) Token: 0x06001D0E RID: 7438 RVA: 0x00112250 File Offset: 0x00110450
	public event Action<UICharacterIcon> OnFocus;

	// Token: 0x06001D0F RID: 7439 RVA: 0x00112285 File Offset: 0x00110485
	public override void Awake()
	{
		base.Awake();
		if (this.logicObject == null)
		{
			this.SetObject(null, null);
		}
	}

	// Token: 0x06001D10 RID: 7440 RVA: 0x001122A0 File Offset: 0x001104A0
	private void OnDestroy()
	{
		this.OnSelect = null;
		this.OnFocus = null;
		if (this.hostGO != null)
		{
			this.hostGO.OnChange -= this.HostGO_OnChange;
		}
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06001D11 RID: 7441 RVA: 0x001122F4 File Offset: 0x001104F4
	public void SetSemiTransparent(bool val)
	{
		this.m_ForceSemiTransparent = val;
		this.UpdateHighlight();
	}

	// Token: 0x06001D12 RID: 7442 RVA: 0x00112304 File Offset: 0x00110504
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_AdviceYes = global::Defs.GetObj<Sprite>("CharacterIconSettings", "icon_advice_yes", null);
		this.m_AdviceNo = global::Defs.GetObj<Sprite>("CharacterIconSettings", "icon_advice_no", null);
		if (this.m_ActionProgress == null && this.m_ActionProgressContainer != null)
		{
			this.m_ActionProgress = this.m_ActionProgressContainer.gameObject.AddComponent<UICharacterIcon.ActionProgress>();
		}
		if (this.m_ReinforcementProgress == null && this.m_ReinforcementProgressContainer != null)
		{
			this.m_ReinforcementProgress = this.m_ReinforcementProgressContainer.AddComponent<UICharacterIcon.ReinforcementProgress>();
		}
		if (this.m_MaintainStatusIcon != null)
		{
			this.m_MaintainStatusIcon.KeepAlive(true);
		}
		UICharacterIcon.LoadColors();
		this.m_Initialzed = true;
	}

	// Token: 0x06001D13 RID: 7443 RVA: 0x001123D4 File Offset: 0x001105D4
	private void Start()
	{
		if (this.Data == null)
		{
			this.hostGO = base.GetComponentInParent<UICharacterDataHost>();
			if (this.hostGO != null)
			{
				Logic.Character characterData = this.hostGO.GetCharacterData();
				if (characterData != null)
				{
					this.hostGO.OnChange += this.HostGO_OnChange;
					this.SetObject(characterData, null);
				}
			}
		}
	}

	// Token: 0x06001D14 RID: 7444 RVA: 0x00112431 File Offset: 0x00110631
	private void HostGO_OnChange(ICharacterDataHost obj)
	{
		this.SetObject(obj.GetCharacterData(), null);
	}

	// Token: 0x06001D15 RID: 7445 RVA: 0x00112440 File Offset: 0x00110640
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject != null && this.logicObject == obj)
		{
			return;
		}
		this.ResetAnimations();
		base.SetObject(obj, vars);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		bool flag = vars != null && vars.Get<bool>("is_reinforcement", false);
		if (vars != null)
		{
			vars.Get<bool>("is_player_side", false);
		}
		if (this.logicObject != null)
		{
			if (obj is Logic.Character)
			{
				this.Data = (obj as Logic.Character);
				this.Data.AddListener(this);
			}
			if (!this.m_DisableTooltip)
			{
				Tooltip tooltip = Tooltip.Get(base.gameObject, true);
				tooltip.SetObj(obj, null, null);
				if (flag)
				{
					tooltip.SetDef("CharacterReinforcementTooltip", vars);
				}
				else
				{
					tooltip.handler = new Tooltip.Handler(this.HandleTooltip);
				}
			}
		}
		else
		{
			this.Data = null;
			Tooltip tooltip2 = Tooltip.Get(base.gameObject, true);
			if (tooltip2 != null)
			{
				if (flag)
				{
					tooltip2.SetDef("FillReinforcementTooltip", vars);
				}
				else
				{
					tooltip2.SetDef(null, null, null);
				}
			}
		}
		if (this.m_ActionIcon == null && this.m_ActionIconContainer != null)
		{
			this.m_ActionIcon = this.m_ActionIconContainer.AddComponent<UICharacterIcon.ActionIcon>();
		}
		UICharacterIcon.ActionIcon actionIcon = this.m_ActionIcon;
		if (actionIcon != null)
		{
			actionIcon.SetCharacter(this.Data);
		}
		UICharacterIcon.ActionProgress actionProgress = this.m_ActionProgress;
		if (actionProgress != null)
		{
			actionProgress.SetCharacter(this.Data, null);
		}
		UICharacterIcon.ReinforcementProgress reinforcementProgress = this.m_ReinforcementProgress;
		if (reinforcementProgress != null)
		{
			reinforcementProgress.SetCharacter(this.Data, vars);
		}
		this.m_InvalidateProgressBars = true;
		if (this.m_CharacterCrown != null)
		{
			this.m_CharacterCrown.SetData(this.Data);
		}
		this.Refresh();
		this.UpdateActionIcon(true);
		this.UpdateAvailableReinforcements(true);
	}

	// Token: 0x06001D16 RID: 7446 RVA: 0x001125FC File Offset: 0x001107FC
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (tooltip.instance == null)
		{
			return false;
		}
		UICharacterTooltip component = tooltip.instance.GetComponent<UICharacterTooltip>();
		if (component == null)
		{
			return false;
		}
		Vars vars = tooltip.vars;
		if (vars != null)
		{
			vars.Set<bool>("hide_icon", true);
		}
		return component.HandleTooltip(this.Data, tooltip.vars, ui, evt);
	}

	// Token: 0x06001D17 RID: 7447 RVA: 0x0011265B File Offset: 0x0011085B
	public void Select(bool selected)
	{
		this.m_Selected = selected;
		this.UpdateHighlight();
	}

	// Token: 0x06001D18 RID: 7448 RVA: 0x0011266A File Offset: 0x0011086A
	public void ShowCrest(bool shown)
	{
		this.showCrest = shown;
		if (this.m_KingdomShield != null)
		{
			this.m_KingdomShield.gameObject.SetActive(this.showCrest);
		}
	}

	// Token: 0x06001D19 RID: 7449 RVA: 0x00112697 File Offset: 0x00110897
	public void ShowMissonKingdomCrest(bool shown)
	{
		this.showMissionKingdomCrest = shown;
		this.UpdateMissinonCrest();
	}

	// Token: 0x06001D1A RID: 7450 RVA: 0x001126A6 File Offset: 0x001108A6
	public void ShowPrisonKingdomCrest(bool shown)
	{
		this.showPrisonKingdomCrest = shown;
		this.UpdateMissinonCrest();
	}

	// Token: 0x06001D1B RID: 7451 RVA: 0x001126B5 File Offset: 0x001108B5
	public void ShowCrown(bool shown)
	{
		this.showCrown = shown;
		this.UpdateCrown();
	}

	// Token: 0x06001D1C RID: 7452 RVA: 0x001126C4 File Offset: 0x001108C4
	public void ShowStatus(bool shown)
	{
		this.showStatus = shown;
		if (this.m_ActionIcon != null)
		{
			this.m_ActionIcon.gameObject.SetActive(this.showStatus && this.CanShowStatus());
		}
		this.m_InvalidateProgressBars = true;
	}

	// Token: 0x06001D1D RID: 7453 RVA: 0x00112703 File Offset: 0x00110903
	public void ShowMaintainStatusIcon(bool shown)
	{
		this.showMaintainStatusIcon = shown;
		this.UpdateMaintainStatusIcon();
	}

	// Token: 0x06001D1E RID: 7454 RVA: 0x00112712 File Offset: 0x00110912
	public void ShowArmyBanner(bool shown)
	{
		this.showLeadArmyStatus = shown;
		this.UpdateClass();
	}

	// Token: 0x06001D1F RID: 7455 RVA: 0x00112721 File Offset: 0x00110921
	public void EnableClassLevel(bool shown)
	{
		this.showClassLevel = shown;
		this.UpdateClassLevel();
	}

	// Token: 0x06001D20 RID: 7456 RVA: 0x00112730 File Offset: 0x00110930
	public void FlipHorizontal(bool fliped)
	{
		if (this.flipHorizontal == fliped)
		{
			return;
		}
		this.flipHorizontal = fliped;
		if (this.Image_Portrait != null)
		{
			this.Image_Portrait.transform.localScale = (this.flipHorizontal ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
		}
	}

	// Token: 0x06001D21 RID: 7457 RVA: 0x0011279E File Offset: 0x0011099E
	public void UpdateLevelVisibilityFilter(bool isFilterOn)
	{
		this.m_IsExperienceVisibleFilter = isFilterOn;
		this.m_InvalidateExpStars = true;
	}

	// Token: 0x06001D22 RID: 7458 RVA: 0x001127B0 File Offset: 0x001109B0
	public void DisableTooltip(bool disabled)
	{
		if (this.m_DisableTooltip == disabled)
		{
			return;
		}
		this.m_DisableTooltip = disabled;
		if (this.m_DisableTooltip)
		{
			UnityEngine.Object.Destroy(Tooltip.Get(base.gameObject, false));
			return;
		}
		if (this.Data == null)
		{
			UnityEngine.Object.Destroy(Tooltip.Get(base.gameObject, false));
			return;
		}
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		tooltip.SetObj(this.Data, null, null);
		tooltip.handler = new Tooltip.Handler(this.HandleTooltip);
	}

	// Token: 0x06001D23 RID: 7459 RVA: 0x00112830 File Offset: 0x00110A30
	private static void LoadColors()
	{
		if (UICharacterIcon.colors_loaded)
		{
			return;
		}
		UICharacterIcon.colors_loaded = true;
		DT.Field field = global::Defs.Get(false).dt.Find("CharacterIcon", null);
		UICharacterIcon.m_IconTintColor_Invalid = global::Defs.GetColor(field, "Color_Invalid", UICharacterIcon.m_IconTintColor_Invalid, null);
		UICharacterIcon.m_IconTintColor_Normal = global::Defs.GetColor(field, "Color_Normal", UICharacterIcon.m_IconTintColor_Normal, null);
		UICharacterIcon.m_IconTintColor_Focused = global::Defs.GetColor(field, "Color_Focused", UICharacterIcon.m_IconTintColor_Focused, null);
		UICharacterIcon.m_IconTintColor_Dead = global::Defs.GetColor(field, "Color_Dead", UICharacterIcon.m_IconTintColor_Dead, null);
		UICharacterIcon.m_IconTintColor_Focused_Dead = global::Defs.GetColor(field, "Color_Focused_Dead", UICharacterIcon.m_IconTintColor_Focused_Dead, null);
		UICharacterIcon.m_IconTintColor_Reinforcement = global::Defs.GetColor(field, "Color_Reinforcement", UICharacterIcon.m_IconTintColor_Reinforcement, null);
		UICharacterIcon.m_IconTintColor_Reinforcement_Focused = global::Defs.GetColor(field, "Color_Reinforcement_Focused", UICharacterIcon.m_IconTintColor_Reinforcement_Focused, null);
		UICharacterIcon.m_IconTintColor_SemiTransparent = global::Defs.GetColor(field, "Color_SemiTransparent", UICharacterIcon.m_IconTintColor_SemiTransparent, null);
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x00112910 File Offset: 0x00110B10
	private Color PortraitColor()
	{
		if (this.Data == null)
		{
			return UICharacterIcon.m_IconTintColor_Invalid;
		}
		bool flag = this.Data.IsAlive();
		bool flag2 = this.vars.Get<bool>("is_reinforcement", false);
		if (this.m_ForceSemiTransparent)
		{
			return UICharacterIcon.m_IconTintColor_SemiTransparent;
		}
		if (flag2)
		{
			if (!this.mouse_in)
			{
				return UICharacterIcon.m_IconTintColor_Reinforcement;
			}
			return UICharacterIcon.m_IconTintColor_Reinforcement_Focused;
		}
		else
		{
			if (this.variable_color != Color.clear)
			{
				return this.variable_color;
			}
			if (this.mouse_in || this.m_Selected)
			{
				if (!flag)
				{
					return UICharacterIcon.m_IconTintColor_Focused_Dead;
				}
				return UICharacterIcon.m_IconTintColor_Focused;
			}
			else
			{
				if (!flag)
				{
					return UICharacterIcon.m_IconTintColor_Dead;
				}
				return UICharacterIcon.m_IconTintColor_Normal;
			}
		}
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x001129B4 File Offset: 0x00110BB4
	private void SetIconDelayed(Sprite sprite, Logic.Character c)
	{
		if (this.Image_Portrait != null && sprite != null)
		{
			if (c != this.Data)
			{
				return;
			}
			this.Image_Portrait.overrideSprite = sprite;
			TweenColor tween = this.Image_Portrait.gameObject.AddComponent<TweenColor>();
			tween.from = new Color(1f, 1f, 1f, 0f);
			tween.to = this.PortraitColor();
			tween.duration = 0.15f;
			tween.method = UITweener.Method.Linear;
			tween.ignoreTimeScale = true;
			tween.PlayForward();
			tween.onFinished.AddListener(delegate()
			{
				UnityEngine.Object.Destroy(tween);
			});
		}
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x00112A94 File Offset: 0x00110C94
	private void SetIcon(Sprite sprite)
	{
		if (this.Image_Portrait != null)
		{
			this.Image_Portrait.color = this.PortraitColor();
			this.Image_Portrait.overrideSprite = sprite;
		}
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x00112AC4 File Offset: 0x00110CC4
	private void Refresh()
	{
		if (this.Data == null)
		{
			if (this.Group_Empty != null)
			{
				this.Group_Empty.gameObject.SetActive(true);
			}
			if (this.Group_Populated != null)
			{
				this.Group_Populated.gameObject.SetActive(false);
			}
			if (this.Image_Portrait != null)
			{
				this.Image_Portrait.color = this.PortraitColor();
				this.Image_Portrait.overrideSprite = null;
			}
		}
		else
		{
			Logic.Character data = this.Data;
			if (((data != null) ? data.game : null) == null)
			{
				return;
			}
			if (this.Data.game.state == Game.State.LoadingMap || this.Data.game.state == Game.State.Quitting)
			{
				return;
			}
			if (this.Group_Empty != null)
			{
				this.Group_Empty.gameObject.SetActive(false);
			}
			if (this.Group_Populated != null)
			{
				this.Group_Populated.gameObject.SetActive(true);
			}
			if (this.Image_Portrait != null)
			{
				RectTransform component = this.Image_Portrait.GetComponent<RectTransform>();
				this.SetIcon(DynamicIconBuilder.Instance.GetSprite(this.Data, component.rect.width));
			}
			if (this.m_KingdomShield != null)
			{
				Logic.Object @object;
				if (this.Data.IsMercenary())
				{
					@object = FactionUtils.GetFactionKingdom(this.Data.game, "MercenaryFaction");
				}
				else
				{
					@object = this.Data;
				}
				Vars vars = new Vars(@object);
				vars.Set<Logic.Character>("character", this.Data);
				this.m_KingdomShield.SetObject(@object, vars);
				this.m_KingdomShield.UpdateShields();
				this.m_KingdomShield.gameObject.SetActive(this.showCrest);
			}
			if (this.m_RoyalFrame != null)
			{
				bool flag = this.Data.IsRoyalty();
				flag &= !this.Data.IsPope();
				flag &= !this.Data.IsCardinal();
				this.m_RoyalFrame.gameObject.SetActive(flag);
			}
			if (this.Image_ClassColor != null)
			{
				if (this.Data.class_def != null && this.Data.class_def.dt_def != null)
				{
					this.Image_ClassColor.gameObject.SetActive(true);
					this.Image_ClassColor.color = global::Defs.GetColor(this.Data.class_def.dt_def.field, "color", null);
				}
				else
				{
					this.Image_ClassColor.gameObject.SetActive(false);
				}
			}
			if (this.Image_ClassColorSecoundary != null)
			{
				if (this.Data.class_def != null && this.Data.class_def.dt_def != null)
				{
					this.Image_ClassColorSecoundary.gameObject.SetActive(true);
					if (global::Defs.GetColor(this.Data.class_def.dt_def.field, "color2", null) == Color.black)
					{
						this.Image_ClassColorSecoundary.color = global::Defs.GetColor(this.Data.class_def.dt_def.field, "color", null);
					}
					else
					{
						this.Image_ClassColorSecoundary.color = global::Defs.GetColor(this.Data.class_def.dt_def.field, "color2", null);
					}
				}
				else
				{
					this.Image_ClassColorSecoundary.gameObject.SetActive(false);
				}
			}
			if (this.Image_Portrait != null)
			{
				this.Image_Portrait.transform.localScale = (this.flipHorizontal ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
			}
		}
		this.UpdateMissinonCrest();
		this.UpdateHighlight();
		this.UpdateExpStars();
		this.UpdateCrown();
		this.UpdateClass();
		this.UpdateClassLevel();
		this.UpdatePrisonBars(false);
		this.UpdateDeceased();
		this.UpdateGovernor();
		this.UpdateStance();
		this.UpdateLevel();
		this.UpdateMaintainStatusIcon();
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x00112ED0 File Offset: 0x001110D0
	private void UpdateActionIcon(bool instant_hide = false)
	{
		if (this == null)
		{
			return;
		}
		if (!this.showStatus || !this.CanShowStatus())
		{
			UICharacterIcon.ActionIcon actionIcon = this.m_ActionIcon;
			if (actionIcon != null)
			{
				actionIcon.Hide(instant_hide);
			}
			UICharacterIcon.ActionProgress actionProgress = this.m_ActionProgress;
			if (actionProgress == null)
			{
				return;
			}
			actionProgress.Enable(false, false);
			return;
		}
		else
		{
			UICharacterIcon.ActionIcon actionIcon2 = this.m_ActionIcon;
			if (actionIcon2 == null)
			{
				return;
			}
			actionIcon2.Refresh();
			return;
		}
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x00112F2C File Offset: 0x0011112C
	private void UpdateReinforcementProgress()
	{
		UICharacterIcon.ReinforcementProgress reinforcementProgress = this.m_ReinforcementProgress;
		if (reinforcementProgress == null)
		{
			return;
		}
		reinforcementProgress.Enable(true, false);
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x00112F40 File Offset: 0x00111140
	private void UpdatePrisonBars(bool animate = false)
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_PrisonBars != null)
		{
			bool flag = this.Data.prison_kingdom != null;
			this.m_PrisonBars.gameObject.SetActive(flag);
			if (this.m_PrisonerTutorialArea != null)
			{
				this.m_PrisonerTutorialArea.SetActive(flag);
			}
			if (!this.m_PriosnBarsShown && flag)
			{
				this.m_PrisonBars.gameObject.SetActive(true);
				UITweener component = this.m_PrisonBars.GetComponent<UITweener>();
				if (animate)
				{
					if (component != null && !component.enabled)
					{
						component.PlayForward();
					}
				}
				else
				{
					component.ResetToEnd();
				}
				this.m_PriosnBarsShown = true;
			}
			if (this.m_PriosnBarsShown && !flag)
			{
				UITweener ht = this.m_PrisonBars.GetComponent<UITweener>();
				if (ht != null && !ht.enabled)
				{
					if (animate)
					{
						ht.PlayReverse();
						ht.onFinished.AddListener(delegate()
						{
							this.m_PrisonBars.SetActive(false);
							this.m_PriosnBarsShown = false;
							ht.onFinished.RemoveAllListeners();
						});
						return;
					}
					ht.ResetToBeginning();
					this.m_PriosnBarsShown = false;
					this.m_PrisonBars.SetActive(false);
					if (this.m_PrisonerTutorialArea != null)
					{
						this.m_PrisonerTutorialArea.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x001130A0 File Offset: 0x001112A0
	private void UpdateGovernor()
	{
		if (this.m_GovernorIndication == null)
		{
			return;
		}
		Logic.Character data = this.Data;
		bool flag = data != null && data.IsGovernor();
		this.m_GovernorIndication.gameObject.SetActive(flag);
		if (flag)
		{
			int classLevelIconIndex = this.GetClassLevelIconIndex(this.Data);
			this.m_GovernorIndication.overrideSprite = global::Defs.GetObj<Sprite>(classLevelIconIndex, "CharacterIconSettings", "tier_governor_border", null);
		}
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x0011310C File Offset: 0x0011130C
	private void UpdateMissinonCrest()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_MissionKingdomShield == null)
		{
			return;
		}
		if (!this.showMissionKingdomCrest && !this.showPrisonKingdomCrest)
		{
			this.m_MissionKingdomShield.SetObject(null, null);
			this.m_MissionKingdomShield.gameObject.SetActive(false);
			this.UpdateActionProgressSize();
			return;
		}
		Logic.Kingdom kingdom = (this.showMissionKingdomCrest && this.Data.mission_kingdom != null) ? this.Data.mission_kingdom : null;
		Logic.Kingdom kingdom2 = (this.Data.GetArmy() != null) ? this.Data.GetLocationKingdom() : null;
		kingdom = ((this.showMissionKingdomCrest && kingdom2 != null && kingdom2 != this.Data.GetKingdom()) ? kingdom2 : kingdom);
		kingdom = ((this.showMissionKingdomCrest && this.Data.IsRebel()) ? this.Data.GetKingdom() : kingdom);
		kingdom = ((this.showPrisonKingdomCrest && this.Data.prison_kingdom != null) ? this.Data.prison_kingdom : kingdom);
		Vars vars = new Vars(kingdom);
		vars.Set<Logic.Character>("character", this.Data);
		this.m_MissionKingdomShield.SetObject(kingdom, vars);
		this.m_MissionKingdomShield.gameObject.SetActive(kingdom != null);
		this.UpdateActionProgressSize();
	}

	// Token: 0x06001D2D RID: 7469 RVA: 0x00113254 File Offset: 0x00111454
	private void UpdateMaintainStatusIcon()
	{
		if (this.m_MaintainStatusIcon == null)
		{
			return;
		}
		if (!this.showMaintainStatusIcon)
		{
			this.m_MaintainStatusIcon.SetObject(null, null);
			this.m_MaintainStatusIcon.gameObject.SetActive(false);
			return;
		}
		Logic.Status pactOrBelifStatus = this.GetPactOrBelifStatus(this.Data);
		this.m_MaintainStatusIcon.SetObject(pactOrBelifStatus, null);
		this.m_MaintainStatusIcon.gameObject.SetActive(pactOrBelifStatus != null);
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x001132C8 File Offset: 0x001114C8
	private Logic.Status GetPactOrBelifStatus(Logic.Character c)
	{
		if (c == null)
		{
			return null;
		}
		Logic.Status status = null;
		if (status == null && c.IsDiplomat())
		{
			Statuses statuses = c.statuses;
			status = ((statuses != null) ? statuses.Find<HoldingAPactStatus>() : null);
		}
		if (status == null && c.IsCleric())
		{
			Statuses statuses2 = c.statuses;
			status = ((statuses2 != null) ? statuses2.Find<PaganBeliefStatus>() : null);
		}
		return status;
	}

	// Token: 0x06001D2F RID: 7471 RVA: 0x0011331C File Offset: 0x0011151C
	private void UpdateActionProgressSize()
	{
		if (this.m_ActionProgressSpacer == null)
		{
			return;
		}
		if (this.m_MissionKingdomShield == null)
		{
			return;
		}
		bool activeInHierarchy = this.m_MissionKingdomShield.gameObject.activeInHierarchy;
		this.m_ActionProgressSpacer.gameObject.SetActive(activeInHierarchy);
	}

	// Token: 0x06001D30 RID: 7472 RVA: 0x0011336C File Offset: 0x0011156C
	private void UpdateCrown()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_CharacterCrown == null)
		{
			return;
		}
		bool flag = this.Data.title == "King";
		flag |= (this.Data.title == "Queen");
		flag |= (this.Data.title == "Prince");
		flag |= (this.Data.title == "Prince");
		flag |= (this.Data.title == "Princess");
		flag |= this.Data.IsPope();
		flag |= this.Data.IsPatriarch();
		flag |= this.Data.IsEcumenicalPatriarch();
		flag |= this.Data.IsRoyalRelative();
		flag |= (this.Data.IsDead() && this.Data.FindStatus<DeadPatriarchStatus>() != null);
		this.m_CharacterCrown.gameObject.SetActive(flag && this.showCrown);
		if (this.m_CrownBackground != null)
		{
			this.m_CrownBackground.gameObject.SetActive(flag && this.showCrown);
		}
	}

	// Token: 0x06001D31 RID: 7473 RVA: 0x001134A8 File Offset: 0x001116A8
	private void UpdateClass()
	{
		if (this == null)
		{
			return;
		}
		if (this.Data == null)
		{
			return;
		}
		if (this.m_ClassBackground != null)
		{
			Sprite classBackground;
			if (this.Data.IsPope())
			{
				classBackground = this.GetClassBackground("Pope");
			}
			else if (this.Data.IsCardinal())
			{
				classBackground = this.GetClassBackground("Cardinal");
			}
			else if (this.Data.IsPatriarch() || this.Data.IsEcumenicalPatriarch())
			{
				classBackground = this.GetClassBackground("Patriarch");
			}
			else if (this.Data.IsQueen())
			{
				classBackground = this.GetClassBackground("Queen");
			}
			else if (this.Data.IsPrincess())
			{
				classBackground = this.GetClassBackground("Princess");
			}
			else if (this.Data.sex == Logic.Character.Sex.Female)
			{
				classBackground = this.GetClassBackground("Woman");
			}
			else if (this.Data.IsDead() && this.Data.FindStatus<DeadPatriarchStatus>() != null)
			{
				classBackground = this.GetClassBackground("Patriarch");
			}
			else if (this.Data.class_def == null || this.Data.class_def == this.Data.game.defs.GetBase<CharacterClass.Def>() || this.Data.IsClasslessPrince())
			{
				classBackground = this.GetClassBackground("Empty");
			}
			else
			{
				classBackground = this.GetClassBackground(this.Data.class_def.id);
			}
			this.m_ClassBackground.overrideSprite = classBackground;
		}
		bool flag = this.Data.IsRebel();
		if (this.m_Rebel != null && this.m_Rebel.Length != 0)
		{
			for (int i = 0; i < this.m_Rebel.Length; i++)
			{
				this.m_Rebel[i].SetActive(flag);
			}
		}
		if (this.m_RebelClassEffect != null)
		{
			this.m_RebelClassEffect.SetActive(flag && this.Data.GetArmy() == null);
		}
		if (this.m_Rebel_Famous != null)
		{
			Logic.Army army = this.Data.GetArmy();
			Logic.Rebel rebel = (army != null) ? army.rebel : null;
			Rebellion rebellion = (rebel != null) ? rebel.rebellion : null;
			this.m_Rebel_Famous.SetActive(rebellion != null && rebellion.IsFamous() && rebel.IsLeader());
		}
		if (this.m_LeadingArmy)
		{
			bool flag2 = this.showLeadArmyStatus && this.Data.GetArmy() != null;
			this.m_LeadingArmy.gameObject.SetActive(flag2);
			if (flag2)
			{
				string text = null;
				if (this.Data.class_def != null)
				{
					text = "Background_ArmyLeader_" + this.Data.class_def.id;
				}
				if (this.Data.IsCardinal())
				{
					text = "Background_ArmyLeader_Cardinal";
				}
				if (this.Data.IsPatriarch())
				{
					text = "Background_ArmyLeader_Patriarch";
				}
				if (this.Data.IsPope())
				{
					text = "Background_ArmyLeader_Pope";
				}
				if (this.Data.IsCrusader())
				{
					text = "Background_ArmyLeader_Crusader";
				}
				if (this.Data.IsRebel())
				{
					text = "Background_ArmyLeader_Rebel";
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.m_LeadingArmy.sprite = global::Defs.GetObj<Sprite>("CharacterIconSettings", text, null);
				}
			}
		}
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x001137E8 File Offset: 0x001119E8
	private void UpdateStance()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_BattleStance == null)
		{
			return;
		}
		string key = "stance." + this.vars.Get<string>("stance", "Neutral") + ".battleStance";
		this.m_BattleStance.sprite = global::Defs.GetObj<Sprite>("CharacterIconSettings", key, null);
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x0011384C File Offset: 0x00111A4C
	private void UpdateLevel()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_Level == null || this.m_LevelText == null)
		{
			return;
		}
		this.m_LevelText.text = this.Data.GetClassLevel().ToString();
		this.m_Level.SetActive(true);
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x001138AC File Offset: 0x00111AAC
	private void UpdateAvailableReinforcements(bool force = false)
	{
		if (this.m_AvailableReinforcementCount == null)
		{
			return;
		}
		if (this.vars == null || !this.vars.Get<bool>("is_reinforcement", false))
		{
			this.m_AvailableReinforcementCount.enabled = false;
			return;
		}
		Logic.Battle battle = this.vars.Get<Logic.Battle>("battle", null);
		if (battle == null)
		{
			this.m_AvailableReinforcementCount.enabled = false;
			return;
		}
		if (battle.game.IsPaused() && !force)
		{
			return;
		}
		int num = this.vars.Get<int>("battle_side", 0);
		if (battle.reinforcements[num] != null)
		{
			this.m_AvailableReinforcementCount.enabled = false;
			return;
		}
		List<Logic.Character> list = battle.FindValidReinforcements(num);
		if (list == null || list.Count == 0)
		{
			this.m_AvailableReinforcementCount.enabled = false;
			return;
		}
		this.m_AvailableReinforcementCount.enabled = true;
		this.m_AvailableReinforcementCount.text = list.Count.ToString();
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x00113994 File Offset: 0x00111B94
	private Sprite GetClassBackground(string classKey)
	{
		string text = "class_background." + classKey;
		Sprite sprite = null;
		if (!string.IsNullOrEmpty(this.IconVariant))
		{
			sprite = global::Defs.GetObj<Sprite>("CharacterIconSettings", text + "." + this.IconVariant, null);
		}
		if (sprite == null)
		{
			sprite = global::Defs.GetObj<Sprite>("CharacterIconSettings", text, null);
		}
		return sprite;
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x001139F0 File Offset: 0x00111BF0
	private void UpdateClassLevel()
	{
		if (this.Data == null)
		{
			return;
		}
		if (this.m_ClassLevel != null)
		{
			if (this.Data.IsAlive() && this.Data.CanHaveSkills() && this.showClassLevel)
			{
				this.m_ClassLevel.gameObject.SetActive(true);
				if (this.m_ClassLevel_Label != null)
				{
					int classLevel = this.Data.GetClassLevel(this.Data.class_def, 1f);
					int skillsCount = this.Data.GetSkillsCount();
					this.m_ClassLevel_Label.text = ((skillsCount == 0) ? "-" : classLevel.ToString());
					this.m_ClassLevel_Label.color = this.GetClassLevelColor(classLevel);
					return;
				}
			}
			else
			{
				this.m_ClassLevel.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x00113AC4 File Offset: 0x00111CC4
	private Color GetClassLevelColor(int level)
	{
		if (level > 0 && level <= 8)
		{
			return new Color32(148, 146, 147, byte.MaxValue);
		}
		if (level > 8 && level <= 20)
		{
			return new Color32(119, 176, 100, byte.MaxValue);
		}
		if (level > 20 && level <= 29)
		{
			return new Color32(86, 124, 170, byte.MaxValue);
		}
		if (level > 29)
		{
			return new Color32(148, 146, 147, byte.MaxValue);
		}
		return new Color(1f, 1f, 1f, 1f);
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x00113B7C File Offset: 0x00111D7C
	private void UpdateExpStars()
	{
		if (this.Data != null && this.m_RankContainer != null)
		{
			bool flag = this.Data.IsAlive() && this.Data.CanHaveSkills() && this.m_IsExperienceVisibleFilter;
			this.m_RankContainer.gameObject.SetActive(flag);
			if (flag)
			{
				int num = UICharacterIcon.<UpdateExpStars>g__GetSkillAtMaxLevelCount|135_0(this.Data);
				for (int i = 0; i < this.m_RankContainer.transform.childCount; i++)
				{
					this.m_RankContainer.GetChild(i).gameObject.SetActive(i == num);
				}
			}
		}
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x00113C1C File Offset: 0x00111E1C
	private void ClearInvalidateFalgs()
	{
		this.m_Invalidate = false;
		this.m_InvalidateDeceased = false;
		this.m_InvalidateActionIcon = false;
		this.m_InvalidateMissinonCrest = false;
		this.m_InvalidateGovernor = false;
		this.m_InvalidateExpStars = false;
		this.m_InvalidateClassLevel = false;
		this.m_InvalidateClass = false;
		this.m_InvalidateCrown = false;
		this.m_InvalidateAdvice = false;
		this.m_ForceSemiTransparent = false;
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x00113C78 File Offset: 0x00111E78
	private void LateUpdate()
	{
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.ClearInvalidateFalgs();
		}
		if (this.m_InvalidateDeceased)
		{
			this.UpdateDeceased();
			this.m_InvalidateDeceased = false;
		}
		if (this.m_InvalidateActionIcon)
		{
			this.UpdateActionIcon(false);
			this.UpdateMaintainStatusIcon();
			this.m_InvalidateActionIcon = false;
		}
		if (this.m_InvalidateMissinonCrest)
		{
			this.UpdateMissinonCrest();
			this.m_InvalidateMissinonCrest = false;
		}
		if (this.m_InvalidateGovernor)
		{
			this.UpdateGovernor();
			this.m_InvalidateGovernor = false;
		}
		if (this.m_InvalidateExpStars)
		{
			this.UpdateExpStars();
			this.m_InvalidateExpStars = false;
		}
		if (this.m_InvalidateClassLevel)
		{
			this.UpdateClassLevel();
			this.UpdateHighlight();
			this.m_InvalidateClassLevel = false;
		}
		if (this.m_InvalidateClass)
		{
			this.UpdateClass();
			this.m_InvalidateClass = false;
		}
		if (this.m_InvalidateCrown)
		{
			this.UpdateCrown();
			this.m_InvalidateCrown = false;
		}
		if (this.m_InvalidateAdvice)
		{
			this.RefreshAdvice(this.m_AdviceParam);
			this.m_InvalidateAdvice = false;
		}
		if (this.m_InvalidateProgressBars)
		{
			this.m_InvalidateProgressBars = false;
			if (this.m_ActionProgress != null)
			{
				this.m_ActionProgress.Enable(this.showStatus && this.CanShowStatus(), true);
			}
			if (this.m_ReinforcementProgress != null)
			{
				this.m_ReinforcementProgress.Enable(true, true);
			}
		}
		this.UpdateAvailableReinforcements(false);
		this.UpdateReinforcementProgress();
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x00113DCB File Offset: 0x00111FCB
	public override void OnRightClick(PointerEventData e)
	{
		base.OnRightClick(e);
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x00113DD4 File Offset: 0x00111FD4
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (e.button == PointerEventData.InputButton.Left)
		{
			if (e.clickCount == 1)
			{
				if (this.OnSelect != null)
				{
					this.OnSelect(this);
				}
				else
				{
					this.ExecuteDefaultSelectAction();
				}
			}
			if (e.clickCount > 1)
			{
				if (this.OnSelect != null)
				{
					this.OnSelect(this);
				}
				if (this.OnFocus != null)
				{
					this.OnFocus(this);
				}
			}
		}
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x00113E46 File Offset: 0x00112046
	public override void OnDoubleClick(PointerEventData e)
	{
		base.OnDoubleClick(e);
		if (e.button == PointerEventData.InputButton.Left)
		{
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
			}
			if (this.OnFocus != null)
			{
				this.OnFocus(this);
			}
		}
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x000023FD File Offset: 0x000005FD
	public void ExecuteDefaultSelectAction()
	{
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x00113E80 File Offset: 0x00112080
	[Obsolete("Character don't have Info Window By latest GD")]
	private void OpenCharacterInfoWindow()
	{
		if (this.Data == null)
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
			UICharacter.Create(this.Data, prefab, gameObject.transform as RectTransform, null);
		}
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x00113F07 File Offset: 0x00112107
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
		this.ShowClassLevel(true);
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x00113F1D File Offset: 0x0011211D
	private void ShowClassLevel(bool show)
	{
		if (this.m_ClassLevel == null)
		{
			return;
		}
		UICharacterIcon.PlayTweens(this.m_ClassLevel, show);
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x00113F3A File Offset: 0x0011213A
	public void ShowFullImage()
	{
		UICharacterIconPreviewContainer.Instance.PreviewCharacter(this.logicObject as Logic.Character);
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x00113F51 File Offset: 0x00112151
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
		this.ShowClassLevel(false);
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x00113F68 File Offset: 0x00112168
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.Image_OverGlow != null)
		{
			this.Image_OverGlow.gameObject.SetActive(this.mouse_in);
		}
		if (this.Image_Border != null)
		{
			int classLevelIconIndex = this.GetClassLevelIconIndex(this.Data);
			Sprite border;
			if (this.m_Selected)
			{
				border = this.GetBorder("selected", classLevelIconIndex);
			}
			else if (this.mouse_in)
			{
				border = this.GetBorder("focused", classLevelIconIndex);
			}
			else
			{
				border = this.GetBorder("normal", classLevelIconIndex);
			}
			this.Image_Border.overrideSprite = border;
		}
		if (this.Image_Portrait != null)
		{
			this.Image_Portrait.color = this.PortraitColor();
		}
		if (this.m_EmptySelected != null)
		{
			this.m_EmptySelected.SetActive(this.m_Selected);
		}
		if (this.m_BorderSelected != null)
		{
			this.m_BorderSelected.SetActive(this.m_Selected);
		}
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x00114062 File Offset: 0x00112262
	private int GetClassLevelIconIndex(Logic.Character c)
	{
		if (c == null)
		{
			return 0;
		}
		return c.GetClassLevel() / 5;
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x00114074 File Offset: 0x00112274
	private Sprite GetBorder(string highlight_state, int tier)
	{
		string text = "highligth_border." + highlight_state;
		Sprite sprite = null;
		if (!string.IsNullOrEmpty(this.IconVariant))
		{
			sprite = global::Defs.GetObj<Sprite>(tier, "CharacterIconSettings", text + "." + this.IconVariant, null);
		}
		if (sprite == null)
		{
			sprite = global::Defs.GetObj<Sprite>(tier, "CharacterIconSettings", text, null);
		}
		return sprite;
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x001140D2 File Offset: 0x001122D2
	private void UpdateDeceased()
	{
		if (this.Image_Portrait != null)
		{
			this.Image_Portrait.color = this.PortraitColor();
		}
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x001140F4 File Offset: 0x001122F4
	private void Update()
	{
		if (this.Data == null)
		{
			return;
		}
		UICharacterIcon.ActionProgress actionProgress = this.m_ActionProgress;
		if (actionProgress != null)
		{
			actionProgress.Refresh();
		}
		UICharacterIcon.ReinforcementProgress reinforcementProgress = this.m_ReinforcementProgress;
		if (reinforcementProgress != null)
		{
			reinforcementProgress.Refresh();
		}
		if (this.m_LastPriosonUpdate + this.m_PriosonUpdateInterval < UnityEngine.Time.unscaledTime)
		{
			this.UpdatePrisonBars(false);
			this.m_LastPriosonUpdate = UnityEngine.Time.unscaledTime;
		}
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x00114152 File Offset: 0x00112352
	public bool CanShowStatus()
	{
		return this.Data != null && this.Data.GetKingdom() == BaseUI.LogicKingdom();
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x00114174 File Offset: 0x00112374
	public static UICharacterIcon Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (character == null)
		{
			Debug.LogWarning("Fail to create character icon! Reson: no character data e provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UICharacterIcon uicharacterIcon = gameObject.GetComponent<UICharacterIcon>();
		if (uicharacterIcon == null)
		{
			uicharacterIcon = gameObject.AddComponent<UICharacterIcon>();
		}
		uicharacterIcon.SetObject(character, vars);
		return uicharacterIcon;
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x001141EC File Offset: 0x001123EC
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num > 1729400461U)
		{
			if (num <= 2790575441U)
			{
				if (num <= 2247867164U)
				{
					if (num != 1780538988U)
					{
						if (num != 2160955024U)
						{
							if (num != 2247867164U)
							{
								return;
							}
							if (!(message == "statuses_changed"))
							{
								return;
							}
							this.m_InvalidateActionIcon = true;
							this.m_InvalidateGovernor = true;
							return;
						}
						else
						{
							if (!(message == "assign_army"))
							{
								return;
							}
							goto IL_354;
						}
					}
					else
					{
						if (!(message == "rank_changed"))
						{
							return;
						}
						this.m_InvalidateExpStars = true;
						return;
					}
				}
				else if (num != 2633781458U)
				{
					if (num != 2714262534U)
					{
						if (num != 2790575441U)
						{
							return;
						}
						if (!(message == "leave_castle"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "rebellion_famous_state_changed"))
						{
							return;
						}
						this.m_InvalidateClassLevel = true;
						this.m_InvalidateClass = true;
						return;
					}
				}
				else
				{
					if (!(message == "realm_crossed"))
					{
						return;
					}
					goto IL_39B;
				}
			}
			else if (num <= 3237025842U)
			{
				if (num != 2935363440U)
				{
					if (num != 3021484928U)
					{
						if (num != 3237025842U)
						{
							return;
						}
						if (!(message == "enter_castle"))
						{
							return;
						}
					}
					else
					{
						if (!(message == "mission_kingdom_changed"))
						{
							return;
						}
						goto IL_39B;
					}
				}
				else
				{
					if (!(message == "character_class_change"))
					{
						return;
					}
					goto IL_32E;
				}
			}
			else if (num <= 3780989157U)
			{
				if (num != 3510854372U)
				{
					if (num != 3780989157U)
					{
						return;
					}
					if (!(message == "kingdom_changed"))
					{
						return;
					}
					goto IL_354;
				}
				else
				{
					if (!(message == "status_changed"))
					{
						return;
					}
					this.m_InvalidateDeceased = true;
					this.m_InvalidateActionIcon = true;
					this.m_InvalidateMissinonCrest = true;
					return;
				}
			}
			else if (num != 4092421942U)
			{
				if (num != 4104256994U)
				{
					return;
				}
				if (!(message == "offer_advice"))
				{
					return;
				}
				this.m_AdviceParam = param;
				this.m_InvalidateAdvice = true;
				return;
			}
			else
			{
				if (!(message == "title_changed"))
				{
					return;
				}
				this.m_Invalidate = true;
				return;
			}
			this.m_InvalidateActionIcon = true;
			return;
		}
		if (num > 696919718U)
		{
			if (num <= 1479753455U)
			{
				if (num != 845558108U)
				{
					if (num != 1211309691U)
					{
						if (num != 1479753455U)
						{
							return;
						}
						if (!(message == "died"))
						{
							return;
						}
					}
					else if (!(message == "destroying"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "refresh_tags"))
					{
						return;
					}
					this.m_InvalidateCrown = true;
					this.m_InvalidateClass = true;
					return;
				}
			}
			else if (num != 1615855124U)
			{
				if (num != 1649643086U)
				{
					if (num != 1729400461U)
					{
						return;
					}
					if (!(message == "portrait_changed"))
					{
						return;
					}
					this.m_Invalidate = true;
					return;
				}
				else if (!(message == "finishing"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "class_changed"))
				{
					return;
				}
				this.m_InvalidateClass = true;
				return;
			}
			this.RemoveFromControlGroup();
			this.m_InvalidateExpStars = true;
			this.m_InvalidateClassLevel = true;
			this.m_InvalidateDeceased = true;
			this.m_InvalidateClass = true;
			return;
		}
		if (num <= 586732532U)
		{
			if (num != 52784063U)
			{
				if (num != 482296307U)
				{
					if (num != 586732532U)
					{
						return;
					}
					if (!(message == "skills_changed"))
					{
						return;
					}
					this.m_InvalidateExpStars = true;
					this.m_InvalidateClassLevel = true;
					this.m_InvalidateGovernor = true;
					return;
				}
				else if (!(message == "royal_relative_status_changed"))
				{
					return;
				}
			}
			else
			{
				if (!(message == "location_kingdom_changed"))
				{
					return;
				}
				goto IL_39B;
			}
		}
		else if (num != 623326961U)
		{
			if (num != 661503156U)
			{
				if (num != 696919718U)
				{
					return;
				}
				if (!(message == "crusader_status_changed"))
				{
					return;
				}
				this.m_InvalidateClass = true;
				return;
			}
			else
			{
				if (!(message == "disband_army"))
				{
					return;
				}
				goto IL_354;
			}
		}
		else
		{
			if (!(message == "prison_changed"))
			{
				return;
			}
			using (Game.Profile("UICharacterIcon UpdatePrisonBars", false, 0f, null))
			{
				this.UpdatePrisonBars(false);
			}
			this.m_InvalidateMissinonCrest = true;
			return;
		}
		IL_32E:
		this.m_Invalidate = true;
		return;
		IL_354:
		this.RemoveFromControlGroup();
		this.m_Invalidate = true;
		return;
		IL_39B:
		this.m_InvalidateMissinonCrest = true;
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x0011461C File Offset: 0x0011281C
	private void RefreshAdvice(object param)
	{
		if (this.m_AdviceIcon == null)
		{
			return;
		}
		if (param == null || this.Data == null || this.m_AdviceYes == null || this.m_AdviceNo == null)
		{
			this.m_AdviceIcon.SetActive(false);
			return;
		}
		bool flag = (bool)param;
		Image component = this.m_AdviceIcon.GetComponent<Image>();
		if (component != null)
		{
			Vars vars = new Vars(this.Data);
			CourtAdvice.SetAdviceVars(this.Data, vars);
			component.sprite = (flag ? this.m_AdviceYes : this.m_AdviceNo);
			this.m_AdviceIcon.SetActive(true);
			Tooltip.Get(this.m_AdviceIcon, true).SetDef("CharacterAdviceTooltip", vars);
			return;
		}
		this.m_AdviceIcon.SetActive(false);
	}

	// Token: 0x06001D4D RID: 7501 RVA: 0x001146EC File Offset: 0x001128EC
	private void RemoveFromControlGroup()
	{
		if (this.Data == null)
		{
			return;
		}
		BaseUI baseUI = BaseUI.Get();
		if (baseUI != null)
		{
			baseUI.ClearFromControlGroup(this.Data);
		}
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x00114720 File Offset: 0x00112920
	private static void PlayTweens(GameObject go, bool forward)
	{
		UITweener[] components = go.GetComponents<UITweener>();
		if (components == null || components.Length == 0)
		{
			return;
		}
		for (int i = 0; i < components.Length; i++)
		{
			if (forward)
			{
				components[i].PlayForward();
			}
			else
			{
				components[i].PlayReverse();
			}
		}
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x00114760 File Offset: 0x00112960
	private static void SetTweensEndPoint(GameObject go, bool forward)
	{
		UITweener[] components = go.GetComponents<UITweener>();
		if (components == null || components.Length == 0)
		{
			return;
		}
		for (int i = 0; i < components.Length; i++)
		{
			if (forward)
			{
				components[i].ResetToEnd();
			}
			else
			{
				components[i].ResetToBeginning();
			}
		}
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x001147A0 File Offset: 0x001129A0
	private void ResetAnimations()
	{
		this.m_PriosnBarsShown = false;
		if (this.m_PrisonerTutorialArea != null)
		{
			this.m_PrisonerTutorialArea.SetActive(false);
		}
		if (this.m_PrisonBars != null)
		{
			this.m_PrisonBars.gameObject.SetActive(false);
			UITweener component = this.m_PrisonBars.GetComponent<UITweener>();
			if (component != null)
			{
				component.Stop();
				component.ResetToBeginning();
			}
		}
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x00114954 File Offset: 0x00112B54
	[CompilerGenerated]
	internal static int <UpdateExpStars>g__GetSkillAtMaxLevelCount|135_0(Logic.Character c)
	{
		if (c == null)
		{
			return 0;
		}
		if (c.skills == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < c.skills.Count; i++)
		{
			Skill skill = c.skills[i];
			if (skill != null && c.GetSkillRank(skill) == skill.MaxRank())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x040012DC RID: 4828
	[UIFieldTarget("id_Group_Empty")]
	[SerializeField]
	private RectTransform Group_Empty;

	// Token: 0x040012DD RID: 4829
	[UIFieldTarget("id_Group_Populated")]
	[SerializeField]
	private RectTransform Group_Populated;

	// Token: 0x040012DE RID: 4830
	[UIFieldTarget("id_Portrait")]
	[SerializeField]
	private Image Image_Portrait;

	// Token: 0x040012DF RID: 4831
	[UIFieldTarget("id_Border")]
	[SerializeField]
	private Image Image_Border;

	// Token: 0x040012E0 RID: 4832
	[UIFieldTarget("id_Glow")]
	[SerializeField]
	private Image Image_OverGlow;

	// Token: 0x040012E1 RID: 4833
	[UIFieldTarget("id_ClassColor")]
	[SerializeField]
	private Image Image_ClassColor;

	// Token: 0x040012E2 RID: 4834
	[UIFieldTarget("id_ClassColorSecoundary")]
	[SerializeField]
	private Image Image_ClassColorSecoundary;

	// Token: 0x040012E3 RID: 4835
	[UIFieldTarget("id_RoyalFrame")]
	[SerializeField]
	private GameObject m_RoyalFrame;

	// Token: 0x040012E4 RID: 4836
	[UIFieldTarget("id_LeadingArmy")]
	[SerializeField]
	private Image m_LeadingArmy;

	// Token: 0x040012E5 RID: 4837
	[UIFieldTarget("id_Rebel")]
	[SerializeField]
	private GameObject[] m_Rebel;

	// Token: 0x040012E6 RID: 4838
	[UIFieldTarget("id_RebelClassEffect")]
	[SerializeField]
	private GameObject m_RebelClassEffect;

	// Token: 0x040012E7 RID: 4839
	[UIFieldTarget("id_Rebel_Famous")]
	[SerializeField]
	private GameObject m_Rebel_Famous;

	// Token: 0x040012E8 RID: 4840
	[UIFieldTarget("id_ClassBackground")]
	[SerializeField]
	private Image m_ClassBackground;

	// Token: 0x040012E9 RID: 4841
	[UIFieldTarget("id_BattleStance")]
	[SerializeField]
	private Image m_BattleStance;

	// Token: 0x040012EA RID: 4842
	[UIFieldTarget("id_CrownBackground")]
	private UICrown m_CrownBackground;

	// Token: 0x040012EB RID: 4843
	[UIFieldTarget("_Crown")]
	private UICrown m_CharacterCrown;

	// Token: 0x040012EC RID: 4844
	[UIFieldTarget("_Crest")]
	private UIKingdomIcon m_KingdomShield;

	// Token: 0x040012ED RID: 4845
	[UIFieldTarget("id_GovernorIndication")]
	private Image m_GovernorIndication;

	// Token: 0x040012EE RID: 4846
	[UIFieldTarget("_MissonKingdomCrest")]
	private UIKingdomIcon m_MissionKingdomShield;

	// Token: 0x040012EF RID: 4847
	[UIFieldTarget("id_ClassLevel")]
	private GameObject m_ClassLevel;

	// Token: 0x040012F0 RID: 4848
	[UIFieldTarget("id_ClassLevel_Label")]
	private TextMeshProUGUI m_ClassLevel_Label;

	// Token: 0x040012F1 RID: 4849
	[UIFieldTarget("id_ActionProgress")]
	private RectTransform m_ActionProgressContainer;

	// Token: 0x040012F2 RID: 4850
	[UIFieldTarget("id_ActionProgressSpacer")]
	private RectTransform m_ActionProgressSpacer;

	// Token: 0x040012F3 RID: 4851
	[UIFieldTarget("id_ReinforcementProgress")]
	private GameObject m_ReinforcementProgressContainer;

	// Token: 0x040012F4 RID: 4852
	[UIFieldTarget("id_ActionIcon")]
	private GameObject m_ActionIconContainer;

	// Token: 0x040012F5 RID: 4853
	[UIFieldTarget("id_MaintainStatusIcon")]
	private UIStatusIcon m_MaintainStatusIcon;

	// Token: 0x040012F6 RID: 4854
	[UIFieldTarget("id_PrisonBars")]
	private GameObject m_PrisonBars;

	// Token: 0x040012F7 RID: 4855
	[UIFieldTarget("id_Deceased")]
	private GameObject m_Deceased;

	// Token: 0x040012F8 RID: 4856
	[UIFieldTarget("id_RankContainer")]
	private Transform m_RankContainer;

	// Token: 0x040012F9 RID: 4857
	[UIFieldTarget("id_AvailableReinforcementCount")]
	private TextMeshProUGUI m_AvailableReinforcementCount;

	// Token: 0x040012FA RID: 4858
	[UIFieldTarget("id_Level")]
	[SerializeField]
	private GameObject m_Level;

	// Token: 0x040012FB RID: 4859
	[UIFieldTarget("id_LevelText")]
	[SerializeField]
	private TextMeshProUGUI m_LevelText;

	// Token: 0x040012FC RID: 4860
	[UIFieldTarget("id_EmptySelected")]
	private GameObject m_EmptySelected;

	// Token: 0x040012FD RID: 4861
	[UIFieldTarget("id_BorderSelected")]
	private GameObject m_BorderSelected;

	// Token: 0x040012FE RID: 4862
	[UIFieldTarget("tut_prisoner_area")]
	private GameObject m_PrisonerTutorialArea;

	// Token: 0x040012FF RID: 4863
	[UIFieldTarget("id_AdviceIcon")]
	private GameObject m_AdviceIcon;

	// Token: 0x04001300 RID: 4864
	private Sprite m_AdviceYes;

	// Token: 0x04001301 RID: 4865
	private Sprite m_AdviceNo;

	// Token: 0x04001302 RID: 4866
	[SerializeField]
	public string IconVariant = "";

	// Token: 0x04001303 RID: 4867
	[SerializeField]
	private bool showCrest;

	// Token: 0x04001304 RID: 4868
	[SerializeField]
	private bool showMissionKingdomCrest;

	// Token: 0x04001305 RID: 4869
	[SerializeField]
	private bool showPrisonKingdomCrest;

	// Token: 0x04001306 RID: 4870
	[SerializeField]
	private bool showCrown = true;

	// Token: 0x04001307 RID: 4871
	[SerializeField]
	private bool showStatus = true;

	// Token: 0x04001308 RID: 4872
	[SerializeField]
	private bool showLeadArmyStatus = true;

	// Token: 0x04001309 RID: 4873
	[SerializeField]
	private bool showClassLevel;

	// Token: 0x0400130A RID: 4874
	[SerializeField]
	private bool flipHorizontal;

	// Token: 0x0400130B RID: 4875
	[SerializeField]
	private bool showMaintainStatusIcon;

	// Token: 0x0400130C RID: 4876
	private UICharacterIcon.ActionIcon m_ActionIcon;

	// Token: 0x0400130D RID: 4877
	private UICharacterIcon.ActionProgress m_ActionProgress;

	// Token: 0x0400130E RID: 4878
	private UICharacterIcon.ReinforcementProgress m_ReinforcementProgress;

	// Token: 0x0400130F RID: 4879
	private bool m_Selected;

	// Token: 0x04001312 RID: 4882
	private UICharacterDataHost hostGO;

	// Token: 0x04001315 RID: 4885
	private bool m_Initialzed;

	// Token: 0x04001316 RID: 4886
	private bool m_DisableTooltip;

	// Token: 0x04001317 RID: 4887
	private bool m_IsExperienceVisibleFilter = true;

	// Token: 0x04001318 RID: 4888
	private static bool colors_loaded = false;

	// Token: 0x04001319 RID: 4889
	private object m_AdviceParam;

	// Token: 0x0400131A RID: 4890
	private bool m_Invalidate;

	// Token: 0x0400131B RID: 4891
	private bool m_InvalidateDeceased;

	// Token: 0x0400131C RID: 4892
	private bool m_InvalidateActionIcon;

	// Token: 0x0400131D RID: 4893
	private bool m_InvalidateMissinonCrest;

	// Token: 0x0400131E RID: 4894
	private bool m_InvalidateGovernor;

	// Token: 0x0400131F RID: 4895
	private bool m_InvalidateExpStars;

	// Token: 0x04001320 RID: 4896
	private bool m_InvalidateClassLevel;

	// Token: 0x04001321 RID: 4897
	private bool m_InvalidateClass;

	// Token: 0x04001322 RID: 4898
	private bool m_InvalidateCrown;

	// Token: 0x04001323 RID: 4899
	private bool m_InvalidateAdvice;

	// Token: 0x04001324 RID: 4900
	private bool m_InvalidateProgressBars;

	// Token: 0x04001325 RID: 4901
	private bool m_ForceSemiTransparent;

	// Token: 0x04001326 RID: 4902
	private static Color m_IconTintColor_Invalid = new Color(0.8490566f, 0.5967426f, 0.5967426f, 1f);

	// Token: 0x04001327 RID: 4903
	private static Color m_IconTintColor_Normal = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001328 RID: 4904
	private static Color m_IconTintColor_Focused = new Color(1f, 1f, 1f, 0.49f);

	// Token: 0x04001329 RID: 4905
	private static Color m_IconTintColor_Dead = new Color(0.9440247f, 0.9440247f, 0.9878349f, 0.1215686f);

	// Token: 0x0400132A RID: 4906
	private static Color m_IconTintColor_Focused_Dead = new Color(1f, 1f, 0.9982741f, 0.1215686f);

	// Token: 0x0400132B RID: 4907
	private static Color m_IconTintColor_Reinforcement = new Color32(194, 194, 184, 31);

	// Token: 0x0400132C RID: 4908
	private static Color m_IconTintColor_Reinforcement_Focused = new Color32(194, 194, 184, 31);

	// Token: 0x0400132D RID: 4909
	private static Color m_IconTintColor_SemiTransparent = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 167);

	// Token: 0x0400132E RID: 4910
	public Color variable_color;

	// Token: 0x0400132F RID: 4911
	private bool m_PriosnBarsShown;

	// Token: 0x04001330 RID: 4912
	private float m_LastPriosonUpdate;

	// Token: 0x04001331 RID: 4913
	private float m_PriosonUpdateInterval = 1f;

	// Token: 0x0200072A RID: 1834
	internal class ActionIcon : MonoBehaviour
	{
		// Token: 0x060049E7 RID: 18919 RVA: 0x0021E962 File Offset: 0x0021CB62
		private void Init()
		{
			if (this.m_Initiazlied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			this.m_Initiazlied = true;
		}

		// Token: 0x060049E8 RID: 18920 RVA: 0x0021E97B File Offset: 0x0021CB7B
		public void SetCharacter(Logic.Character c)
		{
			this.Init();
			this.Character = c;
			this.last_status = null;
			this.last_show_in_porttrait = false;
			this.m_Invalidate = true;
		}

		// Token: 0x060049E9 RID: 18921 RVA: 0x0021E99F File Offset: 0x0021CB9F
		public void Refresh()
		{
			this.m_Invalidate = true;
		}

		// Token: 0x060049EA RID: 18922 RVA: 0x0021E9A8 File Offset: 0x0021CBA8
		private void LateUpdate()
		{
			if (this.m_Invalidate)
			{
				this.m_Invalidate = false;
				this.UpdateStatusIcon();
			}
			bool flag = this.CheckTimedAction();
			if (flag != this.focused)
			{
				this.focused = flag;
				this.Focus(this.focused);
			}
		}

		// Token: 0x060049EB RID: 18923 RVA: 0x0021E9F0 File Offset: 0x0021CBF0
		private void UpdateStatusIcon()
		{
			if (this.Character == null)
			{
				this.Hide(true);
				return;
			}
			Logic.Status status = this.Character.status;
			if (status != null && !status.IsValid())
			{
				status = null;
			}
			if (status == null)
			{
				this.Hide(true);
				return;
			}
			bool flag = status.def.show_in_portrait.Value(status, true, true);
			if (status == this.last_status && this.last_show_in_porttrait == flag)
			{
				if (this.m_statusIcon != null && this.m_statusIcon.Data != null)
				{
					this.m_statusIcon.Populate();
				}
				return;
			}
			this.last_status = status;
			this.last_show_in_porttrait = flag;
			if (!flag)
			{
				this.Hide(true);
				return;
			}
			this.Show(true);
			if (this.m_statusIcon == null)
			{
				this.m_statusIcon = global::Common.FindChildComponent<UIStatusIcon>(base.gameObject, "id_Status");
			}
			UIStatusIcon statusIcon = this.m_statusIcon;
			if (statusIcon != null)
			{
				statusIcon.SetObject(this.last_status, null);
			}
			UIStatusIcon statusIcon2 = this.m_statusIcon;
			if (statusIcon2 == null)
			{
				return;
			}
			statusIcon2.KeepAlive(true);
		}

		// Token: 0x060049EC RID: 18924 RVA: 0x0021EAF0 File Offset: 0x0021CCF0
		private bool CheckTimedAction()
		{
			return this.Character != null && this.Character.cur_action != null && this.Character.cur_action.state_end_time.milliseconds > 0L;
		}

		// Token: 0x060049ED RID: 18925 RVA: 0x0021EB24 File Offset: 0x0021CD24
		public void Show(bool instant = true)
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (this.m_statusIcon != null)
			{
				this.m_statusIcon.gameObject.SetActive(true);
			}
			if (this.shown && !this.inTrasition)
			{
				return;
			}
			if (this.tranistionRoutine != null)
			{
				base.StopCoroutine(this.tranistionRoutine);
				this.tranistionRoutine = null;
			}
			this.shown = true;
			this.inTrasition = true;
			if (!base.gameObject.activeSelf)
			{
				this.inTrasition = false;
				return;
			}
			UICharacterIcon.PlayTweens(base.gameObject, true);
			if (base.gameObject.activeInHierarchy)
			{
				this.tranistionRoutine = base.StartCoroutine(this.InvokeAfter(delegate
				{
					this.inTrasition = false;
				}, 0.2f));
				return;
			}
			this.inTrasition = false;
		}

		// Token: 0x060049EE RID: 18926 RVA: 0x0021EBF4 File Offset: 0x0021CDF4
		public void Hide(bool instant = true)
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (!this.shown && !this.inTrasition)
			{
				UIStatusIcon statusIcon = this.m_statusIcon;
				if (statusIcon == null)
				{
					return;
				}
				statusIcon.gameObject.SetActive(false);
				return;
			}
			else
			{
				if (this.tranistionRoutine != null)
				{
					base.StopCoroutine(this.tranistionRoutine);
					this.tranistionRoutine = null;
				}
				this.inTrasition = true;
				this.shown = false;
				if (!base.gameObject.activeInHierarchy)
				{
					this.inTrasition = false;
					return;
				}
				UICharacterIcon.PlayTweens(base.gameObject, false);
				this.tranistionRoutine = base.StartCoroutine(this.InvokeAfter(delegate
				{
					this.inTrasition = false;
					UIStatusIcon statusIcon2 = this.m_statusIcon;
					if (statusIcon2 == null)
					{
						return;
					}
					statusIcon2.gameObject.SetActive(false);
				}, 0.5f));
				return;
			}
		}

		// Token: 0x060049EF RID: 18927 RVA: 0x0021ECA4 File Offset: 0x0021CEA4
		private void Focus(bool focus)
		{
			if (this.m_statusIcon != null)
			{
				UICharacterIcon.PlayTweens(this.m_statusIcon.gameObject, focus);
			}
		}

		// Token: 0x060049F0 RID: 18928 RVA: 0x0021ECC5 File Offset: 0x0021CEC5
		private IEnumerator InvokeAfter(Action a, float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			if (a != null)
			{
				a();
			}
			yield break;
		}

		// Token: 0x040038C9 RID: 14537
		[UIFieldTarget("id_Status")]
		private UIStatusIcon m_statusIcon;

		// Token: 0x040038CA RID: 14538
		private bool inTrasition;

		// Token: 0x040038CB RID: 14539
		private bool shown;

		// Token: 0x040038CC RID: 14540
		private Logic.Character Character;

		// Token: 0x040038CD RID: 14541
		private Logic.Status last_status;

		// Token: 0x040038CE RID: 14542
		private bool last_show_in_porttrait;

		// Token: 0x040038CF RID: 14543
		private bool focused;

		// Token: 0x040038D0 RID: 14544
		private bool m_Initiazlied;

		// Token: 0x040038D1 RID: 14545
		private bool m_Invalidate;

		// Token: 0x040038D2 RID: 14546
		private UnityEngine.Coroutine tranistionRoutine;
	}

	// Token: 0x0200072B RID: 1835
	internal class CharacterProgressBar : MonoBehaviour
	{
		// Token: 0x060049F4 RID: 18932 RVA: 0x0021ED03 File Offset: 0x0021CF03
		private void Init()
		{
			if (this.m_Initazlied)
			{
				return;
			}
			UICommon.FindComponents(this, true);
			this.m_Initazlied = true;
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x0021ED1C File Offset: 0x0021CF1C
		public void SetCharacter(Logic.Character c, Vars vars = null)
		{
			this.Init();
			this.Character = c;
			this.vars = vars;
			this.m_InTrasition = false;
			this.ValidateState(false);
			this.Refresh();
		}

		// Token: 0x060049F6 RID: 18934 RVA: 0x0002C538 File Offset: 0x0002A738
		protected virtual bool Valid()
		{
			return false;
		}

		// Token: 0x060049F7 RID: 18935 RVA: 0x0021ED46 File Offset: 0x0021CF46
		private void ValidateState(bool instant = false)
		{
			if (this.m_Enabled && this.Valid())
			{
				this.Show(instant, true);
				return;
			}
			if (this.m_Enabled && !this.Valid())
			{
				this.Hide(instant, true);
				return;
			}
			this.Hide(instant, true);
		}

		// Token: 0x060049F8 RID: 18936 RVA: 0x0021ED82 File Offset: 0x0021CF82
		public void Enable(bool enabled, bool forceRefresh = false)
		{
			this.m_Enabled = enabled;
			this.ValidateState(false);
		}

		// Token: 0x060049F9 RID: 18937 RVA: 0x0021ED92 File Offset: 0x0021CF92
		public void Refresh()
		{
			if (this.Character == null)
			{
				this.Hide(false, false);
			}
			this.UpdateActionPorgressBar();
		}

		// Token: 0x060049FA RID: 18938 RVA: 0x0021EDAA File Offset: 0x0021CFAA
		private void Update()
		{
			this.UpdateActionPorgressBar();
		}

		// Token: 0x060049FB RID: 18939 RVA: 0x0021EDB4 File Offset: 0x0021CFB4
		public void Show(bool instant = false, bool force = false)
		{
			if (!force && !this.m_Enabled)
			{
				this.m_Shown = true;
				return;
			}
			if ((this.m_Shown || this.m_InTrasition) && !force)
			{
				return;
			}
			if (this.m_TranistionRoutine != null)
			{
				base.StopCoroutine(this.m_TranistionRoutine);
				this.m_TranistionRoutine = null;
			}
			this.m_Shown = true;
			this.m_InTrasition = true;
			this.m_Container.gameObject.SetActive(true);
			if (!base.gameObject.activeInHierarchy || instant)
			{
				this.m_InTrasition = false;
				UICharacterIcon.SetTweensEndPoint(this.m_Container, true);
				return;
			}
			UICharacterIcon.PlayTweens(this.m_Container, true);
			this.m_TranistionRoutine = base.StartCoroutine(this.InvokeAfter(delegate
			{
				this.m_InTrasition = false;
			}, 0.2f));
		}

		// Token: 0x060049FC RID: 18940 RVA: 0x0021EE78 File Offset: 0x0021D078
		public void Hide(bool instant = false, bool force = false)
		{
			if (!force && !this.m_Enabled)
			{
				this.m_Shown = false;
				return;
			}
			if ((!this.m_Shown || this.m_InTrasition) && !force)
			{
				return;
			}
			if (this.m_TranistionRoutine != null)
			{
				base.StopCoroutine(this.m_TranistionRoutine);
				this.m_TranistionRoutine = null;
			}
			this.m_InTrasition = true;
			this.m_Shown = false;
			if (!base.gameObject.activeInHierarchy || instant)
			{
				this.m_InTrasition = false;
				GameObject container = this.m_Container;
				if (container != null)
				{
					container.gameObject.SetActive(false);
				}
				UICharacterIcon.SetTweensEndPoint(this.m_Container, false);
				return;
			}
			UICharacterIcon.PlayTweens(this.m_Container, false);
			this.m_TranistionRoutine = base.StartCoroutine(this.InvokeAfter(delegate
			{
				this.m_InTrasition = false;
				GameObject container2 = this.m_Container;
				if (container2 == null)
				{
					return;
				}
				container2.gameObject.SetActive(false);
			}, 0.2f));
		}

		// Token: 0x060049FD RID: 18941 RVA: 0x0021EF41 File Offset: 0x0021D141
		protected virtual void GetProgress(out float progress, out float max_progress)
		{
			progress = 0f;
			max_progress = 1f;
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x0021EF54 File Offset: 0x0021D154
		private void UpdateActionPorgressBar()
		{
			if (this.Character == null)
			{
				this.Hide(false, false);
				return;
			}
			if (!this.m_Enabled && this.m_Shown)
			{
				this.Hide(true, false);
				return;
			}
			if (!this.m_Enabled)
			{
				return;
			}
			if (!this.Valid())
			{
				if (this.m_Shown && !this.m_InTrasition)
				{
					this.Hide(false, false);
				}
				return;
			}
			float num;
			float num2;
			this.GetProgress(out num, out num2);
			if (num2 == 0f)
			{
				this.Hide(false, false);
				return;
			}
			if (num2 > 0f)
			{
				float num3 = num / num2;
				Image component = this.m_Foregroud.GetComponent<Image>();
				if (component != null && this.m_Foregroud.GetComponent<Image>().type == Image.Type.Filled)
				{
					component.fillAmount = num3;
				}
				else
				{
					(this.m_Foregroud.transform as RectTransform).anchorMax = new Vector2(num3, 1f);
				}
				if (!this.m_Shown && !this.m_InTrasition)
				{
					this.Show(false, false);
					return;
				}
			}
			else
			{
				this.Hide(false, false);
			}
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x0021F04E File Offset: 0x0021D24E
		private IEnumerator InvokeAfter(Action a, float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			if (a != null)
			{
				a();
			}
			yield break;
		}

		// Token: 0x040038D3 RID: 14547
		[UIFieldTarget("id_Contianer")]
		private GameObject m_Container;

		// Token: 0x040038D4 RID: 14548
		[UIFieldTarget("id_ProgressBarForeground")]
		private GameObject m_Foregroud;

		// Token: 0x040038D5 RID: 14549
		protected Logic.Character Character;

		// Token: 0x040038D6 RID: 14550
		private bool m_InTrasition;

		// Token: 0x040038D7 RID: 14551
		private bool m_Shown;

		// Token: 0x040038D8 RID: 14552
		private UnityEngine.Coroutine m_TranistionRoutine;

		// Token: 0x040038D9 RID: 14553
		private bool m_Initazlied;

		// Token: 0x040038DA RID: 14554
		private bool m_Enabled = true;

		// Token: 0x040038DB RID: 14555
		protected Vars vars;
	}

	// Token: 0x0200072C RID: 1836
	internal class ActionProgress : UICharacterIcon.CharacterProgressBar
	{
		// Token: 0x06004A03 RID: 18947 RVA: 0x0021F09B File Offset: 0x0021D29B
		protected override void GetProgress(out float progress, out float max_progress)
		{
			Logic.Character character = this.Character;
			if (((character != null) ? character.cur_action : null) == null)
			{
				progress = 0f;
				max_progress = 1f;
			}
			this.Character.cur_action.GetProgress(out progress, out max_progress);
		}

		// Token: 0x06004A04 RID: 18948 RVA: 0x0021F0D1 File Offset: 0x0021D2D1
		protected override bool Valid()
		{
			Logic.Character character = this.Character;
			return ((character != null) ? character.cur_action : null) != null;
		}
	}

	// Token: 0x0200072D RID: 1837
	internal class ReinforcementProgress : UICharacterIcon.CharacterProgressBar
	{
		// Token: 0x06004A06 RID: 18950 RVA: 0x0021F0F0 File Offset: 0x0021D2F0
		protected override void GetProgress(out float progress, out float max_progress)
		{
			progress = 0f;
			max_progress = 1f;
			Logic.Battle battle = this.battle;
			if (battle == null)
			{
				return;
			}
			ComputableValue timer = battle.reinforcements[this.reinforcement_id].timer;
			if (timer == null)
			{
				return;
			}
			max_progress = timer.GetMax();
			progress = max_progress - timer.Get();
		}

		// Token: 0x06004A07 RID: 18951 RVA: 0x0021F140 File Offset: 0x0021D340
		protected override bool Valid()
		{
			Logic.Battle battle = this.battle;
			return battle != null && battle.reinforcements[this.reinforcement_id].timer != null;
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06004A08 RID: 18952 RVA: 0x0021F16E File Offset: 0x0021D36E
		private Logic.Battle battle
		{
			get
			{
				if (this.vars == null)
				{
					return null;
				}
				return this.vars.Get<Logic.Battle>("battle", null);
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06004A09 RID: 18953 RVA: 0x0021F18B File Offset: 0x0021D38B
		private int reinforcement_id
		{
			get
			{
				if (this.vars == null)
				{
					return -1;
				}
				return this.vars.Get<int>("battle_side", 0);
			}
		}
	}
}
