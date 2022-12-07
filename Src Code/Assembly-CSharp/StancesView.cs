using System;
using System.Collections.Generic;
using Logic;
using Logic.ExtensionMethods;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class StancesView : PoliticalView
{
	// Token: 0x06000BD9 RID: 3033 RVA: 0x00085320 File Offset: 0x00083520
	public Color GetColor(string key)
	{
		Color result;
		if (this.colors.TryGetValue(key, out result))
		{
			return result;
		}
		return this.color_none;
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00085348 File Offset: 0x00083548
	private void LoadColor(DT.Field f)
	{
		if (string.IsNullOrEmpty(f.key))
		{
			return;
		}
		Color color = global::Defs.GetColor(f, this.color_none, null);
		this.colors.Add(f.key, color);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00085384 File Offset: 0x00083584
	public override void LoadDef(DT.Field field)
	{
		base.LoadDef(field);
		this.colors.Clear();
		DT.Field field2 = field.FindChild("colors", null, true, true, true, '.');
		if (field2 != null && field2.children != null)
		{
			for (int i = 0; i < field2.children.Count; i++)
			{
				DT.Field f = field2.children[i];
				this.LoadColor(f);
			}
		}
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x000853EC File Offset: 0x000835EC
	public void GetColorKeys(Logic.Kingdom k1, Logic.Kingdom k2, out string r, out string g, out string b)
	{
		string text;
		if (k1 == k2)
		{
			b = (text = "Own");
			g = (text = text);
			r = text;
			return;
		}
		War war = k1.FindWarWith(k2);
		if (war == null)
		{
			KingdomAndKingdomRelation kingdomAndKingdomRelation = KingdomAndKingdomRelation.Get(k1, k2, true, false);
			string text2;
			if (k1.sovereignState == k2)
			{
				text2 = "Sovereign";
			}
			else if (k2.sovereignState == k1)
			{
				text2 = "Vassal";
			}
			else if (k1.HasPactsWith(k2, Pact.Type.Defensive) || kingdomAndKingdomRelation.stance.IsNonAgression())
			{
				text2 = "NonAggression";
			}
			else if (kingdomAndKingdomRelation.stance.IsTrade())
			{
				text2 = "Trade";
			}
			else
			{
				text2 = "Peace";
			}
			b = (text = text2);
			g = (text = text);
			r = text;
			if (Pact.Find(Pact.Type.Defensive, k2, k1) != null)
			{
				r = "DefensivePactAgainst";
			}
			if (k1.IsAlly(k2))
			{
				b = (text = "Alliance");
				r = text;
			}
			if (kingdomAndKingdomRelation.stance.IsMarriage())
			{
				b = "Marriage";
			}
			return;
		}
		bool flag = war.IsLeader(k1);
		bool flag2 = war.IsLeader(k2);
		if (flag && flag2)
		{
			b = (text = "War");
			g = (text = text);
			r = text;
			return;
		}
		if (flag && !flag2)
		{
			b = (text = "War");
			r = text;
			g = "Peace";
			return;
		}
		if (!flag && flag2)
		{
			b = (text = "War2");
			g = (text = text);
			r = text;
			return;
		}
		b = (text = "War2");
		r = text;
		g = "Peace";
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x0008554C File Offset: 0x0008374C
	public override void OnApply(bool secondary)
	{
		base.OnApply(secondary);
		for (int i = 1; i <= this.realms.Count; i++)
		{
			Logic.Realm logic = this.realms[i - 1].logic;
			Logic.Kingdom kingdom = (logic != null) ? logic.GetKingdom() : null;
			Logic.Realm selecteRelam = base.GetSelecteRelam();
			Logic.Kingdom kingdom2 = (this.kSrc != null) ? this.kSrc.logic : ((selecteRelam != null) ? selecteRelam.GetKingdom() : null);
			if (kingdom == null || kingdom2 == null)
			{
				this.redChannel[i] = (this.greenChannel[i] = (this.blueChannel[i] = this.color_none));
			}
			else
			{
				string key;
				string key2;
				string key3;
				this.GetColorKeys(kingdom2, kingdom, out key, out key2, out key3);
				this.redChannel[i] = this.GetColor(key);
				this.greenChannel[i] = this.GetColor(key2);
				this.blueChannel[i] = this.GetColor(key3);
			}
		}
	}

	// Token: 0x04000944 RID: 2372
	public Dictionary<string, Color> colors = new Dictionary<string, Color>();
}
