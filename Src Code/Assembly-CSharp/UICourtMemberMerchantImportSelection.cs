using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200028C RID: 652
public class UICourtMemberMerchantImportSelection : MonoBehaviour, IListener
{
	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060027EE RID: 10222 RVA: 0x00158965 File Offset: 0x00156B65
	// (set) Token: 0x060027EF RID: 10223 RVA: 0x0015896D File Offset: 0x00156B6D
	public DT.Def Def { get; private set; }

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060027F0 RID: 10224 RVA: 0x00158976 File Offset: 0x00156B76
	// (set) Token: 0x060027F1 RID: 10225 RVA: 0x0015897E File Offset: 0x00156B7E
	public Logic.Character.ImportedGood Good { get; private set; }

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060027F2 RID: 10226 RVA: 0x00158987 File Offset: 0x00156B87
	// (set) Token: 0x060027F3 RID: 10227 RVA: 0x0015898F File Offset: 0x00156B8F
	public int Slot { get; private set; }

	// Token: 0x060027F4 RID: 10228 RVA: 0x00158998 File Offset: 0x00156B98
	public void SetData(UIMerchantImportGoodIcon icon, Logic.Character c)
	{
		this.Icon = icon;
		int slot = icon.Slot;
		this.Init();
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		this.Character = c;
		this.Character.AddListener(this);
		this.Good = this.Character.GetImportedGood(slot);
		this.Slot = slot;
		if (string.IsNullOrEmpty(this.Good.name))
		{
			this.Def = null;
			return;
		}
		this.Def = global::Defs.GetDefField(this.Good.name, null).def;
		this.Refresh();
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x00158A33 File Offset: 0x00156C33
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initalized = true;
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x00158A4C File Offset: 0x00156C4C
	private void Refresh()
	{
		Logic.Kingdom mission_kingdom = this.Character.mission_kingdom;
		if (this.m_ResourceName != null)
		{
			UIText.SetText(this.m_ResourceName, global::Defs.Localize(this.Def.field, "name", null, null, true, true));
		}
		if (this.m_ImportFromText != null)
		{
			UIText.SetTextKey(this.m_ImportFromText, "CourtMemberMerchantImportSelection.importFrom", new Vars(mission_kingdom), null);
		}
		if (this.m_Description != null)
		{
			Vars vars = new Vars();
			vars.Set<Logic.Kingdom>("kingdom", mission_kingdom);
			vars.Set<Logic.Character>("character", this.Character);
			vars.Set<string>("good", "#" + global::Defs.Localize(this.Def.field, "name", null, null, true, true));
			UIText.SetTextKey(this.m_Description, "CourtMemberMerchantImportSelection.description", vars, null);
		}
		if (this.m_CostText != null)
		{
			Vars vars2 = new Vars(this.Character.CalcImportGoodUpkeep(this.Good));
			if (this.Good.discount != 0f)
			{
				vars2.Set<float>("discount", this.Good.discount);
			}
			UIText.SetTextKey(this.m_CostText, "CourtMemberMerchantImportSelection.upkeep", vars2, null);
		}
		if (this.m_CancelImportLabel != null)
		{
			UIText.SetTextKey(this.m_CancelImportLabel, "CourtMemberMerchantImportSelection.stopImport", null, null);
		}
		if (this.m_KingdomShield != null)
		{
			this.m_KingdomShield.SetObject(mission_kingdom, null);
		}
		if (this.m_CancelImport != null)
		{
			Logic.Character character = this.Character;
			object obj;
			if (character == null)
			{
				obj = null;
			}
			else
			{
				Actions actions = character.actions;
				obj = ((actions != null) ? actions.Find("CancelImportGoodAction") : null);
			}
			CancelImportGoodAction cancelImportGoodAction = obj as CancelImportGoodAction;
			if (cancelImportGoodAction != null)
			{
				UIActionIcon.Possess(cancelImportGoodAction.visuals as ActionVisuals, this.m_CancelImport, null).OnSelect = new Action<UIActionIcon, PointerEventData>(this.ActionStartSelectSlot);
			}
		}
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x00158C33 File Offset: 0x00156E33
	private void ActionStartSelectSlot(UIActionIcon icon, PointerEventData ev)
	{
		(icon.Data.logic as CancelImportGoodAction).args = new List<Value>(1)
		{
			this.Slot
		};
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x00158C61 File Offset: 0x00156E61
	public void Close()
	{
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x00158C80 File Offset: 0x00156E80
	private void OnDestroy()
	{
		Logic.Character character = this.Character;
		if (character == null)
		{
			return;
		}
		character.DelListener(this);
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x00158C93 File Offset: 0x00156E93
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "run_cancel_import_good_action")
		{
			UIMerchantImportGoodIcon icon = this.Icon;
			if (icon == null)
			{
				return;
			}
			Action<UIMerchantImportGoodIcon, PointerEventData> onSelect = icon.OnSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this.Icon, null);
		}
	}

	// Token: 0x04001B1B RID: 6939
	[UIFieldTarget("id_ResourceName")]
	private TextMeshProUGUI m_ResourceName;

	// Token: 0x04001B1C RID: 6940
	[UIFieldTarget("id_ImportFromText")]
	private TextMeshProUGUI m_ImportFromText;

	// Token: 0x04001B1D RID: 6941
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x04001B1E RID: 6942
	[UIFieldTarget("id_CostText")]
	private TextMeshProUGUI m_CostText;

	// Token: 0x04001B1F RID: 6943
	[UIFieldTarget("id_CancelImportLabel")]
	private TextMeshProUGUI m_CancelImportLabel;

	// Token: 0x04001B20 RID: 6944
	[UIFieldTarget("id_KingdomShield")]
	private UIKingdomIcon m_KingdomShield;

	// Token: 0x04001B21 RID: 6945
	[UIFieldTarget("id_CancelImport")]
	private GameObject m_CancelImport;

	// Token: 0x04001B24 RID: 6948
	private Logic.Character Character;

	// Token: 0x04001B26 RID: 6950
	private UIMerchantImportGoodIcon Icon;

	// Token: 0x04001B27 RID: 6951
	private bool m_Initalized;
}
