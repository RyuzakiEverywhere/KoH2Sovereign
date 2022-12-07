using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class UIOpenRoyalFamily : MonoBehaviour, IListener
{
	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060028F1 RID: 10481 RVA: 0x0015CE8F File Offset: 0x0015B08F
	// (set) Token: 0x060028F2 RID: 10482 RVA: 0x0015CE97 File Offset: 0x0015B097
	public Logic.RoyalFamily Familiy { get; private set; }

	// Token: 0x060028F3 RID: 10483 RVA: 0x0015CEA0 File Offset: 0x0015B0A0
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

	// Token: 0x060028F4 RID: 10484 RVA: 0x0015CEB0 File Offset: 0x0015B0B0
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom != null && kingdom.IsValid() && kingdom.royalFamily != null && kingdom.royalFamily.IsValid() && this.Familiy != kingdom.royalFamily)
		{
			this.SetFamiliy(kingdom.royalFamily);
		}
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x0015CF00 File Offset: 0x0015B100
	public void SetFamiliy(Logic.RoyalFamily famility)
	{
		if (this.Familiy != null)
		{
			this.Familiy.DelListener(this);
			Logic.Kingdom kingdom = this.Familiy.GetKingdom();
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
		}
		this.Familiy = famility;
		if (this.Familiy != null)
		{
			Logic.Kingdom kingdom2 = this.Familiy.GetKingdom();
			if (kingdom2 != null)
			{
				kingdom2.AddListener(this);
			}
			this.Familiy.AddListener(this);
			Tooltip.Get(base.gameObject, true).SetText("RoyalFamilyWindow.caption", "", new Vars(this.Familiy.GetKingdom()));
		}
		this.Refresh();
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x0015CFA0 File Offset: 0x0015B1A0
	private void Refresh()
	{
		UIText.SetText(this.m_Count, this.GetRoyalFamilityCount().ToString());
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x0015CFC8 File Offset: 0x0015B1C8
	private int GetRoyalFamilityCount()
	{
		if (this.Familiy == null)
		{
			return 0;
		}
		int num = 0;
		if (this.Familiy.Sovereign != null)
		{
			num++;
		}
		if (this.Familiy.Spouse != null)
		{
			num++;
		}
		return num + this.Familiy.Children.Count;
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x0015D017 File Offset: 0x0015B217
	private void HandleClick(BSGButton btn)
	{
		this.OpenOrClose();
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x0015D020 File Offset: 0x0015B220
	public void OpenOrClose()
	{
		if (this.Prefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject.transform.root.gameObject, this.HostRectName, true, true);
		if (gameObject == null)
		{
			return;
		}
		if (!UICommon.DeleteChildren(gameObject.transform, typeof(UIRoyalFamily)))
		{
			UIRoyalFamily.Create(this.Familiy, this.Prefab, gameObject.transform as RectTransform);
		}
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x0015D0A0 File Offset: 0x0015B2A0
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num > 1065923593U)
		{
			if (num <= 1211309691U)
			{
				if (num != 1210552954U)
				{
					if (num != 1211309691U)
					{
						return;
					}
					if (!(message == "destroying"))
					{
						return;
					}
				}
				else
				{
					if (!(message == "royal_child_remove"))
					{
						return;
					}
					goto IL_B4;
				}
			}
			else if (num != 1649643086U)
			{
				if (num != 2392387717U)
				{
					return;
				}
				if (!(message == "royal_new_spouse"))
				{
					return;
				}
				goto IL_B4;
			}
			else if (!(message == "finishing"))
			{
				return;
			}
			this.SetFamiliy(null);
			base.enabled = true;
			return;
		}
		if (num != 190343346U)
		{
			if (num != 291138585U)
			{
				if (num != 1065923593U)
				{
					return;
				}
				if (!(message == "royal_new_born"))
				{
					return;
				}
			}
			else if (!(message == "royal_remove_spouse"))
			{
				return;
			}
		}
		else if (!(message == "royal_new_sovereign"))
		{
			return;
		}
		IL_B4:
		this.Refresh();
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x0015D176 File Offset: 0x0015B376
	private void OnDestroy()
	{
		if (this.Familiy != null)
		{
			Logic.Kingdom kingdom = this.Familiy.GetKingdom();
			if (kingdom != null)
			{
				kingdom.DelListener(this);
			}
			this.Familiy.DelListener(this);
		}
	}

	// Token: 0x04001BAF RID: 7087
	public GameObject Prefab;

	// Token: 0x04001BB0 RID: 7088
	public string HostRectName = "id_MessageContainer";

	// Token: 0x04001BB1 RID: 7089
	private TextMeshProUGUI m_Count;

	// Token: 0x04001BB2 RID: 7090
	private BSGButton id_Button;
}
