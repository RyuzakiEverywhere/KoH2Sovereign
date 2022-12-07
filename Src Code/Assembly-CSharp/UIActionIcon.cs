using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200018D RID: 397
public class UIActionIcon : ObjectIcon
{
	// Token: 0x17000112 RID: 274
	// (get) Token: 0x060015E1 RID: 5601 RVA: 0x000DEAF1 File Offset: 0x000DCCF1
	// (set) Token: 0x060015E2 RID: 5602 RVA: 0x000DEAF9 File Offset: 0x000DCCF9
	public ActionVisuals Data { get; private set; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x060015E3 RID: 5603 RVA: 0x000DEB02 File Offset: 0x000DCD02
	// (set) Token: 0x060015E4 RID: 5604 RVA: 0x000DEB0A File Offset: 0x000DCD0A
	public Vars Vars { get; private set; }

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x060015E5 RID: 5605 RVA: 0x000DEB13 File Offset: 0x000DCD13
	// (set) Token: 0x060015E6 RID: 5606 RVA: 0x000DEB1B File Offset: 0x000DCD1B
	public UIActionIcon.State state { get; private set; }

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x060015E7 RID: 5607 RVA: 0x000DEB24 File Offset: 0x000DCD24
	// (set) Token: 0x060015E8 RID: 5608 RVA: 0x000DEB2C File Offset: 0x000DCD2C
	public DT.Field state_def { get; private set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x060015E9 RID: 5609 RVA: 0x000DEB35 File Offset: 0x000DCD35
	// (set) Token: 0x060015EA RID: 5610 RVA: 0x000DEB3D File Offset: 0x000DCD3D
	public DT.Field slot_def { get; private set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x060015EB RID: 5611 RVA: 0x000DEB46 File Offset: 0x000DCD46
	// (set) Token: 0x060015EC RID: 5612 RVA: 0x000DEB4E File Offset: 0x000DCD4E
	public bool ShowIfNotActive { get; set; }

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060015ED RID: 5613 RVA: 0x000DEB57 File Offset: 0x000DCD57
	// (set) Token: 0x060015EE RID: 5614 RVA: 0x000DEB5F File Offset: 0x000DCD5F
	public bool ExecuteDefaultAction { get; set; } = true;

	// Token: 0x060015EF RID: 5615 RVA: 0x000DEB68 File Offset: 0x000DCD68
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Button = base.GetComponent<BSGButton>();
		this.m_Initialized = true;
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x000DEB90 File Offset: 0x000DCD90
	public void SetOverrideTarget(Logic.Object target)
	{
		this.override_target_object = target;
		this.tooltipVars.Set<Logic.Object>("target", target);
		if (this.Image_Icon != null)
		{
			this.Image_Icon.sprite = global::Defs.GetObj<Sprite>(this.Data.logic.def.dt_def.field, "icon", this.tooltipVars);
		}
		this.Refresh();
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000DEC00 File Offset: 0x000DCE00
	public override void SetObject(object obj, Vars vars)
	{
		this.Init();
		base.SetObject(obj, vars);
		Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
		Action action = obj as Action;
		if (action == null)
		{
			return;
		}
		Tooltip.Get(base.gameObject, true).SetVars(this.tooltipVars);
		this.tooltipVars.obj = action;
		this.tooltipVars.Del("target");
		this.Data = (action.visuals as ActionVisuals);
		this.Vars = vars;
		if (this.Data != null)
		{
			base.enabled = true;
		}
		if (this.Image_Icon != null)
		{
			this.Image_Icon.sprite = global::Defs.GetObj<Sprite>(this.Data.logic.def.dt_def.field, "icon", this.Data.logic);
		}
		this.Refresh();
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000DECE7 File Offset: 0x000DCEE7
	private void Refresh()
	{
		this.UpdateState();
		this.UpdateHighlight();
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000DECF8 File Offset: 0x000DCEF8
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
			this.Image_Icon.color = (this.mouse_in ? global::Defs.GetColor(this.state_def, "icon_color_over", null) : global::Defs.GetColor(this.state_def, "icon_color", null));
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
			this.m_Border.color = (this.mouse_in ? global::Defs.GetColor(this.state_def, "border_color_over", null) : global::Defs.GetColor(this.state_def, "border_color", null));
		}
		if (this.m_Button != null)
		{
			this.m_Button.Enable(this.state != UIActionIcon.State.Invalid && this.state != UIActionIcon.State.InvalidActioInprogress, false);
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x000DEE34 File Offset: 0x000DD034
	private void UpdateLabel()
	{
		if (this.Label != null)
		{
			UIText.SetText(this.Label, global::Defs.Localize(this.Data.logic.def.dt_def.field, "name", this.Data.logic, null, true, true));
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x000DEE8C File Offset: 0x000DD08C
	private void UpdateProgress()
	{
		if (this.Data == null)
		{
			return;
		}
		float num;
		float num2;
		this.Data.logic.GetProgress(out num, out num2);
		float num3 = num / num2;
		bool flag = !float.IsNaN(num3) && num2 != 0f;
		if (this.Object_Progress_Container != null)
		{
			this.Object_Progress_Container.SetActive(this.Data.logic.def.secondary && flag);
		}
		if (this.Image_Progress != null)
		{
			this.Image_Progress.gameObject.SetActive(flag);
			if (flag)
			{
				this.Image_Progress.fillAmount = num3;
			}
		}
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x000DEF2F File Offset: 0x000DD12F
	private void Update()
	{
		if (this.Data == null)
		{
			base.enabled = false;
			return;
		}
		this.UpdateState();
		this.UpdateProgress();
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000DEF50 File Offset: 0x000DD150
	public void UpdateState()
	{
		UIActionIcon.State state = this.DecideState();
		this.SetState(state);
		this.UpdateIcon();
		this.UpdateLabel();
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x000DEF78 File Offset: 0x000DD178
	public void SetState(UIActionIcon.State state)
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
			}
		}
		else
		{
			this.state_def = null;
		}
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this);
		}
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x000DF03C File Offset: 0x000DD23C
	public UIActionIcon.State DecideState()
	{
		ActionVisuals data = this.Data;
		Action action = (data != null) ? data.logic : null;
		if (action == null)
		{
			return UIActionIcon.State.Undefined;
		}
		if (action.state == Action.State.PickingArgs)
		{
			return UIActionIcon.State.PickingArgs;
		}
		if (action.state == Action.State.PickingTarget)
		{
			return UIActionIcon.State.PickingTarget;
		}
		if (action.state == Action.State.Preparing)
		{
			return UIActionIcon.State.Preparing;
		}
		if (action.state == Action.State.Running)
		{
			return UIActionIcon.State.Running;
		}
		Logic.Object target = action.target;
		if (this.override_target_object != null)
		{
			action.target = this.override_target_object;
		}
		string a = action.Validate(false);
		if (a == "ok" && action.state == Action.State.Inactive)
		{
			if (this.Data.logic.NeedsTarget())
			{
				List<Logic.Object> possibleTargets = this.Data.logic.GetPossibleTargets();
				if (possibleTargets == null || possibleTargets.Count == 0)
				{
					a = "no_possible_targets";
				}
			}
			if (this.Data.logic.NeedsArgs() && this.Data.logic.target != null)
			{
				List<Value>[] possibleArgs = this.Data.logic.GetPossibleArgs();
				if (possibleArgs == null || possibleArgs.Length == 0)
				{
					a = "no_possible_args";
				}
			}
			if (this.Data.logic.ValidateRequirements() != "ok")
			{
				a = "requirements_not_met";
			}
		}
		action.target = target;
		bool flag = a != "ok";
		bool flag2 = a == "_cooldown";
		if (flag)
		{
			return UIActionIcon.State.Invalid;
		}
		if (flag2)
		{
			return UIActionIcon.State.OnCooldown;
		}
		if (action.state == Action.State.Inactive)
		{
			return UIActionIcon.State.Valid;
		}
		return UIActionIcon.State.Undefined;
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x000DF1A0 File Offset: 0x000DD3A0
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
		this.UpdateIcon();
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x000DF1B5 File Offset: 0x000DD3B5
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
		this.UpdateIcon();
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x000DF1CA File Offset: 0x000DD3CA
	protected override bool ShouldPlayClickSound()
	{
		return this.DecideState() == UIActionIcon.State.Valid;
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x000DF1D8 File Offset: 0x000DD3D8
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this, e);
		}
		if (!this.ExecuteDefaultAction)
		{
			return;
		}
		if (this.state == UIActionIcon.State.Invalid)
		{
			ActionVisuals.PlayDeclineSound(this.Data.logic, ActionVisuals.ExecuteDeclinedReason.Unavailable);
			return;
		}
		ActionVisuals.ExecuteDeclinedReason executeDeclinedReason = ActionVisuals.AllowExecuteAction(this.Data.logic);
		if (executeDeclinedReason != ActionVisuals.ExecuteDeclinedReason.None && executeDeclinedReason != ActionVisuals.ExecuteDeclinedReason.NoTarget && executeDeclinedReason != ActionVisuals.ExecuteDeclinedReason.NoResources)
		{
			ActionVisuals.PlayDeclineSound(this.Data.logic, executeDeclinedReason);
			return;
		}
		if (this.override_target_object != null)
		{
			this.Data.logic.target = this.override_target_object;
		}
		ActionVisuals.ExecuteAction(this.Data.logic);
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x000DF284 File Offset: 0x000DD484
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		ActionVisuals data = this.Data;
		if (((data != null) ? data.logic : null) == null)
		{
			return;
		}
		if (this.Image_Glow != null)
		{
			bool flag = this.Data.logic.state != Action.State.Preparing && this.Data.logic.state != Action.State.Running;
			this.Image_Glow.gameObject.SetActive((this.mouse_in && flag) || this.Data.logic.state == Action.State.PickingTarget);
		}
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x000DF31C File Offset: 0x000DD51C
	public static UIActionIcon Create(ActionVisuals action, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (action == null)
		{
			Debug.LogWarning("Fail to create UIActionIcon! Reson: no character data provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create UIActionIcon! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create UIActionIcon! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIActionIcon uiactionIcon = gameObject.GetComponent<UIActionIcon>();
		if (uiactionIcon == null)
		{
			uiactionIcon = gameObject.AddComponent<UIActionIcon>();
		}
		uiactionIcon.SetObject(action.logic, vars);
		return uiactionIcon;
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000023FD File Offset: 0x000005FD
	public void SetSkin(string v)
	{
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000DF39C File Offset: 0x000DD59C
	public void Disable()
	{
		if (this.Data != null && (this.Data.logic.state == Action.State.PickingArgs || this.Data.logic.state == Action.State.PickingTarget))
		{
			this.Data.logic.Cancel(false, true);
		}
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000DF3EC File Offset: 0x000DD5EC
	public static UIActionIcon Possess(ActionVisuals action, GameObject host, Vars vars)
	{
		if (action == null)
		{
			return null;
		}
		if (host == null)
		{
			return null;
		}
		UIActionIcon uiactionIcon = host.GetComponent<UIActionIcon>();
		if (uiactionIcon == null)
		{
			uiactionIcon = host.AddComponent<UIActionIcon>();
		}
		uiactionIcon.SetObject(action.logic, vars);
		return uiactionIcon;
	}

	// Token: 0x04000E29 RID: 3625
	[UIFieldTarget("id_ActionLabel")]
	private TextMeshProUGUI Label;

	// Token: 0x04000E2A RID: 3626
	[UIFieldTarget("id_ActionIcon")]
	private Image Image_Icon;

	// Token: 0x04000E2B RID: 3627
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x04000E2C RID: 3628
	[UIFieldTarget("id_Glow")]
	private Image Image_Glow;

	// Token: 0x04000E2D RID: 3629
	[UIFieldTarget("id_Background")]
	private Image Image_Background;

	// Token: 0x04000E2E RID: 3630
	[UIFieldTarget("id_ProgressContainer")]
	private GameObject Object_Progress_Container;

	// Token: 0x04000E2F RID: 3631
	[UIFieldTarget("id_Progress")]
	private Image Image_Progress;

	// Token: 0x04000E35 RID: 3637
	public Logic.Object override_target_object;

	// Token: 0x04000E38 RID: 3640
	public Action<UIActionIcon, PointerEventData> OnSelect;

	// Token: 0x04000E39 RID: 3641
	public Action<UIActionIcon> OnStateChange;

	// Token: 0x04000E3A RID: 3642
	private BSGButton m_Button;

	// Token: 0x04000E3B RID: 3643
	private bool m_Initialized;

	// Token: 0x04000E3C RID: 3644
	private Vars tooltipVars = new Vars();

	// Token: 0x020006C6 RID: 1734
	public enum State
	{
		// Token: 0x040036F7 RID: 14071
		Undefined,
		// Token: 0x040036F8 RID: 14072
		Valid,
		// Token: 0x040036F9 RID: 14073
		Invalid,
		// Token: 0x040036FA RID: 14074
		InvalidActioInprogress,
		// Token: 0x040036FB RID: 14075
		PickingTarget,
		// Token: 0x040036FC RID: 14076
		PickingArgs,
		// Token: 0x040036FD RID: 14077
		Preparing,
		// Token: 0x040036FE RID: 14078
		Running,
		// Token: 0x040036FF RID: 14079
		Finishing,
		// Token: 0x04003700 RID: 14080
		OnCooldown
	}
}
