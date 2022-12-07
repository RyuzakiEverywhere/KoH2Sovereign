using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class UIIncomePanel : MonoBehaviour
{
	// Token: 0x06002172 RID: 8562 RVA: 0x0012F540 File Offset: 0x0012D740
	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x0012F552 File Offset: 0x0012D752
	public void SetObject(IncomePerResource income)
	{
		this.income = income;
		this.Refresh();
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x0012F564 File Offset: 0x0012D764
	public void Load(string def_id)
	{
		this.Init();
		this.def_id = def_id;
		this.rows.Clear();
		UICommon.DeleteChildren(base.transform);
		this.def = global::Defs.GetDefField(def_id, null);
		DT.Field field = this.def;
		List<DT.Field> list = (field != null) ? field.children : null;
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			DT.Field field2 = list[i];
			if (!string.IsNullOrEmpty(field2.key))
			{
				this.LoadRow(field2);
			}
		}
		this.Refresh();
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x0012F5EC File Offset: 0x0012D7EC
	private void LoadRowTooltips()
	{
		for (int i = 0; i < this.rows.Count; i++)
		{
			UIIncomePanel.Row row = this.rows[i];
			if (row.go != null)
			{
				Vars vars = new Vars();
				vars.Set<UIIncomePanel>("panel", this);
				vars.Set<UIIncomePanel.Row>("row", row);
				int num = i + 1;
				if (num < this.rows.Count && (this.rows[num].def.Type() == "details" || this.rows[num].def.Type() == "row") && (row.def.Type() == "subtotal" || row.def.Type() == "subtotal"))
				{
					Tooltip.Get(row.go, true).SetDef("IncomePanelTooltip", vars);
				}
				else
				{
					Tooltip.Get(row.go, true).SetDef("IncomeDescriptionTooltip", vars);
				}
			}
		}
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x0012F708 File Offset: 0x0012D908
	private void LoadRow(DT.Field def)
	{
		string text = def.Type();
		GameObject gameObject;
		if (!(text == "total"))
		{
			if (!(text == "subtotal"))
			{
				if (!(text == "row"))
				{
					if (!(text == "separator"))
					{
						if (!(text == "details"))
						{
							return;
						}
						gameObject = null;
					}
					else
					{
						gameObject = global::Defs.GetObj<GameObject>(this.def, "separator", null);
					}
				}
				else
				{
					gameObject = global::Defs.GetObj<GameObject>(this.def, "row", null);
				}
			}
			else
			{
				gameObject = global::Defs.GetObj<GameObject>(this.def, "subtotal", null);
			}
		}
		else
		{
			gameObject = global::Defs.GetObj<GameObject>(this.def, "total", null);
		}
		GameObject gameObject2 = null;
		if (gameObject != null)
		{
			gameObject2 = global::Common.Spawn(gameObject, base.transform, false, "");
		}
		if (gameObject2 != null)
		{
			gameObject2.name = def.key;
			gameObject2.hideFlags = HideFlags.DontSave;
			gameObject2.SetActive(true);
		}
		UIIncomePanel.Row row = new UIIncomePanel.Row();
		row.def = def;
		row.idx = this.rows.Count;
		row.type = text;
		row.condition = def.FindChild("condition", null, true, true, true, '.');
		row.tooltip_setup = def.FindChild("tooltip_setup", null, true, true, true, '.');
		row.show_dash_if_zero = def.GetBool("show_dash_if_zero", null, false, true, true, true, '.');
		this.ParseValue(row.def, out row.location, out row.mod);
		row.value_form = def.GetString("value_form", null, "", true, true, true, '.');
		row.value_formating = def.FindChild("value_formating", null, true, true, true, '.');
		row.go = gameObject2;
		if (gameObject2 != null)
		{
			row.label = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject2, "Label");
			row.value = global::Common.FindChildComponent<TextMeshProUGUI>(gameObject2, "Value");
		}
		this.rows.Add(row);
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x0012F8E8 File Offset: 0x0012DAE8
	private void ParseValue(DT.Field field, out string location, out string mod)
	{
		if (field == null || !field.value.is_string)
		{
			string text;
			mod = (text = null);
			location = text;
			return;
		}
		string text2 = field.String(null, "");
		if (string.IsNullOrEmpty(text2))
		{
			string text;
			mod = (text = null);
			location = text;
			return;
		}
		int num = text2.IndexOf('.');
		if (num > 0)
		{
			location = text2.Substring(0, num);
			mod = text2.Substring(num + 1);
			return;
		}
		location = text2;
		mod = null;
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x0012F958 File Offset: 0x0012DB58
	public void Refresh()
	{
		for (int i = this.rows.Count - 1; i >= 0; i--)
		{
			UIIncomePanel.Row row = this.rows[i];
			this.CalcValue(row);
			row.SetVars(this.income);
			if (!row.CheckCondition())
			{
				GameObject go = row.go;
				if (go != null)
				{
					go.gameObject.SetActive(false);
				}
			}
			else
			{
				GameObject go2 = row.go;
				if (go2 != null)
				{
					go2.gameObject.SetActive(true);
				}
				this.RefreshLabel(row);
				this.RefreshValue(row);
				this.RefreshTooltip(row);
			}
		}
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x0012F9EC File Offset: 0x0012DBEC
	private void RefreshTooltip(UIIncomePanel.Row row)
	{
		if (row.go != null)
		{
			Vars vars = new Vars();
			vars.Set<UIIncomePanel>("panel", this);
			vars.Set<UIIncomePanel.Row>("row", row);
			vars.Set<bool>("is_tooltip", true);
			vars.Set<bool>("show_subtotal_tooltips", this.def.GetBool("show_subtotal_tooltips", null, true, true, true, true, '.'));
			int num = row.idx + 1;
			if (num < this.rows.Count && (this.rows[num].def.type == "details" || this.rows[num].def.type == "row"))
			{
				Tooltip.Get(row.go, true).SetDef("IncomePanelTooltip", vars);
				return;
			}
			Tooltip.Get(row.go, true).SetDef("IncomeDescriptionTooltip", vars);
		}
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x0012FAE0 File Offset: 0x0012DCE0
	private UIIncomePanel.Row FetchRowFromParentTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		Vars vars = ((ht != null) ? ht.vars : null) as Vars;
		if (vars == null)
		{
			return null;
		}
		UIHyperText.Element element = vars.GetRaw("obj").Get<UIHyperText.Element>();
		if (element == null)
		{
			return null;
		}
		Vars vars2 = element.row.instance_obj as Vars;
		if (vars2 == null)
		{
			return null;
		}
		string text = (vars2 != null) ? vars2.GetRaw("label").String(null) : null;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string a = text.Substring(1);
		for (int i = 0; i < this.rows.Count; i++)
		{
			if (a == this.GetLabelStr(this.rows[i]))
			{
				return this.rows[i];
			}
		}
		return null;
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x0012FBAE File Offset: 0x0012DDAE
	private static Value SetupRowTooltip(UIHyperText ht, UIIncomePanel.Row row, Vars vars)
	{
		UIHyperText.Callback(row.def, "tooltip_setup", ht, ht.last_row, null, -1, null);
		return Value.Unknown;
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x0012FBD0 File Offset: 0x0012DDD0
	public static Value SetupIncomeTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		object obj;
		if (ht == null)
		{
			obj = null;
		}
		else
		{
			IVars vars = ht.vars;
			obj = ((vars != null) ? vars.GetVar("panel", null, true).obj_val : null);
		}
		UIIncomePanel uiincomePanel = obj as UIIncomePanel;
		if (uiincomePanel == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht2 = arg.ht;
		object obj2;
		if (ht2 == null)
		{
			obj2 = null;
		}
		else
		{
			IVars vars2 = ht2.vars;
			obj2 = ((vars2 != null) ? vars2.GetVar("row", null, true).obj_val : null);
		}
		UIIncomePanel.Row row = obj2 as UIIncomePanel.Row;
		if (row == null)
		{
			row = uiincomePanel.FetchRowFromParentTooltip(arg);
		}
		if (row == null)
		{
			return Value.Unknown;
		}
		Vars vars3 = arg.ht.vars as Vars;
		if (vars3 == null)
		{
			vars3 = new Vars();
			arg.ht.vars = vars3;
		}
		Vars vars4 = row.vars;
		if (vars4 != null)
		{
			vars4.Set<bool>("is_tooltip", true);
		}
		UIIncomePanel.SetupRowTooltip(arg.ht, row, vars3);
		vars3.Set<bool>("show_description", row.def.GetBool("force_show_description", null, false, true, true, true, '.'));
		vars3.Set<string>("caption", "#" + uiincomePanel.GetLabelStr(row));
		vars3.Set<string>("amount", "#" + uiincomePanel.GetValueStr(row));
		vars3.Set<string>("description", "#" + uiincomePanel.GetDescriptionStr(row));
		Vars vars5 = row.vars;
		if (vars5 != null)
		{
			vars5.Set<bool>("is_tooltip", false);
		}
		return Value.Unknown;
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x0012FD40 File Offset: 0x0012DF40
	public static Value PopulateIncomeTooltipHT(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		object obj;
		if (ht == null)
		{
			obj = null;
		}
		else
		{
			IVars vars = ht.vars;
			obj = ((vars != null) ? vars.GetVar("panel", null, true).obj_val : null);
		}
		UIIncomePanel uiincomePanel = obj as UIIncomePanel;
		if (uiincomePanel == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht2 = arg.ht;
		object obj2;
		if (ht2 == null)
		{
			obj2 = null;
		}
		else
		{
			IVars vars2 = ht2.vars;
			obj2 = ((vars2 != null) ? vars2.GetVar("row", null, true).obj_val : null);
		}
		UIIncomePanel.Row row = obj2 as UIIncomePanel.Row;
		if (row == null)
		{
			return Value.Unknown;
		}
		List<Vars> list = new List<Vars>();
		for (int i = row.idx + 1; i < uiincomePanel.rows.Count; i++)
		{
			UIIncomePanel.Row row2 = uiincomePanel.rows[i];
			if (row2.def.Type() != "details" && row2.def.Type() != "row")
			{
				break;
			}
			if (row2.CheckCondition())
			{
				Vars vars3 = row2.vars;
				if (vars3 != null)
				{
					vars3.Set<bool>("is_tooltip", true);
				}
				Vars vars4 = new Vars();
				vars4.Set<string>("label", "#" + uiincomePanel.GetLabelStr(row2));
				vars4.Set<string>("value", "#" + uiincomePanel.GetValueStr(row2));
				list.Add(vars4);
			}
		}
		return new Value(list);
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x0012FEA4 File Offset: 0x0012E0A4
	public static Value SetupVassalTributesTooltip(UIHyperText.CallbackParams arg)
	{
		UIHyperText ht = arg.ht;
		object obj;
		if (ht == null)
		{
			obj = null;
		}
		else
		{
			IVars vars = ht.vars;
			obj = ((vars != null) ? vars.GetVar("panel", null, true).obj_val : null);
		}
		UIIncomePanel uiincomePanel = obj as UIIncomePanel;
		if (uiincomePanel == null)
		{
			return Value.Unknown;
		}
		UIHyperText ht2 = arg.ht;
		object obj2;
		if (ht2 == null)
		{
			obj2 = null;
		}
		else
		{
			IVars vars2 = ht2.vars;
			obj2 = ((vars2 != null) ? vars2.GetVar("row", null, true).obj_val : null);
		}
		UIIncomePanel.Row row = obj2 as UIIncomePanel.Row;
		if (row == null)
		{
			row = uiincomePanel.FetchRowFromParentTooltip(arg);
		}
		if (row == null)
		{
			return Value.Unknown;
		}
		Vars vars3 = row.vars;
		if (vars3 == null)
		{
			return Value.Unknown;
		}
		Logic.Kingdom kingdom = vars3.Get<Logic.Kingdom>("kingdom", null);
		if (kingdom == null)
		{
			return Value.Unknown;
		}
		string text = "";
		for (int i = 0; i < kingdom.vassalStates.Count; i++)
		{
			Logic.Kingdom kingdom2 = kingdom.vassalStates[i];
			text += global::Defs.Localize("@{obj}", kingdom2, null, true, true);
			text += ": ";
			text += Math.Max(0f, kingdom2.taxForSovereign).ToString();
			text += "{gold_icon}";
			if (i < kingdom.vassalStates.Count - 1)
			{
				text += "{p}";
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			vars3.Set<string>("vassals_text", "@" + text);
		}
		return Value.Unknown;
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x0013002C File Offset: 0x0012E22C
	private float GetStatModValue(string location_name, string stat_name, string mod_key)
	{
		IncomeLocation incomeLocation = this.income.FindLocation(location_name);
		Stat stat;
		if (incomeLocation == null)
		{
			stat = null;
		}
		else
		{
			IncomeModifier incomeModifier = incomeLocation.FindMod(stat_name);
			stat = ((incomeModifier != null) ? incomeModifier.stat : null);
		}
		List<Stat.Factor> factors = stat.GetFactors(true);
		int i = 0;
		while (i < factors.Count)
		{
			Stat.Factor factor = factors[i];
			Stat.Modifier mod = factor.mod;
			string a;
			if (mod == null)
			{
				a = null;
			}
			else
			{
				DT.Field field = mod.GetField();
				a = ((field != null) ? field.key : null);
			}
			if (!(a == mod_key))
			{
				Stat stat2 = factor.stat;
				string a2;
				if (stat2 == null)
				{
					a2 = null;
				}
				else
				{
					Stat.Def def = stat2.def;
					if (def == null)
					{
						a2 = null;
					}
					else
					{
						DT.Field field2 = def.field;
						a2 = ((field2 != null) ? field2.key : null);
					}
				}
				if (!(a2 == mod_key))
				{
					i++;
					continue;
				}
			}
			return factor.value;
		}
		return 0f;
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x001300E4 File Offset: 0x0012E2E4
	private void CalcValue(UIIncomePanel.Row row)
	{
		row.flat_val = (row.perc_val = 0f);
		if (this.income == null)
		{
			return;
		}
		if (row.def.value.obj_val is Expression)
		{
			float num = row.def.Float(this.income, 0f);
			if (row.def.FindChild("perc", null, true, true, true, '.') != null)
			{
				row.perc_val = num;
				return;
			}
			row.flat_val = num;
			return;
		}
		else
		{
			if (row.location == null)
			{
				return;
			}
			if (row.mod != null)
			{
				IncomeLocation incomeLocation = this.income.FindLocation(row.location);
				if (incomeLocation == null)
				{
					return;
				}
				IncomeModifier incomeModifier = incomeLocation.FindMod(row.mod);
				if (incomeModifier == null)
				{
					return;
				}
				if (incomeModifier.def.perc)
				{
					row.perc_val = incomeModifier.value;
					row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
					return;
				}
				row.flat_val = incomeModifier.value;
				return;
			}
			else
			{
				string location = row.location;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(location);
				if (num2 <= 2116909549U)
				{
					if (num2 <= 1560861452U)
					{
						if (num2 <= 211118529U)
						{
							if (num2 != 177560250U)
							{
								if (num2 != 211118529U)
								{
									return;
								}
								if (!(location == "ROYAL_TRADE"))
								{
									return;
								}
								Logic.Kingdom kingdom;
								if ((kingdom = (this.income.obj as Logic.Kingdom)) == null)
								{
									return;
								}
								row.flat_val = kingdom.goldFromRoyalMerchants;
								return;
							}
							else
							{
								if (!(location == "TAXES"))
								{
									return;
								}
								IncomeLocation incomeLocation2 = this.income.FindLocation("Kingdom");
								IncomeModifier incomeModifier2 = (incomeLocation2 != null) ? incomeLocation2.FindMod("ks_tax_rate") : null;
								row.perc_val = incomeModifier2.value;
								row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
								return;
							}
						}
						else if (num2 != 246083825U)
						{
							if (num2 != 1560861452U)
							{
								return;
							}
							if (!(location == "TAXES_AUTHORITY"))
							{
								return;
							}
							row.perc_val = this.GetStatModValue("Kingdom", "ks_tax_rate", "from_authority");
							row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
							return;
						}
						else
						{
							if (!(location == "CULTURE"))
							{
								return;
							}
							Logic.Kingdom kingdom2;
							if ((kingdom2 = (this.income.obj as Logic.Kingdom)) == null)
							{
								return;
							}
							row.flat_val = kingdom2.GetStat(Stats.ks_culture, true);
							return;
						}
					}
					else if (num2 <= 1610340318U)
					{
						if (num2 != 1591503363U)
						{
							if (num2 != 1610340318U)
							{
								return;
							}
							if (!(location == "TAXES_TRADITIONS"))
							{
								return;
							}
							row.perc_val = this.GetStatModValue("Kingdom", "ks_tax_rate", "from_traditions");
							row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
							return;
						}
						else if (!(location == "SPY_WAGES"))
						{
							return;
						}
					}
					else if (num2 != 1731086675U)
					{
						if (num2 != 2100829181U)
						{
							if (num2 != 2116909549U)
							{
								return;
							}
							if (!(location == "SUBTOTAL"))
							{
								return;
							}
							for (int i = row.idx + 1; i < this.rows.Count; i++)
							{
								UIIncomePanel.Row row2 = this.rows[i];
								if (row2.type != "row" && row2.type != "details")
								{
									break;
								}
								row.flat_val += row2.flat_val;
							}
							return;
						}
						else
						{
							if (!(location == "NON_ROYAL_TRADE"))
							{
								return;
							}
							Logic.Kingdom kingdom3;
							if ((kingdom3 = (this.income.obj as Logic.Kingdom)) == null)
							{
								return;
							}
							row.flat_val = kingdom3.goldFromMerchants;
							return;
						}
					}
					else if (!(location == "MARSHAL_WAGES"))
					{
						return;
					}
				}
				else
				{
					if (num2 <= 3380581445U)
					{
						if (num2 <= 2746097507U)
						{
							if (num2 != 2487727266U)
							{
								if (num2 != 2746097507U)
								{
									return;
								}
								if (!(location == "CLERIC_WAGES"))
								{
									return;
								}
								goto IL_7C1;
							}
							else if (!(location == "PROVINCES"))
							{
								return;
							}
						}
						else if (num2 != 2858012893U)
						{
							if (num2 != 3095336578U)
							{
								if (num2 != 3380581445U)
								{
									return;
								}
								if (!(location == "TAXES_SKILLS"))
								{
									return;
								}
								row.perc_val = this.GetStatModValue("Kingdom", "ks_tax_rate", "from_skills");
								row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
								return;
							}
							else
							{
								if (!(location == "TOTAL_PERC"))
								{
									return;
								}
								IncomeLocation incomeLocation3 = this.income.FindLocation("Kingdom");
								IncomeModifier incomeModifier3 = (incomeLocation3 != null) ? incomeLocation3.FindMod("ks_tax_rate") : null;
								row.perc_val = this.income.value.perc_value;
								if (incomeModifier3 != null)
								{
									row.perc_val -= incomeModifier3.value;
								}
								row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
								return;
							}
						}
						else
						{
							if (!(location == "TOTAL"))
							{
								return;
							}
							row.flat_val = this.income.value.untaxed_value;
							return;
						}
					}
					else if (num2 <= 3833330305U)
					{
						if (num2 != 3390308793U)
						{
							if (num2 != 3833330305U)
							{
								return;
							}
							if (!(location == "NON_ROYAL_LANDS"))
							{
								return;
							}
						}
						else
						{
							if (!(location == "DIPLOMAT_WAGES"))
							{
								return;
							}
							goto IL_7C1;
						}
					}
					else if (num2 != 3847846085U)
					{
						if (num2 != 3965232792U)
						{
							if (num2 != 4234990469U)
							{
								return;
							}
							if (!(location == "TAXES_RATE"))
							{
								return;
							}
							IncomeLocation incomeLocation4 = this.income.FindLocation("Kingdom");
							Stat stat;
							if (incomeLocation4 == null)
							{
								stat = null;
							}
							else
							{
								IncomeModifier incomeModifier4 = incomeLocation4.FindMod("ks_tax_rate");
								stat = ((incomeModifier4 != null) ? incomeModifier4.stat : null);
							}
							Stat stat2 = stat;
							if (stat2 != null)
							{
								row.perc_val = stat2.base_value;
							}
							else
							{
								row.perc_val = 0f;
							}
							row.flat_val = (this.income.value.base_value + this.income.value.flat_value) * row.perc_val * 0.01f;
							return;
						}
						else
						{
							if (!(location == "INFLUENCE"))
							{
								return;
							}
							Logic.Kingdom kingdom4;
							if ((kingdom4 = (this.income.obj as Logic.Kingdom)) == null)
							{
								return;
							}
							row.flat_val = kingdom4.GetStat(Stats.ks_influence, true);
							return;
						}
					}
					else
					{
						if (!(location == "ROYAL_LANDS"))
						{
							return;
						}
						if (this.income.children == null)
						{
							return;
						}
						for (int j = 0; j < this.income.children.Count; j++)
						{
							IncomePerResource incomePerResource = this.income.children[j];
							Logic.Realm realm;
							if ((realm = (incomePerResource.obj as Logic.Realm)) != null)
							{
								Castle castle = realm.castle;
								if (((castle != null) ? castle.governor : null) != null)
								{
									row.flat_val += incomePerResource.value.untaxed_value;
								}
							}
						}
						return;
					}
					if (this.income.children == null)
					{
						return;
					}
					for (int k = 0; k < this.income.children.Count; k++)
					{
						IncomePerResource incomePerResource2 = this.income.children[k];
						Logic.Realm realm2;
						if ((realm2 = (incomePerResource2.obj as Logic.Realm)) != null)
						{
							Castle castle2 = realm2.castle;
							if (((castle2 != null) ? castle2.governor : null) == null)
							{
								row.flat_val += incomePerResource2.value.taxed_value;
							}
						}
					}
					return;
				}
				IL_7C1:
				Logic.Kingdom kingdom5;
				if ((kingdom5 = (this.income.obj as Logic.Kingdom)) == null)
				{
					return;
				}
				row.flat_val = kingdom5.GetVar(row.location, null, true).Float(0f);
				return;
			}
		}
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x0013093F File Offset: 0x0012EB3F
	private string GetLabelStr(UIIncomePanel.Row row)
	{
		return global::Defs.Localize(row.def, "label", row.vars, null, true, true);
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x0013095A File Offset: 0x0012EB5A
	private string GetDescriptionStr(UIIncomePanel.Row row)
	{
		return global::Defs.Localize(row.def, "description", row.vars, null, true, true);
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x00130978 File Offset: 0x0012EB78
	private void RefreshLabel(UIIncomePanel.Row row)
	{
		if (row.label == null)
		{
			return;
		}
		string labelStr = this.GetLabelStr(row);
		UIText.SetText(row.label, labelStr);
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x001309A8 File Offset: 0x0012EBA8
	private string GetValueStr(UIIncomePanel.Row row)
	{
		return global::Defs.LocalizedNumber(DT.FloatToStr(row.flat_val, -1), null, row.value_form);
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x001309C8 File Offset: 0x0012EBC8
	private void RefreshValue(UIIncomePanel.Row row)
	{
		if (row.value == null)
		{
			return;
		}
		string text;
		if (row.flat_val == 0f && row.show_dash_if_zero)
		{
			text = "--";
		}
		else
		{
			text = this.GetValueStr(row);
		}
		UIText.SetText(row.value, text);
		if (row.value_formating == null)
		{
			UIText.SetText(row.value, text);
			return;
		}
		row.vars.Set<string>("value", "#" + text);
		UIText.SetText(row.value, global::Defs.Localize(row.value_formating, row.vars, null, true, true));
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x00130A64 File Offset: 0x0012EC64
	private void OnEnable()
	{
		this.Load(this.def_id);
	}

	// Token: 0x04001666 RID: 5734
	public string def_id;

	// Token: 0x04001667 RID: 5735
	private DT.Field def;

	// Token: 0x04001668 RID: 5736
	private IncomePerResource income;

	// Token: 0x04001669 RID: 5737
	private bool initialized;

	// Token: 0x0400166A RID: 5738
	private GameObject prefab_total;

	// Token: 0x0400166B RID: 5739
	private GameObject prefab_subtotal;

	// Token: 0x0400166C RID: 5740
	private GameObject prefab_row;

	// Token: 0x0400166D RID: 5741
	private GameObject prefab_separator;

	// Token: 0x0400166E RID: 5742
	private List<UIIncomePanel.Row> rows = new List<UIIncomePanel.Row>();

	// Token: 0x02000776 RID: 1910
	private class Row
	{
		// Token: 0x06004C28 RID: 19496 RVA: 0x00228B10 File Offset: 0x00226D10
		public void SetVars(IncomePerResource income)
		{
			this.vars.obj = new Value((income != null) ? income.obj : null);
			this.vars.Set<float>("val", this.flat_val);
			float num = Mathf.Round(this.perc_val);
			if (num != 0f)
			{
				this.vars.Set<float>("perc", num);
				return;
			}
			this.vars.Set<Value>("perc", Value.Null);
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x00228B8A File Offset: 0x00226D8A
		public bool CheckCondition()
		{
			return this.condition == null || this.condition.Bool(this.vars, false);
		}

		// Token: 0x04003AB7 RID: 15031
		public DT.Field def;

		// Token: 0x04003AB8 RID: 15032
		public int idx;

		// Token: 0x04003AB9 RID: 15033
		public string type;

		// Token: 0x04003ABA RID: 15034
		public DT.Field condition;

		// Token: 0x04003ABB RID: 15035
		public DT.Field tooltip_setup;

		// Token: 0x04003ABC RID: 15036
		public string location;

		// Token: 0x04003ABD RID: 15037
		public string mod;

		// Token: 0x04003ABE RID: 15038
		public string value_form;

		// Token: 0x04003ABF RID: 15039
		public DT.Field value_formating;

		// Token: 0x04003AC0 RID: 15040
		public GameObject go;

		// Token: 0x04003AC1 RID: 15041
		public TextMeshProUGUI label;

		// Token: 0x04003AC2 RID: 15042
		public TextMeshProUGUI value;

		// Token: 0x04003AC3 RID: 15043
		public float flat_val;

		// Token: 0x04003AC4 RID: 15044
		public float perc_val;

		// Token: 0x04003AC5 RID: 15045
		public Vars vars = new Vars();

		// Token: 0x04003AC6 RID: 15046
		public bool show_dash_if_zero;
	}
}
