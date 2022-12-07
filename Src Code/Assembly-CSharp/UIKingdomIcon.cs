using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200020D RID: 525
public class UIKingdomIcon : ObjectIcon, IListener, IPoolable
{
	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06001FDE RID: 8158 RVA: 0x001259B3 File Offset: 0x00123BB3
	public Logic.Kingdom kingdom
	{
		get
		{
			Logic.Object @object = this.obj;
			if (@object == null)
			{
				return null;
			}
			return @object.GetKingdom();
		}
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x001259C6 File Offset: 0x00123BC6
	public override void Awake()
	{
		base.Awake();
		this.m_WasActivated = true;
		if (this.m_AddListeners)
		{
			Logic.Kingdom kingdom = this.kingdom;
			if (kingdom == null)
			{
				return;
			}
			kingdom.AddListener(this);
		}
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x001259EE File Offset: 0x00123BEE
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x00125A07 File Offset: 0x00123C07
	public void SetIndex(int index)
	{
		this.Init();
		this.m_Primary.ForceSetIndex(index);
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x00125A1C File Offset: 0x00123C1C
	public override void SetObject(object obj, Vars vars = null)
	{
		this.Init();
		if (this.logicObject == obj && vars == null)
		{
			return;
		}
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
		}
		base.SetObject(obj, vars);
		this.obj = (obj as Logic.Object);
		this.FillVars();
		if (this.m_WasActivated)
		{
			Logic.Kingdom kingdom = this.kingdom;
			if (kingdom != null)
			{
				kingdom.AddListener(this);
			}
		}
		else
		{
			this.m_AddListeners = true;
		}
		if (this.kingdom == null)
		{
			return;
		}
		this.UpdateShields();
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x00125A9C File Offset: 0x00123C9C
	private void FillVars()
	{
		if (this.obj is Rebellion)
		{
			this.vars.Set<Rebellion>("rebellion", this.obj as Rebellion);
		}
		if (this.obj is Logic.Rebel)
		{
			Logic.Rebel rebel = this.obj as Logic.Rebel;
			Rebellion val = (rebel != null) ? rebel.rebellion : null;
			this.vars.Set<Rebellion>("rebellion", val);
		}
		if (this.obj is Mercenary)
		{
			this.vars.Set<Mercenary>("mercenary", this.obj as Mercenary);
		}
		if (this.obj is Logic.Character)
		{
			Logic.Character character = this.obj as Logic.Character;
			if (character.IsRebel())
			{
				Rebellion rebellion;
				if (character == null)
				{
					rebellion = null;
				}
				else
				{
					Logic.Army army = character.GetArmy();
					if (army == null)
					{
						rebellion = null;
					}
					else
					{
						Logic.Rebel rebel2 = army.rebel;
						rebellion = ((rebel2 != null) ? rebel2.rebellion : null);
					}
				}
				Rebellion rebellion2 = rebellion;
				if (rebellion2 != null)
				{
					this.vars.Set<Rebellion>("rebellion", rebellion2);
				}
			}
		}
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x00125B89 File Offset: 0x00123D89
	public void UpdateShields()
	{
		this.UpdateShield();
		this.UpdateOverlordShield();
		this.UpdateLoyalToShield();
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x00125B9D File Offset: 0x00123D9D
	public void ShowGlow(bool shown)
	{
		if (this.m_Glow)
		{
			this.m_Glow.gameObject.SetActive(true);
		}
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x00125BBD File Offset: 0x00123DBD
	public void ShowOverlord(bool shown)
	{
		this.showOverlord = shown;
		this.UpdateOverlordShield();
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x00125BCC File Offset: 0x00123DCC
	private void UpdateShield()
	{
		if (this.m_Primary != null)
		{
			this.m_Primary.SetObject(this.obj, this.vars);
		}
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x00125BF4 File Offset: 0x00123DF4
	private void UpdateOverlordShield()
	{
		if (this.m_Overlord == null)
		{
			return;
		}
		this.showOverlord = false;
		if (!this.showOverlord)
		{
			this.m_Overlord.gameObject.SetActive(false);
			return;
		}
		if (this.kingdom == null)
		{
			return;
		}
		if (this.kingdom.sovereignState == null)
		{
			this.m_Overlord.gameObject.SetActive(false);
			return;
		}
		if (this.kingdom.sovereignState == null)
		{
			return;
		}
		this.m_Overlord.SetObject(this.kingdom.sovereignState, this.vars);
		this.m_Overlord.gameObject.SetActive(true);
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x00125C94 File Offset: 0x00123E94
	private void UpdateLoyalToShield()
	{
		if (this.m_LoyalTo == null)
		{
			return;
		}
		Rebellion rebellion = this.obj as Rebellion;
		if (rebellion != null && rebellion.IsLoyalist())
		{
			this.m_LoyalTo.SetObject(rebellion.GetLoyalTo(), null);
			this.m_LoyalTo.gameObject.SetActive(true);
			return;
		}
		this.m_LoyalTo.SetObject(null, null);
		this.m_LoyalTo.gameObject.SetActive(false);
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x00125D09 File Offset: 0x00123F09
	public KingdomShield GetPrimary()
	{
		this.Init();
		return this.m_Primary;
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x00125D17 File Offset: 0x00123F17
	public KingdomShield GetOverlord()
	{
		this.Init();
		return this.m_Overlord;
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x00125D25 File Offset: 0x00123F25
	public KingdomShield GetLoyalTo()
	{
		this.Init();
		return this.m_LoyalTo;
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x00125D34 File Offset: 0x00123F34
	private void OnDestroy()
	{
		if (this.m_Primary != null)
		{
			this.m_Primary.onClick = null;
		}
		if (this.m_Overlord != null)
		{
			this.m_Overlord.onClick = null;
		}
		AspectRatioFitter component = base.GetComponent<AspectRatioFitter>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		if (this.kingdom != null)
		{
			this.kingdom.DelListener(this);
		}
		this.obj = null;
		this.logicObject = null;
		base.transform.position = Vector3.zero;
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x00125DC0 File Offset: 0x00123FC0
	public void OnMessage(object obj, string message, object param)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1631227408U)
		{
			if (num != 1211309691U)
			{
				if (num != 1434340769U)
				{
					if (num != 1631227408U)
					{
						return;
					}
					if (!(message == "sovereign_removed"))
					{
						return;
					}
					goto IL_C4;
				}
				else
				{
					if (!(message == "name_changed"))
					{
						return;
					}
					this.UpdateShield();
					return;
				}
			}
			else if (!(message == "destroying"))
			{
				return;
			}
		}
		else if (num <= 2068350458U)
		{
			if (num != 1649643086U)
			{
				if (num != 2068350458U)
				{
					return;
				}
				if (!(message == "great_powers_changed"))
				{
					return;
				}
				goto IL_C4;
			}
			else if (!(message == "finishing"))
			{
				return;
			}
		}
		else if (num != 3465720332U)
		{
			if (num != 4081519659U)
			{
				return;
			}
			if (!(message == "vassals_changed"))
			{
				return;
			}
			goto IL_C4;
		}
		else
		{
			if (!(message == "sovereign_set"))
			{
				return;
			}
			goto IL_C4;
		}
		(obj as Logic.Object).DelListener(this);
		return;
		IL_C4:
		this.UpdateShield();
		this.UpdateOverlordShield();
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x00125EA4 File Offset: 0x001240A4
	public void OnPoolDeactivated()
	{
		this.OnDestroy();
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x0400151D RID: 5405
	[UIFieldTarget("id_Primary")]
	private KingdomShield m_Primary;

	// Token: 0x0400151E RID: 5406
	[UIFieldTarget("id_Overlord")]
	private KingdomShield m_Overlord;

	// Token: 0x0400151F RID: 5407
	[UIFieldTarget("id_LoyalTo")]
	private KingdomShield m_LoyalTo;

	// Token: 0x04001520 RID: 5408
	[UIFieldTarget("id_Glow")]
	private GameObject m_Glow;

	// Token: 0x04001521 RID: 5409
	[SerializeField]
	private bool showOverlord;

	// Token: 0x04001522 RID: 5410
	private Logic.Object obj;

	// Token: 0x04001523 RID: 5411
	private bool m_WasActivated;

	// Token: 0x04001524 RID: 5412
	private bool m_AddListeners;

	// Token: 0x04001525 RID: 5413
	private bool m_Initialzied;
}
