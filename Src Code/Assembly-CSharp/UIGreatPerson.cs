using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class UIGreatPerson : MonoBehaviour, IListener
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06002B89 RID: 11145 RVA: 0x0016F7EB File Offset: 0x0016D9EB
	// (set) Token: 0x06002B8A RID: 11146 RVA: 0x0016F7F3 File Offset: 0x0016D9F3
	public Logic.Character Data { get; private set; }

	// Token: 0x06002B8B RID: 11147 RVA: 0x0016F7FC File Offset: 0x0016D9FC
	public void SetObject(Logic.Character greatPerson)
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.Data = greatPerson;
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
		UICommon.FindComponents(this, false);
		this.Refresh();
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x0016F83C File Offset: 0x0016DA3C
	private void Refresh()
	{
		if (this.m_CharacterIcon != null)
		{
			this.m_CharacterIcon.SetObject(this.Data, null);
			this.m_CharacterIcon.ShowCrest(false);
		}
		UIText.SetTextKey(this.m_Name, "Character.title_name", new Vars(this.Data), null);
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x0016F896 File Offset: 0x0016DA96
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "destroying" || message == "finishing")
		{
			this.Refresh();
		}
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x0016F8B8 File Offset: 0x0016DAB8
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x0002C53B File Offset: 0x0002A73B
	public static bool CheckIfUnlocked(Castle castle)
	{
		return true;
	}

	// Token: 0x04001DAD RID: 7597
	[UIFieldTarget("id_CharacterIcon")]
	private UICharacterIcon m_CharacterIcon;

	// Token: 0x04001DAE RID: 7598
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI m_Name;

	// Token: 0x02000811 RID: 2065
	private struct BuildPreferenceData
	{
		// Token: 0x04003DA0 RID: 15776
		public int index;
	}
}
