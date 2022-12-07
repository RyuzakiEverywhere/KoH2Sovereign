using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020002C0 RID: 704
public class SettlementIcon : ObjectIcon
{
	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06002C2E RID: 11310 RVA: 0x0017252D File Offset: 0x0017072D
	// (set) Token: 0x06002C2F RID: 11311 RVA: 0x00172535 File Offset: 0x00170735
	public global::Settlement settlement { get; private set; }

	// Token: 0x06002C30 RID: 11312 RVA: 0x00172540 File Offset: 0x00170740
	public override void SetObject(object obj, Vars vars)
	{
		base.SetObject(obj, vars);
		Logic.Settlement settlement = obj as Logic.Settlement;
		if (settlement != null)
		{
			this.settlement = (settlement.visuals as global::Settlement);
		}
		Tooltip.Get(base.gameObject, true).SetObj(obj, null, null);
		this.UpdateShield();
		this.UpdateHighlight();
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x00172590 File Offset: 0x00170790
	public void SetSettlement(global::Settlement settlement)
	{
		this.SetObject(settlement.logic, null);
		Tooltip.Get(base.gameObject, true).SetObj((settlement == null) ? null : settlement.logic, null, null);
		this.UpdateShield();
		this.UpdateHighlight();
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x001725D0 File Offset: 0x001707D0
	public void UpdateShield()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.ui == null)
		{
			this.ui = WorldUI.Get();
		}
		if (this.ui == null)
		{
			return;
		}
		Transform transform = base.transform.Find("Shield");
		if (transform == null)
		{
			return;
		}
		if (this.settlement == null || this.settlement.logic == null)
		{
			transform.gameObject.SetActive(false);
			return;
		}
		int kingdom_id = this.settlement.logic.kingdom_id;
		if (kingdom_id == this.ui.kingdom)
		{
			transform.gameObject.SetActive(false);
			return;
		}
		transform.gameObject.SetActive(true);
		UIKingdomIcon component = global::Common.GetComponent<UIKingdomIcon>(transform, null);
		if (component != null)
		{
			global::Kingdom kingdom = global::Kingdom.Get(kingdom_id);
			if (kingdom != null)
			{
				component.SetObject(kingdom.logic, null);
			}
		}
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x001726B8 File Offset: 0x001708B8
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
		if (this.settlement == null || this.settlement.logic == null)
		{
			component.enabled = false;
			color = this.ui.selectionSettings.neutralColor;
		}
		else
		{
			component.enabled = true;
			color = this.ui.GetStanceColor(this.settlement.logic, true);
		}
		if (this.mouse_in)
		{
			color *= 1.2f;
			color.a = 1f;
		}
		component.color = color;
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x00172759 File Offset: 0x00170959
	public void UpdateAll()
	{
		if (this.ui == null || this.settlement == null)
		{
			return;
		}
		this.UpdateShield();
		this.UpdateHighlight();
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x00172784 File Offset: 0x00170984
	protected override void OnEnable()
	{
		base.OnEnable();
		this.ui = WorldUI.Get();
		this.UpdateAll();
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x0017279D File Offset: 0x0017099D
	private void OnValidate()
	{
		if (this.ui == null)
		{
			return;
		}
		this.UpdateAll();
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x001727B4 File Offset: 0x001709B4
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x001727C3 File Offset: 0x001709C3
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x001727D4 File Offset: 0x001709D4
	public override void OnClick(PointerEventData e)
	{
		if (this.ui == null)
		{
			return;
		}
		if (e.button == PointerEventData.InputButton.Left && this.settlement != null && this.settlement.gameObject != null)
		{
			if (e.clickCount <= 1)
			{
				this.ui.SelectObj(this.settlement.gameObject, false, true, true, true);
				return;
			}
			this.ui.LookAt(this.settlement.gameObject.transform.position, false);
		}
	}

	// Token: 0x04001E2C RID: 7724
	private WorldUI ui;
}
