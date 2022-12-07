using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine.EventSystems;

// Token: 0x02000203 RID: 515
public class UIKingdomFame : Hotspot, IListener
{
	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06001F55 RID: 8021 RVA: 0x00122A7C File Offset: 0x00120C7C
	// (set) Token: 0x06001F56 RID: 8022 RVA: 0x00122A84 File Offset: 0x00120C84
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x06001F57 RID: 8023 RVA: 0x00122A8D File Offset: 0x00120C8D
	private IEnumerator Start()
	{
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

	// Token: 0x06001F58 RID: 8024 RVA: 0x00122A95 File Offset: 0x00120C95
	private void Init()
	{
		if (this.m_Initialzed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzed = true;
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x00122AB0 File Offset: 0x00120CB0
	public void SetKingdom(Logic.Kingdom kingdom)
	{
		this.Init();
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			kingdom2.DelListener(this);
		}
		Logic.Kingdom kingdom3 = this.Kingdom;
		if (kingdom3 != null)
		{
			Game game = kingdom3.game;
			if (game != null)
			{
				game.religions.DelListener(this);
			}
		}
		this.Kingdom = kingdom;
		Logic.Kingdom kingdom4 = this.Kingdom;
		if (kingdom4 != null)
		{
			kingdom4.AddListener(this);
		}
		Logic.Kingdom kingdom5 = this.Kingdom;
		if (kingdom5 != null)
		{
			Game game2 = kingdom5.game;
			if (game2 != null)
			{
				game2.religions.AddListener(this);
			}
		}
		this.Refresh();
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x00122B38 File Offset: 0x00120D38
	protected override void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
		base.OnEnable();
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x00122B4C File Offset: 0x00120D4C
	protected override void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
		base.OnDisable();
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x00122B60 File Offset: 0x00120D60
	private void LateUpdate()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.Kingdom != kingdom && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
			return;
		}
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x00122BA4 File Offset: 0x00120DA4
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		UIGreatPowersWindow.ToggleOpen(this.Kingdom.game.great_powers);
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x00122BC4 File Offset: 0x00120DC4
	private void Refresh()
	{
		if (this.m_KingdomName != null)
		{
			UIText.SetTextKey(this.m_KingdomName, (this.Kingdom == null) ? "" : "Kingdom.name", new Vars(this.Kingdom), null);
		}
		if (base.gameObject != null)
		{
			Tooltip.Get(base.gameObject, true).SetDef("FameTooltip", new Vars(this.Kingdom));
		}
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x00122C44 File Offset: 0x00120E44
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "name_changed" || message == "religion_changed" || message == "caliphate_claimed" || message == "caliphate_abandoned")
		{
			this.m_Invalidate = true;
			return;
		}
		if (!(message == "destroying"))
		{
			return;
		}
		this.SetKingdom(null);
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x00122CA2 File Offset: 0x00120EA2
	private void OnDestroy()
	{
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 == null)
		{
			return;
		}
		Game game = kingdom2.game;
		if (game == null)
		{
			return;
		}
		game.religions.AddListener(this);
	}

	// Token: 0x040014D3 RID: 5331
	[UIFieldTarget("id_KingdomName")]
	private TextMeshProUGUI m_KingdomName;

	// Token: 0x040014D5 RID: 5333
	private bool m_Invalidate;

	// Token: 0x040014D6 RID: 5334
	private bool m_Initialzed;
}
