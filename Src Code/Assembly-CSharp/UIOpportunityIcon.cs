using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000191 RID: 401
public class UIOpportunityIcon : ObjectIcon
{
	// Token: 0x17000121 RID: 289
	// (get) Token: 0x0600163A RID: 5690 RVA: 0x000E0336 File Offset: 0x000DE536
	// (set) Token: 0x0600163B RID: 5691 RVA: 0x000E033E File Offset: 0x000DE53E
	public Opportunity Data { get; private set; }

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x0600163C RID: 5692 RVA: 0x000E0347 File Offset: 0x000DE547
	// (set) Token: 0x0600163D RID: 5693 RVA: 0x000E034F File Offset: 0x000DE54F
	public Vars Vars { get; private set; }

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x0600163E RID: 5694 RVA: 0x000E0358 File Offset: 0x000DE558
	// (set) Token: 0x0600163F RID: 5695 RVA: 0x000E0360 File Offset: 0x000DE560
	public UIOpportunityIcon.State state { get; private set; }

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06001640 RID: 5696 RVA: 0x000E0369 File Offset: 0x000DE569
	// (set) Token: 0x06001641 RID: 5697 RVA: 0x000E0371 File Offset: 0x000DE571
	public DT.Field state_def { get; private set; }

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06001642 RID: 5698 RVA: 0x000E037A File Offset: 0x000DE57A
	// (set) Token: 0x06001643 RID: 5699 RVA: 0x000E0382 File Offset: 0x000DE582
	public DT.Field slot_def { get; private set; }

