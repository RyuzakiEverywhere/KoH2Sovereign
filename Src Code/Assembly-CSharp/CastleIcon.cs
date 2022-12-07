using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020001E1 RID: 481
public class CastleIcon : ObjectIcon
{
	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06001CB3 RID: 7347 RVA: 0x00110524 File Offset: 0x0010E724
	// (set) Token: 0x06001CB4 RID: 7348 RVA: 0x0011052C File Offset: 0x0010E72C
	public Castle Castle { get; private set; }

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x00110535 File Offset: 0x0010E735
	// (set) Token: 0x06001CB6 RID: 7350 RVA: 0x0011053D File Offset: 0x0010E73D
	public CastleIcon.State state { get; private set; }

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06001CB7 RID: 7351 RVA: 0x00110546 File Offset: 0x0010E746
	// (set) Token: 0x06001CB8 RID: 7352 RVA: 0x0011054E File Offset: 0x0010E74E
	public DT.Field state_def { get; private set; }

	// Token: 0x17000181 RID: 385
	// (get) Token: 0x06001CB9 RID: 7353 RVA: 0x00110557 File Offset: 0x0010E757
	// (set) Token: 0x06001CBA RID: 7354 RVA: 0x0011055F File Offset: 0x0010E75F
	public DT.Field icon_def { get; private set; }

