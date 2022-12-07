using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000282 RID: 642
public class UIDiplomacyOfferMessage : MonoBehaviour, IListener
{
	// Token: 0x0600273A RID: 10042 RVA: 0x00155535 File Offset: 0x00153735
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x00155544 File Offset: 0x00153744
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x00155555 File Offset: 0x00153755
	private void OnDisable()
	{
		Logic.Kingdom kingdom = this.kingdom;
		if (kingdom == null)
		{
			return;
		}
		kingdom.DelListener(this);
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x00155568 File Offset: 0x00153768
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x00155578 File Offset: 0x00153778
	private void ExtractData()
	{
		this.m_DataExtracted = true;
		BaseUI baseUI = BaseUI.Get();
		MessageWnd messageWnd = ((baseUI != null) ? baseUI.window_dispatcher.GetFocusWindow() : null) as MessageWnd;
		if (messageWnd != null)
		{
			this.SetData(messageWnd.def_field, messageWnd.vars, messageWnd);
		}
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x001555C4 File Offset: 0x001537C4
	public void RefreshKing()
	{
		if (this.kingdom == null)
		{
			return;
		}
		if (this.m_Sovereign != null)
		{
			this.m_Sovereign.SetObject(this.kingdom.royalFamily.Sovereign, null);
			this.m_Sovereign.ShowCrest(false);
			this.m_Sovereign.ShowStatus(false);
		}
		if (this.m_SovereignName != null)
		{
			UIText.SetTextKey(this.m_SovereignName, "Character.title_name", new Vars(this.kingdom.royalFamily.Sovereign), null);
		}
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x00155658 File Offset: 0x00153858
	public void SetData(DT.Field def_field, Vars vars, MessageWnd message)
	{
		UICommon.FindComponents(this, false);
		this.kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
		Logic.Kingdom kingdom = vars.Get<Logic.Kingdom>("plr_kingdom", null);
		if (kingdom == null)
		{
			kingdom = GameLogic.Get(true).vars.Get<Logic.Kingdom>("plr_kingdom", null);
		}
		if (kingdom == this.kingdom)
		{
			this.kingdom = vars.Get<Logic.Kingdom>("target_kingdom", null);
		}
		if (this.kingdom != null)
		{
			this.kingdom.AddListener(this);
			this.RefreshKing();
			if (this.m_kingdom != null)
			{
				this.m_kingdom.SetObject(this.kingdom, null);
			}
			if (this.m_Relations != null)
			{
				this.m_Relations.gameObject.SetActive(kingdom != null);
				this.m_Relations.SetData(kingdom, this.kingdom);
			}
		}
		UIText.SetTextKey(this.m_KingdomName, "Kingdom.name", new Vars(this.kingdom), null);
		UIText.ForceNextLinks(UIText.LinkSettings.Mode.NotColorized);
		UIText.SetText(this.Value_Caption, def_field, "caption", vars, null);
		UIText.SetText(this.Value_Body, def_field, "body", vars, null);
		Sprite obj = global::Defs.GetObj<Sprite>(def_field, "illustration", vars);
		if (obj != null && this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = obj;
		}
		this.offer = (vars.obj.obj_val as Offer);
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x001557C6 File Offset: 0x001539C6
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "royal_new_sovereign")
		{
			this.RefreshKing();
		}
	}

	// Token: 0x04001A9E RID: 6814
	[UIFieldTarget("id_KingdomShield")]
	private UIKingdomIcon m_kingdom;

	// Token: 0x04001A9F RID: 6815
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI Value_Caption;

	// Token: 0x04001AA0 RID: 6816
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI Value_Body;

	// Token: 0x04001AA1 RID: 6817
	[UIFieldTarget("id_Sovereign")]
	private UICharacterIcon m_Sovereign;

	// Token: 0x04001AA2 RID: 6818
	[UIFieldTarget("id_SovereignName")]
	private TextMeshProUGUI m_SovereignName;

	// Token: 0x04001AA3 RID: 6819
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x04001AA4 RID: 6820
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x04001AA5 RID: 6821
	[UIFieldTarget("id_Relations")]
	private UIKingdomRelations m_Relations;

	// Token: 0x04001AA6 RID: 6822
	public Logic.Kingdom kingdom;

	// Token: 0x04001AA7 RID: 6823
	private bool m_Started;

	// Token: 0x04001AA8 RID: 6824
	public Offer offer;

	// Token: 0x04001AA9 RID: 6825
	private bool m_DataExtracted;
}
