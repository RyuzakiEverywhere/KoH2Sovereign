using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020001F9 RID: 505
public static class CourtAdvice
{
	// Token: 0x06001EBC RID: 7868 RVA: 0x0011CD1C File Offset: 0x0011AF1C
	private static void AddAdvices(WeightedRandom<CourtAdvice.Advice> rnd, ProsAndCons pc, List<ProsAndCons.Factor> factors, Vars txt_vars)
	{
		for (int i = 0; i < factors.Count; i++)
		{
			ProsAndCons.Factor factor = factors[i];
			if (factor.value > 0)
			{
				bool flag = pc.pros != null && pc.pros.Contains(factor);
				string text = flag ? "reason_to_accept" : "reason_to_decline";
				DT.Field field = global::Defs.FindTextField("ProsAndCons.factors." + factor.def.field.key + "." + text);
				if (field == null)
				{
					field = global::Defs.FindTextField("ProsAndCons.generic_texts." + text);
				}
				if (field != null)
				{
					CourtAdvice.Advice val = new CourtAdvice.Advice(flag, factor, field);
					if (factor.def.field.key == "pc_base_pros" || factor.def.field.key == "pc_base_cons")
					{
						rnd.AddOption(val, 5f);
					}
					else
					{
						rnd.AddOption(val, (float)factor.value);
					}
				}
			}
		}
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x0011CE20 File Offset: 0x0011B020
	private static Logic.Character PickAdvisor(List<Logic.Character> characters, CourtAdvice.Advice advise)
	{
		if (characters == null || characters.Count == 0)
		{
			return null;
		}
		Logic.Character character;
		if (advise.advise_by_field != null)
		{
			int num = advise.advise_by_field.NumValues();
			for (int i = 0; i < num; i++)
			{
				string text = advise.advise_by_field.String(i, null, "");
				if (!string.IsNullOrEmpty(text))
				{
					for (int j = 0; j < characters.Count; j++)
					{
						character = characters[j];
						if (!(((character != null) ? character.class_name : null) != text))
						{
							characters.RemoveAt(j);
							return character;
						}
					}
				}
			}
		}
		int index = Random.Range(0, characters.Count);
		character = characters[index];
		characters.RemoveAt(index);
		return character;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x0011CED1 File Offset: 0x0011B0D1
	private static bool ShouldGiveAdvice(Logic.Character c)
	{
		return !c.IsKing() && !c.IsDead() && !c.IsRebel();
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x0011CEF0 File Offset: 0x0011B0F0
	public static void RefreshAdvice(Offer offer)
	{
		if (offer == null)
		{
			return;
		}
		bool flag = CourtAdvice.advices.ContainsKey(offer);
		Logic.Kingdom kingdom = offer.to as Logic.Kingdom;
		if (((kingdom != null) ? kingdom.court : null) == null)
		{
			return;
		}
		if (!flag)
		{
			CourtAdvice.advices.Add(offer, new Dictionary<Logic.Character, CourtAdvice.Advice>());
		}
		Dictionary<Logic.Character, CourtAdvice.Advice> dictionary = CourtAdvice.advices[offer];
		CourtAdvice.tmp_characters.Clear();
		for (int i = 0; i < kingdom.court.Count; i++)
		{
			Logic.Character character = kingdom.court[i];
			if (character != null && CourtAdvice.ShouldGiveAdvice(character) && !dictionary.ContainsKey(character))
			{
				CourtAdvice.tmp_characters.Add(character);
			}
		}
		if (CourtAdvice.tmp_characters.Count == 0)
		{
			return;
		}
		ProsAndCons prosAndCons;
		if (offer.def.id == "CounterOffer" || offer.def.id == "SweetenOffer")
		{
			prosAndCons = ProsAndCons.Get(offer.args[0].obj_val as Offer, "accept", offer.def.id == "CounterOffer");
		}
		else
		{
			prosAndCons = ProsAndCons.Get(offer, "accept", false);
		}
		if (prosAndCons == null)
		{
			return;
		}
		Logic.Kingdom val = offer.from as Logic.Kingdom;
		CourtAdvice.tmp_text_vars.Set<Logic.Kingdom>("plr_kingdom", kingdom);
		CourtAdvice.tmp_text_vars.Set<Logic.Kingdom>("kingdom", val);
		WeightedRandom<CourtAdvice.Advice> temp = WeightedRandom<CourtAdvice.Advice>.GetTemp(32);
		float @float = global::Defs.GetFloat("CharacterTooltip", "advise_decline_bellow", null, -10000f);
		float float2 = global::Defs.GetFloat("CharacterTooltip", "advise_accept_above", null, 10000f);
		if (prosAndCons.pros != null && prosAndCons.eval > @float)
		{
			CourtAdvice.AddAdvices(temp, prosAndCons, prosAndCons.pros, CourtAdvice.tmp_text_vars);
		}
		if (prosAndCons.cons != null && prosAndCons.eval < float2)
		{
			CourtAdvice.AddAdvices(temp, prosAndCons, prosAndCons.cons, CourtAdvice.tmp_text_vars);
		}
		if (temp.options.Count < 1)
		{
			Game.Log(string.Format("No advices available for offer {0}", offer), Game.LogType.Warning);
			return;
		}
		int count = CourtAdvice.tmp_characters.Count;
		for (int j = 0; j < count; j++)
		{
			CourtAdvice.Advice advice = temp.Choose(default(CourtAdvice.Advice), false);
			Logic.Character character2 = CourtAdvice.PickAdvisor(CourtAdvice.tmp_characters, advice);
			if (character2 != null)
			{
				advice.reason_text = "#" + global::Defs.Localize(advice.reason_text_field, CourtAdvice.tmp_text_vars, null, true, true);
				CourtAdvice.advices[offer].Add(character2, advice);
			}
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x0011D170 File Offset: 0x0011B370
	public static void ShowAdviceForOffer(Offer offer)
	{
		foreach (KeyValuePair<Logic.Character, CourtAdvice.Advice> keyValuePair in CourtAdvice.advices[offer])
		{
			if (!CourtAdvice.ShouldGiveAdvice(keyValuePair.Key))
			{
				keyValuePair.Key.NotifyListeners("offer_advice", null);
			}
			else
			{
				keyValuePair.Key.NotifyListeners("offer_advice", keyValuePair.Value.accept);
			}
		}
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x0011D208 File Offset: 0x0011B408
	public static void HideAdvice()
	{
		for (int i = 1; i < BaseUI.LogicKingdom().court.Count; i++)
		{
			Logic.Character character = BaseUI.LogicKingdom().court[i];
			if (character != null)
			{
				character.NotifyListeners("offer_advice", null);
			}
		}
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x0011D250 File Offset: 0x0011B450
	private static void SetAdviceVarsImpl(Logic.Character c, Vars vars)
	{
		if (CourtAdvice.activeOffer == null)
		{
			return;
		}
		if (!CourtAdvice.advices.ContainsKey(CourtAdvice.activeOffer))
		{
			return;
		}
		CourtAdvice.Advice advice;
		if (!CourtAdvice.advices[CourtAdvice.activeOffer].TryGetValue(c, out advice))
		{
			return;
		}
		vars.Set<string>("advise_text", advice.key);
		if (advice.reason_text_field != null)
		{
			vars.Set<string>("advise_reason", advice.reason_text);
		}
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x0011D2BC File Offset: 0x0011B4BC
	public static void SetAdviceVars(Logic.Character c, Vars vars)
	{
		CourtAdvice.SetAdviceVarsImpl(c, vars);
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x0011D2C8 File Offset: 0x0011B4C8
	public static void Update()
	{
		CourtAdvice.previousOffer = CourtAdvice.activeOffer;
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null || kingdom.court == null)
		{
			return;
		}
		Offers component = kingdom.GetComponent<Offers>();
		if (((component != null) ? component.incoming : null) == null)
		{
			CourtAdvice.HideAdvice();
			return;
		}
		for (int i = 0; i < component.incoming.Count; i++)
		{
			CourtAdvice.RefreshAdvice(component.incoming[i]);
		}
		CourtAdvice.tmp_offers.AddRange(CourtAdvice.advices.Keys);
		foreach (Offer offer in CourtAdvice.tmp_offers)
		{
			if (!component.incoming.Contains(offer))
			{
				CourtAdvice.advices.Remove(offer);
			}
		}
		CourtAdvice.tmp_offers.Clear();
		BaseUI baseUI = BaseUI.Get();
		List<UIWindow> list;
		if (baseUI == null)
		{
			list = null;
		}
		else
		{
			UIWindowDispatcher window_dispatcher = baseUI.window_dispatcher;
			list = ((window_dispatcher != null) ? window_dispatcher.active_windows_stack : null);
		}
		List<UIWindow> list2 = list;
		if (list2 != null && list2.Count > 0)
		{
			int j = list2.Count - 1;
			while (j >= 0)
			{
				UIWindow uiwindow = list2[j];
				if (uiwindow == null)
				{
					goto IL_157;
				}
				AudienceWindow audienceWindow;
				if ((audienceWindow = (uiwindow as AudienceWindow)) == null)
				{
					MessageWnd messageWnd;
					if ((messageWnd = (uiwindow as MessageWnd)) == null)
					{
						goto IL_157;
					}
					UIDiplomacyOfferMessage component2 = messageWnd.GetComponent<UIDiplomacyOfferMessage>();
					CourtAdvice.activeOffer = ((component2 != null) ? component2.offer : null);
				}
				else
				{
					CourtAdvice.activeOffer = audienceWindow.proposed_offer;
				}
				IL_15D:
				if (CourtAdvice.activeOffer == null)
				{
					CourtAdvice.HideAdvice();
					j--;
					continue;
				}
				Dictionary<Logic.Character, CourtAdvice.Advice> dictionary;
				if (!CourtAdvice.advices.TryGetValue(CourtAdvice.activeOffer, out dictionary))
				{
					CourtAdvice.HideAdvice();
					return;
				}
				CourtAdvice.ShowAdviceForOffer(CourtAdvice.activeOffer);
				return;
				IL_157:
				CourtAdvice.activeOffer = null;
				goto IL_15D;
			}
			return;
		}
		CourtAdvice.HideAdvice();
	}

	// Token: 0x0400141E RID: 5150
	private static Dictionary<Offer, Dictionary<Logic.Character, CourtAdvice.Advice>> advices = new Dictionary<Offer, Dictionary<Logic.Character, CourtAdvice.Advice>>();

	// Token: 0x0400141F RID: 5151
	private static Offer activeOffer = null;

	// Token: 0x04001420 RID: 5152
	private static Offer previousOffer = null;

	// Token: 0x04001421 RID: 5153
	private static List<Logic.Character> tmp_characters = new List<Logic.Character>();

	// Token: 0x04001422 RID: 5154
	private static Vars tmp_text_vars = new Vars();

	// Token: 0x04001423 RID: 5155
	private static List<Offer> tmp_offers = new List<Offer>();

	// Token: 0x02000736 RID: 1846
	private struct Advice
	{
		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06004A23 RID: 18979 RVA: 0x0021F6B8 File Offset: 0x0021D8B8
		public string key
		{
			get
			{
				if (!this.accept)
				{
					return "CharacterTooltip.advise_decline";
				}
				return "CharacterTooltip.advise_accept";
			}
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x0021F6CD File Offset: 0x0021D8CD
		public Advice(bool accept, ProsAndCons.Factor factor, DT.Field reason_text_field)
		{
			this.accept = accept;
			this.factor = factor;
			this.reason_text_field = reason_text_field;
			this.reason_text = null;
			this.advise_by_field = factor.def.field.FindChild("advise_by", null, true, true, true, '.');
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x0021F70C File Offset: 0x0021D90C
		public override string ToString()
		{
			return (this.accept ? "accept" : "decline") + " (" + this.factor.def.field.key + ")";
		}

		// Token: 0x040038FB RID: 14587
		public bool accept;

		// Token: 0x040038FC RID: 14588
		public ProsAndCons.Factor factor;

		// Token: 0x040038FD RID: 14589
		public DT.Field reason_text_field;

		// Token: 0x040038FE RID: 14590
		public DT.Field advise_by_field;

		// Token: 0x040038FF RID: 14591
		public string reason_text;
	}
}
