using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000225 RID: 549
public class UIKingdomRankingCategory : MonoBehaviour
{
	// Token: 0x0600215E RID: 8542 RVA: 0x0012EB5E File Offset: 0x0012CD5E
	public void SetObject(KingdomRankingCategory cat)
	{
		this.Data = cat;
		UICommon.FindComponents(this, false);
		this.Init();
		this.Refresh();
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x0012EB7C File Offset: 0x0012CD7C
	private void Init()
	{
		UIText.SetText(this.m_Name, this.Data.def.field, "name", null, null);
		UIText.SetText(this.m_Description, this.Data.def.field, "description", null, null);
		if (!this.m_DisableTooltip)
		{
			Tooltip.Get(this.m_TooltipHotSpot ?? base.gameObject, true).SetDef("KingdomRankingCategoryTooltip", new Vars(this.Data));
		}
		if (this.m_Illustration != null)
		{
			this.m_Illustration.overrideSprite = global::Defs.GetObj<Sprite>(this.Data.def.field, "illustration", null);
		}
		this.InitRankings();
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x0012EC40 File Offset: 0x0012CE40
	private void InitRankings()
	{
		this.m_Rankings.Clear();
		if (this.m_RankingsContainer == null)
		{
			return;
		}
		int i = this.m_RankingsContainer.transform.childCount;
		if (this.m_RankingPrototype == null)
		{
			if (i < 1)
			{
				return;
			}
			this.m_RankingPrototype = this.m_RankingsContainer.transform.GetChild(0).gameObject;
			this.m_RankingPrototype.SetActive(false);
		}
		while (i > 1)
		{
			UnityEngine.Object.DestroyImmediate(this.m_RankingsContainer.transform.GetChild(--i).gameObject);
		}
		List<string> rankings = this.Data.def.rankings;
		if (rankings == null)
		{
			return;
		}
		for (int j = 0; j < rankings.Count; j++)
		{
			GameObject gameObject = global::Common.Spawn(this.m_RankingPrototype, false, false);
			gameObject.transform.SetParent(this.m_RankingsContainer.transform, false);
			gameObject.SetActive(true);
			this.m_Rankings.Add(gameObject);
		}
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x0012ED36 File Offset: 0x0012CF36
	public void Refresh()
	{
		this.RefreshRankings();
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x0012ED40 File Offset: 0x0012CF40
	public void RefreshRankings()
	{
		List<string> rankings = this.Data.def.rankings;
		if (rankings == null)
		{
			return;
		}
		if (this.m_Rankings.Count != rankings.Count)
		{
			return;
		}
		for (int i = 0; i < rankings.Count; i++)
		{
			string name = rankings[i];
			KingdomRankings rankings2 = this.Data.game.rankings;
			KingdomRanking ranking = (rankings2 != null) ? rankings2.Find(name) : null;
			this.RefreshRanking(this.m_Rankings[i], ranking);
		}
		if (this.m_Power != null)
		{
			int score = this.Data.GetScore();
			string text = (score <= 0) ? "0" : score.ToString();
			UIText.SetText(this.m_Power, text);
		}
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x0012EE00 File Offset: 0x0012D000
	private void RefreshRanking(GameObject go, KingdomRanking ranking)
	{
		BSGButton component = go.GetComponent<BSGButton>();
		if (component != null && component.onClick == null)
		{
			component.onClick = delegate(BSGButton b)
			{
				UIKingdomRankingsWindow.Create(ranking, null);
			};
		}
		int rank = ranking.GetRank(this.Data.kingdom);
		int fame = ranking.GetFame(this.Data.kingdom);
		Vars vars = new Vars(ranking);
		vars.Set<Logic.Kingdom>("kingdom", this.Data.kingdom);
		vars.Set<int>("rank", rank);
		vars.Set<int>("fame", fame);
		vars.Set<int>("ranks", ranking.rows.Count);
		string text = global::Defs.Localize(ranking.def.field, "name", vars, null, true, true);
		string txt = "#" + global::Defs.Localize(ranking.def.field, "tooltip", vars, null, true, true);
		Tooltip.Get(go, true).SetText(txt, "#" + text, null);
		Image image = global::Common.FindChildComponent<Image>(go, "id_Icon");
		if (image != null)
		{
			image.overrideSprite = global::Defs.GetObj<Sprite>(ranking.def.field, "icon", null);
		}
		TextMeshProUGUI textMeshProUGUI = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_Rank");
		if (textMeshProUGUI != null)
		{
			UIText.SetTextKey(textMeshProUGUI, "KingdomAdvantage.rank", vars, null);
		}
		TextMeshProUGUI textMeshProUGUI2 = global::Common.FindChildComponent<TextMeshProUGUI>(go, "id_Fame");
		if (textMeshProUGUI2 != null)
		{
			textMeshProUGUI2.text = fame.ToString();
		}
		TextMeshProUGUI textMeshProUGUI3 = global::Common.FindChildComponent<TextMeshProUGUI>(go, "NameRank");
		if (textMeshProUGUI3 != null)
		{
			UIText.SetText(textMeshProUGUI3, text);
		}
		Image image2 = global::Common.FindChildComponent<Image>(go, "id_RankingIllustration");
		if (image2 != null)
		{
			image2.overrideSprite = global::Defs.GetObj<Sprite>(ranking.def.field, "illustration", null);
		}
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x0012F018 File Offset: 0x0012D218
	public void DisableTooltip(bool disabled)
	{
		if (this.m_DisableTooltip == disabled)
		{
			return;
		}
		this.m_DisableTooltip = disabled;
		GameObject obj = this.m_TooltipHotSpot ?? base.gameObject;
		if (this.m_DisableTooltip)
		{
			UnityEngine.Object.Destroy(Tooltip.Get(obj, false));
			return;
		}
		if (this.Data == null)
		{
			UnityEngine.Object.Destroy(Tooltip.Get(obj, false));
			return;
		}
		Tooltip.Get(base.gameObject, true).SetObj(this.Data, null, null);
	}

	// Token: 0x04001652 RID: 5714
	[UIFieldTarget("id_Name")]
	private TextMeshProUGUI m_Name;

	// Token: 0x04001653 RID: 5715
	[UIFieldTarget("id_Description")]
	private TextMeshProUGUI m_Description;

	// Token: 0x04001654 RID: 5716
	[UIFieldTarget("id_Power")]
	private TextMeshProUGUI m_Power;

	// Token: 0x04001655 RID: 5717
	[UIFieldTarget("id_Illustration")]
	private Image m_Illustration;

	// Token: 0x04001656 RID: 5718
	[UIFieldTarget("id_RankingsContainer")]
	private GameObject m_RankingsContainer;

	// Token: 0x04001657 RID: 5719
	private GameObject m_RankingPrototype;

	// Token: 0x04001658 RID: 5720
	private List<GameObject> m_Rankings = new List<GameObject>();

	// Token: 0x04001659 RID: 5721
	[UIFieldTarget("id_TooltipHotSpot")]
	private GameObject m_TooltipHotSpot;

	// Token: 0x0400165A RID: 5722
	public KingdomRankingCategory Data;

	// Token: 0x0400165B RID: 5723
	private bool m_DisableTooltip;
}
