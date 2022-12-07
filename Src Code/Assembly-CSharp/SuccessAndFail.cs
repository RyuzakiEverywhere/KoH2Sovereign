using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class SuccessAndFail
{
	// Token: 0x06001518 RID: 5400 RVA: 0x000D578C File Offset: 0x000D398C
	public static void Init()
	{
		Logic.SuccessAndFail.get_factors_texts = new Logic.SuccessAndFail.GetFactorsText(global::SuccessAndFail.GetFactorsText);
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x000D57A0 File Offset: 0x000D39A0
	public static string GetFactorName(Logic.SuccessAndFail sf, Logic.SuccessAndFail.Factor factor)
	{
		string text = global::Defs.Localize(factor.def.field.Path(false, false, '.') + ".name", sf.vars, null, false, true);
		if (text != null)
		{
			return "#" + text;
		}
		text = global::Defs.Localize("SuccessAndFail." + factor.def.field.key + ".name", sf.vars, null, false, true);
		if (text != null)
		{
			return "#" + text;
		}
		return "#{" + factor.def.field.key + "}";
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x000D5848 File Offset: 0x000D3A48
	private static string GetFactorText(Logic.SuccessAndFail sf, Logic.SuccessAndFail.Factor factor, Vars vars)
	{
		string text;
		if (factor.value > 0)
		{
			text = "positive";
		}
		else if (factor.value < 0)
		{
			text = "negative";
		}
		else
		{
			text = "zero";
		}
		if (sf.def.field.Type() == "penalty")
		{
			text += "_penalty";
		}
		if (factor.def.perc)
		{
			text += "_perc";
		}
		text = "SuccessAndFail." + text;
		vars.Set<Logic.SuccessAndFail.Factor>("factor", factor);
		vars.Set<string>("name", global::SuccessAndFail.GetFactorName(sf, factor));
		vars.Set<int>("value", factor.value);
		return global::Defs.Localize(text, vars, null, true, true);
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x000D5904 File Offset: 0x000D3B04
	private static void AddFactorsText(StringBuilder sb, Logic.SuccessAndFail sf, List<Logic.SuccessAndFail.Factor> factors, Vars vars)
	{
		if (factors == null)
		{
			return;
		}
		for (int i = 0; i < factors.Count; i++)
		{
			Logic.SuccessAndFail.Factor factor = factors[i];
			string factorText = global::SuccessAndFail.GetFactorText(sf, factor, vars);
			sb.AppendLine(factorText);
		}
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x000D5940 File Offset: 0x000D3B40
	public static string GetFactorsText(Logic.SuccessAndFail sf)
	{
		Vars vars = new Vars(sf.vars);
		StringBuilder stringBuilder = new StringBuilder();
		global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.success_factors, vars);
		global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.success_perc_factors, vars);
		if (UICommon.GetKey(KeyCode.RightAlt, false) && Game.CheckCheatLevel(Game.CheatLevel.Medium, "see all success/fail factors", true))
		{
			global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.no_factors, vars);
			global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.no_perc_factors, vars);
		}
		global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.fail_factors, vars);
		global::SuccessAndFail.AddFactorsText(stringBuilder, sf, sf.fail_perc_factors, vars);
		return stringBuilder.ToString();
	}
}
