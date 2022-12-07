using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200021E RID: 542
public class UITreasury : MonoBehaviour, IListener
{
	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x060020D0 RID: 8400 RVA: 0x0012B6CF File Offset: 0x001298CF
	// (set) Token: 0x060020D1 RID: 8401 RVA: 0x0012B6D7 File Offset: 0x001298D7
	public Logic.Kingdom Data { get; private set; }

	// Token: 0x060020D2 RID: 8402 RVA: 0x0012B6E0 File Offset: 0x001298E0
	private void Start()
	{
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
			if (kingdom != null)
			{
				this.SetData(kingdom.logic);
			}
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x0012B718 File Offset: 0x00129918
	public void SetData(Logic.Kingdom kingodm)
	{
		UICommon.FindComponents(this, false);
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.Data = kingodm;
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
		this.Refresh();
		this.Button_OpenTreasury.onClick.RemoveAllListeners();
		this.Button_OpenTreasury.onClick.AddListener(new UnityAction(this.OpenTreasury));
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000023FD File Offset: 0x000005FD
	private void OpenTreasury()
	{
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x0012B790 File Offset: 0x00129990
	private void Refresh()
	{
		UIText.SetText(this.Value_Gold, ((int)this.Data.resources[ResourceType.Gold]).ToString());
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x040015DB RID: 5595
	[UIFieldTarget("id_GoldValue")]
	private TextMeshProUGUI Value_Gold;

	// Token: 0x040015DC RID: 5596
	[UIFieldTarget("id_Button_OpenTreasury")]
	private Button Button_OpenTreasury;
}
