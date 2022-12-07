using System;
using System.Collections;
using Logic;
using UnityEngine;

// Token: 0x02000295 RID: 661
public class UIKingFrame : MonoBehaviour, IListener
{
	// Token: 0x060028BD RID: 10429 RVA: 0x0015C0ED File Offset: 0x0015A2ED
	private IEnumerator Start()
	{
		yield return null;
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
		UICommon.FindComponents(this, false);
		yield break;
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x0015C0FC File Offset: 0x0015A2FC
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (this.m_Kingdom != kingdom || this.invalid)
		{
			this.invalid = false;
			if (kingdom != null && kingdom.IsValid() && kingdom.visuals != null && kingdom.royalFamily != null)
			{
				this.SetKingdom(kingdom.visuals as global::Kingdom);
			}
		}
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x0015C154 File Offset: 0x0015A354
	public void SetKingdom(global::Kingdom k)
	{
		if (k == null || k.logic == null)
		{
			this.m_Sovereign = null;
			this.m_Kingdom = null;
			return;
		}
		Logic.Kingdom kingdom = this.m_Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		Logic.Kingdom kingdom2 = this.m_Kingdom;
		if (kingdom2 != null)
		{
			Logic.RoyalFamily royalFamily = kingdom2.royalFamily;
			if (royalFamily != null)
			{
				royalFamily.DelListener(this);
			}
		}
		this.m_Kingdom = k.logic;
		this.SetCharacter(this.m_Kingdom.royalFamily.Sovereign);
		Logic.Kingdom kingdom3 = this.m_Kingdom;
		if (kingdom3 != null)
		{
			kingdom3.AddListener(this);
		}
		Logic.Kingdom kingdom4 = this.m_Kingdom;
		if (kingdom4 != null)
		{
			kingdom4.royalFamily.AddListener(this);
		}
		if (this.kingdomCrest != null)
		{
			this.kingdomCrest.SetObject(this.m_Kingdom, null);
			return;
		}
		this.invalid = true;
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x0015C224 File Offset: 0x0015A424
	private void SetCharacter(Logic.Character sovereign)
	{
		if (sovereign == null)
		{
			return;
		}
		this.m_Sovereign = sovereign;
		UICommon.FindComponents(this, false);
		if (this.Icon_King != null)
		{
			this.Icon_King.SetObject(sovereign, null);
			this.Icon_King.OnSelect += this.HandleOnCharacterSelect;
			this.Icon_King.ShowCrest(false);
		}
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x0015C281 File Offset: 0x0015A481
	private void HandleOnCharacterSelect(UICharacterIcon obj)
	{
		if (this.m_Kingdom == null || this.m_Sovereign == null)
		{
			return;
		}
		if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
		{
			this.OpenRoyalFamily();
			return;
		}
		this.OpenCharacterInfoWindow(obj.Data);
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x0015C2C4 File Offset: 0x0015A4C4
	private void OpenCharacterInfoWindow(Logic.Character character)
	{
		if (this.m_Kingdom == null || this.m_Sovereign == null)
		{
			return;
		}
		if (character == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		GameObject prefab = UICommon.GetPrefab("CharacterInfo", null);
		if (prefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
		if (gameObject != null)
		{
			UICommon.DeleteChildren(gameObject.transform, typeof(UICharacter));
			UICharacter.Create(character, prefab, gameObject.transform as RectTransform, null);
		}
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x0015C354 File Offset: 0x0015A554
	public void OpenRoyalFamily()
	{
		if (this.RoyalFamilyPrortype == null)
		{
			return;
		}
		GameObject gameObject = global::Common.FindChildByName(base.gameObject.transform.root.gameObject, this.HostRectName, true, true);
		if (gameObject == null)
		{
			return;
		}
		UICommon.DeleteChildren(gameObject.transform, typeof(UIRoyalFamily));
		WorldUI worldUI = WorldUI.Get();
		if (worldUI != null)
		{
			global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
			if (kingdom != null && kingdom.logic != null)
			{
				UIRoyalFamily.Create(kingdom.logic.royalFamily, this.RoyalFamilyPrortype, gameObject.transform as RectTransform);
			}
		}
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x0015C3FC File Offset: 0x0015A5FC
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "royal_new_sovereign")
		{
			this.SetCharacter(this.m_Kingdom.royalFamily.Sovereign);
		}
		if (message == "destroying" || message == "finishing")
		{
			this.SetKingdom(null);
			base.enabled = true;
		}
	}

	// Token: 0x060028C5 RID: 10437 RVA: 0x0015C454 File Offset: 0x0015A654
	private void OnDestroy()
	{
		Logic.Kingdom kingdom = this.m_Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		Logic.Kingdom kingdom2 = this.m_Kingdom;
		if (kingdom2 == null)
		{
			return;
		}
		Logic.RoyalFamily royalFamily = kingdom2.royalFamily;
		if (royalFamily == null)
		{
			return;
		}
		royalFamily.DelListener(this);
	}

	// Token: 0x04001B8E RID: 7054
	public string HostRectName = "id_MessageContainer";

	// Token: 0x04001B8F RID: 7055
	public GameObject RoyalFamilyPrortype;

	// Token: 0x04001B90 RID: 7056
	[UIFieldTarget("id_IconKing")]
	private UICharacterIcon Icon_King;

	// Token: 0x04001B91 RID: 7057
	[UIFieldTarget("id_KingdomCrest")]
	private UIKingdomIcon kingdomCrest;

	// Token: 0x04001B92 RID: 7058
	private Logic.Character m_Sovereign;

	// Token: 0x04001B93 RID: 7059
	private Logic.Kingdom m_Kingdom;

	// Token: 0x04001B94 RID: 7060
	private bool invalid;
}
