using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200022F RID: 559
public class UILoadingScreenTips : MonoBehaviour
{
	// Token: 0x060021D1 RID: 8657 RVA: 0x001320F8 File Offset: 0x001302F8
	private void OnEnable()
	{
		this.ShowRandomTip();
	}

	// Token: 0x060021D2 RID: 8658 RVA: 0x00132100 File Offset: 0x00130300
	private void Init()
	{
		if (this.m_Initazlied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initazlied = true;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x0013211C File Offset: 0x0013031C
	private void ShowRandomTip()
	{
		this.Init();
		if (global::Defs.Get(false) == null)
		{
			this.m_GameplayTipsContianer.gameObject.SetActive(false);
			return;
		}
		if (this.m_GameplayTip != null)
		{
			DT.Field defField = global::Defs.GetDefField("LoadingScreen", null);
			DT.Field field = (defField != null) ? defField.FindChild("tips", null, true, true, true, '.') : null;
			DT.Field field2;
			if (BattleViewLoader.sm_InTrasition)
			{
				field2 = ((field != null) ? field.FindChild("battleview", null, true, true, true, '.') : null);
			}
			else
			{
				field2 = ((field != null) ? field.FindChild("worldview", null, true, true, true, '.') : null);
			}
			if (field2 != null)
			{
				UIText.SetText(this.m_GameplayTip, global::Defs.Localize(field2, null, null, true, true));
			}
		}
	}

	// Token: 0x040016AE RID: 5806
	[UIFieldTarget("id_GameplayTipsContianer")]
	private GameObject m_GameplayTipsContianer;

	// Token: 0x040016AF RID: 5807
	[UIFieldTarget("id_GameplayTip")]
	private TextMeshProUGUI m_GameplayTip;

	// Token: 0x040016B0 RID: 5808
	private bool m_Initazlied;
}
