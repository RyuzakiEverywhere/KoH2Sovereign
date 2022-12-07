using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200027B RID: 635
public class UIKingdomStabilityRebellionRiskRow : MonoBehaviour
{
	// Token: 0x060026CF RID: 9935 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Awake()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x00152FEA File Offset: 0x001511EA
	public void SetValue(float value)
	{
		this.m_valueColor.color = ((value < 0f) ? this.negativeColor : this.positiveColor);
		UIText.SetText(this.m_value, value.ToString());
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x0015301F File Offset: 0x0015121F
	public void SetDef(RebellionRiskCategory.Def def)
	{
		this.m_icon.SetDef(def);
		UIText.SetText(this.m_name, global::Defs.Localize(def.field, "caption", null, null, true, true));
	}

	// Token: 0x04001A3E RID: 6718
	[UIFieldTarget("id_EffectIconHolder")]
	private UIKingdomStabilityRebellionRiskIcon m_icon;

	// Token: 0x04001A3F RID: 6719
	[UIFieldTarget("id_EffectName")]
	private TextMeshProUGUI m_name;

	// Token: 0x04001A40 RID: 6720
	[UIFieldTarget("id_EffectValue")]
	private TextMeshProUGUI m_value;

	// Token: 0x04001A41 RID: 6721
	[UIFieldTarget("id_ValueColor")]
	private Image m_valueColor;

	// Token: 0x04001A42 RID: 6722
	public Color positiveColor;

	// Token: 0x04001A43 RID: 6723
	public Color negativeColor;
}
