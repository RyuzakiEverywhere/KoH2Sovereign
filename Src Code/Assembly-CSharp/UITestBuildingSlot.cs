using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000325 RID: 805
public class UITestBuildingSlot : MonoBehaviour
{
	// Token: 0x06003215 RID: 12821 RVA: 0x0019627C File Offset: 0x0019447C
	public void Init(DT.Field field, Color clr)
	{
		UIText.SetText(base.gameObject, "text", field.key);
		if (clr == Color.clear)
		{
			return;
		}
		Image component = base.GetComponent<Image>();
		if (component == null)
		{
			return;
		}
		clr.a = component.color.a;
		component.color = clr;
	}
}
