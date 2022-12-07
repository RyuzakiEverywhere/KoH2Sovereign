using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000190 RID: 400
public class UIMerchantImportGoodIcon : Hotspot
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06001624 RID: 5668 RVA: 0x000DFFBA File Offset: 0x000DE1BA
	// (set) Token: 0x06001625 RID: 5669 RVA: 0x000DFFC2 File Offset: 0x000DE1C2
	public DT.Def Def { get; private set; }

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06001626 RID: 5670 RVA: 0x000DFFCB File Offset: 0x000DE1CB
	// (set) Token: 0x06001627 RID: 5671 RVA: 0x000DFFD3 File Offset: 0x000DE1D3
	public Logic.Character.ImportedGood good { get; private set; }

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06001628 RID: 5672 RVA: 0x000DFFDC File Offset: 0x000DE1DC
	// (set) Token: 0x06001629 RID: 5673 RVA: 0x000DFFE4 File Offset: 0x000DE1E4
	public Logic.Character Character { get; private set; }

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x0600162A RID: 5674 RVA: 0x000DFFED File Offset: 0x000DE1ED
	// (set) Token: 0x0600162B RID: 5675 RVA: 0x000DFFF5 File Offset: 0x000DE1F5
	public int Slot { get; private set; }

	// Token: 0x0600162C RID: 5676 RVA: 0x000E0000 File Offset: 0x000DE200
	public void SetData(int slot, Logic.Character character)
	{
		this.Slot = slot;
		if (character == null)
		{
			return;
		}
		this.Init();
		this.Character = character;
		this.good = this.Character.GetImportedGood(slot);
		if (this.good.name == null)
		{
			this.Def = null;
		}
		else
		{
			DT.Field defField = global::Defs.GetDefField(this.good.name, null);
			this.Def = ((defField != null) ? defField.def : null);
		}
		this.Populate();
		this.UpdateState();
		this.UpdateHighlight();
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x000E0082 File Offset: 0x000DE282
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.OnSelect = (Action<UIMerchantImportGoodIcon, PointerEventData>)Delegate.Combine(this.OnSelect, new Action<UIMerchantImportGoodIcon, PointerEventData>(this.Select));
		this.m_Initialzed = true;
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x000E00C0 File Offset: 0x000DE2C0
	private void Populate()
	{
		UIActionIcon empty = this.m_Empty;
		if (empty != null)
		{
			empty.gameObject.SetActive(this.Def == null);
		}
		GameObject populated = this.m_Populated;
		if (populated != null)
		{
			populated.gameObject.SetActive(this.Def != null);
		}
		if (this.Def != null)
		{
			Resource resource = this.Character.CalcImportGoodUpkeep(this.good);
			if (this.m_Icon != null)
			{
				this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.Def.field, "icon", null);
			}
			if (this.m_UpkeepLabel != null)
			{
				UIText.SetTextKey(this.m_UpkeepLabel, "MerchantImportGoodIcon.upkeep", new Vars(resource), null);
			}
			Vars vars = new Vars(this.Def);
			vars.Set<float>("gold_upkeep", resource[ResourceType.Gold]);
			vars.Set<float>("trade_upkeep", resource[ResourceType.Trade]);
			Tooltip.Get(base.gameObject, true).SetDef("MerchantImportedGoodIconTooltip", vars);
			return;
		}
		if (this.m_Empty != null)
		{
			this.m_Empty.SetObject(this.Character.actions.Find("ImportGoodAction"), null);
			UIActionIcon empty2 = this.m_Empty;
			empty2.OnSelect = (Action<UIActionIcon, PointerEventData>)Delegate.Combine(empty2.OnSelect, new Action<UIActionIcon, PointerEventData>(this.ChangeActionSlot));
		}
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000E0220 File Offset: 0x000DE420
	private void ChangeActionSlot(UIActionIcon icon, PointerEventData ev)
	{
		(icon.Data.logic as ImportGoodAction).args = new List<Value>
		{
			Value.Unknown,
			this.Slot
		};
		Action<UIMerchantImportGoodIcon, PointerEventData> onSelect = this.OnSelect;
		if (onSelect == null)
		{
			return;
		}
		onSelect(this, ev);
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000E0275 File Offset: 0x000DE475
	public void UpdateState()
	{
		this.UpdateHighlight();
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000E027D File Offset: 0x000DE47D
	public void Select(bool select)
	{
		this.m_Selected = (select && this.Def != null);
		this.UpdateHighlight();
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x000023FD File Offset: 0x000005FD
	public void Select(UIMerchantImportGoodIcon icon, PointerEventData ev)
	{
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x000E029A File Offset: 0x000DE49A
	public void UpdateHighlight()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_Glow != null)
		{
			this.m_Glow.gameObject.SetActive(this.mouse_in || this.m_Selected);
		}
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000E02D3 File Offset: 0x000DE4D3
	public override void OnClick(PointerEventData e)
	{
		this.UpdateHighlight();
		Action<UIMerchantImportGoodIcon, PointerEventData> onSelect = this.OnSelect;
		if (onSelect == null)
		{
			return;
		}
		onSelect(this, e);
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000E02ED File Offset: 0x000DE4ED
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x000E02FC File Offset: 0x000DE4FC
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.UpdateHighlight();
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000E030B File Offset: 0x000DE50B
	private void Clear()
	{
		this.m_Selected = false;
		this.Def = null;
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x000E031B File Offset: 0x000DE51B
	public bool IsSelected()
	{
		return this.m_Selected;
	}

	// Token: 0x04000E4B RID: 3659
	[UIFieldTarget("id_Empty")]
	private UIActionIcon m_Empty;

	// Token: 0x04000E4C RID: 3660
	[UIFieldTarget("id_Populated")]
	private GameObject m_Populated;

	// Token: 0x04000E4D RID: 3661
	[UIFieldTarget("id_ResourceIcon")]
	private Image m_Icon;

	// Token: 0x04000E4E RID: 3662
	[UIFieldTarget("id_UpkeepLabel")]
	private TextMeshProUGUI m_UpkeepLabel;

	// Token: 0x04000E4F RID: 3663
	[UIFieldTarget("id_Glow")]
	private GameObject m_Glow;

	// Token: 0x04000E50 RID: 3664
	private List<UIMerchantImportGoodIcon> siblings = new List<UIMerchantImportGoodIcon>();

	// Token: 0x04000E51 RID: 3665
	public Action<UIMerchantImportGoodIcon, PointerEventData> OnSelect;

	// Token: 0x04000E52 RID: 3666
	private bool m_Initialzed;

	// Token: 0x04000E53 RID: 3667
	private bool m_Selected;
}