	// Token: 0x06001644 RID: 5700 RVA: 0x000E038B File Offset: 0x000DE58B
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000E03A4 File Offset: 0x000DE5A4
	public override void SetObject(object obj, Vars vars)
	{
		this.Init();
		base.SetObject(obj, vars);
		Opportunity data = obj as Opportunity;
		this.Data = data;
		this.Vars = vars;
		if (this.Data != null)
		{
			base.enabled = true;
		}
		this.Refresh();
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x000E03EC File Offset: 0x000DE5EC
	private void Refresh()
	{
		this.m_Empty.gameObject.SetActive(this.Data == null);
		this.m_Populated.gameObject.SetActive(this.Data != null);
		this.SetupTooltip();
		if (this.Data == null)
		{
			return;
		}
		if (this.Image_Icon != null)
		{
			Image image_Icon = this.Image_Icon;
			Action action = this.Data.action;
			image_Icon.sprite = global::Defs.GetObj<Sprite>((action != null) ? action.def.dt_def.field : null, "icon", null);
		}
		this.UpdateResources();
		this.UpdateActionTarget();
		this.UpdateState();
		this.UpdateHighlight();
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x000E0498 File Offset: 0x000DE698
	public void RefreshTooltip()
	{
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		if (tooltip != null)
		{
			tooltip.Refresh();
		}
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x000E04C1 File Offset: 0x000DE6C1
	private void Update()
	{
		if (this.Data == null)
		{
			return;
		}
		this.UpdateUnseenIcon();
		this.RefreshDynamics();
		this.UpdateState();
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x000E04E0 File Offset: 0x000DE6E0
	public void UpdateState()
	{
		UIOpportunityIcon.State state = this.DecideState();
		this.SetState(state);
		this.UpdateIcon();
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x000E0504 File Offset: 0x000DE704
	public UIOpportunityIcon.State DecideState()
	{
		Opportunity data = this.Data;
		Action action = (data != null) ? data.action : null;
		if (action == null)
		{
			return UIOpportunityIcon.State.Undefined;
		}
		if (action.state == Action.State.PickingArgs)
		{
			return UIOpportunityIcon.State.PickingArgs;
		}
		if (action.state == Action.State.PickingTarget)
		{
			return UIOpportunityIcon.State.PickingTarget;
		}
		if (action.target == this.Data.target)
		{
			if (action.state == Action.State.Preparing)
			{
				return UIOpportunityIcon.State.Preparing;
			}
			if (action.state == Action.State.Running)
			{
				return UIOpportunityIcon.State.Running;
			}
		}
		string a;
		using (new Opportunity.TempActionArgs(action, this.Data.target, this.Data.args))
		{
			a = action.GetValidateKey(null);
		}
		if (action.state == Action.State.Inactive)
		{
			if ((a == "ok" || a == "_no_possible_targets") && this.Data.action.NeedsTarget())
			{
				List<Logic.Object> possibleTargets = action.GetPossibleTargets();
				if (possibleTargets == null || possibleTargets.Count == 0)
				{
					a = "no_possible_targets";
				}
			}
			if ((a == "ok" || a == "_no_possible_args") && this.Data.action.NeedsArgs() && this.Data.action.target != null)
			{
				List<Value>[] possibleArgs = this.Data.action.GetPossibleArgs();
				if (possibleArgs == null || possibleArgs.Length == 0)
				{
					a = "no_possible_args";
				}
			}
		}
		bool flag = a != "ok";
		bool flag2 = a == "_cooldown";
		if (flag)
		{
			return UIOpportunityIcon.State.Invalid;
		}
		if (flag2)
		{
			return UIOpportunityIcon.State.OnCooldown;
		}
		if (action.state == Action.State.Inactive)
		{
			return UIOpportunityIcon.State.Valid;
		}
		return UIOpportunityIcon.State.Undefined;
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x000E0688 File Offset: 0x000DE888
	public void SetState(UIOpportunityIcon.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.slot_def == null)
		{
			this.slot_def = global::Defs.GetDefField("ActionIcon", null);
		}
		if (this.slot_def != null)
		{
			this.state_def = this.slot_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.slot_def.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.state_def = null;
		}
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x000E0738 File Offset: 0x000DE938
	private void UpdateIcon()
	{
		if (this.state_def == null)
		{
			return;
		}
		if (this.Image_Icon != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(this.state_def, "icon", null);
			if (obj != null)
			{
				this.Image_Icon.overrideSprite = obj;
			}
			else
			{
				this.Image_Icon.overrideSprite = null;
			}
			this.Image_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		if (this.m_Border != null)
		{
			Sprite obj2 = global::Defs.GetObj<Sprite>(this.state_def, "border", null);
			if (obj2 != null)
			{
				this.m_Border.overrideSprite = obj2;
			}
			else
			{
				this.m_Border.overrideSprite = null;
			}
			this.m_Border.color = global::Defs.GetColor(this.state_def, "border_color", null);
		}
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x000E080C File Offset: 0x000DEA0C
	private void SetupTooltip()
	{
		if (this.Data != null)
		{
			Tooltip tooltip = Tooltip.Get(base.gameObject, true);
			if (tooltip != null)
			{
				tooltip.SetDef("OpportunityTooltip", new Vars(this.Data));
				return;
			}
		}
		else
		{
			Tooltip tooltip2 = Tooltip.Get(base.gameObject, false);
			if (tooltip2 != null)
			{
				global::Common.DestroyObj(tooltip2);
			}
		}
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x000E0870 File Offset: 0x000DEA70
	private void UpdateResources()
	{
		if (this.m_ResourceLabel == null)
		{
			return;
		}
		if (this.Data == null)
		{
			this.m_ResourceLabel.text = "-";
			return;
		}
		Opportunity data = this.Data;
		SpyPlot spyPlot = ((data != null) ? data.action : null) as SpyPlot;
		if (spyPlot == null)
		{
			this.m_ResourceLabel.text = "-";
			return;
		}
		this.m_ResourcesVars.obj = spyPlot;
		this.vars.Set<Logic.Object>("target", spyPlot.target);
		UIText.SetTextKey(this.m_ResourceLabel, "OpprtunityIcon.succesAndRevealChance", this.vars, null);
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x000E0910 File Offset: 0x000DEB10
	private void UpdateUnseenIcon()
	{
		if (this.m_Unseen == null)
		{
			return;
		}
		this.m_Unseen.SetActive(this.Data != null && !this.Data.seen && !this.Data.def.IsPermanent());
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x000E0965 File Offset: 0x000DEB65
	private void RefreshDynamics()
	{
		if (this.Data == null)
		{
			return;
		}
		this.UpdateResources();
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x000E0978 File Offset: 0x000DEB78
	private void UpdateActionTarget()
	{
		if (this.m_TargetContainer == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_TargetContainer);
		if (this.Data == null || this.Data.target == null)
		{
			return;
		}
		if (this.Data.target is Logic.Realm)
		{
			return;
		}
		if (this.Data.target is Castle)
		{
			return;
		}
		Vars vars = new Vars(this.Data.target);
		vars.Set<string>("variant", (this.Data.target is Logic.Character) ? "micro" : "compact");
		GameObject icon = ObjectIcon.GetIcon(this.Data.target, vars, this.m_TargetContainer);
		if (icon != null)
		{
			UICharacterIcon component = icon.GetComponent<UICharacterIcon>();
			if (component != null)
			{
				component.ShowCrest(false);
				component.DisableTooltip(true);
			}
			UICommon.AddAspectRatioFitter(icon);
		}
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x000E0A64 File Offset: 0x000DEC64
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
		if (this.Data != null && !this.Data.seen)
		{
			this.Data.seen = true;
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x000E0A94 File Offset: 0x000DEC94
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x000E0AA4 File Offset: 0x000DECA4
	public override void OnRightClick(PointerEventData e)
	{
		base.OnRightClick(e);
		if (!Input.GetKey(KeyCode.LeftControl))
		{
			return;
		}
		if (this.Data == null || this.Data.action == null)
		{
			return;
		}
		Action action = this.Data.action;
		Opportunity.Def def;
		if (action == null)
		{
			def = null;
		}
		else
		{
			Action.Def def2 = action.def;
			def = ((def2 != null) ? def2.opportunity : null);
		}
		Opportunity.Def def3 = def;
		if (def3 == null || def3.IsPermanent())
		{
			return;
		}
		bool key = Input.GetKey(KeyCode.LeftShift);
		Logic.Character character = this.Data.action.owner as Logic.Character;
		if (character != null)
		{
			character.actions.DelOpportunity(this.Data.action, this.Data.target, this.Data.args, key);
		}
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x000E0B60 File Offset: 0x000DED60
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		Action<UIOpportunityIcon, PointerEventData> onSelect = this.OnSelect;
		if (onSelect != null)
		{
			onSelect(this, e);
		}
		Opportunity data = this.Data;
		if (((data != null) ? data.action : null) == null || this.state == UIOpportunityIcon.State.Invalid)
		{
			Opportunity data2 = this.Data;
			ActionVisuals.PlayDeclineSound((data2 != null) ? data2.action : null, ActionVisuals.ExecuteDeclinedReason.Unavailable);
			return;
		}
		ActionVisuals.ExecuteDeclinedReason executeDeclinedReason = ActionVisuals.AllowExecuteAction(this.Data.action);
		if (executeDeclinedReason != ActionVisuals.ExecuteDeclinedReason.AlreadyActive)
		{
			this.Data.action.target = this.Data.target;
			this.Data.action.args = this.Data.args;
			ActionVisuals.ExecuteAction(this.Data.action);
			return;
		}
		if (this.Data.action.target == this.Data.target && this.Data.action.args == this.Data.args)
		{
			ActionVisuals.PlayDeclineSound(null, executeDeclinedReason);
			return;
		}
		Opportunity data3 = this.Data;
		ActionVisuals.PlayDeclineSound((data3 != null) ? data3.action : null, executeDeclinedReason);
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x000E0C74 File Offset: 0x000DEE74
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.Image_Glow != null)
		{
			if (this.Data.action == null)
			{
				this.Image_Glow.gameObject.SetActive(false);
				return;
			}
			bool flag = this.Data.action.state != Action.State.Preparing && this.Data.action.state != Action.State.Running;
			this.Image_Glow.gameObject.SetActive((this.mouse_in && flag) || this.Data.action.state == Action.State.PickingTarget);
		}
	}

	// Token: 0x04000E58 RID: 3672
	[UIFieldTarget("id_Empty")]
	private GameObject m_Empty;

	// Token: 0x04000E59 RID: 3673
	[UIFieldTarget("id_Populated")]
	private GameObject m_Populated;

	// Token: 0x04000E5A RID: 3674
	[UIFieldTarget("id_OpportunityLabel")]
	private TextMeshProUGUI m_ResourceLabel;

	// Token: 0x04000E5B RID: 3675
	[UIFieldTarget("id_OpportunityIcon")]
	private Image Image_Icon;

	// Token: 0x04000E5C RID: 3676
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04000E5D RID: 3677
	[UIFieldTarget("id_TargetContainer")]
	private RectTransform m_TargetContainer;

	// Token: 0x04000E5E RID: 3678
	[UIFieldTarget("id_Background")]
	private Image Image_Background;

	// Token: 0x04000E5F RID: 3679
	[UIFieldTarget("id_Progress")]
	private Image Image_Progress;

	// Token: 0x04000E60 RID: 3680
	[UIFieldTarget("id_Unseen")]
	private GameObject m_Unseen;

	// Token: 0x04000E61 RID: 3681
	[UIFieldTarget("id_Glow")]
	private Image Image_Glow;

	// Token: 0x04000E67 RID: 3687
	public Action<UIOpportunityIcon, PointerEventData> OnSelect;

	// Token: 0x04000E68 RID: 3688
	private Vars m_ResourcesVars = new Vars();

	// Token: 0x04000E69 RID: 3689
	private bool m_Initialzied;

	// Token: 0x020006CA RID: 1738
	public enum State
	{
		// Token: 0x04003708 RID: 14088
		Undefined,
		// Token: 0x04003709 RID: 14089
		Valid,
		// Token: 0x0400370A RID: 14090
		Invalid,
		// Token: 0x0400370B RID: 14091
		InvalidActioInprogress,
		// Token: 0x0400370C RID: 14092
		PickingTarget,
		// Token: 0x0400370D RID: 14093
		PickingArgs,
		// Token: 0x0400370E RID: 14094
		Preparing,
		// Token: 0x0400370F RID: 14095
		Running,
		// Token: 0x04003710 RID: 14096
		Finishing,
		// Token: 0x04003711 RID: 14097
		OnCooldown
	}
}
