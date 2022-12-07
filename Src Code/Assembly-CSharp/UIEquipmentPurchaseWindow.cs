using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020002B2 RID: 690
public class UIEquipmentPurchaseWindow : UIWindow
{
	// Token: 0x06002B4E RID: 11086 RVA: 0x0016EC6A File Offset: 0x0016CE6A
	public override string GetDefId()
	{
		return UIEquipmentPurchaseWindow.def_id;
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x0016EC71 File Offset: 0x0016CE71
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = new BSGButton.OnClick(this.HandleOnClose);
		}
		this.m_Initialized = true;
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x0016ECAF File Offset: 0x0016CEAF
	private void HandleOnClose(BSGButton b)
	{
		if (this.m_Close)
		{
			this.m_Close.onClick = null;
		}
		this.Close(false);
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x0011796E File Offset: 0x00115B6E
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x0016ECD1 File Offset: 0x0016CED1
	public void SetData(Castle castle, int slotIndex, Action<Logic.Unit.Def, int> OnHireCallback, bool showAll = false)
	{
		this.Init();
		this.m_Castle = castle;
		this.m_SlotIndex = slotIndex;
		this.onPurchase = OnHireCallback;
		this.m_HireTarget = null;
		this.m_ShowAll = showAll;
		this.Refresh();
		this.Open();
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x0016ED09 File Offset: 0x0016CF09
	public override void Open()
	{
		if (this.m_Caption != null)
		{
			UIText.SetTextKey(this.m_Caption, "EquipmentPurchaseWindow.caption", null, null);
		}
		base.Open();
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x0016ED34 File Offset: 0x0016CF34
	private void Refresh()
	{
		if (this.m_Container == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.m_Container);
		for (int i = 0; i < this.slots.Count; i++)
		{
			UnityEngine.Object.Destroy(this.slots[i].gameObject);
		}
		this.slots.Clear();
		GameObject prefab = UICommon.GetPrefab("ArmyInvetoryItemSlot", null);
		int num = 0;
		if (prefab != null)
		{
			Game game = GameLogic.Get(true);
			List<Logic.Unit.Def> list;
			if (this.m_ShowAll)
			{
				list = new List<Logic.Unit.Def>();
				using (Dictionary<string, Def>.Enumerator enumerator = game.defs.Get(typeof(Logic.Unit.Def)).defs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, Def> keyValuePair = enumerator.Current;
						Logic.Unit.Def def = keyValuePair.Value as Logic.Unit.Def;
						if (def.type == Logic.Unit.Type.InventoryItem)
						{
							list.Add(def);
						}
					}
					goto IL_F4;
				}
			}
			list = this.m_Castle.GetAvailableEquipmentTypes();
			IL_F4:
			foreach (Logic.Unit.Def unitDef in list)
			{
				UIArmyItemIcon component = UnityEngine.Object.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity, this.m_Container).GetComponent<UIArmyItemIcon>();
				if (component != null)
				{
					component.SetDef(unitDef, num, this.m_Castle, null);
					component.ShowDisbandButton(false);
					component.OnSelected += this.HandleSlotSelect;
					component.Select(false);
					this.slots.Add(component);
				}
			}
			num++;
		}
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x0016EEEC File Offset: 0x0016D0EC
	private void HandleOnUnitRecruit(BSGButton btn)
	{
		if (this.m_HireTarget != null && this.onPurchase != null)
		{
			this.onPurchase(this.m_HireTarget, this.m_SlotIndex);
		}
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x0016EF18 File Offset: 0x0016D118
	private void HandleSlotSelect(UIArmyItemIcon obj, PointerEventData e)
	{
		this.m_HireTarget = obj.UnitDef;
		for (int i = 0; i < this.slots.Count; i++)
		{
			UIArmyItemIcon uiarmyItemIcon = this.slots[i];
			uiarmyItemIcon.Select(uiarmyItemIcon == obj);
		}
		if (this.m_HireTarget != null && this.onPurchase != null)
		{
			this.onPurchase(this.m_HireTarget, this.m_SlotIndex);
		}
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x0016EF86 File Offset: 0x0016D186
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab(UIEquipmentPurchaseWindow.def_id, null);
	}

	// Token: 0x04001D8E RID: 7566
	private static string def_id = "EquipmentPurchaseWindow";

	// Token: 0x04001D8F RID: 7567
	[UIFieldTarget("id_Container")]
	private RectTransform m_Container;

	// Token: 0x04001D90 RID: 7568
	[UIFieldTarget("id_Caption")]
	private TextMeshProUGUI m_Caption;

	// Token: 0x04001D91 RID: 7569
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001D92 RID: 7570
	private int m_SlotIndex;

	// Token: 0x04001D93 RID: 7571
	private Castle m_Castle;

	// Token: 0x04001D94 RID: 7572
	private Logic.Unit.Def m_HireTarget;

	// Token: 0x04001D95 RID: 7573
	private bool m_ShowAll;

	// Token: 0x04001D96 RID: 7574
	private List<UIArmyItemIcon> slots = new List<UIArmyItemIcon>();

	// Token: 0x04001D97 RID: 7575
	private Action<Logic.Unit.Def, int> onPurchase;
}
