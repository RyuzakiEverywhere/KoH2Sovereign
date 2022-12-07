using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002D7 RID: 727
public class UITitleIllustrations : MonoBehaviour
{
	// Token: 0x06002DF6 RID: 11766 RVA: 0x0017CDF6 File Offset: 0x0017AFF6
	private void Start()
	{
		this.window_def = global::Defs.GetDefField(UITitleIllustrations.def_id, null);
		this.Populate();
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000023FD File Offset: 0x000005FD
	private void Populate()
	{
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x000023FD File Offset: 0x000005FD
	private void UpdateIllustration()
	{
	}

	// Token: 0x04001F20 RID: 7968
	private static string def_id = "TitleIllustrations";

	// Token: 0x04001F21 RID: 7969
	[UIFieldTarget("id_BackgorundTileable")]
	public Image m_BackgorundTileable;

	// Token: 0x04001F22 RID: 7970
	[UIFieldTarget("id_Center")]
	public Image m_Center;

	// Token: 0x04001F23 RID: 7971
	[UIFieldTarget("id_Front_Left")]
	public Image m_Front_Left;

	// Token: 0x04001F24 RID: 7972
	[UIFieldTarget("id_Front_Right")]
	public Image m_Front_Right;

	// Token: 0x04001F25 RID: 7973
	private DT.Field window_def;
}
