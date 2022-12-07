using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000280 RID: 640
public class UIAudienceInaccessible : MonoBehaviour
{
	// Token: 0x06002721 RID: 10017 RVA: 0x00154DE9 File Offset: 0x00152FE9
	private void Start()
	{
		this.ExtractData();
		this.m_Started = true;
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x00154DF8 File Offset: 0x00152FF8
	private void OnEnable()
	{
		if (this.m_Started)
		{
			this.m_DataExtracted = false;
		}
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x00154E09 File Offset: 0x00153009
	private void LateUpdate()
	{
		if (!this.m_DataExtracted)
		{
			this.ExtractData();
		}
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x00154E1C File Offset: 0x0015301C
	private void ExtractData()
	{
		this.m_DataExtracted = true;
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			this.SetData(component.def_field, component.vars, component);
		}
	}

	// Token: 0x06002725 RID: 10021 RVA: 0x00154E54 File Offset: 0x00153054
	public void SetData(DT.Field def_field, Vars vars, MessageWnd message)
	{
		UICommon.FindComponents(this, false);
		Logic.Kingdom kingdom = vars.Get<Logic.Kingdom>("kingdom", null);
		Logic.Kingdom kingdom2 = vars.Get<Logic.Kingdom>("plr_kingdom", null);
		if (kingdom2 == null)
		{
			kingdom2 = GameLogic.Get(true).vars.Get<Logic.Kingdom>("plr_kingdom", null);
		}
		if (kingdom2 == kingdom)
		{
			kingdom = vars.Get<Logic.Kingdom>("target_kingdom", null);
		}
		if (kingdom != null)
		{
			if (this.m_Sovereign != null)
			{
				this.m_Sovereign.SetObject(kingdom.royalFamily.Sovereign, null);
				this.m_Sovereign.ShowCrest(false);
			}
			if (this.m_SovereignName != null)
			{
				UIText.SetTextKey(this.m_SovereignName, "Character.title_name", new Vars(kingdom.royalFamily.Sovereign), null);
			}
			if (this.m_kingdom != null)
			{
				this.m_kingdom.SetObject(kingdom, null);
			}
			if (this.m_Relations != null && kingdom2 != null)
			{
				this.m_Relations.SetData(kingdom2, kingdom);
			}
		}
		if (this.Button_Close != null)
		{
			this.Button_Close.onClick = new BSGButton.OnClick(this.HanldeOnCloseClick);
		}
		UIText.SetText(this.Value_Caption, def_field, "caption", vars, null);
		UIText.SetText(this.Value_Body, def_field, "body", vars, null);
		Sprite obj = global::Defs.GetObj<Sprite>(def_field, "illustration", vars);
		if (obj != null && this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = obj;
		}
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x00154FCC File Offset: 0x001531CC
	private void HanldeOnCloseClick(BSGButton btn)
	{
		this.HandleClose();
		this.HandleFate();
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x00154FDC File Offset: 0x001531DC
	private void HandleClose()
	{
		MessageWnd component = base.GetComponent<MessageWnd>();
		if (component != null)
		{
			component.CloseAndDismiss(true);
		}
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x00155000 File Offset: 0x00153200
	private void HandleAdditionalAction(BSGButton e)
	{
		this.HandleFate();
		this.HandleClose();
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleFate()
	{
	}

	// Token: 0x04001A86 RID: 6790
	[UIFieldTarget("id_KingdomShield")]
	private UIKingdomIcon m_kingdom;

	// Token: 0x04001A87 RID: 6791
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI Value_Caption;

	// Token: 0x04001A88 RID: 6792
	[UIFieldTarget("id_Body")]
	private TextMeshProUGUI Value_Body;

	// Token: 0x04001A89 RID: 6793
	[UIFieldTarget("id_Button_Close")]
	private BSGButton Button_Close;

	// Token: 0x04001A8A RID: 6794
	[UIFieldTarget("id_Button_Accept")]
	private BSGButton Button_Accept;

	// Token: 0x04001A8B RID: 6795
	[UIFieldTarget("id_Sovereign")]
	private UICharacterIcon m_Sovereign;

	// Token: 0x04001A8C RID: 6796
	[UIFieldTarget("id_SovereignName")]
	private TextMeshProUGUI m_SovereignName;

	// Token: 0x04001A8D RID: 6797
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x04001A8E RID: 6798
	[UIFieldTarget("id_Relations")]
	private UIKingdomRelations m_Relations;

	// Token: 0x04001A8F RID: 6799
	private bool m_Started;

	// Token: 0x04001A90 RID: 6800
	private bool m_DataExtracted;
}
