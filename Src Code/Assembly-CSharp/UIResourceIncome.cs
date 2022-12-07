using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002BB RID: 699
public class UIResourceIncome : MonoBehaviour
{
	// Token: 0x06002BE1 RID: 11233 RVA: 0x00170D1C File Offset: 0x0016EF1C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x00170D35 File Offset: 0x0016EF35
	public void SetObject(ResourceType resource, Logic.Object obj, Vars vars = null)
	{
		this.Init();
		this.logicObject = obj;
		this.vars = vars;
		this.m_Resource = resource;
		this.Populate();
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x00170D58 File Offset: 0x0016EF58
	public void ShowSign(bool shown)
	{
		this.m_ShownSign = shown;
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x00170D64 File Offset: 0x0016EF64
	private void Populate()
	{
		if (this.m_Icon != null)
		{
			Logic.Settlement settlement = this.logicObject as Logic.Settlement;
			if (settlement != null)
			{
				string text = null;
				if (this.m_Resource == ResourceType.Piety)
				{
					Logic.Realm realm = settlement.GetRealm();
					Logic.Realm realm2 = settlement.GetRealm();
					bool negative = ((realm2 != null) ? realm2.income[this.m_Resource] : 0f) < 0f || !this.IsSameFamily(realm.religion, realm.GetKingdom().religion);
					Logic.Kingdom kingdom = settlement.GetKingdom();
					text = ((kingdom != null) ? kingdom.GetPietyIcon(negative) : null);
				}
				if (this.m_Resource == ResourceType.Trade)
				{
					Logic.Realm realm3 = settlement.GetRealm();
					if (realm3 != null && realm3.IsTradeCenter())
					{
						text = "trade_center_commerce_icon";
					}
				}
				if (text == null)
				{
					text = this.m_Resource.ToString();
				}
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>("ResourceIconSettings", text, null);
			}
			else
			{
				this.m_Icon.overrideSprite = global::Defs.GetObj<Sprite>("ResourceIconSettings", this.m_Resource.ToString(), null);
			}
		}
		string path = (this.m_Resource != ResourceType.Piety) ? (this.m_Resource.ToString() + "RealmTooltip") : "FaithRealmTooltip";
		DT.Def def = global::Defs.Get(false).dt.FindDef(path);
		if (def != null)
		{
			Tooltip.Get(base.gameObject, true).SetDef(def, new Vars(this.logicObject), null);
		}
		this.UpdateData();
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x00170EE8 File Offset: 0x0016F0E8
	private bool IsSameFamily(Religion r1, Religion r2)
	{
		return r1 == r2 || (r1.def.christian && r2.def.christian) || (r1.def.muslim && r2.def.muslim);
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x00170F34 File Offset: 0x0016F134
	private void Update()
	{
		this.UpdateData();
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x00170F3C File Offset: 0x0016F13C
	private void UpdateData()
	{
		if (this.m_Value != null)
		{
			if (this.logicObject is Logic.Settlement)
			{
				Logic.Realm realm = (this.logicObject as Logic.Settlement).GetRealm();
				if (realm != null)
				{
					float taxed_value = realm.incomes[this.m_Resource].value.taxed_value;
					UIText.SetText(this.m_Value, this.GetFormated(taxed_value, this.m_ShownSign, 1));
					Color color;
					if (this.GetColor(taxed_value, this.vars, out color))
					{
						this.m_Value.color = color;
					}
				}
			}
			if (this.logicObject is Logic.Realm)
			{
				Logic.Realm realm2 = this.logicObject as Logic.Realm;
				if (realm2 != null)
				{
					float taxed_value2 = realm2.incomes[this.m_Resource].value.taxed_value;
					UIText.SetText(this.m_Value, this.GetFormated(taxed_value2, this.m_ShownSign, 1));
					Color color2;
					if (this.GetColor(taxed_value2, this.vars, out color2))
					{
						this.m_Value.color = color2;
					}
				}
			}
			if (this.logicObject is Logic.Kingdom)
			{
				Logic.Kingdom kingdom = this.logicObject as Logic.Kingdom;
				if (kingdom != null)
				{
					float taxed_value3 = kingdom.incomes[this.m_Resource].value.taxed_value;
					UIText.SetText(this.m_Value, this.GetFormated(taxed_value3, this.m_ShownSign, 1));
					Color color3;
					if (this.GetColor(taxed_value3, this.vars, out color3))
					{
						this.m_Value.color = color3;
					}
				}
			}
		}
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x001710B4 File Offset: 0x0016F2B4
	private bool GetColor(float num, Vars vars, out Color color)
	{
		if (vars == null)
		{
			color = Color.white;
			return false;
		}
		if (num >= 0f && vars.ContainsKey("color_positive"))
		{
			color = vars.Get<Color>("color_positive", Color.white);
			return true;
		}
		if (vars.ContainsKey("color_negative"))
		{
			color = vars.Get<Color>("color_negatove", Color.white);
			return true;
		}
		color = Color.white;
		return false;
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x00171130 File Offset: 0x0016F330
	private string GetFormated(float num, bool signed, int max_precision = 1)
	{
		if (!signed)
		{
			return DT.FloatToStr(num, max_precision);
		}
		if (num > 0f)
		{
			return "+ " + DT.FloatToStr(Mathf.Abs(num), max_precision);
		}
		if (num < 0f)
		{
			return "- " + DT.FloatToStr(Mathf.Abs(num), max_precision);
		}
		return DT.FloatToStr(Mathf.Abs(num), max_precision);
	}

	// Token: 0x04001DDC RID: 7644
	[UIFieldTarget("id_ResourceValue")]
	private TextMeshProUGUI m_Value;

	// Token: 0x04001DDD RID: 7645
	[UIFieldTarget("id_ResourceIcon")]
	private Image m_Icon;

	// Token: 0x04001DDE RID: 7646
	public Logic.Object logicObject;

	// Token: 0x04001DDF RID: 7647
	public Vars vars;

	// Token: 0x04001DE0 RID: 7648
	private ResourceType m_Resource;

	// Token: 0x04001DE1 RID: 7649
	private bool m_ShownSign = true;

	// Token: 0x04001DE2 RID: 7650
	private bool m_Initialzied;
}