	// Token: 0x06001CBB RID: 7355 RVA: 0x00110568 File Offset: 0x0010E768
	private void Init()
	{
		if (this.m_Intialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_UI = BaseUI.Get();
		this.m_Intialized = true;
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0011058C File Offset: 0x0010E78C
	public override void SetObject(object obj, Vars vars)
	{
		this.Init();
		base.SetObject(obj, vars);
		this.Castle = (obj as Castle);
		base.enabled = (this.Castle != null);
		Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
		this.UpdateHighlight();
		this.UpdateState();
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x001105E2 File Offset: 0x0010E7E2
	private void Update()
	{
		if (this.Castle == null)
		{
			base.enabled = false;
			return;
		}
		this.UpdateState();
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x001105FC File Offset: 0x0010E7FC
	public void UpdateState()
	{
		CastleIcon.State state = this.DecideState();
		this.SetState(state);
		this.UpdateIcon();
		if (this.m_Governor != null && this.showGovernor)
		{
			if (this.m_Governor.Data != this.Castle.governor)
			{
				this.m_Governor.SetObject(this.Castle.governor, null);
			}
			this.m_Governor.gameObject.SetActive(this.m_Governor.Data != null);
		}
		bool active = !this.showGovernor || this.Castle.governor == null;
		if (this.m_Icon != null)
		{
			this.m_Icon.gameObject.SetActive(active);
		}
		if (this.m_Border != null)
		{
			this.m_Border.gameObject.SetActive(active);
		}
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x001106D8 File Offset: 0x0010E8D8
	private void UpdateIcon()
	{
		if (this.state_def == null)
		{
			return;
		}
		int citadelLevel = this.Castle.GetCitadelLevel();
		if (this.m_Icon != null)
		{
			Sprite obj = global::Defs.GetObj<Sprite>(citadelLevel, this.icon_def, "icon", null);
			if (obj != null)
			{
				this.m_Icon.overrideSprite = obj;
			}
			else
			{
				this.m_Icon.overrideSprite = null;
			}
			this.m_Icon.color = global::Defs.GetColor(this.state_def, "icon_color", null);
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
		if (this.m_StatusIcon != null)
		{
			Sprite obj3 = global::Defs.GetObj<Sprite>(this.state_def, "status_icon", null);
			if (obj3 != null)
			{
				this.m_StatusIcon.overrideSprite = obj3;
			}
			else
			{
				this.m_StatusIcon.overrideSprite = null;
			}
			this.m_StatusIcon.color = global::Defs.GetColor(this.state_def, "status_color", null);
			if (this.m_Status != null)
			{
				this.m_Status.SetActive(this.state_def.GetBool("show_status", null, false, true, true, true, '.'));
			}
		}
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0011084C File Offset: 0x0010EA4C
	public void SetState(CastleIcon.State state)
	{
		if (this.state == state && this.state_def != null)
		{
			return;
		}
		this.state = state;
		if (this.icon_def == null)
		{
			this.icon_def = global::Defs.GetDefField("CastleIcon", null);
		}
		if (this.icon_def != null)
		{
			this.state_def = this.icon_def.FindChild(state.ToString(), null, true, true, true, '.');
			if (this.state_def == null)
			{
				Debug.LogWarning(string.Format("{0}: undefined state '{1}'", this, state));
				this.state_def = this.icon_def.FindChild("State", null, true, true, true, '.');
				return;
			}
		}
		else
		{
			this.state_def = null;
		}
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x001108FC File Offset: 0x0010EAFC
	public CastleIcon.State DecideState()
	{
		Castle castle = this.Castle;
		if (castle == null)
		{
			return CastleIcon.State.Normal;
		}
		if (castle.battle != null)
		{
			return CastleIcon.State.Siege;
		}
		Logic.Realm realm = castle.GetRealm();
		if (realm != null && realm.IsDisorder())
		{
			return CastleIcon.State.Disorder;
		}
		Logic.Realm realm2 = castle.GetRealm();
		if (realm2 != null && realm2.IsOccupied())
		{
			return CastleIcon.State.Occupation;
		}
		return CastleIcon.State.Normal;
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0011094C File Offset: 0x0010EB4C
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		Image component = base.GetComponent<Image>();
		if (component == null)
		{
			return;
		}
		Color color;
		if (this.Castle == null)
		{
			component.enabled = false;
			color = this.m_UI.selectionSettings.neutralColor;
		}
		else
		{
			component.enabled = true;
			color = this.m_UI.GetStanceColor(this.Castle, true);
		}
		if (this.mouse_in)
		{
			color *= 1.2f;
			color.a = 1f;
		}
		component.color = color;
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x001109D5 File Offset: 0x0010EBD5
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x001109E4 File Offset: 0x0010EBE4
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x001109F4 File Offset: 0x0010EBF4
	public override void OnClick(PointerEventData e)
	{
		if (this.m_UI == null)
		{
			return;
		}
		if (e.button == PointerEventData.InputButton.Left && this.Castle != null && this.Castle.visuals != null)
		{
			if (e.clickCount <= 1)
			{
				this.m_UI.SelectObj((this.Castle.visuals as global::Settlement).gameObject, false, true, true, true);
				return;
			}
			this.m_UI.LookAt((this.Castle.visuals as global::Settlement).transform.position, false);
		}
	}

	// Token: 0x040012B6 RID: 4790
	[UIFieldTarget("id_Icon")]
	private Image m_Icon;

	// Token: 0x040012B7 RID: 4791
	[UIFieldTarget("id_Status")]
	private GameObject m_Status;

	// Token: 0x040012B8 RID: 4792
	[UIFieldTarget("id_StatustIcon")]
	private Image m_StatusIcon;

	// Token: 0x040012B9 RID: 4793
	[UIFieldTarget("id_Border")]
	private Image m_Border;

	// Token: 0x040012BA RID: 4794
	[UIFieldTarget("id_Governor")]
	private UICharacterIcon m_Governor;

	// Token: 0x040012BF RID: 4799
	public bool showGovernor;

	// Token: 0x040012C0 RID: 4800
	private BaseUI m_UI;

	// Token: 0x040012C1 RID: 4801
	private bool m_Intialized;

	// Token: 0x02000722 RID: 1826
	public enum State
	{
		// Token: 0x04003870 RID: 14448
		Normal,
		// Token: 0x04003871 RID: 14449
		Siege,
		// Token: 0x04003872 RID: 14450
		Disorder,
		// Token: 0x04003873 RID: 14451
		Occupation
	}
}
