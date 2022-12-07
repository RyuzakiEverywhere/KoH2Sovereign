using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class UIOpenQuests : MonoBehaviour, IListener
{
	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x060026A0 RID: 9888 RVA: 0x00152729 File Offset: 0x00150929
	// (set) Token: 0x060026A1 RID: 9889 RVA: 0x00152731 File Offset: 0x00150931
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x060026A2 RID: 9890 RVA: 0x0015273A File Offset: 0x0015093A
	private IEnumerator Start()
	{
		this.m_Count = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_Count");
		this.id_Button = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_Button");
		if (this.id_Button != null)
		{
			this.id_Button.onClick = new BSGButton.OnClick(this.HandleClick);
		}
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		yield break;
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x00152749 File Offset: 0x00150949
	private void Update()
	{
		if (this.Kingdom != BaseUI.LogicKingdom())
		{
			this.SetKingdom(BaseUI.LogicKingdom());
		}
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x00152764 File Offset: 0x00150964
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		if (this.Kingdom != null)
		{
			this.Kingdom.DelListener(this);
		}
		this.Kingdom = kingdom;
		if (this.Kingdom != null)
		{
			this.Kingdom.AddListener(this);
			Tooltip.Get(base.gameObject, true).SetText("Quest.Info.caption", "", new Vars(this.Kingdom));
		}
		this.Refresh();
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x001527D4 File Offset: 0x001509D4
	private void Refresh()
	{
		if (this.m_Count != null)
		{
			UIText.SetText(this.m_Count, (this.Kingdom != null) ? this.Kingdom.quests.Count.ToString() : "0");
		}
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x00152821 File Offset: 0x00150A21
	private void HandleClick(BSGButton btn)
	{
		UIQuestsInfoWindow.ToggleOpen();
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x00152829 File Offset: 0x00150A29
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "quest_changed")
		{
			this.Refresh();
		}
	}

	// Token: 0x04001A27 RID: 6695
	public string HostRectName = "id_MessageContainer";

	// Token: 0x04001A28 RID: 6696
	private TextMeshProUGUI m_Count;

	// Token: 0x04001A29 RID: 6697
	private BSGButton id_Button;
}
