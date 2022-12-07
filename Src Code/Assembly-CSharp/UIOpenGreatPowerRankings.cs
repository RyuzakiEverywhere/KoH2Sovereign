using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000217 RID: 535
public class UIOpenGreatPowerRankings : MonoBehaviour, IListener
{
	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06002070 RID: 8304 RVA: 0x001293B9 File Offset: 0x001275B9
	// (set) Token: 0x06002071 RID: 8305 RVA: 0x001293C1 File Offset: 0x001275C1
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x06002072 RID: 8306 RVA: 0x001293CA File Offset: 0x001275CA
	private IEnumerator Start()
	{
		this.hotspot = global::Common.FindChildComponent<BSGButton>(base.gameObject, "id_Button");
		if (this.hotspot != null)
		{
			this.hotspot.onClick = new BSGButton.OnClick(this.HandleClick);
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

	// Token: 0x06002073 RID: 8307 RVA: 0x001293D9 File Offset: 0x001275D9
	private void Update()
	{
		if (this.Kingdom != BaseUI.LogicKingdom())
		{
			this.SetKingdom(BaseUI.LogicKingdom());
			return;
		}
		if (this.lastUpdateTime + this.updateInterval < UnityEngine.Time.unscaledTime)
		{
			this.Refresh();
			this.lastUpdateTime = UnityEngine.Time.unscaledTime;
		}
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x0012941C File Offset: 0x0012761C
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
		}
		if (this.Kingdom != null)
		{
			Tooltip.Get(this.hotspot.gameObject, true).SetDef("GreatPowersTooltip", null);
			Tooltip.Get(this.hotspot.gameObject, true).SetVars(new Vars(this.Kingdom));
		}
		if (this.m_Position == null)
		{
			this.m_Position = global::Common.FindChildComponent<TextMeshProUGUI>(base.gameObject, "id_Position");
		}
		this.Refresh();
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x001294CC File Offset: 0x001276CC
	private void Refresh()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		if (this.m_Position != null)
		{
			this.m_Position.text = this.Kingdom.GetVar("great_power_ranking_position", null, true).ToString();
		}
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x0012951B File Offset: 0x0012771B
	private void HandleClick(BSGButton btn)
	{
		this.OpenOrClose();
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x00129524 File Offset: 0x00127724
	public void OpenOrClose()
	{
		if (this.current != null)
		{
			UnityEngine.Object.Destroy(this.current.gameObject);
			this.current = null;
			return;
		}
		this.current = UIGreatPowerRankingsWindow.Create(this.Kingdom.game.great_powers, null);
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x0400159D RID: 5533
	public string HostRectName = "id_MessageContainer";

	// Token: 0x0400159E RID: 5534
	private TextMeshProUGUI m_Count;

	// Token: 0x0400159F RID: 5535
	private BSGButton hotspot;

	// Token: 0x040015A0 RID: 5536
	private UIGreatPowerRankingsWindow current;

	// Token: 0x040015A2 RID: 5538
	private TextMeshProUGUI m_Position;

	// Token: 0x040015A3 RID: 5539
	private float lastUpdateTime;

	// Token: 0x040015A4 RID: 5540
	private float updateInterval = 1f;
}
