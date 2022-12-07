using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027A RID: 634
public class UIKingdomStabilityRebellionRiskIcon : MonoBehaviour
{
	// Token: 0x060026CC RID: 9932 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x00152FB4 File Offset: 0x001511B4
	public void SetDef(RebellionRiskCategory.Def def)
	{
		this.m_icon.sprite = def.field.GetValue("icon", null, true, true, true, '.').Get<Sprite>();
	}

	// Token: 0x04001A3D RID: 6717
	[UIFieldTarget("id_EffectIcon")]
	private Image m_icon;
}
