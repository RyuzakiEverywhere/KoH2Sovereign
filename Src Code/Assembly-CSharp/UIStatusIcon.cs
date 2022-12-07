using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000192 RID: 402
public class UIStatusIcon : ObjectIcon, IListener
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06001658 RID: 5720 RVA: 0x000E0D26 File Offset: 0x000DEF26
	// (set) Token: 0x06001659 RID: 5721 RVA: 0x000E0D2E File Offset: 0x000DEF2E
	public Logic.Status Data { get; set; }

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x0600165A RID: 5722 RVA: 0x000E0D37 File Offset: 0x000DEF37
	public bool Selected
	{
		get
		{
			return this.m_Selected;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x0600165B RID: 5723 RVA: 0x000E0D3F File Offset: 0x000DEF3F
	// (set) Token: 0x0600165C RID: 5724 RVA: 0x000E0D47 File Offset: 0x000DEF47
	public UIStatusIcon.State state { get; private set; }

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x0600165D RID: 5725 RVA: 0x000E0D50 File Offset: 0x000DEF50
	// (set) Token: 0x0600165E RID: 5726 RVA: 0x000E0D58 File Offset: 0x000DEF58
	public DT.Field state_def { get; private set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x0600165F RID: 5727 RVA: 0x000E0D61 File Offset: 0x000DEF61
	// (set) Token: 0x06001660 RID: 5728 RVA: 0x000E0D69 File Offset: 0x000DEF69
	public DT.Field slot_def { get; private set; }

	// Token: 0x06001661 RID: 5729 RVA: 0x000E0D72 File Offset: 0x000DEF72
	public override void Awake()
	{
		base.Awake();
		this.awakePassed = true;
		if (this.addListeners && this.Data != null)
		{
			this.Data.AddListener(this);
		}
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x000E0D9D File Offset: 0x000DEF9D
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x000E0DB8 File Offset: 0x000DEFB8
	public override void SetObject(object obj, Vars vars = null)
	{
		base.SetObject(obj, vars);
		this.Init();
		Logic.Status data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		if (obj is Logic.Status)
		{
			this.Data = (obj as Logic.Status);
		}
		else
		{
			this.Data = null;
		}
		if (this.awakePassed)
		{
			if (this.Data != null)
			{
				this.Data.AddListener(this);
			}
		}
		else
		{
			this.addListeners = true;
		}
		if (this.Data != null)
		{
			base.enabled = true;
		}
		this.PopulateTargetIcon();
		this.SetupTooltip();
		this.Populate();
		this.UpdateState();
	}

	// Token: 0x06001664 RID: 5732 RVA: 0x000E0E50 File Offset: 0x000DF050
	private void SetupTooltip()
	{
		Tooltip tooltip = Tooltip.Get(base.gameObject, true);
		tooltip.SetDef("StatusTooltip", null);
		if (this.Data != null)
		{
			Vars vars = new Vars(this.Data);
			tooltip.SetVars(vars);
			tooltip.SetObj(this.Data, null, null);
		}
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x000E0EA4 File Offset: 0x000DF0A4
	public void KeepAlive(bool keepAlive)
	{
		this.m_keepAlive = keepAlive;
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000E0EAD File Offset: 0x000DF0AD
	public void Select(bool value)
	{
		this.m_Selected = value;
		this.UpdateHighlight();
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x000E0EBC File Offset: 0x000DF0BC
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x000E0ED4 File Offset: 0x000DF0D4
	public void Populate()
	{
		if (this.Image_Icon != null)
		{
			Logic.Status status = this.logicObject as Logic.Status;
			DT.Field field;
			if (status == null)
			{
				field = null;
			}
			else
			{
				Logic.Status.Def def = status.def;
				field = ((def != null) ? def.field : null);
			}
			Sprite obj = global::Defs.GetObj<Sprite>(field, "icon", this.vars);
			this.Image_Icon.overrideSprite = obj;
		}
		else
		{
			Debug.Log("Missing icon for object" + this.logicObject);
		}
		Tooltip.Get(base.gameObject, true).Refresh();
		this.UpdateHighlight();
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x000E0F60 File Offset: 0x000DF160
	private void PopulateTargetIcon()
	{
		if (this.m_TargetCharacterContainer == null)
		{
			return;
		}
		if (this.Data == null)
		{
			this.m_TargetCharacterContainer.gameObject.SetActive(false);
			return;
		}
		HasPuppetStatus hasPuppetStatus = this.Data as HasPuppetStatus;
		if (hasPuppetStatus == null)
		{
			this.m_TargetCharacterContainer.gameObject.SetActive(false);
			return;
		}
		if (hasPuppetStatus.puppet == null)
		{
			this.m_TargetCharacterContainer.gameObject.SetActive(false);
			return;
		}
		if (this.m_TargetCharacterIcon != null)
		{
			this.m_TargetCharacterIcon.SetObject(hasPuppetStatus.puppet, null);
			this.m_TargetCharacterIcon.ShowCrest(false);
			this.m_TargetCharacterIcon.ShowStatus(false);
			this.m_TargetCharacterContainer.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x000E101C File Offset: 0x000DF21C
	private void UpdateProgress()
	{
		if (this.Image_Progress == null)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		Logic.Status data = this.Data;
		if (data != null)
		{
			data.GetProgress(out num, out num2);
		}
		if (num2 != 0f)
		{
			this.Image_Progress.enabled = true;
			this.Image_Progress.fillAmount = num / num2;
			Image progressContainer = this.m_ProgressContainer;
			if (progressContainer == null)
			{
				return;
			}
			progressContainer.gameObject.SetActive(true);
			return;
		}
		else
		{
			this.Image_Progress.enabled = false;
			Image progressContainer2 = this.m_ProgressContainer;
			if (progressContainer2 == null)
			{
				return;
			}
			progressContainer2.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x000E10B4 File Offset: 0x000DF2B4
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

	// Token: 0x0600166C RID: 5740 RVA: 0x000E10D4 File Offset: 0x000DF2D4
	public void UpdateState()
	{
		UIStatusIcon.State state = this.DecideState();
		this.SetState(state);
		if (this.m_ColorizeState && this.Image_Icon != null)
		{
			this.Image_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
		}
		bool flag;
		bool flag2;
		this.GetInteractionState(out flag, out flag2);
		flag |= (state == UIStatusIcon.State.Running);
		flag2 &= (state == UIStatusIcon.State.Valid);
		if (this.Image_Border != null)
		{
			this.Image_Border.color = global::Defs.GetColor(this.slot_def, flag ? "border_color.interactable" : "border_color.non_interactable", null);
		}
		if (this.Image_ActionArrow != null)
		{
			this.Image_ActionArrow.gameObject.SetActive(flag2);
		}
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x000E118C File Offset: 0x000DF38C
	public void SetState(UIStatusIcon.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.slot_def == null)
		{
			this.slot_def = global::Defs.GetDefField("StatusIcon", null);
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

	// Token: 0x0600166E RID: 5742 RVA: 0x000E123C File Offset: 0x000DF43C
	public UIStatusIcon.State DecideState()
	{
		if (this.Data == null)
		{
			return UIStatusIcon.State.Undefined;
		}
		float num;
		float num2;
		this.Data.GetProgress(out num, out num2);
		if (num2 != 0f && num > 0f)
		{
			return UIStatusIcon.State.Running;
		}
		UIStatusIcon.tempDefList.Clear();
		this.Data.GetActions(UIStatusIcon.tempDefList);
		if (UIStatusIcon.tempDefList.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < UIStatusIcon.tempDefList.Count; i++)
			{
				Action.Def def = UIStatusIcon.tempDefList[i];
				Actions actions = this.Data.own_character.actions;
				Action action = (actions != null) ? actions.Find(UIStatusIcon.tempDefList[i]) : null;
				if (action == null)
				{
					Actions actions2 = this.Data.own_kingdom.actions;
					action = ((actions2 != null) ? actions2.Find(UIStatusIcon.tempDefList[i]) : null);
				}
				if (action != null && action.Validate(true) == "ok")
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return UIStatusIcon.State.Invalid;
			}
		}
		return UIStatusIcon.State.Valid;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x000E1340 File Offset: 0x000DF540
	public void GetInteractionState(out bool inetractable, out bool showExpand)
	{
		inetractable = false;
		showExpand = false;
		if (this.Data == null)
		{
			return;
		}
		UIStatusIcon.tempDefList.Clear();
		this.Data.GetActions(UIStatusIcon.tempDefList);
		if (UIStatusIcon.tempDefList.Count == 0)
		{
			return;
		}
		inetractable = true;
		bool @bool = this.Data.def.field.GetBool("show_singleton_action", this.Data, false, true, true, true, '.');
		showExpand = (UIStatusIcon.tempDefList.Count != 1 || @bool);
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x000E13C3 File Offset: 0x000DF5C3
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.OnSelect != null)
		{
			this.OnSelect(this);
		}
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x000E13E0 File Offset: 0x000DF5E0
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x000E13EF File Offset: 0x000DF5EF
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x000E1400 File Offset: 0x000DF600
	public void UpdateHighlight()
	{
		if (this.Image_Glow != null)
		{
			this.Image_Glow.gameObject.SetActive(this.mouse_in || this.m_Selected);
		}
		if (this.Group_Seleced != null)
		{
			this.Group_Seleced.gameObject.SetActive(this.m_Selected);
		}
		if (this.m_FocusedBorder != null)
		{
			bool flag;
			bool flag2;
			this.GetInteractionState(out flag, out flag2);
			this.m_FocusedBorder.gameObject.SetActive((this.mouse_in || this.m_Selected) && flag);
		}
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x000E149C File Offset: 0x000DF69C
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		(obj as Logic.Object).DelListener(this);
		if (!this.m_keepAlive)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.SetObject(null, null);
	}

	// Token: 0x04000E6A RID: 3690
	[UIFieldTarget("id_StatusIcon")]
	private Image Image_Icon;

	// Token: 0x04000E6B RID: 3691
	[UIFieldTarget("id_Glow")]
	private Image Image_Glow;

	// Token: 0x04000E6C RID: 3692
	[UIFieldTarget("id_FocusedBorder")]
	private Image m_FocusedBorder;

	// Token: 0x04000E6D RID: 3693
	[UIFieldTarget("id_ProgressContainer")]
	private Image m_ProgressContainer;

	// Token: 0x04000E6E RID: 3694
	[UIFieldTarget("id_Progress")]
	private Image Image_Progress;

	// Token: 0x04000E6F RID: 3695
	[UIFieldTarget("id_GroupSelected")]
	private GameObject Group_Seleced;

	// Token: 0x04000E70 RID: 3696
	[UIFieldTarget("id_TargetCharacterContainer")]
	private RectTransform m_TargetCharacterContainer;

	// Token: 0x04000E71 RID: 3697
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon m_TargetCharacterIcon;

	// Token: 0x04000E72 RID: 3698
	[UIFieldTarget("id_Border")]
	private Image Image_Border;

	// Token: 0x04000E73 RID: 3699
	[UIFieldTarget("id_ActionArrow")]
	private Image Image_ActionArrow;

	// Token: 0x04000E75 RID: 3701
	public Action<UIStatusIcon> OnSelect;

	// Token: 0x04000E76 RID: 3702
	private bool m_Selected;

	// Token: 0x04000E7A RID: 3706
	public bool m_keepAlive;

	// Token: 0x04000E7B RID: 3707
	public bool m_ColorizeState = true;

	// Token: 0x04000E7C RID: 3708
	private bool awakePassed;

	// Token: 0x04000E7D RID: 3709
	private bool addListeners;

	// Token: 0x04000E7E RID: 3710
	private bool m_Initialzied;

	// Token: 0x04000E7F RID: 3711
	private static List<Action.Def> tempDefList = new List<Action.Def>();

	// Token: 0x020006CB RID: 1739
	public enum State
	{
		// Token: 0x04003713 RID: 14099
		Undefined,
		// Token: 0x04003714 RID: 14100
		Valid,
		// Token: 0x04003715 RID: 14101
		Invalid,
		// Token: 0x04003716 RID: 14102
		Running
	}
}
