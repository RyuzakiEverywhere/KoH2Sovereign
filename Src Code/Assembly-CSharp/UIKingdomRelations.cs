using System;
using Logic;
using Logic.ExtensionMethods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000210 RID: 528
public class UIKingdomRelations : MonoBehaviour
{
	// Token: 0x06001FFE RID: 8190 RVA: 0x00126017 File Offset: 0x00124217
	private void Init()
	{
		if (this.m_Initilazed)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.vars = new Vars();
		this.LocalzeStatic();
		this.m_Initilazed = true;
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x00126044 File Offset: 0x00124244
	public void SetData(Logic.Kingdom kingdom1, Logic.Kingdom kingdom2)
	{
		this.Init();
		this.vars.Set<Logic.Kingdom>("kingdom", kingdom1);
		this.vars.Set<Logic.Kingdom>("target_kigdom", kingdom2);
		this.vars.Set<int>("negative_pacts", 0);
		this.vars.Set<int>("positive_pacts", 0);
		this.Kingdom1 = kingdom1;
		this.Kingdom2 = kingdom2;
		if (this.m_Pacts != null)
		{
			Tooltip.Get(this.m_Pacts, true).SetDef("KingdomRelationsPacks", this.vars);
		}
		if (this.m_RelationBar != null)
		{
			bool flag = kingdom1 != this.Kingdom2;
			if (flag)
			{
				Tooltip.Get(this.m_RelationBar.gameObject, true).SetDef("KingdomRelationship", this.vars);
			}
			this.m_RelationBar.gameObject.SetActive(flag);
		}
		this.Refresh();
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x00126129 File Offset: 0x00124329
	private void Refresh()
	{
		this.UpdateRelationBar();
		this.UpdateStances();
		this.UpdateRelationValueAndLabel();
		this.UpdatePacts();
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x00126144 File Offset: 0x00124344
	private void UpdatePacts()
	{
		if (this.m_Pacts == null)
		{
			return;
		}
		if (this.Kingdom1 == null || this.Kingdom2 == null)
		{
			return;
		}
		if (this.Kingdom1 == this.Kingdom2)
		{
			this.m_Pacts.gameObject.SetActive(false);
			return;
		}
		bool flag = this.hideStanceIconsAtWar && this.Kingdom1.IsEnemy(this.Kingdom2);
		this.m_Pacts.gameObject.SetActive(!flag);
		int positivePactsWith = this.Kingdom1.GetPositivePactsWith(this.Kingdom2);
		int negativePactsWith = this.Kingdom1.GetNegativePactsWith(this.Kingdom2);
		this.vars.Set<int>("positive_pacts", positivePactsWith);
		this.vars.Set<int>("negative_pacts", negativePactsWith);
		if (this.m_PositivePacts != null)
		{
			this.m_PositivePacts.text = positivePactsWith.ToString();
		}
		if (this.m_NegativePacts != null)
		{
			this.m_NegativePacts.text = negativePactsWith.ToString();
		}
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x00126248 File Offset: 0x00124448
	private void OpenWarWindow(UIWarIcon icon)
	{
		UIWarsOverviewWindow.ToggleOpen(this.Kingdom1, icon.War, null);
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x0012625C File Offset: 0x0012445C
	private void UpdateStances()
	{
		bool flag = this.Kingdom1.IsEnemy(this.Kingdom2);
		bool flag2 = this.Kingdom1.HasTradeAgreement(this.Kingdom2);
		bool marriage = KingdomAndKingdomRelation.GetMarriage(this.Kingdom1, this.Kingdom2);
		bool flag3 = this.hideStanceIconsAtWar && flag;
		RectTransform war = this.m_War;
		if (war != null)
		{
			war.gameObject.SetActive(flag3);
		}
		Image stance_Border = this.m_Stance_Border;
		if (stance_Border != null)
		{
			stance_Border.gameObject.SetActive(!flag3);
		}
		Image stance = this.m_Stance;
		if (stance != null)
		{
			stance.gameObject.SetActive(!flag3);
		}
		Image trade_Border = this.m_Trade_Border;
		if (trade_Border != null)
		{
			trade_Border.gameObject.SetActive(!flag3);
		}
		Image trade = this.m_Trade;
		if (trade != null)
		{
			trade.gameObject.SetActive(!flag3);
		}
		Image marriage_Border = this.m_Marriage_Border;
		if (marriage_Border != null)
		{
			marriage_Border.gameObject.SetActive(!flag3);
		}
		Image marriage2 = this.m_Marriage;
		if (marriage2 != null)
		{
			marriage2.gameObject.SetActive(!flag3);
		}
		if (this.m_Stance != null)
		{
			Vars vars = new Vars();
			vars.Set<Logic.Kingdom>("target_kingdom", this.Kingdom2);
			if (this.Kingdom2.sovereignState == this.Kingdom1)
			{
				vars.Set<string>("name", "Kingdom.Stance.Vassalage.Vassal.name");
				vars.Set<string>("tooltip", "Kingdom.Stance.Vassalage.Vassal.tooltip");
				Tooltip.Get(this.m_Stance.gameObject, true).SetDef("KingdomStanceTooltip", vars);
				this.m_Stance.overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", "Stance.Vassalage.Vassal.icon", null);
			}
			else if (this.Kingdom1.sovereignState == this.Kingdom2)
			{
				vars.Set<string>("name", "Kingdom.Stance.Vassalage.Liege.name");
				vars.Set<string>("tooltip", "Kingdom.Stance.Vassalage.Liege.tooltip");
				Tooltip.Get(this.m_Stance.gameObject, true).SetDef("KingdomStanceTooltip", vars);
				this.m_Stance.overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", "Stance.Vassalage.Liege.icon", null);
			}
			else
			{
				string str;
				if (this.Kingdom1.HasStance(this.Kingdom2, RelationUtils.Stance.NonAggression))
				{
					str = RelationUtils.Stance.NonAggression.ToString();
				}
				else
				{
					RelationUtils.Stance warStance = this.Kingdom1.GetWarStance(this.Kingdom2);
					if (warStance.IsPeace() && this.Kingdom1.IsInTruceWith(this.Kingdom2))
					{
						str = "Truce";
					}
					else
					{
						str = warStance.ToString();
					}
				}
				string str2 = "Kingdom.Stance." + str;
				vars.Set<string>("name", str2 + ".name");
				vars.Set<string>("tooltip", str2 + ".tooltip");
				Tooltip.Get(this.m_Stance.gameObject, true).SetDef("KingdomStanceTooltip", vars);
				this.m_Stance.overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", "Stance." + str + ".icon", null);
			}
		}
		if (this.m_Stance_Border != null)
		{
			string key = flag ? "border_negative" : "border_normal";
			this.m_Stance_Border.overrideSprite = global::Defs.GetObj<Sprite>("UIKingdomRelations", key, null);
		}
		if (this.m_Trade != null)
		{
			string str3 = flag2 ? "Trade" : "None";
			string str4 = "Kingdom.Stance.Trade." + str3;
			Vars vars2 = new Vars();
			vars2.Set<string>("name", str4 + ".name");
			vars2.Set<string>("tooltip", str4 + ".tooltip");
			vars2.Set<Logic.Kingdom>("target_kingdom", this.Kingdom2);
			Tooltip.Get(this.m_Trade.gameObject, true).SetDef("KingdomStanceTooltip", vars2);
			this.m_Trade.overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", "Stance.Trade." + str3 + ".icon", null);
		}
		if (this.m_Trade_Border != null)
		{
			string key2 = flag2 ? "border_normal" : "border_disbaled";
			this.m_Trade_Border.overrideSprite = global::Defs.GetObj<Sprite>("UIKingdomRelations", key2, null);
		}
		if (this.m_Marriage != null)
		{
			string str5 = marriage ? "Active" : "None";
			string str6 = "Kingdom.Stance.Marriage." + str5;
			Vars vars3 = new Vars();
			vars3.Set<string>("name", str6 + ".name");
			vars3.Set<string>("tooltip", str6 + ".tooltip");
			vars3.Set<Logic.Kingdom>("target_kingdom", this.Kingdom2);
			Tooltip.Get(this.m_Marriage.gameObject, true).SetDef("KingdomStanceTooltip", vars3);
			this.m_Marriage.overrideSprite = global::Defs.GetObj<Sprite>("Kingdom", "Stance.Marriage." + str5 + ".icon", null);
		}
		if (this.m_Marriage_Border != null)
		{
			string key3 = marriage ? "border_normal" : "border_disbaled";
			this.m_Marriage_Border.overrideSprite = global::Defs.GetObj<Sprite>("UIKingdomRelations", key3, null);
		}
		if (this.m_WarIcon != null && flag3)
		{
			this.m_WarIcon.SetObject(this.Kingdom1.FindWarWith(this.Kingdom2), null);
			this.m_WarIcon.OnSelect -= this.OpenWarWindow;
			this.m_WarIcon.OnSelect += this.OpenWarWindow;
		}
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x001267DB File Offset: 0x001249DB
	private void LocalzeStatic()
	{
		if (this.m_LabelWar != null)
		{
			UIText.SetTextKey(this.m_LabelWar, "Kingdom.Relation.at_war", null, null);
		}
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x00126800 File Offset: 0x00124A00
	private void UpdateRelationBar()
	{
		if (this.m_RelationBar == null || this.m_Tumb == null)
		{
			return;
		}
		float relationship = KingdomAndKingdomRelation.Get(this.Kingdom1, this.Kingdom2, true, false).GetRelationship();
		this.vars.Set<int>("relationship", (int)relationship);
		this.vars.Set<string>("relationship_key", global::Kingdom.GetKingdomRelationsKey(this.Kingdom1, this.Kingdom2));
		float num = (relationship / RelationUtils.Def.maxRelationship + 1f) / 2f;
		float num2 = this.m_RelationBar.rect.width - 6f;
		this.m_Tumb.localPosition = new Vector3(num2 * num - num2 / 2f, this.m_Tumb.localPosition.y, 0f);
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x001268D4 File Offset: 0x00124AD4
	private void UpdateRelationValueAndLabel()
	{
		float relationship = this.Kingdom1.GetRelationship(this.Kingdom2);
		if (this.m_RelationsLabel != null)
		{
			if (relationship < RelationUtils.Def.GetUpperTreshold(RelationUtils.RelationshipType.Hostile))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.hostile", null, null);
			}
			else if (relationship < RelationUtils.Def.GetUpperTreshold(RelationUtils.RelationshipType.Negative))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.negative", null, null);
			}
			else if (relationship < RelationUtils.Def.GetLowerTreshold(RelationUtils.RelationshipType.Reserved))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.reserved", null, null);
			}
			else if (relationship <= RelationUtils.Def.GetUpperTreshold(RelationUtils.RelationshipType.Neutral))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.neutral", null, null);
			}
			else if (relationship <= RelationUtils.Def.GetUpperTreshold(RelationUtils.RelationshipType.Sympathetic))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.sympathetic", null, null);
			}
			else if (relationship <= RelationUtils.Def.GetUpperTreshold(RelationUtils.RelationshipType.Trusting))
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.trusting", null, null);
			}
			else
			{
				UIText.SetTextKey(this.m_RelationsLabel, "Kingdom.Relation.friendly", null, null);
			}
		}
		if (this.m_RelationsValue != null)
		{
			this.m_RelationsValue.text = ((int)relationship).ToString();
		}
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x001269ED File Offset: 0x00124BED
	private void Update()
	{
		if (this.Kingdom1 != null && this.Kingdom2 != null)
		{
			this.UpdateStances();
			this.UpdateRelationBar();
			this.UpdateRelationValueAndLabel();
			this.UpdatePacts();
		}
	}

	// Token: 0x0400152A RID: 5418
	[SerializeField]
	private bool hideStanceIconsAtWar = true;

	// Token: 0x0400152B RID: 5419
	[UIFieldTarget("id_RelationsLabel")]
	private TextMeshProUGUI m_RelationsLabel;

	// Token: 0x0400152C RID: 5420
	[UIFieldTarget("id_RelationsValue")]
	private TextMeshProUGUI m_RelationsValue;

	// Token: 0x0400152D RID: 5421
	[UIFieldTarget("id_LabelWar")]
	private TextMeshProUGUI m_LabelWar;

	// Token: 0x0400152E RID: 5422
	[UIFieldTarget("id_Tumb")]
	private RectTransform m_Tumb;

	// Token: 0x0400152F RID: 5423
	[UIFieldTarget("id_RelationBar")]
	private RectTransform m_RelationBar;

	// Token: 0x04001530 RID: 5424
	[UIFieldTarget("id_War")]
	private RectTransform m_War;

	// Token: 0x04001531 RID: 5425
	[UIFieldTarget("id_WarIcon")]
	private UIWarIcon m_WarIcon;

	// Token: 0x04001532 RID: 5426
	[UIFieldTarget("id_Stance_Border")]
	private Image m_Stance_Border;

	// Token: 0x04001533 RID: 5427
	[UIFieldTarget("id_Stance")]
	private Image m_Stance;

	// Token: 0x04001534 RID: 5428
	[UIFieldTarget("id_Trade_Border")]
	private Image m_Trade_Border;

	// Token: 0x04001535 RID: 5429
	[UIFieldTarget("id_Trade")]
	private Image m_Trade;

	// Token: 0x04001536 RID: 5430
	[UIFieldTarget("id_Marriage_Border")]
	private Image m_Marriage_Border;

	// Token: 0x04001537 RID: 5431
	[UIFieldTarget("id_Marriage")]
	private Image m_Marriage;

	// Token: 0x04001538 RID: 5432
	[UIFieldTarget("id_Pacts")]
	private GameObject m_Pacts;

	// Token: 0x04001539 RID: 5433
	[UIFieldTarget("id_PositivePacts")]
	private TextMeshProUGUI m_PositivePacts;

	// Token: 0x0400153A RID: 5434
	[UIFieldTarget("id_NegativePacts")]
	private TextMeshProUGUI m_NegativePacts;

	// Token: 0x0400153B RID: 5435
	public Logic.Kingdom Kingdom1;

	// Token: 0x0400153C RID: 5436
	public Logic.Kingdom Kingdom2;

	// Token: 0x0400153D RID: 5437
	private bool m_Initilazed;

	// Token: 0x0400153E RID: 5438
	private Vars vars;
}
