using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002D6 RID: 726
public class UITitleDebugTools : MonoBehaviour
{
	// Token: 0x06002DF0 RID: 11760 RVA: 0x0017CCB3 File Offset: 0x0017AEB3
	private void OnEnable()
	{
		this.Init();
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x0017CCBC File Offset: 0x0017AEBC
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.btnClearSaves.onClick = new BSGButton.OnClick(this.OnClearSaves);
		TextMeshProUGUI mpcount = this.m_MPCount;
		Tooltip tooltip = Tooltip.Get((mpcount != null) ? mpcount.gameObject : null, true);
		if (tooltip != null)
		{
			tooltip.SetText("@{mpboss.debug_text}", null, null);
		}
		this.m_Initialized = false;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x0017CD21 File Offset: 0x0017AF21
	private void Update()
	{
		if (UnityEngine.Time.time > this.m_latsUpdate + this.m_RefreshInterval)
		{
			this.Refresh();
			this.m_latsUpdate = UnityEngine.Time.time;
		}
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x0017CD48 File Offset: 0x0017AF48
	private void Refresh()
	{
		if (this.m_MPCount != null)
		{
			List<Logic.Multiplayer> allMultiplayers = THQNORequest.GetAllMultiplayers();
			string text;
			if (allMultiplayers != null)
			{
				text = allMultiplayers.Count.ToString();
			}
			else
			{
				text = "Invalid";
			}
			UIText.SetText(this.m_MPCount, text);
		}
		if (this.m_UserName != null)
		{
			UIText.SetText(this.m_UserName, THQNORequest.playerName);
		}
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x0017CDB0 File Offset: 0x0017AFB0
	private void OnClearSaves(BSGButton btn)
	{
		UserInteractionLogger.LogNewLine(btn, null);
		bool modifierKey = UICommon.GetModifierKey(UICommon.ModifierKey.Shift);
		MPBoss mpboss = MPBoss.Get();
		if (mpboss == null)
		{
			return;
		}
		mpboss.ClearSaves(true, !modifierKey, !modifierKey);
	}

	// Token: 0x04001F1A RID: 7962
	[UIFieldTarget("id_ClearSaves")]
	public BSGButton btnClearSaves;

	// Token: 0x04001F1B RID: 7963
	[UIFieldTarget("id_UserName")]
	public TextMeshProUGUI m_UserName;

	// Token: 0x04001F1C RID: 7964
	[UIFieldTarget("id_mpCount")]
	public TextMeshProUGUI m_MPCount;

	// Token: 0x04001F1D RID: 7965
	private bool m_Initialized;

	// Token: 0x04001F1E RID: 7966
	private float m_RefreshInterval = 1f;

	// Token: 0x04001F1F RID: 7967
	private float m_latsUpdate;
}
