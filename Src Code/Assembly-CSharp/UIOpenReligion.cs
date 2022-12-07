using System;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000283 RID: 643
public class UIOpenReligion : Hotspot, IPointerClickHandler, IEventSystemHandler, IListener
{
	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x06002743 RID: 10051 RVA: 0x001557DB File Offset: 0x001539DB
	// (set) Token: 0x06002744 RID: 10052 RVA: 0x001557E3 File Offset: 0x001539E3
	public Logic.Kingdom Kingdom { get; private set; }

	// Token: 0x06002745 RID: 10053 RVA: 0x001557EC File Offset: 0x001539EC
	private void Init()
	{
		if (this.m_Initalizied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.SetAudioSet(string.Empty);
		this.m_Initalizied = true;
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x00155810 File Offset: 0x00153A10
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.Kingdom != kingdom && kingdom != null && kingdom.IsValid() && kingdom.visuals != null)
		{
			this.SetKingdom(kingdom);
			return;
		}
		if (this.m_Selected != null)
		{
			this.m_Selected.SetActive(UIReligionWindow.IsActive());
		}
		if (this.m_InvalidateIcon)
		{
			this.RefreshIcon();
		}
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x00155873 File Offset: 0x00153A73
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		this.Kingdom = k;
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			kingdom2.AddListener(this);
		}
		this.RefreshIcon();
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x001558AC File Offset: 0x00153AAC
	private void RefreshIcon()
	{
		if (this.m_Icon != null && this.Kingdom != null && this.m_InvalidateIcon)
		{
			this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>("ResourceIconSettings", this.Kingdom.GetPietyIcon(false), null);
		}
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x001558F9 File Offset: 0x00153AF9
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "add/remove resources", true))
		{
			return;
		}
		if (pointerEventData.button == PointerEventData.InputButton.Left)
		{
			this.ToggleOpen();
		}
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x00155925 File Offset: 0x00153B25
	public void ToggleOpen()
	{
		UIReligionWindow.ToggleOpen(this.Kingdom);
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x00155934 File Offset: 0x00153B34
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1649643086U)
		{
			if (num != 819938U)
			{
				if (num != 1211309691U)
				{
					if (num != 1649643086U)
					{
						return;
					}
					if (!(message == "finishing"))
					{
						return;
					}
				}
				else if (!(message == "destroying"))
				{
					return;
				}
				this.SetKingdom(null);
				base.enabled = true;
				return;
			}
			if (!(message == "realm_deleted"))
			{
				return;
			}
		}
		else if (num <= 2275136621U)
		{
			if (num != 2105032289U)
			{
				if (num != 2275136621U)
				{
					return;
				}
				if (!(message == "realm_added"))
				{
					return;
				}
			}
			else if (!(message == "religion_changed"))
			{
				return;
			}
		}
		else if (num != 2842647092U)
		{
			if (num != 3448667035U)
			{
				return;
			}
			if (!(message == "excommunicated"))
			{
				return;
			}
		}
		else if (!(message == "patriarch_changed"))
		{
			return;
		}
		this.m_InvalidateIcon = true;
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x00155A0B File Offset: 0x00153C0B
	private void OnDestroy()
	{
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom == null)
		{
			return;
		}
		kingdom.DelListener(this);
	}

	// Token: 0x04001AAA RID: 6826
	[UIFieldTarget("id_ReligionIcon")]
	private Image m_Icon;

	// Token: 0x04001AAB RID: 6827
	[UIFieldTarget("id_Selected")]
	private GameObject m_Selected;

	// Token: 0x04001AAD RID: 6829
	private bool m_InvalidateIcon = true;

	// Token: 0x04001AAE RID: 6830
	private bool m_Initalizied;
}
