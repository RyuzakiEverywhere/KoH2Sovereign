using System;
using Logic;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class IconsFloodTest : MonoBehaviour
{
	// Token: 0x06003222 RID: 12834 RVA: 0x000023FD File Offset: 0x000005FD
	private void Start()
	{
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x0019687C File Offset: 0x00194A7C
	private void Update()
	{
		if (this.sameMsgMultipleTimesTest)
		{
			this.FloodTest();
			this.sameMsgMultipleTimesTest = false;
		}
		if (this.presetOfDifferentMsgsTest)
		{
			this.FloodIconsBar();
			this.presetOfDifferentMsgsTest = false;
		}
	}

	// Token: 0x06003224 RID: 12836 RVA: 0x001968A8 File Offset: 0x00194AA8
	private void FloodTest()
	{
		for (int i = 0; i < this.messagesCount; i++)
		{
			IconsBar bar = global::Common.FindChildComponent<IconsBar>(WorldUI.Get().gameObject, "id_MessageIcons");
			MessageIcon.Create("KingdomIsPleased", new Vars(), bar, MessageIcon.Type.Message, true, null);
		}
	}

	// Token: 0x06003225 RID: 12837 RVA: 0x001968EF File Offset: 0x00194AEF
	private void FloodIconsBar()
	{
		global::Common.FindChildComponent<IconsBar>(WorldUI.Get().gameObject, "id_MessageIcons");
	}

	// Token: 0x040021AF RID: 8623
	public int messagesCount = 30;

	// Token: 0x040021B0 RID: 8624
	public bool sameMsgMultipleTimesTest;

	// Token: 0x040021B1 RID: 8625
	public bool presetOfDifferentMsgsTest;
}
