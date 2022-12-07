using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000285 RID: 645
public class ResourceBarSlot : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06002756 RID: 10070 RVA: 0x00155C24 File Offset: 0x00153E24
	private void Start()
	{
		this.text = global::Common.GetComponent<TextMeshProUGUI>(this, "Label/Num");
		this.icon = global::Common.GetComponent<Image>(this, "Button/Image");
		this.gold_from_excess_icon = global::Common.GetComponent<Image>(this, "id _GoldFromExcess");
		this.BuildTooltip();
		this.trade_tresholds = GameLogic.Get(true).game.defs.game.defs.Find<Logic.Economy.Def>("Economy").trade_route_capacity_thresholds;
		if (this.resource == ResourceType.Piety || this.resource == ResourceType.Books)
		{
			this.showIncome = true;
			this.showMaxValue = false;
		}
		this.UpdateGoldFromExcess(true);
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x00155CC0 File Offset: 0x00153EC0
	private void BuildTooltip()
	{
		if (WorldUI.Get() != null)
		{
			Logic.Kingdom kingdom = BaseUI.LogicKingdom();
			if (this.vars == null)
			{
				this.vars = new Vars(kingdom);
			}
			else
			{
				this.vars.obj = kingdom;
			}
			this.vars.Set<Value>("capacity", this.GetTotal(kingdom));
			Tooltip.Get(base.gameObject, true).SetDef("Kingdom" + this.resource.ToString() + "Tooltip", this.vars);
			return;
		}
		Tooltip.Get(base.gameObject, true).SetText("Resources." + this.resource.ToString(), null, null);
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x00155D8C File Offset: 0x00153F8C
	private Color GetColor(float income, float max, bool has_excess_gold)
	{
		if (this.resource == ResourceType.Trade && income / max > this.trade_tresholds[0])
		{
			return this.colorRed;
		}
		if (has_excess_gold)
		{
			return this.colorGreen;
		}
		return this.colorNormal;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x00155DC0 File Offset: 0x00153FC0
	private void UpdateText(bool force = false)
	{
		if (this.text == null)
		{
			return;
		}
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		if (this.vars != null && kingdom != this.vars.obj)
		{
			this.vars.obj = kingdom;
		}
		float num = kingdom.resources[this.resource];
		float num2 = kingdom.income[this.resource] - kingdom.expenses[this.resource];
		if (this.resource == ResourceType.Trade)
		{
			num2 = kingdom.GetAllocatedCommerce();
			num = kingdom.GetMaxCommerce();
		}
		if (num == this.last_amount && num2 == this.last_income && !force)
		{
			return;
		}
		this.last_amount = num;
		this.last_income = num2;
		string text = Mathf.Round(num).ToString();
		if (this.resource == ResourceType.Trade)
		{
			text = num2.ToString("F0") + " / " + num.ToString("F0");
			this.text.text = text;
			return;
		}
		this.vars.Set<Value>("current", new Value(num));
		this.vars.Set<Value>("total", this.showMaxValue ? this.GetTotal(kingdom) : Value.Null);
		this.vars.Set<Value>("income", this.showIncome ? new Value(num2) : Value.Null);
		UIText.SetTextKey(this.text, "KingdomResources.full", this.vars, null);
		this.text.color = this.GetColor(num2, num, this.HasGoldFromExcess(kingdom, this.resource));
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x00155F6C File Offset: 0x0015416C
	private Value GetTotal(Logic.Kingdom k)
	{
		if (k == null)
		{
			return Value.Unknown;
		}
		if (this.resource == ResourceType.Books)
		{
			return (int)k.GetStat(Stats.ks_max_books, true);
		}
		if (this.resource == ResourceType.Levy)
		{
			return (int)k.GetStat(Stats.ks_max_levy, true);
		}
		if (this.resource == ResourceType.Piety)
		{
			return (int)k.GetStat(Stats.ks_max_piety, true);
		}
		if (this.resource == ResourceType.Trade)
		{
			return (int)k.GetStat(Stats.ks_commerce, true);
		}
		return Value.Unknown;
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x00155FF8 File Offset: 0x001541F8
	private void UpdateGoldFromExcess(bool force = false)
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		bool flag = this.HasGoldFromExcess(kingdom, this.resource);
		if (this.m_LastHasGoldFromExess == flag && !force)
		{
			return;
		}
		this.m_LastHasGoldFromExess = flag;
		if (this.gold_from_excess_icon != null)
		{
			this.gold_from_excess_icon.gameObject.SetActive(flag);
		}
		this.UpdateText(true);
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x00156057 File Offset: 0x00154257
	public bool HasGoldFromExcess(Logic.Kingdom k, ResourceType resource)
	{
		if (k == null)
		{
			return false;
		}
		if (resource == ResourceType.Books)
		{
			return k.goldFromExcessBooks > 0f;
		}
		if (resource == ResourceType.Piety)
		{
			return k.goldFromExcessPiety > 0f;
		}
		return resource == ResourceType.Levy && k.goldFromExcessLevy > 0f;
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x00156095 File Offset: 0x00154295
	private void Update()
	{
		this.UpdateText(false);
		this.UpdateGoldFromExcess(false);
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x001560A8 File Offset: 0x001542A8
	public void OnPointerClick(PointerEventData eventData)
	{
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "add/remove resources", true))
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI == null)
			{
				return;
			}
			Logic.Kingdom kingdom = GameLogic.Get(true).GetKingdom(worldUI.kingdom);
			if (kingdom == null)
			{
				return;
			}
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				kingdom.AddResources(KingdomAI.Expense.Category.Economy, this.resource, 1000f, true);
			}
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				kingdom.SubResources(KingdomAI.Expense.Category.Economy, this.resource, 1000f, true);
			}
		}
	}

	// Token: 0x04001AB4 RID: 6836
	public ResourceType resource;

	// Token: 0x04001AB5 RID: 6837
	public bool showMaxValue;

	// Token: 0x04001AB6 RID: 6838
	public bool showIncome = true;

	// Token: 0x04001AB7 RID: 6839
	private float last_amount = -1f;

	// Token: 0x04001AB8 RID: 6840
	private float last_income = -1f;

	// Token: 0x04001AB9 RID: 6841
	private TextMeshProUGUI text;

	// Token: 0x04001ABA RID: 6842
	private Color colorNormal = new Color(0.9f, 0.9f, 0.85f);

	// Token: 0x04001ABB RID: 6843
	private Color colorGreen = new Color32(55, 164, 28, byte.MaxValue);

	// Token: 0x04001ABC RID: 6844
	private Color colorRed = new Color(1f, 0f, 0f);

	// Token: 0x04001ABD RID: 6845
	private Image icon;

	// Token: 0x04001ABE RID: 6846
	private Image gold_from_excess_icon;

	// Token: 0x04001ABF RID: 6847
	private List<float> trade_tresholds;

	// Token: 0x04001AC0 RID: 6848
	private Vars vars;

	// Token: 0x04001AC1 RID: 6849
	private bool m_LastHasGoldFromExess;
}
